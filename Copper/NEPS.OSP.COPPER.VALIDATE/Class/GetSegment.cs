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
    public class GetSegment : Intergraph.GTechnology.Interfaces.IGTFunctional
    {
        #region IGTFunctional Members

        private GTArguments m_GTArguments = null;
        private IGTComponents m_GTComponents;
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

        public void Execute()
        {
            try
            {
                // Declaration
                string strJobID = string.Empty;
                string strSql = string.Empty;

                // Component for Splice
                IGTComponent oComp = m_GTComponents.GetComponent(51);
                
                // Get ExcAbb base on Job
                strJobID = DataContext.ActiveJob.ToString();
                ADODB.Recordset rsSQL = new ADODB.Recordset();
                strSql = "SELECT SEGMENT FROM G3E_JOB WHERE G3E_IDENTIFIER = '" + strJobID + "'";
                rsSQL = m_GTDataContext.OpenRecordset(strSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsSQL.RecordCount > 0)
                {
                    rsSQL.MoveFirst();
                    oComp.Recordset.Update("SEGMENT", rsSQL.Fields[0].Value.ToString());
                }
                else
                {
                    rsSQL.MoveFirst();
                    oComp.Recordset.Update("SEGMENT", "");
                }
                rsSQL = null;
                strSql = null;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        #endregion
    }
}
