using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace winlauncher.Models
{
    public class AppEntry
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public ImageSource Icon {  get; set; }
    }
}
