using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

using System.Diagnostics;

namespace NEPS.OSP.COPPER.VALIDATE
{
    public class CopValidation : IGTFeatureValidation
    {
        private GTArguments m_GTArguments = null;
        public static IGTDataContext m_GTDataContext = null;
        private string m_Mode = null;

        public bool Validate(IGTKeyObjects Features, Recordset ValidationErrors)
        {
            string strSQL = string.Empty;
            string strMsg = string.Empty;
            string strLoc = string.Empty;
            string strConn = m_GTDataContext.ConfigurationName.ToString();
            IGTComponent o_Comp = null;
            Recordset rsChk = new RecordsetClass();

            object[] fieldList = new object[] { "ErrorPriority", "ErrorDescription", "ErrorLocation", "Connection", "G3E_FNO", "G3E_FID", "G3E_CNO", "G3E_CID" };
            object[] values = new object[8];
            try
            {
                bool flag = true;
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Start Validation");
                string mode = this.m_Mode;
                // General Parameter
                double isFNO = 0;
                double isFID = 0;
                double isCNO = 0;
                double isCID = 0;
                IGTKeyObject objFeat = Features[0];

                // General Mandatory Checking Value (G3E_JOB --> GC_NETELEM)
                string isExcAbb = string.Empty;
                string isSchemeName = string.Empty;
                string isSegment = string.Empty;
                string isYearInstall = string.Empty;
                Recordset rsJob = new RecordsetClass();
                strSQL = "SELECT EXC_ABB, SCHEME_NAME, SEGMENT, YEAR_INSTALL FROM G3E_JOB WHERE G3E_IDENTIFIER = '" + m_GTDataContext.ActiveJob.ToString() + "'";
                rsJob = m_GTDataContext.OpenRecordset(strSQL, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);
                if (rsJob.RecordCount > 0)
                {
                    isExcAbb = rsJob.Fields[0].Value.ToString();
                    isSchemeName = rsJob.Fields[1].Value.ToString();
                    isSegment = rsJob.Fields[2].Value.ToString();
                    isYearInstall = rsJob.Fields[3].Value.ToString();
                }
                rsJob.Close();
                strSQL = "";

                // General Validation
                //                    if (isYearInstall.Equals(""))

                if (isExcAbb.Equals("") || isSchemeName.Equals("") || isSegment.Equals(""))
                {
                    strMsg = "General Mandatory Information is Empty (Exc Abb:" + isExcAbb + "; Scheme Name:" + isSchemeName + "; Segment: " + isSegment + "; Year Install:" + isYearInstall + "). Check on Job Management.";
                    strLoc = "G3E_JOB where G3E_IDENTIFIER:" + m_GTDataContext.ActiveJob.ToString();
                    flag = false;
                }
                else
                {

                    #region Notes on E-Side Attachment
                    /*
                    1. 6400 : Contactor Gauge --> Is place at a cable/along Cable E-Side. In LNMS user need to identify E-Side Cable/Main Joint/Tie E-Cable.
                    2. 6500 : Contactor Alarm --> Is place at a Joint E-Side/ Main Joint.
                    3. 6600 : Gas Seal --> Is place at a cable/along Cable E-Side. In LNMS user need to identify E-Side Cable/Main Joint/Tie E-Cable.
                    4. 6700 : Test Point --> Is place at Joint E-Side/Cable E-Side/Tie E-Cable.
                    5. 6800 : Loading Coil --> Is place at Joint E-Side/Main Joint.
                    6. 6900 : Transducer --> Is place at Joint E-Side.
                    */
                    #endregion
                    clsAttchValidation attch = new clsAttchValidation();
                    bool attcflag = true;
                                        
                    o_Comp = objFeat.Components.GetComponent(51);
                    if (!isExcAbb.Equals(o_Comp.Recordset.Fields["EXC_ABB"].Value.ToString()))
                    {
                        strMsg = "Current job allow placement only for exchange " + isExcAbb;
                        strLoc = "G3E_JOB where G3E_IDENTIFIER:" + m_GTDataContext.ActiveJob.ToString();
                        flag = false;
                    }
                    else
                    {
                        switch (objFeat.FNO)
                        {
                            case 6000:
                                #region Exchange Validation
                                // Check Attribute
                                o_Comp = objFeat.Components.GetComponent(51);
                                o_Comp.Recordset.MoveFirst();
                                isFID = Convert.ToDouble(o_Comp.Recordset.Fields["G3E_FID"].Value.ToString());
                                if (o_Comp.Recordset.Fields["EXC_ABB"].Value.ToString() == "")
                                {
                                    strMsg = "Exchange Abbreviation cannot be Empty";
                                    strLoc = "GC_NETELEM where G3E_FID:" + o_Comp.Recordset.Fields["G3E_FID"].Value.ToString();
                                    flag = false;
                                }
                                else
                                {
                                    strSQL = "SELECT G3E_FID FROM GC_NETELEM WHERE G3E_FNO = 6000 AND G3E_FID <> " + isFID + " AND EXC_ABB = '" + o_Comp.Recordset.Fields["EXC_ABB"].Value.ToString() + "'";
                                    rsChk = m_GTDataContext.OpenRecordset(strSQL, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);
                                    if (rsChk.RecordCount > 0)
                                    {
                                        strMsg = "Exchange Abbreviation " + o_Comp.Recordset.Fields["EXC_ABB"].Value.ToString() + " already Exist";
                                        strLoc = "GC_NETELEM where G3E_FID:" + isFID.ToString();
                                        flag = false;
                                    }
                                    strSQL = "";
                                    rsChk = null;
                                }
                                break;
                                #endregion
                            case 6200: // added on 2012-10-02
                                #region PDDP [6200 : GC_PDDP]
                                o_Comp = objFeat.Components.GetComponent(6201);
                                string pddpcode = o_Comp.Recordset.Fields["PDDP_CODE"].Value.ToString().Trim();
                                if (pddpcode.Length == 0)
                                    o_Comp.Recordset.Update("PDDP_CODE", GetPDDPCode.NextCode(isExcAbb));
                                else if (GetPDDPCode.DuplicateCode(isExcAbb, pddpcode, objFeat.FID))
                                {
                                    strMsg = "Duplicate PDDP Code " + isExcAbb;
                                    strLoc = "GC_PDDP where G3E_FID:" + objFeat.FID.ToString();
                                    flag = false;
                                }
                                else
                                {
                                    o_Comp.Recordset.Update("DDP_NUM", pddpcode.PadLeft(3, '0'));
                                }
                                break;
                                #endregion
                            case 6300: // added on 2012-10-02
                                #region DDP [6300 : GC_DDP]
                                o_Comp = objFeat.Components.GetComponent(6301);
                                string ddpnum = o_Comp.Recordset.Fields["DDP_NUM"].Value.ToString().Trim();
                                if (ddpnum.Length == 0)
                                    o_Comp.Recordset.Update("DDP_NUM", GetDDPNumber.NextCode(isExcAbb));
                                else if (GetDDPNumber.DuplicateCode(isExcAbb, ddpnum, objFeat.FID))
                                {
                                    strMsg = "Duplicate DDP Code " + isExcAbb;
                                    strLoc = "GC_DDP where G3E_FID:" + objFeat.FID.ToString();
                                    flag = false;
                                }
                                else
                                {
                                    o_Comp.Recordset.Update("DDP_NUM", ddpnum.PadLeft(4, '0'));
                                }
                                break;
                                #endregion
                            /*
                            case 6400:
                                #region Contractor Gauge Validation [6400 : GC_CONTGAUGE]
                                attcflag = attch.AttchValidation_Cable(objFeat, 6400, isExcAbb, "GC_CONTGAUGE", "Contractor Gauge", "CG_NUM");
                                break;
                                #endregion
                            case 6500:
                                #region Contactor Alarm Validation [6500 : GC_CONTALARM]
                                attcflag = attch.AttchValidation_MainJoint(objFeat, 6500, isExcAbb, "GC_CONTALARM", "Contractor Alarm", "CA_NUM");
                                break;
                                #endregion
                            case 6600:
                                #region Gas Seal Validation [6600 : GC_GASSEAL]
                                attcflag = attch.AttchValidation_Cable(objFeat, 6600, isExcAbb, "GC_GASSEAL", "Gas Seal", "GS_NUM");
                                break;
                                #endregion
                            case 6700:
                                #region GC_TESTPNT Validation [6700 : GC_TESTPNT]
                                attcflag = attch.AttchValidation_TestPoint(objFeat, 6700, isExcAbb, "GC_TESTPNT", "GC_TESTPNT", "TP_NUM");
                                break;
                                #endregion
                            case 6800:
                                #region Loading Coil Validation [6800 : GC_LDCOIL]
                                {
                                    attcflag = attch.AttchValidation_MainJoint(objFeat, 6800, isExcAbb, "GC_LDCOIL", "Loading Coil", "LC_NUM");
                                    break;
                                }
                                #endregion
                            case 6900:
                                #region Transducer Validation [6900 : GC_TRNSDCR]
                                attcflag = attch.AttchValidation_Joint(objFeat, 6900, isExcAbb, "GC_TRNSDCR", "Transducer", "TR_NUM");
                                break;
                                #endregion
 */
                            case 7000:
                                #region Cable Validation

                                CableValidation cv = new CableValidation();
                                flag = cv.ValidateCableCode(objFeat, isExcAbb);
                                strMsg = cv.strMsg;
                                strLoc = cv.strLoc;
                                break;

                                #endregion
                            case 10300:
                                #region Cabinet Validation
                                o_Comp = objFeat.Components.GetComponent(10301);
                                o_Comp.Recordset.MoveFirst();
                                isFID = Convert.ToDouble(o_Comp.Recordset.Fields["G3E_FID"].Value.ToString());
                                string itfaceCode = o_Comp.Recordset.Fields["ITFACE_CODE"].Value.ToString();
                                string itfaceClass = o_Comp.Recordset.Fields["ITFACE_CLASS"].Value.ToString();
                                if (itfaceClass == "SDF")
                                    itfaceCode = o_Comp.Recordset.Fields["SDF_CODE"].Value.ToString();

                                //Cat. Changed: 14th Sept 2012, associate SDF CODE/PCAB CODE or ITFACE_CODE

                                if (!itfaceCode.Equals(""))
                                {
                                    // Check Format Code
                                    if (GetCabinetCode.ChkFormatCode(itfaceCode, itfaceClass))
                                    {
                                        if (GetCabinetCode.DuplicateCode(itfaceCode, isFID, isExcAbb))
                                        {
                                            strMsg = (itfaceClass == "SDF") ? "SDF Code already Exist" : (itfaceClass == "PHANTOM CABINET") ? "Phantom Cabinet Code already Exist" : "Cabinet Code already Exist";
                                            strLoc = "GC_ITFACE where G3E_FID:" + isFID.ToString();
                                            flag = false;
                                        }
                                        else if (itfaceClass == "SDF")
                                            o_Comp.Recordset.Update("ITFACE_CODE", itfaceCode);
                                        else if (itfaceClass == "PHANTOM CABINET")
                                            o_Comp.Recordset.Update("PCAB_CODE", itfaceCode);
                                    }
                                    else
                                    {
                                        strMsg = (itfaceClass == "SDF") ? "Format SDF Code is Wrong (Range:4 digit. Character:Allowed.)." : (itfaceClass == "PHANTOM CABINET") ? "Format Phantom Cabinet Code is Wrong (Range:901-999)." : "Format Cabinet Code is Wrong (Range:001-900).";
                                        strLoc = "GC_ITFACE where G3E_FID:" + isFID.ToString();
                                        flag = false;
                                    }
                                }
                                else if (!GetCabinetCode.GenerateCode(objFeat.Components.GetComponent(10301)))
                                {
                                    strMsg = (itfaceClass == "SDF") ? "SDF Code cannot be Empty" : (itfaceClass == "PHANTOM CABINET") ? "Phantom Cabinet Code cannot be Empty" : "Cabinet Code cannot be Empty";
                                    strLoc = "GC_ITFACE where G3E_FID:" + isFID.ToString();
                                    flag = false;
                                }
                                break;
                                #endregion
                            case 10800:
                                #region Joint/Splice

                                o_Comp = objFeat.Components.GetComponent(10801);
                                o_Comp.Recordset.MoveFirst();
                                isFID = Convert.ToDouble(o_Comp.Recordset.Fields["G3E_FID"].Value.ToString());
                                string jointCode = o_Comp.Recordset.Fields["CABLE_CODE"].Value.ToString();
                                string jointClass = o_Comp.Recordset.Fields["SPLICE_CLASS"].Value.ToString().Trim().ToUpper();

                                if (jointClass == "MAIN JOINT")
                                {
                                    if (!jointCode.Equals(""))
                                    {
                                        // Check Format Code
                                        string formatOK = string.Empty;
                                        string jointSQL = "SELECT A.G3E_FID FROM GC_SPLICE A, GC_NETELEM B WHERE A.G3E_FID <> " + objFeat.FID.ToString();
                                        jointSQL += " AND A.CABLE_CODE = '" + jointCode + "' AND UPPER(A.SPLICE_CLASS) = 'MAIN JOINT'";
                                        jointSQL += " AND A.G3E_FID = B.G3E_FID AND B.EXC_ABB = '" + isExcAbb + "'";
                                        rsChk = m_GTDataContext.OpenRecordset(jointSQL, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);
                                        if (rsChk.RecordCount > 0)
                                        {
                                            strMsg = "Cable code already used by others";
                                            strLoc = "GC_SPLICE where G3E_FID:" + isFID.ToString();
                                            flag = false;
                                        }
                                    }
                                }
                                break;
                                #endregion
                            case 13000: // added on 2012-10-02
                                #region DP [13100 : GC_DP]
                                o_Comp = objFeat.Components.GetComponent(13001);
                                string dpnum = o_Comp.Recordset.Fields["DP_NUM"].Value.ToString().Trim();
                                string itface = o_Comp.Recordset.Fields["ITFACE_CODE"].Value.ToString().Trim();
                                string rtcode = o_Comp.Recordset.Fields["RT_CODE"].Value.ToString().Trim();
                                if (GetDPNumber.DuplicateCode(isExcAbb, itface, rtcode, dpnum, objFeat.FID))
                                {
                                    flag = false;
                                    strLoc = "GC_DP where G3E_FID:" + objFeat.FID.ToString();
                                    strMsg = "Duplicate DP Number";
                                }
                                break;
                                #endregion
                            case 13100: // added on 2012-10-02
                                #region PDP [13100 : GC_PDP]
                                //GetPDPCode.AssignCode(
                                break;
                                #endregion
                        }



                        if (!attcflag)
                        {
                            strMsg = attch.strMsg;
                            strLoc = attch.strLoc;
                            flag = false;
                        }
                    }
                    isFNO = Convert.ToDouble(objFeat.FNO.ToString());
                    isCNO = Convert.ToDouble(objFeat.CNO.ToString());
                    isCID = Convert.ToDouble(objFeat.CID.ToString());
                    isFID = Convert.ToDouble(objFeat.FID.ToString());
                }
                
                values[0] = "P1";
                values[1] = strMsg;
                values[2] = strLoc;
                values[3] = strConn;
                values[4] = isFNO;
                values[5] = isFID;
                values[6] = isCNO;
                values[7] = isCID;
                if (!flag)
                {
                    ValidationErrors.AddNew(fieldList, values);
                }
                
                #region Copper Features Duplicate Code Validation
                //Copper Features Duplicate Code Validation Vinod 27-Nov-2012

                string Error = null;
                IGTComponent oComp = null;
                string sSql = null;
                ADODB.Recordset rsPP = new ADODB.Recordset();

                //AG_VALIDATION_BYPASS
                string ACTION = Get_Value("SELECT ACTION FROM AG_VALIDATION_BYPASS WHERE FNO = " + objFeat.FNO + " and VALIDATION_TYPE = 'DUPLICATE'");
                
                if (ACTION == "TEST")
                {
                    //GC_DP -- 13000 DP 
                    //DP Number Unique for CABLE_CODE and DP_NUM
                    if (objFeat.FNO == 13000)
                    {
                        oComp = objFeat.Components.GetComponent(13001);
                        oComp.Recordset.MoveFirst();
                        
                        if (oComp.Recordset.Fields["DP_NUM"].Value.ToString() == "")
                        {
                            Error = "DP Number Cannot be Empty.";
                            flag = false;
                        }
                        else if (oComp.Recordset.Fields["CABLE_CODE"].Value.ToString() == "")
                        {
                            Error = "Cable Code Cannot be Empty.";
                            flag = false;
                        }
                        else if (oComp.Recordset.Fields["DP_NUM"].Value.ToString() != "" && oComp.Recordset.Fields["CABLE_CODE"].Value.ToString() != "")
                        {
                            sSql = "select DP_NUM from GC_DP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + isExcAbb + "' and CABLE_CODE = '" + oComp.Recordset.Fields["CABLE_CODE"].Value.ToString() + "' and DP_NUM = '" + oComp.Recordset.Fields["DP_NUM"].Value.ToString() + "' and A.G3E_FID <> " + oComp.Recordset.Fields["G3E_FID"].Value.ToString();
                            rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                            if (rsPP.RecordCount > 0)
                            {
                                Error = "DP Number " + oComp.Recordset.Fields["DP_NUM"].Value.ToString() + " and Cable Code " + oComp.Recordset.Fields["CABLE_CODE"].Value.ToString() + " Already Exists.";
                                flag = false;
                            }
                        }                        
                    }

                    //GC_PDP -- 13100 PDP
                    if (objFeat.FNO == 13100)
                    {
                        oComp = objFeat.Components.GetComponent(13101);
                        oComp.Recordset.MoveFirst();

                        if (oComp.Recordset.Fields["PDP_CODE"].Value.ToString() != "")
                        {
                            sSql = "select PDP_CODE from GC_PDP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + isExcAbb + "' and PDP_CODE = '" + oComp.Recordset.Fields["PDP_CODE"].Value.ToString() + "' and A.G3E_FID <> " + oComp.Recordset.Fields["G3E_FID"].Value.ToString();
                            rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                            if (rsPP.RecordCount > 0)
                            {
                                Error = "PDP CODE " + oComp.Recordset.Fields["PDP_CODE"].Value.ToString() + " Already Exists.";
                                flag = false;
                            }
                        }
                        else
                        {
                            Error = "PDP CODE Cannot be Empty.";
                            flag = false;
                        }
                    }

                    //GC_DDP -- 6300 DDP
                    if (objFeat.FNO == 6300)
                    {
                        oComp = objFeat.Components.GetComponent(6301);
                        oComp.Recordset.MoveFirst();

                        if (oComp.Recordset.Fields["DDP_NUM"].Value.ToString() != "")
                        {
                            sSql = "select DDP_NUM from GC_DDP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + isExcAbb + "' and DDP_NUM = '" + oComp.Recordset.Fields["DDP_NUM"].Value.ToString() + "' and A.G3E_FID <> " + oComp.Recordset.Fields["G3E_FID"].Value.ToString();
                            rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                            if (rsPP.RecordCount > 0)
                            {
                                Error = "DDP NUM " + oComp.Recordset.Fields["DDP_NUM"].Value.ToString() + " Already Exists.";
                                flag = false;
                            }
                        }
                        else
                        {
                            Error = "DDP NUM Cannot be Empty.";
                            flag = false;
                        }
                    }

                    //GC_PDDP -- 6200 PDDP
                    if (objFeat.FNO == 6200)
                    {
                        oComp = objFeat.Components.GetComponent(6201);
                        oComp.Recordset.MoveFirst();

                        if (oComp.Recordset.Fields["PDDP_CODE"].Value.ToString() != "")
                        {
                            sSql = "select PDDP_CODE from GC_PDDP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + isExcAbb + "' and PDDP_CODE = '" + oComp.Recordset.Fields["PDDP_CODE"].Value.ToString() + "' and A.G3E_FID <> " + oComp.Recordset.Fields["G3E_FID"].Value.ToString();
                            rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                            if (rsPP.RecordCount > 0)
                            {
                                Error = "PDDP CODE " + oComp.Recordset.Fields["PDDP_CODE"].Value.ToString() + " Already Exists.";
                                flag = false;
                            }
                        }
                        else
                        {
                            Error = "PDDP CODE Cannot be Empty.";
                            flag = false;
                        }
                    }

                    //GC_ITFACE -- 10300 CAB
                    if (objFeat.FNO == 10300)
                    {
                        oComp = objFeat.Components.GetComponent(10301);
                        oComp.Recordset.MoveFirst();

                        if (oComp.Recordset.Fields["ITFACE_CODE"].Value.ToString() != "")
                        {
                            sSql = "select ITFACE_CODE from GC_ITFACE A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + isExcAbb + "' and ITFACE_CODE = '" + oComp.Recordset.Fields["ITFACE_CODE"].Value.ToString() + "' and A.G3E_FID <> " + oComp.Recordset.Fields["G3E_FID"].Value.ToString();
                            rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                            if (rsPP.RecordCount > 0)
                            {
                                Error = "CABINET CODE " + oComp.Recordset.Fields["ITFACE_CODE"].Value.ToString() + " Already Exists.";
                                flag = false;
                            }
                        }
                        else
                        {
                            Error = "CABINET CODE Cannot be Empty.";
                            flag = false;
                        }
                    }
                }
                #endregion

                if (Error != null && flag == false)
                {
                    values[0] = "P1";
                    values[1] = Error;
                    values[2] = null;
                    values[3] = "";
                    values[4] = oComp.Recordset.Fields["G3E_FNO"].Value.ToString();
                    values[5] = oComp.Recordset.Fields["G3E_FID"].Value.ToString();
                    values[6] = oComp.Recordset.Fields["G3E_CNO"].Value.ToString();
                    values[7] = oComp.Recordset.Fields["G3E_CID"].Value.ToString();

                    ValidationErrors.AddNew(fieldList, values);
                }
                //End Copper Features Duplicate Code Validation

                return flag;
            }
            catch (Exception exception)
            {
                if (values[1].ToString().Length > 0)
                {
                    values[1] = string.Concat(new object[] { values[1], " | Error: ", exception.Source, "-", exception.Message });
                }
                else
                {
                    values[1] = "Error: " + exception.Source + "-" + exception.Message;
                }
                ValidationErrors.AddNew(fieldList, values);
                return false;
            }
        }

        //Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private bool ValidGeometry(IGTKeyObject objFeat, short CNO)
        {
            IGTComponent o_Comp = null;
            o_Comp = objFeat.Components.GetComponent(CNO);
            o_Comp.Recordset.MoveFirst();
            return (!o_Comp.Recordset.EOF);
        }


        public GTArguments Arguments
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                this.m_GTArguments = value;
            }
        }

        public IGTDataContext DataContext
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                m_GTDataContext = value;
            }
        }

        public string ProcessingMode
        {
            get
            {
                return this.m_Mode;
            }
            set
            {
                this.m_Mode = value;
            }
        }
    }
}

