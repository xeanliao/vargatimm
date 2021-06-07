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
using System.Security;
using System.Collections.Generic;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents the sequence of dashes and gaps that will be applied by a <see cref="T:System.Windows.Media.Pen" />. 
    /// </summary>
    //[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public sealed class DashStyle // : Animatable//, DUCE.IResource
    {
        //internal DUCE.MultiChannelResource _duceResource;
        internal const double c_Offset = 0.0;
        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Media.DashStyle.Dashes" /> dependency property. 
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Media.DashStyle.Dashes" /> dependency property.
        /// </returns>
        public static readonly DependencyProperty DashesProperty;
        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Media.DashStyle.Offset" /> dependency property.
        /// </summary>
        /// <returns>
        /// The <see cref="P:System.Windows.Media.DashStyle.Offset" /> dependency property identifier.
        /// </returns>
        public static readonly DependencyProperty OffsetProperty;
        internal static DoubleCollection s_Dashes = new DoubleCollection();

        static DashStyle()
        {
            Type ownerType = typeof(DashStyle);
            //OffsetProperty = Animatable.RegisterProperty("Offset", typeof(double), ownerType, 0.0, new PropertyChangedCallback(DashStyle.OffsetPropertyChanged), null, true, null);
            //DashesProperty = Animatable.RegisterProperty("Dashes", typeof(DoubleCollection), ownerType, new FreezableDefaultValueFactory(DoubleCollection.Empty), new PropertyChangedCallback(DashStyle.DashesPropertyChanged), null, false, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Media.DashStyle" /> class. 
        /// </summary>
        public DashStyle()
        {
            //this._duceResource = new DUCE.MultiChannelResource();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Media.Animation.TimelineGroup" /> class with the specified <see cref="P:System.Windows.Media.DashStyle.Dashes" /> and <see cref="P:System.Windows.Media.DashStyle.Offset" />.
        /// </summary>
        /// <param name="dashes">
        /// The <see cref="P:System.Windows.Media.DashStyle.Dashes" /> of the <see cref="T:System.Windows.Media.DashStyle" />.
        /// </param>
        /// <param name="offset">
        /// The <see cref="P:System.Windows.Media.DashStyle.Offset" /> of the <see cref="T:System.Windows.Media.DashStyle" />.
        /// </param>
        public DashStyle(IEnumerable<double> dashes, double offset)
        {
            //this._duceResource = new DUCE.MultiChannelResource();
            this.Offset = offset;
            if (dashes != null)
            {
                this.Dashes = new DoubleCollection();
            }
        }

        /// <summary>
        /// Creates a modifiable clone of this <see cref="T:System.Windows.Media.DashStyle" />, making deep copies of this object's values. 
        /// </summary>
        /// <returns>
        /// A modifiable clone of the current object. The cloned object's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is false even if the source's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is true.</returns>
        //public DashStyle Clone()
        //{
        //    return (DashStyle)base.Clone();
        //}

        /// <summary>
        /// Creates a modifiable clone of this <see cref="T:System.Windows.Media.DashStyle" /> object, making deep copies of this object's current values. 
        /// </summary>
        /// <returns>
        /// A modifiable clone of the current object. The cloned object's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is false even if the source's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is true.
        /// </returns>
        //public DashStyle CloneCurrentValue()
        //{
        //    return (DashStyle)base.CloneCurrentValue();
        //}

        //protected override Freezable CreateInstanceCore()
        //{
        //    return new DashStyle();
        //}

        //private static void DashesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((DashStyle)d).PropertyChanged(DashesProperty);
        //}

        //[SecurityCritical]
        //internal unsafe void GetDashData(MIL_PEN_DATA* pData, out double[] dashArray)
        //{
        //    DoubleCollection dashes = this.Dashes;
        //    int count = 0;
        //    if (dashes != null)
        //    {
        //        count = dashes.Count;
        //    }
        //    pData.DashArraySize = (uint)(count * 8);
        //    pData.DashOffset = this.Offset;
        //    if (count > 0)
        //    {
        //        dashArray = dashes._collection.ToArray();
        //    }
        //    else
        //    {
        //        dashArray = null;
        //    }
        //}

        //private static void OffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((DashStyle)d).PropertyChanged(OffsetProperty);
        //}

        //DUCE.ResourceHandle DUCE.IResource.AddRefOnChannel(DUCE.Channel channel)
        //{
        //    using (CompositionEngineLock.Acquire())
        //    {
        //        if (this._duceResource.CreateOrAddRefOnChannel(channel, DUCE.ResourceType.TYPE_DASHSTYLE))
        //        {
        //            this.AddRefOnChannelAnimations(channel);
        //            this.UpdateResource(channel, true);
        //        }
        //        return this._duceResource.GetHandle(channel);
        //    }
        //}

        //DUCE.Channel DUCE.IResource.GetChannel(int index)
        //{
        //    return this._duceResource.GetChannel(index);
        //}

        //int DUCE.IResource.GetChannelCount()
        //{
        //    return this._duceResource.GetChannelCount();
        //}

        //DUCE.ResourceHandle DUCE.IResource.GetHandle(DUCE.Channel channel)
        //{
        //    using (CompositionEngineLock.Acquire())
        //    {
        //        return this._duceResource.GetHandle(channel);
        //    }
        //}

        //void DUCE.IResource.ReleaseOnChannel(DUCE.Channel channel)
        //{
        //    using (CompositionEngineLock.Acquire())
        //    {
        //        if (this._duceResource.ReleaseOnChannel(channel))
        //        {
        //            this.ReleaseOnChannelAnimations(channel);
        //        }
        //    }
        //}

        //[SecurityTreatAsSafe, SecurityCritical]
        //internal override unsafe void UpdateResource(DUCE.Channel channel, bool skipOnChannelCheck)
        //{
        //    if (skipOnChannelCheck || this._duceResource.IsOnChannel(channel))
        //    {
        //        DUCE.MILCMD_DASHSTYLE milcmd_dashstyle;
        //        base.UpdateResource(channel, skipOnChannelCheck);
        //        DoubleCollection dashes = this.Dashes;
        //        DUCE.ResourceHandle animationResourceHandle = base.GetAnimationResourceHandle(OffsetProperty, channel);
        //        int num2 = (dashes == null) ? 0 : dashes.Count;
        //        milcmd_dashstyle.Type = MILCMD.MilCmdDashStyle;
        //        milcmd_dashstyle.Handle = this._duceResource.GetHandle(channel);
        //        if (animationResourceHandle.IsNull)
        //        {
        //            milcmd_dashstyle.Offset = this.Offset;
        //        }
        //        milcmd_dashstyle.hOffsetAnimations = animationResourceHandle;
        //        milcmd_dashstyle.DashesSize = (uint)(8 * num2);
        //        channel.BeginCommand((byte*)&milcmd_dashstyle, sizeof(DUCE.MILCMD_DASHSTYLE), (int)milcmd_dashstyle.DashesSize);
        //        for (int i = 0; i < num2; i++)
        //        {
        //            double num3 = dashes.Internal_GetItem(i);
        //            channel.AppendCommandData((byte*)&num3, 8);
        //        }
        //        channel.EndCommand();
        //    }
        //}

        /// <summary>
        /// Gets or sets the collection of dashes and gaps in this <see cref="T:System.Windows.Media.DashStyle" />. This is a dependency property. 
        /// </summary>
        /// <returns>
        /// The collection of dashes and gaps.  The default is an empty <see cref="T:System.Windows.Media.DoubleCollection" />.
        /// </returns>
        public DoubleCollection Dashes { get; set; } 
        //public DoubleCollection Dashes
        //{
        //    get
        //    {
        //        return (DoubleCollection)base.GetValue(DashesProperty);
        //    }
        //    set
        //    {
        //        base.SetValue(DashesProperty, value);
        //    }
        //}

        //internal override int EffectiveValuesInitialSize
        //{
        //    get
        //    {
        //        return 1;
        //    }
        //}

        /// <summary>
        /// Gets or sets how far in the dash sequence the stroke will start. This is a dependency property. 
        /// </summary>
        /// <returns>
        /// The offset for the dash sequence.  The default is 0.
        /// </returns>
        public double Offset { get; set; }
        //public double Offset
        //{
        //    get
        //    {
        //        return (double)base.GetValue(OffsetProperty);
        //    }
        //    set
        //    {
        //        base.SetValueInternal(OffsetProperty, value);
        //    }
        //}
    }

 

}
