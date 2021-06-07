using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using GPS.DataLayer.DataInfrastructure;
using GPS.DomainLayer.Interfaces;

namespace GPS.ArbitraryUniqueCodeManager
{
    class Program
    {
        private static ISessionFactory _sessionFactory;

        static void Main(string[] args)
        {
            _sessionFactory = new Configuration().Configure().BuildSessionFactory();

            //UpdateArbitraryUniqueCodeForTracts();
            //UpdateArbitraryUniqueCodeForBlockGroups();

            //UpdatePartCountForFiveZips();
            //UpdatePartCountForCRoutes();

            //UpdateIsInnerShapeForFiveZips();
            UpdateIsInnerShapeForCRoutes();

            Console.ReadLine();
        }

        static void UpdateArbitraryUniqueCodeForTracts()
        {
            ISession session = _sessionFactory.OpenSession();

            Int32 total = session.Linq<Tract>().Count();
            for (int i = 0; i < total; i += 1000)
            {
                IEnumerable<Tract> items = session.Linq<Tract>().Where(t => t.Id >= i && t.Id < i + 1000);
                foreach (Tract t in items)
                {
                    t.ArbitraryUniqueCode = t.StateCode.PadLeft(2, '0') + t.CountyCode.PadLeft(3, '0') + t.Code.PadRight(6, '0');
                }
                session.Flush();
                session.Clear();

                Console.WriteLine("Completed: {0}", i);
            }

            Console.WriteLine("Tracts update completed");
        }

        static void UpdateArbitraryUniqueCodeForBlockGroups()
        {
            ISession session = _sessionFactory.OpenSession();

            Int32 total = session.Linq<BlockGroup>().Count();
            for (int i = 0; i < total; i += 1000)
            {
                IEnumerable<BlockGroup> items = session.Linq<BlockGroup>().Where(t => t.Id >= i && t.Id < i + 1000);
                foreach (BlockGroup t in items)
                {
                    t.ArbitraryUniqueCode = t.StateCode.PadLeft(2, '0') + t.CountyCode.PadLeft(3, '0') + t.TractCode.PadRight(6, '0') + t.Code;
                }
                session.Flush();
                session.Clear();

                Console.WriteLine("Completed: {0}", i);
            }

            Console.WriteLine("BlockGroups update completed");
        }

        static void UpdatePartCountForFiveZips()
        {
            ISession session = _sessionFactory.OpenSession();

            Int32 total = session.Linq<FiveZipArea>().Count();
            for (int i = 0; i < total; i += 1000)
            {
                IEnumerable<FiveZipArea> items = session.Linq<FiveZipArea>().Where(t => t.Id >= i && t.Id < i + 1000);
                foreach (FiveZipArea item in items)
                {
                    item.PartCount = session.Linq<FiveZipArea>().Where(t => t.Code == item.Code).Count();
                }
                session.Flush();
                session.Clear();

                Console.WriteLine("Completed: {0}", i);
            }

            Console.WriteLine("FiveZipAreas update completed");
        }

        static void UpdatePartCountForCRoutes()
        {
            ISession session = _sessionFactory.OpenSession();

            Int32 total = session.Linq<PremiumCRoute>().Count();
            for (int i = 0; i < total; i += 1000)
            {
                IEnumerable<PremiumCRoute> items = session.Linq<PremiumCRoute>().Where(t => t.Id >= i && t.Id < i + 1000);
                foreach (PremiumCRoute item in items)
                {
                    item.PartCount = session.Linq<PremiumCRoute>().Where(t => t.Code == item.Code).Count();
                }
                session.Flush();
                session.Clear();

                Console.WriteLine("Completed: {0}", i);
            }

            Console.WriteLine("PremiumCRoutes update completed");
        }

        static void UpdateIsInnerShapeForFiveZips()
        {
            ISession session = _sessionFactory.OpenSession();

            Int32 total = session.Linq<FiveZipArea>().Count();
            for (int i = 0; i < total; i += 1000)
            {
                IEnumerable<FiveZipArea> items = session.Linq<FiveZipArea>().Where(t => t.Id >= i && t.Id < i + 1000);
                foreach (FiveZipArea item in items)
                {
                    //item.IsInnerShape = IsInnerFiveZip(session, item);
                }
                session.Flush();
                session.Clear();

                Console.WriteLine("Completed: {0}", i);
            }

            Console.WriteLine("FiveZipAreas update completed");
        }

        static void UpdateIsInnerShapeForCRoutes()
        {
            ISession session = _sessionFactory.OpenSession();
            //List<int> ids = new List<int>() { 674783, 674737, 674775, 674734 };
            Int32 total = session.Linq<PremiumCRoute>().Count();
            for (int i = 0; i < total; i += 1000)
            {
                IEnumerable<PremiumCRoute> items = session.Linq<PremiumCRoute>().Where(t => t.Id >= i && t.Id < i + 1000);
                foreach (PremiumCRoute item in items)
                {
                   // item.IsInnerShape = IsInnerCRoute(session, item);
                }
                session.Flush();
                session.Clear();

                Console.WriteLine("Completed: {0}", i);
            }

            Console.WriteLine("PremiumCRoutes update completed");
        }

        static bool IsInnerFiveZip(ISession session, FiveZipArea fiveZip)
        {
            bool inner = false;
            const string queryFormat =
                  " select distinct f from FiveZipArea f join f.FiveZipBoxMappings fzbm where"
                + " fzbm.BoxId in (:boxIds) and f.Id != :fzId";
            IList<FiveZipArea> items = session.CreateQuery(queryFormat)
                .SetParameterList("boxIds", fiveZip.FiveZipBoxMappings.Select<FiveZipBoxMapping, Int32>(t => t.BoxId).ToArray())
                .SetParameter("fzId", fiveZip.Id)
                .List<FiveZipArea>();

            foreach (var item in items)
            {
                if (IsInnerFiveZip(item, fiveZip))
                {
                    inner = true;
                    break;
                }
            }

            return inner;
        }

        static bool PointInPolygon(IList<FiveZipAreaCoordinate> coordinates, double lat, double lon)
        {
            bool inPolygon = false;
            int j = coordinates.Count - 1;

            for (int i = 0; i < coordinates.Count; i++)
            {
                if (coordinates[i].Longitude < lon && coordinates[j].Longitude >= lon
                  || coordinates[j].Longitude < lon && coordinates[i].Longitude >= lon)
                {
                    if (coordinates[i].Latitude + (lon - coordinates[i].Longitude) /
                      (coordinates[j].Longitude - coordinates[i].Longitude) * (coordinates[j].Latitude
                        - coordinates[i].Latitude) < lat)
                    {
                        inPolygon = !inPolygon;
                    }
                }
                j = i;
            }

            return inPolygon;
        }

        static bool IsInnerFiveZip(FiveZipArea master, FiveZipArea inner)
        {
            bool isInner = false;

            if ((master.MaxLatitude > inner.MinLatitude) && (master.MinLongitude < inner.MaxLongitude) &&
                (master.MinLatitude < inner.MaxLatitude) && (master.MaxLongitude > inner.MinLongitude))
            {
                int passCount = inner.FiveZipAreaCoordinates.Count;
                int innerCount = 0;
                foreach (var c in inner.FiveZipAreaCoordinates)
                {
                    if (PointInPolygon(master.FiveZipAreaCoordinates, c.Latitude, c.Longitude))
                    {
                        innerCount++;
                    }
                    if (innerCount >= passCount)
                    {
                        isInner = true;
                        break;
                    }
                }
            }
            return isInner;
        }


        static bool PointInPolygon(IList<PremiumCRouteCoordinate> coordinates, double lat, double lon)
        {
            bool inPolygon = false;
            int j = coordinates.Count - 1;

            for (int i = 0; i < coordinates.Count; i++)
            {
                if (coordinates[i].Longitude < lon && coordinates[j].Longitude >= lon
                  || coordinates[j].Longitude < lon && coordinates[i].Longitude >= lon)
                {
                    if (coordinates[i].Latitude + (lon - coordinates[i].Longitude) /
                      (coordinates[j].Longitude - coordinates[i].Longitude) * (coordinates[j].Latitude
                        - coordinates[i].Latitude) < lat)
                    {
                        inPolygon = !inPolygon;
                    }
                }
                j = i;
            }

            return inPolygon;
        }

        static bool IsInnerCRoute(PremiumCRoute master, PremiumCRoute inner)
        {
            bool isInner = false;

            if ((master.MaxLatitude > inner.MinLatitude) && (master.MinLongitude < inner.MaxLongitude) &&
                (master.MinLatitude < inner.MaxLatitude) && (master.MaxLongitude > inner.MinLongitude))
            {
                int passCount = inner.PremiumCRouteCoordinates.Count;
                int innerCount = 0;
                foreach (var c in inner.PremiumCRouteCoordinates)
                {
                    if (PointInPolygon(master.PremiumCRouteCoordinates, c.Latitude, c.Longitude))
                    {
                        innerCount++;
                    }
                    if (innerCount >= passCount)
                    {
                        isInner = true;
                        break;
                    }
                }
            }
            return isInner;
        }

        static bool IsInnerCRoute(ISession session, PremiumCRoute cRoute)
        {
            bool inner = false;
            const string queryFormat =
                  " select distinct cr from PremiumCRoute cr join cr.PremiumCRouteBoxMappings crbm where"
                + " crbm.BoxId in (:boxIds) and cr.Id != :cRouteId";
            IList<PremiumCRoute> items = session.CreateQuery(queryFormat)
                .SetParameterList("boxIds", cRoute.PremiumCRouteBoxMappings.Select<PremiumCRouteBoxMapping, Int32>(t => t.BoxId).ToArray())
                .SetParameter("cRouteId", cRoute.Id)
                .List<PremiumCRoute>();

            foreach (var item in items)
            {
                if (IsInnerCRoute(item, cRoute))
                {
                    inner = true;
                    break;
                }
            }

            return inner;
        }

    }
}
