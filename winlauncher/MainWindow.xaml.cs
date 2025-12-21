using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace winlauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ResultsList.ItemsSource = _apps;
            this.Loaded += (s, e) => ShowLauncher();

        }
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
        private void LaunchSelected()
        {
            if(ResultsList.SelectedItem is string app)
            {
                try
                {
                    System.Diagnostics.Process.Start(app);
                }
                catch
                {
                    MessageBox.Show("Could not launch " + app);
                }
            }
        }
        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            var filtered = _apps
                .Where(a => a.ToLower().Contains(query))
                .ToList();

            ResultsList.ItemsSource = filtered;

            if (filtered.Count > 0)
                ResultsList.SelectedIndex = 0;
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
        private List<string> _apps = new List<string>
        {
            "notepad",
            "calc",
            "mspaint",
            "cmd",
            "powershell"
        };



    }
}