// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

namespace PietschSoft.VE
{
    public class Color
    {
        public Color(int r, int g, int b, double a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        #region Pre-Defined Colors

        /// <summary>
        /// Gets the pre-defined color Red.
        /// </summary>
        public static Color Red
        {
            get { return new Color(255, 0, 0, 1); }
        }

        /// <summary>
        /// Gets the pre-defined color Green.
        /// </summary>
        public static Color Green
        {
            get { return new Color(0, 255, 0, 1); }
        }

        /// <summary>
        /// Gets the pre-defined color Blue.
        /// </summary>
        public static Color Blue
        {
            get { return new Color(0, 0, 255, 1); }
        }


        #endregion

        private int _r;

        /// <summary>
        /// Specifies the red component value. Valid values range from 0 through 255.
        /// </summary>
        public int R
        {
            get { return _r; }
            set
            {
                int i = value;
                if (i < 0)
                    i = 0;
                else if (i > 255)
                    i = 255;
                _r = i;
            }
        }

        private int _g;

        /// <summary>
        /// Specifies the green component value. Valid values range from 0 through 255.
        /// </summary>
        public int G
        {
            get { return _g; }
            set
            {
                int i = value;
                if (i < 0)
                    i = 0;
                else if (i > 255)
                    i = 255;
                _g = i;
            }
        }

        private int _b;

        /// <summary>
        /// Specifies the blue component value. Valid values range from 0 through 255.
        /// </summary>
        public int B
        {
            get { return _b; }
            set
            {
                int i = value;
                if (i < 0)
                    i = 0;
                else if (i > 255)
                    i = 255;
                _b = i;
            }
        }

        private double _a;

        /// <summary>
        /// Specifies the alpha (transparency) component value. Valid values range from 0.0 through 1.0.
        /// </summary>
        public double A
        {
            get { return _a; }
            set
            {
                double i = value;
                if (i < 0)
                    i = 0;
                else if (i > 1)
                    i = 1;
                _a = i;
            }
        }
    }
}
