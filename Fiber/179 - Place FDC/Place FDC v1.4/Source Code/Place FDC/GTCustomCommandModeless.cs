using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.PlaceFDC
{
    #region WinWrapperClass
    public class WinWrapper : System.Windows.Forms.IWin32Window
    {        

        #region IWin32Window Members

        IntPtr IWin32Window.Handle
        {
            get 
            {
                System.IntPtr iptr = new System.IntPtr();
                iptr = (System.IntPtr)GTCustomCommandModeless.application.hWnd;
                //iptr = CType(application.hWnd, System.IntPtr);
                return iptr;
            }
        }

        #endregion
    }
    #endregion

    class GTCustomCommandModeless : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        public static Intergraph.GTechnology.API.IGTApplication application = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper = null;

        bool closestatus = false;
        public static WinWrapper objWinWrapper = new WinWrapper();

      
        public EventHandler<GTFinishedEventArgs> EV_placementService_Finished;
        public EventHandler<GTMouseEventArgs> EV_CommandHelper_MouseUp;
        public EventHandler<GTKeyEventArgs> EV_CommandHelper_KeyUp;

        public IGTPoint oPoint = null;
        public bool bBeginSelect;
        GTWindowsForm_PlaceFDC m_CustomForm = null;

        #region activate
        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                if (application == null) application = GTClassFactory.Create<IGTApplication>();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Placement FDC...");
                
                m_oGTCustomCommandHelper = CustomCommandHelper;

               

                // command helper events
                EV_CommandHelper_MouseUp = new EventHandler<GTMouseEventArgs>(CommandHelper_MouseUp);
                EV_CommandHelper_KeyUp = new EventHandler<GTKeyEventArgs>(CommandHelper_KeyUp);
                CustomCommandHelper.MouseUp += EV_CommandHelper_MouseUp;
                CustomCommandHelper.KeyUp += EV_CommandHelper_KeyUp;

                m_CustomForm = new GTWindowsForm_PlaceFDC();
                m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
                this.bBeginSelect = true;
                m_CustomForm.Show(objWinWrapper);
                // placement events

                m_CustomForm.placementService = GTClassFactory.Create<IGTFeaturePlacementService>(m_oGTCustomCommandHelper);
                EV_placementService_Finished = new EventHandler<GTFinishedEventArgs>(placementService_Finished);
               m_CustomForm.placementService.Finished += EV_placementService_Finished;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error); ExitCmd(); }


        }
        
        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oGTTransactionManager = value;
            }
        }

        #endregion

        #region event
        void CommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            OnMouseUp(e);
        }

        void CommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            OnKeyUp(e);
        }

        protected virtual void OnMouseUp(GTMouseEventArgs args)
        {
            
        }

        protected virtual void OnKeyUp(GTKeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case (short)Keys.Escape:
                    bool bCanceled = m_CustomForm.placementService.CancelPlacement(true);
                 
                    if (bCanceled)
                    {                        
                            EndCommand(true);                      
                    }
                    break;
            }
        }
        #endregion

        #region placement service events

        private void placementService_Finished(object sender, GTFinishedEventArgs e)
        {
            EndCommand(false);
        }

        protected void EndCommand(bool canceled)
        {
          //  placementService.d
            try
            {
                if (canceled)
                {
                    m_CustomForm.placementService.Dispose();
                    m_CustomForm.placementService.Finished -= EV_placementService_Finished;
                    // GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                    ExitCmd();
                }
                else
                {
                    if (m_CustomForm.componentBeingPlaced == 5120)
                    {
                        m_CustomForm.placementService.StartComponent(m_CustomForm.mobjFDCAttribute, 5130); //.StartFeature(mobjFDCAttribute);
                        m_CustomForm.componentBeingPlaced = 5130;
                    }
                    else
                    {
                        m_CustomForm.placementService.Dispose();
                        m_CustomForm.placementService.Finished -= EV_placementService_Finished;

                        m_CustomForm.DrawFDC(5100);
                        application.SetProgressBarPosition(100);
                        ExitCmd();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error); ExitCmd(); }
        }
        #endregion

        #region custom cmd closed event
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(!closestatus)
                ExitCmd();
        }

        #endregion

        #region termination
        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Place FDC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                    //  return true;
                }
               
                return false;

            }
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }

        public void Terminate()
        {
            try
            {

               
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region exit cmd

        public void ExitCmd()
        {
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Exiting...");
           
            if (m_oGTTransactionManager != null)
            {
                if (m_oGTTransactionManager.TransactionInProgress)
                    m_oGTTransactionManager.Rollback();
                m_oGTTransactionManager = null;
            }
            if (application != null)
            {
                application = null;
            }          
            
            if (objWinWrapper != null)
            {
                objWinWrapper = null;
            }
            if (m_CustomForm != null)
            {
                m_CustomForm.placementService = null;
                closestatus = true;
                m_CustomForm.Close();
                m_CustomForm = null;
                closestatus = false;
            }
            

             

            
            m_oGTCustomCommandHelper.Complete();

            //if (m_oGTCustomCommandHelper != null)
            //    m_oGTCustomCommandHelper = null;
    

        }

        #endregion

 
    }
}
