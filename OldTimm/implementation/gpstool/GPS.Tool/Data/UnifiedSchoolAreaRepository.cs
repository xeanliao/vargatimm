using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class UnifiedSchoolAreaRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public IQueryable<UnifiedSchoolArea> GetAll() {
            return _dataContext.UnifiedSchoolAreas;
        }

        public void Add(UnifiedSchoolArea unifiedSchool) {
            _dataContext.UnifiedSchoolAreas.InsertOnSubmit(unifiedSchool);
            Save();
        }

        public int AddAll(List<UnifiedSchoolArea> uniSchoolList) {
            _dataContext.UnifiedSchoolAreas.InsertAllOnSubmit(uniSchoolList);
            Save();

            return uniSchoolList[0].Id;
        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
