using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class SenateAreaCoordRepository
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
        
        public void Add(UpperSenateAreaCoordinate senateAreaCoord)
        {
            _dataContext.UpperSenateAreaCoordinates.InsertOnSubmit(senateAreaCoord);
            Save();
        }

        public void AddAll(List<UpperSenateAreaCoordinate> senateAreaCoordList)
        {
            _dataContext.UpperSenateAreaCoordinates.InsertAllOnSubmit(senateAreaCoordList);
            Save();
        }

        public void Save()
        {
            _dataContext.SubmitChanges();
        }
    }
}
