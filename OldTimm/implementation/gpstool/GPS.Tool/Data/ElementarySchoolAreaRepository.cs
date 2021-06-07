using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class ElementarySchoolAreaRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public IQueryable<ElementarySchoolArea> GetAll() {
            return _dataContext.ElementarySchoolAreas;
        }

        public int AddAll(List<ElementarySchoolArea> eleSchoolAreaList) {
            _dataContext.ElementarySchoolAreas.InsertAllOnSubmit(eleSchoolAreaList);
            Save();

            return eleSchoolAreaList[0].Id;
        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
