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
using System.Collections.Generic;
using System.IO;

namespace silverPDF._compatebleLayer
{
    public class Encoder
    {
        public List<BitmapFrame> Frames = new List<BitmapFrame>();

        public void Save(System.IO.MemoryStream memory)
        {
            if (Frames.Count == 0) throw new IndexOutOfRangeException("There must be at least one BitmapFrame");
            if (Frames.Count > 1) throw new NotSupportedException();

            var frame = Frames[0];

            //Decoders.AddDecoder<ImageTools.IO.Gif.GifDecoder>();
            //Encoders.AddEncoder<ImageTools.IO.Gif.GifEncoder>();

            //Decoders.AddDecoder<ImageTools.IO.Jpeg.JpegDecoder>();
            //Encoders.AddEncoder<ImageTools.IO.Jpeg.JpegEncoder>();

            //Decoders.AddDecoder<ImageTools.IO.Png.PngDecoder>();
            //Encoders.AddEncoder<ImageTools.IO.Png.PngEncoder>();

            //Decoders.AddDecoder<ImageTools.IO.Bmp.BmpDecoder>();
            //Encoders.AddEncoder<ImageTools.IO.Bmp.BmpEncoder>();

            
            //var image = new ImageTools.Image();
            //image.SetSource(frame.BitmapSource.s);

            //var encoder = new ImageTools.IO.Bmp.BmpEncoder();
            //MemoryStream s = new MemoryStream();
            //encoder.Encode(image, s);
            //s.Seek(0, SeekOrigin.Begin);

            //var r = new BinaryReader(s);

            //var length = (int)r.BaseStream.Length;
            //byte[] bytes = new byte[length];
            //r.Read(bytes, 0, length);
            //memory.Write(bytes, 0, length);
        }

        //[SecurityCritical, SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        //public virtual void Save(Stream stream)
        //{
        //    base.VerifyAccess();
        //    this.EnsureBuiltIn();
        //    this.EnsureUnmanagedEncoder();
        //    EncodeState state1 = this._encodeState;
        //    if (this._hasSaved)
        //    {
        //        throw new InvalidOperationException(SR.Get("Image_OnlyOneSave"));
        //    }
        //    if (this._frames == null)
        //    {
        //        throw new NotSupportedException(SR.Get("Image_NoFrames", null));
        //    }
        //    int count = this._frames.Count;
        //    if (count <= 0)
        //    {
        //        throw new NotSupportedException(SR.Get("Image_NoFrames", null));
        //    }
        //    IntPtr zero = IntPtr.Zero;
        //    SafeMILHandle handle = this._encoderHandle;
        //    try
        //    {
        //        zero = StreamAsIStream.IStreamFrom(stream);
        //        HRESULT.Check(UnsafeNativeMethods.WICBitmapEncoder.Initialize(handle, zero, WICBitmapEncodeCacheOption.WICBitmapEncodeNoCache));
        //        this._encodeState = EncodeState.EncoderInitialized;
        //        if (this._thumbnail != null)
        //        {
        //            SafeMILHandle wicSourceHandle = this._thumbnail.WicSourceHandle;
        //            lock (this._thumbnail.SyncObject)
        //            {
        //                HRESULT.Check(UnsafeNativeMethods.WICBitmapEncoder.SetThumbnail(handle, wicSourceHandle));
        //                this._encodeState = EncodeState.EncoderThumbnailSet;
        //            }
        //        }
        //        if ((this._palette != null) && (this._palette.Colors.Count > 0))
        //        {
        //            SafeMILHandle internalPalette = this._palette.InternalPalette;
        //            HRESULT.Check(UnsafeNativeMethods.WICBitmapEncoder.SetPalette(handle, internalPalette));
        //            this._encodeState = EncodeState.EncoderPaletteSet;
        //        }
        //        if ((this._metadata != null) && (this._metadata.GuidFormat == this.ContainerFormat))
        //        {
        //            this.EnsureMetadata(false);
        //            if (this._metadata.InternalMetadataHandle != this._metadataHandle)
        //            {
        //                PROPVARIANT propValue = new PROPVARIANT();
        //                try
        //                {
        //                    propValue.Init(this._metadata);
        //                    lock (this._metadata.SyncObject)
        //                    {
        //                        HRESULT.Check(UnsafeNativeMethods.WICMetadataQueryWriter.SetMetadataByName(this._metadataHandle, "/", ref propValue));
        //                    }
        //                }
        //                finally
        //                {
        //                    propValue.Clear();
        //                }
        //            }
        //        }
        //        for (int i = 0; i < count; i++)
        //        {
        //            SafeMILHandle ppIFramEncode = new SafeMILHandle();
        //            SafeMILHandle ppIEncoderOptions = new SafeMILHandle();
        //            HRESULT.Check(UnsafeNativeMethods.WICBitmapEncoder.CreateNewFrame(handle, out ppIFramEncode, out ppIEncoderOptions));
        //            this._encodeState = EncodeState.EncoderCreatedNewFrame;
        //            this._frameHandles.Add(ppIFramEncode);
        //            this.SaveFrame(ppIFramEncode, ppIEncoderOptions, this._frames[i]);
        //            if (!this._supportsMultipleFrames)
        //            {
        //                break;
        //            }
        //        }
        //        HRESULT.Check(UnsafeNativeMethods.WICBitmapEncoder.Commit(handle));
        //        this._encodeState = EncodeState.EncoderCommitted;
        //    }
        //    finally
        //    {
        //        UnsafeNativeMethods.MILUnknown.ReleaseInterface(ref zero);
        //    }
        //    this._hasSaved = true;
        //}


    }
}
