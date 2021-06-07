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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using System.IO;
using PdfSharp.Drawing;
using System.util.zlib;
using PdfSharp.Pdf.Filters;

namespace PdfSharp.Pdf.Advanced
{
    public class PdfImage: PdfXObject
    {
        public PdfImage(PdfDocument document, XImage source_image)
            : base(document)
        {
            //maskRef = null;
            //this.name = new PdfName(name);
            this.publicImage = source_image;
            this.image = source_image.fakeImage;
            object maskRef = null;

            //Put(PdfName.TYPE, PdfName.XOBJECT);
            //Put(PdfName.SUBTYPE, PdfName.IMAGE);
            Elements.SetName(Keys.Type, "/XObject");
            Elements.SetName(Keys.Subtype, "/Image");

            //Put(PdfName.WIDTH, new PdfNumber(image.Width));
            //Put(PdfName.HEIGHT, new PdfNumber(image.Height));
            Elements[Keys.Width] = new PdfInteger((int)image.Width);
            Elements[Keys.Height] = new PdfInteger((int)image.Height);

            ////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ////if (image.Layer != null)
            ////    Put(PdfName.OC, image.Layer.Ref);
            ////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 0xff))
            //    Put(PdfName.IMAGEMASK, PdfBoolean.PDFTRUE);
            if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 0xff))
                Elements[Keys.ImageMask] = new PdfBoolean(true);

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //if (maskRef != null)
            //{
            //    if (image.Smask)
            //        Put(PdfName.SMASK, maskRef);
            //    else
            //        Put(PdfName.MASK, maskRef);
            //}
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //if (hasMask && hasAlphaMask && pdfVersion >= 14)
            //{
            //    // The image provides an alpha mask (requires Arcrobat 5.0 or higher)
            //    byte[] alphaMaskCompressed = fd.Encode(alphaMask);
            //    PdfDictionary smask = new PdfDictionary(this.document);
            //    smask.Elements.SetName(Keys.Type, "/XObject");
            //    smask.Elements.SetName(Keys.Subtype, "/Image");

            //    this.Owner.irefTable.Add(smask);
            //    smask.Stream = new PdfStream(alphaMaskCompressed, smask);
            //    smask.Elements[Keys.Length] = new PdfInteger(alphaMaskCompressed.Length);
            //    smask.Elements[Keys.Filter] = new PdfName("/FlateDecode");
            //    smask.Elements[Keys.Width] = new PdfInteger(width);
            //    smask.Elements[Keys.Height] = new PdfInteger(height);
            //    smask.Elements[Keys.BitsPerComponent] = new PdfInteger(8);
            //    smask.Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
            //    Elements[Keys.SMask] = smask.Reference;
            //}
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (image.Smask && Owner.Version >= 14)
            {
                var fd = new FlateDecode();
                var alphaMaskCompressed = fd.Encode(image.ImageMask.RawData);

                PdfDictionary smask = new PdfDictionary(this.document);
                smask.Elements.SetName(Keys.Type, "/XObject");
                smask.Elements.SetName(Keys.Subtype, "/Image");

                this.Owner.irefTable.Add(smask);
                smask.Stream = new PdfStream(alphaMaskCompressed, smask);
                smask.Elements[Keys.Length] = new PdfInteger(alphaMaskCompressed.Length);
                smask.Elements[Keys.Filter] = new PdfName("/FlateDecode");
                smask.Elements[Keys.Width] = new PdfInteger((int)image.Width);
                smask.Elements[Keys.Height] = new PdfInteger((int)image.Height);
                smask.Elements[Keys.BitsPerComponent] = new PdfInteger(8);
                smask.Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                Elements[Keys.SMask] = smask.Reference;
            }

            //if (image.IsMask() && image.Inverted)
            //    Put(PdfName.DECODE, new PdfLiteral("[1 0]"));
            if (image.IsMask() && image.Inverted)
                Elements[Keys.Decode] = new PdfLiteral("[1 0]");

            //if (image.Interpolation)
            //    Put(PdfName.INTERPOLATE, PdfBoolean.PDFTRUE);
            if (image.Interpolation)
                Elements[Keys.Interpolate] = new PdfBoolean(true);

            //Stream isp = null;
            System.IO.Stream isp = null;

            //try
            //{
            try
            {

                //    // Raw Image data
                //    if (image.IsImgRaw())
                //    {
                if (image.IsImgRaw())
                {

                    //        // will also have the CCITT parameters
                    //        int colorspace = image.Colorspace;
                    //        int[] transparency = image.Transparency;
                    int colorspace = image.Colorspace;
                    int[] transparency = image.Transparency;

                    //        if (transparency != null && !image.IsMask() && maskRef == null)
                    //        {
                    //            String s = "[";
                    //            for (int k = 0; k < transparency.Length; ++k)
                    //                s += transparency[k] + " ";
                    //            s += "]";
                    //            Put(PdfName.MASK, new PdfLiteral(s));
                    //        }
                    if (transparency != null && !image.IsMask() && maskRef == null)
                    {
                        String s = "[";
                        for (int k = 0; k < transparency.Length; ++k)
                            s += transparency[k] + " ";
                        s += "]";
                        //Put(PdfName.MASK, new PdfLiteral(s));
                        Elements[Keys.Mask] = new PdfLiteral(s);
                    }

                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //        bytes = image.RawData;
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    bytes = image.RawData;

                    //        Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                    Elements[Keys.Length] = new PdfInteger(bytes.Length);

                    //        int bpc = image.Bpc;
                    //        if (bpc > 0xff)
                    //        {
                    int bpc = image.Bpc;
                    if (bpc > 0xff)
                    {

                        //            if (!image.IsMask())
                        //                Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                        if (!image.IsMask())
                            Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");

                        //            Put(PdfName.BITSPERCOMPONENT, new PdfNumber(1));
                        Elements[Keys.BitsPerComponent] = new PdfInteger(1);

                        //            Put(PdfName.FILTER, PdfName.CCITTFAXDECODE);
                        Elements[Keys.Filter] = new PdfName("/CCITTFaxDecode");

                        //            int k = bpc - Image.CCITTG3_1D;
                        int k = bpc - iTextSharp.text.Image.CCITTG3_1D;

                        //            PdfDictionary decodeparms = new PdfDictionary();
                        PdfDictionary decodeparams = new PdfDictionary();

                        //            if (k != 0)
                        //                decodeparms.Put(PdfName.K, new PdfNumber(k));
                        if (k != 0)
                            decodeparams.Elements["/K"] = new PdfInteger(k);

                        //            if ((colorspace & Image.CCITT_BLACKIS1) != 0)
                        //                decodeparms.Put(PdfName.BLACKIS1, PdfBoolean.PDFTRUE);
                        if ((colorspace & iTextSharp.text.Image.CCITT_BLACKIS1) != 0)
                            decodeparams.Elements["/BlackIs1"] = new PdfBoolean(true);

                        //            if ((colorspace & Image.CCITT_ENCODEDBYTEALIGN) != 0)
                        //                decodeparms.Put(PdfName.ENCODEDBYTEALIGN, PdfBoolean.PDFTRUE);
                        if ((colorspace & iTextSharp.text.Image.CCITT_ENCODEDBYTEALIGN) != 0)
                            decodeparams.Elements["/EncodedByteAlign"] = new PdfBoolean(true);

                        //            if ((colorspace & Image.CCITT_ENDOFLINE) != 0)
                        //                decodeparms.Put(PdfName.ENDOFLINE, PdfBoolean.PDFTRUE);
                        if ((colorspace & iTextSharp.text.Image.CCITT_ENDOFLINE) != 0)
                            decodeparams.Elements["/EndOfLine"] = new PdfBoolean(true);

                        //            if ((colorspace & Image.CCITT_ENDOFBLOCK) != 0)
                        //                decodeparms.Put(PdfName.ENDOFBLOCK, PdfBoolean.PDFFALSE);
                        if ((colorspace & iTextSharp.text.Image.CCITT_ENDOFBLOCK) != 0)
                            decodeparams.Elements["/EndOfBlock"] = new PdfBoolean(false);

                        //            decodeparms.Put(PdfName.COLUMNS, new PdfNumber(image.Width));
                        //            decodeparms.Put(PdfName.ROWS, new PdfNumber(image.Height));
                        decodeparams.Elements["/Columns"] = new PdfInteger((int)image.Width);
                        decodeparams.Elements["/Rows"] = new PdfInteger((int)image.Height);

                        //            Put(PdfName.DECODEPARMS, decodeparms);
                        Elements[Keys.DecodeParms] = decodeparams;

                        //        }
                        //        else
                        //        {
                        //            switch (colorspace)
                        //            {
                    }
                    else
                    {
                        switch (colorspace)
                        {

                            //                case 1:
                            //                    Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                            //                    if (image.Inverted)
                            //                        Put(PdfName.DECODE, new PdfLiteral("[1 0]"));
                            //                    break;
                            case 1:
                                //Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                                Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                                if (image.Inverted)
                                    //Put(PdfName.DECODE, new PdfLiteral("[1 0]"));
                                    Elements[Keys.Decode] = new PdfLiteral("[1 0]");
                                break;


                            //                case 3:
                            //                    Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                            //                    if (image.Inverted)
                            //                        Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0]"));
                            //                    break;
                            case 3:
                                //Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                                Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
                                if (image.Inverted)
                                    //Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0]"));
                                    Elements[Keys.Decode] = new PdfLiteral("[1 0 1 0 1 0]");
                                break;


                            //                case 4:
                            //                default:
                            //                    Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                            //                    if (image.Inverted)
                            //                        Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
                            //                    break;
                            //            }
                            case 4:
                            default:
                                //Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                                Elements[Keys.ColorSpace] = new PdfName("/DeviceCMYK");
                                if (image.Inverted)
                                    //Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
                                    Elements[Keys.Decode] = new PdfLiteral("[1 0 1 0 1 0 1 0]");
                                break;
                        }

                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //            PdfDictionary additional = image.Additional;
                        //            if (additional != null)
                        //                Merge(additional);
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        if (image.Additional != null)
                        {
                            Merge(image.Additional);
                        }

                        //            if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 8))
                        //                Remove(PdfName.COLORSPACE);
                        if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 8))
                            Elements.Remove(Keys.ColorSpace);

                        //            Put(PdfName.BITSPERCOMPONENT, new PdfNumber(image.Bpc));
                        Elements[Keys.BitsPerComponent] = new PdfInteger(image.Bpc);

                        //            if (image.Deflated)
                        //                Put(PdfName.FILTER, PdfName.FLATEDECODE);
                        //            else
                        //            {
                        //                FlateCompress(image.CompressionLevel);
                        //            }
                        if (image.Deflated)
                            Elements[Keys.Filter] = new PdfName("/FlateDecode");
                        else
                            FlateCompress(image.CompressionLevel);

                        //        }
                        //        return;
                        //    }
                    }
                    return;
                }


                //    // GIF, JPEG or PNG
                //    String errorID;
                string errorID;

                //    if (image.RawData == null)
                //    {
                //        throw new Exception("!!!!");
                //        //isp = WebRequest.Create(image.Url).GetResponse().GetResponseStream();
                //        errorID = image.Url.ToString();
                //    }
                //    else
                //    {
                //        isp = new MemoryStream(image.RawData);
                //        errorID = "Byte array";
                //    }
                if (image.RawData == null)
                {
                    throw new Exception("!!!!");
                    //isp = WebRequest.Create(image.Url).GetResponse().GetResponseStream();
                    errorID = image.Url.ToString();
                }
                else
                {
                    isp = new MemoryStream(image.RawData);
                    errorID = "Byte array";
                }



                //    switch (image.Type)
                //    {
                switch (image.Type)
                {

                    //        case Image.JPEG:
                    //            Put(PdfName.FILTER, PdfName.DCTDECODE);
                    case iTextSharp.text.Image.JPEG:
                        Elements[Keys.Filter] = new PdfName("/DCTDecode");

                        //            switch (image.Colorspace)
                        //            {
                        switch (image.Colorspace)
                        {

                            //                case 1:
                            //                    Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                            //                    break;
                            case 1:
                                Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                                break;

                            //                case 3:
                            //                    Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                            //                    break;
                            case 3:
                                Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
                                break;

                            //                default:
                            //                    Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                            //                    if (image.Inverted)
                            //                    {
                            //                        Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
                            //                    }
                            //                    break;
                            //            }
                            default:
                                Elements[Keys.ColorSpace] = new PdfName("/DeviceCMYK");
                                if (image.Inverted)
                                {
                                    Elements[Keys.Decode] = new PdfLiteral("[1 0 1 0 1 0 1 0]");
                                }
                                break;
                        }

                        //            Put(PdfName.BITSPERCOMPONENT, new PdfNumber(8));
                        Elements[Keys.BitsPerComponent] = new PdfInteger(8);

                        //            if (image.RawData != null)
                        //            {
                        if (image.RawData != null)
                        {

                            //                bytes = image.RawData;
                            //                Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                            bytes = image.RawData;
                            Elements[Keys.Length] = new PdfInteger(bytes.Length);

                            //                return;
                            //            }
                            return;
                        }

                        streamBytes = new MemoryStream();
                        TransferBytes(isp, streamBytes, -1);
                        //            break;
                        /////////////////!!!!!??!?!?/////!!!?!??!?/////!!!?///!/////////////////////////////////////////////////////
                        Stream = new PdfStream(streamBytes.GetAllBytes(), this);
                        break;

                    //        case Image.JPEG2000:
                    //            Put(PdfName.FILTER, PdfName.JPXDECODE);
                    case iTextSharp.text.Image.JPEG2000:
                        Elements[Keys.Filter] = new PdfName("/JPXDecode");

                        //            if (image.Colorspace > 0)
                        //            {
                        if (image.Colorspace > 0)
                        {

                            //                switch (image.Colorspace)
                            //                {
                            switch (image.Colorspace)
                            {

                                //                    case 1:
                                //                        Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                                //                        break;
                                case 1:
                                    Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                                    break;

                                //                    case 3:
                                //                        Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                                //                        break;
                                case 3:
                                    Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
                                    break;

                                //                    default:
                                //                        Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                                //                        break;
                                default:
                                    Elements[Keys.ColorSpace] = new PdfName("/DeviceCMYK");
                                    break;

                                //                }
                                //                Put(PdfName.BITSPERCOMPONENT, new PdfNumber(image.Bpc));
                                //            }
                            }
                            Elements[Keys.BitsPerComponent] = new PdfInteger(image.Bpc);
                        }

                        //            if (image.RawData != null)
                        //            {
                        if (image.RawData != null)
                        {

                            //                bytes = image.RawData;
                            //                Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                            bytes = image.RawData;
                            Elements[Keys.Length] = new PdfInteger(bytes.Length);

                            //                return;
                            //            }
                            return;
                        }

                        streamBytes = new MemoryStream();
                        TransferBytes(isp, streamBytes, -1);
                        //            break;
                        /////////////////!!!!!??!?!?/////!!!?!??!?/////!!!?///!/////////////////////////////////////////////////////
                        Stream = new PdfStream(streamBytes.GetAllBytes(), this);
                        break;

                    //        case Image.JBIG2:
                    case iTextSharp.text.Image.JBIG2:

                        //            Put(PdfName.FILTER, PdfName.JBIG2DECODE);
                        Elements[Keys.Filter] = new PdfName("/JBIG2Decode");

                        //            Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                        Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");

                        //            Put(PdfName.BITSPERCOMPONENT, new PdfNumber(1));
                        Elements[Keys.BitsPerComponent] = new PdfInteger(1);

                        //            if (image.RawData != null)
                        //            {
                        if (image.RawData != null)
                        {

                            //                bytes = image.RawData;
                            //                Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                            bytes = image.RawData;
                            Elements[Keys.Length] = new PdfInteger(bytes.Length);

                            //                return;
                            //            }
                            return;
                        }

                        streamBytes = new MemoryStream();
                        TransferBytes(isp, streamBytes, -1);
                        //break;
                        /////////////////!!!!!??!?!?/////!!!?!??!?/////!!!?///!/////////////////////////////////////////////////////
                        Stream = new PdfStream(streamBytes.GetAllBytes(), this);
                        break;

                    //        default:
                    //            throw new IOException(errorID + " is an unknown Image format.");
                    //    }
                    default:
                        throw new IOException(errorID + " is an unknown Image format.");
                }

                /////////////////!!!!!??!?!?/////!!!?!??!?/////!!!?///!/////////////////////////////////////////////////////
                //    Put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
                
                
                Elements[Keys.Length] = new PdfInteger((int)streamBytes.Length);
                
                
                /////////////////!!!!!??!?!?/////!!!?!??!?/////!!!?///!/////////////////////////////////////////////////////

                //}
                //finally
                //{
                //    if (isp != null)
                //    {
                //        try
                //        {
                //            isp.Close();
                //        }
                //        catch
                //        {
                //            // empty on purpose
                //        }
                //    }
                //}
            }
            finally
            {
                if (isp != null)
                {
                    try
                    {
                        isp.Close();
                    }
                    catch
                    {
                        // empty on purpose
                    }
                }
            }
        }
                
        /** the content of this <CODE>PdfObject</CODE> */
        private byte[] _bytes;
        protected byte[] bytes
        {
            get
            {
                return _bytes;
            }
            set 
            {
                _bytes = value;
                Stream = new PdfStream(value, this);
            }
        }

        protected int compressionLevel = 0;
        protected MemoryStream streamBytes = null;

        private void FlateCompress(int compressionLevel)
        {
            //if (!Document.Compress)
            //    return;
            // check if the flateCompress-method has allready been
            //if (compressed)
            //{
            //    return;
            //}

            this.compressionLevel = compressionLevel;

            //if (inputStream != null)
            //{
            //    compressed = true;
            //    return;
            //}

            // check if a filter allready exists
            //PdfObject filter = PdfReader.GetPdfObject(Get(PdfName.FILTER));
            var filter = Elements[Keys.Filter];
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

            //!!!!!!!!!!!!!!!
            var l = (int)streamBytes.Length;
            streamBytes.Seek(0, SeekOrigin.Begin);
            bytes = new BinaryReader(streamBytes).ReadBytes(l);

            //Put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
            Elements[Keys.Length] = new PdfInteger(l);

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
            if (filter == null)
            {
                Elements[Keys.Filter] = new PdfName("/FlateDecode");
            }
            else
            {
                throw new Exception();
            }
            
            //compressed = true;
        }

        public void Merge(PdfDictionary other)
        {
            foreach (var key in other.Elements.Keys)
            {
                //var k = new PdfName(key);
                var t = other.Elements.GetObject(key);
                Elements[key] = t;

                
            }
            //foreach (object key in other.hashMap.Keys)
            //{
            //    hashMap[key] = other.hashMap[key];
            //}
        }

        internal const int TRANSFERSIZE = 4096;
        internal static void TransferBytes(Stream inp, Stream outp, int len)
        {
            byte[] buffer = new byte[TRANSFERSIZE];
            if (len < 0)
                len = 0x7ffffff;
            int size;
            while (len != 0)
            {
                size = inp.Read(buffer, 0, Math.Min(len, TRANSFERSIZE));
                if (size <= 0)
                    return;
                outp.Write(buffer, 0, size);
                len -= size;
            }
        }

        /// <summary>
        /// Gets the underlying XImage object.
        /// </summary>
        public XImage Image
        {
            get { return this.publicImage; }
        }
        XImage publicImage;
        iTextSharp.text.Image image;
        

        /// <summary>
        /// Common keys for all streams.
        /// </summary>
        public sealed new class Keys : PdfXObject.Keys
        {
            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes;
            /// if present, must be XObject for an image XObject.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) The type of XObject that this dictionary describes;
            /// must be Image for an image XObject.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string Subtype = "/Subtype";

            /// <summary>
            /// (Required) The width of the image, in samples.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string Width = "/Width";

            /// <summary>
            /// (Required) The height of the image, in samples.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string Height = "/Height";

            /// <summary>
            /// (Required for images, except those that use the JPXDecode filter; not allowed for image masks)
            /// The color space in which image samples are specified; it can be any type of color space except
            /// Pattern. If the image uses the JPXDecode filter, this entry is optional:
            /// • If ColorSpace is present, any color space specifications in the JPEG2000 data are ignored.
            /// • If ColorSpace is absent, the color space specifications in the JPEG2000 data are used.
            ///   The Decode array is also ignored unless ImageMask is true.
            /// </summary>
            [KeyInfo(KeyType.NameOrArray | KeyType.Required)]
            public const string ColorSpace = "/ColorSpace";

            /// <summary>
            /// (Required except for image masks and images that use the JPXDecode filter)
            /// The number of bits used to represent each color component. Only a single value may be specified;
            /// the number of bits is the same for all color components. Valid values are 1, 2, 4, 8, and 
            /// (in PDF 1.5) 16. If ImageMask is true, this entry is optional, and if specified, its value 
            /// must be 1.
            /// If the image stream uses a filter, the value of BitsPerComponent must be consistent with the 
            /// size of the data samples that the filter delivers. In particular, a CCITTFaxDecode or JBIG2Decode 
            /// filter always delivers 1-bit samples, a RunLengthDecode or DCTDecode filter delivers 8-bit samples,
            /// and an LZWDecode or FlateDecode filter delivers samples of a specified size if a predictor function
            /// is used.
            /// If the image stream uses the JPXDecode filter, this entry is optional and ignored if present.
            /// The bit depth is determined in the process of decoding the JPEG2000 image.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string BitsPerComponent = "/BitsPerComponent";

            /// <summary>
            /// (Optional; PDF 1.1) The name of a color rendering intent to be used in rendering the image.
            /// Default value: the current rendering intent in the graphics state.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Intent = "/Intent";

            /// <summary>
            /// (Optional) A flag indicating whether the image is to be treated as an image mask.
            /// If this flag is true, the value of BitsPerComponent must be 1 and Mask and ColorSpace should
            /// not be specified; unmasked areas are painted using the current nonstroking color.
            /// Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string ImageMask = "/ImageMask";

            /// <summary>
            /// (Optional except for image masks; not allowed for image masks; PDF 1.3)
            /// An image XObject defining an image mask to be applied to this image, or an array specifying 
            /// a range of colors to be applied to it as a color key mask. If ImageMask is true, this entry
            /// must not be present.
            /// </summary>
            [KeyInfo(KeyType.StreamOrArray | KeyType.Optional)]
            public const string Mask = "/Mask";

            /// <summary>
            /// (Optional) An array of numbers describing how to map image samples into the range of values
            /// appropriate for the image’s color space. If ImageMask is true, the array must be either
            /// [0 1] or [1 0]; otherwise, its length must be twice the number of color components required 
            /// by ColorSpace. If the image uses the JPXDecode filter and ImageMask is false, Decode is ignored.
            /// Default value: see “Decode Arrays”.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Decode = "/Decode";

            /// <summary>
            /// (Optional) A flag indicating whether image interpolation is to be performed. 
            /// Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string Interpolate = "/Interpolate";

            /// <summary>
            /// (Optional; PDF 1.3) An array of alternate image dictionaries for this image. The order of 
            /// elements within the array has no significance. This entry may not be present in an image 
            /// XObject that is itself an alternate image.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Alternates = "/Alternates";

            /// <summary>
            /// (Optional; PDF 1.4) A subsidiary image XObject defining a soft-mask image to be used as a 
            /// source of mask shape or mask opacity values in the transparent imaging model. The alpha 
            /// source parameter in the graphics state determines whether the mask values are interpreted as
            /// shape or opacity. If present, this entry overrides the current soft mask in the graphics state,
            /// as well as the image’s Mask entry, if any. (However, the other transparencyrelated graphics 
            /// state parameters—blend mode and alpha constant—remain in effect.) If SMask is absent, the 
            /// image has no associated soft mask (although the current soft mask in the graphics state may
            /// still apply).
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string SMask = "/SMask";

            /// <summary>
            /// (Optional for images that use the JPXDecode filter, meaningless otherwise; PDF 1.5)
            /// A code specifying how soft-mask information encoded with image samples should be used:
            /// 0 If present, encoded soft-mask image information should be ignored.
            /// 1 The image’s data stream includes encoded soft-mask values. An application can create
            ///   a soft-mask image from the information to be used as a source of mask shape or mask 
            ///   opacity in the transparency imaging model.
            /// 2 The image’s data stream includes color channels that have been preblended with a 
            ///   background; the image data also includes an opacity channel. An application can create
            ///   a soft-mask image with a Matte entry from the opacity channel information to be used as
            ///   a source of mask shape or mask opacity in the transparency model. If this entry has a 
            ///   nonzero value, SMask should not be specified.
            /// Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string SMaskInData = "/SMaskInData";

            /// <summary>
            /// (Required in PDF 1.0; optional otherwise) The name by which this image XObject is 
            /// referenced in the XObject subdictionary of the current resource dictionary.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Name = "/Name";

            /// <summary>
            /// (Required if the image is a structural content item; PDF 1.3) The integer key of the 
            /// image’s entry in the structural parent tree.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string StructParent = "/StructParent";

            /// <summary>
            /// (Optional; PDF 1.3; indirect reference preferred) The digital identifier of the image’s
            /// parent Web Capture content set.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string ID = "/ID";

            /// <summary>
            /// (Optional; PDF 1.2) An OPI version dictionary for the image. If ImageMask is true, 
            /// this entry is ignored.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string OPI = "/OPI";

            /// <summary>
            /// (Optional; PDF 1.4) A metadata stream containing metadata for the image.
            /// </summary>
            [KeyInfo(KeyType.Stream | KeyType.Optional)]
            public const string Metadata = "/Metadata";

            /// <summary>
            /// (Optional; PDF 1.5) An optional content group or optional content membership dictionary,
            /// specifying the optional content properties for this image XObject. Before the image is
            /// processed, its visibility is determined based on this entry. If it is determined to be 
            /// invisible, the entire image is skipped, as if there were no Do operator to invoke it.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string OC = "/OC";
        }
    }
}
