/*
 * GetJointAttribute.cs
 * - read connected cable attributes from AG_CABLE_JOINT and opend and send value to frmJointAttribute
 *  
 * edited : m.zam @ 12-09-2012
 * issues :
 * - read CAB_TYPE from AG_CABLE_JOINT - this to handle copper cable connected to Fiber Cabinet
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.VALIDATE
{
    class GetJointAttribute : Intergraph.GTechnology.Interfaces.IGTFunctional
    {
        #region IGTFunctional Members

        private GTArguments m_GTArguments = null;
        public static IGTComponents m_GTComponents;
        public static IGTDataContext m_GTDataContext = null;
        public static Logger log;

        #region standard code from sample
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
                throw new System.NotImplementedException();
            }
            set
            {
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

        #endregion

        public void Execute()
        {
            try
            {
                //Get Exchange
                string Exch = string.Empty;
                ADODB.Recordset rsE = new ADODB.Recordset();

                rsE = m_GTDataContext.OpenRecordset("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_GTDataContext.ActiveJob.ToString() + "'", ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsE.RecordCount > 0)
                {
                    Exch = rsE.Fields[0].Value.ToString();
                }
                rsE.Close();

                string filename = "COPPER_PLACE_JOINT";
                log = Logger.getInstance();
                log.OpenFile(filename);

                // get joint FID from gc_splice
                IGTComponent oComp = GetJointAttribute.m_GTComponents.GetComponent(10801);
                oComp.Recordset.MoveFirst();
                int jFID = int.Parse(oComp.Recordset.Fields["G3E_FID"].Value.ToString());
                string jClass = oComp.Recordset.Fields["SPLICE_CLASS"].Value.ToString();
                if ((jClass.IndexOf("STU") == -1) && (jClass.IndexOf("MAIN") == -1))
                {
                    string[] cableAttr = GetCableAttribute(jFID);
                    if (cableAttr != null)
                    {
                        frmJointAttribute f = new frmJointAttribute(jFID, cableAttr);
                        f.ShowDialog();
                    }
                    DeleteCableAttribute(jFID);
                }
            }
            catch (Exception ex) { log.WriteLog(ex.Message); }
            finally { log.CloseFile(); }

        }

        #endregion

        #region myMethod

        private string[] GetCableAttribute(int jFID)
        {
            string ssql = "SELECT * FROM AG_CABLE_JOINT WHERE JOINT_FID = " + jFID.ToString();
            ADODB.Recordset rsSQL = new ADODB.Recordset();

            rsSQL = GetJointAttribute.m_GTDataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
            if (rsSQL.RecordCount > 0)
            {
                rsSQL.MoveFirst();
                string[] attr = new string[] {
                    rsSQL.Fields["CABLE_FID"].Value.ToString(),
                    rsSQL.Fields["ITFACE_CODE"].Value.ToString(),
                    rsSQL.Fields["RT_CODE"].Value.ToString(),
                    rsSQL.Fields["CABLE_CODE"].Value.ToString(),
                    rsSQL.Fields["CTYPE"].Value.ToString(),
                    rsSQL.Fields["TOTAL_SIZE"].Value.ToString(),
                    rsSQL.Fields["GAUGE"].Value.ToString()      
                };
                return attr;
            }
            else
            {
                log.WriteLog("REC.NOT FOUND - " + ssql);
                return null;
            }
        }

        private void DeleteCableAttribute(int jFID)
        {
            string ssql = "DELETE FROM AG_CABLE_JOINT WHERE JOINT_FID = " + jFID.ToString();
            log.WriteLog(ssql);

            int iR;
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        }

        #endregion
    }
}
