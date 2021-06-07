﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Web;

namespace GPS.Tool.Data
{
    public class ShpReader:IDisposable
    {
        int filecode, filelength, version, shapetype;
        public double xMin, yMin, xMax, yMax, zMin, zMax, mMin, mMax;
        public int x1, y1, x2, y2;
        public int offsetX = 0;
        public int offsetY = 0;
        public bool down = false;
        FileStream fsShapeFile;
        BinaryReader brShapeFile;

        public List<PointF> points;
        public struct Line
        {
            public double[] box;
            public int numParts;
            public int numPoints;
            public int[] parts;
            public PointF[] points;
        }
        public List<ShpReader.Line> lines;
        public struct Polygon
        {
            /// <summary>
            /// Bounding Box --- Xmin,Ymin,Xmax,Ymax
            /// </summary>
            public double[] box;
            /// <summary>
            /// Integer Parts Number
            /// </summary>
            public int numParts;
            /// <summary>
            /// point total
            /// </summary>
            public int numPoints;
            /// <summary>
            /// 
            /// </summary>
            public int[] parts;
            /// <summary>
            /// ALL point of all parts
            /// </summary>
            public PointF[] points;
        }
        public List<Polygon> polygons;

        public ShpReader()
        {
            points = new List<PointF>();
            lines = new List<Line>();
            polygons = new List<Polygon>();
        }

        public void readShapeFile(string FolderPath, string fileName)
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

        public int readIntBig(byte[] data, int pos)
        {
            byte[] bytes = new byte[4];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public int readIntLittle(byte[] data, int pos)
        {
            byte[] bytes = new byte[4];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            return BitConverter.ToInt32(bytes, 0);
        }

        public double readDoubleLittle(byte[] data, int pos)
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

        public void ShowShape(string filePath)
        {
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
                        polygon.points[i].Y = 0 - polygon.points[i].Y;
                    }
                    polygons.Add(polygon);
                }
                currentPosition = recordStart + (4 + contentLength) * 2;
            }
        }

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
                    fsShapeFile.Dispose();
                }
                m_disposed = true;
            }
        }

        #endregion
    }
}
