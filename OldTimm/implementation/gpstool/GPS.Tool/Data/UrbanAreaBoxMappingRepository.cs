using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class UrbanAreaBoxMappingRepository
    {
        private AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(List<UrbanAreaBoxMapping> mappings)
        {
            _dataContext.UrbanAreaBoxMappings.InsertAllOnSubmit(mappings);
            Save();
        }

        private void Save()
        {
            _dataContext.SubmitChanges();
        }
    }
}
