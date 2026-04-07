using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class permissionSelected : INotifyPropertyChanged
    {
        private bool _selected;
        public int PermisoId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Modulo { get; set; }
        public string Description { get; set; }

        public bool ItemSelected { get => _selected;
            set {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged();
                }
            } 
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
