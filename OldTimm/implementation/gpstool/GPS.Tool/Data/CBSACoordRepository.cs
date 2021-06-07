using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class CBSACoordRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void AddAll(List<MetropolitanCoreAreaCoordinate> cbsaCoordList) {
            _dataContext.MetropolitanCoreAreaCoordinates.InsertAllOnSubmit(cbsaCoordList);
            Save();
        }

        private void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
