using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.NEPSDuctNestDel
{
    class GTDuctNestDel : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        public static Intergraph.GTechnology.API.IGTDataContext m_IGTDataContext = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        IGTLocateService mobjLocateService = null;
        public static int step=0;
        public static int DelFeatureFID = 0;
        #endregion

        #region Mouse Click
        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            try
            {
                IGTPoint WorldPoint = e.WorldPoint;

                if (e.Button != 2)//left button
                {  
                   #region Get Selected DuctNest to delete
                    if (step == 1) 
                    {
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DuctNest feature for deleting! Right Click to exit.");
                       
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {
                           foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                            {
                               if (oDDCKeyObject.FNO != 2400)
                                {
                                    MessageBox.Show("Please select a DuctNest!", "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DuctNest feature for deleting! Right Click to exit.");
                                    return;
                                }
                                DialogResult retVal = MessageBox.Show("Are you sure that you want to delete\nDuctNest and all Ducts, SubDucts and InnerDucts?", "DuctNest Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (retVal == DialogResult.Yes)
                                {
                                    Messages frm = new Messages();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, deleting in process...");
                                    frm.Show();
                                    if (DeleteDuctNest(oDDCKeyObject.FID))
                                    {
                                        frm.Message(2);
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Completed succefully!");
                                        frm.Hide();
                                    }
                                    else
                                    {

                                        frm.Hide();
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Error! Exiting application.");
                                        ExitCmd();
                                        return;
                                    }
                                }
                                else
                                {
                                    m_gtapp.SelectedObjects.Clear();
                                    return;
                                }
                                break;                                
                            }

                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DuctNest feature for deleting! Right Click to exit.");

                        }
                        else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                        {
                            MessageBox.Show("Please select only one DuctNest at once!", "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            m_gtapp.SelectedObjects.Clear();
                            m_gtapp.ActiveMapWindow.Activate();
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DuctNest feature for deleting! Right Click to exit.");
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
                            DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "DuctNest Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                MessageBox.Show(ex.Message, "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        } 
        #endregion

        #region MouseMove
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            Messages frm = new Messages();
            try
            {
                
                
                    #region Get Selected DuctNest to delete
                    if (step == 1)
                    {
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DuctNest feature for deleting! Right Click to exit.");

                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {
                            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                            {
                                if (oDDCKeyObject.FNO != 2400)
                                {
                                    MessageBox.Show("Please select a DuctNest!", "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DuctNest feature for deleting! Right Click to exit.");
                                    return;
                                }
                                DialogResult retVal = MessageBox.Show("Are you sure that you want to delete\nDuctNest and all Ducts, SubDucts,InnerDucts and Cabels?", "DuctNest Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (retVal == DialogResult.Yes)
                                {
                                    
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, deleting in process...");
                                    frm.Show();
                                    if (DeleteDuctNest(oDDCKeyObject.FID))
                                    {
                                        frm.Message(2);
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Completed succefully!");
                                        frm.Hide();
                                    }
                                    else
                                    {
                                        frm.Close();
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Error! Exiting application.");
                                        ExitCmd();
                                        return;
                                    }
                                }
                                else
                                {
                                    m_gtapp.SelectedObjects.Clear();
                                    return;
                                }
                                break;
                            }

                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DuctNest feature for deleting! Right Click to exit.");

                        }
                        else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                        {
                            MessageBox.Show("Please select only one DuctNest at once!", "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            m_gtapp.SelectedObjects.Clear();
                            m_gtapp.ActiveMapWindow.Activate();
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DuctNest feature for deleting! Right Click to exit.");
                            return;
                        }
                        return;
                    }
                    #endregion
            }
            catch (Exception ex)
            {
                frm.Close();
                MessageBox.Show(ex.Message, "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                ExitCmd();
            }
        }
        #endregion

        #region unusing events
        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
           // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }       

        void m_oIGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
           // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");            
        }

        void m_oIGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
          //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }      

        void m_oIGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
           // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            //
        }

        void m_oIGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
          // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oIGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
         //   GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {

          //  if (e.KeyCode == 27 || e.KeyCode == 9)//button ESC
          //   { m_gtapp.ActiveMapWindow.Activate(); }
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }

        #endregion

        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                m_gtapp = GTClassFactory.Create<IGTApplication>();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Delete DuctNest . . . ");
                m_oIGTCustomCommandHelper = CustomCommandHelper;
                m_IGTDataContext = m_gtapp.DataContext;
                mobjLocateService = m_gtapp.ActiveMapWindow.LocateService;

                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
                }

                mobjLocateService = m_gtapp.ActiveMapWindow.LocateService;
                SubscribeEvents();
                              
                step = 1;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DuctNest feature for deleting! Right Click to exit.");
             }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();                
            }
        }
      

        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "DuctNest Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            step += 50000;
            
        }

        public void Resume()
        {
            step -= 50000;
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
      
        #region subscribe/unsubscribe events
        public void SubscribeEvents()
        {
            // Subscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
          //  m_oIGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oIGTCustomCommandHelper_Activate);
          //  m_oIGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oIGTCustomCommandHelper_Deactivate);
          //  m_oIGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oIGTCustomCommandHelper_GainedFocus);
         //   m_oIGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oIGTCustomCommandHelper_LostFocus);
         //   m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
         //   m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
          //  m_oIGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oIGTCustomCommandHelper_KeyPress);
          //  m_oIGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_Click);
           // m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);
            m_oIGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseMove);
         //   m_oIGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseDown);
            m_oIGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseUp);
         //   m_oIGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oIGTCustomCommandHelper_WheelRotate);
        }
        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
          //  m_oIGTCustomCommandHelper.Activate -= m_oIGTCustomCommandHelper_Activate;
          //  m_oIGTCustomCommandHelper.Deactivate -= m_oIGTCustomCommandHelper_Deactivate;
          //  m_oIGTCustomCommandHelper.GainedFocus -= m_oIGTCustomCommandHelper_GainedFocus;
          //  m_oIGTCustomCommandHelper.LostFocus -= m_oIGTCustomCommandHelper_LostFocus;
          //  m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
          //  m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
          //  m_oIGTCustomCommandHelper.KeyPress -= m_oIGTCustomCommandHelper_KeyPress;
          //  m_oIGTCustomCommandHelper.Click -= m_oIGTCustomCommandHelper_Click;
          //  m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
          //  m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
         //   m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }
        #endregion
        
        #region Exit CustomCommand
        public void ExitCmd()
        {
            m_gtapp.SetProgressBarRange(0, 0);
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting..."); 
            m_gtapp.SelectedObjects.Clear();
            UnsubscribeEvents();
            DelFeatureFID = 0;
            step = 0;       
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

        #region Delete DuctNest
        private bool DeleteDuctNest(int iFID)
        {

            string sSql = "  (select count(g3e_fid) from GC_CONTAIN where (G3E_FNO =4400 or G3E_FNO =4500 or "+
                " G3E_FNO =7000 or  G3E_FNO =7200 or  G3E_FNO =7400 ) and g3e_ownerfid in " +
" ( select g3e_fid from GC_CONTAIN where G3E_FNO in (16100,2100,2300) and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) " +
" union " +
 " ( select g3e_fid from GC_CONTAIN where  G3E_FNO in (2100,16100) and g3e_ownerfno=2300 and g3e_ownerfid in " +
 " ( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " )) " +
" union  " +
"( select g3e_fid from GC_CONTAIN where   G3E_FNO = 2100 and g3e_ownerfno=16100 and g3e_ownerfid in  " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO =16100 and g3e_ownerfno=2300 and g3e_ownerfid in " +
 " ( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ))))";

            int countCable = int.Parse(Get_Value(sSql));
            
            if (countCable > 0)
             {
                 MessageBox.Show("Not allowed to delete DuctNest with cables! Delete cabels first!", "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return false;                   
             }
            string DuctPathFID= Get_Value("select g3e_ownerfid from GC_CONTAIN where G3E_FNO=2400 and g3e_fid=" + iFID.ToString());
            int countDuct = int.Parse(Get_Value("select count(g3e_fid) from GC_CONTAIN where G3E_FNO =2300 and "+
                                                "  g3e_ownerfno=2400 and g3e_ownerfid=" + iFID.ToString()));
            int countInnerDuct = int.Parse(Get_Value("select count(g3e_fid) from GC_NETELEM where G3E_FNO =2100  and feature_state='PPF' and g3e_fid in ( " +
" select g3e_fid as countInn from GC_CONTAIN where   G3E_FNO = 2100 and g3e_ownerfno=16100 and g3e_ownerfid in " +
"(  select g3e_fid from GC_CONTAIN where G3E_FNO =16100 and g3e_ownerfno=2300 and g3e_ownerfid in " +
" ( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400  and g3e_ownerfid = " + iFID.ToString() + " ))  " +
" union  " +
" select g3e_fid as countInn  from GC_CONTAIN where  G3E_FNO =2100 and g3e_ownerfno=2300 and g3e_ownerfid in " +
" ( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400    and g3e_ownerfid = " + iFID.ToString() + " ) " +
"union  " +
" select g3e_fid as countInn  from GC_CONTAIN where G3E_FNO =2100 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " )"));
            int countSubDuct = int.Parse(Get_Value("select count(g3e_fid) from GC_NETELEM where G3E_FNO = 16100 and feature_state='PPF' and g3e_fid in ( " +
" select g3e_fid as countSub  from GC_CONTAIN where  G3E_FNO =16100 and g3e_ownerfno=2300 and g3e_ownerfid in " +
"(  select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) " +
" union  "+
" select g3e_fid as countSub  from GC_CONTAIN where G3E_FNO =16100 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + "  )"));

            sSql = "select g3e_fno, g3e_fid from GC_CONTAIN where   G3E_FNO = 2100 and g3e_ownerfno=16100 and g3e_ownerfid in  " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO =16100 and g3e_ownerfno=2300 and g3e_ownerfid in " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ))  " +
 "union "+
 "select g3e_fno, g3e_fid from GC_CONTAIN where  G3E_FNO in (2100,16100) and g3e_ownerfno=2300 and g3e_ownerfid in " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) " +
"union  "+
"( select g3e_fno, g3e_fid from GC_CONTAIN where G3E_FNO in (16100,2100,2300) and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) ";
            
            ADODB.Recordset rsPP = new ADODB.Recordset();
            rsPP = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            try
            {

                m_oIGTTransactionManager.Begin("DeleteDuctNestFID=" + iFID.ToString());
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        if (!DeleteFeature(short.Parse(rsPP.Fields[0].Value.ToString()), int.Parse(rsPP.Fields[1].Value.ToString())))
                        {

                            m_oIGTTransactionManager.Rollback();
                            return false;
                        }
                        rsPP.MoveNext();
                    }

                }
                rsPP = null;
                if (!DeleteFeature(2400, iFID))
                {

                    m_oIGTTransactionManager.Rollback();
                    return false;
                }

                if (countDuct > 0)
                {
                    IGTKeyObject DuctPath = m_gtapp.DataContext.OpenFeature(2200,int.Parse(DuctPathFID));
                    int DT_NEST_WAYS = int.Parse(DuctPath.Components.GetComponent(2201).Recordset.Fields["DT_NEST_WAYS"].Value.ToString());
                    int DT_PP_INNDUCT = int.Parse(DuctPath.Components.GetComponent(2201).Recordset.Fields["DT_PP_INNDUCT"].Value.ToString());
                    int DT_PP_SUBDUCT = int.Parse(DuctPath.Components.GetComponent(2201).Recordset.Fields["DT_PP_SUBDUCT"].Value.ToString());
                    DuctPath.Components.GetComponent(2201).Recordset.Update("DT_PP_INNDUCT", DT_PP_INNDUCT - countInnerDuct);
                    DuctPath.Components.GetComponent(2201).Recordset.Update("DT_PP_SUBDUCT", DT_PP_SUBDUCT-  countSubDuct);
                    DuctPath.Components.GetComponent(2201).Recordset.Update("DT_NEST_WAYS", DT_NEST_WAYS - countDuct);
                    //string Sql = " update GC_COND set DT_NEST_WAYS=(DT_NEST_WAYS - " + countDuct.ToString() +
                    //             " ), DT_PP_INNDUCT=(DT_PP_INNDUCT - " + countInnerDuct.ToString() +
                    //             " ), DT_PP_SUBDUCT=(DT_PP_SUBDUCT - " + countSubDuct.ToString() +
                    //             " ) where g3e_fno=2200 and g3e_fid=" + DuctPathFID;
                    //int roweff = 0;
                   // m_IGTDataContext.Execute(Sql, out roweff, -1);
                }
                //if (countInnerDuct > 0)
                //{
                //    string Sql = " update GC_COND set DT_PP_INNDUCT=(DT_PP_INNDUCT - " + countInnerDuct.ToString() +
                //                 " ) where g3e_fno=2200 and g3e_fid=" + DuctPathFID;
                //    int roweff = 0;
                //    m_IGTDataContext.Execute(Sql, out roweff, -1);
                //}
                
                //if (countSubDuct > 0)
                //{
                //    string Sql = " update GC_COND set DT_PP_SUBDUCT=(DT_PP_SUBDUCT - " + countSubDuct.ToString() +
                //                 " ) where g3e_fno=2200 and g3e_fid=" + DuctPathFID;
                //    int roweff = 0;
                //    m_IGTDataContext.Execute(Sql, out roweff, -1);
                //}
                
                m_oIGTTransactionManager.Commit();
                m_oIGTTransactionManager.RefreshDatabaseChanges();
                return true;
            }
            catch (Exception ex)
            {

                m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);               
                return false;
            }
                return true;
        }
        #endregion

        #region Delete Feature
        private bool DeleteFeature(short iFNO, int iFID)
        {
            try
            {
                string Sql = "select co.g3e_table from g3e_component co, g3e_featurecomponent fc "+
                    " where fc.g3e_cno=co.g3e_cno and fc.g3e_fno=" + iFNO.ToString() + " order by fc.g3e_cno asc";
                int roweff = 0;

                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = m_IGTDataContext.OpenRecordset(Sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        Sql = "delete from "+rsPP.Fields[0].Value.ToString()+" where g3e_fno=" + iFNO.ToString()+
                             " and g3e_fid=" + iFID.ToString();

                        m_IGTDataContext.Execute(Sql, out roweff, -1);
                        rsPP.MoveNext();
                    }

                }
                rsPP = null;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DuctNest Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        #endregion

        
    }
}
