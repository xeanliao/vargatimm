using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Data.Linq;
using System.Threading;
using System.Diagnostics;
using FileHelpers;

namespace GPS.Tool.Mapping
{
    class BlockGroupMappingMaker : MappingMaker
    {
        AreaDataContext _dataContext;
        List<CsvAreaRecord> _records;

        private string LogFilePath {
            get {
                return System.Configuration.ConfigurationManager.AppSettings["BlockGroupMappingLogFile"];
            }
        }

        private string TractFiveZipLogFilePath {
            get {
                return System.Configuration.ConfigurationManager.AppSettings["TractFiveZipLogFile"];
            }
        }

        protected override void MakeMapping()
        {
            int start = int.Parse(System.Configuration.ConfigurationManager.AppSettings["BlockGroupMappingStart"]);
            int end = int.Parse(System.Configuration.ConfigurationManager.AppSettings["BlockGroupMappingEnd"]);

            this._dataContext = new AreaDataContext();
            this._dataContext.Tracts.Where(t => t.Id == t.Id);
            this._dataContext.FiveZipAreas.Where(z => z.Id == z.Id);

            this._records = new List<CsvAreaRecord>();

            SendStatus(true, false);

            int count = GetTractCount();
            if (0 == end) {
                end = count;
            }

            SendMessage(true, count, 0, "");

            for (int index = start; index * 100 < end; index++) {
                List<Tract> tracts = GetTracts(index * 100, 100);

                foreach (Tract tract in tracts) {
                    Log(new LogRecord() {
                        Time = DateTime.Now,
                        TractId = tract.Id,
                        Start = true
                    });

                    //if (_stopEnabled) break;

                    List<BlockGroup> bgs = GetBlockGroups(tract);
                    List<FiveZipArea> fiveZipAreas = GetFiveZips(tract);

                    bool exist = false;
                    foreach (FiveZipArea fiveZipArea in fiveZipAreas) {
                        if (AreaInPolygon(fiveZipArea, tract)) {
                            foreach (BlockGroup bg in bgs) {
                                InsertItem(fiveZipArea, tract, bg);
                                exist = true;
                            }
                        }
                    }

                    if (!exist) {
                        Log(new TractFiveZipLogRecord() {
                            TractId = tract.Id,
                            TractCode = tract.Code,
                            FiveZipExists = fiveZipAreas.Count > 0
                        });
                    }

                    Log(new LogRecord() {
                        Time = DateTime.Now,
                        TractId = tract.Id,
                        Start = false
                    });
                }
                
                this._dataContext.SubmitChanges();
            }
            
            SendStatus(false, !_stopEnabled);
        }

        private void Log(LogRecord logRecord) {
            List<LogRecord> logRecords = new List<LogRecord>() {
                logRecord
            };
            FileHelperEngine engine = new FileHelperEngine(typeof(LogRecord));
            engine.AppendToFile(this.LogFilePath, logRecords);
        }

        private void Log(TractFiveZipLogRecord logRecord) {
            List<TractFiveZipLogRecord> logRecords = new List<TractFiveZipLogRecord>() {
                logRecord
            };
            FileHelperEngine engine = new FileHelperEngine(typeof(TractFiveZipLogRecord));
            engine.AppendToFile(this.TractFiveZipLogFilePath, logRecords);
        }

        private void SaveToCsv(IEnumerable<CsvAreaRecord> records) {
            List<CsvAreaRecord> data = new List<CsvAreaRecord>(records);
            ParameterizedThreadStart job = new ParameterizedThreadStart(SaveToCsvDelegate);
            Thread t = new Thread(job);
            t.Start(records);
        }

        private static void SaveToCsvDelegate(object records) {
            FileHelperEngine engine = new FileHelperEngine(typeof(CsvAreaRecord));
            engine.AppendToFile(string.Format(@"c:\data\projects\gps\temp\blockgroupmappings_60000\{0}.csv", DateTime.Now.Ticks), 
                             records as IEnumerable<CsvAreaRecord>);
        }

        private void Assert(bool condition, string message) {
            if (!condition) {
                throw new ArgumentException(message);
            }
        }

        private void InsertItem(FiveZipArea fiveZipArea, Tract tract, BlockGroup blockGroup)
        {
            ThreeZipArea threeZipArea = _dataContext.GetThreeZipArea(fiveZipArea);

            Assert(!string.IsNullOrEmpty(blockGroup.Code), string.Format("invalid block group code {0}", blockGroup.Id));
            Assert(!string.IsNullOrEmpty(tract.Code), string.Format("invalid tract code {0}", tract.Id));
            Assert(!string.IsNullOrEmpty(fiveZipArea.Code), string.Format("invalid five zip code {0}", fiveZipArea.Id));
            Assert(!string.IsNullOrEmpty(threeZipArea.Code), string.Format("invalid three zip code {0}", threeZipArea.Id));

            BlockGroupMapping mapping = new BlockGroupMapping();
            mapping.BlockGroupId = blockGroup.Id;
            mapping.BlockGroupCode = blockGroup.Code;
            mapping.TractId = tract.Id;
            mapping.TractCode = tract.Code;
            mapping.FiveZipAreaId = fiveZipArea.Id;
            mapping.FiveZipAreaCode = fiveZipArea.Code;
            mapping.ThreeZipAreaId = threeZipArea.Id;
            mapping.ThreeZipAreaCode = threeZipArea.Code;
            this._dataContext.BlockGroupMappings.InsertOnSubmit(mapping);
        }

        public bool AreaInPolygon(FiveZipArea fiveZipArea, Tract tract)
        {
            List<ICoordinate> fiveZipCoordinates = new List<ICoordinate>(fiveZipArea.FiveZipAreaCoordinates.ToArray());
            List<ICoordinate> tractCoordinates = new List<ICoordinate>(tract.TractCoordinates.ToArray());
            return ShapeMethods.BoxInBox(fiveZipArea, tract) && 
                (ShapeMethods.PolygonInPolygon(fiveZipCoordinates, tractCoordinates) ||
                ShapeMethods.PolygonInPolygon(tractCoordinates, fiveZipCoordinates));
        }

        private void SendMessage(BlockGroup blockGroup)
        {
            SendMessage(false, -1, blockGroup.Id, string.Format("Zip:{0},trk:{1},BG:{2},BG'Id:{3}", "Null", blockGroup.TractCode, blockGroup.Code, blockGroup.Id));
        }
        private void SendMessage(FiveZipArea fiveZipArea, Tract tract, BlockGroup blockGroup)
        {
            SendMessage(true, -1, tract.Id, string.Format("Zip:{0},trk:{1},BG:{2},BG'Id:{3}", fiveZipArea.Code, blockGroup.TractCode, blockGroup.Code, blockGroup.Id));
        }

        private void SendMessage(Tract tract)
        {
            SendMessage(false, -1, tract.Id, string.Format("Zip:{0},trk:{1}", "Null", tract.Code));
        }

        private List<FiveZipArea> GetFiveZips(Tract tract)
        {
            List<int> ids = ShapeMethods.GetBoxIds(tract, 10, 15);
            //AreaDataContext data = new AreaDataContext();
            //return data.GetFiveZipsByBoxIds(ids);
            return _dataContext.GetFiveZipsByBoxIds(ids);
        }

        private List<BlockGroup> GetBlockGroups(Tract tract)
        {
            //AreaDataContext data = new AreaDataContext();
            //return data.GetBlockGroups(tract);
            return _dataContext.GetBlockGroups(tract);
        }

        private int GetTractCount()
        {
            // START FIX
            //AreaDataContext data = new AreaDataContext();
            //return data.Tracts.Count();
            return _dataContext.Tracts.Count();
            // END FIX
        }

        private List<Tract> GetTracts(int skip, int count)
        {
            // START FIX
            //AreaDataContext data = new AreaDataContext();
            //return data.Tracts.Skip(skip).Take(count).ToList();
            return _dataContext.Tracts.OrderBy(t => t.Id).Skip(skip).Take(count).ToList();
            // END FIX
        }

        //private List 
    }

    [DelimitedRecord(",")]
    public class CsvAreaRecord {
        public int ThreeZipAreaId;
        public string ThreeZipAreaCode;
        public int FiveZipAreaId;
        public string FiveZipAreaCode;
        public int TractId;
        public string TractCode;
        public int BlockGroupId;
        public string BlockGroupCode;
    }

    [DelimitedRecord(",")]
    public class LogRecord {
        public DateTime Time;
        public int TractId;
        public bool Start;
        public int FiveZipId;
    }

    [DelimitedRecord(",")]
    public class TractFiveZipLogRecord {
        public int TractId;
        public string TractCode;
        public bool FiveZipExists;
    }

    delegate void Save(IEnumerable<CsvAreaRecord> data);
}
