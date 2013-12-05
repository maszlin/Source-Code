using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

using System.Diagnostics;
using Microsoft.Win32;

namespace NEPS.GTechnology.Cabinet
{
    class MyRegistry
    {
        #region "Registry"
        static internal string GetSetting(string appl, string section, string key, string defaultval)
        {
            RegistryKey applkey = Registry.CurrentUser.OpenSubKey(appl,true);
            if (applkey == null)
                applkey = Registry.CurrentUser.CreateSubKey(appl);

            RegistryKey sectkey = applkey.OpenSubKey(section,true);
            if (sectkey == null) 
                sectkey = applkey.CreateSubKey(section);

            return (string)sectkey.GetValue(key, defaultval);
        }

        static internal bool SaveSetting(string appl, string section, string key, string val)
        {
            RegistryKey applkey = Registry.CurrentUser.OpenSubKey(appl,true);
            if (applkey == null)
                applkey = Registry.CurrentUser.CreateSubKey(appl);

            RegistryKey sectkey = applkey.OpenSubKey(section,true);
            if (sectkey == null)
                sectkey = applkey.CreateSubKey(section);

            sectkey.SetValue(key, val);
            return true;
        }       

        #endregion

        #region "Font Setting"
        static internal Font GetFontSetting(string appl, string section, string key, Font defaultfont)
        {
            RegistryKey applkey = Registry.CurrentUser.OpenSubKey(appl,true);
            if (applkey == null)
                applkey = Registry.CurrentUser.CreateSubKey(appl);

            RegistryKey sectkey = applkey.OpenSubKey(section,true);
            if (sectkey == null)
                sectkey = applkey.CreateSubKey(section);

            RegistryKey fontkey = sectkey.OpenSubKey(key, true);
            if (fontkey == null)
                fontkey = sectkey.CreateSubKey(key);

            string fontString = (string)fontkey.GetValue("Font", "NotSet");

            if (fontString == "NotSet")
                return defaultfont;
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                // Load an instance of Font from a string 
                Font font = (Font)converter.ConvertFromString(fontString);
                return font;
            }
        }

        static internal bool SaveFontSetting(string appl, string section, string key, Font val)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
            // Saving Font object as a string 
            string fontString = converter.ConvertToString(val);

            RegistryKey applkey = Registry.CurrentUser.OpenSubKey(appl);
            if (applkey == null)
                applkey = Registry.CurrentUser.CreateSubKey(appl);

            RegistryKey sectkey = applkey.OpenSubKey(section, true);
            if (sectkey == null)
                sectkey = applkey.CreateSubKey(section);

            RegistryKey fontkey = sectkey.OpenSubKey(key, true);
            if (fontkey == null)
                fontkey = sectkey.CreateSubKey(key);

            fontkey.SetValue("Font", fontString);
            return true;
        }
        #endregion

       

   
    }
}
