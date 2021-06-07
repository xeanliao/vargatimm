using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.FixData
{
    class PremiumCRouteFixer : Fixer<PremiumCRoute>
    {
        protected override int GetItemCount()
        {
            PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
            return dataContext.PremiumCRoutes.Count();
        }

        protected override List<PremiumCRoute> GetItems(int skip, int count)
        {
            PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
            return dataContext.PremiumCRoutes.OrderBy(t => t.ID).Skip(skip).Take(count).ToList();
        }

        protected override void FixItems(List<PremiumCRoute> items)
        {
            PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
            foreach (PremiumCRoute item in items)
            {
                List<int> ids = GetInnerShapeIds(item);
                if (ids.Count > 0)
                {
                    var coordinates = dataContext.PremiumCRouteCoordinates.Where(t => t.PreminumCRouteId == item.ID && ids.Contains(t.ShapeId));
                    dataContext.PremiumCRouteCoordinates.DeleteAllOnSubmit(coordinates);
                }
            }
            dataContext.SubmitChanges();
        }

        protected override Dictionary<int, List<ICoordinate>> GetShapes(PremiumCRoute t)
        {
            Dictionary<int, List<ICoordinate>> shapes = new Dictionary<int, List<ICoordinate>>();
            int shapeId = 0;
            var coordinates = t.PremiumCRouteCoordinates.OrderBy(p => p.ID).ToList();
            if (coordinates.Count > 0)
            {
                shapeId = coordinates[0].ShapeId;
                List<ICoordinate> shape = new List<ICoordinate>();
                shapes.Add(shapeId, new List<ICoordinate>());
            }

            for (int i = 0; i < coordinates.Count; i++)
            {
                if (shapeId != coordinates[i].ShapeId)
                {
                    shapeId = coordinates[i].ShapeId;
                    shapes.Add(shapeId, new List<ICoordinate>());
                }
                shapes[shapeId].Add(coordinates[i]);
            }
            return shapes;
        }

        protected override string GetCode(PremiumCRoute t)
        {
            return t.CROUTE;
        }
    }
}
