using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using System.Windows.Forms;
using ADODB;

namespace NEPS.GTechnology.BulkMoveFeatures
{
    class GTBulkMoveFeatures : MoveBase
    {

        // OPERATING MODES
        protected enum eMode
        {
            none,
            defineBoundary,
            selectPoint1,
            selectPoint2,
            pointsSelected
        }
        protected eMode currentMode = eMode.none;


        // ON ACTIVATION
        protected override void OnActivate()
        {
            currentMode = eMode.defineBoundary;
            base.OnActivate();
        }

        protected override void OnMouseMove(GTMouseEventArgs args)
        {
            string status = "";
            string coordinates = string.Format("({0:0.0000}, {1:0.0000}", args.WindowPoint.X, args.WorldPoint.Y);
            switch (currentMode)
            {
                case eMode.defineBoundary:
                    if (moveArea != null)
                    {
                        // create a polygon of the currently defined area + the current mouse position
                        IGTPolygonGeometry tempPolygon = (IGTPolygonGeometry)moveArea.Copy();
                        tempPolygon.Points.Add(args.WorldPoint);
                        ShowTemporaryGeometry(tempPolygon, Utility.StyleIDFromArea, 1);
                    }
                    status = StatusBarMessage_DefineBoundary() + " " + coordinates + ".";
                    break;
                case eMode.selectPoint1:
                    pointFrom = args.WorldPoint;
                    status = "FROM point " + coordinates + ". Click to confirm, ESC to cancel.";
                    break;
                case eMode.selectPoint2:

                    // remove from-to line and target area
                    RemoveTemporaryGeometry(3);
                    RemoveTemporaryGeometry(2);

                    // show from - to line
                    IGTPolylineGeometry line = GTClassFactory.Create<IGTPolylineGeometry>();
                    line.Points.Add(pointFrom);
                    line.Points.Add(args.WorldPoint);
                    ShowTemporaryGeometry(line, Utility.StyleIDToArea);

                    // show moved target area
                    pointTo = args.WorldPoint;
                    IGTPolygonGeometry target = (IGTPolygonGeometry)moveArea.Copy();
                    IGTVector delta = Utility.CalculateDelta(pointFrom, pointTo);
                    target.Move(delta);
                    ShowTemporaryGeometry(target, Utility.StyleIDToArea);

                    status = "TO point " + coordinates + ". Click to confirm, ESC to cancel.";
                    break;
                case eMode.pointsSelected:
                    break;
                default:
                    break;
            }
            app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, status);

            base.OnMouseMove(args);
        }

        protected virtual string StatusBarMessage_DefineBoundary()
        {
            return "Define area to be moved. Double-Click to confirm, ESC to cancel.";
        }


        protected void AllPlacementFinished(bool canceled)
        {
            editService.RemoveAllGeometries();
            base.AllPlacementFinished(canceled);
        }



        // ON MOUSE UP
        protected IGTPoint pointFrom = null;
        protected IGTPoint pointTo = null;
        protected IGTPolygonGeometry moveArea = null;
        protected override void OnMouseUp(GTMouseEventArgs args)
        {
            switch (currentMode)
            {
                case eMode.none:
                    break;
                case eMode.defineBoundary:
                    OnMouseUp_DefineBoundary(args);
                    break;
                case eMode.selectPoint1:
                    pointFrom = args.WorldPoint;
                    currentMode = eMode.selectPoint2;
                    break;
                case eMode.selectPoint2:
                    OnMouseUp_SelectPoint2(args);
                    break;
                case eMode.pointsSelected:
                    break;
                default:
                    break;
            }


            base.OnMouseUp(args);
        }

        protected virtual void OnMouseUp_DefineBoundary(GTMouseEventArgs args)
        {
            if (moveArea == null)
                moveArea = GTClassFactory.Create<IGTPolygonGeometry>();

            moveArea.Points.Add(args.WorldPoint);
        }

        private void OnMouseUp_SelectPoint2(GTMouseEventArgs args)
        {
            if (MessageBox.Show("Are you sure you would like to move the features?", "Confirmation", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                pointTo = args.WorldPoint;
                app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Moving features now...");
                currentMode = eMode.pointsSelected;

                // remove the source polygon and the line from the display
                RemoveTemporaryGeometry(2);
                RemoveTemporaryGeometry(1);

                // now perform the actual move
                IGTVector delta = Utility.CalculateDelta(pointFrom, pointTo);

                if (!MoveFeatures(delta))
                    MessageBox.Show("Move canceled.");

            }

            InitiateCurrentMode();
        }

        private void InitiateCurrentMode()
        {
            editService.RemoveAllGeometries();
            moveArea = null;
            pointFrom = null;
            pointTo = null;
            currentMode = eMode.defineBoundary;
        }



        protected override void OnMouseDblClick(GTMouseEventArgs args)
        {
            if (currentMode == eMode.defineBoundary)
            {
                currentMode = eMode.selectPoint1;
            }
            base.OnMouseDblClick(args);
        }

        protected virtual bool MoveFeatures(IGTVector delta)
        {
            List<Utility.FeatureItem> featureItems = Utility.GetFeaturesContained(moveArea, app);

            // user can't pick too many features
            if (featureItems.Count > maxSelectableFeatures)
            {
                MessageBox.Show("Sorry, you have selected " + featureItems.Count + " features. " +
                    "The maximum is " + maxSelectableFeatures + ". Please reselect.");
                return false;
            }

            int i = 1;
            app.BeginWaitCursor();
            transactionManager.Begin("BulkMove");
            foreach (Utility.FeatureItem featureItem in featureItems)
            {
                // do the standard movement
                app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage,
                    "Moving feature " + i++ + " of " + featureItems.Count);
                IGTKeyObject featureObj = app.DataContext.OpenFeature(featureItem.FNO, featureItem.FID);
                MoveFeature(false, featureObj, delta, moveArea);
            }
            transactionManager.Commit();

            app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Finished moving.");
            return true;
        }

        protected void MoveFeature(bool moveEntire, IGTKeyObject featureObj, IGTVector delta, IGTPolygonGeometry moveArea)
        {
            // get the polygon of the boundary first
            foreach (IGTComponent component in featureObj.Components)
            {
                try
                {
                    Recordset rs = component.Recordset;
                    if (rs.RecordCount == 0)
                        continue;

                    rs.MoveFirst();
                    if (component.Geometry != null)
                    {
                        while (!rs.EOF)
                        {
                            component.Geometry = Move(moveEntire, component.Geometry, delta,
                                moveArea.Points, app.DataContext);
                            rs.MoveNext();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }

            // realign / move components which did not get moved because they are not inside the boundary
            // or require special calculations to move
            switch (featureObj.FNO)
            {
                case Utility.FNO_Cable:
                    RotateAndCenterLabel(featureObj, Utility.CNO_CableGeoLabel, Utility.CNO_CableGeoline);
                    break;

                case Utility.FNO_Ductpath:
                    MoveAnnotationLabel(featureObj, Utility.CNO_DuctPathGeoLabel, delta);
                    break;

                case Utility.FNO_FiberDSideCable:
                    RotateAndCenterLabel(featureObj, Utility.CNO_FiberDSideGeoLabel, Utility.CNO_FiberDSideGeoline);
                    break;

                case Utility.FNO_FiberESideCable:
                    RotateAndCenterLabel(featureObj, Utility.CNO_FiberESideGeoLabel, Utility.CNO_FiberESideGeoline);
                    break;


                default:
                    break;
            }




        }

        private void MoveAnnotationLabel(IGTKeyObject featureObj, short labelCNO, IGTVector delta)
        {
            IGTComponent compLabel = featureObj.Components.GetComponent(labelCNO);
            if (compLabel == null)
                return;

            if (compLabel.Geometry == null)
                return;

            IGTOrientedPointGeometry labelGeometry = (IGTOrientedPointGeometry)Move(true,
                compLabel.Geometry, delta, null, app.DataContext);
            compLabel.Geometry = labelGeometry;

        }

        protected void RotateAndCenterLabel(IGTKeyObject cable, short CNOLabel, short CNOLine)
        {
            IGTComponent compLabel = cable.Components.GetComponent(CNOLabel);
            IGTComponent compLine = cable.Components.GetComponent(CNOLine);

            if (compLabel == null || compLine == null)
                return;


            if (compLabel.Recordset.RecordCount > 0)
                compLabel.Recordset.MoveFirst();
            else
                return;
            if (compLine.Recordset.RecordCount > 0)
                compLine.Recordset.MoveFirst();
            else
                return;

            if (compLabel.Geometry == null)
                return;

            // get the composite polyline from the geometry
            if (compLine.Geometry == null)
                return;
            IGTCompositePolylineGeometry composite = (IGTCompositePolylineGeometry)compLine.Geometry;

            // extract the polyline from the composite
            IGTGeometry subGeometry = ((IList<IGTGeometry>)composite)[0];
            IGTPolylineGeometry referenceLine = (IGTPolylineGeometry)subGeometry;

            // calculate the angle between the two points of the reference line
            // and the middle position
            bool useLastPointPair = cable.FNO == Utility.FNO_FiberDSideCable || cable.FNO == Utility.FNO_FiberESideCable;
            IGTVector lineAngle = Utility.GetLineAngle(referenceLine, useLastPointPair);
            IGTPoint lineMiddlePosition = Utility.GetMiddlePosition(referenceLine, 0.3f,
                useLastPointPair);

            // calculate how much to move the label based on its current position
            // and the middle position. I.e. move it to the middle position
            IGTVector labelMove = Utility.CalculateDelta((IGTOrientedPointGeometry)compLabel.Geometry,
                lineMiddlePosition);

            // do the move
            IGTOrientedPointGeometry labelGeometry = (IGTOrientedPointGeometry)Move(true,
                compLabel.Geometry, labelMove, null, app.DataContext);
            labelGeometry.Orientation = lineAngle; // apply the rotation separately
            compLabel.Geometry = labelGeometry; // assign back to the original geometry of the label now that it has been transformed
        }


        private IGTPointGeometry Move(bool moveEntire, IGTPointGeometry source, IGTVector delta, IGTPoints boundaryPolygon, IGTDataContext dc)
        {

            source.Move(delta);

            return source;
        }



        private IGTLineGeometry Move(bool moveEntire, IGTLineGeometry source, IGTVector delta, IGTPoints boundaryPolygon, IGTDataContext dc)
        {
            if (moveEntire)
            {
                source.Move(delta);
                return source;
            }

            for (int i = 0; i < source.KeypointCount; i++)
            {
                IGTPoint pt = source.GetKeypointPosition(i);
                bool shouldMove = Utility.ShouldMove(pt, boundaryPolygon);
                if (shouldMove)
                    pt = pt.AddVectorToPoint(pt, delta);
            }
            return source;

        }

        private IGTPolylineGeometry Move(bool moveEntire, IGTPolylineGeometry source, IGTVector delta, IGTPoints boundaryPolygon, IGTDataContext dc)
        {
            if (moveEntire)
            {
                source.Move(delta);
                return source;
            }

            for (int i = 0; i < source.Points.Count; i++)
            {
                IGTPoint pt = source.GetKeypointPosition(i);

                bool shouldMove = Utility.ShouldMove(pt, boundaryPolygon);
                if (shouldMove)
                    pt = pt.AddVectorToPoint(pt, delta);
                source.Points[i] = pt;
            }
            return source;

        }

        protected IGTGeometry Move(bool moveEntire, IGTGeometry g, IGTVector delta, IGTPoints polygonBoundary, IGTDataContext dc)
        {
            if (g is IGTPointGeometry)
                return Move(moveEntire, (IGTPointGeometry)g, delta, polygonBoundary, dc);
            else if (g is IGTLineGeometry)
                return Move(moveEntire, (IGTLineGeometry)g, delta, polygonBoundary, dc);
            else if (g is IGTPolygonGeometry)
                return Move(moveEntire, (IGTPolygonGeometry)g, delta, polygonBoundary, dc);
            else if (g is IGTCompositePolylineGeometry)
                return Move(moveEntire, (IGTCompositePolylineGeometry)g, delta, polygonBoundary, dc);
            else if (g is IGTPolylineGeometry)
                return Move(moveEntire, (IGTPolylineGeometry)g, delta, polygonBoundary, dc);
            else
                return g;

        }




        private IGTCompositePolylineGeometry Move(bool moveEntire, IGTCompositePolylineGeometry source, IGTVector delta, IGTPoints boundaryPolygon, IGTDataContext dc)
        {
            for (int i = 0; i < source.Count; i++)
            {
                IGTGeometry subGeometry = ((IList<IGTGeometry>)source)[i];
                subGeometry = Move(moveEntire, subGeometry, delta, boundaryPolygon, dc);
            }
            return source;
        }


        private IGTPolygonGeometry Move(bool moveEntire, IGTPolygonGeometry source, IGTVector delta, IGTPoints boundaryPolygon, IGTDataContext dc)
        {
            for (int i = 0; i < source.KeypointCount; i++)
            {
                IGTPoint pt = source.GetKeypointPosition(i);

                bool shouldMove = Utility.ShouldMove(pt, boundaryPolygon);
                if (moveEntire || shouldMove)
                    pt = pt.AddVectorToPoint(pt, delta);
            }
            return source; // assign back the geometry that has been moved to the component     

        }



    }
}
