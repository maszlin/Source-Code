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
    public class GetCabinetCode : Intergraph.GTechnology.Interfaces.IGTFunctional
    {
        #region IGTFunctional Members

        private GTArguments m_GTArguments = null;
        private IGTComponents m_GTComponents;
        private string m_Mode = null;
        private IGTDataContext m_GTDataContext = null;

        public Intergraph.GTechnology.API.GTArguments Arguments
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                m_GTArguments = value;
            }
        }

        public string ComponentName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Intergraph.GTechnology.API.IGTComponents Components
        {
            get
            {
                return m_GTComponents;
            }
            set
            {
                m_GTComponents = value;
            }
        }

        public Intergraph.GTechnology.API.IGTDataContext DataContext
        {
            get
            {
                return m_GTDataContext;
            }
            set
            {
                m_GTDataContext = value;
            }
        }

        public string FieldName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Intergraph.GTechnology.API.IGTFieldValue FieldValueBeforeChange
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public string ProcessingMode
        {
            get
            {
                //throw new System.NotImplementedException();
                return this.m_Mode;
            }
            set
            {
                this.m_Mode = value;
            }
        }

        public Intergraph.GTechnology.Interfaces.GTFunctionalTypeConstants Type
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public void Validate(out string[] ErrorPriorityArray, out string[] ErrorMessageArray)
        {
            bool bErrors = false;
            if (bErrors)
            {
                ErrorPriorityArray = new string[2];
                ErrorMessageArray = new string[2];
                ErrorPriorityArray[0] = "P1";
                ErrorMessageArray[0] = "First .NET Sample Error";
                ErrorPriorityArray[1] = "P3";
                ErrorMessageArray[1] = "Second .NET Sample Error";
            }
            else
            {
                ErrorPriorityArray = null;
                ErrorMessageArray = null;
            }

            GTClassFactory.Create<IGTApplication>().Application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "From .NET Sample: Inside Validate");
        }

        public void Delete()
        {
            GTClassFactory.Create<IGTApplication>().Application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "From .NET Sample: Inside Delete");
        }


        public void Execute()
        {
            try
            {
                // Declaration
                string strJobID = string.Empty;
                string strSql = string.Empty;
                string isExcAbb = string.Empty;
                string cabCode = string.Empty;

                // Component for Cabinet/ITFACE
                IGTComponent oComp = m_GTComponents.GetComponent(10301);
                oComp.Recordset.MoveFirst();

                string cabClass = oComp.Recordset.Fields["ITFACE_CLASS"].Value.ToString();


                // Get ExcAbb base on Job
                strJobID = DataContext.ActiveJob.ToString();
                ADODB.Recordset rsSQL = new ADODB.Recordset();
                strSql = "SELECT GC_OSP_COP_VAL.CNO_10301_GET_ITFACE_CODE('" + strJobID + "','" + cabClass + "') FROM DUAL";

                rsSQL = m_GTDataContext.OpenRecordset(strSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsSQL.RecordCount > 0)
                {
                    cabCode = rsSQL.Fields[0].Value.ToString().Trim();

                    if (cabClass == "SDF")
                        oComp.Recordset.Update("SDF_CODE", cabCode);
                    else if (cabClass == "PHANTOM CABINET")
                        oComp.Recordset.Update("PCAB_CODE", cabCode);

                    oComp.Recordset.Update("ITFACE_CODE", cabCode);
                }
                else
                {
                    if (cabClass == "SDF")
                        oComp.Recordset.Update("SDF_CODE", "0001");
                    else if (cabClass == "PHANTOM CABINET")
                        oComp.Recordset.Update("PCAB_CODE", "901");
                    else if (cabClass == "CABINET")
                        oComp.Recordset.Update("PCAB_CODE", "001");
                }

                rsSQL = null;
                strSql = null;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        public static bool GenerateCode(IGTComponent oComp)
        {
            try
            {
                // Declaration
                string strJobID = string.Empty;
                string strSql = string.Empty;
                string isExcAbb = string.Empty;

                // Component for Splice
                //IGTComponent oComp = cab.Components.GetComponent(10301);
                string cabClass = oComp.Recordset.Fields["ITFACE_CLASS"].Value.ToString();

                // Get ExcAbb base on Job
                strJobID = GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob.ToString();
                ADODB.Recordset rsSQL = new ADODB.Recordset();
                strSql = "SELECT GC_OSP_COP_VAL.CNO_10301_GET_ITFACE_CODE('" + strJobID + "', '" + cabClass + "') FROM DUAL";
                rsSQL = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(
                    strSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);

                if (rsSQL.RecordCount > 0)
                {
                    rsSQL.MoveFirst();
                    string cabCode = rsSQL.Fields[0].Value.ToString().Trim();
                    oComp.Recordset.Update("ITFACE_CODE", cabCode);
                    if (cabClass == "SDF")
                        oComp.Recordset.Update("SDF_CODE", cabCode);
                    else if (cabClass == "PHANTOM CABINET")
                        oComp.Recordset.Update("PCAB_CODE", cabCode);

                }
                else
                {
                    rsSQL.MoveFirst();
                    oComp.Recordset.Update("ITFACE_CODE", rsSQL.Fields[0].Value.ToString().Trim());
                }
                rsSQL = null;
                strSql = null;
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return false;
            }

        }

        public static bool ChkFormatCode(string itface_code, string itface_class)
        {
            try
            {
                if (itface_class == "SDF")
                    return (itface_code.Length == 4);
                else
                {
                    if (itface_code.Length == 3)
                    {
                        int code = myUtil.ParseInt(itface_code);
                        if (code != 0)
                        {
                            if (itface_class == "CABINET")
                                return (code < 901);
                            else
                                return (code > 900);
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DuplicateCode(string itface_code, double iFID, string isExcAbb)
        {
            try
            {
                string ssql = "SELECT A.G3E_FID FROM GC_ITFACE A, GC_NETELEM B " +
                    "WHERE A.G3E_FID = B.G3E_FID AND A.G3E_FID <> " + iFID + " AND B.EXC_ABB = '" + isExcAbb + "' AND A.ITFACE_CODE = '" + itface_code + "'";
                ADODB.Recordset rsChk = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(
                    ssql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);

                return (rsChk.RecordCount > 0);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
    }
}
