using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data {
    public class UnifiedSchoolAreaCoordinateRepository {
        AreaDataContext _dataContext;

        public AreaDataContext DataContext {
            get {
                return _dataContext;
            }
            set {
                _dataContext = value;
            }
        }

        public void Add(UnifiedSchoolAreaCoordinate unifiedSchoolAreaCoord) {
            _dataContext.UnifiedSchoolAreaCoordinates.InsertOnSubmit(unifiedSchoolAreaCoord);
            Save();
        }

        public void AddAll(List<UnifiedSchoolAreaCoordinate> unifiedSchoolAreaCoordList) {
            _dataContext.UnifiedSchoolAreaCoordinates.InsertAllOnSubmit(unifiedSchoolAreaCoordList);
            Save();
        }
        public void Save() {
            _dataContext.SubmitChanges();
        }
    }
}
