using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class SecondarySchoolAreaRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public IQueryable<SecondarySchoolArea> GetAll() {
            return _dataContext.SecondarySchoolAreas;
        }

        public int AddAll(List<SecondarySchoolArea> secSchoolAreaList) {
            _dataContext.SecondarySchoolAreas.InsertAllOnSubmit(secSchoolAreaList);
            Save();

            return secSchoolAreaList[0].Id;
        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
