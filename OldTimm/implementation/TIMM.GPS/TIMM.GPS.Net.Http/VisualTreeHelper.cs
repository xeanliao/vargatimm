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

namespace TIMM.GPS.Silverlight.Utility
{
    public static class UIHelper
    {
        public static T GetElementByName<T>(this FrameworkElement element, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(element) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(element, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetElementByName<T>(element, name);
                    if (grandChild != null)
                    {
                        return grandChild;
                    }
                }
            }
            return null;
        }

        public static List<T> GetElementType<T>(this FrameworkElement element) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(element) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(element, i);

                if (child is T)
                {
                    childList.Add((T)child);
                }
                childList.AddRange(((FrameworkElement)child).GetElementType<T>());
            }
            return childList;
        }

        public static T GetDefaultOrFirstElementByType<T>(this FrameworkElement element) where T : FrameworkElement
        {
            DependencyObject child = null;
            T target = default(T);

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(element) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(element, i);

                if (child is T)
                {
                    target = child as T;
                    break;
                }
                target = ((FrameworkElement)child).GetDefaultOrFirstElementByType<T>();
                if (target != null)
                {
                    break;
                }
            }
            return target;
        }

        public static List<Control> GetChildControl(this FrameworkElement element)
        {
            DependencyObject child = null;
            List<Control> childList = new List<Control>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(element) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(element, i);

                if (child is Control)
                {
                    childList.Add(child as Control);
                }
                childList.AddRange(((FrameworkElement)child).GetChildControl());
            }
            return childList;
        }

        public static List<FrameworkElement> GetChildElement(this FrameworkElement element)
        {
            DependencyObject child = null;
            List<FrameworkElement> childList = new List<FrameworkElement>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(element) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(element, i);

                if (child is FrameworkElement)
                {
                    childList.Add(child as FrameworkElement);
                }
                childList.AddRange(((FrameworkElement)child).GetChildElement());
            }
            return childList;
        }
    }
}
