using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace winlauncher.Helpers
{
    public static class Fuzzy
    {
        public static int Score(string text, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return 0;

            int score = 0;
            int ti = 0;

            text = text.ToLower();
            query = query.ToLower();

            foreach(char qc in query)
            {
                bool found = false;

                while(ti<text.Length)
                {
                    if (text[ti] == qc)
                    {
                        score += 10;
                        found = true;
                        ti++;
                        break;
                    }
                    ti++;
                }
                if (!found)
                    return 0;
            }
            return score - (text.Length-query.Length);
        }
    }
}
