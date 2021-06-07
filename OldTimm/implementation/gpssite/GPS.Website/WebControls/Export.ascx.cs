using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using GPS.Web;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Area;
using GPS.DomainLayer.Interfaces;
using System.Diagnostics;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using FileHelpers;
using FileHelpers.DataLink;
using GPS.DomainLayer.Area.Export;
using GPS.DataLayer.ValueObjects;
using GPS.Utilities.IO;
using org.in2bits.MyXls;
using GPS.DataLayer;
using System.Collections;
using log4net;


namespace GPS.Website.WebControls
{
    public partial class Export : System.Web.UI.UserControl
    {
        private static ILog m_Logger = LogManager.GetLogger(typeof(Export));
        int cid = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                string data = Request.Form["data"];
                data = FixAddress(data);
                string exportFileFormat = Request.Form["exportFileFormat"];
                if (Request.Form["campaignIdForExport"] != null)
                    cid = int.Parse(Request.Form["campaignIdForExport"]);
                if ("txt" == exportFileFormat)
                    ExportTextZip(data);
                else if ("csv" == exportFileFormat)
                    ExportToCsvZip(data);
                else if ("excel" == exportFileFormat)
                    ExportToExcelZip(data);

            }
        }

        private string FixAddress(string exportString)
        {
            string[] orignalString = exportString.Split('$');
            List<string> addressExportArray = new List<string>();
            if (orignalString.Length > 1)
            {
                string[] address = orignalString[1].Split('#');
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    foreach (var item in address)
                    {
                        List<string> relations = new List<string>();
                        var partString = item.Split('|');
                        if (partString.Length > 0)
                        {
                            string addressIdStr = partString[1];
                            int addressId;
                            if (int.TryParse(addressIdStr, out addressId))
                            {
                                var currentAddress = ws.Repositories.AddressRepository.GetEntity(addressId);
                                foreach (var record in currentAddress.Radiuses[2].RadiusRecords)
                                {
                                    if (((int)record.Classification).ToString() == partString[0])
                                    {
                                        relations.Add(string.Format("{0},{1},true;", partString[0], record.AreaId));
                                    }
                                }
                            }
                            partString[1] = string.Join("", relations);
                        }
                        addressExportArray.Add(string.Join("|", partString));
                    }
                }
            }
            if (addressExportArray.Count > 0)
            {
                orignalString[1] = string.Join("#", addressExportArray);
            }
            return string.Join("#", orignalString);
        }

        /// <summary>
        /// Return the path of the folder under which to place imported files.
        /// </summary>
        private string ImportFilesFolderPath
        {
            get
            {
                return Server.MapPath(@"~/Files/ImportFiles/");
            }
        }

        /// <summary>
        /// Return the path of the excel template file used by the ExcelStorage 
        /// engine to write area records.
        /// </summary>
        private string ImportDataTemplateFilePath
        {
            get
            {
                return Server.MapPath(@"~/Files/ImportFiles/Resources/import_data_template.xlt");
            }
        }

        /// <summary>
        /// Transform form data to ZIP file which contains MS Excel file(s), and 
        /// output it to the browser.
        /// </summary>
        /// <param name="data">The form data posted from the browser.</param>
        private void ExportToExcelZip(string data)
        {
            DataSet ds = GetExportDataSet(data);
            string filePath = WriteAreaDatasetToExcelZip(ds);
            if (!string.IsNullOrEmpty(filePath))
            {
                OutputZipFile(filePath);
            }
        }

        /// <summary>
        /// Transform form data to ZIP file which contains CSV file(s), and 
        /// output it to the browser.
        /// </summary>
        /// <param name="data">The form data posted from the browser.</param>
        private void ExportToCsvZip(string data)
        {
            DataSet ds = GetExportDataSet(data);
            string filePath = WriteAreaDatasetToCsvZip(ds);
            if (!string.IsNullOrEmpty(filePath))
            {
                OutputZipFile(filePath);
            }
        }

        /// <summary>
        /// Write dataset which contains area data to a ZIP file which contains MS Excel file(s),
        /// and return the full path of the generated ZIP file.
        /// </summary>
        /// <param name="ds">The <see cref="DataSet"/> containing area data.</param>
        /// <returns>The full pat of the generated ZIP file if successful, and 
        /// an empty string if failed.</returns>
        private string WriteAreaDatasetToExcelZip(DataSet ds)
        {
            // Create the target folder where generated files will be put
            string folder = string.Format("{0}{1}", this.ImportFilesFolderPath, DateTime.Now.Ticks);
            Directory.CreateDirectory(folder);
            //20120105 combine file
            string fileFormat = "{0}\\{1}.{2}";
            string fullPathCombine = string.Format(fileFormat, folder, "Shapes_Addresses_Combine", "xls");
            ExcelAreaRecord[] areasCombine = TransformTableDataToExcelRecordsCombine(ds);
            WriteExcelRecordsToExcelFileCombine(fullPathCombine, areasCombine);
            // Transform and write dataset data to target folder
            foreach (System.Data.DataTable dt in ds.Tables)
            {
                ExcelAreaRecord[] areas = TransformTableDataToExcelRecords(dt);
                
                string fullPath = ConstructFullFilePath(folder, dt.TableName, "xls");
                WriteExcelRecordsToExcelFile(fullPath, areas);                
            }

            // Create zip file with the target folder and return the file path
            string targetFilePath = ZipFile(folder, "") ? folder + ".zip" : string.Empty;
            return targetFilePath;
        }

        /// <summary>
        /// Write dataset which contains area data to a ZIP file which contains Csv file(s),
        /// and return the full path of the generated ZIP file.
        /// </summary>
        /// <param name="ds">The <see cref="DataSet"/> containing area data.</param>
        /// <returns>The full pat of the generated ZIP file if successful, and 
        /// an empty string if failed.</returns>
        private string WriteAreaDatasetToCsvZip(DataSet ds)
        {
            // Create the target folder where generated files will be put
            string folder = string.Format("{0}{1}", this.ImportFilesFolderPath, DateTime.Now.Ticks);
            Directory.CreateDirectory(folder);
            //20120105 combine file
            string fileFormat = "{0}\\{1}.{2}";
            string fullPathCombine = string.Format(fileFormat, folder, "Shapes_Addresses_Combine", "csv");
            CsvAreaRecord[] areasHead = TransformTableDataToCsvRecordsHead(ds.Tables[0]);
            WriteCsvRecordsToCsvFileCombine(fullPathCombine, areasHead);
            // Transform and write dataset data to target folder
            foreach (System.Data.DataTable dt in ds.Tables)
            {
                CsvAreaRecord[] areas = TransformTableDataToCsvRecords(dt);
                //20120105 combine file
                CsvAreaRecord[] areasCombine = TransformTableDataToCsvRecordsCombine(dt);
                string fullPath = ConstructFullFilePath(folder, dt.TableName, "csv");

                WriteCsvRecordsToCsvFile(fullPath, areas);
                WriteCsvRecordsToCsvFileCombine(fullPathCombine, areasCombine);
            }

            // Create zip file with the target folder and return the file path
            string targetFilePath = ZipFile(folder, "") ? folder + ".zip" : string.Empty;
            return targetFilePath;
        }

        /// <summary>
        /// Construct a full file path based on specified folder, file name, and file extension,
        /// and must ensure the file path constructed does not conflict with existing files
        /// under the same folder.
        /// </summary>
        /// <param name="folder">The folder path</param>
        /// <param name="fileName">The file name with no folder path or file extension</param>
        /// <param name="fileExtension">The file extension, e.g. 'txt', 'csv'</param>
        /// <returns></returns>
        private string ConstructFullFilePath(string folder, string fileName, string fileExtension)
        {
            string fileFormat = "{0}\\{1}.{2}";
            string filePath = string.Format(fileFormat, folder, fileName, fileExtension);
            for (int i = 1; File.Exists(filePath); i++)
            {
                filePath = string.Format(fileFormat, folder, fileName + "_" + i.ToString(), fileExtension);
            }
            return filePath;
        }

        /// <summary>
        /// Write <see cref="CsvAreaRecord"/> array to specified Csv file.
        /// </summary>
        /// <param name="filePath">The Csv file full path.</param>
        /// <param name="areas">The <see cref="CsvAreaRecord"/> array to write.</param>
        private void WriteCsvRecordsToCsvFile(string filePath, CsvAreaRecord[] areas)
        {
            FileHelperEngine engine = new FileHelperEngine(typeof(CsvAreaRecord));
            engine.WriteFile(filePath, areas);
        }

        /// <summary>
        /// Write <see cref="CsvAreaRecord"/> array to specified Csv file.
        /// </summary>
        /// <param name="filePath">The Csv file full path.</param>
        /// <param name="areas">The <see cref="CsvAreaRecord"/> array to write.</param>
        private void WriteCsvRecordsToCsvFileCombine(string filePathCombine, CsvAreaRecord[] areasCombine)
        {
            FileHelperEngine engine = new FileHelperEngine(typeof(CsvAreaRecord));
            engine.AppendToFile(filePathCombine, areasCombine);
        }

        /// <summary>
        /// Write <see cref="ExcelAreaRecord"/> array to specified Excel file.
        /// </summary>
        /// <param name="filePath">The Excel file full path.</param>
        /// <param name="areas">The <see cref="ExcelAreaRecord"/> array to write.</param>
        private void WriteExcelRecordsToExcelFile(string filePath, ExcelAreaRecord[] areas)
        {
            //upgrade by steve.yin to use .net native compoments for export excel file
            #region old code
            //FileOperator oper = new FileOperator();
            //GPS.Utilities.OfficeHelpers.AreaRecord[] records = new GPS.Utilities.OfficeHelpers.AreaRecord[areas.Length];
            //for (int i = 0; i < areas.Length; i++)
            //{
            //    records[i] = new GPS.Utilities.OfficeHelpers.AreaRecord()
            //    {
            //        Code = areas[i].Code,
            //        Total = areas[i].Total,
            //        Penetration = areas[i].Penetration
            //    };
            //}

            //oper.WriteFile<GPS.Utilities.OfficeHelpers.AreaRecord>(filePath, this.ImportDataTemplateFilePath, records);
            #endregion
            XlsDocument doc = new XlsDocument();
            Workbook wbk = doc.Workbook;
            Worksheet sht = wbk.Worksheets.Add("Sheet1");
            for (int i = 0; i < areas.Length; i++)
            {
                sht.Cells.Add(i + 1, 1, areas[i].Code);
                sht.Cells.Add(i + 1, 2, areas[i].Total);
                sht.Cells.Add(i + 1, 3, areas[i].Penetration);
            }
            doc.FileName = Path.GetFileName(filePath);
            doc.Save(Path.GetDirectoryName(filePath), true);
            //oper.ReadFile
            //ExcelStorage engine = new ExcelStorage(typeof(ExcelAreaRecord));
            //engine.FileName = filePath;
            //engine.TemplateFile = this.ImportDataTemplateFilePath;
            //engine.InsertRecords(areas);
        }

        /// <summary>
        /// Write <see cref="ExcelAreaRecord"/> array to specified Excel file.
        /// </summary>
        /// <param name="filePath">The Excel file full path.</param>
        /// <param name="areas">The <see cref="ExcelAreaRecord"/> array to write.</param>
        private void WriteExcelRecordsToExcelFileCombine(string filePath, ExcelAreaRecord[] areas)
        {
            #region old code
            //FileOperator oper = new FileOperator();
            //GPS.Utilities.OfficeHelpers.AreaRecord[] records = new GPS.Utilities.OfficeHelpers.AreaRecord[areas.Length];
            //for (int i = 0; i < areas.Length; i++)
            //{
            //    records[i] = new GPS.Utilities.OfficeHelpers.AreaRecord()
            //    {
            //        Code = areas[i].Code,
            //        Total = areas[i].Total,
            //        Penetration = areas[i].Penetration
            //    };
            //}

            //oper.WriteFile<GPS.Utilities.OfficeHelpers.AreaRecord>(filePath, this.ImportDataTemplateFilePath, records);
            #endregion
            XlsDocument doc = new XlsDocument();
            Workbook wbk = doc.Workbook;
            Worksheet sht = wbk.Worksheets.Add("Sheet1");
            for (int i = 0; i < areas.Length; i++)
            {
                sht.Cells.Add(i + 1, 1, areas[i].Code);
                sht.Cells.Add(i + 1, 2, areas[i].Total);
                sht.Cells.Add(i + 1, 3, areas[i].Penetration);
            }
            doc.FileName = Path.GetFileName(filePath);
            doc.Save(Path.GetDirectoryName(filePath), true);
            //oper.ReadFile
            //ExcelStorage engine = new ExcelStorage(typeof(ExcelAreaRecord));
            //engine.FileName = filePath;
            //engine.TemplateFile = this.ImportDataTemplateFilePath;
            //engine.InsertRecords(areas);
        }

        /// <summary>
        /// Transform datatable data to <see cref="ExcelAreaRecord"/> array.
        /// </summary>
        /// <param name="dt">The datatable containing the area data</param>
        /// <returns>A <see cref="ExcelAreaRecord"/> array.</returns>
        private ExcelAreaRecord[] TransformTableDataToExcelRecords(System.Data.DataTable dt)
        {
            IList<ExcelAreaRecord> areas = new List<ExcelAreaRecord>();
            // First we need to include headers
            areas.Add(new ExcelAreaRecord(dt.Columns[0].ColumnName, dt.Columns[1].ColumnName, dt.Columns[2].ColumnName));
            // Then transform each row to object and add it to the list
            foreach (DataRow row in dt.Rows)
            {
                areas.Add(new ExcelAreaRecord(row[0] as string, row[1] as string, row[2] as string));
            }
            return areas.ToArray<ExcelAreaRecord>();
        }

        /// <summary>
        /// Transform datatable data to <see cref="ExcelAreaRecord"/> array.
        /// </summary>
        /// <param name="dt">The datatable containing the area data</param>
        /// <returns>A <see cref="ExcelAreaRecord"/> array.</returns>
        private ExcelAreaRecord[] TransformTableDataToExcelRecordsCombine(System.Data.DataSet ds)
        {
            IList<ExcelAreaRecord> areas = new List<ExcelAreaRecord>();
            DataTable dt = ds.Tables[0];
            // First we need to include headers
            areas.Add(new ExcelAreaRecord(dt.Columns[0].ColumnName, dt.Columns[1].ColumnName, dt.Columns[2].ColumnName));
            // Then transform each row to object and add it to the list
            foreach (DataTable dt1 in ds.Tables)
            {
                foreach (DataRow row in dt1.Rows)
                {
                    areas.Add(new ExcelAreaRecord(row[0] as string, row[1] as string, row[2] as string));
                }
            }
            return areas.ToArray<ExcelAreaRecord>();
        }

        /// <summary>
        /// Transform datatable data to <see cref="CsvAreaRecord"/> array.
        /// </summary>
        /// <param name="dt">The datatable containing the area data</param>
        /// <returns>A <see cref="CsvAreaRecord"/> array.</returns>
        private CsvAreaRecord[] TransformTableDataToCsvRecords(System.Data.DataTable dt)
        {
            IList<CsvAreaRecord> areas = new List<CsvAreaRecord>();
            // First we need to include headers
            areas.Add(new CsvAreaRecord(dt.Columns[0].ColumnName, dt.Columns[1].ColumnName, dt.Columns[2].ColumnName));
            // Then transform each row to object and add it to the list
            foreach (DataRow row in dt.Rows)
            {
                areas.Add(new CsvAreaRecord(row[0] as string, row[1] as string, row[2] as string));
            }
            return areas.ToArray<CsvAreaRecord>();
        }

        /// <summary>
        /// Transform datatable data to <see cref="CsvAreaRecord"/> array.
        /// </summary>
        /// <param name="dt">The datatable containing the area data</param>
        /// <returns>A <see cref="CsvAreaRecord"/> array.</returns>
        private CsvAreaRecord[] TransformTableDataToCsvRecordsHead(System.Data.DataTable dt)
        {
            IList<CsvAreaRecord> areas = new List<CsvAreaRecord>();
            // First we need to include headers
            areas.Add(new CsvAreaRecord(dt.Columns[0].ColumnName, dt.Columns[1].ColumnName, dt.Columns[2].ColumnName));

            return areas.ToArray<CsvAreaRecord>();
        }

        /// <summary>
        /// Transform datatable data to <see cref="CsvAreaRecord"/> array.
        /// </summary>
        /// <param name="dt">The datatable containing the area data</param>
        /// <returns>A <see cref="CsvAreaRecord"/> array.</returns>
        private CsvAreaRecord[] TransformTableDataToCsvRecordsCombine(System.Data.DataTable dt)
        {
            IList<CsvAreaRecord> areas = new List<CsvAreaRecord>();
            // Then transform each row to object and add it to the list
            foreach (DataRow row in dt.Rows)
            {
                areas.Add(new CsvAreaRecord(row[0] as string, row[1] as string, row[2] as string));
            }
            return areas.ToArray<CsvAreaRecord>();
        }

        #region Output Files

        /// <summary>
        /// Popup Dialog Box for open or download file
        /// </summary>
        /// <param name="csvData">file content</param>
        private void OutputCSVFile(string csvData)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AppendHeader("Content-Disposition", "attachment;filename=ExportData.csv");
            Response.ContentType = "text/csv";
            Response.Write(csvData);
            Response.End();
        }

        /// <summary>
        /// Popup Dialog Box for open or download file
        /// </summary>
        /// <param name="csvData">file content</param>
        private void OutputCSVFile(string csvData, string fileName)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".csv");
            Response.ContentType = "text/csv";
            Response.Write(csvData);
            Response.End();
        }

        private void OutputExcelFile(string filePath)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            Response.Clear();
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode("ExportData.xls"));
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "application/ms-excel";
            Response.WriteFile(file.FullName);
            Response.End();
        }

        private void OutputZipFile(string filePath)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            Response.Clear();
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode("ExportData.zip"));
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "application/zip";
            Response.WriteFile(file.FullName);
            Response.End();
        }

        #endregion

        #region ExportCSVFile

        ///// <summary>
        ///// Get areas from query string
        ///// </summary>
        ///// <returns></returns>
        //private string GetExportArea()
        //{
        //    Classifications classification = Classifications.BG;
        //    string idstr = string.Empty;
        //    try
        //    {
        //        //classification = (Classifications)Convert.ToInt32(Request.QueryString["classification"]);
        //        //idstr = Request.QueryString["areas"];
        //        classification = (Classifications)Convert.ToInt32(Request.Form["classification"]);
        //        idstr = Request.Form["areas"];
        //    }
        //    catch { }

        //    if (!string.IsNullOrEmpty(idstr))
        //    {
        //        Dictionary<Classifications, List<int>> areaIds = new Dictionary<Classifications, List<int>>();
        //        Dictionary<Classifications, List<int>> nonareaIds = new Dictionary<Classifications, List<int>>();
        //        string[] records = idstr.Split(';');
        //        foreach (string record in records)
        //        {
        //            string[] recordArray = record.Split(',');
        //            string id = recordArray[0];
        //            bool value = bool.Parse(recordArray[1]);
        //            string[] array = id.Split('$');
        //            Classifications cla = (Classifications)int.Parse(array[0]);
        //            int areaId = int.Parse(array[array.Length - 1]);
        //            if (value)
        //            {
        //                if (!areaIds.Keys.Contains(cla))
        //                {
        //                    areaIds.Add(cla, new List<int>());
        //                }
        //                areaIds[cla].Add(areaId);
        //            }
        //            else
        //            {
        //                if (!nonareaIds.Keys.Contains(cla))
        //                {
        //                    nonareaIds.Add(cla, new List<int>());
        //                }
        //                nonareaIds[cla].Add(areaId);
        //            }
        //        }
        //        return MapAreaManager.GetExportString(classification, areaIds, nonareaIds);
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}

        private DataSet GetExportDataSet(string dsString)
        {
            m_Logger.DebugFormat("begin get export data, splite by '#', input string is: {0}", dsString);
            DataSet ds = new DataSet();

            string[] dtStrings = dsString.Split('#');
            foreach (string dtString in dtStrings)
            {
                System.Data.DataTable dtExport = GetExportDataTable(dtString);
                if (dtExport != null)
                {
                    foreach (System.Data.DataTable dt in ds.Tables)
                    {
                        if (dtExport.TableName == dt.TableName)
                        {
                            dtExport.TableName = dtExport.TableName + "_1";
                        }
                    }
                    ds.Tables.Add(dtExport);
                }
            }

            return ds;
        }

        private System.Data.DataTable GetExportDataTable(string dtString)
        {
            if (string.IsNullOrWhiteSpace(dtString))
            {
                return null;
            }
            System.Data.DataTable dt;
            if (cid == 0) return new System.Data.DataTable();

            Exporter exporter = new Exporter(cid);

            string[] dtarray = dtString.Split('|');
            Classifications classification = (Classifications)int.Parse(dtarray[0]);
            string idstr = dtarray[1];
            string dtName = dtarray[2];

            if (!string.IsNullOrWhiteSpace(idstr))
            {
                Dictionary<Classifications, List<int>> areaIds = new Dictionary<Classifications, List<int>>();
                Dictionary<Classifications, List<int>> nonareaIds = new Dictionary<Classifications, List<int>>();
                string[] records = idstr.Split(';');
                foreach (string record in records)
                {
                    if (string.IsNullOrWhiteSpace(record))
                    {
                        continue;
                    }
                    string[] recordArray = record.Split(',');
                    Classifications cla = (Classifications)int.Parse(recordArray[0]);
                    int areaId = int.Parse(recordArray[1]);
                    bool value = bool.Parse(recordArray[2]);
                    if (value)
                    {
                        if (!areaIds.Keys.Contains(cla))
                        {
                            areaIds.Add(cla, new List<int>());
                        }
                        areaIds[cla].Add(areaId);
                    }
                    else
                    {
                        if (!nonareaIds.Keys.Contains(cla))
                        {
                            nonareaIds.Add(cla, new List<int>());
                        }
                        nonareaIds[cla].Add(areaId);
                    }
                }
                //dt = MapAreaManager.GetExportDataTable(classification, areaIds, nonareaIds);
                //dt = (new Exporter()).GetExportDataTable(classification, areaIds, nonareaIds);

                m_Logger.DebugFormat("begin get export data tabe\r\nclassification: {0}\r\nareaIds:{1}\r\nnonareaIds:{2}",
                    classification, Newtonsoft.Json.JsonConvert.SerializeObject(areaIds),
                    Newtonsoft.Json.JsonConvert.SerializeObject(nonareaIds));

                dt = exporter.GetExportDataTable(classification, areaIds, nonareaIds);
            }
            else
            {
                dt = new System.Data.DataTable();
            }
            dt.TableName = dtName;
            return dt;
        }

        #endregion

        #region ExportZipFile

        private void ExportTextZip(string dsString)
        {
            DataSet ds = GetExportDataSet(dsString);
            string filePath = SaveToTextZip(ds);
            if (!string.IsNullOrEmpty(filePath))
            {
                OutputZipFile(filePath);
            }
        }

        private string SaveToTextZip(DataSet ds)
        {
            string path = string.Format("{0}{1}", this.ImportFilesFolderPath, DateTime.Now.Ticks);
            Directory.CreateDirectory(path);
            foreach (System.Data.DataTable dt in ds.Tables)
            {
                //if (dt.Rows.Count < 1)
                //    continue;
                SaveTableToTextFile(path, dt);
            }

            if (ZipFile(path, ""))
            {
                return path + ".zip";
            }
            else
            {
                return string.Empty;
            }
        }       

        private void SaveTableToTextFile(string path, System.Data.DataTable dt)
        {
            var columnCount = dt.Columns.Count;

            //string lineFormat = "{0},{1},{2}";

            string fileFormat = "{0}\\{1}.txt";
            string filePath = string.Format(fileFormat, path, dt.TableName);
            //20120105 combine file
            string fileCombinedPath = string.Format(fileFormat, path, "Shapes_Addresses_Combine");
            int i = 1;
            while (File.Exists(filePath))
            {
                filePath = string.Format(fileFormat, path, dt.TableName + "_" + i.ToString());
                i++;
            }

            //20120105 combine file
            FileStream fsCombine = new FileStream(fileCombinedPath, FileMode.Append, FileAccess.Write);
            StreamWriter swCombine = new StreamWriter(fsCombine);

            var columnNameline = dt.Columns.ToExportFormat<DataColumn>((item) => { return item.ColumnName; });
            if (fsCombine.Length <= 0)
            {
                swCombine.WriteLine(columnNameline);
                //swCombine.WriteLine(string.Format(lineFormat, dt.Columns[0].ColumnName, dt.Columns[1].ColumnName,
                //    dt.Columns[2].ColumnName));
            }

            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);


            sw.WriteLine(columnNameline);
            //sw.WriteLine(string.Format(lineFormat, dt.Columns[0].ColumnName, dt.Columns[1].ColumnName, dt.Columns[2].ColumnName));
            foreach (DataRow row in dt.Rows)
            {
                var formmatedCells = row.ItemArray.ToExportFormat<object>((item) => { return item; });
                sw.WriteLine(formmatedCells);
                //20120105 combine file
                swCombine.WriteLine(formmatedCells);

                //sw.WriteLine(string.Format(lineFormat, row[0], row[1], row[2]));
                ////20120105 combine file
                //swCombine.WriteLine(string.Format(lineFormat, row[0], row[1], row[2]));
            }
            sw.Flush();
            sw.Close();
            fs.Close();
            //20120105 combine file
            swCombine.Flush();
            swCombine.Close();
            fsCombine.Close();
        }

        private bool ZipFile(string dirPath, string zipFilePath)
        {
            // When zip file path is not specified, the exported zip file name will be 'the directory name + .zip'
            if (zipFilePath == string.Empty)
            {
                if (dirPath.EndsWith("\\"))
                {
                    dirPath = dirPath.Substring(0, dirPath.Length - 1);
                }
                zipFilePath = dirPath + ".zip";
            }

            try
            {
                string[] filenames = Directory.GetFiles(dirPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    s.SetLevel(9);
                    byte[] buffer = new byte[4096];
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                GPS.Utilities.LogUtils.Error("Web application Unhandle error", ex);
                return false;
            }
            return true;
        }

        #endregion
    }

    public static class ExportHelper
    {
        public static string ToExportFormat<T>(this IEnumerable that, Func<T, object> func)
        {
            var formatted = string.Empty;
            foreach (var item in that)
            {
                formatted += func((T)item) + ",";
            }

            if (formatted.Length > 1)
            {
                return formatted.Remove(formatted.Length - 1);// remove last ','
            }
            return formatted; 
        }
    }
}
