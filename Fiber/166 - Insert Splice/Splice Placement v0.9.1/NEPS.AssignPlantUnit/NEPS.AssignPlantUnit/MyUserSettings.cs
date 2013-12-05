using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace NEPS.AssignPlantUnit
{
    class MyUserSettings: ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue("FUJIKURA")]
        public string Manufacturer
        {
            get
            {
                return (this["Manufacturer"].ToString());
            }
            set
            {
                this["Manufacturer"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("APEX")]
        public string Contractor
        {
            get
            {
                return (this["Contractor"].ToString());
            }
            set
            {
                this["Contractor"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string MinMaterial
        {
            get
            {
                return (this["MinMaterial"].ToString());
            }
            set
            {
                this["MinMaterial"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("TM")]
        public string Ownership
        {
            get
            {
                return (this["Ownership"].ToString());
            }
            set
            {
                this["Ownership"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("2012")]
        public string YearPlaced
        {
            get
            {
                return (this["YearPlaced"].ToString());
            }
            set
            {
                this["YearPlaced"] = value;
            }
        }

    }
}
