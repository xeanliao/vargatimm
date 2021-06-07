using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace GPS.FileLayer
{
    public class ShxReader
    {
        public string FolderPath =
            HttpContext.Current.Server.MapPath("App_Data/");

        int FileCode,FileLength,Version; 

        /// <summary>
        /// 0- Null shape  1- Point  3-PolyLine  5-Polygon 8-MultiPoint
        /// </summary>
        int ShapeType; 
        double XMin, YMin, XMax, YMax;

        BinaryReader brShapeIndex;
        FileStream fsShapeIndex;

        /// <summary>
        /// Index file
        /// </summary>
        struct ESRI_IndexRec
        {
            public int Offset;
            public int ContentLen;
        }

        public ShxReader(){ 
            //brShapeIndex = new BinaryReader(
        }

        public ShxReader(string fileName)
        {
            fsShapeIndex = new FileStream(
                FolderPath + fileName,
                FileMode.Open,
                FileAccess.Read);
            long fileLength = fsShapeIndex.Length;
            int shapecount = (int)(fileLength - 100) / 8;
            brShapeIndex = new BinaryReader(
                fsShapeIndex,
                System.Text.Encoding.Unicode);
        }

        public void ReadShxFile(string filename)
        {
            fsShapeIndex = new FileStream(
                FolderPath + filename, 
                FileMode.Open, 
                FileAccess.Read);
            long fileLength = fsShapeIndex.Length;
            int shapecount = (int)(fileLength - 100) / 8;
            brShapeIndex = new BinaryReader(
                fsShapeIndex, 
                System.Text.Encoding.Unicode);

            int[] Offsets = ReadIndex(shapecount);

            int index0 = GetShapeIndex(0);

            int index1 = GetShapeIndex(1);

            int index2 = GetShapeIndex(2);
        }

        /// <summary>
        /// Reads the record offsets from the .shx index file 
        /// and returns the information in an array
        /// </summary>
        /// <param name="shapecount">shape total</param>
        /// <returns>index collections</returns>
        public int[] ReadIndex(int shapecount)
        {
            int[] OffsetOfRecord = new int[shapecount];
            //skip the header
            brShapeIndex.BaseStream.Seek(100, 0);  

            for (int x = 0; x < shapecount; ++x)
            {
                //Read shape data position // ibuffer);
                OffsetOfRecord[x] = 2 * SwapByteOrder(brShapeIndex.ReadInt32());
                //Skip content length
                brShapeIndex.BaseStream.Seek(brShapeIndex.BaseStream.Position + 4, 0);
            }
            return OffsetOfRecord;
        }

        /// <summary>
        /// Gets the file position of the n'th shape
        /// </summary>
        /// <param name="n">Shape ID</param>
        /// <returns></returns>
        public int GetShapeIndex(int n)
        {
            //seek to the position of the index
            brShapeIndex.BaseStream.Seek(100 + n * 8, 0);
            //Read shape data position
            return 2 * SwapByteOrder(brShapeIndex.ReadInt32());
        }

        /// <summary>
        /// Gets the file position content lenght of the n'th shape
        /// </summary>
        /// <param name="n">Shape ID</param>
        /// <returns></returns>
        public int GetShapeContentLenght(int n)
        {
            //seek to the position of the index
            brShapeIndex.BaseStream.Seek(100 + n * 8 + 4, 0);
            //Read shape data position
            return 2 * SwapByteOrder(brShapeIndex.ReadInt32());
        }

        ///<summary>
        ///Swaps the byte order of an int32
        ///</summary>
        /// <param name="i">Integer to swap</param>
        /// <returns>Byte Order swapped int32</returns>
        private int SwapByteOrder(int i)
        {
            byte[] buffer = BitConverter.GetBytes(i);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToInt32(buffer, 0);
        }

        public int ReadIndex(byte[] data, int pos)
        {
            byte[] bytes = new byte[4];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
