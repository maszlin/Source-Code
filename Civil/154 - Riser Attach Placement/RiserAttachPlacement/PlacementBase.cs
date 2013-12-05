using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using Intergraph.GTechnology.Interfaces;
using System.Windows.Forms;

namespace NEPS.GTechnology.RiserAttachPlacement
{
    class PlacementBase : IGTCustomCommandModeless
    {
        protected IGTApplication app = GTClassFactory.Create<IGTApplication>();
        protected IGTCustomCommandHelper commandHelper = null;
        protected IGTFeaturePlacementService placementService = null;
        protected IGTFeatureExplorerService featureExplorerService = null;
        protected IGTTransactionManager transactionManager = null;

        // used during placement
        protected IGTKeyObject featureBeingPlaced = null;
        protected short? componentBeingPlaced = null;
        private EventHandler<GTFinishedEventArgs> EV_placementService_Finished;
        private EventHandler<GTMouseEventArgs>EV_CommandHelper_MouseUp;
        private EventHandler<GTKeyEventArgs> EV_CommandHelper_KeyUp;
        private EventHandler EV_Explorer_SaveClick;
        private EventHandler EV_Explorer_CancelClick;

        // used during forced user selection
        protected short? selectFeatureFNO;
        protected string selectFeatureName;
        protected EventHandler EV_app_SelectedObjectsChanges;
      

        public void Activate(IGTCustomCommandHelper CustomCommandHelper)
        {
            app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage,
                GetActivationStatusMessage());

            app.BeginWaitCursor();

            commandHelper = CustomCommandHelper;

            // placement events
            placementService = GTClassFactory.Create<IGTFeaturePlacementService>(CustomCommandHelper);
            EV_placementService_Finished = new EventHandler<GTFinishedEventArgs>(placementService_Finished);
            placementService.Finished += EV_placementService_Finished;

            // feature explorer events
            featureExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();
            EV_Explorer_CancelClick = new EventHandler(Explorer_CancelClick);
            EV_Explorer_SaveClick = new EventHandler(Explorer_SaveClick);
            featureExplorerService.SaveClick += EV_Explorer_SaveClick;
            featureExplorerService.CancelClick += EV_Explorer_CancelClick;

            // command helper events
            EV_CommandHelper_MouseUp = new EventHandler<GTMouseEventArgs>(CommandHelper_MouseUp);
            EV_CommandHelper_KeyUp = new EventHandler<GTKeyEventArgs>(CommandHelper_KeyUp);
            CustomCommandHelper.MouseUp += EV_CommandHelper_MouseUp;
            CustomCommandHelper.KeyUp += EV_CommandHelper_KeyUp;

            transactionManager.Begin("Feature Placement");
            OnActivate();

            app.EndWaitCursor();
        }

        public bool CanTerminate
        {
            get { return true; }
        }

        public void Pause()
        {

        }

        public void Resume()
        {

        }

        public void Terminate()
        {
            CleanUp();
            app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, GetCancelledMessage());
        }

        #region EventHandlers

        private void app_SelectedObjectsChanged(object sender, EventArgs e)
        {
            if (app.SelectedObjects.FeatureCount != 1)
                return;

            if (selectFeatureFNO == null)
                return;
         
            IGTKeyObject desiredObject = null;
            IGTSelectedObjects selection = app.SelectedObjects;
            IGTDDCKeyObjects selectedObjects = selection.GetObjects();
            foreach (IGTDDCKeyObject selectedObj in selectedObjects)
            {
                if (selectedObj.FNO == selectFeatureFNO)
                {
                    desiredObject = app.DataContext.OpenFeature(selectedObj.FNO, selectedObj.FID);
                    app.SelectedObjectsChanged -= EV_app_SelectedObjectsChanges;
                    selectFeatureFNO = null;
                    break;
                }
            }

            if (desiredObject == null)
                app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please select a " + selectFeatureName);
            else
            {
                FinishSelectingObject(desiredObject);

            }

        }

        void CommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            OnKeyUp(e);
        }


        void CommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            OnMouseUp(e);
        }

        void Explorer_CancelClick(object sender, EventArgs e)
        {
            OnFinishedExplorer(true);
        }

        void Explorer_SaveClick(object sender, EventArgs e)
        {
            OnFinishedExplorer(false);
        }    

        void placementService_Finished(object sender, GTFinishedEventArgs e)
        {
            OnFinishedPlacement(e.ComponentsPlaced, e.ComponentsSkipped);
        }

        #endregion

        #region Methods

        protected IGTKeyObject ForcePreselectedFeature(short desiredFeatureFNO, string desiredFeatureName)
        {
            IGTKeyObject selectedFeature = null;
            foreach (IGTDDCKeyObject selectedObj in app.SelectedObjects.GetObjects())
            {
                if (selectedObj.FNO == desiredFeatureFNO)
                {
                    selectedFeature = app.DataContext.OpenFeature(selectedObj.FNO, selectedObj.FID);
                    app.SelectedObjectsChanged -= EV_app_SelectedObjectsChanges;
                    selectFeatureFNO = null;
                    break;
                }
            }

            if (selectedFeature == null)
                MessageBox.Show("You must select a " + desiredFeatureName + " first");

            return selectedFeature;
        }

        protected void CreateRelationship(IGTKeyObject activeFeature, IGTKeyObject relatedFeature, short RNO)
        {
            IGTRelationshipService relationshipService = 
                GTClassFactory.Create<IGTRelationshipService>(this.commandHelper);
            relationshipService.DataContext = app.DataContext;
            relationshipService.ActiveFeature = activeFeature;
            if (relationshipService.AllowSilentEstablish(relatedFeature))
                relationshipService.SilentEstablish(RNO, relatedFeature);
        }

        protected void ForceSelectFeature(short FNO, string featureName)
        {
            selectFeatureFNO = FNO;
            selectFeatureName = "Pole";

            EV_app_SelectedObjectsChanges = new EventHandler(app_SelectedObjectsChanged);
            app.SelectedObjectsChanged += EV_app_SelectedObjectsChanges;

            MessageBox.Show("Please select a " + selectFeatureName);
        }

     
        protected void AllPlacementFinished(bool canceled)
        {
            if (canceled)
            {
                transactionManager.Rollback();
            }
            else
            {
                transactionManager.Commit(true);
            }
         
            commandHelper.Complete();
            CleanUp();

            if (canceled)
            {
                app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, GetCancelledMessage());
            }
            else
            {
                app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, GetFinishedMessage());
            }
        }

        private void CleanUp()
        {
            placementService.Dispose();
            featureExplorerService.SaveClick -= EV_Explorer_SaveClick;
            featureExplorerService.CancelClick -= EV_Explorer_CancelClick;
            commandHelper.MouseUp -= EV_CommandHelper_MouseUp;
            commandHelper.KeyUp -= EV_CommandHelper_KeyUp;
            placementService.Finished -= EV_placementService_Finished;
        }

        public IGTTransactionManager TransactionManager
        {
            set { transactionManager = value; }
        }  

        protected void StartFeatureExplorer(IGTKeyObject feature)
        {
            app.BeginWaitCursor();
            featureExplorerService.ExploreFeature(feature, "Placement");
            featureExplorerService.Visible = true;
            featureExplorerService.Slide(true);
            app.EndWaitCursor();
        }

        protected IGTKeyObject StartFeaturePlacement(short FNO)
        {
            IGTKeyObject output = app.DataContext.NewFeature(FNO, true);
            StartFeaturePlacement(output);
            return output;
        }

        protected void StartFeaturePlacement(IGTKeyObject feature)
        {
            featureBeingPlaced = feature;
            placementService.StartFeature(feature);
        }

        protected void StartComponentPlacement(IGTKeyObject feature, short CNO)
        {
            placementService.StartComponent(feature, CNO);
            componentBeingPlaced = CNO;
        }

        protected IGTKeyObject FindFeature(IGTKeyObjects source, IGTKeyObject feature)
        {
            foreach (IGTKeyObject find in source)
            {
                if (find.FID == feature.FID)
                    return find;
            }
            return null;
        }

        #endregion

        #region virtual functions

        protected virtual void FinishSelectingObject(IGTKeyObject selectedObject)
        {

        }

        protected virtual void OnFinishedPlacement(IGTKeyObjects iGTKeyObjects, IGTKeyObjects iGTKeyObjects_2)
        {

        }

        protected virtual void OnActivate()
        {

        }

        protected virtual string GetFinishedMessage()
        {
            return "Placement completed.";
        }

        protected virtual string GetActivationStatusMessage()
        {
            return "Placement Started";
        }

        protected virtual void OnFinishedExplorer(bool canceled)
        {
            if (canceled)
                AllPlacementFinished(canceled);

            featureExplorerService.Slide(false);
            featureExplorerService.Visible = false;
        }

        protected virtual string GetCancelledMessage()
        {
            return "Placement cancelled.";
        }

        protected virtual void OnMouseUp(GTMouseEventArgs args)
        {

        }

        protected virtual void OnKeyUp(GTKeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case (short)Keys.Escape:
                    bool bCanceled = placementService.CancelPlacement(true);
                    if (bCanceled)
                    {
                        // canceling component placement
                        if (componentBeingPlaced != null)
                        {
                            OnCancelComponentPlacement(componentBeingPlaced.Value);
                        }
                        else if (featureBeingPlaced != null)
                        {
                            // cancel placement if escape pressed during feature placement
                            AllPlacementFinished(true);
                        }
                    }
                    break;
            }
            
        }

        protected virtual void OnCancelComponentPlacement(short CNO)
        {

        }

        #endregion
    }
}
