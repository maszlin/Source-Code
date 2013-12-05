using System;
using System.Collections.Generic;
using System.Text;

namespace CTA.GTechnology.L3Generation
{
    public class FDPDef
    {
        private string nFDP_NO;
        private short nFDP_FNO;
        private int nFDP_FID;

        private int nStorey;
        private string sFDPType;
        private int nFloor;

        private List<FTPDef> lstUnit;

        #region Getters/Setters
        public void AddUnit(FTPDef oDP)
        {
            if (lstUnit == null)
                lstUnit = new List<FTPDef>();
            lstUnit.Add(oDP);
        }
        private List<FTPDef> lstFTP;

        public FTPDef GetFTP(int Storey)
        {
            if (lstFTP == null)
                return null;
            for (int i = 0; i < lstFTP.Count; i++)
            {
                if (lstFTP[i].Floor == Storey)
                    return lstFTP[i];
            }
            return null;
        }
        public FTPDef GetUnit(int Storey, int UnitNo)
        {
            if (lstUnit == null)
                return null;
            for (int i = 0; i < lstUnit.Count; i++)
            {
                if ((lstUnit[i].Floor == Storey) && (lstUnit[i].UNIT_ID == UnitNo))
                    return lstUnit[i];
            }
            return null;
        }

        public FTPDef GetUnit(string Name)
        {
            if (lstUnit == null)
                return null;
            for (int i = 0; i < lstUnit.Count; i++)
            {
                if (lstUnit[i].FTP_NO == Name)
                    return lstUnit[i];
            }
            return null;
        }

        public List<FTPDef> Units
        {
            set
            { lstUnit = value; }

            get
            { return lstUnit; }
        }

        public int Floor
        {
            set
            { nFloor = value; }

            get
            { return nFloor; }
        }

        public string FDP_NO
        {
            set
            { nFDP_NO = value; }

            get
            { return nFDP_NO; }
        }
        private string nFDP_Serv_Unit;
        public string FDP_Serv_Unit
        {
            set
            { nFDP_Serv_Unit = value; }

            get
            { return nFDP_Serv_Unit; }
        }

        public short FNO
        {
            set
            { nFDP_FNO = value; }

            get
            { return nFDP_FNO; }
        }

        public int FID
        {
            set
            { nFDP_FID = value; }

            get
            { return nFDP_FID; }
        }

        private string sPostalCode;
        public string PostalCode
        {
            set
            { sPostalCode = value; }

            get
            { return sPostalCode; }
        }

        private int nStripNo;
        public int StripNo
        {
            set
            { nStripNo = value; }

            get
            { return nStripNo; }
        }

        private int nCount;
        public int Count
        {
            set
            { nCount = value; }

            get
            { return nCount; }
        }

        private int nLowStrand;
        public int LowStrand
        {
            set
            { nLowStrand = value; }

            get
            { return nLowStrand; }
        }

        private int nHighStrand;
        public int HighStrand
        {
            set
            { nHighStrand = value; }

            get
            { return nHighStrand; }
        }

        private double dLen;
        public double Len
        {
            set
            { dLen = value; }

            get
            { return dLen; }
        }

        private string sCblSize;
        public string CblSize
        {
            set
            { sCblSize = value; }

            get
            { return sCblSize; }
        }

        private string sCblGauge;
        public string CblGauge
        {
            set
            { sCblGauge = value; }

            get
            { return sCblGauge; }
        }

        private string sCblSrcIpid;
        public string CblSrcIpid
        {
            set
            { sCblSrcIpid = value; }

            get
            { return sCblSrcIpid; }
        }

        private string sPrimaryMIN;
        public string PrimaryMIN
        {
            set
            { sPrimaryMIN = value; }

            get
            { return sPrimaryMIN; }
        }

        private string sCblPrimaryMIN;
        public string CblPrimaryMIN
        {
            set
            { sCblPrimaryMIN = value; }

            get
            { return sCblPrimaryMIN; }
        }

        private string sStreetName;
        public string StreetName
        {
            set
            { sStreetName = value; }

            get
            { return sStreetName; }
        }

        private string sBldgName;
        public string BldgName
        {
            set
            { sBldgName = value; }

            get
            { return sBldgName; }
        }

        private string sBldgNo;
        public string BldgNo
        {
            set
            { sBldgNo = value; }

            get
            { return sBldgNo; }
        }
        
        private string sBType;
        public string BType
        {
            set
            { sBType = value; }

            get
            { return sBType; }
        }


        private int iFTPPerFloor;
        public int FTPPerFloor
        {
            set
            { iFTPPerFloor = value; }

            get
            { return iFTPPerFloor; }
        }
        private int iLength;
        public int CableLength
        {
            set
            { iLength = value; }

            get
            { return iLength; }
        }
        private string iStatus;
        public string Status
        {
            set
            { iStatus = value; }

            get
            { return iStatus; }
        }

        private string iDP_MLS_Type;
        public string DP_MLS_Type
        {
            set
            { iDP_MLS_Type = value; }

            get
            { return iDP_MLS_Type; }
        }


        private string iDP_Desc;
        public string DP_Desc
        {
            set
            { iDP_Desc = value; }

            get
            { return iDP_Desc; }
        }

        private string iNode;
        public string Node_Class
        {
            set
            { iNode = value; }

            get
            { return iNode; }
        }

        private string iProjCode;
        public string ProjCode
        {
            set
            { iProjCode = value; }

            get
            { return iProjCode; }
        }

        private string iBlockInd;
        public string BlockInd
        {
            set
            { iBlockInd = value; }

            get
            { return iBlockInd; }
        }

        
        private string iPhBlk;
        public string PhBlk
        {
            set
            { iPhBlk = value; }

            get
            { return iPhBlk; }
        }

        private string iwallpipe;
        public string Wallpipe
        {
            set
            { iwallpipe = value; }

            get
            { return iwallpipe; }
        }


        private int icablno;
        public int Cableno
        {
            set
            { icablno = value; }

            get
            { return icablno; }
        }


        private string iUnit;
        public string Unit
        {
            set
            { iUnit = value; }

            get
            { return iUnit; }
        }


        private string iL3No;
        public string L3No
        {
            set
            { iL3No = value; }

            get
            { return iL3No; }
        }


        private string iParentID;
        public string ParentID
        {
            set
            { iParentID = value; }

            get
            { return iParentID; }
        }


        #endregion
    }
}
