using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class UrbanAreaCoordinateRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(UrbanAreaCoordinate urbanAreaCoord) {
            _dataContext.UrbanAreaCoordinates.InsertOnSubmit(urbanAreaCoord);
            Save();
        }

        public void AddAll(IEnumerable<UrbanAreaCoordinate> urbanAreaCoordList) {
            _dataContext.UrbanAreaCoordinates.InsertAllOnSubmit(urbanAreaCoordList);
            _dataContext.SubmitChanges();
        }

        private void GetEntryByID() {

        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
