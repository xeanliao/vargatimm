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
using System.Windows.Threading;
using Microsoft.Maps.MapControl;

namespace GPS.SilverMonitor
{
    public class DelayShowElement : ContentPresenter
    {
        DispatcherTimer m_Timer;

        public DelayShowElement(long ticks)
        {
            m_Timer = new DispatcherTimer();
            m_Timer.Tick += new EventHandler(m_Timer_Tick);
            this.Visibility = System.Windows.Visibility.Collapsed;
            m_Timer.Interval = new TimeSpan(ticks);
            m_Timer.Start();
        }

        void m_Timer_Tick(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            m_Timer.Stop();
            m_Timer = null;
        }

    }
}
