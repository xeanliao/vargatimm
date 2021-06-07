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
using System.Globalization;

namespace TIMM.GPS.Silverlight.Controls
{
    public class PagerArgs : EventArgs
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public PagerArgs(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }

    public delegate void OnPageChanged(object sender, PagerArgs args);

    public class Pager : Control
    {
        public static readonly DependencyProperty PageIndexProperty;
        public static readonly DependencyProperty PageSizeProperty;
        public static readonly DependencyProperty TotalRecordProperty;
        public static readonly DependencyProperty PageButtonCountProperty;

        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }
        public int TotalRecord
        {
            get { return (int)GetValue(TotalRecordProperty); }
            set { SetValue(TotalRecordProperty, value); }
        }
        /// <summary>
        /// Number of buttons to render before/after current selection
        /// For example, if = 2, buttons appear for a control with 100 pages
        /// /// Button 5 selected:
        ///     Previous 1 2 3 4 (5) 6 7 ... 99 100 Next
        /// Button 6 selected:
        ///     Previous 1 2 ... 4 5 (6) 7 8 ... 99 100 Next
        /// Button 95 selected
        ///     Previous 1 2 ... 93 94 (95) 96 97 ... 99 100 Next
        /// Button 96 selected
        ///     Previous 1 2 ... 94 95 (96) 97 98 99 100 Next
        /// </summary>
        public int PageButtonCount
        {
            get { return (int)GetValue(PageButtonCountProperty); }
            set { SetValue(PageButtonCountProperty, value); }
        }

        static Pager()
        {
            PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(int), typeof(Pager), new PropertyMetadata(0, new PropertyChangedCallback(PagerPropertyChanged)));
            PageSizeProperty = DependencyProperty.Register("PageSize", typeof(int), typeof(Pager), new PropertyMetadata(50, new PropertyChangedCallback(PagerPropertyChanged)));
            TotalRecordProperty = DependencyProperty.Register("TotalRecord", typeof(int), typeof(Pager), new PropertyMetadata(0, new PropertyChangedCallback(PagerPropertyChanged)));
            PageButtonCountProperty = DependencyProperty.Register("PageButtonCount", typeof(int), typeof(Pager), new PropertyMetadata(3, new PropertyChangedCallback(PagerPropertyChanged)));
        }

        private static void PagerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Pager control = d as Pager;
            control.InitControl();
        }

        public event OnPageChanged PageChanged;

        private bool m_IsApplytemplate = false;
        private int m_PageCount = 0;
        private StackPanel m_PagerPanel;
        private TextBlock m_SearchResult;
        private Style m_InnerButtonStyle;
        private Style m_OuterButtonStyle;
        private ComboBox m_PageSizeSelector;
        private TextBox m_PageIndexSelector;
        private bool m_CallPgaeIndexLoseFoucseEvent = false;

        public Pager()
        {
            this.DefaultStyleKey = typeof(Pager);
            PageButtonCount = 3;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_PagerPanel = this.GetTemplateChild("PagerPanel") as StackPanel;
            m_SearchResult = this.GetTemplateChild("SearchResult") as TextBlock;
            m_PageSizeSelector = this.GetTemplateChild("PageSizeSelector") as ComboBox;
            m_PageIndexSelector = this.GetTemplateChild("PageIndexSelector") as TextBox;
            Grid root = this.GetTemplateChild("LayoutRoot") as Grid;
            m_InnerButtonStyle = root.Resources["PagerButtonInnerStyle"] as Style;
            m_OuterButtonStyle = root.Resources["PagerButtonOuterStyle"] as Style;

            if (m_PageSizeSelector != null)
            {
                m_PageSizeSelector.SelectionChanged += new SelectionChangedEventHandler(PageSizeChanged);
            }

            if (m_PageIndexSelector != null)
            {
                m_PageIndexSelector.KeyUp += (sender, args) =>
                {
                    if (args.Key == Key.Enter)
                    {
                        m_CallPgaeIndexLoseFoucseEvent = false;
                        PageIndexChanged();
                    }
                    else
                    {
                        m_CallPgaeIndexLoseFoucseEvent = true;
                    }
                };

                m_PageIndexSelector.LostFocus += (sender, args) =>
                {
                    if (m_CallPgaeIndexLoseFoucseEvent)
                    {
                        m_CallPgaeIndexLoseFoucseEvent = false;
                        PageIndexChanged();
                    }
                };
            }

            m_IsApplytemplate = true;

            InitControl();
        }

        /// <summary>
        /// Renders a Button control for the Pager
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="inner">Flag to denote if this refers to the inner buttons or the Previous/Next buttons</param>
        /// <param name="enabled">Flag to denote if this button is enabled</param>
        /// <returns>Button</returns>
        private Button BuildButton(string text, int pageIndex, bool inner, bool enabled)
        {
            Button b = new Button()
            {
                Content = text,
                Tag = pageIndex,
                Style = inner ? m_InnerButtonStyle : m_OuterButtonStyle,
                Width = 45
            };


            if (inner == false)
                b.Width = 75;

            b.IsEnabled = enabled;

            b.Click += new RoutedEventHandler(PagerButton_Click);

            return b;
        }

        /// <summary>
        /// Renders a selected Button or the Hellip (...) TextBlock
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="border">Flag to denote if this button is to have a border</param>
        /// <returns>UIElement (either a TextBlock or a Border with a TextBlock within)</returns>
        private UIElement BuildSpan(string text, bool border)
        {
            if (border)
            {
                TextBlock t = new TextBlock()
                {
                    Text = text,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 30
                };
                Border b = new Border()
                {
                    Margin = new Thickness(3, 3, 3, 3),
                    BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)),
                    Width = 30,
                    Height = 22,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                b.Child = t;
                return b;
            }
            else
            {
                return new TextBlock()
                {
                    Text = text,
                    Width = 30,
                    Height = 22,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
            }
        }

        private void InitControl()
        {
            if (!m_IsApplytemplate)
            {
                return;
            }

            m_PageCount = (int)Math.Ceiling((double)TotalRecord / (double)PageSize);
            if (m_SearchResult != null)
            {
                m_SearchResult.Text = string.Format("Page {0} of {1}", PageIndex + 1, m_PageCount);
            }
            if (m_PagerPanel != null)
            {
                m_PagerPanel.Children.Clear();
                if (m_PageCount <= 1)
                {
                    m_PagerPanel.Children.Add(BuildButton("Previous", PageIndex - 1, false, false));
                    m_PagerPanel.Children.Add(BuildSpan("1", false));
                    m_PagerPanel.Children.Add(BuildButton("Next", PageIndex + 1, false, false));

                }
                if (m_PageCount > 1)
                {
                    int min = PageIndex - PageButtonCount;
                    int max = PageIndex + PageButtonCount;

                    if (max > m_PageCount - 1)
                    {
                        min -= max - (m_PageCount - 1);
                    }
                    if (min < 0)
                    {
                        max += 0 - min;
                    }

                    min = Math.Max(1, min);
                    max = Math.Min(m_PageCount - 2, max);

                    // Previous Button
                    if (PageIndex > 0)
                    {
                        m_PagerPanel.Children.Add(BuildButton("Previous", PageIndex - 1, false, true));
                    }
                    else
                    {
                        m_PagerPanel.Children.Add(BuildButton("Previous", PageIndex - 1, false, false));
                    }

                    //first button
                    if (PageIndex == 0)
                    {
                        m_PagerPanel.Children.Add(BuildSpan("1", false));

                    }
                    else
                    {
                        m_PagerPanel.Children.Add(BuildButton("1", 0, true, true));
                    }

                    //left dot
                    if (min > 1)
                    {
                        m_PagerPanel.Children.Add(BuildSpan("...", false));
                    }
                    // Middle Buttons
                    for (int i = min; i <= max; i++)
                    {
                        string text = (i + 1).ToString(NumberFormatInfo.InvariantInfo);

                        if (i == PageIndex) // Currently Selected Index
                        {
                            m_PagerPanel.Children.Add(BuildSpan(text, false));
                        }
                        else
                        {
                            m_PagerPanel.Children.Add(BuildButton(text, i, true, true));
                        }
                    }

                    //right dot
                    if (max < m_PageCount - 2)
                    {
                        m_PagerPanel.Children.Add(BuildSpan("...", false));
                    }

                    //last button
                    if (PageIndex == m_PageCount - 1)
                    {
                        m_PagerPanel.Children.Add(BuildSpan((m_PageCount).ToString(), false));
                    }
                    else
                    {
                        m_PagerPanel.Children.Add(BuildButton((m_PageCount).ToString(), m_PageCount - 1, true, true));
                    }

                    // Next Button
                    if (PageIndex < m_PageCount - 1 && m_PageCount > 1)
                    {
                        m_PagerPanel.Children.Add(BuildButton("Next", PageIndex + 1, false, true));
                    }
                    else
                    {
                        m_PagerPanel.Children.Add(BuildButton("Next", PageIndex + 1, false, false));
                    }
                }
            }
        }

        private void PageIndexChanged()
        {
            string index = m_PageIndexSelector.Text;
            int pageIndex;
            if (int.TryParse(index, out pageIndex) && pageIndex <= m_PageCount && pageIndex > 0)
            {
                PageIndex = pageIndex - 1;
                OnPageChanged();
            }
            else
            {
                m_PageIndexSelector.Text = "";
            }
        }

        private void PageSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_PageSizeSelector != null)
            {
                ComboBoxItem selectedItem = m_PageSizeSelector.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    string size = selectedItem.Content as string;
                    int pageSize;
                    if (int.TryParse(size, out pageSize) && pageSize > 0)
                    {
                        PageSize = pageSize;
                        PageIndex = 0;
                        OnPageChanged();
                    }
                }
            }
        }

        private void PagerButton_Click(object sender, EventArgs arg)
        {
            Button control = sender as Button;
            PageIndex = (int)control.Tag;
            OnPageChanged();
        }

        private void OnPageChanged()
        {
            var pageIndexBinding = this.GetBindingExpression(Pager.PageIndexProperty);
            if (pageIndexBinding != null)
            {
                pageIndexBinding.UpdateSource();
            }
            var pageSizeBinding = this.GetBindingExpression(Pager.PageSizeProperty);
            if (pageSizeBinding != null)
            {
                pageSizeBinding.UpdateSource();
            }

            if (PageChanged != null)
            {
                PageChanged(this, new PagerArgs(PageIndex, PageSize));
            }
        }
    }
}
