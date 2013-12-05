using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.NEPS6InchDuctPlacement
{
    class GT6InchDuctPlc : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        public static Intergraph.GTechnology.API.IGTDataContext m_IGTDataContext = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public  Intergraph.GTechnology.API.IGTRelationshipService mobjRelationshipService = null;
        public  Intergraph.GTechnology.API.IGTLocateService mobjLocateService = null;
        public  Intergraph.GTechnology.API.IGTFeatureExplorerService mobjExplorerService = null;
        public  Intergraph.GTechnology.API.IGTFeaturePlacementService mobjPlacementService = null;

        private int startplc = 0;
        private  IGTKeyObject SourceFeature = null;
        private  IGTKeyObject _6inchDuct = null;
        private short FNO_Selected = 0;
        private short CNO_Selected = 0;
        private int FID_Selected = 0;
        private int Wall_Selected = 0;
        private bool Continue = false;
        

        #endregion

        #region Event Handlers
        #region Muse Move
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            
            try
            {
                IGTPoint WorldPoint = e.WorldPoint;
                if (startplc == 1)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select a Manhole. Right click to exit.");
                }
                else if (startplc == 2)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select Wall Number. Right click to exit.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }

        }
        #endregion

        #region Mouse UP (click)
        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            try
            {
                IGTPoint WorldPoint = e.WorldPoint;

                if (e.Button != 2)//left button
                {
                    #region selecting source MH
                    if (startplc == 1) //select source Manhole feature to start copying
                    {
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                        if (CheckSeletedFeature())
                        {
                            if (Wall_Selected != -1 && Wall_Selected != 0)
                            {
                                startplc = 0;
                              //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, Placement in process...");
                                _6InchDuctPlacement();
                            }
                            else
                            {
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select Wall Number. Right click to exit.");
                                MessageBox.Show("Please select Wall Number", "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                startplc = 2;
                            }
                        }
                    }
                    else if (startplc == 2)
                    {
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select Wall Number. Right click to exit.");
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
                        Wall_Selected = GetWallNumber(true);
                        if (Wall_Selected != -1 && Wall_Selected != 0)
                        {
                            startplc = 0;
                            //   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, Placement in process...");
                            _6InchDuctPlacement();
                            return;
                        }
                    }

                    #endregion

                }
                else if (e.Button == 2)//right button
                {
                    if ((startplc == 1) || (startplc == 2))
                    {
                        DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "6 Inch Duct Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (retVal == DialogResult.Yes)
                        {
                            ExitCmd();
                        }
                    }
               
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }
        #endregion

        #region Do not use
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            //
        }

        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            //
        }
        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }


        void m_oIGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
            //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");            
        }

        void m_oIGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
            //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }

        void m_oIGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyUp.");            
        }

        void m_oIGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
            //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }

        void m_oIGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
            //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oIGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
            //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
            // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }
        #endregion
        #endregion

        #region Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            m_gtapp = GTClassFactory.Create<IGTApplication>();
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running 6 Inch Duct Placement . . . ");
            m_oIGTCustomCommandHelper = CustomCommandHelper;
            mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>();
             m_IGTDataContext = m_gtapp.DataContext;
            mobjRelationshipService.DataContext = m_IGTDataContext;
            mobjLocateService = m_gtapp.ActiveMapWindow.LocateService;

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
            }
            SubscribeEvents();
            if (CheckSeletedFeature())
            {
                if (Wall_Selected != -1 && Wall_Selected != 0)
                {
                    startplc = 0;
                    //m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, Placement in process...");
                    _6InchDuctPlacement();
                }
                else
                {
                    startplc = 2;
                }
            }
            else
            {
                startplc = 1;
            }
            
          //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Source Manhole Wall Number! Right Click  to exit.");



        }


        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "6 Inch Duct Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                }
                else
                {
                    return false;
                }

                return false;
            }
        }

        public void Pause()
        {
            startplc += 50000;
        }

        public void Resume()
        {
            startplc -= 50000;
        }

        public void Terminate()
        {
            try
            {

                if (m_oIGTTransactionManager != null)
                {
                    m_oIGTTransactionManager = null;
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
                m_oIGTTransactionManager = value;
            }
        }

        #endregion

        #region subscribe events for windows form
        public void SubscribeEvents()
        {
            // Subscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            //m_oIGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oIGTCustomCommandHelper_Activate);
            //m_oIGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oIGTCustomCommandHelper_Deactivate);
            //m_oIGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oIGTCustomCommandHelper_GainedFocus);
            //m_oIGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oIGTCustomCommandHelper_LostFocus);
            //m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
            //m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
            //m_oIGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oIGTCustomCommandHelper_KeyPress);
            //m_oIGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_Click);
            //m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);
            m_oIGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseMove);
            //m_oIGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseDown);
            m_oIGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseUp);
            //m_oIGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oIGTCustomCommandHelper_WheelRotate);
        }
        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            //m_oIGTCustomCommandHelper.Activate -= m_oIGTCustomCommandHelper_Activate;
            //m_oIGTCustomCommandHelper.Deactivate -= m_oIGTCustomCommandHelper_Deactivate;
            //m_oIGTCustomCommandHelper.GainedFocus -= m_oIGTCustomCommandHelper_GainedFocus;
            //m_oIGTCustomCommandHelper.LostFocus -= m_oIGTCustomCommandHelper_LostFocus;
            //m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
            //m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
            //m_oIGTCustomCommandHelper.KeyPress -= m_oIGTCustomCommandHelper_KeyPress;
            //m_oIGTCustomCommandHelper.Click -= m_oIGTCustomCommandHelper_Click;
            //m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
            //m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
            //m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }
        #endregion

        #region close application
        public void ExitCmd()
        {
            if (m_oIGTTransactionManager != null)
            {
                if (m_oIGTTransactionManager.TransactionInProgress)
                    m_oIGTTransactionManager.Rollback();
                m_oIGTTransactionManager = null;
            }
            _6inchDuct = null;
            SourceFeature = null;

            Continue = false;
            m_gtapp.SetProgressBarRange(0, 0);
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
            m_gtapp.SelectedObjects.Clear();

            if (mobjLocateService != null)
                mobjLocateService = null;
           
            if(mobjExplorerService!=null)
            {
                mobjExplorerService.Clear();
                mobjExplorerService.CancelClick -= mobjExplorerService_CancelClick;
                mobjExplorerService.SaveAndContinueClick -= mobjExplorerService_SaveAndContinueClick;
                mobjExplorerService.SaveClick -= mobjExplorerService_SaveClick;
                mobjExplorerService.Visible = false;
                mobjExplorerService = null;
            }
            if (mobjPlacementService != null)
            {
                mobjPlacementService.Dispose();
                mobjPlacementService.Finished -= placementService_Finished;
                mobjPlacementService = null;
            }
            UnsubscribeEvents();
            if (SourceFeature != null)
            {

                SourceFeature = null;
            }

            if (mobjRelationshipService != null)
            {
                mobjRelationshipService = null;
            }
            startplc = 0;
           
            m_oIGTCustomCommandHelper.Complete();

        }
        #endregion

        #region Check Selected Feture
        private bool CheckSeletedFeature()
        {
            if (m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                return false;
            }

            if (m_gtapp.SelectedObjects.FeatureCount > 1)
            {
                MessageBox.Show("Select only one feature at once!", "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                m_gtapp.SelectedObjects.Clear();
                return false;
            }
            foreach (IGTDDCKeyObject selectedObj in m_gtapp.SelectedObjects.GetObjects())
            {
                if (selectedObj.FNO == 2700 )
                {
                    FID_Selected = selectedObj.FID;
                    FNO_Selected = selectedObj.FNO;
                    if (FNO_Selected == 2700)
                    {
                        CNO_Selected = 2732;
                     }

                    IGTKeyObject SelectedFeature = m_gtapp.DataContext.OpenFeature(FNO_Selected, FID_Selected);

                    // get the wall number component
                    IGTComponent wallNumberComponent = SelectedFeature.Components.GetComponent(CNO_Selected);
                    if (wallNumberComponent.Recordset.EOF)
                    {
                        MessageBox.Show("Selected feature does not have Wall Numbers", "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    Wall_Selected = GetWallNumber(false);
                    if (Wall_Selected == -1 || Wall_Selected == 0)
                    {
                      
                                IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                                IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                                IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                                point1.X = SelectedFeature.Components.GetComponent(2720).Geometry.FirstPoint.X - 3;
                                point1.Y = SelectedFeature.Components.GetComponent(2720).Geometry.FirstPoint.Y - 3;
                                range.BottomLeft = point1;
                                point2.X = SelectedFeature.Components.GetComponent(2720).Geometry.FirstPoint.X + 3;
                                point2.Y = SelectedFeature.Components.GetComponent(2720).Geometry.FirstPoint.Y + 3;
                                range.TopRight = point2;
                                m_gtapp.ActiveMapWindow.ZoomArea(range);
                                m_gtapp.RefreshWindows();
                                m_gtapp.SelectedObjects.Clear();
                         
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("Select a Manhole only!", "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    m_gtapp.SelectedObjects.Clear();
                    return false;
                }

            }

            m_gtapp.SelectedObjects.Clear();
            return false;
        }
        #endregion

        #region GET WALL NUMBER
        private int GetWallNumber(bool flag)
        {
            string FeatureType = "";

            if (FNO_Selected == 2700)
                FeatureType = "Manhole";
            else if (FNO_Selected == 3800)
                FeatureType = "Chamber";
            else if (FNO_Selected == 3300)
                FeatureType = "Tunnel/Trench";

            #region check if selected allowed feature and if successshow detail
            if (m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                m_gtapp.SelectedObjects.Clear();
                return -1;
            }

            if (m_gtapp.SelectedObjects.FeatureCount > 1)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                m_gtapp.SelectedObjects.Clear();
                return -1;
            }


            string WallNum = "";
            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
            {
                if (FNO_Selected != oDDCKeyObject.FNO)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    m_gtapp.SelectedObjects.Clear();
                    return -1;
                }
                if (FID_Selected != oDDCKeyObject.FID)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    m_gtapp.SelectedObjects.Clear();
                    return -1;
                }

                //manhole
                if (FNO_Selected == 2700)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_MANHLW_T")
                    {
                        for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                        {
                            if (oDDCKeyObject.Recordset.Fields[i].Name == "WALL_NUM")
                            {
                                WallNum = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                            }
                        }
                       
                       break;
                    }
                }
                if (flag)
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                m_gtapp.SelectedObjects.Clear();
                return -1;

            }
            #endregion

            m_gtapp.SelectedObjects.Clear();
            return int.Parse(WallNum);
        }
        #endregion

        #region 6InchDuctPlacement
        private void _6InchDuctPlacement()
        {
            try
            {

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter 6 Inch Duct Attributes");
                if (mobjExplorerService == null)
                {
                    mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();
                    mobjExplorerService.CancelClick += new EventHandler(mobjExplorerService_CancelClick);
                    mobjExplorerService.SaveAndContinueClick += new EventHandler(mobjExplorerService_SaveAndContinueClick);
                    mobjExplorerService.SaveClick += new EventHandler(mobjExplorerService_SaveClick);
                }
                m_oIGTTransactionManager.Begin("Place6InchDuct");
                _6inchDuct = m_gtapp.DataContext.NewFeature(16000);
                _6inchDuct.Components.GetComponent(16001).Recordset.Update("MANHOLE_WALL_NUM", Wall_Selected);
                mobjExplorerService.ExploreFeature(_6inchDuct, "6 Inch Duct");
                mobjExplorerService.Visible = true;
                mobjExplorerService.Slide(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }
        #endregion

        #region FeatureExplorer
        private void CloseFeatureExplorer()
        {

            ExitCmd();
        }

        private void SaveAttribute()
        {
            try
            {

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                mobjExplorerService.Visible = false;
                if (mobjPlacementService == null)
                {
                    mobjPlacementService = GTClassFactory.Create<IGTFeaturePlacementService>(m_oIGTCustomCommandHelper);
                    mobjPlacementService.Finished += new EventHandler<GTFinishedEventArgs>(placementService_Finished);                  
                }
                mobjPlacementService.StartComponent(_6inchDuct, 16020);
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }

        }

        void placementService_Finished(object sender, GTFinishedEventArgs e)
        {
            try
            {

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, Placement in process... ");
                CreateRelationship(_6inchDuct);
                if (m_oIGTTransactionManager != null)
                {
                    if (m_oIGTTransactionManager.TransactionInProgress)
                    {
                        m_oIGTTransactionManager.Commit();
                        m_oIGTTransactionManager.RefreshDatabaseChanges();
                    }
                }
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Placement is completed successfully");
                if (Continue)
                {
                    Continue = false;
                    m_gtapp.SelectedObjects.Clear();
                    _6inchDuct = null;
                    SourceFeature = null;
                    
                    //if (mobjExplorerService != null)
                    //{
                    //    mobjExplorerService.Clear();
                    //    mobjExplorerService.CancelClick -= mobjExplorerService_CancelClick;
                    //    mobjExplorerService.SaveAndContinueClick -= mobjExplorerService_SaveAndContinueClick;
                    //    mobjExplorerService.SaveClick -= mobjExplorerService_SaveClick;
                    //    mobjExplorerService.Visible = false;
                    //    mobjExplorerService = null;
                    //}
                               
                    
                    startplc = 1;
                }
                else
                    ExitCmd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }
        private void CreateRelationship(IGTKeyObject relatedFeature)
        {
            try
            {
                SourceFeature = m_gtapp.DataContext.OpenFeature(FNO_Selected, FID_Selected);
                IGTRelationshipService relationshipService =
                    GTClassFactory.Create<IGTRelationshipService>(m_oIGTCustomCommandHelper);
                relationshipService.DataContext = m_gtapp.DataContext;
                relationshipService.ActiveFeature = SourceFeature;
                if (relationshipService.AllowSilentEstablish(relatedFeature))
                    relationshipService.SilentEstablish(1, relatedFeature);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }

       private void mobjExplorerService_CancelClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {

                ExitCmd();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }

       private void mobjExplorerService_SaveAndContinueClick(object sender, EventArgs e)
        {
            try
            {
                SaveAttribute();
                Continue = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }

       private void mobjExplorerService_SaveClick(object sender, EventArgs e)
        {
          try
            {
              SaveAttribute();
          }
          catch (Exception ex)
          {
              MessageBox.Show(ex.Message, "6 Inch Duct Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
              ExitCmd();
          }

        }

        #endregion

        
    }
}
