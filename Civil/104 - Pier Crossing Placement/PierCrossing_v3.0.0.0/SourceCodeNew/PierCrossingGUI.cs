using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ADODB;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NEPS.GTechnology.PierCrossing
{
    public partial class frmPierCrossing : Form
    {
        IGTKeyObject mobjPierCrossAttribute = null;

        private IGTPoints LeftPoints;
        private IGTPoints RightPoints;
        public IGTPoint _SelGeom = null;
        public Boolean selected = false;

        public static IGTPoints RefPoints;
        public static IGTPoints RotationPoints;
        public static IGTGeometryCreationService gGeomCS;
        public static IGTGeometryEditService gGeomES;

        public static int newFID; //    to be used for rotation in "Add" button click event
        public static short newFNO; //    to be used for rotation in "Add" button click event
        public static int selectedFID; // to keep the FNO of selected feature in rotation mode
        public static short selectedFNO; // to keep the FID of selected feature in rotation mode


        private const short PierCrossingFNO = 3600; // FNO stands for Feature Number
        private const short PierCrossingAttribCNO = 3601; //CNo stands for Component Number
        private const short PierCrossingPriGeoCNO = 3620; //
        /*
         *  Modified by : Aini 
         *  Date        : 2/Feb/2012
         *  Remark      : Change the vPXWidth from 5.5 to 0.5 to reduce the Pier Cross hand size, distance between left and right line. 
         *                Code is updated from v1.0.5.0 to v1.0.5.1
         *  Reviewed by : Mike & Anna
         */
        private const double vPXWidth = 0.5;
       
        public static int addMode = 0;
        public static int deleteMode = 0;
        public static int deleteFID = 0;
        public static int moveMode = 0;
        public static int moveFID = 0;
        public static int rotateMode = 0;
        public static int rotateOrigin = 0;
        public static int rotateFID = 0;
        public static int gPXIdx = 0;
         
        
        public frmPierCrossing()
        {
            try
            {
                InitializeComponent();
              //  GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Pier Crossing ...");

                RefPoints = GTClassFactory.Create<IGTPoints>();
                RotationPoints = GTClassFactory.Create<IGTPoints>();
                LeftPoints = GTClassFactory.Create<IGTPoints>();
                RightPoints = GTClassFactory.Create<IGTPoints>();
                gGeomES = GTClassFactory.Create<IGTGeometryEditService>();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }

        private void Frm_PierCrossing_Load(object sender, EventArgs e)
        {
            this.DesktopLocation = new Point(24, 100);
            GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Pier Crossing ...");
        }

        private void DisableAllButtons()
        {
            btnAdd.Enabled = false;
            btnMove.Enabled = false;
            btnRotate.Enabled = false;
            btnDelete.Enabled = false;
            btnClose.Enabled = false;
        }

        private void EnableAllButtons()
        {
            btnAdd.Enabled = true;
            btnMove.Enabled = true;
            btnRotate.Enabled = true;
            btnDelete.Enabled = true;
            btnClose.Enabled = true;
        }

        public void clearall()
        {
            RefPoints.Clear();
            LeftPoints.Clear();
            RightPoints.Clear();
            RotationPoints.Clear();
            gGeomCS.RemoveAllGeometries();
            gGeomES.TargetMapWindow = GTPierCrossing.application.ActiveMapWindow;
            gGeomES.RemoveAllGeometries();
            _SelGeom = null;
        }

        public void cleanup()
        {
            RefPoints = null;
            LeftPoints = null;
            RightPoints = null;
            RotationPoints = null;
            gGeomCS = null;
            gGeomES = null;
            _SelGeom = null;
        }
        
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTClassFactory.Create<IGTDataContext>().OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public void getPreviousSelectedFeature(int previousFID, short previousFNO)
        {
            IGTDDCKeyObjects oGTKeyObjs;

            GTPierCrossing.application.SelectedObjects.Clear();
            oGTKeyObjs = (IGTDDCKeyObjects)GTPierCrossing.application.DataContext.GetDDCKeyObjects(previousFNO, previousFID, GTComponentGeometryConstants.gtddcgAllGeographic);
            for (int K = 0; K < oGTKeyObjs.Count; K++)
                GTPierCrossing.application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[K]);

            GTPierCrossing.application.RefreshWindows();

        }

        #region Pier Cross Placement and Cancellation

        public Boolean _placePX
        {
            set
            {
                PlacePierCrossing();
            }
        }

        
        public void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                HidePierForm();
                RefPoints.Clear();
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
                GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter attributes for Pier Crossing");
                GTPierCrossing.m_oIGTTransactionManager.Begin("PierCrossingAttributes");
                mobjPierCrossAttribute = GTPierCrossing.application.DataContext.NewFeature(PierCrossingFNO, true);
       
                GTPierCrossing.mobjExplorerService.ExploreFeature(mobjPierCrossAttribute, "Pier Cross");// must be the same with G3E_DNO in G3E_DIALOG
                GTPierCrossing.mobjExplorerService.Visible = true;
                GTPierCrossing.mobjExplorerService.Slide(true);

              //  addMode = 1;
               // GTPierCrossing.m_oIGTTransactionManager.Commit();
               // GTPierCrossing.m_oIGTTransactionManager.RefreshDatabaseChanges();

               // GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;

            }
            catch (Exception ex)
            {

                GTPierCrossing.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }

        // function to draw the pier crossing image on the map
        public void PlacePierCrossing()
        {
            try
            {
                addMode = 0;

                if (RefPoints.Count == 2)
                {
                    GTPierCrossing.application.BeginWaitCursor();
                    GTPierCrossing.application.SetProgressBarRange(0, 100);
                    GTPierCrossing.application.BeginProgressBar();
                    GTPierCrossing.m_oIGTTransactionManager.Begin("PlacePierCrossing");
                    Calc_LPXCoord(RefPoints, vPXWidth);
                    if (LeftPoints.Count > 0)
                    {
                        Calc_RPXCoord(RefPoints, vPXWidth);
                        GTPierCrossing.application.SetProgressBarPosition(25);
                        if (RightPoints.Count > 0)
                        {
                            Insert_CompositePolyline(LeftPoints, RightPoints);
                            GTPierCrossing.m_oIGTTransactionManager.Commit();
                            GTPierCrossing.m_oIGTTransactionManager.RefreshDatabaseChanges();
                            GTPierCrossing.application.SetProgressBarPosition(100);
                            GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Pier Crossing Geo Line is created");

                            //select the feature straightaway for rotation
                            getPreviousSelectedFeature(newFID, newFNO);
                            if (gGeomCS != null)
                            {
                                if (gGeomCS.GeometryCount != 0)
                                    gGeomCS.RemoveAllGeometries();
                            }                           
                            GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                            GTPierCrossing.application.EndProgressBar();
                            GTPierCrossing.application.EndWaitCursor();
                            rotateMode = 4; // to initiate rotation when moving the pointer around
                            
                        }
                        else
                        {
                            GTPierCrossing.m_oIGTTransactionManager.Rollback();
                            MessageBox.Show("Error generating coordinate information.\r\n" +
                                            "Unable to place Pier Crossing feature.", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            if (gGeomCS != null)
                            {
                                if (gGeomCS.GeometryCount != 0)
                                    gGeomCS.RemoveAllGeometries();
                            }
                        }
                    }
                    else
                    {
                        GTPierCrossing.m_oIGTTransactionManager.Rollback();
                        MessageBox.Show("Error generating coordinate information.\r\n" +
                                        "Unable to place Pier Crossing feature.", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (gGeomCS != null)
                        {
                            if (gGeomCS.GeometryCount != 0)
                                gGeomCS.RemoveAllGeometries();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error capturing input coordinates.\r\n" +
                                    "Unable to place Pier Crossing feature.", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (gGeomCS != null)
                    {
                        if (gGeomCS.GeometryCount != 0)
                            gGeomCS.RemoveAllGeometries();
                    }
                }
            }
            catch (Exception ex)
            {
                GTPierCrossing.m_oIGTTransactionManager.Rollback();
                addMode = 0;
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
                ShowPierForm();
            }
        }

        public static void CopyComponentAttribute(IGTKeyObject fromFeature, IGTKeyObject toFeature)
        {
            int lFID = toFeature.FID;
            short iFNO = toFeature.FNO;

            for (int i = 0; i < fromFeature.Components.Count; i++)
            {
                short iCNO = fromFeature.Components[i].CNO;
                if ((iCNO == 3601) || (iCNO == 51))
                {
                    if (!fromFeature.Components[i].Recordset.EOF)
                    {
                        if (toFeature.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            toFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                            toFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        }
                        else
                        {
                            toFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                        }

                        for (int j = 0; j < fromFeature.Components[i].Recordset.Fields.Count; j++)
                        {
                            if ((fromFeature.Components[i].Recordset.Fields[j].Name != "G3E_FID") &&
                                (fromFeature.Components[i].Recordset.Fields[j].Name != "G3E_FNO") &&
                                (fromFeature.Components[i].Recordset.Fields[j].Name != "G3E_CNO") &&
                                (fromFeature.Components[i].Recordset.Fields[j].Name != "G3E_CID") &&
                                (fromFeature.Components[i].Recordset.Fields[j].Name != "G3E_ID")
                            )
                                toFeature.Components.GetComponent(iCNO).Recordset.Update(fromFeature.Components[i].Recordset.Fields[j].Name, fromFeature.Components[i].Recordset.Fields[j].Value);
                        }
                    }
                }
            }
        }

        private void Insert_CompositePolyline(IGTPoints NewLeftPoints, IGTPoints NewRightPoints)
        {
            try
            {
                short iFNO = 0;
                short iCNO = 0;
                int iFID = 0;

                IGTKeyObject oNewFeature;
                IGTPolylineGeometry oLeftPolyline = GTClassFactory.Create<IGTPolylineGeometry>();
                IGTPolylineGeometry oRightPolyline = GTClassFactory.Create<IGTPolylineGeometry>();

                iFNO = PierCrossingFNO;
                oNewFeature = GTPierCrossing.application.DataContext.NewFeature(iFNO);
                iFID = oNewFeature.FID;
                newFID = iFID;
                newFNO = iFNO;

                for (int i = 0; i < NewLeftPoints.Count; i++)
                {
                    oLeftPolyline.Points.Add(NewLeftPoints[i]);
                }

                for (int j = 0; j < NewRightPoints.Count; j++)
                {
                    oRightPolyline.Points.Add(NewRightPoints[j]);
                }

                iCNO = PierCrossingAttribCNO;
                if (mobjPierCrossAttribute != null)
                {
                    CopyComponentAttribute(mobjPierCrossAttribute, oNewFeature);
                }
                //NETELEM
                iCNO = 51;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                }
                  
                //Left Geometry
                iCNO = PierCrossingPriGeoCNO;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }               

                oNewFeature.Components.GetComponent(iCNO).Geometry = oLeftPolyline;
                GTPierCrossing.application.SetProgressBarPosition(50); 

                //Right Geometry
                iCNO = PierCrossingPriGeoCNO;
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);

                oNewFeature.Components.GetComponent(iCNO).Geometry = oRightPolyline;
                GTPierCrossing.application.SetProgressBarPosition(75);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }

        private void Calc_LPXCoord(IGTPoints InPoints, double offsetValue)
        {
            try
            {
                double XUnityPoint;
                double YUnityPoint;
                double cUnity;
                double sUnity;
                double lUnity;
                IGTPoint tmpPoint = GTClassFactory.Create<IGTPoint>();

                cUnity = InPoints[1].X - InPoints[0].X;
                sUnity = InPoints[1].Y - InPoints[0].Y;
                lUnity = Math.Sqrt((cUnity * cUnity) + (sUnity * sUnity));

                XUnityPoint = (cUnity / lUnity);
                YUnityPoint = (sUnity / lUnity);

                tmpPoint.X = InPoints[0].X + (4.0 * offsetValue * YUnityPoint) - (2.5 * offsetValue * XUnityPoint);
                tmpPoint.Y = InPoints[0].Y - (4.0 * offsetValue * XUnityPoint) - (2.5 * offsetValue * YUnityPoint);
                tmpPoint.Z = 0.0;
                LeftPoints.Clear();
                LeftPoints.Add(tmpPoint);

                tmpPoint.X = InPoints[0].X + (1.5 * offsetValue * YUnityPoint);
                tmpPoint.Y = InPoints[0].Y - (1.5 * offsetValue * XUnityPoint);
                tmpPoint.Z = 0.0;
                LeftPoints.Add(tmpPoint);

                tmpPoint.X = InPoints[1].X + (1.5 * offsetValue * YUnityPoint);
                tmpPoint.Y = InPoints[1].Y - (1.5 * offsetValue * XUnityPoint);
                tmpPoint.Z = 0.0;
                LeftPoints.Add(tmpPoint);

                tmpPoint.X = InPoints[1].X + (4.0 * offsetValue * YUnityPoint) + (2.5 * offsetValue * XUnityPoint);
                tmpPoint.Y = InPoints[1].Y - (4.0 * offsetValue * XUnityPoint) + (2.5 * offsetValue * YUnityPoint);
                tmpPoint.Z = 0.0;
                LeftPoints.Add(tmpPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }

        private void Calc_RPXCoord(IGTPoints InPoints, double offsetValue)
        {
            try
            {
                double XUnityPoint;
                double YUnityPoint;
                double cUnity;
                double sUnity;
                double lUnity;
                IGTPoint tmpPoint = GTClassFactory.Create<IGTPoint>();

                cUnity = InPoints[1].X - InPoints[0].X;
                sUnity = InPoints[1].Y - InPoints[0].Y;
                lUnity = Math.Sqrt((cUnity * cUnity) + (sUnity * sUnity));

                XUnityPoint = (cUnity / lUnity);
                YUnityPoint = (sUnity / lUnity);

                tmpPoint.X = InPoints[0].X - (4.0 * offsetValue * YUnityPoint) - (2.5 * offsetValue * XUnityPoint);
                tmpPoint.Y = InPoints[0].Y + (4.0 * offsetValue * XUnityPoint) - (2.5 * offsetValue * YUnityPoint);
                tmpPoint.Z = 0.0;
                RightPoints.Clear();
                RightPoints.Add(tmpPoint);

                tmpPoint.X = InPoints[0].X - (1.5 * offsetValue * YUnityPoint);
                tmpPoint.Y = InPoints[0].Y + (1.5 * offsetValue * XUnityPoint);
                tmpPoint.Z = 0.0;
                RightPoints.Add(tmpPoint);

                tmpPoint.X = InPoints[1].X - (1.5 * offsetValue * YUnityPoint);
                tmpPoint.Y = InPoints[1].Y + (1.5 * offsetValue * XUnityPoint);
                tmpPoint.Z = 0.0;
                RightPoints.Add(tmpPoint);

                tmpPoint.X = InPoints[1].X - (4.0 * offsetValue * YUnityPoint) + (2.5 * offsetValue * XUnityPoint);
                tmpPoint.Y = InPoints[1].Y + (4.0 * offsetValue * XUnityPoint) + (2.5 * offsetValue * YUnityPoint);
                tmpPoint.Z = 0.0;
                RightPoints.Add(tmpPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }

        public void Cancel_Placement()
        {
            try
            {
                RefPoints.Clear();
                gGeomCS.RemoveAllGeometries();
                selected = false;
                selectedFID = 0;
                selectedFNO = 0;
                addMode = 0;
                rotateMode = 0;
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Placement is cancelled");
                GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                ShowPierForm();
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }
        #endregion


        #region Pier Cross Rotation and cancellation

        public Boolean _initRotatePX
        {
            set
            {
                InitiateRotation();
            }
        }

        public Boolean _tempRotatePX
        {
            set
            {
                tempRotatePierCrossing();
            }
        }

        public Boolean _confRotatePX
        {
            set
            {
                confRotatePierCrossing();
            }
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            try
            {
                InitiateRotation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }

        private void InitiateRotation()
        {
            try
            {
                short iCNO = 0;
                int iCID = 0;
                int numGeoLine = 0;

                Boolean isValid = true;
                IGTKeyObject oPXFeature = null;
                IGTSelectedObjects SelectedObjects = null;
                IGTPoints oPoints = GTClassFactory.Create<IGTPoints>();
                IGTPoint oPoint = GTClassFactory.Create<IGTPoint>();
                
                HidePierForm();
                rotateMode = 2;
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature ...");
                GTPierCrossing.application.BeginWaitCursor();
                gGeomES.TargetMapWindow = GTPierCrossing.application.ActiveMapWindow;
                gGeomES.RemoveAllGeometries();
                oPoints.Clear();

                SelectedObjects = GTPierCrossing.application.SelectedObjects;

                if (SelectedObjects.FeatureCount == 0) // no feature is selected
                {
                    rotateMode = 1;
                    GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to rotate. Right Click to cancel rotation");
                    GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                }
                else if (SelectedObjects.FeatureCount == 1) // at least one feature selected
                {
                    if (selected)// check selected == true
                        getPreviousSelectedFeature(selectedFID, selectedFNO);

                    foreach (IGTDDCKeyObject oDDCKeyObject in SelectedObjects.GetObjects())
                    {
                        if (oDDCKeyObject.FNO != PierCrossingFNO)
                            isValid = false;
                        else
                        {
                            if (oDDCKeyObject.ComponentViewName == "VGC_PIERCROSS_L")
                            {
                                
                                if (oDDCKeyObject.Recordset.RecordCount > 0)
                                {

                                    oDDCKeyObject.Recordset.MoveFirst();
                                    for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                                    {
                                        if (oDDCKeyObject.Recordset.Fields[i].Name == "G3E_FID")
                                        {
                                            rotateFID = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                        }
                                        if (oDDCKeyObject.Recordset.Fields[i].Name == "G3E_CNO")
                                        {
                                            iCNO = short.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                        }
                                        if (oDDCKeyObject.Recordset.Fields[i].Name == "G3E_CID")
                                        {
                                            iCID = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                        }
                                    }
                                    //rotateFID = int.Parse(rs.Fields[3].Value.ToString());
                                    //iCNO = short.Parse(rs.Fields[2].Value.ToString());
                                    //iCID = int.Parse(rs.Fields[4].Value.ToString());

                                    if (iCNO == PierCrossingPriGeoCNO)
                                    {
                                        if ((iCID == 1) || (iCID == 2))
                                        {
                                            gGeomES.AddGeometry(oDDCKeyObject.Geometry, (int)GTStyleIDConstants.gtstyleLineSelectSolid2);
                                            oPoints.Add(oDDCKeyObject.Geometry.GetKeypointPosition(1));
                                            oPoints.Add(oDDCKeyObject.Geometry.GetKeypointPosition(2));
                                            numGeoLine = numGeoLine + 1;
                                        }
                                        else
                                        {
                                            numGeoLine = numGeoLine + 1;
                                        }
                                    }// end if

                                    oPXFeature = GTPierCrossing.application.DataContext.OpenFeature(oDDCKeyObject.FNO, rotateFID);
                                    selectedFNO = oDDCKeyObject.FNO;
                                    selectedFID = rotateFID;
                                    selected = true;

                                }//end if

                               

                            }//end if

                        }//end if else
                        
                        if (!isValid)
                            break; // stop looping if the selected feature is not pier cross.

                    }//end foreach

                    if (!isValid) // check if the selected feature is Pier Cross
                    {
                        GTPierCrossing.application.EndWaitCursor();
                        GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature ...");
                        GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                        MessageBox.Show("Please select only Pier Crossing feature to rotate", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                      //  ShowPierForm();
                        GTPierCrossing.application.SelectedObjects.Clear();
                        rotateMode = 1;
                        GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to rotate. Right Click to cancel rotation");
                        GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                        return;
                    }
                    else
                    {
                        RotationPoints.Clear();

                        oPoint.X = (oPoints[0].X + oPoints[2].X) / 2.0;
                        oPoint.Y = (oPoints[0].Y + oPoints[2].Y) / 2.0;
                        oPoint.Z = 0.0;
                        RotationPoints.Add(oPoint);

                        oPoint.X = (oPoints[0].X + oPoints[1].X + oPoints[2].X + oPoints[3].X) / 4.0;
                        oPoint.Y = (oPoints[0].Y + oPoints[1].Y + oPoints[2].Y + oPoints[3].Y) / 4.0;
                        oPoint.Z = 0.0;
                        RotationPoints.Add(oPoint);

                        oPoint.X = (oPoints[1].X + oPoints[3].X) / 2.0;
                        oPoint.Y = (oPoints[1].Y + oPoints[3].Y) / 2.0;
                        oPoint.Z = 0.0;
                        RotationPoints.Add(oPoint);

                        gGeomES.BeginRotate(RotationPoints[0], RotationPoints[1]);
                        rotateMode = 3;
                        rotateOrigin = 1;
                        GTPierCrossing.application.EndWaitCursor();
                     //   GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to accept the new rotation angle. Press Space to toggle rotation origin. Right Click to cancel rotation");
                        GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                    }//end if else
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
                GTPierCrossing.application.EndWaitCursor();
                ShowPierForm();
            }
        }

        private void tempRotatePierCrossing()
        {
            try
            {                
                GTPierCrossing.application.BeginWaitCursor();
                GTPierCrossing.application.SetProgressBarRange(0, 100);
                GTPierCrossing.application.BeginProgressBar();
                GTPierCrossing.m_oIGTTransactionManager.Begin("RotatePierCrossing");
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to accept the rotation angel. Right Click to cancel rotation");
                rotateMode = 6; 
            }
            catch (Exception ex)
            {
                GTPierCrossing.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
                ShowPierForm();
            }
        }

        // Only execute when user confirmed with the new Pier Cross angle
        private void confRotatePierCrossing()
        {
            try
            {

                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, rotating in process...");
                short iFNO;
                short iCNO;
                int iFID;
                IGTKeyObject oPXFeature = null;
                IGTSelectedObjects SelectedObjects = null;

                iFNO = PierCrossingFNO;
                iFID = rotateFID;
                iCNO = PierCrossingPriGeoCNO;

                gGeomES.EndRotate(_SelGeom);

                oPXFeature = GTPierCrossing.application.DataContext.OpenFeature(iFNO, iFID);
                GTPierCrossing.application.SetProgressBarPosition(25);

                if (oPXFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    GTPierCrossing.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error getting component of Pier Crossing feature", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (gGeomCS != null)
                    {
                        if (gGeomCS.GeometryCount != 0)
                            gGeomCS.RemoveAllGeometries();
                    }
                }
                else
                {
                    oPXFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                    oPXFeature.Components.GetComponent(iCNO).Geometry = gGeomES.GetGeometry(1).Copy();
                    GTPierCrossing.application.SetProgressBarPosition(50);
                    oPXFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
                    oPXFeature.Components.GetComponent(iCNO).Geometry = gGeomES.GetGeometry(2).Copy();
                    GTPierCrossing.application.SetProgressBarPosition(75);

                    GTPierCrossing.m_oIGTTransactionManager.Commit();
                    GTPierCrossing.m_oIGTTransactionManager.RefreshDatabaseChanges();
                    GTPierCrossing.application.SetProgressBarPosition(100);
                    GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Pier Crossing Geo Line is rotated");
                }

                gGeomES.RemoveAllGeometries();

                selected = false;
                rotateMode = 0;
                SelectedObjects = GTPierCrossing.application.SelectedObjects;
                SelectedObjects.Clear();
                GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                ShowPierForm();
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }

        public void Cancel_Rotation()
        {
            try
            {
                if (GTPierCrossing.m_oIGTTransactionManager.TransactionInProgress)
                    GTPierCrossing.m_oIGTTransactionManager.Rollback();

                IGTSelectedObjects SelectedObjects = null;

                if (rotateMode == 3)
                {
                    gGeomES.CancelRotate();
                    gGeomES.RemoveAllGeometries();
                }
                SelectedObjects = GTPierCrossing.application.SelectedObjects;
                SelectedObjects.Clear();

                rotateMode = 0;
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Rotation is cancelled");
                GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                ShowPierForm();
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }
        #endregion


        #region Pier Cross Movement and Cancellation

       
        public Boolean _tempMovePX
        {
            set
            {
                tempMovePierCrossing();
            }
        }

        public Boolean _confMovePX
        {
            set
            {
                MovePierCrossing();
            }
        }
        public Boolean _initMovePX
        {
            set
            {
                InitiateMove();
            }
        }
        private void tempMovePierCrossing()
        {
            try
            {
                GTPierCrossing.application.BeginWaitCursor();
                GTPierCrossing.application.SetProgressBarRange(0, 100);
                GTPierCrossing.application.BeginProgressBar();
                GTPierCrossing.m_oIGTTransactionManager.Begin("MovePierCrossing");
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to accept the position. Right Click to cancel moving");
                moveMode = 6;
            }
            catch (Exception ex)
            {
                GTPierCrossing.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
                ShowPierForm();
            }
        }
        private void btnMove_Click(object sender, EventArgs e)
        {
            try
            {
                InitiateMove();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }

        private void InitiateMove()
        {
            try
            {
                Boolean isValid = true;
                IGTSelectedObjects SelectedObjects = null;
                IGTPoints oPoints = GTClassFactory.Create<IGTPoints>();
                IGTPoint oPoint = GTClassFactory.Create<IGTPoint>();
                int numGeoLine = 0;
                short iCNO = 0;
                int iCID = 0;

                HidePierForm();
                moveMode = 2;
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature ...");
                GTPierCrossing.application.BeginWaitCursor();
                gGeomES.TargetMapWindow = GTPierCrossing.application.ActiveMapWindow;
                gGeomES.RemoveAllGeometries();
                oPoints.Clear();

                SelectedObjects = GTPierCrossing.application.SelectedObjects;

               if (SelectedObjects.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in SelectedObjects.GetObjects())
                    {
                        if (oDDCKeyObject.FNO != PierCrossingFNO)
                        {
                            isValid = false;
                        }
                        else
                        {
                            if (oDDCKeyObject.ComponentViewName == "VGC_PIERCROSS_L")
                            {
                                if (oDDCKeyObject.Recordset.RecordCount > 0)
                                {
                                    oDDCKeyObject.Recordset.MoveFirst();
                                    for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                                    {
                                        if (oDDCKeyObject.Recordset.Fields[i].Name == "G3E_FID")
                                        {
                                            moveFID = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                        }
                                        if (oDDCKeyObject.Recordset.Fields[i].Name == "G3E_CNO")
                                        {
                                            iCNO = short.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                        }
                                        if (oDDCKeyObject.Recordset.Fields[i].Name == "G3E_CID")
                                        {
                                            iCID = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                        }
                                    }
                                    //moveFID = int.Parse(rs.Fields[2].Value.ToString());
                                    //iCNO = short.Parse(rs.Fields[3].Value.ToString());
                                    //iCID = int.Parse(rs.Fields[4].Value.ToString());

                                    if (iCNO == PierCrossingPriGeoCNO)
                                    {
                                        if ((iCID == 1) || (iCID == 2))
                                        {
                                            gGeomES.AddGeometry(oDDCKeyObject.Geometry, (int)GTStyleIDConstants.gtstyleLineSelectSolid2);
                                            oPoints.Add(oDDCKeyObject.Geometry.GetKeypointPosition(1));
                                            oPoints.Add(oDDCKeyObject.Geometry.GetKeypointPosition(2));
                                            numGeoLine = numGeoLine + 1;
                                        }
                                        else
                                        {
                                            numGeoLine = numGeoLine + 1;
                                        }
                                    }
                                }
                               
                            }
                        }
                    }

                    if (!isValid)
                    {
                        GTPierCrossing.application.EndWaitCursor();
                        GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                        GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                        MessageBox.Show("Please select only Pier Crossing feature to move", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        moveMode = 1;
                        GTPierCrossing.application.SelectedObjects.Clear();
                        GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to move. Right Click to cancel moving");
                        GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                        //  ShowPierForm();
                        return;
                    }
                    else
                    {
                        oPoint.X = (oPoints[0].X + oPoints[1].X + oPoints[2].X + oPoints[3].X) / 4.0;
                        oPoint.Y = (oPoints[0].Y + oPoints[1].Y + oPoints[2].Y + oPoints[3].Y) / 4.0;
                        oPoint.Z = 0.0;

                        gGeomES.BeginMove(oPoint);
                        moveMode = 3;
                        GTPierCrossing.application.EndWaitCursor();
                        //GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to accept the new position. Right Click to cancel move");
                        GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                    }
                }
                else
                {
                    moveMode = 1;
                    GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to move. Right Click to cancel move");
                    GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
                GTPierCrossing.application.EndWaitCursor();
                ShowPierForm();
            }
        }

        private void MovePierCrossing()
        {
            try
            {

                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, moving in process...");
                short iFNO;
                short iCNO;
                int iFID;
                IGTKeyObject oPXFeature = null;

                iFNO = PierCrossingFNO;
                iFID = moveFID;
                iCNO = PierCrossingPriGeoCNO;

             //   moveMode = 4;
             //   GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Updating database ...");
                //GTPierCrossing.application.BeginWaitCursor();
                //GTPierCrossing.application.SetProgressBarRange(0, 100);
                //GTPierCrossing.application.BeginProgressBar();
                //GTPierCrossing.m_oIGTTransactionManager.Begin("MovePierCrossing");

                gGeomES.EndMove(_SelGeom);

                oPXFeature = GTPierCrossing.application.DataContext.OpenFeature(iFNO, iFID);
                GTPierCrossing.application.SetProgressBarPosition(25);

                if (oPXFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    GTPierCrossing.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error getting component of Pier Crossing feature", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (gGeomCS != null)
                    {
                        if (gGeomCS.GeometryCount != 0)
                            gGeomCS.RemoveAllGeometries();
                    }
                }
                else
                {
                    oPXFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                    oPXFeature.Components.GetComponent(iCNO).Geometry = gGeomES.GetGeometry(1).Copy();
                    GTPierCrossing.application.SetProgressBarPosition(50);
                    oPXFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
                    oPXFeature.Components.GetComponent(iCNO).Geometry = gGeomES.GetGeometry(2).Copy();
                    GTPierCrossing.application.SetProgressBarPosition(75);

                    GTPierCrossing.m_oIGTTransactionManager.Commit();
                    GTPierCrossing.m_oIGTTransactionManager.RefreshDatabaseChanges(); 
                    GTPierCrossing.application.SetProgressBarPosition(100);
                    GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Pier Crossing Geo Line is moved");
                }

                gGeomES.RemoveAllGeometries();
                moveMode = 0;
                GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                ShowPierForm();
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
            }
            catch (Exception ex)
            {
                GTPierCrossing.m_oIGTTransactionManager.Rollback();
                rotateMode = 0;
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
                ShowPierForm();
            }
        }

        public void Cancel_Move()
        {
            try
            {
                if (GTPierCrossing.m_oIGTTransactionManager.TransactionInProgress)
                    GTPierCrossing.m_oIGTTransactionManager.Rollback();

                IGTSelectedObjects SelectedObjects = null;

                if (moveMode == 3)
                {
                    gGeomES.CancelMove();
                    gGeomES.RemoveAllGeometries();
                }

                SelectedObjects = GTPierCrossing.application.SelectedObjects;
                SelectedObjects.Clear();

                moveMode = 0;
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Moving is cancelled");
                GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                ShowPierForm();
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }
        #endregion


        # region Pier Cross Deletion and Cancellation

        public Boolean _initDeletePX
        {
            set
            {
                InitiateDeletion();
            }
        } 

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                InitiateDeletion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
            }
        }

        private void InitiateDeletion()
        {
            try
            {
                Boolean isValid = true;
                IGTSelectedObjects SelectedObjects = null;
                IGTPoints oPoints = GTClassFactory.Create<IGTPoints>();
                IGTPoint oPoint = GTClassFactory.Create<IGTPoint>();
                int numGeoLine = 0;
                short iCNO = 0;
                int iCID = 0;

                HidePierForm();
                deleteMode = 2;
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature ...");
                GTPierCrossing.application.BeginWaitCursor();

                SelectedObjects = GTPierCrossing.application.SelectedObjects;

               
                if (SelectedObjects.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in SelectedObjects.GetObjects())
                    {
                        if (oDDCKeyObject.FNO != PierCrossingFNO)
                        {
                            isValid = false;
                        }
                        else
                        {
                            if (oDDCKeyObject.ComponentViewName == "VGC_PIERCROSS_L")
                            {
                                
                                     if (oDDCKeyObject.Recordset.RecordCount > 0)
                                {
                                    oDDCKeyObject.Recordset.MoveFirst();
                                    for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                                    {
                                        if (oDDCKeyObject.Recordset.Fields[i].Name == "G3E_FID")
                                        {
                                            deleteFID = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                        }
                                        if (oDDCKeyObject.Recordset.Fields[i].Name == "G3E_CNO")
                                        {
                                            iCNO = short.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                        }
                                        if (oDDCKeyObject.Recordset.Fields[i].Name == "G3E_CID")
                                        {
                                            iCID = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                        }
                                    }
                                    //deleteFID = int.Parse(rs.Fields[2].Value.ToString());
                                    //iCNO = short.Parse(rs.Fields[3].Value.ToString());
                                    //iCID = int.Parse(rs.Fields[4].Value.ToString());

                                    if (iCNO == PierCrossingPriGeoCNO)
                                    {
                                        if ((iCID == 1) || (iCID == 2))
                                        {
                                            numGeoLine = numGeoLine + 1;
                                        }
                                        else
                                        {
                                            numGeoLine = numGeoLine + 1;
                                        }
                                    }
                                }
                               
                            }
                        }
                    }

                    if (!isValid)
                    {
                        GTPierCrossing.application.EndWaitCursor();
                        MessageBox.Show("Please select only Pier Crossing feature to delete", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        deleteMode = 1;
                        GTPierCrossing.application.SelectedObjects.Clear();
                        GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to delete. Right Click to cancel deletion");
                        GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                        // ShowPierForm();
                        return;
                    }
                    else
                    {
                        deleteMode = 3;
                        GTPierCrossing.application.EndWaitCursor();
                        GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                        GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;

                        if (MessageBox.Show("Selected Pier Crossing Geo Line and its attribute information will be deleted.  Are you sure?",
                                            "Pier Crossing", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                             == System.Windows.Forms.DialogResult.Yes)
                        {
                            DeletePierCrossing();
                        }
                        else
                        {
                            Cancel_Deletion();
                        }
                    }
                }
                else
                {
                    deleteMode = 1;
                    GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to delete. Right Click to cancel deletion");
                    GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
                GTPierCrossing.application.EndWaitCursor();
                ShowPierForm();
            }
        }

        private void DeletePierCrossing()
        {
            try
            {
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, deleting in process ...");
                short iFNO;
                short iCNO;
                int iFID;
                IGTKeyObject oPXFeature = null;

                iFNO = PierCrossingFNO;
                iFID = deleteFID;

                deleteMode = 4;
               // GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Updating database ...");
                GTPierCrossing.application.BeginWaitCursor();
                GTPierCrossing.application.SetProgressBarRange(0, 100);
                GTPierCrossing.application.BeginProgressBar();
                GTPierCrossing.m_oIGTTransactionManager.Begin("DeletePierCrossing");

                oPXFeature = GTPierCrossing.application.DataContext.OpenFeature(iFNO, iFID);

                iCNO = PierCrossingPriGeoCNO;
                if (oPXFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    GTPierCrossing.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error getting component of Pier Crossing feature", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (gGeomCS != null)
                    {
                        if (gGeomCS.GeometryCount != 0)
                            gGeomCS.RemoveAllGeometries();
                    }
                }
                else
                {
                    oPXFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                    oPXFeature.Components.GetComponent(iCNO).Recordset.Delete(AffectEnum.adAffectCurrent);
                    GTPierCrossing.application.SetProgressBarPosition(25);
                    oPXFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                    oPXFeature.Components.GetComponent(iCNO).Recordset.Delete(AffectEnum.adAffectCurrent);
                    GTPierCrossing.application.SetProgressBarPosition(50);
                }

                iCNO = PierCrossingAttribCNO;
                if (oPXFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    GTPierCrossing.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error getting component of Pier Crossing feature", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (gGeomCS != null)
                    {
                        if (gGeomCS.GeometryCount != 0)
                            gGeomCS.RemoveAllGeometries();
                    }
                }
                else
                {
                    oPXFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                    oPXFeature.Components.GetComponent(iCNO).Recordset.Delete(AffectEnum.adAffectCurrent);
                    GTPierCrossing.application.SetProgressBarPosition(75);

                    GTPierCrossing.m_oIGTTransactionManager.Commit();
                    GTPierCrossing.m_oIGTTransactionManager.RefreshDatabaseChanges();
                    GTPierCrossing.application.SetProgressBarPosition(100);
                    GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Pier Crossing Geo Line is deleted");
                }

                deleteMode = 0;
                GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
                ShowPierForm();
            }
            catch (Exception ex)
            {
                GTPierCrossing.m_oIGTTransactionManager.Rollback();
                rotateMode = 0;
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
                ShowPierForm();
            }
        }

        public void Cancel_Deletion()
        {
            try
            {
                IGTSelectedObjects SelectedObjects = null;

                SelectedObjects = GTPierCrossing.application.SelectedObjects;
                SelectedObjects.Clear();

                deleteMode = 0;
                GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deletion is cancelled");
                GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
                ShowPierForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (gGeomCS != null)
                {
                    if (gGeomCS.GeometryCount != 0)
                        gGeomCS.RemoveAllGeometries();
                }
                GTPierCrossing.application.EndProgressBar();
                GTPierCrossing.application.EndWaitCursor();
                ShowPierForm();
            }
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        #region Dialog Show/Hide
        public void HidePierForm()
        {
            DisableAllButtons();
            this.Hide();
        }

        public void ShowPierForm()
        {
           EnableAllButtons();
           GTPierCrossing.application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
           GTPierCrossing.application.EndProgressBar();
           GTPierCrossing.application.EndWaitCursor();
           this.Show();
           this.Focus();
        }
        #endregion

      

        private void Frm_PierCrossing_Shown(object sender, EventArgs e)
        {
            //string msgstr;

            //msgstr = GTPierCrossing.application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);
            //GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, msgstr);
        }

        private void Frm_PierCrossing_Activated(object sender, EventArgs e)
        {
            //string msgstr;

            //msgstr = GTPierCrossing.application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);
            //GTPierCrossing.application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, msgstr);
        }

        private void frmPierCrossing_FormClosing(object sender, FormClosingEventArgs e)
        {
             DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Pier Crossing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
             if (retVal == DialogResult.Yes)
             {
                 e.Cancel = false;
             }
             else { e.Cancel = true; }
        }

        /*private void txtImpYear_KeyPress(object sender, KeyPressEventArgs e)
{            
//          if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar)) e.Handled = true;
}

private void txtMDF_KeyPress(object sender, KeyPressEventArgs e)
{           
//          if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar)) e.Handled = true;
}

private void txtFTB_KeyPress(object sender, KeyPressEventArgs e)
{
//          if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsPunctuation(e.KeyChar)) e.Handled = true;
}
*/
    }       
}