using System.Linq;
using TIMM.GPS.Model;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace TIMM.GPS.RESTService
{
    [ServiceContract]
    public class TaskService
    {
        [WebInvoke(UriTemplate = "query/", Method = "POST")]
        public QueryResult<Task> QueryTask(TaskCriteria criteria)
        {
            using (var context = new TIMMContext())
            {
                QueryResult<Task> result = new QueryResult<Task>();
                var query = context.Tasks.Where(i => i.Status == 0).AsQueryable();
                if (!string.IsNullOrWhiteSpace(criteria.Name))
                {
                    //query = query.Where("Name = @0", criteria.Name);
                    query = query.Where(o => o.Name.Contains(criteria.Name));
                }
                if (!string.IsNullOrWhiteSpace(criteria.SortField))
                {
                    query = query.OrderBy(criteria.SortField);
                }
                else
                {
                    query = query.OrderByDescending(i => i.Id);
                }
                result.TotalRecord = query.Count();
                query = query.Skip(criteria.PageIndex * criteria.PageSize)
                    .Take(criteria.PageSize)
                    .Include(i => i.DistributionMap.SubMap.Campaign)
                    .Include(i => i.Auditor);

                result.Result = query.ToList();
                return result;
            }
        }

        [WebGet(UriTemplate = "report/")]
        public List<Campaign> QueryReport()
        {
            using (var context = new TIMMContext())
            {
                var query = from i in context.Tasks
                            where i.Status == 1
                            orderby i.Date
                            select i.DistributionMap.SubMap.Campaign;

                query = query.AsQueryable().Include("SubMaps.DistributionMaps.Tasks");
                var result = query.Distinct().ToList();
                result.ForEach(i =>
                {
                    i.SubMaps.ForEach(j =>
                    {
                        j.DistributionMaps.ForEach(k =>
                        {
                            k.Tasks.ForEach(l =>
                            {
                                if (l.Status != 1)
                                {
                                    k.Tasks.Remove(l);
                                }
                            });
                        });
                    });
                });
                return result;
            }
        }

        [WebGet(UriTemplate = "report/backtomonitor/{taskId}")]
        public void MissBackToMonitor(int taskId)
        {
            using (var context = new TIMMContext())
            {
                var task = context.Tasks.FirstOrDefault(i => i.Id == taskId);
                if (task != null)
                {
                    task.Status = 0;
                    context.SaveChanges();
                }
            }
        }

        [WebInvoke(UriTemplate = "modify/", Method = "POST")]
        public Task EditTask(Task entity)
        {
            using (var context = new TIMMContext())
            {

                Task old = null;
                if (entity.Id > 0)
                {
                    //update
                    old = context.Tasks.First(i => i.Id == entity.Id);
                }

                old.Name = entity.Name;
                old.Date = entity.Date;
                //old.Auditor = entity.Auditor;
                old.Email = entity.Email;
                old.Telephone = entity.Telephone;

                context.SaveChanges();
                return old;
            }
        }
    }
}
