using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace GPS.SilverlightCampaign
{
    public class Types
    {
        //Contants to control the precision
        static readonly int SCALE = 8;
        static readonly double PRECISION = 1 / Math.Pow(10, SCALE);
        


        public class Loc
        {
            private double x;
            private double y;

            public Loc(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public double getX()
            {
                return x;
            }
            public void setX(double x)
            {
                this.x = x;
            }
            public double getY()
            {
                return y;
            }
            public void setY(double y)
            {
                this.y = y;
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj.GetType() == typeof(Loc))
                {
                    Loc loc = (Loc)obj;
                    if (loc == this)
                        return true;
                    return (loc.x == x && loc.y == y);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return x.GetHashCode() + y.GetHashCode() * 17;
            }

            public override string ToString()
            {
                return x + ", " + y;
            }
        }

        public class Poly
        {
            private List<Loc> locs;
            private Dictionary<Loc, int> loc2pos;

            public Poly(double[][] points)
            {
                Debug.Assert(points != null && points.Count() > 0 && points[0].Count() == 2);
                this.locs = new List<Loc>();
                loc2pos = new Dictionary<Loc, int>();
                int len = points.Count() - 1;
                len = (points[0][0] == points[len][0] && points[0][1] == points[len][1]) ? len - 1 : len;
                for (int i = 0; i <= len; i++)
                {
                    // modified by zhanghj remove identical point
                    if (i > 0 && points[i][0] == points[i - 1][0] && points[i][1] == points[i - 1][1])
                    {
                        continue;
                    }
                    Loc loc = new Loc(points[i][0], points[i][1]);
                    locs.Add(loc);
                }
            }

            public void init()
            {
                for (int i = 0; i < locs.Count(); i++)
                {
                    // modified by zhanghj 
                    if (loc2pos.Keys.Contains(locs[i]))
                    {
                        locs[i].setX(locs[i].getX() + 0.000001);
                    }
                    loc2pos.Add(locs[i], i);
                }
            }

            public int size()
            {
                return locs.Count;
            }

            public Loc getFirst()
            {
                return locs[0];
            }

            public Loc getLoc(int pos)
            {
                return locs[pos % this.size()];
            }

            public void insertLoc(int pos, Loc loc)
            {
                locs.Insert(pos, loc);
            }

            public Loc getNextLocation(Loc current, bool forward)
            {
                int curPos = loc2pos[current];
                int size = locs.Count;
                int pos = curPos;
                if (forward)
                {
                    if (pos < size - 1)
                        pos++;
                    else
                        pos = 0;
                }
                else
                {
                    if (pos > 0)
                    {
                        pos--;
                    }
                    else
                    {
                        pos = size - 1;
                    }
                }
                return locs[pos];
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                foreach (Types.Loc loc in locs)
                {
                    builder.Append(loc).Append("\n");
                }
                return builder.ToString();
            }
        }

        public class Edge
        {
            private Loc from;
            private Loc to;
            //Private member variables to compare the edge by using the vertices in the same order
            private Loc low;
            private Loc high;

            public Edge(Loc from, Loc to)
            {
                this.from = from;
                this.to = to;
                Line line = Line.toLine(from, to);
                if (line.getAxis() == Line.AXIS.X)
                {
                    if (from.getY() < to.getY())
                    {
                        low = from;
                        high = to;
                    }
                    else
                    {
                        low = to;
                        high = from;
                    }
                }
                else
                {
                    if (from.getX() < to.getX())
                    {
                        low = from;
                        high = to;
                    }
                    else
                    {
                        low = to;
                        high = from;
                    }
                }
            }

            public Loc getFrom()
            {
                return from;
            }
            public void setFrom(Loc from)
            {
                this.from = from;
            }
            public Loc getTo()
            {
                return to;
            }
            public void setTo(Loc to)
            {
                this.to = to;
            }
            public override bool Equals(Object arg)
            {
                if (arg == this)
                    return true;
                else if (arg != null && arg.GetType() == typeof(Edge))
                {
                    Edge edge = (Edge)arg;
                    return low.Equals(edge.low) && high.Equals(edge.high);
                }
                return false;
            }
            public override int GetHashCode()
            {
                return this.low.GetHashCode() + this.high.GetHashCode() * 859;
            }

        }

        public class Line
        {
            public enum AXIS { X, Y };

            private AXIS axis;
            private Decimal slop;
            private Decimal intercept;
            public AXIS getAxis()
            {
                return axis;
            }

            public void setAxis(AXIS axis)
            {
                this.axis = axis;
            }

            public Decimal getSlop()
            {
                return slop;
            }

            public void setSlop(Decimal slop)
            {
                //Round the value so that the hash code will be the same with in the same precision range
                this.slop = Math.Round(slop, SCALE);
            }

            public Decimal getIntercept()
            {
                return intercept;
            }

            public void setIntercept(Decimal intercept)
            {
                //Round the value so that the hash code will be the same with in the same precision range
                this.intercept = Math.Round(intercept, SCALE);
            }

            public static Line toLine(Loc from, Loc to)
            {
                double x1 = from.getX();
                double y1 = from.getY();
                double x2 = to.getX();
                double y2 = to.getY();
                double deltaX = Math.Abs(x1 - x2);
                double deltaY = Math.Abs(y1 - y2);

                Line line = new Line();
                AXIS axis = AXIS.X;
                Decimal slop = 0;
                Decimal intercept = 0;
                if (deltaX <= deltaY)
                {
                    axis = AXIS.X;
                    double s = (x1 - x2) / (y1 - y2);
                    try
                    {
                        slop = new Decimal(s);
                    }
                    catch (Exception e)
                    {
                        //Debug.Write(e.StackTrace);
                        throw e;
                    }
                    intercept = new Decimal(x2 - s * y2);
                }
                else
                {
                    axis = AXIS.Y;
                    double s = (y1 - y2) / (x1 - x2);
                    slop = new Decimal(s);
                    intercept = new Decimal(y2 - s * x2);
                }
                line.setAxis(axis);
                line.setSlop(slop);
                line.setIntercept(intercept);
                return line;
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                    return true;
                if (obj.GetType() == typeof(Line))
                {
                    Line oline = (Line)obj;
                    if (this.getAxis() == oline.getAxis() &&
                        Convert.ToDouble(Math.Abs(oline.getSlop() - this.getSlop())) < PRECISION &&
                        Convert.ToDouble(Math.Abs(oline.getIntercept() - this.getIntercept())) < PRECISION)
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (int)this.getAxis() + this.getIntercept().GetHashCode() * 251 + this.getSlop().GetHashCode() * 1291;
            }

            public override string ToString()
            {
                return this.getAxis().ToString() + ", " + this.getSlop() + ", " + this.getIntercept();
            }

        }

        public class LocXComparer : IComparer<Loc>
        {

            #region IComparer<Loc> Members

            public int Compare(Loc x, Loc y)
            {
                if (x.getX() < y.getX())
                    return -1;
                else if (x.getX() > y.getX())
                {
                    return 1;
                }
                return 0;
            }

            #endregion
        }

        public class LocYComparer : IComparer<Loc>
        {

            #region IComparer<Loc> Members

            public int Compare(Loc x, Loc y)
            {
                if (x.getY() < y.getY())
                    return -1;
                else if (x.getY() > y.getY())
                {
                    return 1;
                }
                return 0;
            }

            #endregion
        }
    }
}
