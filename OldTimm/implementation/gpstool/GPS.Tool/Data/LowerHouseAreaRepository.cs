using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class LowerHouseAreaRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public IQueryable<LowerHouseArea> GetAll() {
            return _dataContext.LowerHouseAreas;
        }

        public int AddAll(List<LowerHouseArea> lowerHouseAreaList) {
            _dataContext.LowerHouseAreas.InsertAllOnSubmit(lowerHouseAreaList);
            Save();

            return lowerHouseAreaList[0].Id;
        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
