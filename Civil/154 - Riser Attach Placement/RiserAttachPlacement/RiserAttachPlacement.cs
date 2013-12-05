using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using Intergraph.GTechnology.Interfaces;
using System.Windows.Forms;

namespace NEPS.GTechnology.RiserAttachPlacement
{
    class GTRiserAttachPlacement : PlacementBase
    {

        // FNO/CNO
        private const int FNO_Pole = 3000;
        private const int FNO_RiserAttach = 3900;
        private const int CNO_Label = 3920;
        private const int CNO_LeaderLine = 3930;
        private const int RNO_Owns = 2;
        private const int RNO_OwnedBy = 4;

        // Features
        private IGTKeyObject riserAttach = null;
        private IGTComponent leaderLine = null;


        protected override string GetActivationStatusMessage()
        {
            return "Please place a Riser";
        }

        protected override void OnActivate()
        {
            IGTKeyObject selectedPole = ForcePreselectedFeature(FNO_Pole, "Pole");
            if (selectedPole != null)
            {
                riserAttach = app.DataContext.NewFeature(FNO_RiserAttach, true);
                CreateRelationship(selectedPole, riserAttach, RNO_Owns);
                StartFeatureExplorer(riserAttach);
            }
            else
            {
                AllPlacementFinished(true);
            }          
        }
        
        protected override void OnFinishedExplorer(bool canceled)
        {
            if (!canceled)
                StartFeaturePlacement(riserAttach);
            else
                base.OnFinishedExplorer(canceled);

        }

        protected override void OnFinishedPlacement(IGTKeyObjects placed, IGTKeyObjects skipped)
        {
            if (FindFeature(placed, riserAttach) != null)
            {
                IGTKeyObject placedFeature = FindFeature(placed, riserAttach);
                switch (placedFeature.CNO)
                {
                    case CNO_Label:
                        StartComponentPlacement(riserAttach, CNO_LeaderLine);
                        break;
                    case CNO_LeaderLine:
                        AllPlacementFinished(false);
                        break;
                }
            }
        }

        protected override void OnCancelComponentPlacement(short CNO)
        {
            if (CNO == CNO_LeaderLine)
                AllPlacementFinished(false);
        }

        protected override void FinishSelectingObject(IGTKeyObject selectedObject)
        {
            if (selectedObject == null)
                AllPlacementFinished(true);
            else
            {
                if (selectedObject.FNO == FNO_Pole)
                    CreateRelationship(selectedObject, riserAttach, RNO_Owns);
            }
        }

       
    }
}
