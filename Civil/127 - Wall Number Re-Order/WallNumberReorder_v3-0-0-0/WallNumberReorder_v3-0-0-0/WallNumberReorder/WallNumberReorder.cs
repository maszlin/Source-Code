using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using System.Windows.Forms;

namespace NEPS.GTechnology.WallNumberReorder
{
    class GTWallNumberReorder: PlacementBase
    {
        private class WallNumberLabel : IComparable
        {
            public int ID = 0;
            public IGTPoint coordinates;
            public int numberOnLabel = 0; // 1,2,3,4 etc
            public int originalNumberOnLabel = 0;
            public double angle = 0; // angle relative to first label

            public int CompareTo(object obj)
            {
                WallNumberLabel other = (WallNumberLabel)obj;
                if (angle < other.angle)
                    return -1;
                if (angle > other.angle)
                    return 1;

                return 0;
            }

            public void CalculateRelativeAngle(WallNumberLabel firstLabel)
            {
                angle = Angle(firstLabel.coordinates, coordinates);
                if (angle < 0)
                    angle = 360 + angle;
                if(angle > 360)
                    angle =  angle-360;
            }

            private double Angle(IGTPoint start, IGTPoint end)
            {
                const double Rad2Deg = 180.0 / Math.PI;
              //  const double Deg2Rad = Math.PI / 180.0;
                return Math.Atan2(start.Y - end.Y, end.X - start.X) * Rad2Deg;               
               
            }
        }

        private short FNO_Selected = 0;
        private short CNO_Selected = 0;        
        private int FID_Selected = 0;
        private int Wall_Selected = 0 ;
        private short FNO_DuctPath = 2200;
        private string labelNumberField = "WALL_NUM";
        private string labelTableName = "";
        private string IDField = "G3E_ID";
        IGTLocateService mobjLocateService = null;
        private short step = 0;
        protected override void OnMouseMove(GTMouseEventArgs e)
        {
            if (step == 1)
            {
                app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to select a Manhole/Chamber/Tunnel. Right click to exit.");
            }
            else if (step == 2)
            {
                app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to select Wall Number to be first. Right click to exit.");
            } 
        }
        protected override void OnMouseUp(GTMouseEventArgs e)
        {
            if (e.Button != 2)
            {
                if (step == 1)
                {
                    app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to select a Manhole/Chamber/Tunnel. Right click to exit.");
                    IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                    for (int K = 0; K < feat.Count; K++)
                        app.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                    if (CheckSeletedFeature())
                    {
                        if (Wall_Selected != -1 && Wall_Selected != 0)
                        {
                            step = 0;
                            app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, re-ordering in process...");                            
                            ReOrder();
                      }
                        else
                        {
                            app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to select Wall Number to be first. Right click to exit.");
                            MessageBox.Show("Please select Wall Number to be first", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            step = 2;
                        }
                    }
                }
                else if (step == 2)
                {
                    app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to select Wall Number to be first. Right click to exit.");                        
                    IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                    for (int K = 0; K < feat.Count; K++)
                        app.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
                    Wall_Selected = GetWallNumber(true);
                    if (Wall_Selected != -1 && Wall_Selected != 0)
                    {
                        step = 0;
                        app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, re-ordering in process...");
                        ReOrder();
                    }
                }

            }
            else if (e.Button == 2)
            {
                if ((step == 1) || (step == 2))
                {
                    DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Wall Number Re-Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        EndCommand(true);
                    }
                }
               
            }

        }

        protected override void StepChange(bool pause)
        {
            if (pause)
                step += 1000;
            else step -= 1000;
        }
        protected override void OnActivate()
        {
            mobjLocateService = app.ActiveMapWindow.LocateService;
            if (CheckSeletedFeature())
            {
                if (Wall_Selected != -1 && Wall_Selected != 0)
                {
                    step = 0;
                    app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, re-ordering in process...");
                    ReOrder();
                }
                else
                {
                    step = 2;
                }
            }
            else
            {
                step = 1;
            }
        }
       
        private void ReOrder()
        {
            app.BeginWaitCursor();
            app.SetProgressBarRange(0, 100);
            app.BeginProgressBar();
            app.SetProgressBarPosition(5);
           IGTKeyObject SelectedFeature = app.DataContext.OpenFeature(FNO_Selected, FID_Selected);
           double MH_PointX = 0;
           double MH_PointY = 0;
           if (FNO_Selected == 2700)
           {
               MH_PointX=SelectedFeature.Components.GetComponent(2720).Geometry.FirstPoint.X;
               MH_PointY=SelectedFeature.Components.GetComponent(2720).Geometry.FirstPoint.Y;
           }
           app.SetProgressBarPosition(10);
            // get the wall number component
            IGTComponent wallNumberComponent = SelectedFeature.Components.GetComponent(CNO_Selected);
            // gather all the labels
            List<WallNumberLabel> labels = new List<WallNumberLabel>();
            
                    wallNumberComponent.Recordset.MoveFirst();
                    while (!wallNumberComponent.Recordset.EOF)
                    {
                        // convert the recordset into label objects for sorting later
                        WallNumberLabel label = new WallNumberLabel();
                        label.ID = Convert.ToInt32(wallNumberComponent.Recordset.Fields[IDField].Value);
                        label.coordinates = wallNumberComponent.Geometry.FirstPoint;
                        label.numberOnLabel = Convert.ToInt32(wallNumberComponent.Recordset.Fields[labelNumberField].Value);
                        label.originalNumberOnLabel = label.numberOnLabel;
                        labels.Add(label);

                        wallNumberComponent.Recordset.MoveNext();
                    }
                    app.SetProgressBarPosition(20);
            // at this point you have a list of labels
            // sort them by the relative angle to the first label
            if (labels.Count == 0)
            {
                MessageBox.Show("Selected Feature should have wall numbers", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                app.EndProgressBar();
                app.EndWaitCursor();
                step = 1;// EndCommand(true); // cancel if no labels
                return;
            }
            // calculate the relative angle of each label compared to the first label.

            // identify the first label from the list and set it as reference point
            WallNumberLabel firstLabel = null;
            int min = labels[0].numberOnLabel;
            foreach (WallNumberLabel label in labels)
            {
                if (min > label.numberOnLabel)
                    min = label.numberOnLabel;
            }
            Wall_Selected = min;
            foreach (WallNumberLabel label in labels)
            {
                if (label.numberOnLabel == Wall_Selected)
                {
                    firstLabel = label;
                    break;
                }
            }
            app.SetProgressBarPosition(30);
            // calculate each label's angle to the reference point established above
            if (FNO_Selected != 2700)
            {
                foreach (WallNumberLabel label in labels)
                {
                    label.CalculateRelativeAngle(firstLabel);
                  //  label.angle = 0;
                }
               
            }
            else
            {
                IGTPoint start = GTClassFactory.Create<IGTPoint>();
                start.X = MH_PointX;
                start.Y = MH_PointY;
                start.Z = 0;

                double angleFirst=Math.Atan2(firstLabel.coordinates.Y - MH_PointY, firstLabel.coordinates.X - MH_PointX) * 180.0 / Math.PI;
                if (angleFirst < 0)
                    angleFirst = 360 + angleFirst;
                if (angleFirst > 360)
                    angleFirst = angleFirst - 360;
                foreach (WallNumberLabel label in labels)
                {
                    double angle = Math.Atan2(label.coordinates.Y - MH_PointY, label.coordinates.X - MH_PointX) * 180.0 / Math.PI;
                    if (angle < 0)
                        angle = 360 + angle;
                    if (angle > 360)
                        angle = angle - 360;
                    angle = angle - angleFirst;
                    if (angle <= 0) angle += 360;
                    label.angle = 0-angle;
                }

           
            //Angle(IGTPoint start, IGTPoint end)
          
            }

            // sort from lowest to highest angle
            labels.Sort();
            app.SetProgressBarPosition(40);
            // prepare a list of sorted label numbers e.g. 1,2,3,4
            List<int> sortedLabelNumbers = new List<int>();
            foreach (WallNumberLabel label in labels)
                sortedLabelNumbers.Add(label.numberOnLabel);
            sortedLabelNumbers.Sort();

            // update the label numbers after sorting
            bool noUpdates = true;
            for (int i = 0; i < labels.Count; i++)
            {
                labels[i].numberOnLabel = sortedLabelNumbers[i];
                if (labels[i].numberOnLabel != labels[i].originalNumberOnLabel)
                 noUpdates = false;

            }
            if (noUpdates)
            {
                MessageBox.Show("Wall Numbers are already in the correct order.", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                app.EndProgressBar();
                app.EndWaitCursor();
                step = 1;
               // EndCommand(true);
                return;
            }
            app.SetProgressBarPosition(50);

            // update the wall numbers

            transactionManager.Begin("Updates");
        //    SelectedFeature = app.DataContext.OpenFeature(FNO_Selected, FID_Selected);

            app.SetProgressBarPosition(55);
            foreach (WallNumberLabel label in labels)
            {
                //SelectedFeature.Components.GetComponent(CNO_Selected).Recordset.MoveFirst();
                //while (!SelectedFeature.Components.GetComponent(CNO_Selected).Recordset.EOF)
                //{
                //    if (SelectedFeature.Components.GetComponent(CNO_Selected).Recordset.Fields[labelNumberField].Value.ToString() == label.originalNumberOnLabel.ToString())
                //    {
                //        SelectedFeature.Components.GetComponent(CNO_Selected).Recordset.Update(labelNumberField, label.numberOnLabel);
                //        break;
                //    }

                //    SelectedFeature.Components.GetComponent(CNO_Selected).Recordset.MoveNext();
                //}
                // update the manhole
                string sql = string.Format("update {6} set {0}={1} where {2}={3} and g3e_fno={4} and g3e_fid={5}",
                     labelNumberField, label.numberOnLabel.ToString(), IDField, label.ID.ToString(), FNO_Selected.ToString(), FID_Selected.ToString(), labelTableName);
                int dataAffected = 0;
                app.DataContext.Execute(sql, out dataAffected, 0, null);

            }

            app.SetProgressBarPosition(70);
            // update the wall numbers in the duct paths
            UpdateDuctPaths(SelectedFeature, labels);
           // transactionManager.Rollback();
            transactionManager.Commit();
            app.SetProgressBarPosition(93);
            transactionManager.RefreshDatabaseChanges();
            app.SetProgressBarPosition(100);
            app.EndProgressBar();
            app.EndWaitCursor();
            step = 1;
        }

        protected override string GetActivationStatusMessage()
        {
            return "Running Wall Number Re-Ordering ...";
        }
        private void UpdateDuctPaths(IGTKeyObject SourceFeature, List<WallNumberLabel> labels)
        {
            List<int> terminatingDuctpaths = null;
            List<int> startingDuctPaths = null;
            Utility.GetDuctpaths(app.DataContext,SourceFeature.FNO, SourceFeature.FID, out startingDuctPaths, out terminatingDuctpaths);
            app.SetProgressBarPosition(75);
            foreach (int ductPathFID in startingDuctPaths)
            {
                IGTKeyObject ductPath = app.DataContext.OpenFeature(FNO_DuctPath, ductPathFID);
                ductPath.Components.GetComponent(2201).Recordset.MoveLast();
                string x = ductPath.Components.GetComponent(2201).Recordset.Fields["DT_MH_FRM_WALL"].Value.ToString();


                foreach (WallNumberLabel label in labels)
                {
                    if (label.originalNumberOnLabel == int.Parse(x))
                    {
                        //int t = label.numberOnLabel;
                        ductPath.Components.GetComponent(2201).Recordset.Update("DT_MH_FRM_WALL", label.numberOnLabel);
                        break;
                    }
                }
            }
            app.SetProgressBarPosition(80);
            foreach (int ductPathFID in terminatingDuctpaths)
            {
                IGTKeyObject ductPath = app.DataContext.OpenFeature(FNO_DuctPath, ductPathFID);
                ductPath.Components.GetComponent(2201).Recordset.MoveLast();
                string x = ductPath.Components.GetComponent(2201).Recordset.Fields["DT_MH_TO_WALL"].Value.ToString();
                foreach (WallNumberLabel label in labels)
                {
                    if (label.originalNumberOnLabel == int.Parse(x))
                    {
                        //  int t = label.numberOnLabel;
                        ductPath.Components.GetComponent(2201).Recordset.Update("DT_MH_TO_WALL", label.numberOnLabel);
                        break;
                    }
                }
                ductPath = null;
            }
            app.SetProgressBarPosition(85);
        }
      
        private bool CheckSeletedFeature()
        {
            if (app.SelectedObjects.FeatureCount == 0 )
            {
                return false;
            }

            if (app.SelectedObjects.FeatureCount > 1)
            {
                MessageBox.Show("Select only one feature at once!", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                app.SelectedObjects.Clear();
                return false;
            }
            foreach (IGTDDCKeyObject selectedObj in app.SelectedObjects.GetObjects())
            {
                if (selectedObj.FNO == 2700 || selectedObj.FNO == 3300 || selectedObj.FNO == 3800)
                {
                    FID_Selected = selectedObj.FID;
                    FNO_Selected = selectedObj.FNO;
                    if (FNO_Selected == 2700)
                    {
                        CNO_Selected = 2732;
                        labelTableName = "GC_MANHLW_T";
                    }

                    else if (FNO_Selected == 3800)
                    {
                        CNO_Selected = 3840;
                        labelTableName = "GC_CHAMBERWALL_T";
                    }
                    else if (FNO_Selected == 3300)
                    {
                        CNO_Selected = 3340;
                        labelTableName = "GC_TUNNELWALL_T";
                    }

                   
                    IGTKeyObject SelectedFeature = app.DataContext.OpenFeature(FNO_Selected, FID_Selected);

                    // get the wall number component
                    IGTComponent wallNumberComponent = SelectedFeature.Components.GetComponent(CNO_Selected);
                    if (wallNumberComponent.Recordset.EOF)
                    {
                        MessageBox.Show("Selected feature does not have Wall Numbers", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    Wall_Selected = 1;// GetWallNumber(false); 
                    
                    return true;
                }
                else
                {
                    MessageBox.Show("Select a Manhole/Chamber/Tunnel only!", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    app.SelectedObjects.Clear();
                    return false;
                }

            }

            app.SelectedObjects.Clear();
            return false;
        }

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
            if (app.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                app.SelectedObjects.Clear();
                return -1;
            }

            if (app.SelectedObjects.FeatureCount > 1)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                app.SelectedObjects.Clear();
                return -1;
            }

            
            string WallNum = "";
            foreach (IGTDDCKeyObject oDDCKeyObject in app.SelectedObjects.GetObjects())
            {
                if (FNO_Selected != oDDCKeyObject.FNO)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    app.SelectedObjects.Clear();
                    return -1;
                }
                if (FID_Selected != oDDCKeyObject.FID)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    app.SelectedObjects.Clear();
                    return -1;
                }

                //manhole
                if (FNO_Selected == 2700)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_MANHLW_T")
                    {
                        WallNum = oDDCKeyObject.Recordset.Fields[7].Value.ToString();
                        break;
                    }
                }
                //chamber
                else if (FNO_Selected == 3800)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_CHAMBERWALL_T")
                    {
                        WallNum = oDDCKeyObject.Recordset.Fields[6].Value.ToString();
                        break;
                    }
                }
                //tunnel/trench
                else if (FNO_Selected == 3300)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_TUNNELWALL_T")
                    {
                        WallNum = oDDCKeyObject.Recordset.Fields[6].Value.ToString();                      
                        break;
                    }
                }
                if(flag)
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + FID_Selected.ToString() + " !", "Wall Number Re-Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                app.SelectedObjects.Clear();
                return -1;

            }
            #endregion

            app.SelectedObjects.Clear();
            return int.Parse(WallNum);
        }
    }
}
