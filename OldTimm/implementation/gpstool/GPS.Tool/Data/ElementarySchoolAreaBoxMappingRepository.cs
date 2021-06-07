using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class ElementarySchoolAreaBoxMappingRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(List<ElementarySchoolAreaBoxMapping> mappings) {
            _dataContext.ElementarySchoolAreaBoxMappings.InsertAllOnSubmit(mappings);
            Save();
        }

        private void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
