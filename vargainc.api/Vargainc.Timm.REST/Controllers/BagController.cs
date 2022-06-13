using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.REST.Controllers
{
    [RoutePrefix("bag")]
    [Helper.PublicAccessFilter]
    public class BagController : BaseController
    {
        [Route("all")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllBag()
        {
            var result = await db.GTUBags
                .Select(i => new { i.Id, i.UserId })
                .ToListAsync()
                .ConfigureAwait(false);

            return Json(result);
        }

        [Route("user/none")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBagNoUser()
        {
            var bags = await db.GTUBags.Where(i => i.UserId == null).Select(i => i.Id).ToListAsync().ConfigureAwait(false);
            return Json(bags);
        }

        [Route("user/{userId:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBagInUser(int? userId)
        {
            var bags = await db.GTUBags.Where(i=>i.UserId == userId).Select(i=>i.Id).ToListAsync().ConfigureAwait(false);
            return Json(bags);
        }

        [Route("user/{userId:int}")]
        [HttpPost]
        public IHttpActionResult UpdateBagInUser(int? userId, [FromBody]List<int?> bags)
        {
            var targetBags = bags.ToHashSet();
            using (var tran = db.Database.BeginTransaction())
            {
                db.GTUBags.Where(i => i.UserId == userId).UpdateFromQuery(i => new GTUBag { UserId = null });
                db.GTUBags.Where(i => targetBags.Contains(i.Id)).UpdateFromQuery(i => new GTUBag { UserId = userId });

                tran.Commit();
            }
            return Json(new { success = true });
        }
    }
}