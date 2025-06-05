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
        private readonly DayModel model;
        private bool isSelected;
        private bool isToday;

        public DayViewModel(DayModel model)
        {
            this.model = model;
        }

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public DateTime Date => model.Date;
        public List<DayEventViewModel> Events => model.Events;

        #region public properties
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    OnPropertyChanged(nameof(Stroke));
                    OnPropertyChanged(nameof(StrokeThickness));
                }
            }
        }

        public bool IsToday
        {
            get => isToday;
            set
            {
                if (isToday != value)
                {
                    isToday = value;
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
        #endregion
    }
}
