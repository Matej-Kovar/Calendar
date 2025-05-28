namespace Calendar
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EventCreation), typeof(EventCreation));
        }
    }
}
