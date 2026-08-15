using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Windows.Threading;
using WpfApp1;

namespace WpfApp1.Services
{
    public static class ToastService
    {
        public static ObservableCollection<ToastNotification> Toasts { get; } = new();

        public static void ShowSuccess(string message) => Show(message, EToastType.Success);
        public static void ShowError(string message) => Show(message, EToastType.Error);
        public static void ShowWarning(string message) => Show(message, EToastType.Warning);
        public static void ShowInfo(string message) => Show(message, EToastType.Info);

        private static void Show(string message, EToastType type)
        {
            var toast = new ToastNotification { Message = message, Type = type };

            Application.Current.Dispatcher.Invoke(() =>
            {
                Toasts.Add(toast);

                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3.5) };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    Toasts.Remove(toast);
                };
                timer.Start();
            });
        }
    }
}
