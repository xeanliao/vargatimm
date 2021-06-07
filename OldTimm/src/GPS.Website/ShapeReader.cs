using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Web;
using GPS.Map;

namespace GPS.Website
{
    public class ShapeReader
    {
        #region Property

        int filecode, filelength, version, shapetype;
        double xMin, yMin, xMax, yMax, zMin, zMax, mMin, mMax;

        private string FileName;
        
        public List<PointF> points { get; set; }

        public List<GPSPolygon> polygons { get; set; }

        #endregion

        public ShapeReader(string pFileName)
        {
            this.FileName = pFileName;
            polygons = new List<GPSPolygon>();
        }        

        /// <summary>
        /// Read .shp file
        /// </summary>
        public void readShapeFile()
        {
            FileStream fs = new FileStream(FileName, FileMode.Open);
            long fileLength = fs.Length;
            Byte[] data = new Byte[fileLength];
            fs.Read(data, 0, (int)fileLength);
            fs.Close();

            ReadFileHeader(data);

            int currentPosition = 100;
            while (currentPosition < fileLength)
            {
                int recordStart = currentPosition;
                int recordNumber = readIntBig(data, recordStart);
                int contentLength = readIntBig(data, recordStart + 4);
                int recordContentStart = recordStart + 8;
                if (shapetype == 1)
                    ReadPointF(data, recordContentStart);
                if (shapetype == 3)
                    ReadLine();
                if (shapetype == 5)
                    ReadPolygon(data, recordContentStart);

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

        private void ReadPointF(Byte[] data, int recordContentStart)
        {
            PointF point = new PointF();
            int recordShapeType = readIntLittle(data, recordContentStart);
            point.X = (float)readDoubleLittle(data, recordContentStart + 4);
            point.Y = 0 - (float)readDoubleLittle(data, recordContentStart + 12);
            points.Add(point);
        }

        /// <summary>
        /// Read file Header message
        /// </summary>
        /// <param name="data">file stream</param>
        private void ReadFileHeader(Byte[] data)
        {
            filecode = readIntBig(data, 0);
            filelength = readIntBig(data, 24);
            version = readIntLittle(data, 28);
            //// 1--- point; 3---Line; 5---Polygon
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
        }

        /// <summary>
        /// Read Line shape
        /// </summary>
        private void ReadLine()
        {
            //Line line = new Line();
            //int recordShapeType = readIntLittle(data, recordContentStart);
            //line.box = new Double[4];
            //line.box[0] = readDoubleLittle(data, recordContentStart + 4);
            //line.box[1] = readDoubleLittle(data, recordContentStart + 12);
            //line.box[2] = readDoubleLittle(data, recordContentStart + 20);
            //line.box[3] = readDoubleLittle(data, recordContentStart + 28);
            //line.numParts = readIntLittle(data, recordContentStart + 36);
            //line.parts = new int[line.numParts];
            //line.numPoints = readIntLittle(data, recordContentStart + 40);
            //line.points = new PointF[line.numPoints];
            //int partStart = recordContentStart + 44;
            //for (int i = 0; i < line.numParts; i++)
            //{
            //    line.parts[i] = readIntLittle(data, partStart + i * 4);
            //}
            //int pointStart = recordContentStart + 44 + 4 * line.numParts;
            //for (int i = 0; i < line.numPoints; i++)
            //{
            //    line.points[i].X = (float)readDoubleLittle(data, pointStart + (i * 16));
            //    line.points[i].Y = (float)readDoubleLittle(data, pointStart + (i * 16) + 8);
            //    line.points[i].Y = 0 - line.points[i].Y;
            //}
            //lines.Add(line);
        }

        /// <summary>
        /// Read Polygon shape
        /// </summary>
        /// <param name="data">file stream</param>
        /// <param name="recordContentStart">Polygon record start position</param>
        private void ReadPolygon(Byte[] data, int recordContentStart)
        {
            GPSPolygon polygon = new GPSPolygon();
            int recordShapeType = readIntLittle(data, recordContentStart);
            polygon.box = new List<double>();
            polygon.box.Add(readDoubleLittle(data, recordContentStart + 4));
            polygon.box.Add(readDoubleLittle(data, recordContentStart + 12));
            polygon.box.Add(readDoubleLittle(data, recordContentStart + 20));
            polygon.box.Add(readDoubleLittle(data, recordContentStart + 28));
            polygon.numParts = readIntLittle(data, recordContentStart + 36);
            polygon.parts = new List<int>();
            polygon.numPoints = readIntLittle(data, recordContentStart + 40);
            //polygon.points = new PointF[polygon.numPoints];
            polygon.Locations = new List<PietschSoft.VE.LatLong>();
            int partStart = recordContentStart + 44;
            for (int i = 0; i < polygon.numParts; i++)
            {
                polygon.parts.Add(readIntLittle(data, partStart + i * 4));
            }
            int pointStart = recordContentStart + 44 + 4 * polygon.numParts;
            for (int i = 0; i < polygon.numPoints; i++)
            {
                PietschSoft.VE.LatLong latLong = new PietschSoft.VE.LatLong(
                    
                    (float)readDoubleLittle(data, pointStart + (i * 16) + 8),
                    (float)readDoubleLittle(data, pointStart + (i * 16))
                );
                polygon.Locations.Add(latLong);
            }
            polygons.Add(polygon);
        }
    }
}
