using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class CountyAreaCoordRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(CountyAreaCoordinate countyAreaCoord) {
            _dataContext.CountyAreaCoordinates.InsertOnSubmit(countyAreaCoord);
            Save();
        }

        public void AddAll(List<CountyAreaCoordinate> countyAreaCoordList) {
            _dataContext.CountyAreaCoordinates.InsertAllOnSubmit(countyAreaCoordList);
            Save();
        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
