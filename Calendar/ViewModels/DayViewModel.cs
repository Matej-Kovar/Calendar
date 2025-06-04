using System;
using System.Collections.Generic;
using System.ComponentModel;
using Calendar.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.ViewModels
{
    public class DayViewModel : INotifyPropertyChanged
    {
        private readonly DayModel _model;

        public DateTime Date => _model.Date;
        public List<DayEvent> Events => _model.Events;

        public DayViewModel(DayModel model)
        {
            _model = model;
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    OnPropertyChanged(nameof(Stroke));
                    OnPropertyChanged(nameof(StrokeThickness));
                }
            }
        }

        private bool _isToday;
        public bool IsToday
        {
            get => _isToday;
            set
            {
                if (_isToday != value)
                {
                    _isToday = value;
                    OnPropertyChanged(nameof(IsToday));
                    OnPropertyChanged(nameof(Background));
                }
            }
        }

        public bool IsCurrentMonth { get; set; }

        public Color Background =>
            IsToday ? (Color)Application.Current.Resources["PrimaryColor"] :
            Colors.Transparent;

        public double Opacity => IsCurrentMonth ? 1 : 0.5;

        public Brush Stroke => IsSelected ? (Color)Application.Current.Resources["SecondaryColor"] : Brush.Transparent;

        public double StrokeThickness => IsSelected ? 3 : 1;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
