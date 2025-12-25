using IWshRuntimeLibrary;
using System.IO;
using System.Linq;
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
                    if (shortcut != null && !string.IsNullOrWhiteSpace(shortcut.TargetPath))
                    {
                        apps.Add(new AppEntry
                        {
                            Name = System.IO.Path.GetFileNameWithoutExtension(file),
                            Path = shortcut.TargetPath,
                            Arguments = shortcut.Arguments ?? string.Empty,
                            Icon = IconExtractor.GetIcon(shortcut.TargetPath)
                        });
                    }
                }
            }

            // hardcode file explorer path
            try
            {
                var explorerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
                bool hasExplorer = apps.Any(a => !string.IsNullOrWhiteSpace(a.Path) &&
                                                a.Path.IndexOf("explorer.exe", System.StringComparison.OrdinalIgnoreCase) >= 0);

                if (System.IO.File.Exists(explorerPath) && !hasExplorer)
                {
                    apps.Add(new AppEntry
                    {
                        Name = "File Explorer",
                        Path = explorerPath,
                        Arguments = string.Empty,
                        Icon = IconExtractor.GetIcon(explorerPath)
                    });
                }
            }
            catch
            {
            }

            return apps;
        }
    }
}
