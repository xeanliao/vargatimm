using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace GPS.Website.Print
{
    public class ScaleItem
    {
        private string text;
        private int width;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
    }

    public class ColorItem
    {
        private string name;
        private string colorString;
        private float min;
        private float max;
        private float r;
        private float g;
        private float b;
        private float a;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string ColorString
        {
            get { return colorString; }
            set { colorString = value; }
        }
        public float Min
        {
            get { return min; }
            set { min = value; }
        }
        public float Max
        {
            get { return max; }
            set { max = value; }
        }
        public float R
        {
            get { return r; }
            set { r = value; }
        }
        public float G
        {
            get { return g; }
            set { g = value; }
        }
        public float B
        {
            get { return b; }
            set { b = value; }
        }
        public float A
        {
            get { return a; }
            set { a = value; }
        }
    }

    public class CampaignSubMap
    {
        private string id;
        private string name;
        private string total;
        private string count;
        private string pen;
        private string mapImgUrl;
        private List<SubItem> fiveZips;
        private List<SubItem> cRoutes;
        private List<SubItem> tracts;
        private List<SubItem> blockGroups;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Total
        {
            get { return total; }
            set { total = value; }
        }
        public string Count
        {
            get { return count; }
            set { count = value; }
        }
        public string Pen
        {
            get { return pen; }
            set { pen = value; }
        }
        public string MapImgUrl
        {
            get { return mapImgUrl; }
            set { mapImgUrl = value; }
        }
        public List<SubItem> FiveZips
        {
            get { return fiveZips; }
            set { fiveZips = value; }
        }
        public List<SubItem> CRoutes
        {
            get { return cRoutes; }
            set { cRoutes = value; }
        }
        public List<SubItem> Tracts
        {
            get { return tracts; }
            set { tracts = value; }
        }
        public List<SubItem> BlockGroups
        {
            get { return blockGroups; }
            set { blockGroups = value; }
        }
    }

    public class CampaignDM
    {
        private string id;
        private string name;
        private string total;
        private string count;
        private string pen;
        private string mapImgUrl;
        private List<SubItem> fiveZips;
        private List<SubItem> cRoutes;
        private List<SubItem> tracts;
        private List<SubItem> blockGroups;
        private string nd;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Total
        {
            get { return total; }
            set { total = value; }
        }
        public string Count
        {
            get { return count; }
            set { count = value; }
        }
        public string Pen
        {
            get { return pen; }
            set { pen = value; }
        }
        public string MapImgUrl
        {
            get { return mapImgUrl; }
            set { mapImgUrl = value; }
        }
        public List<SubItem> FiveZips
        {
            get { return fiveZips; }
            set { fiveZips = value; }
        }
        public List<SubItem> CRoutes
        {
            get { return cRoutes; }
            set { cRoutes = value; }
        }
        public List<SubItem> Tracts
        {
            get { return tracts; }
            set { tracts = value; }
        }
        public List<SubItem> BlockGroups
        {
            get { return blockGroups; }
            set { blockGroups = value; }
        }
        public string Nd
        {
            get { return nd; }
            set { nd = value; }
        }
    }

    public class SubItem
    {
        private string orderId;
        private string code;
        private string total;
        private string count;
        private string pen;

        public string OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }

        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        public string Total
        {
            get { return total; }
            set { total = value; }
        }
        public string Count
        {
            get { return count; }
            set { count = value; }
        }
        public string Pen
        {
            get { return pen; }
            set { pen = value; }
        }
    }

    public struct MapPushpin
    {
        public float x;
        public float y;
        public string text;
        public string type;
    }

    public struct MapLocation
    {
        public float x;
        public float y;
        public string text;
    }

    public struct MapImage
    {
        public float x;
        public float y;
        public string path;
        public string url;
    }

    public struct MapShape
    {
        public Color lineColor;
        public Color fillColor;
        public int lineWidth;
        public Point[] points;
    }
}
