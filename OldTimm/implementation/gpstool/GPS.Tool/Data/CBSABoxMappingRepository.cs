using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class CBSABoxMappingRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(List<MetropolitanCoreAreaBoxMapping> cbsaBoxMappingList) {
            _dataContext.MetropolitanCoreAreaBoxMappings.InsertAllOnSubmit(cbsaBoxMappingList);
            Save();
        }

        private void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
