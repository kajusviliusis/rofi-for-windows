using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace winlauncher.Services
{
    public class HotkeyManager
    {
        private readonly Window _window;
        private readonly int _id;

        public HotkeyManager(Window window, int id)
        {
            _window = window;
            _id = id;
        }

        public void Register(Key key, uint modifiers)
        {
            var helper = new WindowInteropHelper(_window);
            RegisterHotKey(helper.Handle, _id, modifiers, (uint)KeyInterop.VirtualKeyFromKey(key));

            var source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HwndHook);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY)
            {
                ((MainWindow)_window).ShowLauncher();
                handled = true;
            }

            return IntPtr.Zero;
        }

        public void Unregister()
        {
            var helper = new WindowInteropHelper(_window);
            UnregisterHotKey(helper.Handle, _id);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
