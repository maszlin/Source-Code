using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;


namespace NEPS.GTechnology.ZoomByFID
{
    class GTZoomByFID : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper = null;


        public bool bBeginSelect;
      //  private short Sel_iFNO = 0;
       // private int Sel_iFID = 0;

       
        #region Event Handlers
        void m_oGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            try
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please select an Address Point.");
                //MessageBox.Show("Hai");
               // Get_Addr();
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }
        void m_oGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please select an Address Point.");
            
        }

        void m_oGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            //if (m_CustomForm != null)
            //{
            //    if (!m_CustomForm.IsAccessible)
            //    {
            //        m_CustomForm.Close();
            //        m_CustomForm.Dispose();
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
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseUp.");
        }

        void m_oGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");
            
        }

        void m_oGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }

        void m_oGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyUp.");
            
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
            //Close Current Map Window
            GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.Close();

            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Loading Find & Zoom By FID");
            string FID = GTClassFactory.Create<IGTApplication>().Properties[0].ToString();

            m_oGTCustomCommandHelper = CustomCommandHelper;
            this.SubscribeEvents();
            this.bBeginSelect = true;
                      
            //Load the New Map Window with Load Legend
            GTClassFactory.Create<IGTApplication>().NewMapWindow("Geobase Engineering");
             
            //Passing FID to Find and ZOOM
            ZoomFID(Convert.ToInt32(FID.ToString()));

            this.UnsubscribeEvents();
            this.Terminate();
            
        }

        
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
              
                MessageBox.Show(ex.Message, "ZoomFID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        //Zoom FID Chidhu Dated 26-11-2011
        private void ZoomFID(int fid)
        {
            short mFNO;
            int mFID = fid;
            string fno = Get_Value("select g3e_fno from gc_netelem where g3e_fid=" + mFID);
            mFNO = Convert.ToInt16(fno.ToString());

            IGTDDCKeyObjects oGTKeyObjs;            

            GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();
            oGTKeyObjs = (IGTDDCKeyObjects)GTClassFactory.Create<IGTApplication>().DataContext.GetDDCKeyObjects(mFNO, mFID, GTComponentGeometryConstants.gtddcgAllGeographic);
            
            for (int K = 0; K < oGTKeyObjs.Count; K++)
                GTClassFactory.Create<IGTApplication>().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[K]);


            GTClassFactory.Create<IGTApplication>().ActiveMapWindow.CenterSelectedObjects();
            GTClassFactory.Create<IGTApplication>().ActiveMapWindow.DisplayScale = 500;

            GTClassFactory.Create<IGTApplication>().RefreshWindows();
            oGTKeyObjs = null;
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

                if (m_oGTTransactionManager != null)
                {
                    m_oGTTransactionManager = null;
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
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Updating...");
            UnsubscribeEvents();
            m_oGTCustomCommandHelper.Complete();
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Address Updated...");

        }

    }
}
