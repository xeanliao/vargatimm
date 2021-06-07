using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Area.AreaOperators;
using GPS.DataLayer;
using System.Data.Linq;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.Area.Import;

namespace GPS.DomainLayer.Area
{
    public abstract class AreaOperatorBase
    {
        public abstract IEnumerable<IArea> GetBoxAreas(int boxId);

        public abstract IEnumerable<IArea> GetBoxAreas(int campaignId, int boxId);

        public abstract IArea GetArea(int id);

        //public abstract 
    }

    public abstract class AreaOperator<T> : AreaOperatorBase
    {
        protected abstract IEnumerable<T> GetBoxItems(int boxId);

        public abstract T GetItem(int id);

        public override IArea GetArea(int id)
        {
            T item = GetItem(id);
            return ConvertToArea(item);
        }

        public override IEnumerable<IArea> GetBoxAreas(int campaignId, int boxId)
        {
            ICampaignRepository repository = WorkSpaceManager.Instance.NewWorkSpace().Repositories.CampaignRepository;
            Campaign campaign = repository.GetEntity(campaignId);
            IEnumerable<T> items = GetBoxItems(boxId);
            if (campaign != null)
            {
                return ConvertToAreas(campaign, items);
            }
            else
            {
                return ConvertToAreas(items);
            }

        }

        public override IEnumerable<IArea> GetBoxAreas(int boxId)
        {
            IEnumerable<T> items = GetBoxItems(boxId);
            return ConvertToAreas(items);
        }

        protected abstract MapArea ConvertToArea(Campaign campaign, T item);

        protected abstract MapArea ConvertToArea(T item);

        public IEnumerable<IArea> ConvertToAreas(IEnumerable<T> items)
        {
            List<IArea> areas = new List<IArea>();
            foreach (T item in items)
            {
                areas.Add(ConvertToArea(item));
            }
            return areas;
        }

        public IEnumerable<IArea> ConvertToAreas(Campaign campaign, IEnumerable<T> items)
        {
            List<IArea> areas = new List<IArea>();
            foreach (T item in items)
            {
                areas.Add(ConvertToArea(campaign, item));
            }
            return areas;
        }
    }

    public class AreaOperatorFacory
    {
        public static AreaOperatorBase CreateOperator(Classifications classification)
        {
            AreaOperatorBase oper = null;
            switch (classification)
            {
                case Classifications.Z3:
                    oper = new ThreeZipAreaOperator();
                    break;
                case Classifications.Z5:
                    oper = new FiveZipAreaOperator();
                    break;
                case Classifications.TRK:
                    oper = new TractOperator();
                    break;
                case Classifications.BG:
                    oper = new BlockGroupOperator();
                    break;
                case Classifications.PremiumCRoute:
                    oper = new PremiumCRouteOperator();
                    break;
            }
            return oper;
        }
    }


}
