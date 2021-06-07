using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class CountyAreaRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(CountyArea countyArea) {
            _dataContext.CountyAreas.InsertOnSubmit(countyArea);
            Save();
        }

        public int AddAll(List<CountyArea> countyAreaList) {
            _dataContext.CountyAreas.InsertAllOnSubmit(countyAreaList);
            Save();

            return countyAreaList[0].Id;
        }

        public IQueryable<CountyArea> GetAll() {
            return _dataContext.CountyAreas;
        }

        public IQueryable<CountyArea> GetCountyAreas(string countyCode) {
            return _dataContext.CountyAreas.Where(c => c.Code == countyCode);
        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
