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

namespace NEPS.GTechnology.NEPSDuctPathReOwn
{
    public partial class DPReOwnForm : Form
    {
        #region var

        private int CloseStatus = 0;

        #endregion
       
        #region init and load form
        public DPReOwnForm()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseStatus = 1;
                this.Close();
            }
        }
        private void DPExpansionFrom_Load(object sender, EventArgs e)
        {
            FillingOriginalAttr();
        }
        #region filling first page
        private void FillingOriginalAttr()
        {
            txtDuctPathFID.Text = GTDuctPathReOwn.DuctPathOrigin.FID.ToString();
            txtDuctSourceFID.Text = GTDuctPathReOwn.DuctPathOrigin.sourceFID.ToString();
            txtDuctSourceType.Text = GTDuctPathReOwn.DuctPathOrigin.sourceType;
            txtDuctSourceWall.Text = GTDuctPathReOwn.DuctPathOrigin.sourceWall.ToString();
            txtDuctTermFID.Text = GTDuctPathReOwn.DuctPathOrigin.termFID.ToString();
            txtDuctTermType.Text = GTDuctPathReOwn.DuctPathOrigin.termType;
            txtDuctTermWall.Text = GTDuctPathReOwn.DuctPathOrigin.termWall.ToString();
            if (txtDuctSourceType.Text == "Civil Node" || txtDuctSourceType.Text == "STUMP" || txtDuctSourceType.Text == "STUB")
                btnWallSource.Visible = false;

            if (txtDuctTermType.Text == "Civil Node" || txtDuctTermType.Text == "STUMP" || txtDuctTermType.Text == "STUB")
                btnWallTerm.Visible = false;
        }
        #endregion
        #endregion

        #region Closing Form
        private void DPExpansionFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseStatus == 0)
            {
                DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Duct Path Re-Own to Wall", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                  //  LocateFeature(2, GTDuctPathReOwn.m_gtapp.ActiveMapWindow);
                    e.Cancel = false;
                }
                else { e.Cancel = true; }
            }
            else
            {
                e.Cancel = false;
            }
        }
        #endregion
        #region Button close2 click
        private void btnClose2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion 

        #region Re-own Source
        private void btnWallSource_Click(object sender, EventArgs e)
        {
            LocateFeature(1, GTDuctPathReOwn.m_gtapp.ActiveMapWindow);
            this.Hide();
            GTDuctPathReOwn.step = 20;
            GTDuctPathReOwn.FromTo = true;
        }

        #endregion

        #region Re-own Term
        private void btnWallTerm_Click(object sender, EventArgs e)
        {
            LocateFeature(3, GTDuctPathReOwn.m_gtapp.ActiveMapWindow);
            this.Hide();
            GTDuctPathReOwn.step = 30;
            GTDuctPathReOwn.FromTo = false;
        }

        #endregion

        #region Get Wall Number
           public bool GetDeviceWithWall( bool flag)
        {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;
            string WallNum = "";
            short iFNOSelected = 0;
            int iFIDSelected = 0;
            string FeatureType = "";

            if (flag)
            {
                FeatureType = GTDuctPathReOwn.DuctPathOrigin.sourceType;// txtTypeSource.Text;
                iFIDSelected = GTDuctPathReOwn.DuctPathOrigin.sourceFID;// int.Parse(txtFIDSource.Text);
                iFNOSelected = GTDuctPathReOwn.DuctPathOrigin.sourceFNO;
            }
            else
            {
                FeatureType = GTDuctPathReOwn.DuctPathOrigin.termType;// txtTypeTerm.Text;
                iFIDSelected = GTDuctPathReOwn.DuctPathOrigin.termFID;// int.Parse(txtFIDTerm.Text);
                iFNOSelected = GTDuctPathReOwn.DuctPathOrigin.termFNO;
            }

            #region check if selected allowed feature and if successshow detail
            if (GTDuctPathReOwn.m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                return false;
            }

            if (GTDuctPathReOwn.m_gtapp.SelectedObjects.FeatureCount > 1)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                return false;
            }


            foreach (IGTDDCKeyObject oDDCKeyObject in GTDuctPathReOwn.m_gtapp.SelectedObjects.GetObjects())
            {
                geom = oDDCKeyObject.Geometry;
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;
               
                if(iFNOSelected != iFNO)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                    return false;
                }
                if (iFIDSelected != iFID)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                    return false;
                }

                //manhole
                if (iFNO == 2700)
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
                //chamber
                else if (iFNO == 3800)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_CHAMBERWALL_T")
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
                //tunnel/trench
                else if (iFNO == 3300)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_TUNNELWALL_T")
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

                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                return false;

            }
            #endregion

            IGTPolylineGeometry DuctPathLineGeomNew = GTClassFactory.Create<IGTPolylineGeometry>();

            if (flag)
            {
                if (txtDuctSourceWall.Text == WallNum)
                {
                    MessageBox.Show("Duct Path is already owned by this wall number!", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                    return false;
                }
                else
                {
                    txtDuctSourceWall.Text = WallNum;
                    DuctPathLineGeomNew.Points.Add(geom.FirstPoint);
                    //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                    for (int i = 1; i < GTDuctPathReOwn.DuctPathOrigin.DuctPathLineGeom.Points.Count; i++)
                    {
                        DuctPathLineGeomNew.Points.Add(GTDuctPathReOwn.DuctPathOrigin.DuctPathLineGeom.Points[i]);
                        // IGTCompositePolylineGeometry
                    }

                    
                }
            }
            else
            {
                if (txtDuctTermWall.Text == WallNum)
                {
                    MessageBox.Show("Duct Path is already owned by this wall number!", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                    GTDuctPathReOwn.m_gtapp.RefreshWindows();
                    return false;
                }
                else
                {
                txtDuctTermWall.Text = WallNum;
                //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                for (int i = 0; i < GTDuctPathReOwn.DuctPathOrigin.DuctPathLineGeom.Points.Count-1; i++)
                {
                    DuctPathLineGeomNew.Points.Add(GTDuctPathReOwn.DuctPathOrigin.DuctPathLineGeom.Points[i]);
                }

                DuctPathLineGeomNew.Points.Add(geom.FirstPoint);

               
            }            
            }

            if (!ReOwnWall(DuctPathLineGeomNew))
            {
                GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                return false;
            }
            if (flag)
            {
                if (GTDuctPathReOwn.DuctPathOrigin.DuctNestFrom != null)
                    if (GTDuctPathReOwn.DuctPathOrigin.DuctNestFrom.Count > 0)
                    {
                        LocateFeature(1, GTDuctPathReOwn.m_gtapp.ActiveMapWindow);
                        this.Hide();
                        GTDuctPathReOwn.step = 50;
                        GTDuctPathReOwn.FromTo = true;
                        return true;
                    }
            }
            else
            {
                if (GTDuctPathReOwn.DuctPathOrigin.DuctNestTo != null)
                
                    if (GTDuctPathReOwn.DuctPathOrigin.DuctNestTo.Count > 0)
                    {
                        LocateFeature(3, GTDuctPathReOwn.m_gtapp.ActiveMapWindow);
                        this.Hide();
                        GTDuctPathReOwn.step = 50;
                        GTDuctPathReOwn.FromTo = false;
                        return true;

                    }
                
                   

            }
            GTDuctPathReOwn.step = 0;
            this.Show();
           // GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
            return true;
        }
        #endregion

        #region Re-OwnWall
        public bool ReOwnWall(IGTPolylineGeometry geomNew)
        {
               try
            {
                GTDuctPathReOwn.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait Re-Owning in process ... ");
                           
                GTDuctPathReOwn.m_gtapp.BeginProgressBar();
                GTDuctPathReOwn.m_gtapp.SetProgressBarRange(0, 100);
                GTDuctPathReOwn.m_gtapp.SetProgressBarPosition(5);
                GTDuctPathReOwn.m_oIGTTransactionManager.Begin("DuctPathUpdateGeom");
                GTDuctPathReOwn.m_gtapp.SetProgressBarPosition(10);
              
                 short iFNO=GTDuctPathReOwn.DuctPathOrigin.FNO;
                   int iFID=GTDuctPathReOwn.DuctPathOrigin.FID;
                   short iCNO = 0;
                   IGTKeyObject oNewFeature = GTDuctPathReOwn.m_IGTDataContext.OpenFeature(iFNO, iFID);
                   GTDuctPathReOwn.m_gtapp.SetProgressBarPosition(20);
                #region Line Graphic
                iCNO = 2210;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);

                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                }
                GTDuctPathReOwn.m_gtapp.SetProgressBarPosition(30);
                oNewFeature.Components.GetComponent(iCNO).Geometry = geomNew;
                //IGTVector tt = GTClassFactory.Create<IGTVector>();
                //   IGTPoint newpp=GTClassFactory.Create<IGTPoint>();
                //   newpp.X=geomNew.FirstPoint.X-15;
                //   newpp.Y=geomNew.FirstPoint.Y;
                //   newpp.Z=0;
                //   //tt=tt.BuildVector(geomNew.FirstPoint, newpp);
                //   //IGTGeometry temptemp = geomNew.Move(tt.BuildVector(geomNew.FirstPoint, newpp));
                ////   geomNew.Move(tt.BuildVector(geomNew.FirstPoint, newpp));
                //   oNewFeature.Components.GetComponent(iCNO).Geometry = geomNew;//.Move(tt.BuildVector(geomNew.FirstPoint, newpp));
                //   oNewFeature.Components.GetComponent(iCNO).Geometry.Move(tt.BuildVector(geomNew.FirstPoint, newpp));
                GTDuctPathReOwn.m_gtapp.SetProgressBarPosition(40);
           
                #endregion

                #region Attributes
                iCNO = 2201;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_FRM_WALL", txtDuctSourceWall.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_TO_WALL", txtDuctTermWall.Text);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_FRM_WALL", txtDuctSourceWall.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_TO_WALL", txtDuctTermWall.Text);
                }
                GTDuctPathReOwn.m_gtapp.SetProgressBarPosition(60);
         

                #endregion
              
                GTDuctPathReOwn.m_oIGTTransactionManager.Commit();
                GTDuctPathReOwn.m_gtapp.SetProgressBarPosition(70);
            
                GTDuctPathReOwn.m_oIGTTransactionManager.RefreshDatabaseChanges();
                GTDuctPathReOwn.m_gtapp.SetProgressBarPosition(100);

                MessageBox.Show("Re-owned of Duct Path is succesfully completed!", "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathReOwn.m_gtapp.EndProgressBar();
                GTDuctPathReOwn.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                GTDuctPathReOwn.DuctPathOrigin.sourceWall = int.Parse(txtDuctSourceWall.Text);
                GTDuctPathReOwn.DuctPathOrigin.termWall = int.Parse(txtDuctTermWall.Text);
                GTDuctPathReOwn.DuctPathOrigin.DuctPathLineGeom = geomNew;
            }
            catch (Exception ex)
            {
                GTDuctPathReOwn.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Re-Own to Wall", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GTDuctPathReOwn.m_gtapp.EndProgressBar();
                GTDuctPathReOwn.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                return false;
            }
            return true;
        }
        #endregion

        #region ZoomIn/ZoomOut

        public void LocateFeature(int flag, IGTMapWindow window)
        {
            if (window == null) return;
            IGTDDCKeyObjects feat = null;
            short iFNO = GTDuctPathReOwn.DuctPathOrigin.sourceFNO;// SourceFNO;
            int lFID = GTDuctPathReOwn.DuctPathOrigin.sourceFID;// SourceFID;

            GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();

            if (flag == 1)//fit for source feature
            {
                feat = GTDuctPathReOwn.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);


                for (int K = 0; K < feat.Count; K++)
                {
                    string t = feat[K].ComponentViewName.ToString();
                    if (feat[K].ComponentViewName == "VGC_MANHL_S" ||
                        feat[K].ComponentViewName == "VGC_PSGCON_S")
                    {
                        //2700 VGC_MANHL_S  
                        //2800 VGC_PSGCON_S
                        IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                        IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                        point1.X = feat[K].Geometry.FirstPoint.X - 3;
                        point1.Y = feat[K].Geometry.FirstPoint.Y - 3;
                        range.BottomLeft = point1;
                        point2.X = feat[K].Geometry.FirstPoint.X + 3;
                        point2.Y = feat[K].Geometry.FirstPoint.Y + 3;
                        range.TopRight = point2;
                        window.ZoomArea(range);
                        GTDuctPathReOwn.m_gtapp.RefreshWindows();
                        GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                        return;
                    }

                    if (feat[K].ComponentViewName == "VGC_CHAMBER_P" ||
                        feat[K].ComponentViewName == "VGC_TUNNEL_P")
                    {
                        //3800 VGC_CHAMBER_P
                        //3300 VGC_TUNNEL_P
                        for (int K2 = 0; K2 < feat.Count; K2++)
                            GTDuctPathReOwn.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                        window.FitSelectedObjects();
                        window.CenterSelectedObjects();
                        GTDuctPathReOwn.m_gtapp.RefreshWindows();
                        GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                        return;
                    }
                }

            }
            if (flag == 2)//copy source feature to selected obj to fit both source and term
            {
                feat = GTDuctPathReOwn.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                for (int K = 0; K < feat.Count; K++)
                    GTDuctPathReOwn.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
            }
            iFNO = GTDuctPathReOwn.DuctPathOrigin.termFNO;// TermFNO;
            lFID = GTDuctPathReOwn.DuctPathOrigin.termFID;// TermFID;// int.Parse(txtFIDTerm.Text);

            if (flag == 3)//fit for term feature
            {
                GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                feat = GTDuctPathReOwn.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);

                for (int K = 0; K < feat.Count; K++)
                {
                    string t = feat[K].ComponentViewName.ToString();
                    if (feat[K].ComponentViewName == "VGC_MANHL_S" ||
                        feat[K].ComponentViewName == "VGC_PSGCON_S")
                    {
                        //2700 VGC_MANHL_S  
                        //2800 VGC_PSGCON_S
                        IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                        IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                        point1.X = feat[K].Geometry.FirstPoint.X - 3;
                        point1.Y = feat[K].Geometry.FirstPoint.Y - 3;
                        range.BottomLeft = point1;
                        point2.X = feat[K].Geometry.FirstPoint.X + 3;
                        point2.Y = feat[K].Geometry.FirstPoint.Y + 3;
                        range.TopRight = point2;
                        window.ZoomArea(range);
                        GTDuctPathReOwn.m_gtapp.RefreshWindows();
                        GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                        return;
                    }

                    if (feat[K].ComponentViewName == "VGC_CHAMBER_P" ||
                        feat[K].ComponentViewName == "VGC_TUNNEL_P")
                    {
                        //3800 VGC_CHAMBER_P
                        //3300 VGC_TUNNEL_P
                        for (int K2 = 0; K2 < feat.Count; K2++)
                            GTDuctPathReOwn.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                        window.FitSelectedObjects();
                        window.CenterSelectedObjects();
                        GTDuctPathReOwn.m_gtapp.RefreshWindows();
                        GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
                        return;
                    }
                }

            }
            //copy term feature to selected obj to fit both source and term
            feat = GTDuctPathReOwn.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
            for (int K = 0; K < feat.Count; K++)
                GTDuctPathReOwn.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

            window.CenterSelectedObjects();
            window.FitSelectedObjects();
            GTDuctPathReOwn.m_gtapp.RefreshWindows();
            // GTDuctPathReOwn.m_gtapp.SelectedObjects.Clear();
        }

        #endregion
    }
}