using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class LowerHouseAreaBoxMappingRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(List<LowerHouseAreaBoxMapping> mappings) {
            _dataContext.LowerHouseAreaBoxMappings.InsertAllOnSubmit(mappings);
            Save();
        }

        private void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
