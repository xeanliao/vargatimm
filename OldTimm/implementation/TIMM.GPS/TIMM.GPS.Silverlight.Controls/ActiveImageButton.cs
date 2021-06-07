using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TIMM.GPS.Silverlight.Controls
{
    public class ActiveImageButton : Control
    {
        private bool _IsApplytemplate = false;
        private Image _ButtonImage;
        private Grid _Root;

        public static readonly DependencyProperty SourceProperty;
        public static readonly DependencyProperty IsActiveProperty;
        public event RoutedEventHandler Click;

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public void OnClick()
        {
            if (Click != null)
            {
                Click(this, new RoutedEventArgs());
            }
        }

        static ActiveImageButton()
        {
            SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource),
                typeof(ActiveImageButton), null);
            IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool),
                typeof(ActiveImageButton), null);
        }

        public ActiveImageButton()
        {
            this.DefaultStyleKey = typeof(ActiveImageButton);
        }

        public override void OnApplyTemplate()
        {
            if (!_IsApplytemplate)
            {
                base.OnApplyTemplate();
                _ButtonImage = this.GetTemplateChild("ButtonImage") as Image;
                _Root = this.GetTemplateChild("LayoutRoot") as Grid;
                _Root.MouseLeftButtonUp += new MouseButtonEventHandler(_Root_MouseLeftButtonUp);
                _Root.MouseEnter += new MouseEventHandler(_Root_MouseEnter);

                _IsApplytemplate = true;
            }

        }

        void _Root_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        void _Root_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnClick();
            //VisualStateManager.GoToState(this, "Active", false);
        }
    }
}
