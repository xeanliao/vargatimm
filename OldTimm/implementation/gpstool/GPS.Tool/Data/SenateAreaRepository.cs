using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class SenateAreaRepository
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

        public IQueryable<UpperSenateArea> GetAll()
        {
            return _dataContext.UpperSenateAreas;
        }
        
        public void Add(UpperSenateArea senateArea)
        {
            _dataContext.UpperSenateAreas.InsertOnSubmit(senateArea);
            Save();
        }        

        public int AddAll(List<UpperSenateArea> senateAreaList)
        {
            _dataContext.UpperSenateAreas.InsertAllOnSubmit(senateAreaList);
            Save();

            return senateAreaList[0].Id;
        }

        public void Save()
        {
            _dataContext.SubmitChanges();
        }
    }
}
