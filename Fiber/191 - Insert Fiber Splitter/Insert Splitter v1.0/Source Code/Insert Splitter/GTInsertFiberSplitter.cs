using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace AG.GTechnology.InsertFiberSplitter
{


    class GTInsertFiberSplitter : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        public static Intergraph.GTechnology.API.IGTApplication application = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper = null;

        bool closestatus = false;
        public static bool selectparent = false;
      

        GTWindowsForm_InsertSplitter m_CustomForm = null;

        #region activate
        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                if (application == null) application = GTClassFactory.Create<IGTApplication>();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Insert Splitter(s)...");
                
                m_oGTCustomCommandHelper = CustomCommandHelper;

               

                m_CustomForm = new GTWindowsForm_InsertSplitter();
                m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);

                if (application.SelectedObjects.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in application.SelectedObjects.GetObjects())
                    {
                        if (!m_CustomForm.SelectOwner(oDDCKeyObject.FID, oDDCKeyObject.FNO))
                        {
                            application.SelectedObjects.Clear();
                            
                        }
                        break;
                    }
                }

                    m_CustomForm.Show();

               }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Error); ExitCmd(); }


        }
        
        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oGTTransactionManager = value;
            }
        }

        #endregion

        #region mouse event
        void CommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {

           
        }

        void CommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
           
          
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
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Add Splitter(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            
          
            if (m_CustomForm != null)
            {
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
