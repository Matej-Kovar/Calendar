using System.Security.Cryptography;
using Calendar.Resources.Styles;


namespace Calendar
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            LoadTheme(Application.Current.RequestedTheme);

            Application.Current.RequestedThemeChanged += (s, e) =>
            {
                LoadTheme(e.RequestedTheme);
            };
        }
        void LoadTheme(AppTheme theme)
        {
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new Calendar.Resources.Styles.Colors());
            Resources.MergedDictionaries.Add(new AppStyles());

            if (/*theme == AppTheme.Dark*/false)
            {
                Resources.MergedDictionaries.Add(new DarkColors());
            }
            else
            {
                Resources.MergedDictionaries.Add(new LightColors());
            }
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}