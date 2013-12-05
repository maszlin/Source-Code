using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Text.RegularExpressions;
using ADODB;

//VERSION 3.4

namespace NEPS.GEN_CODE
{
    public class GetCode : Intergraph.GTechnology.Interfaces.IGTFunctional
    {

        private GTArguments m_GTArguments = null;
        private IGTComponents m_GTComponents;
        private IGTDataContext m_GTDataContext = null;
        private string m_GTComponent = null;
        private string m_FieldName = null;
        private string m_Mode = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null; 

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
                return m_GTComponent;
            }
            set
            {
                m_GTComponent = value;
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
                //throw new System.NotImplementedException();
                return m_FieldName;
            }
            set
            {
                m_FieldName = value;
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
            GTClassFactory.Create< IGTApplication >().Application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "From .NET Sample: Inside Delete");
        }
                
        public string ProcessingMode
        {
            get {
                //throw new System.NotImplementedException();
                return this.m_Mode;
            }
            set {
                this.m_Mode = value;
            }                  
        }

        public void Execute()
        {
            IGTComponent oComp;
            string sSql = null;
            string Exch = null;
            int maxNum = 0;
            string tCode = null;

            try
            {
                string Mode = this.m_Mode;
                //Get Exchange
                ADODB.Recordset rsE = new ADODB.Recordset();
                rsE = m_GTDataContext.OpenRecordset("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_GTDataContext.ActiveJob.ToString() + "'", ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsE.RecordCount > 0)
                {
                    Exch = rsE.Fields[0].Value.ToString();
                }
                rsE.Close();
                ADODB.Recordset rsPP = new ADODB.Recordset();

                //------------------- DEVICE ---------------------------
                //GC_UPE -- 5400 UPE
                if (m_GTComponent == "GC_UPE")
                {
                    oComp = m_GTComponents.GetComponent(5401);                    
                    oComp.Recordset.MoveFirst();

                    sSql = "select substr(UPE_CODE,2,length(UPE_CODE)) from GC_UPE A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and UPE_CODE <> '***'";
                    //sSql = "select UPE_CODE from GC_UPE A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("UPE_CODE", "U01");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                            
                            oComp.Recordset.Update("UPE_CODE", "U" + tCode);
                        }
                    }
                    else oComp.Recordset.Update("UPE_CODE", "U01");
                }

                //GC_EPE -- 5200 EPE
                if (m_GTComponent == "GC_EPE")
                {
                    oComp = m_GTComponents.GetComponent(5201);
                    oComp.Recordset.MoveFirst();

                    sSql = "select substr(EPE_CODE,2,length(EPE_CODE)) from GC_EPE A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and EPE_CODE <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("EPE_CODE", "E0001");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "000" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 2) tCode = "00" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 3) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 4) tCode = maxNum.ToString();

                            oComp.Recordset.Update("EPE_CODE", "E" + tCode);
                        }
                    }else oComp.Recordset.Update("EPE_CODE", "E0001");
                }

              
                //GC_RT -- 9600 RT
                if (m_GTComponent == "GC_RT")
                {
                    oComp = m_GTComponents.GetComponent(9601);
                    oComp.Recordset.MoveFirst();

                    if (oComp.Recordset.Fields["RT_TYPE"].Value.ToString() == "FTTO")
                    {
                        //Vinod 19-July-2012 Request from Kelvin to Remove the RT_TYPE, SQL from Mike on 17-Sep-2012, Kamal asked to add RT_TYPE on 18-Sep-2012
                        //sSql = "select max(substr(RT_CODE,6,length(RT_CODE))) from GC_RT A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_TYPE = 'FTTO' and RT_CODE <> '***'";
                        sSql = "select substr(RT_CODE,6,4) from GC_RT A, GC_NETELEM B  where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_CODE like 'F999-%' and RT_TYPE = 'FTTO' and RT_CODE <> '***' ";
                        rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                        if (rsPP.RecordCount > 0)
                        {
                            if (rsPP.Fields[0].Value.ToString() == "")
                            {
                                oComp.Recordset.Update("RT_CODE", "F999-0001");
                            }
                            else
                            {
                                rsPP.MoveFirst();
                                for (int i = 0; i < rsPP.RecordCount; i++)
                                {
                                    int tempint = 0;
                                    if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                    {
                                        if (maxNum < tempint)
                                            maxNum = tempint;
                                    }
                                    rsPP.MoveNext();


                                }
                                maxNum = maxNum + 1;
                                if (maxNum.ToString().Length == 1) tCode = "000" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 2) tCode = "00" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 3) tCode = "0" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 4) tCode = maxNum.ToString();
                                else tCode = maxNum.ToString();

                                oComp.Recordset.Update("RT_CODE", "F999-" + tCode);
                            }
                        }else oComp.Recordset.Update("RT_CODE", "F999-0001");
                    }
                    else if (oComp.Recordset.Fields["RT_TYPE"].Value.ToString() == "FTTS")
                    {
                        //Vinod 19-July-2012 Request from Kelvin to Remove the RT_TYPE, SQL from Mike on 17-Sep-2012 , Kamal asked to add RT_TYPE on 18-Sep-2012
                        //sSql = "select max(substr(RT_CODE,2,length(RT_CODE))) from GC_RT A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_TYPE = 'FTTS' and RT_CODE <> '***'";
                        sSql = "select substr(RT_CODE,2,3) from GC_RT A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_CODE like 'F%' and RT_CODE not like 'F9%' and length(RT_CODE)=4 and RT_TYPE = 'FTTS' and  RT_CODE <> '***'";
                        rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                        if (rsPP.RecordCount > 0)
                        {
                            if (rsPP.Fields[0].Value.ToString() == "")
                            {
                                oComp.Recordset.Update("RT_CODE", "F001");
                            }
                            else
                            {
                                rsPP.MoveFirst();
                                for (int i = 0; i < rsPP.RecordCount; i++)
                                {
                                    int tempint = 0;
                                    if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                    {
                                        if (maxNum < tempint)
                                            maxNum = tempint;
                                    }
                                    rsPP.MoveNext();


                                }
                                maxNum = maxNum + 1;
                                if (maxNum.ToString().Length == 1) tCode = "00" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 2) tCode = "0" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 3) tCode = maxNum.ToString();
                                else tCode = maxNum.ToString();

                                oComp.Recordset.Update("RT_CODE", "F" + tCode);
                            }
                        }else oComp.Recordset.Update("RT_CODE", "F001");
                    }
                    else if (oComp.Recordset.Fields["RT_TYPE"].Value.ToString() == "PFTTS")
                    {
                        //Vinod 19-July-2012 Request from Kelvin to Remove the RT_TYPE, SQL from Mike on 17-Sep-2012 , Kamal asked to add RT_TYPE on 18-Sep-2012
                        //sSql = "select max(substr(RT_CODE,3,length(RT_CODE))) from GC_RT A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_TYPE = 'PFTTS' and RT_CODE <> '***'";
                        sSql = "select substr(RT_CODE,3,2) from GC_RT A, GC_NETELEM B  where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_CODE like 'F9%' and length(RT_CODE)=4 and RT_TYPE = 'PFTTS' and RT_CODE <> '***'";
                        rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                        if (rsPP.RecordCount > 0)
                        {
                            if (rsPP.Fields[0].Value.ToString() == "")
                            {
                                oComp.Recordset.Update("RT_CODE", "F901");
                            }
                            else
                            {
                                rsPP.MoveFirst();
                                for (int i = 0; i < rsPP.RecordCount; i++)
                                {
                                    int tempint = 0;
                                    if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                    {
                                        if (maxNum < tempint)
                                            maxNum = tempint;
                                    }
                                    rsPP.MoveNext();


                                }
                                maxNum = maxNum + 1;
                                if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 2) tCode = maxNum.ToString();
                                else tCode = maxNum.ToString();

                                oComp.Recordset.Update("RT_CODE", "F9" + tCode);
                            }
                        }else oComp.Recordset.Update("RT_CODE", "F901");
                    }
                    else if (oComp.Recordset.Fields["RT_TYPE"].Value.ToString() == "FTTZ")
                    {
                        //Vinod 19-July-2012 Request from Kelvin to Remove the RT_TYPE, SQL from Mike on 17-Sep-2012 , Kamal asked to add RT_TYPE on 18-Sep-2012
                        //sSql = "select max(substr(RT_CODE,2,length(RT_CODE))) from GC_RT A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_TYPE = 'FTTZ' and RT_CODE <> '***'";
                        sSql = "select substr(RT_CODE,2,3) from GC_RT A, GC_NETELEM B  where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_CODE like 'F%' and RT_CODE not like 'F9%' and length(RT_CODE)=4 and RT_TYPE = 'FTTZ' and RT_CODE <> '***'";
                        rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                        if (rsPP.RecordCount > 0)
                        {
                            if (rsPP.Fields[0].Value.ToString() == "")
                            {
                                oComp.Recordset.Update("RT_CODE", "F001");
                            }
                            else
                            {
                                rsPP.MoveFirst();
                                for (int i = 0; i < rsPP.RecordCount; i++)
                                {
                                    int tempint = 0;
                                    if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                    {
                                        if (maxNum < tempint)
                                            maxNum = tempint;
                                    }
                                    rsPP.MoveNext();


                                }
                                maxNum = maxNum + 1;
                                if (maxNum.ToString().Length == 1) tCode = "00" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 2) tCode = "0" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 3) tCode = maxNum.ToString();
                                else tCode = maxNum.ToString();

                                oComp.Recordset.Update("RT_CODE", "F" + tCode);
                            }
                        }else oComp.Recordset.Update("RT_CODE", "F001");
                    }
                }

                //GC_MSAN -- 9100 MSAN
                if (m_GTComponent == "GC_MSAN")
                {
                    oComp = m_GTComponents.GetComponent(9101);
                    oComp.Recordset.MoveFirst();

                    if (oComp.Recordset.Fields["RT_TYPE"].Value.ToString() == "FTTO")
                    {
                        //Vinod 19-July-2012 Request from Kelvin to Remove the RT_TYPE
                        //sSql = "select max(substr(RT_CODE,8,length(RT_CODE))) from GC_MSAN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_TYPE = 'FTTO' and RT_CODE <> '***'";
                        sSql = "select substr(RT_CODE,8,2) from GC_MSAN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_CODE like 'F999-03%' and RT_CODE <> '***'";
                        rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                        if (rsPP.RecordCount > 0)
                        {
                            if (rsPP.Fields[0].Value.ToString() == "")
                            {
                                oComp.Recordset.Update("RT_CODE", "F999-0301");
                            }
                            else
                            {
                                rsPP.MoveFirst();
                                for (int i = 0; i < rsPP.RecordCount; i++)
                                {
                                    int tempint = 0;
                                    if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                    {
                                        if (maxNum < tempint)
                                            maxNum = tempint;
                                    }
                                    rsPP.MoveNext();


                                }
                                maxNum = maxNum + 1;
                                if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 2) tCode = maxNum.ToString();
                                else tCode = maxNum.ToString();

                                oComp.Recordset.Update("RT_CODE", "F999-03" + tCode);
                            }
                        }
                        else oComp.Recordset.Update("RT_CODE", "F999-0301");
                    }
                    else if (oComp.Recordset.Fields["RT_TYPE"].Value.ToString() == "FTTS")
                    {
                        //Vinod 19-July-2012 Request from Kelvin to Remove the RT_TYPE
                        //sSql = "select max(substr(RT_CODE,3,length(RT_CODE))) from GC_MSAN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_TYPE = 'FTTS' and RT_CODE <> '***'";
                        sSql = "select substr(RT_CODE,3,2) from GC_MSAN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_CODE like 'F3%' and RT_CODE <> '***'";
                        rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                        if (rsPP.RecordCount > 0)
                        {
                            if (rsPP.Fields[0].Value.ToString() == "")
                            {
                                oComp.Recordset.Update("RT_CODE", "F301");
                            }
                            else
                            {
                                rsPP.MoveFirst();
                                for (int i = 0; i < rsPP.RecordCount; i++)
                                {
                                    int tempint = 0;
                                    if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                    {
                                        if (maxNum < tempint)
                                            maxNum = tempint;
                                    }
                                    rsPP.MoveNext();


                                }
                                maxNum = maxNum + 1;
                                if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 2) tCode = maxNum.ToString();
                                else tCode = maxNum.ToString();

                                oComp.Recordset.Update("RT_CODE", "F3" + tCode);
                            }
                        }
                        else oComp.Recordset.Update("RT_CODE", "F301");
                    }
                    else if (oComp.Recordset.Fields["RT_TYPE"].Value.ToString() == "PFTTS")
                    {
                        //Vinod 19-July-2012 Request from Kelvin to Remove the RT_TYPE
                        //sSql = "select max(substr(RT_CODE,4,length(RT_CODE))) from GC_MSAN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_TYPE = 'PFTTS' and RT_CODE <> '***'";
                        sSql = "select substr(RT_CODE,4,1) from GC_MSAN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_CODE like 'F91%' and RT_CODE <> '***'";
                        rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                        if (rsPP.RecordCount > 0)
                        {
                            if (rsPP.Fields[0].Value.ToString() == "")
                            {
                                oComp.Recordset.Update("RT_CODE", "F910");
                            }
                            else
                            {
                                rsPP.MoveFirst();
                                for (int i = 0; i < rsPP.RecordCount; i++)
                                {
                                    int tempint = 0;
                                    if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                    {
                                        if (maxNum < tempint)
                                            maxNum = tempint;
                                    }
                                    rsPP.MoveNext();


                                }
                                maxNum = maxNum + 1;
                                tCode = maxNum.ToString();
                                oComp.Recordset.Update("RT_CODE", "F91" + tCode);
                            }
                        }
                        else oComp.Recordset.Update("RT_CODE", "F910");
                    }
                    else if (oComp.Recordset.Fields["RT_TYPE"].Value.ToString() == "FTTZ")
                    {
                        //Vinod 19-July-2012 Request from Kelvin to Remove the RT_TYPE
                        //sSql = "select max(substr(RT_CODE,3,length(RT_CODE))) from GC_MSAN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_TYPE = 'FTTZ' and RT_CODE <> '***'";
                        sSql = "select substr(RT_CODE,3,2) from GC_MSAN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_CODE like 'F3%' and RT_CODE <> '***'";
                        rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                        if (rsPP.RecordCount > 0)
                        {
                            if (rsPP.Fields[0].Value.ToString() == "")
                            {
                                oComp.Recordset.Update("RT_CODE", "F301");
                            }
                            else
                            {
                                rsPP.MoveFirst();
                                for (int i = 0; i < rsPP.RecordCount; i++)
                                {
                                    int tempint = 0;
                                    if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                    {
                                        if (maxNum < tempint)
                                            maxNum = tempint;
                                    }
                                    rsPP.MoveNext();


                                }
                                maxNum = maxNum + 1;
                                if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 2) tCode = maxNum.ToString();
                                else tCode = maxNum.ToString();

                                oComp.Recordset.Update("RT_CODE", "F3" + tCode);
                            }
                        }
                        else oComp.Recordset.Update("RT_CODE", "F301");
                    }
                }

                //GC_VDSL2 -- 9800 VDSL2 //Anna add
                if (m_GTComponent == "GC_VDSL2")
                {
                    oComp = m_GTComponents.GetComponent(9801);
                    oComp.Recordset.MoveFirst();

                    
                        sSql = "select substr(RT_CODE,3,3) from GC_VDSL2 A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and RT_CODE like 'V1%' and RT_CODE <> '***'";
                        rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                        if (rsPP.RecordCount > 0)
                        {
                            if (rsPP.Fields[0].Value.ToString() == "")
                            {
                                oComp.Recordset.Update("RT_CODE", "V1001");
                            }
                            else
                            {
                                rsPP.MoveFirst();
                                for (int i = 0; i < rsPP.RecordCount; i++)
                                {
                                    int tempint = 0;
                                    if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                    {
                                        if (maxNum < tempint)
                                            maxNum = tempint;
                                    }
                                    rsPP.MoveNext();


                                }
                                maxNum = maxNum + 1;
                                if (maxNum.ToString().Length == 1) tCode = "00" + maxNum.ToString();
                                else if (maxNum.ToString().Length == 2) tCode ="0"+ maxNum.ToString();
                                else if (maxNum.ToString().Length >= 3) tCode = maxNum.ToString();
                              

                                oComp.Recordset.Update("RT_CODE", "V1" + tCode);
                            }
                        }
                        else oComp.Recordset.Update("RT_CODE", "V1001");
                    
                   
                  
                }

                //GC_ODF -- 5500 ODF
                if (m_GTComponent == "GC_ODF")
                {
                    oComp = m_GTComponents.GetComponent(5501);
                    oComp.Recordset.MoveFirst();

                    sSql = "select ODF_NUM from GC_ODF A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and ODF_NUM <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("ODF_NUM", "0001");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "000" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 2) tCode = "00" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 3) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 4) tCode = maxNum.ToString();

                            oComp.Recordset.Update("ODF_NUM", tCode);
                        }
                    }else oComp.Recordset.Update("ODF_NUM", "0001");
                }

                //GC_MINIMUX -- 9500 MINIMUX
                if (m_GTComponent == "GC_MINIMUX")
                {
                    oComp = m_GTComponents.GetComponent(9501);
                    oComp.Recordset.MoveFirst();

                    sSql = "select substr(MUX_CODE,6,length(MUX_CODE)) from GC_MINIMUX A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and MUX_CODE <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("MUX_CODE", "M999-0001");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "000" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 2) tCode = "00" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 3) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 4) tCode = maxNum.ToString();

                            oComp.Recordset.Update("MUX_CODE", "M999-" + tCode);
                        }
                    }else oComp.Recordset.Update("MUX_CODE", "M999-0001");
                }

                //GC_NDH -- 9400 NDH
                if (m_GTComponent == "GC_NDH")
                {
                    oComp = m_GTComponents.GetComponent(9401);
                    oComp.Recordset.MoveFirst();

                    sSql = "select substr(NDH_CODE,4,length(NDH_CODE)) from GC_NDH A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and NDH_CODE <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("NDH_CODE", "FDH01");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 2) tCode = maxNum.ToString();

                            oComp.Recordset.Update("NDH_CODE", "FDH" + tCode);
                        }
                    }else oComp.Recordset.Update("NDH_CODE", "FDH01");
                }

                //GC_DDN -- 9300 DDN
                if (m_GTComponent == "GC_DDN")
                {
                    oComp = m_GTComponents.GetComponent(9301);
                    oComp.Recordset.MoveFirst();

                    sSql = "select substr(DDN_CODE,4,length(DDN_CODE)) from GC_DDN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and DDN_CODE <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("DDN_CODE", "S6M01");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 2) tCode = maxNum.ToString();

                            oComp.Recordset.Update("DDN_CODE", "S6M" + tCode);
                        }
                    }else oComp.Recordset.Update("DDN_CODE", "S6M01");
                }
                
                //GC_FAN -- 9700 FAN
                if (m_GTComponent == "GC_FAN")
                {
                    oComp = m_GTComponents.GetComponent(9701);
                    oComp.Recordset.MoveFirst();

                    sSql = "select substr(FAN_CODE,3,length(FAN_CODE)) from GC_FAN A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and FAN_CODE like 'FA%' and LENGTH(TRIM(FAN_CODE)) = 4";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("FAN_CODE", "FA01");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 2) tCode = maxNum.ToString();

                            oComp.Recordset.Update("FAN_CODE", "FA" + tCode);
                        }
                    }else oComp.Recordset.Update("FAN_CODE", "FA01");
                }

                //GC_FPATCHPANEL -- 12200 FTB(AC)
                if (m_GTComponent == "GC_FPATCHPANEL")
                {
                    oComp = m_GTComponents.GetComponent(12201);
                    oComp.Recordset.MoveFirst();

                    sSql = "select PATCH_CODE from GC_FPATCHPANEL A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and PATCH_CODE <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("PATCH_CODE", "0001");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "000" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 2) tCode = "00" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 3) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 4) tCode = maxNum.ToString();

                            oComp.Recordset.Update("PATCH_CODE", tCode);
                        }
                    }else oComp.Recordset.Update("PATCH_CODE", "0001");
                }

                //GC_FPATCH -- 4900 FTB(TX)
                if (m_GTComponent == "GC_FPATCH")
                {
                    oComp = m_GTComponents.GetComponent(4901);
                    oComp.Recordset.MoveFirst();

                    sSql = "select PATCH_CODE from GC_FPATCH A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and PATCH_CODE <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("PATCH_CODE", "0001");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "000" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 2) tCode = "00" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 3) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 4) tCode = maxNum.ToString();

                            oComp.Recordset.Update("PATCH_CODE", tCode);
                        }
                    }else oComp.Recordset.Update("PATCH_CODE", "0001");
                }

                //GC_FNODE -- 4800 Fibre Node
                if (m_GTComponent == "GC_FNODE")
                {
                    oComp = m_GTComponents.GetComponent(4801);
                    oComp.Recordset.MoveFirst();

                    sSql = "select NODE_CODE from GC_FNODE A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and NODE_CODE <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("NODE_CODE", "0001");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "000" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 2) tCode = "00" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 3) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 4) tCode = maxNum.ToString();

                            oComp.Recordset.Update("NODE_CODE", tCode);
                        }
                    }else oComp.Recordset.Update("NODE_CODE", "0001");
                }

                //------------------- CABLE ---------------------------
                //GC_FCBL 7200 Fibre E-Side Cable Code
                if (m_GTComponent == "GC_FCBL")
                {
                    oComp = m_GTComponents.GetComponent(53);
                    oComp.Recordset.MoveFirst();
                    
                    //Get IN and OUT Device
                    string InFNO = oComp.Recordset.Fields["IN_FNO"].Value.ToString();
                    string InFID = oComp.Recordset.Fields["IN_FID"].Value.ToString();
                    string OutFNO = oComp.Recordset.Fields["OUT_FNO"].Value.ToString();
                    string OutFID = oComp.Recordset.Fields["OUT_FID"].Value.ToString();
                    string CODE = "";
                    oComp = m_GTComponents.GetComponent(7201);
                    oComp.Recordset.MoveFirst();

                    if (oComp.Recordset.EditMode.ToString() == "adEditAdd") //To Support only Add Mode. 12-Sep-2012
                    {
                        //Vinod Change from Kelvin on 26th June 2012
                        //Vinod Change from Mike on 29th Aug 2012
                        if (InFNO == "5500" || InFNO == "12200")
                        {
                            sSql = "select substr(CABLE_CODE,2,length(CABLE_CODE)) from GC_FCBL A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and CABLE_CODE <> '***' and A.G3E_FID IN (SELECT G3E_FID FROM GC_NR_CONNECT WHERE G3E_FNO = 7200 AND IN_FNO IN(5500, 12200))";
                            rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                            if (rsPP.RecordCount > 0)
                            {
                                if (rsPP.Fields[0].Value.ToString() == "")
                                {
                                    CODE = "F01";
                                    oComp.Recordset.Update("CABLE_CODE", "F01");
                                }
                                else
                                {
                                    rsPP.MoveFirst();
                                    for (int i = 0; i < rsPP.RecordCount; i++)
                                    {
                                        int tempint = 0;
                                        if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                        {
                                            if (maxNum < tempint)
                                                maxNum = tempint;
                                        }
                                        rsPP.MoveNext();


                                    }
                                    maxNum = maxNum + 1;
                                    if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                                    else if (maxNum.ToString().Length >= 2) tCode = maxNum.ToString();

                                    oComp.Recordset.Update("CABLE_CODE", "F" + tCode);
                                    CODE = "F" + tCode;
                                }
                            }
                            else
                            {
                                oComp.Recordset.Update("CABLE_CODE", "F01");
                                CODE = "F01";
                            }
                        }
                        else if (InFNO == "11800" && OutFNO == "11800")
                        {
                            string CblCode = Get_Value("SELECT  CABLE_CODE FROM GC_FCBL WHERE G3E_FID IN (SELECT G3E_FID  FROM GC_NR_CONNECT WHERE OUT_FID =" + InFID + " and IN_FNO = 11800)");
                            if (CblCode != null)
                            {
                                oComp.Recordset.Update("CABLE_CODE", CblCode);
                                CODE = CblCode;
                            }
                            else
                            {
                                CblCode = Get_Value("SELECT substr(CABLE_CODE,2,length(CABLE_CODE)) FROM GC_FCBL WHERE G3E_FID IN (SELECT G3E_FID  FROM GC_NR_CONNECT WHERE OUT_FID =" + InFID + " and IN_FNO IN(5500, 12200))");
                                if (CblCode != null)
                                {
                                    maxNum = Convert.ToInt32(CblCode) + 1;
                                    if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                                    else if (maxNum.ToString().Length >= 2) tCode = maxNum.ToString();
                                    oComp.Recordset.Update("CABLE_CODE", "F" + CblCode + "/F" + tCode);
                                    CODE = "F" + CblCode;
                                }
                            }

                        }
                        else if (OutFNO == "5500" || OutFNO == "12200")
                        {
                            string CblCode = Get_Value("SELECT  CABLE_CODE FROM GC_FCBL WHERE G3E_FID IN (SELECT G3E_FID  FROM GC_NR_CONNECT WHERE OUT_FID =" + InFID + ")");
                            if (CblCode != null)
                            {
                                string[] arFea = new string[2];
                                char[] splitter = { '/' };
                                arFea = CblCode.Split(splitter);
                                if (arFea.Length == 2)
                                {
                                    oComp.Recordset.Update("CABLE_CODE", arFea[1].ToString());
                                    CODE = arFea[1].ToString();
                                }
                                else
                                {
                                    oComp.Recordset.Update("CABLE_CODE", CblCode);
                                    CODE = CblCode;
                                }
                            }
                        }
                    }

                    //GC_FCBL.SECT_NUM
                    maxNum = 0;
                    sSql = "select SECT_NUM from GC_FCBL A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and A.CABLE_CODE = '"+CODE+"'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("SECT_NUM", "1");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            oComp.Recordset.Update("SECT_NUM", maxNum);
                        }
                    }else oComp.Recordset.Update("SECT_NUM", "1");
                }

                //GC_FDCBL 7400 Fibre D-Side Cable
                if (m_GTComponent == "GC_FDCBL")
                {
                    oComp = m_GTComponents.GetComponent(53);
                    oComp.Recordset.MoveFirst();
                    
                    //Get IN and OUT Device
                    string InFNO = oComp.Recordset.Fields["IN_FNO"].Value.ToString();
                    string InFID = oComp.Recordset.Fields["IN_FID"].Value.ToString();
                    string OutFNO = oComp.Recordset.Fields["OUT_FNO"].Value.ToString();
                    string OutFID = oComp.Recordset.Fields["OUT_FID"].Value.ToString();
                    string CODE = "";

                    oComp = m_GTComponents.GetComponent(7401);
                    oComp.Recordset.MoveFirst();

                    if (oComp.Recordset.EditMode.ToString() == "adEditAdd") //To Support only Add Mode. 12-Sep-2012
                    {
                        string EditMode = oComp.Recordset.EditMode.ToString();
                        string ExCblCode = oComp.Recordset.Fields["FCABLE_CODE"].Value.ToString();

                        //Vinod Change from Kelvin on 26th June 2012                    
                        if (InFNO == "5100")
                        {                            
                            sSql = "select substr(FCABLE_CODE,3,length(FCABLE_CODE)) from GC_FDCBL A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and FCABLE_CODE <> '***' and a.g3e_fid in (select g3e_fid from gc_nr_connect where in_fid = " + InFID + ")";
                            //sSql = "select max(substr(FCABLE_CODE,3,length(FCABLE_CODE))) from GC_FDCBL A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and FCABLE_CODE <> '***'";
                            rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                            if (rsPP.RecordCount > 0)
                            {
                                if (rsPP.Fields[0].Value.ToString() == "")
                                {
                                    oComp.Recordset.Update("FCABLE_CODE", "FD1");
                                    CODE = "FD1";
                                }
                                else
                                {
                                    rsPP.MoveFirst();
                                    for (int i = 0; i < rsPP.RecordCount; i++)
                                    {
                                        int tempint = 0;
                                        if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                        {
                                            if (maxNum < tempint)
                                                maxNum = tempint;
                                        }
                                        rsPP.MoveNext();


                                    }
                                    maxNum = maxNum + 1;
                                    oComp.Recordset.Update("FCABLE_CODE", "FD" + maxNum.ToString());
                                    CODE = "FD" + maxNum.ToString();
                                }
                            }
                            else
                            {
                                oComp.Recordset.Update("FCABLE_CODE", "FD1");
                                CODE = "FD1";
                            }
                        }
                        else if (InFNO == "11800")
                        {
                            string CblCode = Get_Value("SELECT  FCABLE_CODE FROM GC_FDCBL WHERE G3E_FID IN (SELECT G3E_FID  FROM GC_NR_CONNECT WHERE OUT_FID =" + InFID + ")");
                            oComp.Recordset.Update("FCABLE_CODE", CblCode);
                            CODE = CblCode;
                        }
                    }

                    //GC_FDCBL.SECT_NUM
                    maxNum = 0;
                    sSql = "SELECT  SECT_NUM FROM GC_FDCBL WHERE G3E_FID IN (SELECT G3E_FID  FROM GC_NR_CONNECT WHERE OUT_FID =" + InFID + ")";
                       // "select SECT_NUM from GC_FDCBL A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and A.FCABLE_CODE = '" + CODE + "'"
                       //  +" and a.g3e_fid in (select g3e_fid from gc_nr_connect where in_fid = " + InFID + ")";;
                         
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("SECT_NUM", "1");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            oComp.Recordset.Update("SECT_NUM", maxNum);
                        }
                    }
                    else oComp.Recordset.Update("SECT_NUM", "1");
                }

                //GC_FCBL_JNT   Fibre Junction Cable
                if (m_GTComponent == "GC_FCBL_JNT")
                {
                    oComp = m_GTComponents.GetComponent(4501);
                    oComp.Recordset.MoveFirst();

                    sSql = "select substr(CABLE_CODE,2,length(CABLE_CODE)) from GC_FCBL_JNT A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and CABLE_CODE <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("CABLE_CODE", "J0001");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "000" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 2) tCode = "00" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 3) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 4) tCode = maxNum.ToString();

                            oComp.Recordset.Update("CABLE_CODE", "J" + tCode);
                        }
                    }
                    else oComp.Recordset.Update("CABLE_CODE", "J0001");
                }

                //GC_FCBL_TRUNK Fibre Trunk Cable
                if (m_GTComponent == "GC_FCBL_TRUNK")
                {
                    oComp = m_GTComponents.GetComponent(4401);
                    oComp.Recordset.MoveFirst();

                    sSql = "select substr(CABLE_CODE,2,length(CABLE_CODE)) from GC_FCBL_TRUNK A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and A.CABLE_CODE <> '***'";
                    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        if (rsPP.Fields[0].Value.ToString() == "")
                        {
                            oComp.Recordset.Update("CABLE_CODE", "T0001");
                        }
                        else
                        {
                            rsPP.MoveFirst();
                            for (int i = 0; i < rsPP.RecordCount; i++)
                            {
                                int tempint = 0;
                                if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                                {
                                    if (maxNum < tempint)
                                        maxNum = tempint;
                                }
                                rsPP.MoveNext();


                            }
                            maxNum = maxNum + 1;
                            if (maxNum.ToString().Length == 1) tCode = "000" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 2) tCode = "00" + maxNum.ToString();
                            else if (maxNum.ToString().Length == 3) tCode = "0" + maxNum.ToString();
                            else if (maxNum.ToString().Length >= 4) tCode = maxNum.ToString();

                            oComp.Recordset.Update("CABLE_CODE", "T" + tCode);
                        }
                    }else oComp.Recordset.Update("CABLE_CODE", "T0001");
                }
                                
            }
            catch (Exception ex)
            {               
                
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


    }
}
