using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Model
{
    public class CalendarDay : INotifyPropertyChanged
    {
        public DateTime Date { get; set; }

        public List<DayEvent> Events { get; set; } = new List<DayEvent>();  

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
            IsToday ? Color.FromRgba("#EAE2B7") :
            !IsCurrentMonth ? Colors.LightGray :
            Colors.Transparent;

        public Brush Stroke => IsSelected ? Color.FromRgba("#F3D180") : Brush.Transparent;
        public double StrokeThickness => IsSelected ? 3 : 1;

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}