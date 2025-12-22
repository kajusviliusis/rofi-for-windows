using System.Windows;
using System.Windows.Input;
using winlauncher.Helpers;
using winlauncher.Models;
using winlauncher.Services;

namespace winlauncher
{
    public partial class MainWindow : Window
    {
        private List<AppEntry> _apps;
        private HotkeyManager _hotkeys;

        public MainWindow()
        {
            InitializeComponent();

            // load apps
            var scanner = new AppScanner();
            _apps = scanner.LoadStartMenuApps();
            ResultsList.ItemsSource = _apps;

            // hotkey manager
            _hotkeys = new HotkeyManager(this, 1);

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // register CTRL + Space
            _hotkeys.Register(Key.Space, 2);
        }

        protected override void OnClosed(EventArgs e)
        {
            _hotkeys.Unregister();
            base.OnClosed(e);
        }

        // UI logic

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Hide();

            else if (e.Key == Key.Enter)
                LaunchSelected();

            else if (e.Key == Key.Down)
                ResultsList.SelectedIndex =
                    Math.Min(ResultsList.SelectedIndex + 1, ResultsList.Items.Count - 1);

            else if (e.Key == Key.Up)
                ResultsList.SelectedIndex =
                    Math.Max(ResultsList.SelectedIndex - 1, 0);
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            var filtered = _apps
                .Select(a => new
                {
                    App = a,
                    Score = Fuzzy.Score(a.Name, query)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.App)
                .ToList();
            
            ResultsList.ItemsSource = filtered;

            if (filtered.Count > 0)
                ResultsList.SelectedIndex = 0;
        }

        private void LaunchSelected()
        {
            if (ResultsList.SelectedItem is AppEntry app)
            {
                try
                {
                    System.Diagnostics.Process.Start(app.Path);
                }
                catch
                {
                    MessageBox.Show("Could not launch " + app.Name);
                }
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void ShowLauncher()
        {
            this.Show();
            this.Activate();
            SearchBox.Text = "";
            SearchBox.Focus();
        }
    }
}
