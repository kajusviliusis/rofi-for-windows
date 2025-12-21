using IWshRuntimeLibrary;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
//using System.Windows.Shapes;

namespace winlauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _apps = LoadStartMenuApps();
            ResultsList.ItemsSource = _apps;
            this.Loaded += MainWindow_Loaded;

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
            if(ResultsList.SelectedItem is AppEntry app)
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
        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            var filtered = _apps
                .Where(a => a.Name.ToLower().Contains(query))
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
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY)
            {
                MessageBox.Show("Hotkey fired");
                ShowLauncher();
                handled = true;
            }

            return IntPtr.Zero;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            RegisterHotKey(helper.Handle, 1, 2, (uint)KeyInterop.VirtualKeyFromKey(Key.Space));

            var source = System.Windows.Interop.HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HwndHook);
        }
        
        protected override void OnClosed(EventArgs e)
        {
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, 1);
            base.OnClosed(e);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private List<AppEntry> LoadStartMenuApps()
        {
            var apps = new List<AppEntry>();

            string[] startMenuPaths =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)
            };

            foreach (var path in startMenuPaths)
            {
                foreach (var file in Directory.GetFiles(path, "*.lnk", SearchOption.AllDirectories))
                {
                    var shortcut = GetShortcutTarget(file);
                    if (shortcut != null)
                    {
                        apps.Add(new AppEntry
                        {
                            Name = Path.GetFileNameWithoutExtension(file),
                            Path = shortcut
                        });
                    }
                }
            }

            return apps;
        }
        private string? GetShortcutTarget(string shortcutPath)
        {
            try
            {
                var shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                return link.TargetPath;
            }
            catch
            {
                return null;
            }
        }

        private List<AppEntry> _apps = new List<AppEntry>();


    }
}