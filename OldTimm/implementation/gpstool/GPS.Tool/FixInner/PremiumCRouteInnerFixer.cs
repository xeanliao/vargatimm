using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Collections;


namespace GPS.Tool.FixInner
{
    class PremiumCRouteInnerFixer : FixerInner<PremiumCRoute>
    {

        protected override int GetItemCount()
        {
            PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
            int count = dataContext.PremiumCRouteBoxMappings.Select(b => b.BoxId ).Distinct().Count();
            return count;
        }

        protected override List<int> GetItems(int skip, int count)
        {
            PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
            return dataContext.PremiumCRouteBoxMappings.OrderBy(b => b.BoxId).Select(b => b.BoxId).Distinct().Skip(skip).Take(count).ToList();
        }

        protected void Test()
        {
            PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
            PremiumCRoute p1 = dataContext.PremiumCRouteBoxMappings.Where(t => t.PreminumCRouteId == 268981).Select(t => t.PremiumCRoute).First();
            PremiumCRoute p2 = dataContext.PremiumCRouteBoxMappings.Where(t => t.PreminumCRouteId == 558863).Select(t => t.PremiumCRoute).First();
            List<Types.Loc> masterList = new List<Types.Loc>();
            foreach (ICoordinate o in p1.PremiumCRouteCoordinates)
            {
                masterList.Add(new Types.Loc(o.Latitude, o.Longitude));
            }
            Types.Poly oPoly = new Types.Poly(masterList);

            if (InnerShape(oPoly, p2.PremiumCRouteCoordinates.ToList()))
            {

            }
        }


        protected override void FixItems(List<int> items)
        {
            PremiumAreasDataContext dataContext = new PremiumAreasDataContext();


            //var premiumcroutes = dataContext.PremiumCRoutes.Where(t => t.IsInnerShape != 0);
            //List<PremiumCRoute> list = premiumcroutes.ToList<PremiumCRoute>();
            //Dictionary<string, Types.Poly> dic = new Dictionary<string, Types.Poly>();
            //int i, j;
            //i = j = 0;

            //foreach (PremiumCRoute croute in list)
            //{
            //    Types.Poly oPoly = null;

            //    if (dic.ContainsKey(croute.IsInnerShape.ToString()))
            //    {
            //        oPoly = dic[croute.IsInnerShape.ToString()];
            //    }
            //    else
            //    {
            //        PremiumCRoute father = dataContext.PremiumCRoutes.Where(t => t.ID == croute.IsInnerShape).First();
            //        List<Types.Loc> masterList = new List<Types.Loc>();
            //        foreach (ICoordinate o in father.PremiumCRouteCoordinates)
            //        {
            //            masterList.Add(new Types.Loc(o.Latitude, o.Longitude));
            //        }
            //        oPoly = new Types.Poly(masterList);
            //        dic.Add(croute.IsInnerShape.ToString(), oPoly);
            //    }

                
            //    Types.Loc loc = new Types.Loc(croute.Latitude, croute.Longitude);
            //    if (!GeoUtils.inPoly(loc, oPoly))
            //    {
            //        croute.IsInnerShape = 0;
            //        i++;
            //    }
            //    else
            //    {
            //        j++;

            //    }

            //}

            //string count = i.ToString() + "---" + j.ToString();



            foreach (int item in items)
            {
                var premiumcroutes = dataContext.PremiumCRouteBoxMappings.Where(t => t.BoxId == item).Select(t => t.PremiumCRoute);
                List<PremiumCRoute> changeList = new List<PremiumCRoute>();

                if (premiumcroutes != null)
                {
                    List<List<PremiumCRouteCoordinate>> shapes = new List<List<PremiumCRouteCoordinate>>();
                    foreach (PremiumCRoute croute in premiumcroutes)
                    {
                        if (croute.IsInnerShape == null || croute.IsInnerShape == 0)
                        {
                            List<PremiumCRouteCoordinate> list = croute.PremiumCRouteCoordinates.ToList<PremiumCRouteCoordinate>();
                            shapes.Add(list);
                        }
                        else
                        {
                            base._innerCount++;
                        }
                    }



                    for (int i = 0; i < shapes.Count; i++)
                    {
                        Types.Poly oPoly = null;

                        List<PremiumCRouteCoordinate> parentList = shapes[i];
                        PremiumCRoute parent = parentList[0].PremiumCRoute;
                        //double pminLat = parent.MinLatitude;
                        //double pmaxLat = parent.MaxLatitude;
                        //double pminLon = parent.MinLongitude;
                        //double pmaxLon = parent.MaxLongitude;   

                        if (parent.IsInnerShape != null && parent.IsInnerShape != 0)
                        {
                            continue;
                        }


                        List<Types.Loc> masterList = new List<Types.Loc>();
                        foreach (ICoordinate o in parentList)
                        {
                            masterList.Add(new Types.Loc(o.Latitude, o.Longitude));
                        }
                        oPoly = new Types.Poly(masterList);


                        for (int j = 0; j < shapes.Count; j++)
                        {
                            if (i != j)
                            {
                                List<PremiumCRouteCoordinate> childList = shapes[j];
                                PremiumCRoute child = childList[0].PremiumCRoute;
                                //double cminLat = child.MinLatitude;
                                //double cmaxLat = child.MaxLatitude;
                                //double cminLon = child.MinLongitude;
                                //double cmaxLon = child.MaxLongitude;


                               


                                //if (cmaxLat <= pmaxLat && cminLat >= pminLat && cmaxLon <= pmaxLon && cminLon >= pminLon)
                                //{
                                if (child.IsInnerShape != null && child.IsInnerShape != 0)
                                {
                                    continue;
                                }

                                List<PremiumCRouteCoordinate> finalchildList = new List<PremiumCRouteCoordinate>();
                                for (int m = 0; m < childList.Count; m++)
                                {
                                    finalchildList.Add(childList[m]);

                                    if (m + 1 < childList.Count)
                                    {
                                        PremiumCRouteCoordinate coord = new PremiumCRouteCoordinate();
                                        coord.Latitude = (childList[m].Latitude + childList[m + 1].Latitude) / 2;
                                        coord.Longitude = (childList[m].Longitude + childList[m + 1].Longitude) / 2;
                                        finalchildList.Add(coord);
                                    }

                                }

                                if (InnerShape(oPoly, finalchildList))
                                {

                                    child.IsInnerShape = parentList[0].PreminumCRouteId;
                                    changeList.Add(child);
                                    base._innerCount++;

                                }
                                //}

                            }
                        }
                    }

                }

                if (changeList.Count > 0)
                {
                    dataContext.SubmitChanges();
                }

                base._current++;
                SendMessage(item.ToString(), changeList.Count > 0);
            }
        }


        private bool InnerShape(Types.Poly oPoly, List<PremiumCRouteCoordinate> innerCoordinates)
        {
            bool inner = true;
            foreach (PremiumCRouteCoordinate coordinate in innerCoordinates)
            {
                Types.Loc loc = new Types.Loc(coordinate.Latitude, coordinate.Longitude);
                if (!GeoUtils.inPoly(loc, oPoly))
                {
                    inner = false;
                    break;
                }
            }
            return inner;
        }





        //public bool IsPointInPolygon(List<Types.Loc> oLocList, double lat, double lon)
        //{

        //    Types.Poly oPoly = new Types.Poly(oLocList);
        //    Types.Loc loc = new Types.Loc(lat, lon);
        //    bool bRet = GeoUtils.inPoly(loc, oPoly);

        //    return bRet;
        //}


        

       
    }
}
