using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class SenateAreaBoxMappingRepository
    {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(List<UpperSenateAreaBoxMapping> mappings)
        {
            _dataContext.UpperSenateAreaBoxMappings.InsertAllOnSubmit(mappings);
            Save();
        }

        private void Save()
        {
            _dataContext.SubmitChanges();
        }
    }
}
