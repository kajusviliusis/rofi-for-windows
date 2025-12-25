using IWshRuntimeLibrary;
using winlauncher.Models;

namespace winlauncher.Helpers
{
    public static class ShortcutResolver
    {
        public static ShortcutInfo? ResolveShortcut(string shortcutPath)
        {
            try
            {
                var shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                return new ShortcutInfo
                {
                    TargetPath = link.TargetPath,
                    Arguments = link.Arguments
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
