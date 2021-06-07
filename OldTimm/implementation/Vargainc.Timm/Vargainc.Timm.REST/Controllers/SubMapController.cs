using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Vargainc.Timm.EF;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("campaign")]
    public class SubMapController : ApiController
    {
        private TimmContext db = new TimmContext();

        [HttpGet]
        [Route("{campaignId:int}/submap/{submapId:int}")]
        public IHttpActionResult GetSubMaps(int campaignId, int submapId)
        {
            var submap = db.SubMaps.FirstOrDefault(i => i.Id == submapId);
            if(submap == null)
            {
                return NotFound();
            }
            ConcurrentDictionary<int, List<AbstractArea>> submapLocations = new ConcurrentDictionary<int, List<AbstractArea>>();

            Parallel.ForEach(submap.SubMapRecords, i => {
                IQueryable query = null;
                switch (i.Classification)
                {
                    case 1:
                        query = db.FiveZipAreas.Where(s => s.Id == i.AreaId);
                        break;
                    case 2:
                        query = db.Tracts.Where(s => s.Id == i.AreaId);
                        break;
                    case 3:
                        query = db.BlockGroups.Where(s => s.Id == i.AreaId);
                        break;
                    case 15:
                        query = db.PremiumCRoutes.Where(s => s.Id == i.AreaId);
                        break;
                    default:
                        break;
                }
                
            });

            return NotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
