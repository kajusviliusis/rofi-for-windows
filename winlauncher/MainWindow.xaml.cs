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
        private List<AppEntry> recentApps = new List<AppEntry>();
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
            {
                if(ResultsList.Items.Count>0)
                {
                    ResultsList.SelectedIndex = Math.Min(ResultsList.SelectedIndex + 1, ResultsList.Items.Count - 1);
                    ResultsList.ScrollIntoView(ResultsList.SelectedItem);
                }
                e.Handled = true;
            }

            else if (e.Key == Key.Up)
            {
                if (ResultsList.Items.Count>0)
                {
                    ResultsList.SelectedIndex = Math.Max(ResultsList.SelectedIndex - 1, 0);
                    ResultsList.ScrollIntoView(ResultsList.SelectedItem);
                }
                e.Handled= true;
            }
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = SearchBox.Text.Trim();

            if(string.IsNullOrWhiteSpace(query))
            {
                if (recentApps.Count == 0)
                {
                    ResultsList.ItemsSource = null;
                    ResultsList.Visibility = Visibility.Collapsed;
                    RecentLabel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ResultsList.ItemsSource = recentApps;
                    ResultsList.Visibility = Visibility.Visible;
                    RecentLabel.Visibility = Visibility.Visible;
                }

                return;
            }
            RecentLabel.Visibility = Visibility.Collapsed;
            ResultsList.Visibility = Visibility.Visible;
            ResultsRow.Height = new GridLength(1, GridUnitType.Star);

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
                    AddToRecent(app);
                }
                catch
                {
                    MessageBox.Show("Could not launch " + app.Name);
                }
            }
        }
        private void AddToRecent(AppEntry app)
        {
            recentApps.RemoveAll(a => a.Path == app.Path); //duplicates
            recentApps.Insert(0,app);
            if(recentApps.Count > 10)
            {
                recentApps.RemoveAt(recentApps.Count - 1);
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

            if(recentApps.Count== 0)
            {
                ResultsList.Visibility = Visibility.Collapsed;
                RecentLabel.Visibility = Visibility.Collapsed;
                ResultsRow.Height = new GridLength(0);
                ResultsList.ItemsSource = null;
            }
            else
            {
                ResultsList.Visibility = Visibility.Visible;
                RecentLabel.Visibility = Visibility.Visible;
                ResultsRow.Height = new GridLength(1, GridUnitType.Star);
                ResultsList.ItemsSource = recentApps;
            }
        }
    }
}
