using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class LowerHouseAreaCoordRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void AddAll(List<LowerHouseAreaCoordinate> lowerHouseAreaCoordList) {
            _dataContext.LowerHouseAreaCoordinates.InsertAllOnSubmit(lowerHouseAreaCoordList);
            Save();
        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
