using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Web;

namespace GPS.FileLayer
{
    public class ShpReader:IDisposable
    {
        #region property

        private static string FolderPath = HttpContext.Current.Server.MapPath("~/App_Data/");
        int filecode, filelength, version, shapetype;
        double xMin, yMin, xMax, yMax, zMin, zMax, mMin, mMax;

        public List<PointF> points;
        public struct Line
        {
            public double[] box;
            public int numParts;
            public int numPoints;
            public int[] parts;
            public PointF[] points;
        }
        public List<Line> lines;
        public struct Polygon
        {
            public double[] box;
            public int numParts;
            public int numPoints;
            public int[] parts;
            public PointF[] points;
        }
        public List<Polygon> polygons = new List<Polygon>();

        ShxReader shx;
        FileStream fsShapeFile;
        BinaryReader brShapeFile;

        #endregion

        #region structure

        public ShpReader()
        {
 
        }

        public ShpReader(string shpFileName,string shxFileName)
        {
            shx = new ShxReader(shxFileName);
            fsShapeFile = new FileStream(FolderPath + shpFileName, FileMode.Open, FileAccess.Read);
            brShapeFile = new BinaryReader(fsShapeFile);
        }

        #endregion

        #region IDisposable Members

        protected bool m_disposed = false;

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="isDisposing">the isDisposing</param>
        protected void Dispose(bool isDisposing)
        {
            if (!m_disposed)
            {
                if (isDisposing)
                {
                    // 
                    brShapeFile = null;
                    fsShapeFile.Dispose();
                }
                m_disposed = true;
            }
        }

        #endregion

        #region method

        public void readShapeFile(string fileName)
        {
            string filePath = FolderPath + fileName;
            fsShapeFile = new FileStream(filePath, FileMode.Open);
            long fileLength = fsShapeFile.Length;
            Byte[] data = new Byte[fileLength];
            fsShapeFile.Read(data, 0, (int)fileLength);
            fsShapeFile.Close();
            filecode = readIntBig(data, 0);
            filelength = readIntBig(data, 24);
            version = readIntLittle(data, 28);
            shapetype = readIntLittle(data, 32);
            xMin = readDoubleLittle(data, 36);
            yMin = readDoubleLittle(data, 44);
            yMin = 0 - yMin;
            xMax = readDoubleLittle(data, 52);
            yMax = readDoubleLittle(data, 60);
            yMax = 0 - yMax;
            zMin = readDoubleLittle(data, 68);
            zMax = readDoubleLittle(data, 76);
            mMin = readDoubleLittle(data, 84);
            mMax = readDoubleLittle(data, 92);
            int currentPosition = 100;
            while (currentPosition < fileLength)
            {
                int recordStart = currentPosition;
                int recordNumber = readIntBig(data, recordStart);
                int contentLength = readIntBig(data, recordStart + 4);
                int recordContentStart = recordStart + 8;
                if (shapetype == 1)
                {
                    PointF point = new PointF();
                    int recordShapeType = readIntLittle(data, recordContentStart);
                    point.X = (float)readDoubleLittle(data, recordContentStart + 4);
                    point.Y = 0 - (float)readDoubleLittle(data, recordContentStart + 12);
                    points.Add(point);
                }
                if (shapetype == 3)
                {
                    Line line = new Line();
                    int recordShapeType = readIntLittle(data, recordContentStart);
                    line.box = new Double[4];
                    line.box[0] = readDoubleLittle(data, recordContentStart + 4);
                    line.box[1] = readDoubleLittle(data, recordContentStart + 12);
                    line.box[2] = readDoubleLittle(data, recordContentStart + 20);
                    line.box[3] = readDoubleLittle(data, recordContentStart + 28);
                    line.numParts = readIntLittle(data, recordContentStart + 36);
                    line.parts = new int[line.numParts];
                    line.numPoints = readIntLittle(data, recordContentStart + 40);
                    line.points = new PointF[line.numPoints];
                    int partStart = recordContentStart + 44;
                    for (int i = 0; i < line.numParts; i++)
                    {
                        line.parts[i] = readIntLittle(data, partStart + i * 4);
                    }
                    int pointStart = recordContentStart + 44 + 4 * line.numParts;
                    for (int i = 0; i < line.numPoints; i++)
                    {
                        line.points[i].X = (float)readDoubleLittle(data, pointStart + (i * 16));
                        line.points[i].Y = (float)readDoubleLittle(data, pointStart + (i * 16) + 8);
                        line.points[i].Y = 0 - line.points[i].Y;
                    }
                    lines.Add(line);
                }
                if (shapetype == 5)
                {
                    Polygon polygon = new Polygon();
                    int recordShapeType = readIntLittle(data, recordContentStart);
                    polygon.box = new Double[4];
                    polygon.box[0] = readDoubleLittle(data, recordContentStart + 4);
                    polygon.box[1] = readDoubleLittle(data, recordContentStart + 12);
                    polygon.box[2] = readDoubleLittle(data, recordContentStart + 20);
                    polygon.box[3] = readDoubleLittle(data, recordContentStart + 28);
                    polygon.numParts = readIntLittle(data, recordContentStart + 36);
                    polygon.parts = new int[polygon.numParts];
                    polygon.numPoints = readIntLittle(data, recordContentStart + 40);
                    polygon.points = new PointF[polygon.numPoints];
                    int partStart = recordContentStart + 44;
                    for (int i = 0; i < polygon.numParts; i++)
                    {
                        polygon.parts[i] = readIntLittle(data, partStart + i * 4);
                    }
                    int pointStart = recordContentStart + 44 + 4 * polygon.numParts;
                    for (int i = 0; i < polygon.numPoints; i++)
                    {
                        polygon.points[i].X = (float)readDoubleLittle(data, pointStart + (i * 16));
                        polygon.points[i].Y = (float)readDoubleLittle(data, pointStart + (i * 16) + 8);
                        //polygon.points[i].Y = 0 - polygon.points[i].Y;
                    }
                    polygons.Add(polygon);
                }
                currentPosition = recordStart + (4 + contentLength) * 2;
            }
        }
        
        private int readIntBig(byte[] data, int pos)
        {
            byte[] bytes = new byte[4];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private int readIntLittle(byte[] data, int pos)
        {
            byte[] bytes = new byte[4];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            return BitConverter.ToInt32(bytes, 0);
        }

        private double readDoubleLittle(byte[] data, int pos)
        {
            byte[] bytes = new byte[8];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            bytes[4] = data[pos + 4];
            bytes[5] = data[pos + 5];
            bytes[6] = data[pos + 6];
            bytes[7] = data[pos + 7];
            return BitConverter.ToDouble(bytes, 0);
        }

        /// <summary>
        /// Reads and parses the geometry with ID 'oid' from the ShapeFile
        /// </summary>
        /// <param name="oid">Object ID</param>
        /// <returns>geometry</returns>
        public Polygon ReadGeometry(int oid)
        {
            //Skip record number and content length
            brShapeFile.BaseStream.Seek(shx.GetShapeIndex(oid) + 8, 0);
            //Shape type
            ShapeType _ShapeType = (ShapeType)brShapeFile.ReadInt32();
            if (_ShapeType == ShapeType.Null)
                return new Polygon();
            
            if (_ShapeType == ShapeType.Polygon ||
                _ShapeType == ShapeType.PolygonM ||
                _ShapeType == ShapeType.PolygonZ)
            {
                Polygon polygon = new Polygon();
                //skip min/max box
                polygon.box = new Double[4];
                polygon.box[0] = brShapeFile.ReadDouble();
                polygon.box[1] = brShapeFile.ReadDouble();
                polygon.box[2] = brShapeFile.ReadDouble();
                polygon.box[3] = brShapeFile.ReadDouble();
                polygon.numParts = brShapeFile.ReadInt32();
                polygon.parts = new int[polygon.numParts];
                polygon.numPoints = brShapeFile.ReadInt32();
                polygon.points = new PointF[polygon.numPoints];
                for (int i = 0; i < polygon.numParts; i++)
                {
                    polygon.parts[i] = brShapeFile.ReadInt32();
                }
                for (int i = 0; i < polygon.numPoints; i++)
                {
                    polygon.points[i].X = (float)brShapeFile.ReadDouble();
                    polygon.points[i].Y = (float)brShapeFile.ReadDouble();
                    //polygon.points[i].Y = 0 - polygon.points[i].Y;
                }
                return polygon;
            }
            else
                throw (new ApplicationException("Shapefile type " + _ShapeType.ToString() + " not supported"));
        }

        #endregion

        //public void ReadGeometry(List<int> IdList)
        //{
        //    foreach (int id in IdList)
        //    {
        //        ReadGeometry(id);
        //    }
        //}

        /// <summary>
        /// Reads and parses the geometry with ID 'oid' from the ShapeFile
        /// </summary>
        /// <remarks><see cref="FilterDelegate">Filtering</see> is not applied to this method</remarks>
        /// <param name="oid">Object ID</param>
        /// <returns>geometry</returns>
        //private void ReadGeometry(int oid)
        //{
        //    //Skip record number and content length
        //    brShapeFile.BaseStream.Seek(shx.GetShapeIndex(oid) + 8, 0); 
        //    //Shape type
        //    ShapeType _ShapeType = (ShapeType)brShapeFile.ReadInt32();
        //    if (_ShapeType == ShapeType.Null)
        //        return ;
        //    if (_ShapeType == ShapeType.Point || _ShapeType == ShapeType.PointM || _ShapeType == ShapeType.PointZ)
        //    {
        //        PointF point = new PointF((float)brShapeFile.ReadDouble(), (float)brShapeFile.ReadDouble());
        //        points.Add(point);
        //    }
        //    else if (_ShapeType == ShapeType.Multipoint || _ShapeType == ShapeType.MultiPointM || _ShapeType == ShapeType.MultiPointZ)
        //    {
        //        ////skip min/max box
        //        //brShapeFile.BaseStream.Seek(32 + brShapeFile.BaseStream.Position, 0); 
        //        //SharpMap.Geometries.MultiPoint feature = new SharpMap.Geometries.MultiPoint();
        //        //// get the number of points
        //        //int nPoints = brShapeFile.ReadInt32(); 
        //        //if (nPoints == 0)
        //        //    return null;
        //        //for (int i = 0; i < nPoints; i++)
        //        //    feature.Points.Add(new SharpMap.Geometries.Point(brShapeFile.ReadDouble(), brShapeFile.ReadDouble()));

        //        //return feature;
        //    }
        //    else if (_ShapeType == ShapeType.PolyLine || 
        //                _ShapeType == ShapeType.PolyLineM || 
        //                _ShapeType == ShapeType.PolyLineZ )
        //    {
        //        #region ployline
        //        //skip min/max box
        //        brShapeFile.BaseStream.Seek(32 + brShapeFile.BaseStream.Position, 0);
        //        // get number of parts (segments)
        //        int nParts = brShapeFile.ReadInt32(); 
        //        if (nParts == 0) return;
        //        // get number of points
        //        int nPoints = brShapeFile.ReadInt32();
        //        int[] segments = new int[nParts + 1];
        //        //Read in the segment indexes
        //        for (int b = 0; b < nParts; b++)
        //            segments[b] = brShapeFile.ReadInt32();
        //        //add end point
        //        segments[nParts] = nPoints;

        //        if ((int)_ShapeType % 10 == 3)
        //        {
        //            for (int LineID = 0; LineID < nParts; LineID++)
        //            {
        //                Line line = new Line();
        //                for (int i = segments[LineID]; i < segments[LineID + 1]; i++)
        //                    line.points[LineID] = new PointF(
        //                        (float)brShapeFile.ReadDouble(), 
        //                        (float)brShapeFile.ReadDouble());
        //                lines.Add(line);
        //            }
        //        }
        //        #endregion
        //    }
        //    else if (_ShapeType == ShapeType.Polygon ||
        //        _ShapeType == ShapeType.PolygonM ||
        //        _ShapeType == ShapeType.PolygonZ)
        //    {
        //        Polygon polygon = new Polygon();
        //        //skip min/max box
        //        polygon.box = new Double[4];
        //        polygon.box[0] = brShapeFile.ReadDouble();
        //        polygon.box[1] = brShapeFile.ReadDouble();
        //        polygon.box[2] = brShapeFile.ReadDouble();
        //        polygon.box[3] = brShapeFile.ReadDouble();
        //        polygon.numParts = brShapeFile.ReadInt32();
        //        polygon.parts = new int[polygon.numParts];
        //        polygon.numPoints = brShapeFile.ReadInt32();
        //        polygon.points = new PointF[polygon.numPoints];
        //        for (int i = 0; i < polygon.numParts; i++)
        //        {
        //            polygon.parts[i] = brShapeFile.ReadInt32();
        //        }
        //        for (int i = 0; i < polygon.numPoints; i++)
        //        {
        //            polygon.points[i].X = (float)brShapeFile.ReadDouble();
        //            polygon.points[i].Y = (float)brShapeFile.ReadDouble();
        //            //polygon.points[i].Y = 0 - polygon.points[i].Y;
        //        }
        //        polygons.Add(polygon);
        //    }
        //    else
        //        throw (new ApplicationException("Shapefile type " + _ShapeType.ToString() + " not supported"));
        //}
    }
}
