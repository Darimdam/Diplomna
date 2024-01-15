
using System.Windows;
using System.Windows.Controls;

namespace Diplomna.View
{
    public class PasswordBoxBehaviour
    {
        #region Dependency Properties

        #region Enabled

        public static DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(PasswordBoxBehaviour), new PropertyMetadata(false, OnEnabledPropertyChanged));

        public static bool GetEnabled(DependencyObject d)
        {
            return (bool)d.GetValue(EnabledProperty);
        }

        public static void SetEnabled(DependencyObject d, bool value)
        {
            d.SetValue(EnabledProperty, value);
        }

        public static void OnEnabledPropertyChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = s as PasswordBox;
            if (pb == null) return;

            if ((bool)e.OldValue) pb.PasswordChanged -= OnPasswordBoxPasswordChanged;
            if ((bool)e.NewValue) pb.PasswordChanged += OnPasswordBoxPasswordChanged;
        }

        #endregion

        #region Password

        public static DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxBehaviour), new PropertyMetadata(null, OnPasswordPropertyChanged));

        public static string GetPassword(DependencyObject d)
        {
            return (string)d.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject d, string value)
        {
            d.SetValue(PasswordProperty, value);
        }

        public static void OnPasswordPropertyChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = s as PasswordBox;

            if (pb == null) return;
            if (!GetEnabled(pb)) return;

            pb.PasswordChanged -= OnPasswordBoxPasswordChanged;
            if (!GetUpdatingPassword(pb)) pb.Password = (string)e.NewValue;
            pb.PasswordChanged += OnPasswordBoxPasswordChanged;
        }

        #endregion

        #region UpdatingPassword

        public static DependencyProperty UpdatingPasswordProperty = DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxBehaviour));

        public static bool GetUpdatingPassword(DependencyObject d)
        {
            return (bool)d.GetValue(UpdatingPasswordProperty);
        }

        public static void SetUpdatingPassword(DependencyObject d, bool value)
        {
            d.SetValue(UpdatingPasswordProperty, value);
        }

        #endregion

        #endregion

        #region Event Handlers

        private static void OnPasswordBoxPasswordChanged(object s, RoutedEventArgs e)
        {
            PasswordBox pb = s as PasswordBox;
            if (s == null) return;

            SetUpdatingPassword(pb, true);
            SetPassword(pb, pb.Password);
            SetUpdatingPassword(pb, false);
        }

        #endregion
    }
}
