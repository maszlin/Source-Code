using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using System.Windows.Forms;

namespace NEPS.GTechnology.BulkMoveFeatures
{
    class MoveBase:PlacementBase
    {
        protected const int maxSelectableFeatures = 100;
        protected IGTGeometryEditService editService = null;

        protected override void OnActivate()
        {
            // create edit service
            editService = GTClassFactory.Create<IGTGeometryEditService>();
            editService.TargetMapWindow = app.ActiveMapWindow;
            base.OnActivate();
        }

        protected void RemoveTemporaryGeometry(int index)
        {
            if (editService.GeometryCount >= index)
                editService.RemoveGeometry(index);
        }

        protected void ShowTemporaryGeometry(IGTGeometry toShow, int styleID, int index)
        {
            // remove existing polygon then add the geometry
            RemoveTemporaryGeometry(index);
            editService.AddGeometry(toShow, styleID);
        }

        protected void ShowTemporaryGeometry(IGTGeometry toShow, int styleID)
        {
            editService.AddGeometry(toShow, styleID);
        }

        protected override void OnKeyUp(GTKeyEventArgs args)
        {
            // if user presses ESC
            if (args.KeyCode == 27)
            {

                if (MessageBox.Show("Would you like to cancel the move?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    editService.RemoveAllGeometries();
                    AllPlacementFinished(true);
                    return;
                }

            }

            base.OnKeyUp(args);
        }

       

      


       
    }
}
