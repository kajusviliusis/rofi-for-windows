using IWshRuntimeLibrary;
using System.IO;
using winlauncher.Models;
using winlauncher.Helpers;

namespace winlauncher.Services
{
    public class AppScanner
    {
        public List<AppEntry> LoadStartMenuApps()
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
                    var shortcut = ShortcutResolver.ResolveShortcut(file);
                    if (shortcut != null)
                    {
                        apps.Add(new AppEntry
                        {
                            Name = System.IO.Path.GetFileNameWithoutExtension(file),
                            Path = shortcut,
                            Icon = IconExtractor.GetIcon(shortcut)
                        });
                    }
                }
            }

            return apps;
        }
    }
}
