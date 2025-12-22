using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using winlauncher.Models;

namespace winlauncher.Helpers
{
    public static class IconExtractor
    {
        public static ImageSource? GetIcon(string exePath)
        {
            try
            {
                Icon icon = Icon.ExtractAssociatedIcon(exePath);
                if (icon == null)
                    return null;

                using (var iconStream = new MemoryStream())
                {
                    icon.ToBitmap().Save(iconStream, System.Drawing.Imaging.ImageFormat.Png);
                    iconStream.Seek(0, SeekOrigin.Begin);

                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = iconStream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();

                    return image;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
