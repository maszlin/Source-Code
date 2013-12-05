using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using System.Drawing;
using System.Windows.Forms;

namespace NEPS.GTechnology.BulkMoveFeatures
{
    class GTMoveAndScale : MoveBase
    {
        // OPERATING MODES
        private enum eMode
        {
            none,
            defineFromArea,
            defineToArea,
            areasDefined
        }
        private eMode currentMode = eMode.none;


        // ON ACTIVATION
        protected override void OnActivate()
        {
            currentMode = eMode.defineFromArea;
            base.OnActivate();
        }

        IGTPolygonGeometry fromArea = null, toArea = null;
        Rectangle fromBoundingRect = null, toBoundingRect = null;

        protected override void OnMouseMove(GTMouseEventArgs args)
        {
            string status = "";
            string coordinates = string.Format("({0:0.0000}, {1:0.0000}", args.WindowPoint.X, args.WorldPoint.Y);
            switch (currentMode)
            {
                case eMode.defineFromArea:
                    if (fromArea != null)
                    {
                        // create a polygon of the currently defined area + the current mouse position
                        IGTPolygonGeometry tempPolygon = (IGTPolygonGeometry)fromArea.Copy();
                        tempPolygon.Points.Add(args.WorldPoint);
                        ShowTemporaryGeometry(tempPolygon, Utility.StyleIDFromArea, 1);
                    }
                    status = "Define area to be moved " + coordinates + ". Double-Click to confirm, ESC to cancel.";
                    break;

                case eMode.defineToArea:
                    if (toArea != null)
                    {
                        // create a polygon of the currently defined area + the current mouse position
                        IGTPolygonGeometry tempPolygon = (IGTPolygonGeometry)toArea.Copy();
                        tempPolygon.Points.Add(args.WorldPoint);
                        ShowTemporaryGeometry(tempPolygon, Utility.StyleIDToArea, 3);
                    }
                    status = "Define target area " + coordinates + ". Double-Click to confirm, ESC to cancel.";
                    break;
                default:
                    break;
            }
            app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, status);

            base.OnMouseMove(args);
        }

        protected override void OnMouseUp(GTMouseEventArgs args)
        {
            if (currentMode == eMode.defineFromArea)
            {
                if (fromArea == null)
                    fromArea = GTClassFactory.Create<IGTPolygonGeometry>();

                fromArea.Points.Add(args.WorldPoint);
            }
            else if (currentMode == eMode.defineToArea)
            {
                if (toArea == null)
                    toArea = GTClassFactory.Create<IGTPolygonGeometry>();

                toArea.Points.Add(args.WorldPoint);

                // the toArea should have only 3 points
                if (toArea.Points.Count == 3)
                {
                    OnFinishedDefiningToArea();
                }
            }

            base.OnMouseUp(args);
        }

        protected override void OnMouseDblClick(GTMouseEventArgs args)
        {
            if (currentMode == eMode.defineFromArea)
            {
                IGTPolygonGeometry rect = Utility.CreateBoundingRect(fromArea, out fromBoundingRect);
                ShowTemporaryGeometry(rect, Utility.StyleIDFromArea, 2);
                currentMode = eMode.defineToArea;
            }
            else if (currentMode == eMode.defineToArea)
            {
                OnFinishedDefiningToArea();
            }
            base.OnMouseDblClick(args);
        }

        private void OnFinishedDefiningToArea()
        {
            IGTPolygonGeometry rect = Utility.CreateBoundingRect(toArea, out toBoundingRect);
            ShowTemporaryGeometry(rect, Utility.StyleIDToArea, 4);
            currentMode = eMode.areasDefined;

            bool result = false;
            if (MessageBox.Show("Are you sure you would like to move the features?", "Confirmation", MessageBoxButtons.OKCancel) ==
                DialogResult.OK)
            {
                result = MoveFeatures();
            }

            editService.RemoveAllGeometries();
            AllPlacementFinished(!result);
        }

        private bool MoveFeatures()
        {

            // find out the features inside the from area
            List<Utility.FeatureItem> featureItems = Utility.GetFeaturesContained(fromArea, app);

            // user can't pick too many features
            if (featureItems.Count > maxSelectableFeatures)
            {
                MessageBox.Show("Sorry, you have selected " + featureItems.Count + " features. " +
                    "The maximum is " + maxSelectableFeatures + ". Please reselect.");
                return false;
            }

            // calculate the scale (toRect divided by fromRect)
            double scaleY = toBoundingRect.Height / fromBoundingRect.Height;
            double scaleX = toBoundingRect.Width / fromBoundingRect.Width;

            // the center of the bounding rects
            Point fromCenter = fromBoundingRect.Center;
            Point toCenter = toBoundingRect.Center;


            int i = 1;
            app.BeginWaitCursor();
            transactionManager.Begin("BulkMove");
            foreach (Utility.FeatureItem featureItem in featureItems)
            {
                app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage,
                    "Moving feature " + i++ + " of " + featureItems.Count);
                IGTKeyObject featureObj = app.DataContext.OpenFeature(featureItem.FNO, featureItem.FID);

                MoveFeature(featureObj, fromArea.Points, scaleX, scaleY, fromCenter, toCenter);
            }
            app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Finished moving.");
            return true;
        }

        private void MoveFeature(IGTKeyObject featureObj, IGTPoints boundaryPolygon, double scaleX, double scaleY, Point fromCenter, Point toCenter)
        {

            // get the polygon of the boundary first
            foreach (IGTComponent component in featureObj.Components)
            {
                if (component.Geometry != null)
                    component.Geometry = Move((IGTGeometry)component.Geometry, boundaryPolygon, scaleX, scaleY, fromCenter, toCenter);

            }

        }

        private IGTGeometry Move(IGTGeometry source, IGTPoints boundaryPolygon, double scaleX, double scaleY, Point fromCenter, Point toCenter)
        {
            if (source is IGTPointGeometry)
                return Move((IGTPointGeometry)source, boundaryPolygon, scaleX, scaleY, fromCenter, toCenter);
            else if (source is IGTLineGeometry)
                return Move((IGTLineGeometry)source, boundaryPolygon, scaleX, scaleY, fromCenter, toCenter);
            else if (source is IGTPolygonGeometry)
                return Move((IGTPolygonGeometry)source, boundaryPolygon, scaleX, scaleY, fromCenter, toCenter);
            else if (source is IGTCompositePolylineGeometry)
                return Move((IGTCompositePolylineGeometry)source, boundaryPolygon, scaleX, scaleY, fromCenter, toCenter);
            else if (source is IGTPolylineGeometry)
                return Move((IGTPolylineGeometry)source, boundaryPolygon, scaleX, scaleY, fromCenter, toCenter);
            else
                return source;
        }

        private IGTGeometry Move(IGTPointGeometry source, IGTPoints boundaryPolygon, double scaleX, double scaleY, Point fromCenter, Point toCenter)
        {
           
            // if it is contained by the boundary, then move it.
            if (Utility.ShouldMove(source.FirstPoint, boundaryPolygon))
            {
                // move it to the center first then to the final location
                IGTVector toCenterDelta = Utility.CalculateDelta(source.FirstPoint, Utility.ToIGTPoint(toCenter));
                IGTVector toFinalDelta = CalculateDeltaFromCenter(scaleX, scaleY, source.FirstPoint.X, source.FirstPoint.Y, fromCenter,toCenter);

                source.Move(toCenterDelta);
                source.Move(toFinalDelta);
            }
            return source;
        }

        private IGTVector CalculateDeltaFromCenter(double scaleX, double scaleY, double x, double y, Point fromCenter, Point toCenter)
        {
            double newX = toCenter.X;
            double newY = toCenter.Y;
            newX += (x - fromCenter.X) * scaleX;
            newY += (y - fromCenter.Y) * scaleY;

            Point newPoint = new Point(newX, newY);

            IGTPoint toPoint = Utility.ToIGTPoint(newPoint);
            IGTPoint centerPoint = Utility.ToIGTPoint(toCenter);
            IGTVector delta = Utility.CalculateDelta(centerPoint, toPoint);

            return delta;
        }

        private IGTGeometry Move(IGTLineGeometry source, IGTPoints boundaryPolygon, double scaleX, double scaleY, 
            Point fromCenter, Point toCenter)
        {

            for (int i = 0; i < source.KeypointCount; i++)
            {
                IGTPoint pt = source.GetKeypointPosition(i);

                bool shouldMove = Utility.ShouldMove(pt, boundaryPolygon);
                if (shouldMove)
                {
                    pt = MovePoint(scaleX, scaleY, fromCenter, toCenter, pt);
                }
            }
            return source;
        }


        private IGTGeometry Move(IGTPolygonGeometry source, IGTPoints boundaryPolygon, double scaleX, double scaleY, Point fromCenter, Point toCenter)
        {
            for (int i = 0; i < source.KeypointCount; i++)
            {
                IGTPoint pt = source.GetKeypointPosition(i);
                bool shouldMove = Utility.ShouldMove(pt, boundaryPolygon);
                if (shouldMove)
                {
                    pt = MovePoint(scaleX, scaleY, fromCenter, toCenter, pt);
                }
            }
            return source; // assign back the geometry that has been moved to the component     

        }

        private IGTGeometry Move(IGTCompositePolylineGeometry source, IGTPoints boundaryPolygon, double scaleX, double scaleY, Point fromCenter, Point toCenter)
        {
            for (int i = 0; i < source.Count; i++)
            {
                IGTGeometry subGeometry = ((IList<IGTGeometry>)source)[i];
                subGeometry = Move(subGeometry, boundaryPolygon, scaleX, scaleY, fromCenter, toCenter);
            }
            return source;

        }

        private IGTGeometry Move(IGTPolylineGeometry source, IGTPoints boundaryPolygon, double scaleX, double scaleY, Point fromCenter, Point toCenter)
        {
            for (int i = 0; i < source.Points.Count; i++)
            {
                IGTPoint pt = source.GetKeypointPosition(i);

                bool shouldMove = Utility.ShouldMove(pt, boundaryPolygon);
                if (shouldMove)
                {
                    pt = MovePoint(scaleX, scaleY, fromCenter, toCenter, pt);
                    source.Points[i] = pt;
                }
            }
            return source;
        }

        private IGTPoint MovePoint(double scaleX, double scaleY, Point fromCenter, Point toCenter, IGTPoint source)
        {
            // move it to the center first then to the final location
            IGTVector toCenterDelta = Utility.CalculateDelta(source, Utility.ToIGTPoint(toCenter));
            IGTVector toFinalDelta = CalculateDeltaFromCenter(scaleX, scaleY, source.X, source.Y, fromCenter, toCenter);

            source = source.AddVectorToPoint(source, toCenterDelta);
            source = source.AddVectorToPoint(source, toFinalDelta);
            return source;
        }








    }
}