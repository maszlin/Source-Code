using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Data;

using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.MSANConfig
{
    class Utilities
    {
        public const int SHELF_FNO = 15800;
        public const int SLOT_FNO = 12500;
        public const int CARD_FNO = 15900;
        public const int MSAN_FNO = 9100;
        public const int RT_FNO = 9600;
        public const int VDSL_FNO = 9800;
        public const int IMUX_FNO = 9500;

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

        public static string rsField(ADODB.Recordset rs, string fieldname)
        {
            try
            {
                object val = rs.Fields[fieldname].Value.ToString();
                if (val == null)
                    return "";
                else
                    return val.ToString().Trim();
            }
            catch { return ""; }
        }

        public static string CellValue(object val)
        {
            try
            {
                if (val == null)
                    return "";
                else
                    return val.ToString().Trim();
            }
            catch { return ""; }
        }

        public static bool FillComboBox(string ssql, DataGridViewComboBoxColumn comboBox, string fieldName )
        {

            int rec_count = 0;
            IGTDataContext dc = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = dc.Execute(ssql, out rec_count, (int)CommandTypeEnum.adCmdText, null);
            comboBox.Items.Clear();
            if (rs != null && !rs.BOF)
            {

                rs.MoveFirst();
                do
                {
                    comboBox.Items.Add(rsField(rs, fieldName));
                    rs.MoveNext();
                }
                while (!rs.EOF);
                return true;
            }
            else
                return false;
        }

        internal static void SelectFirstItem(DataGridViewComboBoxColumn comboBox, DataGridViewCell cell)
        {
            if (comboBox.Items.Count > 0)
            {
                string firstValue = comboBox.Items[0].ToString();
                cell.Value = firstValue;
            }
        }
    }
}
