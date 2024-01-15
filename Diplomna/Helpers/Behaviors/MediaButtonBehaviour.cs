using Gu.Wpf.Media;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Diplomna.Helpers.Behaviors
{
    public class MediaButtonBehaviour : DependencyObject
    {
        public static readonly DependencyProperty MediaButtonProperty =
            DependencyProperty.RegisterAttached(
                "MediaButton",
                typeof(MediaButtonTypes),
                typeof(MediaButtonBehaviour),
                new FrameworkPropertyMetadata(MediaButtonTypes.Default, new PropertyChangedCallback(OnChangeButtonProperty)));

        public static MediaButtonTypes GetMediaButton(DependencyObject d)
        {
            return (MediaButtonTypes)d.GetValue(MediaButtonProperty);
        }

        public static void SetMediaButton(DependencyObject d, MediaButtonTypes value)
        {
            d.SetValue(MediaButtonProperty, value);
        }

        private static void OnChangeButtonProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = d as UIElement;
            if (element == null) return;
            if (e.NewValue != e.OldValue)
            {
                if ((MediaButtonTypes)e.NewValue != MediaButtonTypes.Default)
                {
                    element.PreviewMouseDown -= OnMouseClick;
                    element.PreviewMouseDown += OnMouseClick;
                }
                else
                {
                    element.PreviewMouseDown -= OnMouseClick;
                }
            }
        }

        private static void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;
            FrameworkElement parent = (FrameworkElement)VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(element));
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                if (VisualTreeHelper.GetChild(parent, i).GetType() == typeof(MediaElementWrapper))
                {
                    MediaElementWrapper me = (MediaElementWrapper) VisualTreeHelper.GetChild(parent, i);
                    switch (GetMediaButton(element))
                    {
                        case MediaButtonTypes.Play:
                            me.Play();
                            break;
                        case MediaButtonTypes.Stop:
                            me.Rewind();
                            me.Stop();
                            break;
                        case MediaButtonTypes.Pause:
                            me.PausePlayback();
                            break;
                        case MediaButtonTypes.Forward:
                            me.Position += TimeSpan.FromSeconds(5);
                            break;
                        case MediaButtonTypes.Backward:
                            me.Position -= TimeSpan.FromSeconds(5);
                            break;
                        case MediaButtonTypes.Mute:
                            me.IsMuted = !me.IsMuted;
                            break;
                    }
                    break;
                }
            }
        }
    }
}
