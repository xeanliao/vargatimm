using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class SecondarySchoolAreaBoxMappingRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(List<SecondarySchoolAreaBoxMapping> mappings) {
            _dataContext.SecondarySchoolAreaBoxMappings.InsertAllOnSubmit(mappings);
            Save();
        }

        private void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
