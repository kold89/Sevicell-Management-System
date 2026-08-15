using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public enum EToastType { Success, Error, Warning, Info }
    public class ToastNotification
    {
        public string Message { get; set; }
        public EToastType Type { get; set; }

        public string Icon => Type switch
        {
            EToastType.Success => "✓",
            EToastType.Error => "✕",
            EToastType.Warning => "⚠",
            EToastType.Info => "ℹ",
            _ => ""
        };

        public string BackgroundColor => Type switch
        {
            EToastType.Success => "#3B6D11",
            EToastType.Error => "#DC2626",
            EToastType.Warning => "#D97706",
            EToastType.Info => "#2563EB",
            _ => "#333333"
        };
    }
}
