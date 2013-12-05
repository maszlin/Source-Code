using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using Intergraph.GTechnology.Interfaces;
using System.Windows.Forms;

namespace NEPS.GTechnology.WallNumberReorder
{
    class PlacementBase : IGTCustomCommandModeless
    {
        protected IGTApplication app = GTClassFactory.Create<IGTApplication>();
        protected IGTCustomCommandHelper commandHelper = null;
        protected IGTTransactionManager transactionManager = null;

        
        private EventHandler<GTMouseEventArgs>EV_CommandHelper_MouseUp;
        private EventHandler<GTKeyEventArgs> EV_CommandHelper_KeyUp;
        private EventHandler<GTMouseEventArgs> EV_CommandHelper_MouseMove;
        
        public void Activate(IGTCustomCommandHelper CustomCommandHelper)
        {
            app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage,
                GetActivationStatusMessage());

            app.BeginWaitCursor();

            commandHelper = CustomCommandHelper;

                     

            // command helper events
            EV_CommandHelper_MouseUp = new EventHandler<GTMouseEventArgs>(CommandHelper_MouseUp);
            EV_CommandHelper_KeyUp = new EventHandler<GTKeyEventArgs>(CommandHelper_KeyUp);
            EV_CommandHelper_MouseMove = new EventHandler<GTMouseEventArgs>(CommandHelper_MouseMove);
            CustomCommandHelper.MouseUp += EV_CommandHelper_MouseUp;
            CustomCommandHelper.KeyUp += EV_CommandHelper_KeyUp;
            CustomCommandHelper.MouseMove += EV_CommandHelper_MouseMove;
            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                app.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
            }
            OnActivate();

            app.EndWaitCursor();
        }

        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Wall Number Re-Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    CleanUp();
                    app.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, GetCancelledMessage());
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
            StepChange(true);
        }

        public void Resume()
        {
            StepChange(false);
        }

        public void Terminate()
        {
          
        }

        #region EventHandlers

        void CommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            OnKeyUp(e);
        }


        void CommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            OnMouseUp(e);
        }

        void CommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            OnMouseMove(e);
        }


        #endregion

        #region Methods
     
        protected void EndCommand(bool canceled)
        {
            
            if (canceled)
            {
                if (transactionManager.TransactionInProgress)
                    transactionManager.Rollback();
            }
            else
            {
                if (transactionManager.TransactionInProgress)
                {
                    transactionManager.Commit();
                    transactionManager.RefreshDatabaseChanges();
                }
            }
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
            if (transactionManager != null)
            {
                if (transactionManager.TransactionInProgress)
                    transactionManager.Rollback();
                transactionManager = null;
            }
            
            
            commandHelper.MouseUp -= EV_CommandHelper_MouseUp;
            commandHelper.KeyUp -= EV_CommandHelper_KeyUp;
            commandHelper.MouseMove -= EV_CommandHelper_MouseMove;
            commandHelper.Complete();
            app.SelectedObjects.Clear();
            app.EndWaitCursor();
            app.EndProgressBar();

        }

        public IGTTransactionManager TransactionManager
        {
            set { transactionManager = value; }
        }  
       

        #endregion

        #region virtual functions        
       

        protected virtual void OnActivate()
        {

        }

        protected virtual string GetFinishedMessage()
        {
            return "Completed.";
        }

        protected virtual string GetActivationStatusMessage()
        {
            return "Started";
        }

      

        protected virtual string GetCancelledMessage()
        {
            return "Cancelled.";
        }

        protected virtual void OnMouseUp(GTMouseEventArgs args)
        {
           
        }
        protected virtual void OnMouseMove(GTMouseEventArgs args)
        {

        }

        protected virtual void OnKeyUp(GTKeyEventArgs args)
        {
        }

        protected virtual void StepChange(bool pause)
        {
        }
        #endregion
    }
}
