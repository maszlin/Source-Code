using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

//Version 1.0

namespace NEPS.GTechnology.MergeNetwork
{

    public class WinWrapper : System.Windows.Forms.IWin32Window
    {        

        #region IWin32Window Members

        IntPtr IWin32Window.Handle
        {
            get 
            {
                System.IntPtr iptr = new System.IntPtr();
                iptr = (System.IntPtr)GTClassFactory.Create<IGTApplication>().hWnd;
                //iptr = CType(GTClassFactory.Create<IGTApplication>().hWnd, System.IntPtr);
                return iptr;
            }
        }

        #endregion
    }
       

    class GTCustomCommandModeless : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper = null;
        
        public static WinWrapper objWinWrapper = new WinWrapper();

        //public GisLib objGisLib = null;
        public IGTPoint oPoint = null;
        public int MergeValue = 0;

        private string In_FID = null;
        private string Out_FID = null;
        private int Splice_FID = 0;

        private IGTDataContext m_GTDataContext = null;
        public IGTMergeFeaturesService m_oMergeFeatureService;
               
        #region Event Handlers

        void m_oGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            try
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Clicked!: " + e.MapWindow.Caption + ". Double Click to exit custom modeless command.");
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }
        void m_oGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {            
                  
        }

        void m_oGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            if (MergeValue ==1)
            {
                MergeValue = 2;
                //m_oGTCustomCommandHelper_DblClick(sender, e);
            }
            else if (MergeValue == 2)
            {
                MergeValue = 3;                
                m_oMergeFeatureService_Finished();
            }

            //if (m_CustomForm != null)
            //{
            //    if (!IsAccessible)
            //    {
            //        Close();
            //        Dispose();
            //    }
            //    m_CustomForm = null;
            //}
            //else
            //    ExitCmd();
        }
        void m_oGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }

        void m_oGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
                  
        }
        
        void m_oGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
            
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");            
            // To Get Points            
            oPoint = e.WorldPoint;
            XPointGeom = oPoint.X.ToString() + "," + oPoint.Y.ToString();              

        }

        void m_oGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }

        void m_oGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Press ESC to cancel.");
            
            if (e.KeyCode == 27)
            {
                m_oMergeFeatureService.CancelMerge();
                ExitCmd();
            }
            
        }

        void m_oGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }

        void m_oGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }

        #endregion
        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running .NET custom modeless command...");
            
            m_oGTCustomCommandHelper = CustomCommandHelper;

            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount > 0)
            {
                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    if (oDDCKeyObject.FNO != 11800)
                    {
                        MessageBox.Show("Please Select a Splcie to Merge Feature", "Merge Network", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ExitCmd();
                        return;
                    }
                    else
                    {
                        In_FID = Get_Value("SELECT G3E_FID FROM GC_NR_CONNECT WHERE OUT_FID = " + oDDCKeyObject.FID);
                        Out_FID = Get_Value("SELECT G3E_FID FROM GC_NR_CONNECT WHERE IN_FID = " + oDDCKeyObject.FID);
                        Splice_FID = oDDCKeyObject.FID;
                        
                        if (In_FID == "" || Out_FID == "")
                        {
                            MessageBox.Show("Only Allowed to Merge Cable if the In and Out of the Splice is a Cable.", "Merge Network", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ExitCmd();
                            return;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Select a Splcie to Merge Feature", "Merge Network", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ExitCmd();
                return;
            }

            SubscribeEvents();
            //m_CustomForm = new GTWindowsForm_InsertSplice();
            //FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
            //this.bBeginSelect = true;
            //Show(objWinWrapper);           
                  

            log = Logger.getInstance();

            m_oMergeFeatureService = GTClassFactory.Create<IGTMergeFeaturesService>(m_oGTCustomCommandHelper);

            MergeNetwork();
        }
                
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitCmd();
        }

        public bool CanTerminate
        {
            get
            {
                return true;
            }
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }

        public void Terminate()
        {
            try
            {

                //if (m_oGTTransactionManager != null)
                //{
                //    m_oGTTransactionManager = null;
                //}

                if (m_oGTTransactionManager != null)
                {
                    if (m_oGTTransactionManager.TransactionInProgress)
                    {
                        m_oGTTransactionManager.Rollback();
                    }
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oGTTransactionManager = value;
            }
        }

        #endregion
        public void SubscribeEvents()
        {
            // Subscribe to m_oIGTCustomCommandHelper events using C# 1.0 syntax
            m_oGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oGTCustomCommandHelper_Activate);
            m_oGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oGTCustomCommandHelper_Deactivate);
            m_oGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oGTCustomCommandHelper_GainedFocus);
            m_oGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oGTCustomCommandHelper_LostFocus);
            m_oGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oGTCustomCommandHelper_KeyUp);
            m_oGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oGTCustomCommandHelper_KeyDown);
            m_oGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oGTCustomCommandHelper_KeyPress);
            m_oGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_Click);
            m_oGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_DblClick);
            m_oGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_MouseMove);
            m_oGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_MouseDown);
            m_oGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_MouseUp);
            m_oGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oGTCustomCommandHelper_WheelRotate);
            //m_oGTCustomCommandHelper.DblClick += new EventHandler(m_oMergeFeatureService_Finished);
        }

        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIGTCustomCommandHelper events using C# 1.0 syntax
            m_oGTCustomCommandHelper.Activate -= m_oGTCustomCommandHelper_Activate;
            m_oGTCustomCommandHelper.Deactivate -= m_oGTCustomCommandHelper_Deactivate;
            m_oGTCustomCommandHelper.GainedFocus -= m_oGTCustomCommandHelper_GainedFocus;
            m_oGTCustomCommandHelper.LostFocus -= m_oGTCustomCommandHelper_LostFocus;
            m_oGTCustomCommandHelper.KeyUp -= m_oGTCustomCommandHelper_KeyUp;
            m_oGTCustomCommandHelper.KeyDown -= m_oGTCustomCommandHelper_KeyDown;
            m_oGTCustomCommandHelper.KeyPress -= m_oGTCustomCommandHelper_KeyPress;
            m_oGTCustomCommandHelper.Click -= m_oGTCustomCommandHelper_Click;
            m_oGTCustomCommandHelper.DblClick -= m_oGTCustomCommandHelper_DblClick;
            m_oGTCustomCommandHelper.MouseMove -= m_oGTCustomCommandHelper_MouseMove;
            m_oGTCustomCommandHelper.MouseDown -= m_oGTCustomCommandHelper_MouseDown;
            m_oGTCustomCommandHelper.MouseUp -= m_oGTCustomCommandHelper_MouseUp;
            m_oGTCustomCommandHelper.WheelRotate -= m_oGTCustomCommandHelper_WheelRotate;
        }

        public void ExitCmd()
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exiting...");
            UnsubscribeEvents();
            m_oGTCustomCommandHelper.Complete();
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exited.");

        }
        
   
        //*************************
        //         FORM CODE
        //*************************
       
        public static IGTApplication m_GeoApp;        
        public Logger log;

        public bool flag = false;
        public string vPARENT = null;
        public string vCOMP = null;
        public string vFNO = null;

        public bool PlaceFlag = false;
        public int PlaceValue = 0;
        public bool CopyFlag = false;

        public int PlacedFID = 0;

        string ParentXY = "";
        string[] ParantBND;
        int ParentFID = 0;

        private string _XPoint;
        public string XPointGeom
        {
            get
            {
                return _XPoint;
            }
            set
            {
                                
            }
        }

        private string _YPoint;
        public string YPointGeom
        {
            get
            {
                return _YPoint;
            }
            set
            {
               // listBox2.Items.Add(value);              
            }
        }

       
        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null; 
        //IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();

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
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Merge Network", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
               
        public void MergeNetwork()
        {
            try
            {               
                IGTKeyObject m_oFeature1;
                IGTKeyObject m_oFeature2;
                IGTKeyObject m_Splice;

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("DeleteSplice");

                m_Splice = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(11800, Splice_FID);

                if (m_Splice.Components.GetComponent(11801).Recordset.RecordCount > 0)
                    m_Splice.Components.GetComponent(11801).Recordset.Delete(AffectEnum.adAffectCurrent);

                if (m_Splice.Components.GetComponent(11820).Recordset.RecordCount > 0)
                    m_Splice.Components.GetComponent(11820).Recordset.Delete(AffectEnum.adAffectCurrent);

                if (m_Splice.Components.GetComponent(11830).Recordset.RecordCount > 0)
                    m_Splice.Components.GetComponent(11830).Recordset.Delete(AffectEnum.adAffectCurrent);

                if (m_Splice.Components.GetComponent(11832).Recordset.RecordCount > 0)
                    m_Splice.Components.GetComponent(11832).Recordset.Delete(AffectEnum.adAffectCurrent);

                if (m_Splice.Components.GetComponent(50).Recordset.RecordCount > 0)
                    m_Splice.Components.GetComponent(50).Recordset.Delete(AffectEnum.adAffectCurrent);

                if (m_Splice.Components.GetComponent(54).Recordset.RecordCount > 0)
                    m_Splice.Components.GetComponent(54).Recordset.Delete(AffectEnum.adAffectCurrent);

                if (m_Splice.Components.GetComponent(77).Recordset.RecordCount > 0)
                    m_Splice.Components.GetComponent(77).Recordset.Delete(AffectEnum.adAffectCurrent);

                if (m_Splice.Components.GetComponent(51).Recordset.RecordCount > 0)
                    m_Splice.Components.GetComponent(51).Recordset.Delete(AffectEnum.adAffectCurrent);

                GTCustomCommandModeless.m_oGTTransactionManager.Commit();

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("MergeNetwork");

                m_oFeature1 = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7200, Convert.ToInt32(In_FID));
                m_oFeature2 = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7200, Convert.ToInt32(Out_FID));

                MergeValue = 1;                

                m_oMergeFeatureService.StartMerge(m_oFeature1, m_oFeature2);                

                //m_oMergeFeatureService.Dispose();

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Merge Network", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

        public void GTMergeFeaturesServiceEventArgs( bool DisplayErrors)
        {
            GTMergeFeaturesServiceEventArgs evnt;
            evnt = GTClassFactory.Create<GTMergeFeaturesServiceEventArgs>();            

        }

        private void m_oMergeFeatureService_Aborted()
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Aborted.");
            GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
            m_oGTCustomCommandHelper.Complete();
        }

        private void m_oMergeFeatureService_Disallowed(bool DisplayErrors)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Disallowed.");

            m_oMergeFeatureService.ErrorDialogCaption = "Merge Feature";
            m_oMergeFeatureService.ErrorDialogMessage = "The merge was not allowed for the following reasons:";
            DisplayErrors = true;
            //DisplayErrors = false;
            //m_oMergeFeatureService.DisallowedErrorArray;
        }

        private void m_oMergeFeatureService_Disapproved(bool DisplayErrors)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Disapproved.");
            m_oMergeFeatureService.ErrorDialogCaption = "Merge Feature";
            m_oMergeFeatureService.ErrorDialogMessage = "The merge was not approved for the following reasons:";
            DisplayErrors = true;
        }

        private void m_oMergeFeatureService_Finished()
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Finished.");
            GTCustomCommandModeless.m_oGTTransactionManager.Commit();
            m_oMergeFeatureService.Dispose();
            ExitCmd();
        }


        
    }

}
