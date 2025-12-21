using IWshRuntimeLibrary;

namespace winlauncher.Helpers
{
    public static class ShortcutResolver
    {
        public static string? ResolveShortcut(string shortcutPath)
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
    }
}
