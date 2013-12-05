// CREATED by M.ZAM @ 04-06-2013 - Address


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.SERVICE.ASSIGN
{
    class clsAddress
    {
        private string building_name = "";
        private string floor_no = "";
        private string apt_num = "";
        private string sec_name = "";
        private string street_type = "";
        private string street_name = "";
        private string postal_code = "";
        private string state_code = "";
        private string city = "";
        private string country = "";

        private string lot_number = "";
        private string house_number = "";
        private string objectid = "";
        private string property_type = "";

        #region properties
        public string FloorNum // apartment unit floor number
        {
            set { this.floor_no = value; }
        }
        public string AptNum // apartment unit house number
        {
            set { this.apt_num = value; }
        }
        public string AptID // object ID for apartment unit
        {
            set { objectid = value; }
        }
        
        #endregion


        public clsAddress(string buildingID, string segment)
        {
            BuildingAddress(buildingID, segment);
        }

        private void BuildingAddress(string buildingID, string segment)
        {
            string ssql =
                "SELECT st.SECTION_NAME, st.STREET_TYPE, st.STREET_NAME, st.STREET_ALT_NAME, st.POSTAL_NUM, st.CITY_NAME, st.STATE_CODE, " +
                "bl.BUILDING_NAME, bl.BUILDING_NAME2, pr.PROPERTY_TYPE, pr.LOT_NUMBER, pr.HOUSE_NUMBER " +
                "FROM GC_ADM_STREET st, GC_ADM_BLDG bl, GC_ADM_PROPERTY pr " +
                "WHERE bl.SEGMENT = '" + segment + "' AND bl.OBJECTID = " + buildingID + " " +
                "AND pr.SEGMENT = '" + segment + "' AND pr.OBJECTID = bl.PROPERTY_ID " +
                "AND st.SEGMENT = '" + segment + "' AND st.OBJECTID = pr.STREET_ID";

            ADODB.Recordset rs = myUtil.ADODB_ExecuteQuery(ssql);

            this.building_name = myUtil.rsField(rs, "BUILDING_NAME");
            this.lot_number = myUtil.rsField(rs, "LOT_NUMBER");
            this.house_number = myUtil.rsField(rs, "HOUSE_NUMBER");
            this.property_type = myUtil.rsField(rs, "PROPERTY_TYPE");

            this.street_name = myUtil.rsField(rs, "STREET_NAME");
            this.street_type = myUtil.rsField(rs, "STREET_TYPE");
            this.sec_name = myUtil.rsField(rs, "SECTION_NAME");
            this.postal_code = myUtil.rsField(rs, "POSTAL_NUM");
            this.state_code = myUtil.rsField(rs, "STATE_CODE");
            this.city = myUtil.rsField(rs, "CITY_NAME");
            this.country = "MALAYSIA";
        }

        #region gc_address
        public void Save_GC_Address(short iFNO, int iFID)
        {
            short iCNO;
            IGTKeyObject oFeature;
            oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(iFNO, iFID);

            iCNO = 52; //GC_ADDRESS
            ADODB.Recordset rs = oFeature.Components.GetComponent(iCNO).Recordset;

            if (rs.EOF)
            {
                rs.AddNew("G3E_FID", iFID);
                rs.Update("G3E_FNO", iFNO);
            }
            else
            {
                rs.MoveLast();
            }
            rs.Update("BUILDING_NAME", this.building_name);
            rs.Update("FLOOR_NO", this.floor_no);
            //rs.Update("HSE_NUM", this.apt_num);
            rs.Update("STREET_NAME", this.street_name);
            rs.Update("STREET_TYPE", this.street_type);
            rs.Update("POSTAL_CODE", this.postal_code);
            rs.Update("SEC_NAME", this.sec_name);
            rs.Update("CITY", this.city);
            rs.Update("STATE_CODE", this.state_code);
        }  
        #endregion

        #region gc_serviceaddress
        public void Save_GC_ServiceAddress(int bndFID)
        {
            short iCNO;
            short bndFNO = 24000;
            IGTKeyObject oFeature;
            oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(bndFNO, bndFID);

            iCNO = 24004; //GC_SERVICEADDRESS

            ADODB.Recordset rs = oFeature.Components.GetComponent(iCNO).Recordset;

            if (rs.EOF)
            {
                rs.AddNew("G3E_FID", bndFID);
                rs.Update("G3E_FNO", bndFNO);
            }
            else
            {
                rs.MoveLast();
                if (myUtil.rsField(rs, "OBJECTID") != this.objectid)
                {
                    rs.AddNew("G3E_FID", bndFID);
                    rs.Update("G3E_FNO", bndFNO);
                }
            }
 
            rs.Update("CITY", this.city);
            rs.Update("HOUSE_NUMBER", this.apt_num);
            //rs.Update("LOT_NUMBER", this.floor_no);
            rs.Update("OBJECTID", this.objectid); //?
            rs.Update("POSTAL_CODE", this.postal_code);
            rs.Update("PROPERTY_TYPE", this.property_type );
            rs.Update("SEC_NAME", this.sec_name);
            rs.Update("STATE_CODE", this.state_code);
            rs.Update("STREET_NAME", this.street_name);
            rs.Update("STREET_TYPE", this.street_type);
        }

        public static void Delete_GC_ServiceAddress(int bndFID, int aptID)
        {
            string ssql = "DELETE FROM GC_SERVICEADDRESS WHERE G3E_FID = " + bndFID + " AND OBJECTID = " + aptID;
            myUtil.ADODB_ExecuteNonQuery(ssql);
        }
      
        #endregion

        #region reporting
        public string PrintBuildingAddress()
        {
            string addr = this.building_name + ", ";
            addr = LotOrHouseNumber(addr, this.lot_number, this.house_number);
            addr = StreetName(addr, this.street_type, this.street_name);
            addr += this.postal_code + " " + this.city + ", " + this.state_code;

            return addr;
        }

        private string LotOrHouseNumber(string addr, string lot, string house)
        {
            if (lot.Length > 0 && lot != "''")
                return addr + lot + ", ";
            else if (house.Length > 0 && house != "''")
                return addr + house + ", ";
            else
                return "";
        }

        private string StreetName(string addr, string st_type, string st_name)
        {
            if ((st_type.Length > 0 && st_type != "''")
                && (st_name.Length > 0 && st_name != "''"))
            {
                return (addr + st_type + " " + st_name + ", ");
            }
            return addr;
        }
        #endregion
    }

}
