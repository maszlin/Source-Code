using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;



namespace NEPS.GTechnology.NEPSDuctPathEdit
{
    class GTDuctPathEdit : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        public static Intergraph.GTechnology.API.IGTDataContext m_IGTDataContext = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTRelationshipService mobjRelationshipService = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditServiceRotate = null;
        IGTLocateService mobjLocateService = null;
        public Form_DuctPathEdit DPEditForm = null;
        public int Sect1ToInsert = 0;
        public int Sect2ToInsert = 0;
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

        public class DuctPathSect
        {
            public string NumDuctWaysSect;
            public string SectionLength;
            public int SectGraphicLength;
            public string SectDiam;
            public string YearExpanded;
            public string YearExtended;
            public string SectOwner;
            public string SectType;
            public string SectPlc;
            public string SectBillingRate;
            public string Encasement;
            public string SectBackFill;
            public string PUSect;
            public short CID;
        };

        public class SectSlash
        {
            public int length;
            public IGTOrientedPointGeometry Slash;
            public short CID;
        };

        public class SectLabelLeaderLine
        {
            public IGTTextPointGeometry Label;
            public string LabelText;
            public int LabelAlight;
            public IGTPolylineGeometry LeaderLine;
            public short CID;
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
            public List<SectSlash> SectSlashes = null;
            public List<SectLabelLeaderLine> SectLabels = null;
            public List<DuctPathSect> Sections = null;
            public List<DuctNest> DuctNestFrom;
            public List<DuctNest> DuctNestTo;
        };
        #endregion

        public class NewSection
        {
            public SectSlash StartSlash = null;
            public SectSlash EndSlash = null;
            public DuctPathSect Attr = null;
            public SectLabelLeaderLine Label = null;
        };

        public static NewSection DPNewSect = null; 
        public static int step = 0;
        public static DuctPath DuctPathOrigin = null;
        #endregion

        #region Mouse Click
        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            try
            {
                IGTPoint WorldPoint = e.WorldPoint;

                if (e.Button ==1)//left button
                {
                    #region Get Selected Duct Path for editing
                    if (step == 1)
                    {
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");

                        IGTDDCKeyObjects feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {
                            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                            {
                                if (oDDCKeyObject.FNO != 2200)
                                {
                                    MessageBox.Show("Please select a Duct Path!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                    return;
                                }
                                if (DuctPathOrigin == null)
                                {
                                    if (GetDuctPathOrigin(oDDCKeyObject.FNO, oDDCKeyObject.FID))
                                    {
                                        if (DuctPathOrigin == null)
                                        {
                                            MessageBox.Show("Only ASB and PPF features are allowed to be modified!\nPlease select one more time!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            m_gtapp.ActiveMapWindow.Activate();
                                            m_gtapp.SelectedObjects.Clear();
                                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                            return;
                                        }
                                        step = 0;
                                        if (DPEditForm == null)
                                        {
                                            DPEditForm = new Form_DuctPathEdit();
                                            DPEditForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
                                        }
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                                        DPEditForm.Show();
                                        return;
                                    }
                                    else
                                    {
                                        if (DuctPathOrigin != null)
                                            DuctPathOrigin = null;
                                        MessageBox.Show("Please select one more time!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        m_gtapp.ActiveMapWindow.Activate();
                                        m_gtapp.SelectedObjects.Clear();
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                        return;
                                    }
                                }


                            }
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");

                        }
                        else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                        {
                            MessageBox.Show("Please select only one Duct Path at once!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            m_gtapp.SelectedObjects.Clear();
                            m_gtapp.ActiveMapWindow.Activate();
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                            return;
                        }
                        return;
                    }
                    #endregion

                    #region Select first sect for new
                    if (step == 100)
                    {
                        if (!CheckPOintForSection(WorldPoint))
                        {
                            MessageBox.Show("Please select only Duct Path section line!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            m_gtapp.SelectedObjects.Clear();
                            return;
                        }
                        else
                        {
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select second Duct Path Section to insert new section in between two selected! Right Click to use only one section.");
                            m_gtapp.SelectedObjects.Clear();
                            step = 101;
                            return;
                        }
                    }
                    #endregion

                    #region Select second sect for new
                    if (step == 101)
                    {
                        if (!CheckPOintForSection(WorldPoint))
                        {
                            MessageBox.Show("Please select only Duct Path section line!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            m_gtapp.SelectedObjects.Clear();
                            return;
                        }
                        else
                        {

                            //m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select second Duct Path Section to insert new section in between two selected! Right Click to use only one section.");
                            m_gtapp.SelectedObjects.Clear();
                            if (mobjEditService.GeometryCount > 0)
                            {
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            }
                            mobjEditService.AddGeometry(DefaultPositonSlash(true), 24500);
                            step = 102;
                            return;
                        }
                    }
                    #endregion

                    #region sect slash start
                    if (step == 102)
                    {
                        if (Sect1ToInsert == 0)
                            Sect1ToInsert = 1;
                     //   step = 103;
                            return;
                    }
                    #endregion
                }
                else  if (e.Button == 2)//right click
                    {
                       
                        #region exiting from application
                        if (step == 1)//exiting from application
                        {
                            DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Duct Path Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (retVal == DialogResult.Yes)
                            {
                                ExitCmd();
                                return;
                            }
                            m_gtapp.ActiveMapWindow.Activate();
                            return;
                        }
                        #endregion

                        #region cancel Select first sect for new
                        if (step == 100)
                        {
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                            step = 0;
                            DPEditForm.Show();
                            return;
                        }
                        #endregion

                        #region cancel Select second sect for new
                        if (step == 101)
                        {
                          //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                            step = 102;
                            return;
                        }
                        #endregion

                        #region sect slash start, start moving along line
                        if (step == 102)
                        {
                            if (Sect1ToInsert == 0)
                                Sect1ToInsert = 1;
                            step = 103;
                            return;
                        }
                        #endregion
                        #region sect slash start , return from moving along line to default position
                        if (step == 103)
                        {
                            
                           if (mobjEditService.GeometryCount > 0)
                            {
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            }
                            mobjEditService.AddGeometry(DefaultPositonSlash(true), 24500);
                            step = 102;
                            return;
                        }
                    #endregion
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        } 
        #endregion

        #region MouseMove
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            try
            {

                #region Get Selected Duct Path for editing
                if (step == 1)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");

                    if (m_gtapp.SelectedObjects.FeatureCount == 1)
                    {
                        foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                        {
                            if (oDDCKeyObject.FNO != 2200)
                            {
                                MessageBox.Show("Please select a Duct Path!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                m_gtapp.ActiveMapWindow.Activate();
                                m_gtapp.SelectedObjects.Clear();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                return;
                            }
                            if (DuctPathOrigin == null)
                            {
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, loading Duct Path information");
                                if (GetDuctPathOrigin(oDDCKeyObject.FNO, oDDCKeyObject.FID))
                                {
                                    if (DuctPathOrigin == null)
                                    {
                                        MessageBox.Show("Only ASB and PPF features are allowed to be modified!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        m_gtapp.ActiveMapWindow.Activate();
                                        m_gtapp.SelectedObjects.Clear();
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                        return;
                                    }
                                    step = 0;
                                    if (DPEditForm == null)
                                    {
                                        DPEditForm = new Form_DuctPathEdit();
                                        DPEditForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
                                    }
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                                    DPEditForm.Show();
                                    return;
                                }
                                else
                                {
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                                    if (DuctPathOrigin != null)
                                        DuctPathOrigin = null;
                                    MessageBox.Show("Please select one more time!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                    return;
                                }
                            }


                        }
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");

                    }
                    else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                    {
                        MessageBox.Show("Please select only one Duct Path at once!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_gtapp.SelectedObjects.Clear();
                        m_gtapp.ActiveMapWindow.Activate();
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                        return;
                    }
                    return;
                }
                #endregion

                #region Select first sect for new
                if (step == 100)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path Section to insert new section! Right Click to cancel.");
                                        return;
                }
                #endregion

                #region Select second sect for new
                if (step == 101)
                {
                   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select 2nd Duct Path Section to insert new in between two! Right Click to use only one section(No 1st="+Sect1ToInsert.ToString()+")");
                  return;
                }
                #endregion

                #region sect slash start
                if (step == 102)
                {

                    if (Sect1ToInsert == 0)
                    {
                        Sect1ToInsert = 1;
                        if (mobjEditService.GeometryCount > 0)
                        {
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        mobjEditService.AddGeometry(DefaultPositonSlash(true), 24500);
                    }
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to accept default position of start-slash! Right Click to start move slash");
                    
                    return;
                }
                #endregion

                #region sect slash start moving along line
                if (step == 103)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to accept new position of start-slash! Right Click to set default position");
                    
                    IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        tempp.X = e.WorldPoint.X;
                        tempp.Y = e.WorldPoint.Y;
                        tempp.Z = 0.0;
                        temp.Origin = tempp;
                        IGTPoint projectPoint = PointOnConduit(tempp.X, tempp.Y, true);

                        if (projectPoint.X == 0 && projectPoint.Y == 0)
                        {
                            return;
                        }
                        else
                        {
                            temp.Origin = projectPoint;
                            IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                            temp.Orientation = Orientation.BuildVector(  projectPoint,tempp);
                            int length = int.Parse(projectPoint.Z.ToString());
                            if (Sect1ToInsert == 1 && Sect2ToInsert == 0)//if only  one section and fisrt
                            {
                                if (length < 0 || length >= int.Parse(DuctPathOrigin.Sections[Sect1ToInsert-1].SectionLength) - 1)
                                    return;
                            }
                            if (Sect1ToInsert == 1 && Sect2ToInsert > 0)//if two secto start with first 
                            {
                                if (length < 0 || length >= DuctPathOrigin.SectSlashes[Sect2ToInsert-1].length - 1)
                                    return;
                            }
                            if (Sect1ToInsert > 1 && Sect2ToInsert == 0)//if only  one section
                            {
                                if (length < DuctPathOrigin.SectSlashes[Sect1ToInsert - 2].length)
                                    return;
                                if ((Sect1ToInsert - 1) >= DuctPathOrigin.SectSlashes.Count)
                                {
                                    if (length >= DuctPathOrigin.Length - 1)
                                        return;
                                }
                                else if (length >= DuctPathOrigin.SectSlashes[Sect1ToInsert - 1].length - 1)
                                    return;

                            }

                            if (Sect1ToInsert > 1 && Sect2ToInsert > 0)//if 2 section
                            {
                                if (length < DuctPathOrigin.SectSlashes[Sect1ToInsert - 2].length)
                                    return;
                                if ((Sect2ToInsert - 1) >= DuctPathOrigin.SectSlashes.Count)
                                {
                                    if (length >= DuctPathOrigin.Length - 1)
                                        return;
                                }
                                else if (length >= DuctPathOrigin.SectSlashes[Sect2ToInsert - 1].length - 1)
                                    return;

                            }
                            

                            if (mobjEditService.GeometryCount > 0)
                            {
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            }
                            mobjEditService.AddGeometry(temp, 24500);
                            return;
                        }
                }
                #endregion

                #region sect slash end moving along line
                if (step == 104)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to accept new position of end-slash!");

                    IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;
                    temp.Origin = tempp;
                    IGTPoint projectPoint = PointOnConduit(tempp.X, tempp.Y, true);

                    if (projectPoint.X == 0 && projectPoint.Y == 0)
                    {
                        return;
                    }
                    else
                    {
                        temp.Origin = projectPoint;
                        IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                        temp.Orientation = Orientation.BuildVector(tempp, projectPoint);
                        int length = int.Parse(projectPoint.Z.ToString());
                        if (Sect1ToInsert == 1)
                            if (length < DPNewSect.StartSlash.length)
                                return;
                        if (Sect1ToInsert == 1 && Sect2ToInsert == 0)//if only  one section and fisrt
                        {
                            if (length < 0 || length >= int.Parse(DuctPathOrigin.Sections[Sect1ToInsert - 1].SectionLength) - 1)
                                return;
                        }
                        if (Sect1ToInsert == 1 && Sect2ToInsert > 0)//if two secto start with first 
                        {
                            if (length < 0 || length >= DuctPathOrigin.SectSlashes[Sect2ToInsert - 1].length - 1)
                                return;
                        }
                        if (Sect1ToInsert > 1 && Sect2ToInsert == 0)//if only  one section
                        {
                            if (length < DuctPathOrigin.SectSlashes[Sect1ToInsert - 2].length)
                                return;
                            if ((Sect1ToInsert - 1) >= DuctPathOrigin.SectSlashes.Count)
                            {
                                if (length >= DuctPathOrigin.Length - 1)
                                    return;
                            }
                            else if (length >= DuctPathOrigin.SectSlashes[Sect1ToInsert - 1].length - 1)
                                return;

                        }

                        if (Sect1ToInsert > 1 && Sect2ToInsert > 0)//if only  one section
                        {
                            if (length < DuctPathOrigin.SectSlashes[Sect1ToInsert - 2].length)
                                return;
                            if ((Sect2ToInsert - 1) >= DuctPathOrigin.SectSlashes.Count)
                            {
                                if (length >= DuctPathOrigin.Length - 1)
                                    return;
                            }
                            else if (length >= DuctPathOrigin.SectSlashes[Sect2ToInsert - 1].length - 1)
                                return;

                        }


                        if (mobjEditService.GeometryCount > 1)
                        {
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        mobjEditService.AddGeometry(temp, 24500);
                        return;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }
        #endregion

        #region Mouse Double Click
        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            #region sect slash start
            if (step == 102 || step == 103)
            {
                if(DPNewSect==null)
                    DPNewSect = new NewSection();
                //DPNewSect.StartSlash.CID = Sect1ToInsert + 1;
                if (DPNewSect.StartSlash == null)
                    DPNewSect.StartSlash = new SectSlash();
            //    DPNewSect.StartSlash.Slash = GTClassFactory.Create<IGTOrientedPointGeometry>();
                DPNewSect.StartSlash.Slash = (IGTOrientedPointGeometry)mobjEditService.GetGeometry(1);
                DPNewSect.StartSlash.length = (int)DPNewSect.StartSlash.Slash.Origin.Z;

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to accept new position of end-slash! ");
                step = 104;
                return;
            }
           
            #endregion

            #region sect slash end
            if (step == 104)
            {
                if(DPNewSect==null)
                    DPNewSect = new NewSection();
               // DPNewSect.EndSlash.CID = Sect1ToInsert + 1;
                if (DPNewSect.EndSlash == null)
                    DPNewSect.EndSlash = new SectSlash();
                DPNewSect.EndSlash.Slash = (IGTOrientedPointGeometry)mobjEditService.GetGeometry(2);
                DPNewSect.EndSlash.length = (int)DPNewSect.EndSlash.Slash.Origin.Z;
                step = 0;
                DPEditForm.Show();
                return;
            }
            #endregion
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
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Duct Path Edit . . . ");
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
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
             }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();                
            }
        }
      

        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Duct Path Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);
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
            m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
          //  m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
         //   m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }
        #endregion
        
        #region Exit CustomCommand
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitCmd();
        }
            
        public void ExitCmd()
        {
        //    m_gtapp.SetProgressBarRange(0, 0);
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting..."); 
            m_gtapp.SelectedObjects.Clear();
            UnsubscribeEvents();
            step = 0;
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
            }
            if (mobjLocateService != null)
                mobjLocateService = null;
            if (mobjRelationshipService != null)
            {
                mobjRelationshipService = null;
            }
            if(DuctPathOrigin.Sections!=null)
                DuctPathOrigin.Sections.Clear();
            if (DuctPathOrigin.SectLabels != null)
                DuctPathOrigin.SectLabels.Clear();
            if (DuctPathOrigin.SectSlashes != null)
                DuctPathOrigin.SectSlashes.Clear();
            DuctPathOrigin = null;
            if (DPNewSect != null)
                DPNewSect = null;

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
                if (DuctPathOrigin.Feature_state != "ASB" && DuctPathOrigin.Feature_state != "PPF")
                {
                    DuctPathOrigin = null;
                    return true;
                }
                #endregion
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
                #region sections attr
                if (!oDuctPathFeature.Components.GetComponent(2202).Recordset.EOF)
                {
                    DuctPathOrigin.Sections = new List<DuctPathSect>();
                    oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveFirst();
                    for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2202).Recordset.RecordCount; j++)
                    {

                        DuctPathSect secttemp = new DuctPathSect();
                        for (int k = 0; k < oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields.Count; k++)
                        {
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "G3E_CID")
                            {
                                secttemp.CID = short.Parse(oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString());
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_WAYS")
                            {
                                secttemp.NumDuctWaysSect = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_LENGTH")
                            {
                                secttemp.SectionLength = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_DIAMETER")
                            {
                                secttemp.SectDiam = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "COMMON_TRENCH")
                            {
                                secttemp.SectOwner = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_PLACMNT")
                            {
                                secttemp.SectPlc = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_TYPE")
                            {
                                secttemp.SectType = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "BILLING_RATE")
                            {
                                secttemp.SectBillingRate = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_ENCASE")
                            {
                                secttemp.Encasement = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_BACKFILL")
                            {
                                secttemp.SectBackFill = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "MIN_MATERIAL")
                            {
                                secttemp.PUSect = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "YEAR_EXTENDED")
                            {
                                
                                    string tt = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                                    if (tt == "")
                                        secttemp.YearExtended = "0";
                                    else
                                        secttemp.YearExtended = tt.Substring(tt.Length - 4);
                                
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "YEAR_EXPANDED")
                            {
                                
                                    string tt = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                                    if(tt=="")
                                        secttemp.YearExpanded = "0";
                                    else
                                    secttemp.YearExpanded = tt.Substring(tt.Length - 4);
                              
                            }
                            
                        }

                       // secttemp.PUSect = "0";
                        DuctPathOrigin.Sections.Add(secttemp);
                        oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveNext();
                    }

                }
                #endregion
                #region sections labels
                if (!oDuctPathFeature.Components.GetComponent(2230).Recordset.EOF)
                {
                    DuctPathOrigin.SectLabels = new List<SectLabelLeaderLine>();
                    oDuctPathFeature.Components.GetComponent(2230).Recordset.MoveFirst();
                    for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2230).Recordset.RecordCount; j++)
                    {
                        SectLabelLeaderLine labelsect=new SectLabelLeaderLine();
                                    IGTOrientedPointGeometry tempcomp = (IGTOrientedPointGeometry)oDuctPathFeature.Components.GetComponent(2230).Geometry;
                                    TextPointGeom ttemp = new TextPointGeom();
                                    ttemp.CNO = 2230;

                                    IGTTextPointGeometry templabel = GTClassFactory.Create<IGTTextPointGeometry>();
                                    templabel.Origin = tempcomp.Origin;
                                    templabel.Rotation = AngleBtwPoint(tempcomp.Orientation.I, tempcomp.Orientation.J);
                                    for (int k = 0; k < oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields.Count; k++)
                                    {
                                        if (oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Name == "G3E_TEXT")
                                        {
                                            labelsect.LabelText = oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Value.ToString();
                                        }
                                        if (oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Name == "G3E_ALIGNMENT")
                                        {//(GTAlignmentConstants)
                                            labelsect.LabelAlight = int.Parse(oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Value.ToString());
                                        }
                                        if (oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Name == "G3E_CID")
                                        {
                                            labelsect.CID = short.Parse(oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Value.ToString());
                                        }
                                    }

                                    labelsect.Label = templabel;                                    
                                    DuctPathOrigin.SectLabels.Add(labelsect);
                                    oDuctPathFeature.Components.GetComponent(2230).Recordset.MoveNext();
                    }
                    if (!oDuctPathFeature.Components.GetComponent(2212).Recordset.EOF)
                    {
                        oDuctPathFeature.Components.GetComponent(2212).Recordset.MoveFirst();
                        for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2212).Recordset.RecordCount; j++)
                        {
                            int cid = 0;
                            for (int k = 0; k < oDuctPathFeature.Components.GetComponent(2212).Recordset.Fields.Count; k++)
                            {
                                if (oDuctPathFeature.Components.GetComponent(2212).Recordset.Fields[k].Name == "G3E_CID")
                                {
                                    cid = short.Parse(oDuctPathFeature.Components.GetComponent(2212).Recordset.Fields[k].Value.ToString());
                                }
                            }
                            for (int k = 0; k < DuctPathOrigin.SectLabels.Count; k++)
                            {
                                if (DuctPathOrigin.SectLabels[k].CID == cid)
                                {
                                    if (oDuctPathFeature.Components.GetComponent(2212).Geometry != null)
                                    {
                                        IGTCompositePolylineGeometry tempcomp = (IGTCompositePolylineGeometry)oDuctPathFeature.Components.GetComponent(2212).Geometry;
                                        DuctPathOrigin.SectLabels[k].LeaderLine = (IGTPolylineGeometry)(tempcomp.ExtractGeometry(tempcomp.FirstPoint, tempcomp.LastPoint, false));

                                    }
                                    break;
                                }
                            }
                            oDuctPathFeature.Components.GetComponent(2212).Recordset.MoveNext();
                        }
                    }
                }
                 #endregion
                #region section slashes
                if (!oDuctPathFeature.Components.GetComponent(2220).Recordset.EOF)
                {
                    DuctPathOrigin.SectSlashes = new List<SectSlash>();
                    oDuctPathFeature.Components.GetComponent(2220).Recordset.MoveFirst();
                    for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2220).Recordset.RecordCount; j++)
                    {
                        SectSlash tempslash = new SectSlash();
                        for (int k = 0; k < oDuctPathFeature.Components.GetComponent(2220).Recordset.Fields.Count; k++)
                        {
                            if (oDuctPathFeature.Components.GetComponent(2220).Recordset.Fields[k].Name == "G3E_CID")
                            {
                                tempslash.CID = short.Parse(oDuctPathFeature.Components.GetComponent(2220).Recordset.Fields[k].Value.ToString());
                            }
                        }
                        tempslash.Slash = (IGTOrientedPointGeometry)(oDuctPathFeature.Components.GetComponent(2220).Geometry);
                        DuctPathOrigin.SectSlashes.Add(tempslash);
                        oDuctPathFeature.Components.GetComponent(2220).Recordset.MoveNext();
                    }

                    for (int k = 0; k < DuctPathOrigin.SectSlashes.Count; k++)
                    {
                        DuctPathOrigin.SectSlashes[k].length = 0;
                        for (int j = 0; j < DuctPathOrigin.Sections.Count; j++)
                        {
                            if (DuctPathOrigin.SectSlashes[k].CID >= DuctPathOrigin.Sections[j].CID)
                            {
                                DuctPathOrigin.SectSlashes[k].length += int.Parse(DuctPathOrigin.Sections[j].SectionLength);
                            }
                        }
                    }

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
                                            ttemp.CID = int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
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
                            if (DuctPathOrigin.DuctNestFrom == null)
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
            if (FNO == 2100)
                return 2120002;
            if (FNO == 2300)
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

        #region Check if point belong duct path and wich section
        public bool CheckPOintForSection(IGTPoint pointTocheck)
        {
      //     public IGTPoint PointOnConduit(double Xslash, double Yslash, bool conf)
       // {
            double Xslash = pointTocheck.X;
            double Yslash = pointTocheck.Y;
            IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
            projectPoint.X = 0;
            projectPoint.Y = 0;
            projectPoint.Z = 0;
            List<IGTPoint> ProjectedPoints = new List<IGTPoint>();
            for (int i = 0; i < DuctPathOrigin.DuctPathLineGeom.Points.Count - 1; i++)
            {
                    ProjectedPoints.Add(ProjectedPointOnConduit(DuctPathOrigin.DuctPathLineGeom.Points[i].X,
                    DuctPathOrigin.DuctPathLineGeom.Points[i].Y,
                    DuctPathOrigin.DuctPathLineGeom.Points[i + 1].X,
                    DuctPathOrigin.DuctPathLineGeom.Points[i + 1].Y,
                    Xslash, Yslash));
            }
            double min = 0;
            int sectnum = 0;
            for (int i = 0; i < ProjectedPoints.Count; i++)
            {
                double difX = ProjectedPoints[i].X - Xslash;
                double difY = ProjectedPoints[i].Y - Yslash;

                if (difX < 0) difX *= -1;
                if (difY < 0) difY *= -1;
                double disMin = difY + difX;
                if (min == 0) min = disMin+1;
                if (min > disMin)
                {
                    projectPoint.X = ProjectedPoints[i].X;
                    projectPoint.Y = ProjectedPoints[i].Y;
                    projectPoint.Z = 0;
                    min = disMin;
                    sectnum = i+1;
                }

            }
            //checking if mouse click too far from conduit
            if (min > 3)
            {
                projectPoint.X = 0;
                projectPoint.Y = 0;
                projectPoint.Z = 0;
                sectnum = 0;
            //    MessageBox.Show("Second selected section must be different from first(No=" + Sect1ToInsert.ToString() + ")", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            //if point on conduit confirm
           
                if (sectnum > 0)
                {

                    int length = 0;
                    for (int i = 0; i < DuctPathOrigin.DuctPathLineGeom.Points.Count - 1; i++)
                    {
                        if (sectnum == i + 1)
                        {
                            length += LegthBtwTwoPoints(DuctPathOrigin.DuctPathLineGeom.Points[i].X,
                            DuctPathOrigin.DuctPathLineGeom.Points[i].Y, Xslash, Yslash);
                            break;
                        }
                        else
                            length += LegthBtwTwoPoints(DuctPathOrigin.DuctPathLineGeom.Points[i].X,
                                DuctPathOrigin.DuctPathLineGeom.Points[i].Y,
                                DuctPathOrigin.DuctPathLineGeom.Points[i + 1].X,
                                DuctPathOrigin.DuctPathLineGeom.Points[i + 1].Y);
                    }

                    bool first = true;
                    if (step==100)
                    {
                        first = true;
                        Sect1ToInsert = 1;
                    }
                    else if(step==101)
                    {
                        first = false;
                        Sect2ToInsert = 1;
                    }

                    if (DuctPathOrigin.SectSlashes != null)
                    {
                        for (int i = 0; i < DuctPathOrigin.SectSlashes.Count; i++)
                        {
                            if (i == 0 && length < DuctPathOrigin.SectSlashes[i].length)
                            {
                                if (first) Sect1ToInsert = 1;
                                else
                                {
                                    Sect2ToInsert = 1;
                                    if (Sect1ToInsert == Sect2ToInsert)
                                    {
                                        MessageBox.Show("Second selected section must be different from first(No=" + Sect1ToInsert.ToString()+")", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return false;
                                    }
                                    if (((Sect2ToInsert - Sect1ToInsert) > 0 && (Sect2ToInsert - Sect1ToInsert) > 1)
                                               || ((Sect2ToInsert - Sect1ToInsert) < 0 && (Sect1ToInsert - Sect2ToInsert) > 1))
                                    {
                                        MessageBox.Show("Second selected section must be neighbor for first(No=" + Sect1ToInsert.ToString() + ")", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return false;
                                    }

                                    if ((Sect2ToInsert - Sect1ToInsert) < 0)
                                    {
                                        int temp = Sect2ToInsert;
                                        Sect2ToInsert = Sect1ToInsert;
                                        Sect1ToInsert = temp;
                                    }
                                }
                            }

                            if (length > DuctPathOrigin.SectSlashes[i].length)
                            {
                                if ((i + 1) == DuctPathOrigin.SectSlashes.Count)
                                {
                                    if (first) Sect1ToInsert = i + 2;
                                    else
                                    {
                                        Sect2ToInsert = i + 2;
                                        if (Sect1ToInsert == Sect2ToInsert)
                                        {
                                            MessageBox.Show("Second selected section must be different from first(No=" + Sect1ToInsert.ToString() + ")", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return false;
                                        }
                                        if (((Sect2ToInsert - Sect1ToInsert) > 0 && (Sect2ToInsert - Sect1ToInsert) > 1)
                                               || ((Sect2ToInsert - Sect1ToInsert) < 0 && (Sect1ToInsert - Sect2ToInsert) > 1))
                                        {
                                            MessageBox.Show("Second selected section must be neighbor for first(No=" + Sect1ToInsert.ToString() + ")", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return false;
                                        }

                                        if ((Sect2ToInsert - Sect1ToInsert) < 0)
                                        {
                                            int temp = Sect2ToInsert;
                                            Sect2ToInsert = Sect1ToInsert;
                                            Sect1ToInsert = temp;
                                        }
                                    }
                                    return true;
                                }

                                if((i+1)<DuctPathOrigin.SectSlashes.Count)
                                    if (length < DuctPathOrigin.SectSlashes[i+1].length)
                                    {
                                        if (first) Sect1ToInsert = i + 2;
                                        else 
                                        {
                                            Sect2ToInsert = i + 2;
                                            if (Sect1ToInsert == Sect2ToInsert)
                                            {
                                                MessageBox.Show("Second selected section must be different from first(No=" + Sect1ToInsert.ToString() + ")", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                return false;
                                            }

                                            if (((Sect2ToInsert - Sect1ToInsert) > 0 && (Sect2ToInsert - Sect1ToInsert) > 1)
                                                || ((Sect2ToInsert - Sect1ToInsert) < 0 && ( Sect1ToInsert-Sect2ToInsert) > 1))
                                            {
                                                MessageBox.Show("Second selected section must be neighbor for first(No=" + Sect1ToInsert.ToString() + ")", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                return false;
                                            }

                                            if ((Sect2ToInsert - Sect1ToInsert) < 0)
                                            {
                                                int temp = Sect2ToInsert;
                                                Sect2ToInsert = Sect1ToInsert;
                                                Sect1ToInsert = temp;
                                            }
                                            
                                        }
                                        return true;
                                    }

                            }
                        }
                    }

                }
                    
            return true;
        }

        #region Coordinate for Projected Point on SEGMENT of Conduit line
        public IGTPoint ProjectedPointOnConduit(double stX, double stY, double endX, double endY, double slashX, double slashY)
        {
            IGTSegment sectT = GTClassFactory.Create<IGTSegment>();
            IGTSegmentPoint slashT = GTClassFactory.Create<IGTSegmentPoint>();
            IGTPoint clickT = GTClassFactory.Create<IGTPoint>();
            IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
            sectT.Point1.X = stX;
            sectT.Point1.Y = stY;
            sectT.Point1.Z = 0;
            sectT.Point2.X = endX;
            sectT.Point2.Y = endY;
            sectT.Point2.Z = 0;

            clickT.X = slashX;
            clickT.Y = slashY;
            clickT.Z = 0;

            slashT = clickT.ProjectTo(sectT);

            projectPoint.X = slashT.Point.X;
            projectPoint.Y = slashT.Point.Y;
            projectPoint.Z = 0;

            return projectPoint;
        }
        #endregion

        #region Between Two points on sumple line
        private int LegthBtwTwoPoints(double startPointX, double startPointY, double endPointX, double endPointY)
        {
            return Convert.ToInt32(Math.Round(Math.Sqrt(Math.Pow((endPointX - startPointX), 2) + Math.Pow((endPointY - startPointY), 2)), 0));
        }
        #endregion
        #endregion

        #region Coordinate for Projected Point on Conduit line
        public IGTPoint PointOnConduit(double Xslash, double Yslash, bool conf)
        {

            IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
            projectPoint.X = 0;
            projectPoint.Y = 0;
            projectPoint.Z = 0;
            List<IGTPoint> ProjectedPoints = new List<IGTPoint>();
            for (int i = 0; i < DuctPathOrigin.DuctPathLineGeom.Points.Count - 1; i++)
            {
                ProjectedPoints.Add(ProjectedPointOnConduit(DuctPathOrigin.DuctPathLineGeom.Points[i].X,
                DuctPathOrigin.DuctPathLineGeom.Points[i].Y,
                DuctPathOrigin.DuctPathLineGeom.Points[i + 1].X,
                DuctPathOrigin.DuctPathLineGeom.Points[i + 1].Y,
                Xslash, Yslash));
            }
            double min = 0;
            int sectnum = 0;
            for (int i = 0; i < ProjectedPoints.Count; i++)
            {
                double difX = ProjectedPoints[i].X - Xslash;
                double difY = ProjectedPoints[i].Y - Yslash;

                if (difX < 0) difX *= -1;
                if (difY < 0) difY *= -1;
                double disMin = difY + difX;
                if (min == 0) min = disMin + 1;
                if (min > disMin)
                {
                    projectPoint.X = ProjectedPoints[i].X;
                    projectPoint.Y = ProjectedPoints[i].Y;
                    projectPoint.Z = 0;
                    min = disMin;
                    sectnum = i + 1;
                }

            }
            //checking if mouse click too far from conduit
            if (min > 3)
            {
                projectPoint.X = 0;
                projectPoint.Y = 0;
                projectPoint.Z = 0;
                sectnum = 0;
            }

            //if point on conduit confirm
            if (conf)
            {
                if (sectnum > 0)
                {

                    int length = 0;
                    for (int i = 0; i < DuctPathOrigin.DuctPathLineGeom.Points.Count - 1; i++)
                    {
                        if (sectnum == i + 1)
                        {
                            length += LegthBtwTwoPoints(DuctPathOrigin.DuctPathLineGeom.Points[i].X,
                            DuctPathOrigin.DuctPathLineGeom.Points[i].Y, Xslash, Yslash);
                            break;
                        }
                        else
                            length += LegthBtwTwoPoints(DuctPathOrigin.DuctPathLineGeom.Points[i].X,
                                DuctPathOrigin.DuctPathLineGeom.Points[i].Y,
                                DuctPathOrigin.DuctPathLineGeom.Points[i + 1].X,
                                DuctPathOrigin.DuctPathLineGeom.Points[i + 1].Y);
                    }
                    projectPoint.Z = double.Parse(length.ToString());
                }
            }
            return projectPoint;

        }
        #endregion

        #region default position for slash
        public IGTOrientedPointGeometry DefaultPositonSlash(bool start)
        {
            IGTOrientedPointGeometry slash = GTClassFactory.Create<IGTOrientedPointGeometry>();
            if (start)
            {
                if (Sect1ToInsert == 1)
                {
                    IGTPoint temp = DuctPathOrigin.DuctPathLineGeom.FirstPoint;
                    slash.Origin = temp;
                    return slash;
                }
               
                if (Sect1ToInsert > 1 )
                {
                    return DuctPathOrigin.SectSlashes[Sect1ToInsert - 2].Slash;
                }
            }
            else
            {
                if (Sect1ToInsert == 1 && Sect2ToInsert == 0)//if only  one section and fisrt
                {
                    IGTPoint temp = DuctPathOrigin.DuctPathLineGeom.LastPoint;
                    slash.Origin = temp;
                    return slash;
                }
                if (Sect1ToInsert == 1 && Sect2ToInsert > 0)//if two secto start with first 
                {
                    return DuctPathOrigin.SectSlashes[Sect2ToInsert - 1].Slash;
                      
                }
                if (Sect1ToInsert > 1 && Sect2ToInsert == 0)//if only  one section
                {

                    if ((Sect1ToInsert - 1) >= DuctPathOrigin.SectSlashes.Count)
                    {
                        IGTPoint temp = DuctPathOrigin.DuctPathLineGeom.LastPoint;
                        slash.Origin = temp;
                        return slash;
                    }
                    else return DuctPathOrigin.SectSlashes[Sect1ToInsert - 1].Slash;

                }

                if (Sect1ToInsert > 1 && Sect2ToInsert > 0)//if only  one section
                {
                    if ((Sect2ToInsert - 1) >= DuctPathOrigin.SectSlashes.Count)
                    {
                        IGTPoint temp = DuctPathOrigin.DuctPathLineGeom.LastPoint;
                        slash.Origin = temp;
                        return slash;
                    }
                    else return DuctPathOrigin.SectSlashes[Sect2ToInsert - 1].Slash;

                }
            }

            return slash;
        }
        #endregion
    }
}
