using System;
using System.Collections.Generic;
using System.Text;

namespace NEPS.GTechnology.Property_Count
{
    class PropertyCount
    {
        private string sProperty_Type;
        public string Property_Type
        {
            set
            { sProperty_Type = value; }

            get
            { return sProperty_Type; }
        }

        private int sProperty_Type_No;
        public int Property_Type_No
        {
            set
            { sProperty_Type_No = value; }

            get
            { return sProperty_Type_No; }
        }

        private string sProperty_Type_Desc;
        public string Property_Type_Desc
        {
            set
            { sProperty_Type_Desc = value; }

            get
            { return sProperty_Type_Desc; }
        }

        private int sProperty_Type_Qty;
        public int Property_Type_Qty
        {
            set
            { sProperty_Type_Qty = value; }

            get
            { return sProperty_Type_Qty; }
        }
    }

}
