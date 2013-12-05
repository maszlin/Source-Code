using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.PlaceFDP
{

    public class WinWrapper : System.Windows.Forms.IWin32Window
    {        

        #region IWin32Window Members

        IntPtr IWin32Window.Handle
        {
            get 
            {
                System.IntPtr iptr = new System.IntPtr();
                iptr = (System.IntPtr)GTCustomCommandModeless.application.hWnd;
                //iptr = CType(application.hWnd, System.IntPtr);
                return iptr;
            }
        }

        #endregion
    }
       

    class GTCustomCommandModeless : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        public static Intergraph.GTechnology.API.IGTApplication application = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper = null;

        public static IGTGeometryEditService mobjEditServiceTemp;
        public static IGTGeometryEditService mobjEditServicePoint;
        public static IGTGeometryEditService mobjEditServiceText;
        IGTLocateService mobjLocateService = null;        
        IGTKeyObject mobjAttribute = null;
        bool mblnVisible = false;
        bool closestatus = false;
        public static IGTGeometry oPointPoint = null;
        public static IGTGeometry oTextPoint = null;

        public static WinWrapper objWinWrapper = new WinWrapper();

        //public GisLib objGisLib = null;
        public IGTPoint oPoint = null;
        public bool bBeginSelect;
       // private string sFNOFilter;

        GTWindowsForm_PlaceFDP m_CustomForm = null;
                
        #region Event Handlers

        void m_oGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            try
            {
              //  application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Clicked!: " + e.MapWindow.Caption + ". Double Click to exit custom modeless command.");
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }
        void m_oGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {            
            IGTPoint WorldPoint = e.WorldPoint;
           // m_CustomForm.PlaceValue = 1;
            if (m_CustomForm.PlaceValue == 1) 
            {
                if(m_CustomForm.PlaceFlag)
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage,"Point to place new "+m_CustomForm.FEATURE+"!Right click to cancel placement");// "Point to place the " + m_CustomForm.FEATURE + " Symbol and Text");
                else if(m_CustomForm.CopyFlag)
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to place copying feature! Right click to cancel copying");// "Point to place the " + m_CustomForm.FEATURE + " Symbol and Text");
                
                IGTPointGeometry oPoint = PGeoLib.CreatePointGeom(WorldPoint.X, WorldPoint.Y);
               if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.AddGeometry(oPoint,m_CustomForm.StyleId);// 5620001);
                oPointPoint = oPoint;
            //}
            //else if (m_CustomForm.PlaceFlag == true && m_CustomForm.PlaceValue == 2)
            //{
                //application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Place the FDP Text.");//"F\nFDP",
                IGTPointGeometry oPointT = PGeoLib.CreateTextGeom(WorldPoint.X, WorldPoint.Y, m_CustomForm.TextContent, 0, m_CustomForm.StyleTextAlignment);
                //if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.AddGeometry(oPointT, m_CustomForm.StyleTextId);// 5610001);
                oTextPoint = oPointT;
                return;
            }
            if (m_CustomForm.PlaceValue == 100)
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select " + m_CustomForm .vPARENT+ "!Right click to cancel selection");
                
            }
            if (m_CustomForm.PlaceValue == 200)
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select FDP for copying!Right click to cancel selection");

            }
           //ExitCmd();
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
            //application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }

        void m_oGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            #region left click
            if (e.Button == 1)
            {
                if (m_CustomForm.PlaceValue == 1)
                {
                    IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                    for (int K = 0; K < feat.Count; K++)
                        application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);


                    if (application.SelectedObjects.FeatureCount == 0)
                    {
                        DialogResult retVal = MessageBox.Show("No cabel selected!\nDo you want to place FDP without connection?", "Place FDP", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (retVal == DialogResult.No)
                            return;
                    }
                    int DsideCableFID = 0;
                    if (application.SelectedObjects.FeatureCount > 0)
                    {
                        foreach (IGTDDCKeyObject oDDCKeyObject in application.SelectedObjects.GetObjects())
                        {
                            if (oDDCKeyObject.FNO == 7400)
                            {
                                DsideCableFID = oDDCKeyObject.FID;
                                break;
                            }
                            else
                            {
                                DialogResult retVal = MessageBox.Show("Selected feature is not D-side fiber cabel!\nDo you want to place FDP without connection?", "Place FDP", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (retVal == DialogResult.No)
                                {
                                    application.SelectedObjects.Clear();
                                    return;
                                }
                                else break;
                            }
                        }
                    }
                    if (DsideCableFID != 0)
                    {
                        string output = m_CustomForm.Get_Value("select out_fid from GC_NR_CONNECT where g3e_fid=" + DsideCableFID.ToString());
                        if (output != "" && output != "0")
                        {
                            DialogResult retVal = MessageBox.Show("Selected Cabel already has connection!\nDo you want to reestablish relationship?", "Place FDP", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (retVal == DialogResult.No)
                            {
                                application.SelectedObjects.Clear();
                                return;
                            }
                        }
                    }
                    m_CustomForm.PlaceValue = 0;
                    if (mobjEditServicePoint.GeometryCount > 0) mobjEditServicePoint.RemoveAllGeometries();
                    mobjEditServicePoint.AddGeometry(oPointPoint, m_CustomForm.StyleId);                   
                    if (mobjEditServiceText.GeometryCount > 0) mobjEditServiceText.RemoveAllGeometries();
                    mobjEditServiceText.AddGeometry(oTextPoint, m_CustomForm.StyleTextId);
                     application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, database is updating...");

                    if (m_CustomForm.PlaceFlag == true)
                    {
                        application.BeginWaitCursor();
                        m_CustomForm.DrawFDP(m_CustomForm.FNO, DsideCableFID, 7400,false);
                        application.EndWaitCursor();
                        m_CustomForm.Show();
                        m_CustomForm.PlaceValue = 0;
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                    }
                    else if (m_CustomForm.CopyFlag == true)
                    {
                        application.BeginWaitCursor();
                        m_CustomForm.DrawFDP(m_CustomForm.FNO, DsideCableFID, 7400,true);
                        application.EndWaitCursor();
                        m_CustomForm.PlaceValue = 1;
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                    }
                    return;
                }

                #region select parent device
                if (m_CustomForm.PlaceValue == 100)
                {
                    IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                    for (int K = 0; K < feat.Count; K++)
                        application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                    if (application.SelectedObjects.FeatureCount == 1)
                    {
                        m_CustomForm.PickFDC();
                    }
                    return;
                }
                #endregion
                #region select fdp for copy
                if (m_CustomForm.PlaceValue == 200)
                {
                    IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                    for (int K = 0; K < feat.Count; K++)
                        application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                    if (application.SelectedObjects.FeatureCount == 1)
                    {
                        m_CustomForm.PickCopyFDP();
                    }
                    return;
                }
                #endregion
            }
            #endregion
 
            #region right click
            else 
            {
                if (m_CustomForm.PlaceValue == 100 || m_CustomForm.PlaceValue == 1 || m_CustomForm.PlaceValue == 200)
                {
                    application.SelectedObjects.Clear();
                    if (mobjEditServicePoint.GeometryCount > 0)
                        mobjEditServicePoint.RemoveAllGeometries();
                    if (mobjEditServiceText.GeometryCount > 0)
                        mobjEditServiceText.RemoveAllGeometries();
                    if (mobjEditServiceTemp.GeometryCount > 0)
                        mobjEditServiceTemp.RemoveAllGeometries();
                    m_CustomForm.Show();
                    m_CustomForm.PlaceValue = 0;
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                    return;
                }                
            }
            #endregion
        }

        void m_oGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
            
            //application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");
            
            // To Get Points
            
           

        }

        void m_oGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
            //application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
            
        }

        void m_oGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
        //   
            
            
        }

        void m_oGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
            //application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }

        void m_oGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
           // application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
           // application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
          //  application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
          //  application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }

        #endregion
        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                if (application == null) application = GTClassFactory.Create<IGTApplication>();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Placement FDP...");
                
                m_oGTCustomCommandHelper = CustomCommandHelper;
                mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServicePoint = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServicePoint.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServiceText = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceText.TargetMapWindow = application.ActiveMapWindow;

                mobjLocateService = application.ActiveMapWindow.LocateService;
                SubscribeEvents();
                m_CustomForm = new GTWindowsForm_PlaceFDP();
                m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
                this.bBeginSelect = true;
                m_CustomForm.Show(objWinWrapper);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error); ExitCmd(); }
            

        }
                
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(!closestatus)
                ExitCmd();
        }

        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Place FDP", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                    //  return true;
                }
               
                return false;

            }
        }

        public void Pause()
        {
            if(m_CustomForm.PlaceValue!=0)
                m_CustomForm.PlaceValue += 50000;
        }

        public void Resume()
        {
            if (m_CustomForm.PlaceValue != 0)
                m_CustomForm.PlaceValue -= 50000;
        }

        public void Terminate()
        {
            try
            {

               
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
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Exiting...");
           
            if (m_oGTTransactionManager != null)
            {
                if (m_oGTTransactionManager.TransactionInProgress)
                    m_oGTTransactionManager.Rollback();
                m_oGTTransactionManager = null;
            }
            if (application != null)
            {
                application = null;
            }
            if (mobjEditServiceTemp != null)
            {
                if (mobjEditServiceTemp.GeometryCount > 0)
                    mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp = null;
            }
            if (mobjEditServicePoint != null)
            {
                if (mobjEditServicePoint.GeometryCount > 0)
                    mobjEditServicePoint.RemoveAllGeometries();
                mobjEditServicePoint = null;
            }
            if (mobjEditServiceText != null)
            {
                if (mobjEditServiceText.GeometryCount > 0)
                    mobjEditServiceText.RemoveAllGeometries();
                mobjEditServiceText = null;
            }
            if (oPointPoint != null)
            {
                oPointPoint = null;
            }
            if (oTextPoint != null)
            {
                oTextPoint = null;
            }
            if (objWinWrapper != null)
            {
                objWinWrapper = null;
            }
            if (m_CustomForm != null)
            {
                closestatus = true;
                m_CustomForm.Close();
                m_CustomForm = null;
            }
            mobjLocateService = null;
            UnsubscribeEvents();

          
            m_oGTCustomCommandHelper.Complete();

        }

   

    }
}
