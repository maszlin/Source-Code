using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Data;

using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace NEPS.GTechnology.Cabinet
{
    class Utilities
    {
        public static Color FontSelectedColor = Color.DarkGreen;
        public static Color FontHoverColor = Color.Red;
        public static Color FontNormalColor = Color.Navy;
        public static Color BackSelectedColor = Color.Yellow;
        public static Color BackNormalColor = Color.White;
        /// <summary>
        /// Pulangkan katakunci untuk carian rekod dari table MySQL
        /// Gunakan format Key1+Key2+... apabila lebih dari satu katakunci
        /// Pastikan urutannya betul jika tidak rekod yang dicari mungkin tidak dijumpai
        /// </summary>
        /// <param name="katakunci">Key1+Key2+..</param>
        /// <returns>katakunci dalam format MySQL</returns>
        public static string KataKunci(string katakunci)
        {
            string key = "";
            if (katakunci.Length > 0)
            {
                string[] keys = katakunci.Split('+');
                key = keys[0];
                for (int i = 1; i < keys.Length; i++)
                {
                    key += "%" + keys[i];
                }
            }
            return key;
        }

       }
}
