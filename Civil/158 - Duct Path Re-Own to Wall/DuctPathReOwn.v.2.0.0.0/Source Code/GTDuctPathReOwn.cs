using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.NEPSDuctPathReOwn
{
    class GTDuctPathReOwn : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables member
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        public static Intergraph.GTechnology.API.IGTDataContext m_IGTDataContext = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditServiceRotate = null;
        DPReOwnForm DPExpanForm = null;
        IGTLocateService mobjLocateService = null;

        #region define Duct Path class
        public class TextPointGeom
        {
            public IGTTextPointGeometry geom;
            public int CID;
            public short CNO;
        };
        public class OrientedPointGeom
        {
            public IGTOrientedPointGeometry geom;
            public int CID;
            public short CNO;
        };
        public class PolylineGeom
        {
            public IGTPolylineGeometry geom;
            public int CID;
            public short CNO;
        };
        public class Duct
        {
            public int FID;
            public short FNO;
            public int styleID;
           // public string Feature_state;
            public List<OrientedPointGeom> Form;
        };
        public class DuctNest
        {
            public int FID;
            public short FNO;
            //public string Feature_state;
            public int styleIDlabel;
            public int styleIDform;
            public List<TextPointGeom> Labels;
            public List<PolylineGeom> Form;
            public List<Duct> Ducts;
            
        };
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
            public IGTPolylineGeometry DuctPathLineGeom;
            public List<DuctNest> DuctNestFrom;
            public List<DuctNest> DuctNestTo;
        };
        #endregion
        public static int step = 0;
        public static DuctPath DuctPathOrigin = null;
        public static bool FromTo = true;
        public static int CountGeomDuctNest = 0;
        #endregion      

        #region Event Handlers
       
          #region Mouse Move
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
           try
           {
               IGTPoint WorldPoint = e.WorldPoint;

                #region Get Selected Duct Path for editing
                if (step == 1)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");

                    if (m_gtapp.SelectedObjects.FeatureCount == 1)
                    {
                        foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                        {
                            if (oDDCKeyObject.FNO != 2200)
                            {
                                MessageBox.Show("Please select a Duct Path!", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                m_gtapp.ActiveMapWindow.Activate();
                                m_gtapp.SelectedObjects.Clear();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");
                                return;
                            }
                            if (DuctPathOrigin == null)
                            {
                                if (GetDuctPathOrigin(oDDCKeyObject.FNO, oDDCKeyObject.FID))
                                {

                                    step = 0;
                                    if (DPExpanForm == null)
                                    {
                                        DPExpanForm = new DPReOwnForm();
                                        DPExpanForm.FormClosed += new FormClosedEventHandler(DPExpanForm_FormClosed);
                                    }
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                                    DPExpanForm.Show();
                                    return;
                                }
                                else
                                {
                                    if (DuctPathOrigin != null)
                                        DuctPathOrigin = null;
                                    MessageBox.Show("Please select one more time!", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");
                                    return;
                                }
                            }


                        }
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");

                    }
                    else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                    {
                        MessageBox.Show("Please select only one Duct Path at once!", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_gtapp.SelectedObjects.Clear();
                        m_gtapp.ActiveMapWindow.Activate();
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");
                        return;
                    }
                    return;
                }
                #endregion

                #region Get Selected Source Wall OR Term Wall of Device
                if (step == 20 || step == 30)
                {
                    string wall = "";
                    if (FromTo)
                        wall = DuctPathOrigin.sourceWall.ToString();
                    else wall = DuctPathOrigin.termWall.ToString();
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Wall to Re-Own! Right Click to cancel. Current Wall - "+wall);
                    m_gtapp.SelectedObjects.Clear();
                    return;
                }
                #endregion

                #region move duct nest
                if (step == 50)
                {

                   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location! Right Click to cancel moving.");
                   m_gtapp.SelectedObjects.Clear();
                   IGTDDCKeyObjects feat = null;
                   

                    if (FromTo)
                    {
                        mobjEditService.RemoveAllGeometries();
                        mobjEditServiceRotate.RemoveAllGeometries();
                        int i = CountGeomDuctNest;


                        feat = GTDuctPathReOwn.m_gtapp.DataContext.GetDDCKeyObjects(DuctPathOrigin.DuctNestFrom[i].FNO, DuctPathOrigin.DuctNestFrom[i].FID, GTComponentGeometryConstants.gtddcgAllGeographic);
                        for (int K2 = 0; K2 < feat.Count; K2++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                      //  m_gtapp.RefreshWindows();

                            for (int j = 0; j < DuctPathOrigin.DuctNestFrom[i].Form.Count; j++)
                            {
                                mobjEditService.AddGeometry(DuctPathOrigin.DuctNestFrom[i].Form[j].geom, DuctPathOrigin.DuctNestFrom[i].styleIDform);
                             //   mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestFrom[i].Form[j], DuctPathOrigin.DuctNestFrom[i].styleIDform);
                            
                            }
                            for (int j = 0; j < DuctPathOrigin.DuctNestFrom[i].Labels.Count; j++)
                            {
                                mobjEditService.AddGeometry(DuctPathOrigin.DuctNestFrom[i].Labels[j].geom, DuctPathOrigin.DuctNestFrom[i].styleIDlabel);
                              //  mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestFrom[i].Labels[j], DuctPathOrigin.DuctNestFrom[i].styleIDlabel);
                            
                            }

                            for (int j = 0; j < DuctPathOrigin.DuctNestFrom[i].Ducts.Count; j++)
                            {
                                for (int k = 0; k < DuctPathOrigin.DuctNestFrom[i].Ducts[j].Form.Count; k++)
                                {
                                    mobjEditService.AddGeometry(DuctPathOrigin.DuctNestFrom[i].Ducts[j].Form[k].geom, DuctPathOrigin.DuctNestFrom[i].Ducts[j].styleID);
                                 //   mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestFrom[i].Ducts[j].Form[k], DuctPathOrigin.DuctNestFrom[i].Ducts[j].styleID);
                                }
                            }
                                            }
                    else
                    {
                        int i = CountGeomDuctNest;
                        feat = GTDuctPathReOwn.m_gtapp.DataContext.GetDDCKeyObjects(DuctPathOrigin.DuctNestTo[i].FNO, DuctPathOrigin.DuctNestTo[i].FID, GTComponentGeometryConstants.gtddcgAllGeographic);
                        for (int K2 = 0; K2 < feat.Count; K2++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                        //  m_gtapp.RefreshWindows();
                            for (int j = 0; j < DuctPathOrigin.DuctNestTo[i].Form.Count; j++)
                            {
                                mobjEditService.AddGeometry(DuctPathOrigin.DuctNestTo[i].Form[j].geom, DuctPathOrigin.DuctNestTo[i].styleIDform);
                             //   mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestTo[i].Form[j], DuctPathOrigin.DuctNestTo[i].styleIDform);
                            }
                            for (int j = 0; j < DuctPathOrigin.DuctNestTo[i].Labels.Count; j++)
                            {
                                mobjEditService.AddGeometry(DuctPathOrigin.DuctNestTo[i].Labels[j].geom, DuctPathOrigin.DuctNestTo[i].styleIDlabel);
                             //   mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestTo[i].Labels[j], DuctPathOrigin.DuctNestTo[i].styleIDlabel);
                            }

                            for (int j = 0; j < DuctPathOrigin.DuctNestTo[i].Ducts.Count; j++)
                            {
                                for (int k = 0; k < DuctPathOrigin.DuctNestTo[i].Ducts[j].Form.Count; k++)
                                {
                                    mobjEditService.AddGeometry(DuctPathOrigin.DuctNestTo[i].Ducts[j].Form[k].geom, DuctPathOrigin.DuctNestTo[i].Ducts[j].styleID);
                                  //  mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestTo[i].Ducts[j].Form[k], DuctPathOrigin.DuctNestTo[i].Ducts[j].styleID);
                                
                                }
                            }
                        
                    }
                    mobjEditService.BeginMove(mobjEditService.GetGeometry(1).FirstPoint);
                    step = 51;
                    return;
                } 
               if (step == 51)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location! Right Click to cancel moving.");
                    mobjEditService.Move(WorldPoint);
                    return;
                }

                #endregion
                #region rotating nest
                if (step == 60)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm rotation! Right Click  to skip rotation.");
                    if (FromTo)
                    {

                        int countgeom = 0; //mobjEditService.GeometryCount;
                        int i = CountGeomDuctNest;
                        for (int j = 0; j < DuctPathOrigin.DuctNestFrom[i].Form.Count; j++)
                        {
                            countgeom++;
                            mobjEditServiceRotate.AddGeometry(mobjEditService.GetGeometry(countgeom), DuctPathOrigin.DuctNestFrom[i].styleIDform);

                        }
                        for (int j = 0; j < DuctPathOrigin.DuctNestFrom[i].Labels.Count; j++)
                        {
                            countgeom++;
                            mobjEditServiceRotate.AddGeometry(mobjEditService.GetGeometry(countgeom), DuctPathOrigin.DuctNestFrom[i].styleIDlabel);

                        }

                        for (int j = 0; j < DuctPathOrigin.DuctNestFrom[i].Ducts.Count; j++)
                        {
                            for (int k = 0; k < DuctPathOrigin.DuctNestFrom[i].Ducts[j].Form.Count; k++)
                            {
                                countgeom++;
                                mobjEditServiceRotate.AddGeometry(mobjEditService.GetGeometry(countgeom), DuctPathOrigin.DuctNestFrom[i].Ducts[j].styleID);
                            }
                        }
                    }
                    else
                    {
                        int countgeom = 0; //mobjEditService.GeometryCount;
                        int i = CountGeomDuctNest;
                        for (int j = 0; j < DuctPathOrigin.DuctNestTo[i].Form.Count; j++)
                        {
                            countgeom++;
                            mobjEditServiceRotate.AddGeometry(mobjEditService.GetGeometry(countgeom), DuctPathOrigin.DuctNestTo[i].styleIDform);

                        }
                        for (int j = 0; j < DuctPathOrigin.DuctNestTo[i].Labels.Count; j++)
                        {
                            countgeom++;
                            mobjEditServiceRotate.AddGeometry(mobjEditService.GetGeometry(countgeom), DuctPathOrigin.DuctNestTo[i].styleIDlabel);

                        }

                        for (int j = 0; j < DuctPathOrigin.DuctNestTo[i].Ducts.Count; j++)
                        {
                            for (int k = 0; k < DuctPathOrigin.DuctNestTo[i].Ducts[j].Form.Count; k++)
                            {
                                countgeom++;
                                mobjEditServiceRotate.AddGeometry(mobjEditService.GetGeometry(countgeom), DuctPathOrigin.DuctNestTo[i].Ducts[j].styleID);
                            }
                        }
                    }
                    mobjEditServiceRotate.BeginRotate(mobjEditServiceRotate.GetGeometry(1).FirstPoint, mobjEditServiceRotate.GetGeometry(1).FirstPoint);
                    step = 61;
                    return;
                }
                if (step == 61)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm rotation! Right Click  to skip rotation.");
                    mobjEditServiceRotate.Rotate(WorldPoint);
                    return;
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if (e.Button == 1)//left button
                {
                    #region moving nest
                    if (step == 51)
                    {
                        mobjEditService.EndMove(WorldPoint);
                        step = 60;
                        return;
                    }
                    #endregion

                    #region rotating nest
                    if (step == 61)
                    {
                        mobjEditServiceRotate.EndRotate(WorldPoint);
                        mobjEditService.RemoveAllGeometries();
                        if(FromTo)
                        {
                            UpdateGeomDuctNest(DuctPathOrigin.DuctNestFrom[CountGeomDuctNest]);
                            mobjEditServiceRotate.RemoveAllGeometries();
                            if (CountGeomDuctNest < DuctPathOrigin.DuctNestFrom.Count - 1)
                            {
                                CountGeomDuctNest++;
                                step = 50;
                            }
                            else
                            {
                                step = 0;
                                CountGeomDuctNest = 0; 
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                                //DPExpanForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                                DPExpanForm.Show();
                                return;
                            }
                        }
                        else 
                        {
                            UpdateGeomDuctNest(DuctPathOrigin.DuctNestTo[CountGeomDuctNest]);
                            mobjEditServiceRotate.RemoveAllGeometries();
                            if (CountGeomDuctNest < DuctPathOrigin.DuctNestTo.Count - 1)
                            {
                                CountGeomDuctNest++;
                                step = 50;
                            }
                            else
                            {
                                step = 0;
                                CountGeomDuctNest = 0;
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                               // DPExpanForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                                DPExpanForm.Show();
                                return;
                            }
                        }
                        
                        return;
                    }
                    #endregion

                    #region Get Selected Duct Path for editing
                    if (step == 1) 
                    {
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");
                       
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
                        
                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {
                            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                            {
                                if (oDDCKeyObject.FNO != 2200)
                                {
                                    MessageBox.Show("Please select a Duct Path!", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");
                                    return;
                                }
                                if (DuctPathOrigin == null)
                                {
                                    if (GetDuctPathOrigin(oDDCKeyObject.FNO, oDDCKeyObject.FID))
                                    {
                                        step = 0;
                                        if (DPExpanForm == null)
                                        {
                                            DPExpanForm = new DPReOwnForm();
                                            DPExpanForm.FormClosed += new FormClosedEventHandler(DPExpanForm_FormClosed);
                                        }
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                                        DPExpanForm.Show();
                                        return;
                                    }
                                    else
                                    {
                                        if (DuctPathOrigin != null)
                                            DuctPathOrigin = null;
                                        MessageBox.Show("Please select one more time!", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        m_gtapp.ActiveMapWindow.Activate();
                                        m_gtapp.SelectedObjects.Clear();
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");
                                        return;
                                    }
                                }
                                

                            }
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");

                        }
                        else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                        {
                            MessageBox.Show("Please select only one Duct Path at once!", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            m_gtapp.SelectedObjects.Clear();
                            m_gtapp.ActiveMapWindow.Activate();
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for Re-Own to Wall! Right Click to exit.");
                            return;
                        }
                        return;
                    }
                    #endregion
                    
                    #region Get Selected Source Wall OR Term Wall of Device
                    if (step == 21 || step == 31)
                    {
                        if (step == 21)
                            step = 20;
                        if (step == 31)
                            step = 30;
                        m_gtapp.SelectedObjects.Clear();
                        m_gtapp.RefreshWindows();
                        return;
                    }
                    if (step == 20 || step == 30)
                    {

                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Wall to Re-Own! Right Click to cancel.");
                        IGTDDCKeyObjects feat = null;
                            feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
                        
                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {
                            if ((step == 20 && DPExpanForm.GetDeviceWithWall(true))//source
                                || (step == 30 && DPExpanForm.GetDeviceWithWall(false)))//term
                            {
                                //step = 0;
                               // DPExpanForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                              //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                               // DPExpanForm.Show();
                                return;
                            }
                            else
                            {
                                if (step == 20 )
                                step = 21;
                            if (step == 30)
                                step = 31;
                                m_gtapp.SelectedObjects.Clear();
                                m_gtapp.RefreshWindows();
                                return;
                            }
                        }

                    }
                    #endregion
                   
                }
                else  if (e.Button == 2)//right click
                {
                    #region exiting from application
                    if (step == 1 )//exiting from application
                        {
                            DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Duct Path Re-Own to Wall", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (retVal == DialogResult.Yes)
                            {
                                ExitCmd();
                                return;
                            }                            
                            m_gtapp.ActiveMapWindow.Activate();
                            return;
                        }
                    #endregion

                    #region Cancelation for Wall Selection
                        if (step == 20 || step == 30 )
                {
                   // DPExpanForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                    DPExpanForm.Show();
                    step = 0;
                   // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                    return;
                }
                #endregion

                    #region cancel moving nest
                if (step == 51)
                {
                    mobjEditService.EndMove(WorldPoint);
                    mobjEditService.RemoveAllGeometries();
                    m_gtapp.SelectedObjects.Clear();
                    step = 0;
                    CountGeomDuctNest = 0;
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                    DPExpanForm.Show();
                    return;
                }
                #endregion

                #region cancel rotating nest
                if (step == 61)
                {
                    mobjEditServiceRotate.EndRotate(mobjEditServiceRotate.GetGeometry(1).FirstPoint);
                    mobjEditService.RemoveAllGeometries();
                    if (FromTo)
                    {
                        UpdateGeomDuctNest(DuctPathOrigin.DuctNestFrom[CountGeomDuctNest]);
                        mobjEditServiceRotate.RemoveAllGeometries();
                        if (CountGeomDuctNest < DuctPathOrigin.DuctNestFrom.Count - 1)
                        {
                            CountGeomDuctNest++;
                            step = 50;
                        }
                        else
                        {
                            step = 0;
                            CountGeomDuctNest = 0;
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                           // DPExpanForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                            DPExpanForm.Show();
                            return;
                        }
                    }
                    else
                    {
                        UpdateGeomDuctNest(DuctPathOrigin.DuctNestTo[CountGeomDuctNest]);
                        mobjEditServiceRotate.RemoveAllGeometries();
                        if (CountGeomDuctNest < DuctPathOrigin.DuctNestTo.Count - 1)
                        {
                            CountGeomDuctNest++;
                            step = 50;
                        }
                        else
                        {
                            step = 0;
                            CountGeomDuctNest = 0;
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                          //  DPExpanForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                            DPExpanForm.Show();
                            return;
                        }
                    }
                    return;
                }
                #endregion

                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Duct Path Re-Own to Wall . . . ");
            m_oIGTCustomCommandHelper = CustomCommandHelper;
            m_IGTDataContext = m_gtapp.DataContext;
            mobjLocateService = m_gtapp.ActiveMapWindow.LocateService;
            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;
            mobjEditServiceRotate = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceRotate.TargetMapWindow = m_gtapp.ActiveMapWindow;

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
            }

            SubscribeEvents();

            step = 1;
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for re-own to wall! Right Click to exit.");
            
            SubscribeEvents();
          
        }
                
        void DPExpanForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitCmd();
        }

        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Duct Path Re-Own to Wall", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            step += 1000;
        }

        public void Resume()
        {
            step -= 1000;
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
            CountGeomDuctNest = 0;

            if (mobjLocateService != null)
                mobjLocateService = null;
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
            }
            if (mobjEditServiceRotate != null)
            {
                mobjEditServiceRotate.RemoveAllGeometries();
                mobjEditServiceRotate = null;
            }
            if (DuctPathOrigin != null)
            {
                if (DuctPathOrigin.DuctNestTo != null)
                {
                    DuctPathOrigin.DuctNestTo.Clear();
                    DuctPathOrigin.DuctNestTo = null;
                }
                if (DuctPathOrigin.DuctNestFrom != null)
                {
                    DuctPathOrigin.DuctNestFrom.Clear();
                    DuctPathOrigin.DuctNestFrom = null;
                }
                DuctPathOrigin = null;
            }
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
                            DuctPathOrigin.sourceType = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString().Trim();
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
                            DuctPathOrigin.termType = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString().Trim();
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
                #region Geom
                if (!oDuctPathFeature.Components.GetComponent(2210).Recordset.EOF)
                {
                   
                    IGTCompositePolylineGeometry tempcomp = (IGTCompositePolylineGeometry)oDuctPathFeature.Components.GetComponent(2210).Geometry;
                    DuctPathOrigin.DuctPathLineGeom = (IGTPolylineGeometry)(tempcomp.ExtractGeometry(tempcomp.FirstPoint, tempcomp.LastPoint, false));

                    //IGTGeometry temp = (IGTGeometry)oDuctPathFeature.Components.GetComponent(2210).Geometry;                  
                    //DuctPathOrigin.DuctPathLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    //for (int i = 0; i < temp.KeypointCount; i++)
                    //{
                    //    DuctPathOrigin.DuctPathLineGeom.Points.Add(temp.GetKeypointPosition(i));
                    //}
                }
                #endregion
                #region DuctNest
                
                int CountDuctNest = int.Parse(Get_Value("select count(g3e_fid) from GC_CONTAIN where G3E_FNO=2400 and g3e_ownerfno=2200 and g3e_ownerfid =" + DuctPathOrigin.FID.ToString()));
                if (CountDuctNest > 0)
                {
                    string sSql = "select g3e_fid from GC_CONTAIN where G3E_FNO=2400 and g3e_ownerfno=2200 and g3e_ownerfid =" + DuctPathOrigin.FID.ToString();
                    ADODB.Recordset rsPP = new ADODB.Recordset();
                    rsPP = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rsPP.RecordCount > 0)
                    {
                        rsPP.MoveFirst();
                        for (int i = 0; i < rsPP.RecordCount; i++)
                        {
                            
                            DuctNest tempFrom = new DuctNest();
                            tempFrom.FID = int.Parse(rsPP.Fields[0].Value.ToString());
                            tempFrom.FNO = 2400;
                            if (tempFrom.Ducts == null)
                                tempFrom.Ducts = new List<Duct>();
                            tempFrom.Ducts = GetAllDuct(tempFrom.FID, true);
                            tempFrom.styleIDform = 2410002;
                            tempFrom.styleIDlabel = 2430002;

                            DuctNest tempTo = new DuctNest();
                            tempTo.FID = tempFrom.FID;
                            tempTo.FNO = 2400;
                            if (tempTo.Ducts == null)
                                tempTo.Ducts = new List<Duct>();
                            tempTo.Ducts = GetAllDuct(tempTo.FID, false);
                            tempTo.styleIDform = 2410002;
                            tempTo.styleIDlabel = 2430002;
                            IGTKeyObject oDuctNestFeature = m_IGTDataContext.OpenFeature(2400, tempFrom.FID);

                            short sCNO = 2410;
                            if (!oDuctNestFeature.Components.GetComponent(sCNO).Recordset.EOF)
                            {
                                oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveFirst();
                                for (int j = 0; j < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                                {
                                    //string t = oDuctNestFeature.Components.GetComponent(sCNO).Geometry.Type;
                                   // t = "";
                                  //  IGTCompositePolylineGeometry tempcomp = (IGTCompositePolylineGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    IGTPolylineGeometry tempform = (IGTPolylineGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    PolylineGeom ttemp = new PolylineGeom();
                                    ttemp.geom = tempform;
                                    ttemp.CNO = sCNO;
                                    for (int k = 0; k < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                                    {
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                        {
                                            ttemp.CID=int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                        }
                                    } 
                                    if (tempFrom.Form == null)
                                        tempFrom.Form = new List<PolylineGeom>();
                                    tempFrom.Form.Add(ttemp);
                                    oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveNext();
                                }
                            }
                            sCNO = 2430;
                            if (!oDuctNestFeature.Components.GetComponent(sCNO).Recordset.EOF)
                            {
                                oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveFirst();
                                for (int j = 0; j < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                                {
                                   IGTOrientedPointGeometry tempcomp = (IGTOrientedPointGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                   TextPointGeom ttemp = new TextPointGeom();
                                   ttemp.CNO = sCNO;
                                   
                                    IGTTextPointGeometry templabel = GTClassFactory.Create<IGTTextPointGeometry>();
                                    templabel.Origin = tempcomp.Origin;
                                    templabel.Rotation = AngleBtwPoint(tempcomp.Orientation.I, tempcomp.Orientation.J);
                                    for (int k = 0; k < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                                    {
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_TEXT")
                                        {
                                            templabel.Text = oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString();
                                        }
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_ALIGNMENT")
                                        {
                                            templabel.Alignment = (GTAlignmentConstants)(int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString()));
                                        }
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                        {
                                            ttemp.CID = int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                        }
                                    }
                                    ttemp.geom = templabel;
                                        //(IGTTextPointGeometry)(tempcomp.ExtractGeometry(tempcomp.FirstPoint, tempcomp.LastPoint, false)); 
                                    if (tempFrom.Labels == null)
                                        tempFrom.Labels = new List<TextPointGeom>();
                                    tempFrom.Labels.Add(ttemp);
                                    oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveNext();
                                }
                            }
                            sCNO = 2412;
                            if (!oDuctNestFeature.Components.GetComponent(sCNO).Recordset.EOF)
                            {
                                oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveFirst();
                                for (int j = 0; j < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                                {
                                   // IGTCompositePolylineGeometry tempcomp = (IGTCompositePolylineGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    IGTPolylineGeometry tempform = (IGTPolylineGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    PolylineGeom ttemp = new PolylineGeom();
                                    ttemp.geom = tempform;
                                    ttemp.CNO = sCNO;
                                    for (int k = 0; k < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                                    {
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                        {
                                            ttemp.CID = int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                        }
                                    } 
                                    if (tempTo.Form == null)
                                        tempTo.Form = new List<PolylineGeom>();
                                    tempTo.Form.Add(ttemp);
                                    oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveNext();
                                }
                            }
                            sCNO = 2432;
                            if (!oDuctNestFeature.Components.GetComponent(sCNO).Recordset.EOF)
                            {
                                oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveFirst();
                                for (int j = 0; j < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                                {
                                    IGTOrientedPointGeometry tempcomp = (IGTOrientedPointGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    TextPointGeom ttemp = new TextPointGeom();
                                    ttemp.CNO = sCNO;
                                
                                    IGTTextPointGeometry templabel = GTClassFactory.Create<IGTTextPointGeometry>();
                                    templabel.Origin = tempcomp.Origin;
                                    templabel.Rotation = AngleBtwPoint(tempcomp.Orientation.I, tempcomp.Orientation.J);
                                    for (int k = 0; k < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                                    {
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_TEXT")
                                        {
                                            templabel.Text = oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString();
                                        }
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_ALIGNMENT")
                                        {
                                            templabel.Alignment = (GTAlignmentConstants)(int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString()));
                                        }
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                        {
                                            ttemp.CID = int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                        }
                                    }

                                    ttemp.geom = templabel;
                                    if (tempTo.Labels == null)
                                        tempTo.Labels = new List<TextPointGeom>();
                                    tempTo.Labels.Add(ttemp);
                                    oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveNext();
                                }
                            }
                            if(DuctPathOrigin.DuctNestFrom==null)
                                DuctPathOrigin.DuctNestFrom = new List<DuctNest>();
                            if (DuctPathOrigin.DuctNestTo == null)
                                DuctPathOrigin.DuctNestTo = new List<DuctNest>();

                            DuctPathOrigin.DuctNestFrom.Add(tempFrom);
                            DuctPathOrigin.DuctNestTo.Add(tempTo);
                            rsPP.MoveNext();
                        }

                    }
                    rsPP = null;
           
                }

                #endregion
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region get all duct/subduct/innerduct/cable
        public List<Duct> GetAllDuct(int iFID, bool from)
        {
            List<Duct> ductsnew = new List<Duct>();

            string sSql = "  select g3e_fno, g3e_fid from GC_CONTAIN where (G3E_FNO =4400 or G3E_FNO =4500 or " +
                " G3E_FNO =7000 or  G3E_FNO =7200 or  G3E_FNO =7400 ) and g3e_ownerfid in (" +
" ( select g3e_fid from GC_CONTAIN where G3E_FNO in (16100,2100,2300) and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) " +
" union " +
 " ( select g3e_fid from GC_CONTAIN where  G3E_FNO in (2100,16100) and g3e_ownerfno=2300 and g3e_ownerfid in " +
 " ( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " )) " +
" union  " +
"( select g3e_fid from GC_CONTAIN where   G3E_FNO = 2100 and g3e_ownerfno=16100 and g3e_ownerfid in  " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO =16100 and g3e_ownerfno=2300 and g3e_ownerfid in " +
 " ( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ))))" +
 " union " +
                "select g3e_fno, g3e_fid from GC_CONTAIN where   G3E_FNO = 2100 and g3e_ownerfno=16100 and g3e_ownerfid in  " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO =16100 and g3e_ownerfno=2300 and g3e_ownerfid in " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ))  " +
 " union " +
 "select g3e_fno, g3e_fid from GC_CONTAIN where  G3E_FNO in (2100,16100) and g3e_ownerfno=2300 and g3e_ownerfid in " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) " +
" union  " +
"( select g3e_fno, g3e_fid from GC_CONTAIN where G3E_FNO in (16100,2100,2300) and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) ";

            ADODB.Recordset rsPP = new ADODB.Recordset();
            rsPP = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

            if (rsPP.RecordCount > 0)
            {
                rsPP.MoveFirst();
                for (int i = 0; i < rsPP.RecordCount; i++)
                {
                    Duct temp = new Duct();
                    temp.FNO = short.Parse(rsPP.Fields[0].Value.ToString());
                    temp.FID = int.Parse(rsPP.Fields[1].Value.ToString());
                    IGTKeyObject oDuct = m_IGTDataContext.OpenFeature(temp.FNO, temp.FID);
                    short sCNO = GetCno(temp.FNO, from);
                    temp.styleID = GetStyleId(temp.FNO);
                    if (!oDuct.Components.GetComponent(sCNO).Recordset.EOF)
                    {
                        oDuct.Components.GetComponent(sCNO).Recordset.MoveFirst();
                        for (int j = 0; j < oDuct.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                        {
                            IGTOrientedPointGeometry temppoint = (IGTOrientedPointGeometry)(oDuct.Components.GetComponent(sCNO).Geometry);//.ExtractGeometry(((IGTCompositePolylineGeometry)oDuctPathFeature.Components.GetComponent(2210).Geometry).FirstPoint, ((IGTCompositePolylineGeometry)oDuctPathFeature.Components.GetComponent(2210).Geometry).LastPoint, false);
                            OrientedPointGeom ttemp = new OrientedPointGeom();
                            ttemp.geom = temppoint;
                            ttemp.CNO = sCNO;
                            for (int k = 0; k < oDuct.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                            {
                                if (oDuct.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                {
                                    ttemp.CID = int.Parse(oDuct.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                }
                            } 
                            if (temp.Form == null)
                                temp.Form = new List<OrientedPointGeom>();
                            temp.Form.Add(ttemp);
                            oDuct.Components.GetComponent(sCNO).Recordset.MoveNext();
                        }
                    }
                    ductsnew.Add(temp);
                    rsPP.MoveNext();
                }

            }
            rsPP = null;
            return ductsnew;
        }

        public short GetCno(short FNO, bool from)
        {
            if (FNO == 16100 && from)
                return 16120;
            if (FNO == 16100 && !from)
                return 16122;

            if (FNO == 2100 && from)
                return 2120;
            if (FNO == 2100 && !from)
                return 2122;

            if (FNO == 2300 && from)
                return 2320;
            if (FNO == 2300 && !from)
                return 2322;

            if (FNO == 4400)// && from)
                return 4422;
            //if (FNO == 4400 && !from)
            //   return 4400;

            if (FNO == 4500)//&& from)
                return 4522;
            //if (FNO == 4500 && !from)
            //   return 4500;

            if (FNO == 7000)// && from)
                return 7020;
            //if (FNO == 7000 && !from)
            //   return 7000;

            if (FNO == 7200)//&& from)
                return 7222;
            //if (FNO == 7200 && !from)
            //   return 7200;

            if (FNO == 7400)//&& from)
                return 7422;
            //if (FNO == 7400 && !from)
            //   return 7400;
            return 0;
        }

        public int GetStyleId(short FNO)
        {
            if (FNO == 16100)
                return 1612002;
            if (FNO == 2100 )
                return 2120002;
            if (FNO == 2300 )
                return 2320002;
           
            if (FNO == 4400)
                return 7020002;
            if (FNO == 4500)
                return 7020002;
            if (FNO == 7000)
                return 7020002;
            if (FNO == 7200)
                return 7020002;
            if (FNO == 7400)
                return 7020002;
            return 0;
        }
        #endregion
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

        #region angle between points
        public double AngleBtwPoint(double diffX, double diffY)
        {
            double t1 = diffY;
            double t2 = diffX;

            if (t1 == 0 && t2 == 0) return 0;
            if (t2 == 0)
            {
                if (t1 > 0)
                    return 90;
                if (t1 < 0)
                    return -90;
                if (t1 == 0)
                    return 0;
            }

            double grad = Math.Atan(Math.Abs(t1 / t2)) * 180 / Math.PI;

            if (t2 > 0)
            {
                if (t1 > 0)
                    return grad;
                if (t1 < 0)
                    return -grad;
                if (t1 == 0)
                    return 0;
            }
            if (t2 < 0)
            {
                if (t1 > 0)
                    return 180 - grad;
                if (t1 < 0)
                    return 180 + grad;
                if (t1 == 0)
                    return 180;
            }
            return 0;

        }
        #endregion

        #region Update Geometry for Duct Nest
        public bool UpdateGeomDuctNest(DuctNest dnest)
        { 
               try
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait Updating Geometry in process ... ");
                           
                m_gtapp.BeginProgressBar();
                m_gtapp.SetProgressBarRange(0, 100);
                m_gtapp.SetProgressBarPosition(5);
                m_oIGTTransactionManager.Begin("DuctNestUpdateGeom");
                m_gtapp.SetProgressBarPosition(10);

                short iFNO = dnest.FNO;
                int iFID = dnest.FID;
                short iCNO = 0;// GetCno(iFNO, from);
                int iCID = 0;
                int countgeom = 0; 
                IGTKeyObject oNewFeature = m_IGTDataContext.OpenFeature(iFNO, iFID);
                   m_gtapp.SetProgressBarPosition(20);
                   #region formation line
                   for (int i = 0; i < dnest.Form.Count; i++)
                   {
                       iCNO = dnest.Form[i].CNO;
                       iCID = dnest.Form[i].CID;
                       if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                       {
                           oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                           oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                           oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", iCID);

                       }
                       else
                       {
                           oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                           oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                           oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", iCID);
                       }
                       countgeom++;
                       dnest.Form[i].geom = (IGTPolylineGeometry) mobjEditServiceRotate.GetGeometry(countgeom);
                       oNewFeature.Components.GetComponent(iCNO).Geometry = mobjEditServiceRotate.GetGeometry(countgeom);
                   }
                   #endregion
                   m_gtapp.SetProgressBarPosition(30);
                   #region labels
                   for (int i = 0; i < dnest.Labels.Count; i++)
                   {
                       iCNO = dnest.Labels[i].CNO;
                       iCID = dnest.Labels[i].CID;
                       if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                       {
                           oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                           oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                           oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", iCID);

                       }
                       else
                       {
                           oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                           oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                           oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", iCID);
                       }

                       countgeom++;
                       dnest.Labels[i].geom = (IGTTextPointGeometry)mobjEditServiceRotate.GetGeometry(countgeom);
                       oNewFeature.Components.GetComponent(iCNO).Geometry = mobjEditServiceRotate.GetGeometry(countgeom);
                   }
                   #endregion
                   m_gtapp.SetProgressBarPosition(40);
                   #region ducts
                   for (int i = 0; i < dnest.Ducts.Count; i++)
                   {
                       iFNO = dnest.Ducts[i].FNO;
                       iFID = dnest.Ducts[i].FID;
                       IGTKeyObject oDuct = m_IGTDataContext.OpenFeature(iFNO, iFID);
                       for (int j = 0; j < dnest.Ducts[i].Form.Count; j++)
                       {
                           iCNO = dnest.Ducts[i].Form[j].CNO;
                           iCID = dnest.Ducts[i].Form[j].CID;
                           if (oDuct.Components.GetComponent(iCNO).Recordset.EOF)
                           {
                               oDuct.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                               oDuct.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                               oDuct.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", iCID);

                           }
                           else
                           {
                               oDuct.Components.GetComponent(iCNO).Recordset.MoveLast();
                               oDuct.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                               oDuct.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", iCID);
                           }

                           countgeom++;
                           dnest.Ducts[i].Form[j].geom = (IGTOrientedPointGeometry)mobjEditServiceRotate.GetGeometry(countgeom);
                           oDuct.Components.GetComponent(iCNO).Geometry = mobjEditServiceRotate.GetGeometry(countgeom);
                       }
                   }
                   #endregion

                   m_gtapp.SetProgressBarPosition(60);
                m_oIGTTransactionManager.Commit();
                m_gtapp.SetProgressBarPosition(70);
            
                m_oIGTTransactionManager.RefreshDatabaseChanges();
                m_gtapp.SetProgressBarPosition(100);

                m_gtapp.EndProgressBar();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
            }
            catch (Exception ex)
            {
                m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_gtapp.EndProgressBar();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                return false;
            }
            return true;
        }
        #endregion
    }
}
