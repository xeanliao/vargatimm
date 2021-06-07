using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WHYTAlgorithmService.Geo
{
    //The GeoUtils provide static compulation utility methods for vertex, edges, lines and polygon
    //To test the position of a point relative to a polygon, there are 3 conditions
    //(1) The point is a vertex of a polygon:  isPolyVertex
    //(2) The point laying inside an edge of a polygon:  onPolysEdge
    //(3) The point is inside or outsite of the polygons: inPoly
    // Usually sitiation of (1) and (2) are rarely happening, so the checking of such 3 conditions are splitted 3 methods
    // It's upto the caller to decide which methods to call in order to check the point position.
    //
    class GeoUtils
    {
        //Test if a location point is a vertex of the polygons
        public static bool isPolysVertex(Types.Loc loc, Types.Poly poly)
        {
            int size = poly.size();
            for (int i = 0; i < size; i++)
            {
                Types.Loc vet = poly.getLoc(i);
                if (loc.Equals(vet))
                {
                    return true;
                }
            }
            return false;
        }

        //Test if a location point is on the edge of the polygons, the point is not an vertex of a
        //polygon
        public static bool onPolysEdge(Types.Loc loc, Types.Poly poly)
        {
            int size = poly.size();
            for (int i = 0; i < size; i++)
            {
                Types.Loc from = poly.getLoc(i);
                Types.Loc to = poly.getLoc(i + 1);
                Types.Edge edge = new Types.Edge(from, to);
                if (inEdge(loc, edge))
                {
                    return true;
                }
            }
            return false;
        }

        //Test if a point is inside a polygon, the precondition is that the point is not on any edge of the polygon
        public static bool inPoly(Types.Loc loc, Types.Poly poly)
        {
            //Find the good line pass the point and not passing through any vertices of the polygon
            Types.Line line = findGoodLine(loc, poly);
            List<Types.Edge> processedEdges = new List<Types.Edge>();
            List<Types.Loc> inters = findLinePolyIntersections(line, poly, processedEdges);
            inters.Add(loc);
            sortIntersections(line, inters);
            return (inters.IndexOf(loc) % 2) == 1;
        }

        //To test if an point is inside an edge including the case 
        //when the point is one of the endpoing of the edge
        public static bool inEdge(Types.Loc loc, Types.Edge edge)
        {
            if (!isOnline(loc, edge))
                return false;

            double a1, a2, a;
            Types.Line.AXIS axis = edge.getAxis();
            if (axis.Equals(Types.Line.AXIS.X))
            {
                a1 = edge.getFrom().getX();
                a2 = edge.getTo().getX();
                a = loc.getX();
            }
            else
            {
                a1 = edge.getFrom().getY();
                a2 = edge.getTo().getY();
                a = loc.getY();
            }

            if ((a1 < a2) ? ((a1 <= a) && (a2 >= a)) : ((a2 <= a) && (a1 >= a)))
            {
                return true;
            }
            else
            {
                return false;//return Math.Abs(a - a1) < Types.PRECISION || Math.Abs(a - a2) < Types.PRECISION;
            }
        }


        //Find the intersections of a line with a collection of polygons
        //If a polygon is in the excluded set, then ignore it
        public static List<Types.Loc> findLinePolysIntersections(Types.Line line, List<Types.Poly> polys, List<Types.Poly> excludedPolys, List<Types.Edge> ignoredEdges)
        {
            List<Types.Loc> _locs = new List<Types.Loc>();
            foreach (Types.Poly poly in polys)
            {
                if (excludedPolys == null || !excludedPolys.Contains(poly))
                {
                    _locs.AddRange(findLinePolyIntersections(line, poly, ignoredEdges));
                }
            }
            return _locs;
        }

        //Sort the intersection list on a line
        public static void sortIntersections(Types.Line line, List<Types.Loc> intersections)
        {
            if (line.getAxis().Equals(Types.Line.AXIS.X))
            {
                intersections.Sort(new Types.LocXComparer());
            }
            else
            {
                intersections.Sort(new Types.LocYComparer());
            }
        }
        
        public static Boolean isOnline(Types.Loc loc, Types.Line line)
        {
            return Math.Abs(line.compute(loc)) < Types.PRECISION2;
        }

        //Find intersection of a line and an edge
        public static List<Types.Loc> findLineEdgeIntersection(Types.Line line, Types.Edge edge)
        {
            List<Types.Loc> locs = new List<Types.Loc>();
            double a1 = line.getA(), a2 = edge.getA(),
                   b1 = line.getB(), b2 = edge.getB(),
                   c1 = line.getC(), c2 = edge.getC();
            double denorm = a1 * b2 - a2 * b1;
            if (Math.Abs(denorm) < Types.PRECISION2)
            {
                //For sigular condition, check if the two edges are overlapped
                //The two lines are parrallel lines
                if (Math.Abs(line.compute(edge.getFrom())) < Types.PRECISION2)
                {
                    //edge is on the line, so add the two ends of the edges in the intersection list
                    locs.Add(edge.getFrom());
                    locs.Add(edge.getTo());
                }
                return locs;
            }
            //For non parallel lines, compute the intersection
            double x = (c1 * b2 - c2 * b1) / denorm;
            double y = (c2 * a1 - c1 * a2) / denorm;

            Types.Loc inter = new Types.Loc(x, y);
            if (inter != null)
            {
                if (inEdge(inter, edge))
                {
                    //If the intersection is in the edge, then return it
                    locs.Add(inter);
                }
            }
            return locs;
        }

        //Find the intersections of a line with a polygon, if an edge is in the ignore list, then don't calcuate
        //the intersection with the edge
        public static List<Types.Loc> findLinePolyIntersections(Types.Line line, Types.Poly poly, List<Types.Edge> ignoredEdges)
        {
            List<Types.Loc> locs = new List<Types.Loc>();
            int size = poly.size();
            for (int pos = 0; pos < size; pos++)
            {
                Types.Loc from = poly.getLoc(pos);
                Types.Loc to = poly.getLoc(pos + 1);
                Types.Edge edge = new Types.Edge(from, to);
                if (ignoredEdges == null || !ignoredEdges.Contains(edge))
                {
                    if (ignoredEdges != null)
                        ignoredEdges.Add(edge);
                    List<Types.Loc> inters = GeoUtils.findLineEdgeIntersection(line, edge);
                    if (inters.Count > 0)
                    {
                        locs.AddRange(inters);
                    }
                }
            }
            return locs;
        }

        //Find a line starting from the given point that doesn't pass through any vertices of the polygon
        public static Types.Line findGoodLine(Types.Loc loc, Types.Poly poly)
        {
            List<Double> kList = new List<Double>();
            int size = poly.size();
            for (int i = 0; i < size; i++)
            {
                Types.Loc loc2 = poly.getLoc(i);
                Types.Edge edge = new Types.Edge(loc, loc2);
                if (edge.getAxis().Equals(Types.Line.AXIS.Y))
                {
                    double k = -edge.getB() / edge.getA();
                    kList.Add(k);
                }
            }

            kList.Add(-1d);
            kList.Add(1d);
            kList.Sort();
            int pos = 0;
            double M = 0;
            for (int i = 0; i < kList.Count - 1; i++)
            {
                double k1 = kList[i];
                double k2 = kList[i + 1];
                double tmpM = k2 - k1;
                if (tmpM > M)
                {
                    //find the max gap of two slops
                    M = tmpM;
                    pos = i;
                }
            }
            //The middle value of the two adjacent slops with max gap
            double km = (kList[pos] + kList[pos + 1]) / 2;
            Types.Loc loc1 = new Types.Loc(loc.getX() + km, loc.getY() + 1);
            return new Types.Line(loc, loc1);
        }
    }
}
