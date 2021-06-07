/*
silverPDF is sponsored by Aleyant Systems (http://www.aleyant.com)

silverPDF is based on PdfSharp (http://www.pdfsharp.net) and iTextSharp (http://itextsharp.sourceforge.net)

Developers: Ai_boy (aka Oleksii Okhrymenko)

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above information and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR SPONSORS
BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

*/
using System;
using System.IO;
using System.Net;
using iTextSharp.text;
using System.Collections.Generic;
using System.util.zlib;

namespace iTextSharp.text.pdf {
    /**
    * <CODE>PdfImage</CODE> is a <CODE>PdfStream</CODE> containing an image-<CODE>Dictionary</CODE> and -stream.
    */

    public class PdfImage //: PdfStream 
    {
        
        internal const int TRANSFERSIZE = 4096;
        // membervariables
        
        /** This is the <CODE>PdfName</CODE> of the image. */
        protected PdfName name = null;
        
        // constructor
        
        /**
        * Constructs a <CODE>PdfImage</CODE>-object.
        *
        * @param image the <CODE>Image</CODE>-object
        * @param name the <CODE>PdfName</CODE> for this image
        * @throws BadPdfFormatException on error
        */
        //public PdfImage(Image image, String name, PdfIndirectReference maskRef) 
        public PdfImage(Image image, String name, PdfObject maskRef) 
        {
            maskRef = null;

            this.name = new PdfName(name);
            Put(PdfName.TYPE, PdfName.XOBJECT);
            Put(PdfName.SUBTYPE, PdfName.IMAGE);
            Put(PdfName.WIDTH, new PdfNumber(image.Width));
            Put(PdfName.HEIGHT, new PdfNumber(image.Height));
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //if (image.Layer != null)
            //    Put(PdfName.OC, image.Layer.Ref);
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 0xff))
                Put(PdfName.IMAGEMASK, PdfBoolean.PDFTRUE);
            if (maskRef != null) {
                if (image.Smask)
                    Put(PdfName.SMASK, maskRef);
                else
                    Put(PdfName.MASK, maskRef);
            }
            if (image.IsMask() && image.Inverted)
                Put(PdfName.DECODE, new PdfLiteral("[1 0]"));
            if (image.Interpolation)
                Put(PdfName.INTERPOLATE, PdfBoolean.PDFTRUE);
            Stream isp = null;
            try {
                // Raw Image data
                if (image.IsImgRaw()) {
                    // will also have the CCITT parameters
                    int colorspace = image.Colorspace;
                    int[] transparency = image.Transparency;
                    if (transparency != null && !image.IsMask() && maskRef == null) {
                        String s = "[";
                        for (int k = 0; k < transparency.Length; ++k)
                            s += transparency[k] + " ";
                        s += "]";
                        Put(PdfName.MASK, new PdfLiteral(s));
                    }
                    bytes = image.RawData;
                    Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                    int bpc = image.Bpc;
                    if (bpc > 0xff) {
                        if (!image.IsMask())
                            Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                        Put(PdfName.BITSPERCOMPONENT, new PdfNumber(1));
                        Put(PdfName.FILTER, PdfName.CCITTFAXDECODE);
                        int k = bpc - Image.CCITTG3_1D;
                        PdfDictionary decodeparms = new PdfDictionary();
                        if (k != 0)
                            decodeparms.Put(PdfName.K, new PdfNumber(k));
                        if ((colorspace & Image.CCITT_BLACKIS1) != 0)
                            decodeparms.Put(PdfName.BLACKIS1, PdfBoolean.PDFTRUE);
                        if ((colorspace & Image.CCITT_ENCODEDBYTEALIGN) != 0)
                            decodeparms.Put(PdfName.ENCODEDBYTEALIGN, PdfBoolean.PDFTRUE);
                        if ((colorspace & Image.CCITT_ENDOFLINE) != 0)
                            decodeparms.Put(PdfName.ENDOFLINE, PdfBoolean.PDFTRUE);
                        if ((colorspace & Image.CCITT_ENDOFBLOCK) != 0)
                            decodeparms.Put(PdfName.ENDOFBLOCK, PdfBoolean.PDFFALSE);
                        decodeparms.Put(PdfName.COLUMNS, new PdfNumber(image.Width));
                        decodeparms.Put(PdfName.ROWS, new PdfNumber(image.Height));
                        Put(PdfName.DECODEPARMS, decodeparms);
                    }
                    else {
                        switch (colorspace) {
                            case 1:
                                Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                                if (image.Inverted)
                                    Put(PdfName.DECODE, new PdfLiteral("[1 0]"));
                                break;
                            case 3:
                                Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                                if (image.Inverted)
                                    Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0]"));
                                break;
                            case 4:
                            default:
                                Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                                if (image.Inverted)
                                    Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
                                break;
                        }
                        PdfSharp.Pdf.PdfDictionary additional = image.Additional;
                        if (additional != null)
                            Merge(additional);
                        if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 8))
                            Remove(PdfName.COLORSPACE);
                        Put(PdfName.BITSPERCOMPONENT, new PdfNumber(image.Bpc));
                        if (image.Deflated)
                            Put(PdfName.FILTER, PdfName.FLATEDECODE);
                        else {
                            FlateCompress(image.CompressionLevel);
                        }
                    }
                    return;
                }
                
                // GIF, JPEG or PNG
                String errorID;
                if (image.RawData == null){
                    throw new Exception("!!!!");
                    //isp = WebRequest.Create(image.Url).GetResponse().GetResponseStream();
                    errorID = image.Url.ToString();
                }
                else{
                    isp = new MemoryStream(image.RawData);
                    errorID = "Byte array";
                }
                switch (image.Type) {
                    case Image.JPEG:
                        Put(PdfName.FILTER, PdfName.DCTDECODE);
                        switch (image.Colorspace) {
                            case 1:
                                Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                                break;
                            case 3:
                                Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                                break;
                            default:
                                Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                                if (image.Inverted) {
                                    Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
                                }
                                break;
                        }
                        Put(PdfName.BITSPERCOMPONENT, new PdfNumber(8));
                        if (image.RawData != null){
                            bytes = image.RawData;
                            Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                            return;
                        }
                        streamBytes = new MemoryStream();
                        TransferBytes(isp, streamBytes, -1);
                        break;
                    case Image.JPEG2000:
                        Put(PdfName.FILTER, PdfName.JPXDECODE);
                        if (image.Colorspace > 0) {
                            switch (image.Colorspace) {
                                case 1:
                                    Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                                    break;
                                case 3:
                                    Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                                    break;
                                default:
                                    Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                                    break;
                            }
                            Put(PdfName.BITSPERCOMPONENT, new PdfNumber(image.Bpc));
                        }
                        if (image.RawData != null){
                            bytes = image.RawData;
                            Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                            return;
                        }
                        streamBytes = new MemoryStream();
                        TransferBytes(isp, streamBytes, -1);
                        break;
                    case Image.JBIG2:
                        Put(PdfName.FILTER, PdfName.JBIG2DECODE);
                        Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                        Put(PdfName.BITSPERCOMPONENT, new PdfNumber(1));
                        if (image.RawData != null){
                            bytes = image.RawData;
                            Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                            return;
                        }
                        streamBytes = new MemoryStream();
                        TransferBytes(isp, streamBytes, -1);
                        break;
                    default:
                        throw new IOException(errorID + " is an unknown Image format.");
                }
                Put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
            }
            finally {
                if (isp != null) {
                    try{
                        isp.Close();
                    }
                    catch  {
                        // empty on purpose
                    }
                }
            }
        }

        /** the content of this <CODE>PdfObject</CODE> */
        protected byte[] bytes;

        /** This is the hashmap that contains all the values and keys of the dictionary */
        protected internal Dictionary<object, object> hashMap;

        /** Adds a <CODE>PdfObject</CODE> and its key to the <CODE>PdfDictionary</CODE>.
         * If the value is <CODE>null</CODE> or <CODE>PdfNull</CODE> the key is deleted.*/
        public void Put(PdfName key, PdfObject value)
        {
            if (value == null || value.IsNull())
                hashMap.Remove(key);
            else
                hashMap[key] = value;
        }

        public void Merge(PdfSharp.Pdf.PdfDictionary other)
        {
            //foreach (object key in other.hashMap.Keys)
            //{
            //    hashMap[key] = other.hashMap[key];
            //}
        }
        public void Remove(PdfName key)
        {
            hashMap.Remove(key);
        }

        protected int compressionLevel = NO_COMPRESSION;

        protected MemoryStream streamBytes = null;

        /** is the stream compressed? */
        protected bool compressed = false;
        protected Stream inputStream;
        protected int inputStreamLength = -1;

        /**
        * Compresses the stream.
        * @param compressionLevel the compression level (0 = best speed, 9 = best compression, -1 is default)
        * @since   2.1.3
        */
        public void FlateCompress(int compressionLevel)
        {
            //if (!Document.Compress)
            //    return;
            // check if the flateCompress-method has allready been
            if (compressed)
            {
                return;
            }
            this.compressionLevel = compressionLevel;
            if (inputStream != null)
            {
                compressed = true;
                return;
            }
            // check if a filter allready exists
            //PdfObject filter = PdfReader.GetPdfObject(Get(PdfName.FILTER));
            //if (filter != null)
            //{
            //    if (filter.IsName())
            //    {
            //        if (PdfName.FLATEDECODE.Equals(filter))
            //            return;
            //    }
            //    else if (filter.IsArray())
            //    {
            //        if (((PdfArray)filter).Contains(PdfName.FLATEDECODE))
            //            return;
            //    }
            //    else
            //    {
            //        throw new Exception("Stream could not be compressed: filter is not a name or array.");
            //    }
            //}
            // compress
            MemoryStream stream = new MemoryStream();
            ZDeflaterOutputStream zip = new ZDeflaterOutputStream(stream, compressionLevel);
            if (streamBytes != null)
                streamBytes.WriteTo(zip);
            else
                zip.Write(bytes, 0, bytes.Length);
            //zip.Close();
            zip.Finish();
            // update the object
            streamBytes = stream;
            bytes = null;
            Put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //if (filter == null)
            //{
            //    Put(PdfName.FILTER, PdfName.FLATEDECODE);
            //}
            //else
            //{
            //    PdfArray filters = new PdfArray(filter);
            //    filters.Add(PdfName.FLATEDECODE);
            //    Put(PdfName.FILTER, filters);
            //}
            compressed = true;
        }

        /**
        * Returns the <CODE>PdfName</CODE> of the image.
        */
        public PdfName Name {
            get {
                return name;
            }
        }
        
        internal static void TransferBytes(Stream inp, Stream outp, int len) {
            byte[] buffer = new byte[TRANSFERSIZE];
            if (len < 0)
                len = 0x7ffffff;
            int size;
            while (len != 0) {
                size = inp.Read(buffer, 0, Math.Min(len, TRANSFERSIZE));
                if (size <= 0)
                    return;
                outp.Write(buffer, 0, size);
                len -= size;
            }
        }

        /**
        * A possible compression level.
        * @since   2.1.3
        */
        public const int DEFAULT_COMPRESSION = -1;
        /**
        * A possible compression level.
        * @since   2.1.3
        */
        public const int NO_COMPRESSION = 0;
        /**
        * A possible compression level.
        * @since   2.1.3
        */
        public const int BEST_SPEED = 1;
        /**
        * A possible compression level.
        * @since   2.1.3
        */
        public const int BEST_COMPRESSION = 9;   

        //protected void ImportAll(PdfImage dup) {
        //    name = dup.name;
        //    compressed = dup.compressed;
        //    compressionLevel = dup.compressionLevel;
        //    streamBytes = dup.streamBytes;
        //    bytes = dup.bytes;
        //    hashMap = dup.hashMap;
        //}
    }
}