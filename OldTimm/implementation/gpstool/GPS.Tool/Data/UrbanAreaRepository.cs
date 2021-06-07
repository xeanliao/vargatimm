using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class UrbanAreaRepository {
        private AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public IQueryable<UrbanArea> GetAll() {
            return _dataContext.UrbanAreas;
        }

        public void Add(UrbanArea urban) {
            _dataContext.UrbanAreas.InsertOnSubmit(urban);
            Save();
        }

        public int AddAll(List<UrbanArea> urbanList) {
            _dataContext.UrbanAreas.InsertAllOnSubmit(urbanList);
            Save();

            return urbanList[0].Id;
        }

        private void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
