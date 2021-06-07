using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WHYTAlgorithmService.Geo
{
    public class Types
    {
        //Contants to control the precision
        static readonly int SCALE = 6;
        public static readonly double PRECISION = 1 / Math.Pow(10, SCALE);
        public static readonly double PRECISION2 = PRECISION * PRECISION;

        public class Loc
        {
            private double x;
            private double y;
            //private Decimal X;
            //private Decimal Y;
            private double X;
            private double Y;

            public Loc(double x, double y)
            {
                this.x = x;
                this.y = y;
                this.X = round(x);//Math.Round(new Decimal(x), SCALE, MidpointRounding.AwayFromZero);
                this.Y = round(y);// Math.Round(new Decimal(y), SCALE, MidpointRounding.AwayFromZero);
            }

            public double getX()
            {
                return x;
            }
            public double getY()
            {
                return y;
            }


            public double dist(Loc other)
            {
                double dx = getX() - other.getX();
                double dy = getY() - other.getY();
                return Math.Max(Math.Abs(dx), Math.Abs(dy));
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj.GetType() == typeof(Loc))
                {
                    Loc loc = (Loc)obj;
                    if (loc == this)
                        return true;
                    return (loc.X.Equals(X) && loc.Y.Equals(Y));
                }
                return false;
            }

            public override int GetHashCode()
            {
                return X.GetHashCode() + Y.GetHashCode() * 1249;
            }

            public override string ToString()
            {
                return x + ", " + y;
            }
        }

        public class Poly
        {
            private List<Loc> _locs;

            public Poly(double[][] points)
            {
                this._locs = new List<Loc>();
                int len = points.Count() - 1;
                len = (points[0][0] == points[len][0] && points[0][1] == points[len][1]) ? len - 1 : len;
                for (int i = 0; i <= len; i++)
                {
                    Loc loc = new Loc(points[i][0], points[i][1]);
                    _locs.Add(loc);
                }

            }

            public Poly(List<Loc> locs)
            {
                _locs = locs;
            }

            public int size()
            {
                return _locs.Count;
            }

            public Loc getLoc(int pos)
            {
                return _locs[pos % this.size()];
            }

            public void insertLoc(int pos, Loc loc)
            {
                _locs.Insert(pos, loc);
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                foreach (Types.Loc loc in _locs)
                {
                    builder.Append(loc).Append("\n");
                }
                return builder.ToString();
            }

            public List<Loc> getLocs()
            {
                return _locs;
            }

        }

        public class Edge : Line
        {
            private Loc from;
            private Loc to;
            //Private member variables to compare the edge by using the vertices in the same order
            private Loc low;
            private Loc high;

            public Edge(Loc from, Loc to)
                : base(from, to)
            {
                this.from = from;
                this.to = to;
                if (this.getAxis().Equals(Line.AXIS.X))
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
                else
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
            }

            public Loc getFrom()
            {
                return from;
            }
            public Loc getTo()
            {
                return to;
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

            public String toString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("[").Append(from).Append("]\n[").Append(to).Append("]");
                return builder.ToString();
            }

        }

        public class Line
        {
            public enum AXIS { X, Y };

            private AXIS axis;
            private double a;
            private double b;
            private double c;

            public Line(Types.Loc from, Types.Loc to)
            {
                init(from, to);
            }

            public AXIS getAxis()
            {
                return axis;
            }

            public void setAxis(AXIS axis)
            {
                this.axis = axis;
            }
            public double getA()
            {
                return a;
            }
            public double getB()
            {
                return b;
            }
            public double getC()
            {
                return c;
            }

            public double compute(double x, double y)
            {
                return a * x + b * y - c;
            }

            public double compute(Types.Loc loc)
            {
                return this.compute(loc.getX(), loc.getY());
            }

            private void init(Loc from, Loc to)
            {
                this.a = to.getY() - from.getY();
                this.b = from.getX() - to.getX();
                this.c = from.getX() * to.getY() - from.getY() * to.getX();
                this.axis = (Math.Abs(b) <= Math.Abs(a)) ? Line.AXIS.Y : Line.AXIS.X;
            }

            public override string ToString()
            {
                return this.getAxis().ToString() + ", " + this.getA() + "X " + this.getB() + "Y = " + this.getC();
            }

        }

        private static double round(double x)
        {
            return ((long)(x / PRECISION + 0.5)) * PRECISION;
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
