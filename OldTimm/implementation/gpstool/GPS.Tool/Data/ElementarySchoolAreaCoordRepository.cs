using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class ElementarySchoolAreaCoordRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void AddAll(List<ElementarySchoolAreaCoordinate> eleSchoolAreaCoordList) {
            _dataContext.ElementarySchoolAreaCoordinates.InsertAllOnSubmit(eleSchoolAreaCoordList);
            Save();
        }

        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
