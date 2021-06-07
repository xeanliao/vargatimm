using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;

namespace TIMM.GPS.Net.Http
{
    public class ValidationManager
    {
        public static bool Validate(FrameworkElement element)
        {
            return new X8Validator().Validate(element);
        }


    }

    internal sealed class X8Validator
    {
        private List<Control> ErrorControls = new List<Control>();
        private List<Control> Controls = new List<Control>();



        private bool FindControlByBindingPath(string path)
        {
            var propertyName = "";
            var result = Controls.FirstOrDefault(p =>
            {
                if (p as TextBox != null)
                {
                    propertyName = "Text";
                }
                else if (p as PasswordBox != null)
                {
                    propertyName = "Password";
                }
                else if (p.GetType().Name == "Combox")
                {
                    propertyName = "SelectedValue";
                }
                else if (p as ComboBox != null)
                {
                    propertyName = "SelectedValue";
                }
                else if (p.GetType().Name == "DatePicker")
                {
                    propertyName = "SelectedDate";
                }
                else if (p.GetType().Name == "DateRange")
                {
                    propertyName = "SelectedRangeType";
                }
                else if (p.GetType().Name == "TimePicker")
                {
                    propertyName = "Value";
                }

                FrameworkElement element = (FrameworkElement)p;
                var field = element.GetType().GetFields(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                .Where(q => q.FieldType == typeof(DependencyProperty) && q.Name == (propertyName + "Property"))
                .FirstOrDefault();

                if (field != null)
                {
                    var be = element.GetBindingExpression((DependencyProperty)field.GetValue(null));
                    if (be != null)
                    {
                        return be.ParentBinding.Path.Path.Trim().ToLower() == path.Trim().ToLower();
                    }
                }
                return false;
            });

            return result != null;
        }

        private static void ForEachElement(DependencyObject root, Action<DependencyObject> action)
        {
            if (root != null)
            {
                int childCount = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < childCount; i++)
                {
                    var obj = VisualTreeHelper.GetChild(root, i);

                    action(obj);

                    ForEachElement(obj, action);
                }
            }
        }

        public bool Validate(FrameworkElement container)
        {
            Controls.Clear();
            ErrorControls.Clear();

            ForEachElement(container, p =>
            {
                var control = p as Control;
                if (control != null)
                {
                    Controls.Add(control);
                }
            });

            var entity = container.DataContext as Entity;
            container.BindingValidationError += new EventHandler<ValidationErrorEventArgs>(container_BindingValidationError);

            //var flag = Validate(entity);

            this.ErrorControls.Sort((x, y) =>
            {
                return x.TabIndex - y.TabIndex;
            });

            if (this.ErrorControls.Count > 0)
            {
                ScrollToElement(this.ErrorControls[0]);
                this.ErrorControls[0].Focus();
            }
            return false;
        }

        void container_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            var container = sender as FrameworkElement;

            var control = e.OriginalSource as Control;
            if (e.Action == ValidationErrorEventAction.Removed)
            {
                ErrorControls.Remove(control);
            }
            else if (e.Action == ValidationErrorEventAction.Added)
            {
                ErrorControls.Add(control);
            }
        }

        private static ScrollViewer GetScrollViewer(UIElement element)
        {
            DependencyObject parent = null;

            if (element != null)
            {
                parent = element;
                while ((parent = VisualTreeHelper.GetParent(parent)) != null)
                {
                    if (parent is ScrollViewer)
                    {
                        break;
                    }
                }
            }

            return parent as ScrollViewer;
        }

        private static void ScrollToElement(UIElement element)
        {
            ScrollViewer viewer;
            if (element != null && (viewer = GetScrollViewer(element)) != null)
            {
                Point point = element.TransformToVisual(viewer).Transform(new Point());
                if (viewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    viewer.ScrollToHorizontalOffset(point.X);
                }

                if (viewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    viewer.ScrollToVerticalOffset(point.Y);
                }
            }
        }
    }
}
