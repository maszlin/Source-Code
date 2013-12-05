using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using System.Windows.Forms;
using ADODB;

namespace NEPS.GTechnology.BulkMoveFeatures
{
    class GTMoveAssociatedFeatures : GTBulkMoveFeatures
    {

        IGTKeyObject mainFeature = null; // the feature which the user is trying to move
        protected override void OnMouseUp_DefineBoundary(GTMouseEventArgs args)
        {
            IGTLocateService ls = app.ActiveMapWindow.LocateService;
            IGTDDCKeyObjects ddcObjects = ls.Locate(args.WorldPoint, 10, 10, GTSelectionTypeConstants.gtmwstSelectSingle);
            foreach (IGTDDCKeyObject ddcObj in ddcObjects)
            {
                if (IsMoveable(ddcObj.FNO))
                    mainFeature = app.DataContext.OpenFeature(ddcObj.FNO, ddcObj.FID);
            }

            if (mainFeature == null)
            {
                MessageBox.Show("Please click on a moveable feature.");
                return;
            }

            OnMainFeatureSelected();
        }

        private bool IsMoveable(short FNO)
        {
            // skip the static features
            foreach (short staticFNO in Utility.StaticFNOs)
            {
                if (FNO == staticFNO)
                    return false;
            }

            Dictionary<short, Utility.MoveableFeature> moveableFeatures = Utility.GetMoveableFeatures();
            if (moveableFeatures.ContainsKey(FNO))
                return true;

            return false;
        }

        private bool IsMoveable(short mainFeatureFNO, short movingFeatureFNO)
        {
            Dictionary<short, Utility.MoveableFeature> moveableFeatures = Utility.GetMoveableFeatures();
            if (!moveableFeatures.ContainsKey(mainFeatureFNO))
                return false;

            Utility.MoveableFeature main = moveableFeatures[mainFeatureFNO];
            foreach (short associatedFNO in main.AssociatedFNOs)
            {
                if (associatedFNO == movingFeatureFNO)
                    return true;
            }
            return false;
        }

        private void OnMainFeatureSelected()
        {
            double pad = DetermineBoundaryRadius(mainFeature.FNO);
            IGTPoint refPoint = null;

            refPoint = GetReferencePoint(mainFeature);

            // create a boundary around the main feature
            // which is used to include the associated features.
            moveArea = GTClassFactory.Create<IGTPolygonGeometry>();
            Utility.AddPoint(moveArea, refPoint.X - pad, refPoint.Y + pad);
            Utility.AddPoint(moveArea, refPoint.X + pad, refPoint.Y + pad);
            Utility.AddPoint(moveArea, refPoint.X + pad, refPoint.Y - pad);
            Utility.AddPoint(moveArea, refPoint.X - pad, refPoint.Y - pad);

            pointFrom = refPoint;
            ShowTemporaryGeometry(moveArea, Utility.StyleIDFromArea);
            currentMode = eMode.selectPoint2;
        }

        private double DetermineBoundaryRadius(short FNO)
        {
            switch (FNO)
            {
                case Utility.FNO_Cabinet:
                    return 6.0d;

                case Utility.FNO_FiberDistributionCabinet:
                    return 12.0d;

                case Utility.FNO_Manhole:
                    return 3.0d;

                case Utility.FNO_DP:
                    return 3.0d;

                default:
                    return 2.0d;
            }

        }

        /// <summary>
        /// This could be enhanced to handle certain features with specific logic. It should ideally return the
        /// 'center point' of the feature.
        /// </summary>
        /// <param name="toMove"></param>
        /// <returns></returns>
        private IGTPoint GetReferencePoint(IGTKeyObject toMove)
        {
            foreach (IGTComponent c in toMove.Components)
            {
                IGTGeometry g = c.Geometry;
                if (g != null)
                {
                    return g.FirstPoint;
                }
            }
            return null;
        }

        protected override string StatusBarMessage_DefineBoundary()
        {
            return "Select the feature you want to move. ESC to cancel.";
        }

        protected List<IGTKeyObject> moveEntireFeatures = new List<IGTKeyObject>();
        protected override bool MoveFeatures(IGTVector delta)
        {

            // main feature should be moved entirely
            moveEntireFeatures.Add(mainFeature);

            // to add .. other features which need to be moved entirely

            int i = 1;
            app.BeginWaitCursor();
            transactionManager.Begin("BulkMove");

            // find all features inside the move area and move them
            List<Utility.FeatureItem> containedFeatures = Utility.GetFeaturesContained(moveArea, app);
            containedFeatures = FilterAssociatedFeatures(containedFeatures);

            // add features which are contained in features, etc.
            containedFeatures = QueryForAdditionalFeatures(mainFeature, containedFeatures);

            // removed due to error being reported
            // containedFeatures = RemoveUnrelatedFeatures(mainFeature, containedFeatures);

            // some feature types need to be moved entirely even if they are composite polylines
            foreach (Utility.FeatureItem item in containedFeatures)
            {
                switch (item.FNO)
                {
                    case Utility.FNO_Duct:
                    case Utility.FNO_DuctNest:
                        {
                            IGTKeyObject obj = app.DataContext.OpenFeature(item.FNO, item.FID);
                            moveEntireFeatures.Add(obj);
                        }
                        break;
                }
            }

            int count=0;
            foreach (Utility.FeatureItem item in containedFeatures)
            {
                app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Moving " + count++ + " out of " + containedFeatures.Count);

                // should the feature be moved entirely?
                IGTKeyObject feature = Utility.FindByFID(moveEntireFeatures, item.FID);
                if (feature != null)
                {
                    MoveFeature(true, feature, delta, moveArea);
                }
                else
                {
                    IGTKeyObject featureObj = app.DataContext.OpenFeature(item.FNO, item.FID);
                    MoveFeature(false, featureObj, delta, moveArea);
                }
            }

            // if main feature is a DP, realign the cable on the perimeter of the DP.
            if (mainFeature.FNO == Utility.FNO_DP ||
                mainFeature.FNO == Utility.FNO_FiberDP)
            {
                // find a cable inside the boundary, and which point of the cable is touching the DP.
                IGTKeyObject objCable = null;
                foreach (Utility.FeatureItem feature in containedFeatures)
                {
                    switch (feature.FNO)
                    {
                        case Utility.FNO_Cable:
                            RecenterCable(delta, feature.FNO, feature.FID, Utility.CNO_CableGeoLabel, Utility.CNO_CableGeoline);
                            break;

                        case Utility.FNO_FiberDSideCable:
                            RecenterCable(delta, feature.FNO, feature.FID, Utility.CNO_FiberDSideGeoLabel, Utility.CNO_FiberDSideGeoline);
                            break;

                        case Utility.FNO_FiberESideCable:
                            RecenterCable(delta, feature.FNO, feature.FID, Utility.CNO_FiberESideGeoLabel, Utility.CNO_FiberESideGeoline);
                            break;

                    }

                }

            }
            transactionManager.Commit();


            return true;
        }

        private List<Utility.FeatureItem> RemoveUnrelatedFeatures(IGTKeyObject mainFeature, List<Utility.FeatureItem> containedFeatures)
        {
            List<Utility.FeatureItem> output = new List<Utility.FeatureItem>();

            // duct paths
            List<IGTKeyObject> connectedDuctPaths = GetConnectedDuctPaths(mainFeature.FID);
            foreach (Utility.FeatureItem item in containedFeatures)
            {
                if (item.FNO == Utility.FNO_Ductpath)
                {
                    if (Utility.FindByFID(connectedDuctPaths, item.FID) != null)
                        output.Add(item);
                }
                else
                    output.Add(item);
            }

            return output;
            
        }

        private List<Utility.FeatureItem> QueryForAdditionalFeatures(IGTKeyObject mainFeature, List<Utility.FeatureItem> originalList)
        {
            IGTRelationshipService svc = GTClassFactory.Create<IGTRelationshipService>();
            svc.DataContext = app.DataContext;
              
            List<IGTKeyObject> additionalObjects = new List<IGTKeyObject>();

            // for manhole, get all associated ductnests
            if (mainFeature.FNO == Utility.FNO_Manhole)
            {
                List<IGTKeyObject> ductPaths = GetConnectedDuctPaths(mainFeature.FID);
                foreach (IGTKeyObject ductPath in ductPaths)
                {
                    if (ductPath.FNO == Utility.FNO_Ductpath)
                    {
                        AddContainedFeatures(ductPath, additionalObjects, svc);
                    }
                }
            }

            //foreach (Utility.FeatureItem item in originalList)
            //{
            //    if (item.FNO == Utility.FNO_Ductpath)
            //    {
            //        IGTKeyObject obj = app.DataContext.OpenFeature(item.FNO, item.FID);
            //        AddContainedFeatures(obj, additionalObjects, svc);
            //    }
            //}

            foreach (IGTKeyObject additional in additionalObjects)
            {
                if (Utility.FindByFID(originalList, additional.FID) == null)
                {
                    Utility.FeatureItem item = new Utility.FeatureItem();
                    item.FNO = additional.FNO;
                    item.FID = additional.FID;
                    originalList.Add(item);
                }
            }

            return originalList;
        }

        private List<IGTKeyObject> GetConnectedDuctPaths(int nodeFID)
        {
            List<IGTKeyObject> output = new List<IGTKeyObject>();
            string sql = string.Format("SELECT * FROM gc_cond where dt_nd_frm_id={0} OR dt_nd_to_id={0}", nodeFID);

            int count = 0;
            Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();
                while (!rs.EOF)
                {
                    IGTKeyObject obj = app.DataContext.OpenFeature(
                        Convert.ToInt16(rs.Fields["G3E_FNO"].Value),
                        Convert.ToInt32(rs.Fields["G3E_FID"].Value));
                    output.Add(obj);
                    rs.MoveNext();
                }
            }

            return output;
        }

        private void AddContainedFeatures(IGTKeyObject parentFeature, List<IGTKeyObject> additionalObjects, IGTRelationshipService svc)
        {
            svc.ActiveFeature = parentFeature;

            try
            {
                IGTKeyObjects containedObjects = svc.GetRelatedFeatures(Utility.RNO_Contains);
                foreach (IGTKeyObject containedObj in containedObjects)
                {
                    if (Utility.FindByFID(additionalObjects, containedObj.FID) == null)
                        additionalObjects.Add(containedObj);

                    AddContainedFeatures(containedObj, additionalObjects, svc);
                }
            }
            catch (Exception)
            {
               
            }
            
        }


        private List<Utility.FeatureItem> FilterAssociatedFeatures(List<Utility.FeatureItem> source)
        {
            List<Utility.FeatureItem> output = new List<Utility.FeatureItem>();
            foreach (Utility.FeatureItem item in source)
            {
                if (item.FNO == mainFeature.FNO ||
                    IsMoveable(mainFeature.FNO, item.FNO))
                    output.Add(item);
            }


            return output;
        }

        private void RecenterCable(IGTVector delta, short cableFNO, int cableFID, short CNOLabel, short CNOGeoLine)
        {
            IGTKeyObject cable = app.DataContext.OpenFeature(cableFNO, cableFID);

            // get the point inside the polyline which is inside the boundary
            IGTComponent compCableGeoLine = cable.Components.GetComponent(CNOGeoLine);
            IGTCompositePolylineGeometry composite = (IGTCompositePolylineGeometry)compCableGeoLine.Geometry;
            IGTGeometry subGeometry = ((IList<IGTGeometry>)composite)[0];
            IGTPolylineGeometry polyLine = (IGTPolylineGeometry)subGeometry;

            // get the oriented point of the center component (this is usually the label complnent of the reference feature)
            IGTComponent comp = mainFeature.Components.GetComponent(Utility.CNO_FiberDPLabel);
            if (comp == null)
                comp = mainFeature.Components.GetComponent(Utility.CNO_DPLabel);
            IGTOrientedPointGeometry centerPoint_Oriented =
                (IGTOrientedPointGeometry)comp.Geometry;
            IGTPoint centerPoint = centerPoint_Oriented.FirstPoint;

            // project the original movearea with the offset
            IGTPolygonGeometry moveToArea = (IGTPolygonGeometry)moveArea.Copy();
            moveToArea.Move(delta);

            int iPoint = 0;
            foreach (IGTPoint pt in polyLine.Points)
            {
                if (Utility.IsPointInPolygon(pt, moveToArea.Points))
                {
                    // find the reference point to calculate angle
                    // use the end point which is not the same as the point being moved
                    IGTPoint sourcePoint = polyLine.FirstPoint;
                    if (sourcePoint == pt)
                        sourcePoint = polyLine.LastPoint;

                    // get the angle between the point and the center of the DP
                    double angle = Utility.degreeBetweenPoints(centerPoint, sourcePoint);
                    const double radius = 1.9d;
                    IGTPoint finalPos = Utility.GetPointOnCircle(angle, radius, centerPoint);

                    IGTVector finalMove = Utility.CalculateDelta(pt, finalPos);
                    polyLine.Points[iPoint] = pt.AddVectorToPoint(pt, finalMove); ;
                }

                iPoint++;
            }

            compCableGeoLine.Geometry = composite;

            RotateAndCenterLabel(cable, CNOLabel, CNOGeoLine);
        }




    }
}
