using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.FixData
{
    class FiveZipFixer : Fixer<FiveZipArea>
    {
        protected override int GetItemCount()
        {
            AreaDataContext dataContext = new AreaDataContext();
            return dataContext.FiveZipAreas.Count();
        }

        protected override List<FiveZipArea> GetItems(int skip, int count)
        {
            AreaDataContext dataContext = new AreaDataContext();
            return dataContext.FiveZipAreas.OrderBy(t => t.Id).Skip(skip).Take(count).ToList();
        }

        protected override Dictionary<int, List<ICoordinate>> GetShapes(FiveZipArea t)
        {
            Dictionary<int, List<ICoordinate>> shapes = new Dictionary<int, List<ICoordinate>>();
            int shapeId = 0;
            var coordinates = t.FiveZipAreaCoordinates.OrderBy(f => f.Id).ToList();
            if (coordinates.Count > 0)
            {
                shapeId = coordinates[0].ShapeId;
                shapes.Add(shapeId, new List<ICoordinate>());
            }
            for (int i = 0; i < coordinates.Count; i++)
            {
                if (shapeId < coordinates[i].ShapeId)
                {
                    shapeId = coordinates[i].ShapeId;
                    shapes.Add(shapeId, new List<ICoordinate>());
                }
                shapes[shapeId].Add(coordinates[i]);
            }
            return shapes;
        }

        protected override void FixItems(List<FiveZipArea> items)
        {
            AreaDataContext dataContext = new AreaDataContext();
            foreach (FiveZipArea item in items)
            {
                List<int> ids = GetInnerShapeIds(item);
                if (ids.Count > 0)
                {
                    var coordinates = dataContext.FiveZipAreaCoordinates.Where(t => t.FiveZipAreaId == item.Id && ids.Contains(t.ShapeId));
                    dataContext.FiveZipAreaCoordinates.DeleteAllOnSubmit(coordinates);
                }
            }
            dataContext.SubmitChanges();
        }

        protected override string GetCode(FiveZipArea t)
        {
            return t.Code;
        }
    }
}
