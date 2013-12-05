using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.NEPSDuctPathExpansion
{
    class GTDuctPathExpansion : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables member
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        public static Intergraph.GTechnology.API.IGTDataContext m_IGTDataContext = null;
        DPExpansionForm DPExpanForm = null;
        IGTLocateService mobjLocateService = null;

        #region define Duct Path class
        
        public class DuctPath
        {
            public int FID;
            public short FNO;
            public string Feature_state;
            public int sourceFID;
            public short sourceFNO;
            public int sourceWall;
            public string sourceType;
            public int termFID;
            public short termFNO;
            public int termWall;
            public string termType;
            public string EXC_ABB;
            public int Length;
            public int DuctWay;
            public string ConstructBy;
            public string InstallYear;
            public string BillingRate;
            public string DBFlag;
            public int PPDuctWay;
        };
        #endregion
        public static int step = 0;
        public static DuctPath DuctPathOrigin = null;
      
        #endregion      

        #region Event Handlers
       
          #region Mouse Move
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
           try
           {
                #region Get Selected Duct Path for editing
                if (step == 1)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");

                    int FID = 0;
                    short FNO = 0;
                    if (m_gtapp.SelectedObjects.FeatureCount == 1)
                    {
                        foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                        {
                            if (oDDCKeyObject.FNO != 2200)
                            {
                                MessageBox.Show("Please select a Duct Path!", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                m_gtapp.ActiveMapWindow.Activate();
                                m_gtapp.SelectedObjects.Clear();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                return;
                            }
                            if (GetDuctPathOrigin(oDDCKeyObject.FNO, oDDCKeyObject.FID))
                            {
                                if (DuctPathOrigin.Feature_state != "ASB" && DuctPathOrigin.Feature_state != "MOD")
                                {
                                    MessageBox.Show("Expansion function is only avaliable for ASB/MOD Duct Path", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.SelectedObjects.Clear();
                                }
                                else
                                {
                                    step = 0;
                                    if (DPExpanForm == null)
                                    {
                                        DPExpanForm = new DPExpansionForm();
                                        DPExpanForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
                                    }
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                                    DPExpanForm.Show();
                                }
                            }
                            else
                            {
                                if (DuctPathOrigin != null)
                                    DuctPathOrigin = null;
                                MessageBox.Show("Please select one more time!", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                m_gtapp.ActiveMapWindow.Activate();
                                m_gtapp.SelectedObjects.Clear();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                return;
                            }


                        }
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");

                    }
                    else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                    {
                        MessageBox.Show("Please select only one Duct Path at once!", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_gtapp.SelectedObjects.Clear();
                        m_gtapp.ActiveMapWindow.Activate();
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                        return;
                    }
                    return;
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }
        #endregion       
     
  #region Mouse Up
        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            try
            {
                   IGTPoint WorldPoint = e.WorldPoint;

                if (e.Button != 2)//left button
                {  
                   #region Get Selected Duct Path for editing
                    if (step == 1) 
                    {
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                       
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
                        int FID=0;
                        short FNO=0;
                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {
                            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                            {
                                if (oDDCKeyObject.FNO != 2200)
                                {
                                    MessageBox.Show("Please select a Duct Path!", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                    return;
                                }
                                if (GetDuctPathOrigin(oDDCKeyObject.FNO, oDDCKeyObject.FID))
                                {
                                    step = 0;
                                    if (DPExpanForm == null)
                                    {
                                        DPExpanForm = new DPExpansionForm();
                                        DPExpanForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
                                    }
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                                    DPExpanForm.Show();
                                }
                                else
                                {
                                    if (DuctPathOrigin != null)
                                        DuctPathOrigin = null;
                                    MessageBox.Show("Please select one more time!", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                    return;
                                }
                                

                            }
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");

                        }
                        else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                        {
                            MessageBox.Show("Please select only one Duct Path at once!", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            m_gtapp.SelectedObjects.Clear();
                            m_gtapp.ActiveMapWindow.Activate();
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                            return;
                        }
                        return;
                    }
                    #endregion
                }
                else  if (e.Button == 2)//right click
                    {
                        if (step == 1 )//exiting from application
                        {
                            DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Duct Path Expansion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (retVal == DialogResult.Yes)
                            {
                                ExitCmd();
                                return;
                            }                            
                            m_gtapp.ActiveMapWindow.Activate();
                        }

                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }          
        }

       #endregion

        #region Unused
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
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Duct Path Expansion . . . ");
            m_oIGTCustomCommandHelper = CustomCommandHelper;
            m_IGTDataContext = m_gtapp.DataContext;
            mobjLocateService = m_gtapp.ActiveMapWindow.LocateService;

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
            }

            SubscribeEvents();

            step = 1;
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
            
            SubscribeEvents();
          
        }
                
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitCmd();
        }

        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Duct Path Expansion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                    //  return true;
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
        }

        public void Resume()
        {
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
           // m_oIGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseDown);
            m_oIGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseUp);
           // m_oIGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oIGTCustomCommandHelper_WheelRotate);
        }
        #endregion

        #region close application
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
        //    m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
        //    m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }

        public void ExitCmd()
        {                                
            
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
            m_gtapp.SelectedObjects.Clear();
            UnsubscribeEvents();
            step = 0;
            
            if (mobjLocateService != null)
                mobjLocateService = null;
          
            m_oIGTCustomCommandHelper.Complete();


        }
        #endregion

        #region Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        #endregion

        #region GetDuctPathOrigin
        public bool GetDuctPathOrigin(short FNO, int FID)
        {
            try
            {
                if (DuctPathOrigin == null)
                    DuctPathOrigin = new DuctPath();
                IGTKeyObject oDuctPathFeature = m_IGTDataContext.OpenFeature(FNO, FID);
                #region Attr
                if (!oDuctPathFeature.Components.GetComponent(2201).Recordset.EOF)
                {
                    oDuctPathFeature.Components.GetComponent(2201).Recordset.MoveLast();
                    DuctPathOrigin.FNO = 2200;

                    for (int i = 0; i < oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields.Count; i++)
                    {
                        string TT = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name.ToString();
                        if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "G3E_FID")
                        {
                            DuctPathOrigin.FID = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_ND_FRM_ID")
                        {
                            DuctPathOrigin.sourceFID = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_ND_FRM_TY")
                        {
                            DuctPathOrigin.sourceType = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                            DuctPathOrigin.sourceFNO = GetFNObyFeatureType(DuctPathOrigin.sourceType);
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_MH_FRM_WALL")
                        {
                            DuctPathOrigin.sourceWall = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_ND_TO_ID")
                        {
                            DuctPathOrigin.termFID = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_ND_TO_TY")
                        {
                            DuctPathOrigin.termType = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                            DuctPathOrigin.termFNO = GetFNObyFeatureType(DuctPathOrigin.termType);
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_MH_TO_WALL")
                        {
                            DuctPathOrigin.termWall = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "TOTAL_LENGTH")
                        {
                            DuctPathOrigin.Length = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_WAYS")
                        {
                            DuctPathOrigin.DuctWay = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_CONSTRUCTION")
                        {
                            DuctPathOrigin.ConstructBy = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "EXPAND_FLAG")
                        {
                            DuctPathOrigin.DBFlag = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                        }
                    }
                }
                #endregion
                #region Netelem
                if (!oDuctPathFeature.Components.GetComponent(51).Recordset.EOF)
                {
                    oDuctPathFeature.Components.GetComponent(51).Recordset.MoveLast();

                    for (int i = 0; i < oDuctPathFeature.Components.GetComponent(51).Recordset.Fields.Count; i++)
                    {
                        if (oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Name == "EXC_ABB")
                        {
                            DuctPathOrigin.EXC_ABB = oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Name == "BILLING_RATE")
                        {
                            DuctPathOrigin.BillingRate = oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Name == "YEAR_PLACED")
                        {
                            DuctPathOrigin.InstallYear = oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Name == "FEATURE_STATE")
                        {
                            DuctPathOrigin.Feature_state = oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Value.ToString();
                        }

                    }
                }
                #endregion
              #region Sect
                if (!oDuctPathFeature.Components.GetComponent(2202).Recordset.EOF)
                {
                    oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveFirst();

                    for (int i = 0; i < oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields.Count; i++)
                    {
                        if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[i].Name == "DT_S_PP_WAYS")
                        {
                            int y = 0;
                            if (int.TryParse(oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[i].Value.ToString(),out y))
                                DuctPathOrigin.PPDuctWay = y;
                            else DuctPathOrigin.PPDuctWay = 0;
                            break;
                        }
                    }
                }

              #endregion
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region GetFNObyFeatureType
        public short GetFNObyFeatureType(string type)
        {
            if (type.Trim().ToUpper() == "MANHOLE")
                return 2700;
            if (type.TrimEnd().TrimStart().ToUpper() == "CIVIL NODE")
                return 2800;
            if (type.Trim().ToUpper() == "CHAMBER")
                return 3800;
            if (type.Trim().ToUpper() == "TUNNEL" || type.Trim().ToUpper() == "TRENCH")
                return 3300;
            return 0;
        }
        #endregion
    }
}
