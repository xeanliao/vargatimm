using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.FixInner
{
    class FiveZipInnerFixer : FixerInner<FiveZipArea>
    {
        protected override int GetItemCount()
        {
            AreaDataContext dataContext = new AreaDataContext();
            int count = dataContext.FiveZipBoxMappings.Select(b => b.BoxId).Distinct().Count();
            return count;
        }

        protected override List<int> GetItems(int skip, int count)
        {
            AreaDataContext dataContext = new AreaDataContext();
            return dataContext.FiveZipBoxMappings.OrderBy(b => b.BoxId).Select(b => b.BoxId).Distinct().Skip(skip).Take(count).ToList();
        }

        protected override void FixItems(List<int> items)
        {
            AreaDataContext dataContext = new AreaDataContext();


            foreach (int item in items)
            {
                var fivezips = dataContext.FiveZipBoxMappings.Where(t => t.BoxId == item).Select(t => t.FiveZipArea);
                List<FiveZipArea> changeList = new List<FiveZipArea>();

                if (fivezips != null)
                {
                    List<List<FiveZipAreaCoordinate>> shapes = new List<List<FiveZipAreaCoordinate>>();
                    foreach (FiveZipArea f in fivezips)
                    {
                        if (f.IsInnerShape == null || f.IsInnerShape == 0)
                        {
                            List<FiveZipAreaCoordinate> list = f.FiveZipAreaCoordinates.ToList<FiveZipAreaCoordinate>();
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

                        List<FiveZipAreaCoordinate> parentList = shapes[i];
                        FiveZipArea parent = parentList[0].FiveZipArea;
                        double pminLat = parent.MinLatitude;
                        double pmaxLat = parent.MaxLatitude;
                        double pminLon = parent.MinLongitude;
                        double pmaxLon = parent.MaxLongitude;

                        if (parent.IsInnerShape != null && parent.IsInnerShape != 0)   // already innershape
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
                                List<FiveZipAreaCoordinate> childList = shapes[j];
                                FiveZipArea child = childList[0].FiveZipArea;
                                //double cminLat = child.MinLatitude;
                                //double cmaxLat = child.MaxLatitude;
                                //double cminLon = child.MinLongitude;
                                //double cmaxLon = child.MaxLongitude;


                                if (child.IsInnerShape != null && child.IsInnerShape != 0)
                                {
                                    continue;
                                }


                                List<FiveZipAreaCoordinate> finalchildList = new List<FiveZipAreaCoordinate>();
                                for (int m = 0; m < childList.Count; m++)
                                {
                                    finalchildList.Add(childList[m]);

                                    if (m + 1 < childList.Count)
                                    {
                                        FiveZipAreaCoordinate coord = new FiveZipAreaCoordinate();
                                        coord.Latitude = (childList[m].Latitude + childList[m + 1].Latitude) / 2;
                                        coord.Longitude = (childList[m].Longitude + childList[m + 1].Longitude) / 2;
                                        finalchildList.Add(coord);
                                    }

                                }


                                //if (cmaxLat <= pmaxLat && cminLat >= pminLat && cmaxLon <= pmaxLon && cminLon >= pminLon)
                                //{

                                if (InnerShape(oPoly, finalchildList))
                                {
                                    child.IsInnerShape = parentList[0].FiveZipAreaId;
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


        private bool InnerShape(Types.Poly oPoly, List<FiveZipAreaCoordinate> innerCoordinates)
        {
            bool inner = true;
            foreach (FiveZipAreaCoordinate coordinate in innerCoordinates)
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

    }

}
