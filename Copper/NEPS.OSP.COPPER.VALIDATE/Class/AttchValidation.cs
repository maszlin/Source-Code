using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.VALIDATE
{
    class clsAttchValidation
    {
        public string strSQL = string.Empty;
        public string strMsg = string.Empty;
        public string strLoc = string.Empty;
        string mExcAbb = "";

        public bool AttchValidation_MainJoint(IGTKeyObject objFeat, int FNO, string excabb, string tablename, string attachment, string attchnum_fieldname)
        {
            string strConn = CopValidation.m_GTDataContext.ConfigurationName.ToString();
            Recordset rsChk = new RecordsetClass();

            strLoc = tablename + " where G3E_FID:" + objFeat.FID.ToString();
            strMsg = "Place " + attachment + " must be connect to Joint E-Side/Main Joint. Please try again.";
            mExcAbb = excabb;

            double isOWNER_ID = ValidOwnership(objFeat);
            if (isOWNER_ID > 0)
            {
                string isCableCode = string.Empty;
                string isCableClass = string.Empty;

                strSQL = "SELECT A.G3E_FNO, A.CABLE_CODE, A.SPLICE_CLASS FROM GC_SPLICE A, GC_OWNERSHIP B WHERE A.G3E_FID = B.G3E_FID AND B.G3E_ID = " + isOWNER_ID;
                rsChk = CopValidation.m_GTDataContext.OpenRecordset(strSQL, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);
                if (rsChk.RecordCount > 0)
                {
                    isCableCode = rsChk.Fields[1].Value.ToString();
                    isCableClass = rsChk.Fields[2].Value.ToString().ToUpper();

                    // Check E-Side or D-Side
                    if (isCableClass == "JOINT E-SIDE" || isCableClass == "MAIN JOINT")
                    {
                        if (ValidAttachment(objFeat, FNO, tablename, attachment, attchnum_fieldname, isCableCode))
                            return true;
                    }
                }
            }
            return false;
        }

        public bool AttchValidation_Cable(IGTKeyObject objFeat, int FNO, string excabb, string tablename, string attachment, string attchnum_fieldname)
        {
            string strConn = CopValidation.m_GTDataContext.ConfigurationName.ToString();
            Recordset rsChk = new RecordsetClass();

            strLoc = tablename + " where G3E_FID:" + objFeat.FID.ToString();
            strMsg = "Place " + attachment + " must be connect to cable E-Side. Please try again.";
            mExcAbb = excabb;

            double isOWNER_ID = ValidOwnership(objFeat);
            if (isOWNER_ID > 0)
            {
                string isCableCode = string.Empty;
                string isCableClass = string.Empty;

                strSQL = "SELECT A.G3E_FNO, A.CABLE_CODE, A.CABLE_CLASS FROM GC_CBL A, GC_OWNERSHIP B WHERE A.G3E_FID = B.G3E_FID AND B.G3E_ID = " + isOWNER_ID;
                rsChk = CopValidation.m_GTDataContext.OpenRecordset(strSQL, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);
                if (rsChk.RecordCount > 0)
                {
                    isCableCode = rsChk.Fields[1].Value.ToString();
                    isCableClass = rsChk.Fields[2].Value.ToString();
                    // Check E-Side or D-Side
                    if ((isCableClass.IndexOf("E-") > -1) && (isCableClass.IndexOf("STU") == -1))
                    {
                        if (ValidAttachment(objFeat, FNO, tablename, attachment, attchnum_fieldname, isCableCode))
                            return true;
                    }
                }
            }
            return false;
        }

        public bool AttchValidation_Joint(IGTKeyObject objFeat, int FNO, string excabb, string tablename, string attachment, string attchnum_fieldname)
        {
            string strConn = CopValidation.m_GTDataContext.ConfigurationName.ToString();
            Recordset rsChk = new RecordsetClass();

            strLoc = tablename + " where G3E_FID:" + objFeat.FID.ToString();
            strMsg = "Place " + attachment + " must be connect to Joint E-Side. Please try again.";
            mExcAbb = excabb;

            double isOWNER_ID = ValidOwnership(objFeat);
            if (isOWNER_ID > 0)
            {
                string isCableCode = string.Empty;
                string isCableClass = string.Empty;

                strSQL = "SELECT A.G3E_FNO, A.CABLE_CODE, A.SPLICE_CLASS FROM GC_SPLICE A, GC_OWNERSHIP B WHERE A.G3E_FID = B.G3E_FID AND B.G3E_ID = " + isOWNER_ID;
                rsChk = CopValidation.m_GTDataContext.OpenRecordset(strSQL, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);
                if (rsChk.RecordCount > 0)
                {
                    isCableCode = rsChk.Fields[1].Value.ToString();
                    isCableClass = rsChk.Fields[2].Value.ToString();

                    // Check E-Side or D-Side
                    if (isCableClass == "JOINT E-SIDE")
                    {
                        if (ValidAttachment(objFeat, FNO, tablename, attachment, attchnum_fieldname, isCableCode))
                            return true;
                    }
                }
            }
            return false;
        }

        public bool AttchValidation_TestPoint(IGTKeyObject objFeat, int FNO, string excabb, string tablename, string attachment, string attchnum_fieldname)
        {
            string strConn = CopValidation.m_GTDataContext.ConfigurationName.ToString();
            Recordset rsChk = new RecordsetClass();

            strLoc = tablename + " where G3E_FID:" + objFeat.FID.ToString();
            strMsg = "Place Test Point must be connect to E-Side cable/joint/tie. Please try again.";
            mExcAbb = excabb;

            double isOWNER_ID = ValidOwnership(objFeat);
            if (isOWNER_ID > 0)
            {
                string isCableClass = string.Empty;
                string isCableCode = string.Empty;

                strSQL = "SELECT A.G3E_FNO, A.CABLE_CODE, A.CABLE_CLASS FROM GC_CBL A, GC_OWNERSHIP B " +
                    "WHERE A.G3E_FID = B.G3E_FID AND B.G3E_ID = " + isOWNER_ID;
                rsChk = CopValidation.m_GTDataContext.OpenRecordset(strSQL, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);

                if (rsChk.RecordCount == 0)
                {
                    strSQL = "SELECT A.G3E_FNO, A.CABLE_CODE, A.SPLICE_CLASS FROM GC_SPLICE A, GC_OWNERSHIP B " +
                        "WHERE A.G3E_FID = B.G3E_FID AND B.G3E_ID = " + isOWNER_ID;
                    rsChk = CopValidation.m_GTDataContext.OpenRecordset(strSQL, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);
                }

                if (rsChk.RecordCount > 0)
                {
                    isCableCode = rsChk.Fields[1].Value.ToString();
                    isCableClass = rsChk.Fields[2].Value.ToString().ToUpper();
                    // Check E-Side or D-Side
                    if ((isCableClass.IndexOf("E-") > -1) && (isCableClass.IndexOf("STU") == -1) && (isCableClass != "MAIN JOINT"))
                    {
                        if (ValidAttachment(objFeat, FNO, tablename, attachment, attchnum_fieldname, isCableCode))
                            return true;
                    }
                }
            }
            return false;
        }

        private double ValidOwnership(IGTKeyObject objFeat)
        {
            IGTComponent o_Comp = null;
            o_Comp = objFeat.Components.GetComponent(64); //gc_ownership
            o_Comp.Recordset.MoveFirst();
            // Check Ownership to Joint
            string isG3E_FID = o_Comp.Recordset.Fields["G3E_FID"].Value.ToString();
            string isOWNER1_ID = o_Comp.Recordset.Fields["OWNER1_ID"].Value.ToString();
            string isOWNER2_ID = o_Comp.Recordset.Fields["OWNER2_ID"].Value.ToString();

            if (isG3E_FID.Equals("") || (isOWNER1_ID.Equals("") && isOWNER2_ID.Equals("")))
                return -1;
            else
                return (isOWNER1_ID == "") ? (isOWNER2_ID == "") ? 0 : Convert.ToDouble(isOWNER2_ID) : Convert.ToDouble(isOWNER1_ID);
        }

        private bool ValidAttachment(IGTKeyObject objFeat, int FNO, string tablename, string attachment, string attchnum_fieldname, string owner_cablecode)
        {
            IGTComponent o_Comp = null;
            Recordset rsChk = new RecordsetClass();
            bool flag = false;

            o_Comp = objFeat.Components.GetComponent((short)(FNO + 1));
            o_Comp.Recordset.MoveFirst();

            string isAttchNum = o_Comp.Recordset.Fields[attchnum_fieldname].Value.ToString();
            string isAttchCableCode = o_Comp.Recordset.Fields["CABLE_CODE"].Value.ToString();

            if (isAttchNum.Equals(""))
                strMsg = attachment + " Number cannot be null.";

            else
            {
                strSQL = "SELECT A.G3E_FID FROM " + tablename + " A, GC_NETELEM B WHERE A.G3E_FID = B.G3E_FID AND B.G3E_FNO = " + FNO + " AND B.G3E_FID <> " + objFeat.FID + " AND B.EXC_ABB = '" + mExcAbb + "' AND A.CABLE_CODE = '" + isAttchCableCode + "' AND A." + attchnum_fieldname + " = '" + isAttchNum + "'";
                rsChk = CopValidation.m_GTDataContext.OpenRecordset(strSQL, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);
                if (rsChk.RecordCount > 0)
                    strMsg = attachment + " Number " + isAttchNum + " already exist";
                else
                {
                    if (!isAttchCableCode.Equals(owner_cablecode))
                        o_Comp.Recordset.Update("CABLE_CODE", owner_cablecode);
                    flag = true;
                }
                rsChk = null;
            }

            if (flag)
            {
                flag = false;
                if (!ValidComponent(objFeat, (short)(FNO + 20)))
                    strMsg = "Cannot find geometry for " + attachment ;
                //else if (!ValidComponent(objFeat, (short)(FNO + 30)))
                //    strMsg = "Label for " + attachment + " not placed properly";
                else
                    flag = true;

            }

            return flag;
        }

        private bool ValidComponent(IGTKeyObject objFeat, short CNO)
        {
            IGTComponent o_Comp = null;
            try
            {
                o_Comp = objFeat.Components.GetComponent(CNO);
                o_Comp.Recordset.MoveFirst();
                return (!o_Comp.Recordset.EOF);
            }
            catch {
                return false;
            }
        }

    }

}
