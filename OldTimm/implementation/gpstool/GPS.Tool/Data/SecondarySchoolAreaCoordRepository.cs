using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class SecondarySchoolAreaCoordRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void AddAll(List<SecondarySchoolAreaCoordinate> secSchoolAreaCoordList) {
            _dataContext.SecondarySchoolAreaCoordinates.InsertAllOnSubmit(secSchoolAreaCoordList);
            Save();
        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
