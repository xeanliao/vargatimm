using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Utilities.Boundary
{
    public class BoundaryFinder
    {
        /**
         * @param data is a list of small polygons
         * @return List of bigger polygons
        */
        public static List<List<Types.Loc>> find2(List<double[][]> data)
        {
            List<List<Types.Loc>> ret = new List<List<Types.Loc>>();
            //Edge to polygon map
            Dictionary<Types.Edge, List<Types.Poly>> edge2polysMap = new Dictionary<Types.Edge, List<Types.Poly>>();
            List<Types.Poly> originalPolys = new List<Types.Poly>();
            //Calibrate the data structure
            foreach (double[][] polyPoints in data)
            {
                Types.Poly poly = new Types.Poly(polyPoints);
                originalPolys.Add(poly);
            }

            adjustPolygons(originalPolys);

            //Populate the edge to polygon map
            foreach (Types.Poly poly in originalPolys)
            {
                updatepolysMap(poly, edge2polysMap);
            }

            Dictionary<Types.Loc, List<Types.Loc>> edgeMap = new Dictionary<Types.Loc, List<Types.Loc>>();
            //First step: find all the edges and put the end points in the map
            //The key is one end of an edge, the value is the list of vertices of the other end for the edges
            foreach (Types.Edge edge in edge2polysMap.Keys)
            {
                List<Types.Poly> polys = edge2polysMap.Keys.Contains(edge) ? edge2polysMap[edge] : null;
                if (polys != null && polys.Count == 1)
                {
                    Types.Loc from = edge.getFrom();
                    Types.Loc to = edge.getTo();
                    List<Types.Loc> fromEnds = edgeMap.Keys.Contains(from) ? edgeMap[from] : null;
                    if (fromEnds == null)
                    {
                        fromEnds = new List<Types.Loc>();
                        edgeMap.Add(from, fromEnds);
                    }
                    fromEnds.Add(to);
                    List<Types.Loc> toEnds = edgeMap.Keys.Contains(to) ? edgeMap[to] : null;
                    if (toEnds == null)
                    {
                        toEnds = new List<Types.Loc>();
                        edgeMap.Add(to, toEnds);
                    }
                    toEnds.Add(from);
                }
            }

            //Second step: traverse the vertices in the sequence of adjacent edges
            List<Types.Loc> polyList = null;

            Types.Poly currentPoly = null;
            while (edgeMap.Count > 0)
            {
                Types.Loc from = edgeMap.Keys.First();
                polyList = new List<Types.Loc>();
                while (from != null)
                {
                    //Add the vertex in the list of the polygon's boundary list
                    polyList.Add(from);
                    List<Types.Loc> ends = edgeMap.Keys.Contains(from) ? edgeMap[from] : null;
                    Types.Loc to = null;
                    if (ends != null && ends.Count > 0)
                    {
                        if (currentPoly != null)
                        {
                            for (int i = 0; i < ends.Count; i++)
                            {
                                Types.Loc loc = ends[i];
                                Types.Edge edge = new Types.Edge(from, loc);
                                List<Types.Poly> polys = edge2polysMap[edge];
                                Types.Poly poly = polys.First();
                                //First traverse the node on the same polyon
                                if (poly == currentPoly)
                                {
                                    to = loc;
                                    //Remove the edge when it's traversed
                                    ends.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        //If no node on the same polygon, then select the next available node
                        if (to == null)
                        {
                            to = ends[0];
                            Types.Edge edge = new Types.Edge(from, to);
                            List<Types.Poly> polys = edge2polysMap[edge];
                            currentPoly = polys.First();
                            //Remove the edge when it's traversed
                            ends.RemoveAt(0);
                        }
                        //Also remove the edge that being keyed by the other vertex
                        List<Types.Loc> heads = edgeMap.Keys.Contains(to) ? edgeMap[to] : null;
                        if (heads != null)
                        {
                            for (int i = 0; i < heads.Count; i++)
                            {
                                if (heads[i].Equals(from))
                                {
                                    heads.RemoveAt(i);
                                    break;
                                }
                            }
                            //Remove empty list from the map
                            if (heads.Count == 0)
                            {
                                edgeMap.Remove(to);
                            }
                        }
                        if (ends.Count == 0)
                        {
                            edgeMap.Remove(from);
                        }
                    }
                    from = to;
                }
                //One boundary is found and add it to the return list
                ret.Add(polyList);
                currentPoly = null;
            }
            return ret;
        }

        private static int binSearch(List<Types.Loc> locs, double val, bool interceptX)
        {
            int pos = -1;
            int size = locs.Count();
            int first = 0;
            int end = size - 1;
            while (first <= end)
            {
                pos = first + (end - first) / 2;
                Types.Loc loc = locs[pos];
                if (interceptX)
                {
                    if (val < loc.getY())
                    {
                        end = pos - 1;
                    }
                    else if (val > loc.getY())
                    {
                        first = pos + 1;
                    }
                    else
                    {
                        return pos;
                    }
                }
                else
                {
                    if (val < loc.getX())
                    {
                        end = pos - 1;
                    }
                    else if (val > loc.getX())
                    {
                        first = pos + 1;
                    }
                    else
                    {
                        return pos;
                    }
                }
            }
            return -1;
        }

        //This method will adjust the vertices of the polygons, such that if polygon P1 has a edge landing inside
        //polygon P2, then add the two vertices of the P1's edge in P2
        /*private*/
        public static void adjustPolygons(List<Types.Poly> polys)
        {
            Dictionary<Types.Line, List<Types.Loc>> line2locsMap = new Dictionary<Types.Line, List<Types.Loc>>();
            //Create line to vertices map
            foreach (Types.Poly poly in polys)
            {
                int size = poly.size();
                for (int i = 0; i < size; i++)
                {
                    Types.Loc from = poly.getLoc(i);
                    Types.Loc to = poly.getLoc(i + 1);
                    Types.Line line = Types.Line.toLine(from, to);
                    List<Types.Loc> locs;
                    if (!line2locsMap.ContainsKey(line))
                    {
                        locs = new List<Types.Loc>();
                        line2locsMap.Add(line, locs);
                    }
                    else
                    {
                        locs = line2locsMap[line];
                    }
                    locs.Add(from);
                    locs.Add(to);
                }
            }

            foreach (Types.Line line in line2locsMap.Keys)
            {

                List<Types.Loc> locs = line2locsMap[line];
                //Sort the vertices on the line
                if (line.getAxis() == Types.Line.AXIS.X)
                {
                    locs.Sort(new Types.LocYComparer());

                }
                else
                {
                    locs.Sort(new Types.LocXComparer());
                }
            }

            foreach (Types.Poly poly in polys)
            {
                int size = poly.size();
                int count = 0;
                int pos = 0;
                //Adjust the polygons to insert the vertices inside an edge
                while (count < size)
                {
                    count++;
                    Types.Loc from = poly.getLoc(pos);
                    Types.Loc to = poly.getLoc(pos + 1);
                    Types.Line line = Types.Line.toLine(from, to);
                    List<Types.Loc> locs = line2locsMap[line];
                    int sPos = -1;
                    bool interceptX = (line.getAxis() == Types.Line.AXIS.X);
                    double fromVal = (interceptX) ? from.getY() : from.getX();
                    double toVal = (interceptX) ? to.getY() : to.getX();
                    sPos = binSearch(locs, fromVal, interceptX);
                    int delta = (fromVal < toVal) ? 1 : -1;

                    sPos += delta;
                    while (sPos >= 0 && sPos < locs.Count)
                    {
                        Types.Loc next = locs[sPos];
                        double nextVal = interceptX ? next.getY() : next.getX();
                        if (fromVal == nextVal)
                        {
                            sPos += delta;
                            continue;
                        }
                        else if ((delta == 1 && (nextVal < toVal)) ||
                            (delta == -1 && nextVal > toVal))
                        {
                            poly.insertLoc(++pos, next);
                        }
                        else
                        {
                            break;
                        }
                        sPos += delta;
                    }
                    pos++;
                }
                //Initialize the polygons to book keeping the vertex and it's location
                poly.init();
            }
        }

        private static void updatepolysMap(Types.Poly poly, Dictionary<Types.Edge, List<Types.Poly>> edge2polysMap)
        {
            int size = poly.size();
            Types.Loc cur = poly.getFirst();
            for (int i = 0; i < size; i++)
            {
                Types.Loc next = poly.getNextLocation(cur, true);
                Types.Edge edge = new Types.Edge(cur, next);
                List<Types.Poly> polys = edge2polysMap.Keys.Contains(edge) ? edge2polysMap[edge] : null;
                if (polys == null)
                {
                    polys = new List<Types.Poly>();
                    edge2polysMap.Add(edge, polys);
                }
                if (!polys.Contains(poly))
                {
                    polys.Add(poly);
                }
                cur = next;
            }
        }
    }
}
