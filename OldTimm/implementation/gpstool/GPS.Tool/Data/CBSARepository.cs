using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class CBSARepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public IQueryable<MetropolitanCoreArea> GetAll() {
            return _dataContext.MetropolitanCoreAreas;
        }

        public int AddAll(List<MetropolitanCoreArea> cbsaList) {
            _dataContext.MetropolitanCoreAreas.InsertAllOnSubmit(cbsaList);
            Save();

            return cbsaList[0].Id;
        }

        private void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
