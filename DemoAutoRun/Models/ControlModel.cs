using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DemoAutoRun.Models
{
    /// <summary>
    /// Базовая модель для элемента управления визуализации загрузки.
    /// </summary>
    public class ControlModel : INotifyPropertyChanged
    {
        private bool _isEnabled;
        private Visibility _visibility;
        private string _status;

       public event PropertyChangedEventHandler PropertyChanged;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    OnPropertyChanged("Visibility");
                }
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

    

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
