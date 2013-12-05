using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.CopyPole
{
    class GTCopyCivilPole : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        public static Intergraph.GTechnology.API.IGTDataContext m_IGTDataContext = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;
        public static Intergraph.GTechnology.API.IGTRelationshipService mobjRelationshipService = null;
        IGTLocateService mobjLocateService;
        public static int startplc=0;
            public static int SourceFeatureFID = 0;
            public static double SourceFeatureX = 0;
            public static double SourceFeatureY = 0;
            public static int CopiedFeatureFID = 0;
            public static double CopiedFeatureX = 0;
            public static double CopiedFeatureY = 0;
            public static IGTVector CopiedFeatureOrientation = null;
            public static short AssociateFNO = 0;
            public static int AssociateFID = 0;
        public static bool NonDuctPath = false;
        public static int PoleStyleId = 0;
       // public static int NonDuctPathStyleId = 0;
        public static Distance DistForm = null;
        public static bool FormClosing = false;
        public static bool Precision = false;

        public static Intergraph.GTechnology.API.IGTFeatureExplorerService mobjExplorerService =null;
        #endregion

        #region Mouse Click
        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            try
            {

                IGTPoint WorldPoint = e.WorldPoint;

                if (e.Button != 2)//left button
                {
                    if (startplc == 1 && !Precision)//moving position of pole symbol in NON-precision mode
                    {

                        IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        tempp.X = e.WorldPoint.X;
                        tempp.Y = e.WorldPoint.Y;
                        tempp.Z = 0.0;
                        temp.Origin = tempp;
                        if (mobjEditService.GeometryCount > 0)
                            mobjEditService.RemoveAllGeometries();
                        CopiedFeatureX = tempp.X;
                        CopiedFeatureY = tempp.Y;                       

                        mobjEditService.AddGeometry(temp, PoleStyleId);

                        startplc = 2;
                        DistForm.MessageHelpChange(3);
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete Copying! Press ESC to change position.");
                        return;
                    } 
                    if (startplc == 1 && Precision)//moving position of pole symbol in precision mode
                    {

                        IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        IGTPoint tempp = PointBasedOnEnteredLength(double.Parse(DistForm.txtDistance.Value.ToString()),
                                     SourceFeatureX, SourceFeatureY, e.WorldPoint.X, e.WorldPoint.Y);
                        temp.Origin = tempp;
                        if (mobjEditService.GeometryCount > 0)
                            mobjEditService.RemoveAllGeometries();
                        CopiedFeatureX = tempp.X;
                        CopiedFeatureY = tempp.Y;
                        

                        mobjEditService.AddGeometry(temp, PoleStyleId);

                        startplc = 2;
                        DistForm.MessageHelpChange(3);
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete Copying! Press ESC to change position.");
                        return;
                    }
                    if (startplc == 2)//confirm rotation of pole symbol
                    {
                        IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        tempp.X = e.WorldPoint.X;
                        tempp.Y = e.WorldPoint.Y;
                        tempp.Z = 0.0;
                        IGTPoint tempp2 = GTClassFactory.Create<IGTPoint>();
                        tempp2.X = CopiedFeatureX;
                        tempp2.Y = CopiedFeatureY;
                        tempp2.Z = 0.0;
                        IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                        //  Orientation.BuildVector(tempp, tempp2);
                        temp.Origin = tempp2;
                        temp.Orientation = Orientation.BuildVector(tempp, tempp2);
                       if( CopiedFeatureOrientation==null)
                           CopiedFeatureOrientation = GTClassFactory.Create<IGTVector>();

                       CopiedFeatureOrientation = Orientation.BuildVector(tempp, tempp2);

                        if (mobjEditService.GeometryCount > 0)
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.AddGeometry(temp, PoleStyleId);

                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete Copying! Press ESC to change position.");
                        
                        DistForm.MessageHelpChange(3);
                        startplc = 5;
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                DistForm.Hide();
                MessageBox.Show(ex.Message, "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        } 
        #endregion

        #region Mouse Move
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
           // ExitCmd();
            try
            {
              //  m_gtapp.ActiveMapWindow.Activate();
              //  DistForm.Update();
              //  DistForm.Refresh();
             //  DistForm.
               // DistForm.Hide();
                //DistForm.Show();
             //   DistForm.
                m_gtapp.ActiveMapWindow.Activate();
               // DistForm.WindowState
                //m_gtapp.ActiveMapWindow.
                if (DistForm == null)
                {
                    DistForm = new Distance();
                    DistForm.Show();
                    DistForm.FormClosing += new FormClosingEventHandler(Distance_FormClosing);
                    DistForm.KeyUp += new KeyEventHandler(DistForm_KeyUp);
                    m_gtapp.ActiveMapWindow.Activate();
                }

                if (startplc == 1 && !Precision)//moving position of pole symbol in non-precision mode
                {
                    DistForm.MessageHelpChange(1);                    
                    IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;
                    temp.Origin = tempp;
                    if (mobjEditService.GeometryCount > 0)
                        mobjEditService.RemoveAllGeometries();                   
                   

                    mobjEditService.AddGeometry(temp, PoleStyleId);

                    int distance = LengthBtwTwoPoints(SourceFeatureX, SourceFeatureY, e.WorldPoint.X, e.WorldPoint.Y);
                    DistForm.txtDistance.Value = distance;
                    string addTostatus = " Distance  -  " + distance.ToString() + "m";
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location! Press ESC to exit." + addTostatus);
                    
                    return;
                }
                if (startplc == 1 && Precision)//moving position of pole symbol in precision mode
                {
                    DistForm.MessageHelpChange(1); 
                                     
                    IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    IGTPoint tempp = PointBasedOnEnteredLength(double.Parse(DistForm.txtDistance.Value.ToString()),
                                     SourceFeatureX, SourceFeatureY, e.WorldPoint.X, e.WorldPoint.Y);
                    temp.Origin = tempp;
                    if (mobjEditService.GeometryCount > 0)
                        mobjEditService.RemoveAllGeometries();

                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                        IGTPoint tempp2 = GTClassFactory.Create<IGTPoint>();
                        tempp2.X = SourceFeatureX;
                        tempp2.Y = SourceFeatureY;
                        tempp2.Z = 0.0;

                        oLineGeom.Points.Add(tempp2);
                        oLineGeom.Points.Add(tempp);
                        mobjEditService.AddGeometry(oLineGeom, 2410007);
                    
                    mobjEditService.AddGeometry(temp, PoleStyleId);
                   
                    string addTostatus = " Distance  -  " + DistForm.txtDistance.Value.ToString() + "m";
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location! Press ESC to exit." + addTostatus);

                    return;
                }

                if (startplc == 2)//rotating of pole symbol 
                {
                    DistForm.MessageHelpChange(2); 
                    IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;

                    IGTPoint tempp2 = GTClassFactory.Create<IGTPoint>();
                    tempp2.X = CopiedFeatureX;
                    tempp2.Y = CopiedFeatureY;
                    tempp2.Z = 0.0;
                    IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                    temp.Origin = tempp2;
                    if (mobjEditService.GeometryCount > 0)
                        mobjEditService.RemoveAllGeometries();

                    if (Precision)
                    {
                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                        IGTPoint tempp3 = GTClassFactory.Create<IGTPoint>();
                        tempp3.X = SourceFeatureX;
                        tempp3.Y = SourceFeatureY;
                        tempp3.Z = 0.0;

                        oLineGeom.Points.Add(tempp3);
                        oLineGeom.Points.Add(tempp2);
                        mobjEditService.AddGeometry(oLineGeom, 2410007);
                    }

                    mobjEditService.AddGeometry(temp, PoleStyleId);
                    temp.Orientation = Orientation.BuildVector(tempp, tempp2);
                    mobjEditService.AddGeometry(temp, PoleStyleId);
                    string addTostatus = " Distance  -  " + DistForm.txtDistance.Value.ToString() + "m";
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm rotation! Press ESC to change location." + addTostatus);

                    return;
                }
                if (startplc == 10) //select source Pole feature to start copying
                {
                    DistForm.MessageHelpChange(0); 
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Source Pole feature! Press ESC to exit."); 
                    
                    if (m_gtapp.SelectedObjects.FeatureCount == 1)
                    {
                        IGTGeometry geom = null;
                        short iFNO = 0;
                        int iFID = 0;

                        foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                        {
                            iFNO = oDDCKeyObject.FNO;
                            iFID = oDDCKeyObject.FID;
                            geom = oDDCKeyObject.Geometry;
                           
                            if (iFNO != 3000)
                            {
                                DistForm.Hide();
                                MessageBox.Show("Please select a Pole!", "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                DistForm.Show();
                                m_gtapp.ActiveMapWindow.Activate();
                                m_gtapp.SelectedObjects.Clear();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Source Pole feature! Press ESC to exit."); 
                                return;
                            }
                            if (geom.Type == "OrientedPointGeometry")
                                break;
                        }
                        SourceFeatureFID = iFID;
                        SourceFeatureX = geom.FirstPoint.X;
                        SourceFeatureY = geom.FirstPoint.Y;
                        string type = Get_Value("select POLE_TYPE from GC_POLE where G3E_FID=" + SourceFeatureFID);
                        string styletmp = Get_Value("select  MIN(G3E_SNO) from G3E_STYLERULE where G3E_SRNO=302001 " +
                            " and upper(G3E_FILTER) like upper('%PPF%') " +
                            " and upper(G3E_FILTER) like upper('%" + type + "%') ");
                        
                        if (styletmp == "")
                            PoleStyleId = 3020002;
                        else PoleStyleId = int.Parse(styletmp);
               
                        startplc = 1;
                        DistForm.MessageHelpChange(1); 
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confrim location! Press ESC to exit.");  

                    }
                    else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                    {
                        DistForm.Hide();
                        MessageBox.Show("Please select only one Pole at once!", "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DistForm.Show();
                        m_gtapp.ActiveMapWindow.Activate();
                        m_gtapp.SelectedObjects.Clear();
                        DistForm.MessageHelpChange(0);
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source Pole feature! Press ESC to exit."); 
                        return;
                    }
                    return;
                }

                if (startplc == 15)// select DP/FDP feature to associate with pole
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DP/FDP feature! Press ESC to cancel");
                    DistForm.MessageHelpChange(4);
                    if (m_gtapp.SelectedObjects.FeatureCount == 1)
                    {
                        short iFNO = 0;
                        int iFID = 0;

                        foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                        {
                            iFNO = oDDCKeyObject.FNO;
                            iFID = oDDCKeyObject.FID;

                            if (iFNO != 13000 && iFNO != 5600)
                            {
                                DistForm.Hide();
                                MessageBox.Show("Please select a DP/FDP!", "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                DistForm.Show();
                                m_gtapp.ActiveMapWindow.Activate();
                                m_gtapp.SelectedObjects.Clear();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DP/FDP feature! Press ESC to cancel.");
                                return;
                            }
                            break;
                          }
                          Messages frmMsg = new Messages();
                          frmMsg.Message(2);
                          frmMsg.Show();
                        AssociateFNO = iFNO;
                        AssociateFID = iFID;
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, associating in process...");
                        if (AssociateWithDP())
                        {

                            frmMsg.Message(3);
                            AssociateFNO = 0;
                            AssociateFID = 0;
                            startplc = 1;
                            frmMsg.Close();
                        }
                        frmMsg.Close();
                        m_gtapp.SelectedObjects.Clear();
                        DistForm.Show();
                        m_gtapp.ActiveMapWindow.Activate();
                    }
                    else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                    {
                        DistForm.Hide();
                        MessageBox.Show("Please select only one DP/FDP at once!", "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DistForm.Show();
                        m_gtapp.ActiveMapWindow.Activate();
                        m_gtapp.SelectedObjects.Clear();
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DP/FDP feature! Press ESC to cancel.");
                        return;
                    }
                    return;
                }

                if (startplc == 5)//update status bar while waiting for final confirmation for placement
                {
                    DistForm.MessageHelpChange(3); 
                    int distance = LengthBtwTwoPoints(SourceFeatureX, SourceFeatureY, CopiedFeatureX, CopiedFeatureY);
                    DistForm.txtDistance.Value = distance;
                    string addTostatus = " Distance  -  " + distance.ToString() + "m";
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to finished Copying! Press ESC to change location." + addTostatus);
                    return;
                }
            }
            catch (Exception ex)
            {
                DistForm.Hide();
                MessageBox.Show(ex.Message, "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }

       
        #endregion

        #region Press key ESC and TAB
        void m_oIGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            try
            {

                m_gtapp.ActiveMapWindow.Activate();
                if (e.KeyCode == 27)//button ESC
                {

                    if (startplc == 2 || startplc == 5)//start moving pole symbol again, 2- while choosing rotation, 5- while waiting for final placement confirmation
                    {
                      
                        DistForm.MessageHelpChange(1);
                        startplc = 1;
                        return;
                    }

                    if (startplc == 10 || startplc == 1)//exiting while moving pole symbol or before selected source pole feature
                    {
                        if (DistForm != null)
                        DistForm.Hide();
                       DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Copy Pole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                       if (retVal == DialogResult.Yes)
                       {
                           ExitCmd();
                           return;
                       }
                       if (DistForm != null)
                           DistForm.Show();
                       else
                       {
                           DistForm = new Distance();
                           DistForm.Show();
                       }
                       m_gtapp.ActiveMapWindow.Activate();
                    }

                    if (startplc == 15)//cancel association with DP/FDP
                    {
                        AssociateFNO = 0;
                        AssociateFID = 0;
                        startplc = 1;
                        DistForm.MessageHelpChange(1);
                        DistForm.Show();
                        m_gtapp.ActiveMapWindow.Activate();
                        return;
                    }

                }

                //if(e.KeyCode == 9)//button TAB
                //{
                //    if (startplc == 1)//switch With/Without Non-Duct Path mode while moving pole symbol
                //    {
                //        if (NonDuctPath)
                //            NonDuctPath = false;//without
                //        else  NonDuctPath = true;//with
                //    }

                //    if (startplc == 5)//switch With/Without Non-Duct Path mode while waiting for final placement confirmation
                //    {
                //        if (NonDuctPath)
                //            NonDuctPath = false;
                //        else NonDuctPath = true;

                //        IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                //        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                //        tempp.X = CopiedFeatureX;
                //        tempp.Y = CopiedFeatureY;
                //        tempp.Z = 0.0;
                //        temp.Origin = tempp;
                //        if (mobjEditService.GeometryCount > 0)
                //            mobjEditService.RemoveAllGeometries();
                //        if (CopiedFeatureOrientation != null)
                //            temp.Orientation = CopiedFeatureOrientation;
                //        if (NonDuctPath)
                //        {
                //            IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                //            IGTPoint tempp2 = GTClassFactory.Create<IGTPoint>();
                //            tempp2.X = SourceFeatureX;
                //            tempp2.Y = SourceFeatureY;
                //            tempp2.Z = 0.0;

                //            oLineGeom.Points.Add(tempp2);
                //            oLineGeom.Points.Add(tempp);
                //            mobjEditService.AddGeometry(oLineGeom, NonDuctPathStyleId);
                //        }
                //        mobjEditService.AddGeometry(temp, PoleStyleId);
                //    }
                //}
            }
            catch (Exception ex)
            {
                DistForm.Hide();
                MessageBox.Show(ex.Message, "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }

        }

        #endregion

        #region Mouse DOuble Click
        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {

            try
            {
                if (startplc == 5)//final confirmation for placement
                {
                    startplc = 0;
                    DistForm.Hide();
                    DialogResult retVal = MessageBox.Show("Do you want to place Non-Duct Path between Source and Copied Poles?", "Copy Pole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        NonDuctPath = true;
                    }


                    Messages frmMsg = new Messages();
                    frmMsg.Message(1);
                    frmMsg.Show();

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, copying in process...");
                    m_gtapp.SetProgressBarRange(0, 100);
                    m_gtapp.SetProgressBarPosition(50);

                    if (!CopyPole())//create pole and non-duct path features
                    {
                        frmMsg.Close();
                        ExitCmd();
                    }
                    frmMsg.Message(3);
                    frmMsg.Close();
                    SourceFeatureFID = CopiedFeatureFID;
                    SourceFeatureX = CopiedFeatureX;
                    SourceFeatureY = CopiedFeatureY;
                    CopiedFeatureFID = 0;
                    CopiedFeatureX = 0;
                    CopiedFeatureY = 0;
                    CopiedFeatureOrientation=null;
                    if (mobjEditService.GeometryCount > 0)
                        mobjEditService.RemoveAllGeometries();
                    m_gtapp.SetProgressBarPosition(50);
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Copying successfully completed!");
                    m_gtapp.SetProgressBarRange(0, 0);
                    NonDuctPath = false;
                    retVal = MessageBox.Show("Do you want to associate Pole with DP/FDP?", "Copy Pole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        startplc = 15;
                        DistForm.MessageHelpChange(4);
                        DistForm.Show();
                        m_gtapp.ActiveMapWindow.Activate();
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select DP/FDP feature! Press ESC to cancel");
                    }
                    else
                    {
                        AssociateFNO = 0;
                        AssociateFID = 0;
                        startplc = 1;
                        DistForm.MessageHelpChange(1);
                        DistForm.Show();
                        m_gtapp.ActiveMapWindow.Activate();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                DistForm.Hide();
                MessageBox.Show(ex.Message, "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
           
        }
        #endregion

        #region unusing events
        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
           // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }       

        void m_oIGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
           // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");            
        }

        void m_oIGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
          //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }      

        void m_oIGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
           // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            //
        }

        void m_oIGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
          // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oIGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
         //   GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {

          //  if (e.KeyCode == 27 || e.KeyCode == 9)//button ESC
          //   { m_gtapp.ActiveMapWindow.Activate(); }
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }

        #endregion

        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                m_gtapp = GTClassFactory.Create<IGTApplication>();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Copy Pole . . . ");
                m_oIGTCustomCommandHelper = CustomCommandHelper;

                mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();
                mobjExplorerService.CancelClick += new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick += new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick += new EventHandler(mobjExplorerService_SaveClick);

                mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>();
                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;
                m_IGTDataContext = m_gtapp.DataContext;
                mobjRelationshipService.DataContext = m_IGTDataContext;
                
                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
                }

                mobjLocateService = m_gtapp.ActiveMapWindow.LocateService;
                SubscribeEvents();
                //m_gtapp.ActiveMapWindow.
                //m_oIGTTransactionManager.Begin("OpenExplorer");
                //IGTKeyObject mobjPole = m_IGTDataContext.NewFeature(3000);//OpenFeature(3000,);//application.DataContext.NewFeature(iManholeFNO, true);
                //mobjExplorerService.ExploreFeature(mobjPole, "Pole");
                //mobjExplorerService.Visible = true;
                //mobjExplorerService.Slide(true);                
                startplc = 10;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Source Pole feature! Press ESC to exit.");
             }
            catch (Exception ex)
            {
               // m_oIGTTransactionManager.Rollback();
                DistForm.Hide();
                MessageBox.Show(ex.Message, "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();                
            }
        }
      

        public bool CanTerminate
        {
            get
            {
                return true;
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

                if (m_oIGTTransactionManager != null)
                {
                    m_oIGTTransactionManager = null;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oIGTTransactionManager = value;
            }
        }

        #endregion
      
        #region subscribe/unsubscribe events
        public void SubscribeEvents()
        {
            // Subscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
          //  m_oIGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oIGTCustomCommandHelper_Activate);
          //  m_oIGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oIGTCustomCommandHelper_Deactivate);
          //  m_oIGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oIGTCustomCommandHelper_GainedFocus);
         //   m_oIGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oIGTCustomCommandHelper_LostFocus);
            m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
         //   m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
          //  m_oIGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oIGTCustomCommandHelper_KeyPress);
          //  m_oIGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_Click);
            m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);
            m_oIGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseMove);
         //   m_oIGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseDown);
            m_oIGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseUp);
         //   m_oIGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oIGTCustomCommandHelper_WheelRotate);
        }
        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
          //  m_oIGTCustomCommandHelper.Activate -= m_oIGTCustomCommandHelper_Activate;
          //  m_oIGTCustomCommandHelper.Deactivate -= m_oIGTCustomCommandHelper_Deactivate;
          //  m_oIGTCustomCommandHelper.GainedFocus -= m_oIGTCustomCommandHelper_GainedFocus;
          //  m_oIGTCustomCommandHelper.LostFocus -= m_oIGTCustomCommandHelper_LostFocus;
            m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
          //  m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
          //  m_oIGTCustomCommandHelper.KeyPress -= m_oIGTCustomCommandHelper_KeyPress;
          //  m_oIGTCustomCommandHelper.Click -= m_oIGTCustomCommandHelper_Click;
            m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
          //  m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
         //   m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }
        #endregion
        
        #region Exit CustomCommand
        public void ExitCmd()
        {
         //   m_oIGTTransactionManager.Rollback();
            m_gtapp.SetProgressBarRange(0, 0);
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting..."); 
            m_gtapp.SelectedObjects.Clear();
            NonDuctPath = false;
            UnsubscribeEvents();
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
            }
            FormClosing = true;
            if (DistForm != null)
            {
                DistForm.Close();
                DistForm = null;
            }
            FormClosing = false;
            mobjRelationshipService = null;
            SourceFeatureFID = 0;
            SourceFeatureX = 0;
            SourceFeatureY = 0;
            CopiedFeatureFID = 0;
            CopiedFeatureX = 0;
            CopiedFeatureY = 0;
            if(CopiedFeatureOrientation!=null)
                CopiedFeatureOrientation = null;
            AssociateFNO = 0;
            AssociateFID = 0;
            NonDuctPath = false;
            PoleStyleId = 0;
          //  NonDuctPathStyleId = 0;
            FormClosing = false;
            Precision = false;
            if (mobjExplorerService != null)
            {
                mobjExplorerService.Clear();
                mobjExplorerService = null;
            }
            m_oIGTCustomCommandHelper.Complete();

        }
        #endregion

        #region Distance Form events
        private void Distance_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormClosing)
            {
                DistForm.Hide();
                DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Copy Pole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                    return;
                }

                e.Cancel = true;
                if (DistForm != null)
                    DistForm.Show();
            }
            m_gtapp.ActiveMapWindow.Activate();

        }

        private void DistForm_KeyUp(object sender, KeyEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            if (e.KeyCode == Keys.Escape)//button ESC
            {
               if (startplc == 2 || startplc == 5)//start moving pole symbol again, 2- while choosing rotation, 5- while waiting for final placement confirmation
                {

                    DistForm.MessageHelpChange(1);
                    startplc = 1;
                    return;
                }

                if (startplc == 10 || startplc == 1)//exiting while moving pole symbol or before selected source pole feature
                {
                    if (DistForm != null)
                        DistForm.Hide();
                    DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Copy Pole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        ExitCmd();
                        return;
                    }
                    if (DistForm != null)
                        DistForm.Show();
                    else
                    {
                        DistForm = new Distance();
                        DistForm.Show();
                    }
                    m_gtapp.ActiveMapWindow.Activate();
                }

                if (startplc == 15)//cancel association with DP/FDP
                {
                    AssociateFNO = 0;
                    AssociateFID = 0;
                    startplc = 1;
                    DistForm.MessageHelpChange(1);
                    DistForm.Show();
                    m_gtapp.ActiveMapWindow.Activate();
                    return;
                }
            }
        }

        #endregion

        #region Copy Pole
        private bool CopyPole()
        {
            try
            {

                m_oIGTTransactionManager.Rollback();
                short iCNO;

                short iFNO = 3000;      //Pole
                int iFID;
                // progressBar1.Value = 5;

                m_oIGTTransactionManager.Begin("CopyPole");

                IGTKeyObject oCopyFeature = m_IGTDataContext.OpenFeature(iFNO, SourceFeatureFID);
                IGTKeyObject oNewFeature =  m_IGTDataContext.NewFeature(iFNO);
                 iFID = oNewFeature.FID;
                 CopiedFeatureFID = iFID;
                //newFID = iFID;

                iCNO = 51; //GC_NETELEM
                CopyAttributes(oCopyFeature.Components.GetComponent(iCNO).Recordset, oNewFeature.Components.GetComponent(iCNO).Recordset);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");

                iCNO = 3001; // Attribute
                CopyAttributes(oCopyFeature.Components.GetComponent(iCNO).Recordset, oNewFeature.Components.GetComponent(iCNO).Recordset);
                
                //Geo Symbol Geometry
                iCNO = 3020;

                IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                IGTOrientedPointGeometry oPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
                Point1.X = CopiedFeatureX;
                Point1.Y = CopiedFeatureY;
                Point1.Z = 0.0;

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                }
                oPointGeom.Origin = Point1;
                if (CopiedFeatureOrientation != null)
                    oPointGeom.Orientation = CopiedFeatureOrientation;
                oNewFeature.Components.GetComponent(iCNO).Geometry = oPointGeom;
               
                m_oIGTTransactionManager.Commit();
                m_oIGTTransactionManager.RefreshDatabaseChanges();
               
                if (NonDuctPath)
                {
                    if (CreatePath())
                        return true;
                    else return false;
                }

                return true;
               
            }
            catch (Exception ex)
            {
                m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
                
            }
        }
        private void CopyAttributes(Recordset FromRec, Recordset ToRec)
        {
            if (!FromRec.EOF)
            {
                for (int i = 0; i < FromRec.Fields.Count; i++)
                {
                    if ((FromRec.Fields[i].Name != "G3E_FID") && (FromRec.Fields[i].Name != "G3E_ID"))
                    {
                        ToRec.Update(FromRec.Fields[i].Name, FromRec.Fields[i].Value);
                    }
                }
            }
        }
        #endregion

        #region Create Non Duct Path
        private bool CreatePath()
        {
            try
            {
                double TotalLength=0;
                short iFNO = 0;
                short iCNO = 0;
                int iFID = 0;
                double degree;

                m_oIGTTransactionManager.Begin("CreatePath");

                IGTKeyObject oNewFeature;
                IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                IGTPoint Point2 = GTClassFactory.Create<IGTPoint>();
                IGTPoint Point3 = GTClassFactory.Create<IGTPoint>();

                IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();

                Point1.X = SourceFeatureX;
                Point1.Y = SourceFeatureY;
                Point1.Z = 0.0;

                Point2.X = CopiedFeatureX;
                Point2.Y = CopiedFeatureY;
                Point2.Z = 0.0;

                Point3.X = Point1.X + (Point2.X - Point1.X) / 2;
                Point3.Y = Point1.Y + (Point2.Y - Point1.Y) / 2;
                Point3.Z = 0.0;

                oLineGeom.Points.Add(Point1);
                oLineGeom.Points.Add(Point2);
                TotalLength = LengthBtwTwoPoints(SourceFeatureX,SourceFeatureY,CopiedFeatureX,CopiedFeatureY);
                degree = AngleBtwPoint(SourceFeatureX, SourceFeatureY, CopiedFeatureX, CopiedFeatureY);

                iFNO = 3500;
                oNewFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(iFNO);

                iFID = oNewFeature.FID;

                // Attribute
                iCNO = 3501;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {

                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", Convert.ToString(TotalLength));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", "ON POLE");//Get_Value("SELECT PL_VALUE from REF_CIV_NONDUCTPATHTYPE where PL_NUM = 10"));

                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", Convert.ToString(TotalLength));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", "ON POLE");//Get_Value("SELECT PL_VALUE from REF_CIV_NONDUCTPATHTYPE where PL_NUM = 10"));
                }
             

                //GC_NETELEM
                iCNO = 51;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");


                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");

                }
         
                //Geometry
                iCNO = 3510;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }
                oNewFeature.Components.GetComponent(iCNO).Geometry = oLineGeom;
              
                //Label
                iCNO = 3530;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }
                oTextGeom.Origin = Point3;
                if (degree > 90 && degree <= 180)
                    degree = degree + 180;
                else if (degree < -90 && degree >= -180)
                    degree = degree - 180;
                else if (degree > 180 && degree <= 270)
                    degree = degree - 180;
                oTextGeom.Rotation = degree;
                oTextGeom.Alignment = GTAlignmentConstants.gtalBottomCenter;

                

                oNewFeature.Components.GetComponent(iCNO).Geometry = oTextGeom;

                m_oIGTTransactionManager.Commit();
                m_oIGTTransactionManager.RefreshDatabaseChanges();
                if(CreateNEConnectionPath(iFID))
                              return true; 
                return false;
            }
            catch (Exception ex)
            {
                m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CreateNEConnectionPath(int iFID)
        {
            try
            {
                m_oIGTTransactionManager.Begin("CreateNEConnection");
                IGTKeyObject oPoleSource = m_IGTDataContext.OpenFeature(3000, SourceFeatureFID);
                IGTKeyObject oPoleCopied = m_IGTDataContext.OpenFeature(3000, CopiedFeatureFID);
                IGTKeyObject oPath = m_IGTDataContext.OpenFeature(3500, iFID);
                mobjRelationshipService.ActiveFeature = oPoleSource;

                if (mobjRelationshipService.AllowSilentEstablish(oPath))
                    mobjRelationshipService.SilentEstablish(1, oPath, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal1);
                else
                {
                    m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                mobjRelationshipService.ActiveFeature = oPath;
                if (mobjRelationshipService.AllowSilentEstablish(oPoleCopied))
                    mobjRelationshipService.SilentEstablish(1, oPoleCopied, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal2);
                else
                {
                    m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                m_oIGTTransactionManager.Commit();
                m_oIGTTransactionManager.RefreshDatabaseChanges();
                return true; ;
            }
            catch (Exception ex)
            {
                m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region Associate Pole with DP/FDP (owns relationship)
        private bool AssociateWithDP()
        {
            try
            {
                short iFNO = 3000;//Pole

                m_oIGTTransactionManager.Begin("AssociateWithDP");

                IGTKeyObject oPole= m_IGTDataContext.OpenFeature(iFNO, SourceFeatureFID);
                IGTKeyObject oDP= m_IGTDataContext.OpenFeature(AssociateFNO, AssociateFID);

                string ownership = Get_Value("  select owner1_id from GC_OWNERSHIP where g3e_fid=" + AssociateFID);
                if (ownership != "")
                {
                    DistForm.Hide();
                    DialogResult retVal = MessageBox.Show("Selected DP/FDP already has relationship! Do you want to reestablish relationship?", "Copy Pole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        mobjRelationshipService.ActiveFeature = oPole;

                        if (mobjRelationshipService.AllowSilentEstablish(oDP))
                            mobjRelationshipService.SilentEstablish(2, oDP);
                        else
                        {
                            m_oIGTTransactionManager.Rollback();
                            MessageBox.Show("Error during trying reestablish relationship!", "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        oPole.Components.GetComponent(3001).Recordset.Update("POLE_USAGE", "DISTRIBUTION");
                    }
                    else
                    {
                        m_oIGTTransactionManager.Rollback();
                        return false;
                    }

                }
                else
                {
                    mobjRelationshipService.ActiveFeature = oPole;

                    if (mobjRelationshipService.AllowSilentEstablish(oDP))
                        mobjRelationshipService.SilentEstablish(2, oDP);
                    else
                    {
                        m_oIGTTransactionManager.Rollback();
                        MessageBox.Show("Error during trying reestablish relationship!", "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    oPole.Components.GetComponent(3001).Recordset.Update("POLE_USAGE", "DISTRIBUTION");
                }

                m_oIGTTransactionManager.Commit();
                m_oIGTTransactionManager.RefreshDatabaseChanges();

                return true; 
            }
            catch (Exception ex)
            {
                m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Copy Pole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
       
        #region Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        #endregion

        #region Angle between segment and OX by start and end's points 
        public double AngleBtwPoint(double stX, double stY, double endX, double endY)
        {
            double t1 = endY - stY;
            double t2 = endX - stX;

            if (t1 == 0 && t2 == 0) return 0;
            if (t2 == 0)
            {
                if (t1 > 0)
                    return 90;
                if (t1 < 0)
                    return -90;
                if (t1 == 0)
                    return 0;
            }

            double grad = Math.Atan(Math.Abs(t1 / t2)) * 180 / Math.PI;

            if (t2 > 0)
            {
                if (t1 > 0)
                    return grad;
                if (t1 < 0)
                    return -grad;
                if (t1 == 0)
                    return 0;
            }
            if (t2 < 0)
            {
                if (t1 > 0)
                    return 180 - grad;
                if (t1 < 0)
                    return 180 + grad;
                if (t1 == 0)
                    return 180;
            }
            return 0;

        }
        #endregion

        #region Between Two points on sumple line
        private int LengthBtwTwoPoints(double startPointX, double startPointY, double endPointX, double endPointY)
        {
            return Convert.ToInt32(Math.Round(Math.Sqrt(Math.Pow((endPointX - startPointX), 2) + Math.Pow((endPointY - startPointY), 2)), 0));
        }
        #endregion

        #region Calculate Coord for point base on  length
        public IGTPoint PointBasedOnEnteredLength(double len, double startPointX, double startPointY, double endPointX, double endPointY)
        {
            double Angle = AngleBtwPoint(startPointX, startPointY, endPointX, endPointY);
            
            IGTPoint NewPoint = GTClassFactory.Create<IGTPoint>();
            NewPoint.X = startPointX + len * Math.Cos(Angle * Math.PI / 180);
            NewPoint.Y = startPointY + len * Math.Sin(Angle * Math.PI / 180);
            NewPoint.Z = 0.0;
           // int temp = LengthBtwTwoPoints(startPointX,  startPointY,  endPointX,  endPointY);
           // NewPoint.X = len * (endPointX - startPointX) / temp + startPointX;
           // NewPoint.Y = len * (endPointY - startPointY) / temp + startPointY;
           // NewPoint.Z = 0.0;
            return NewPoint;
        }
        #endregion

        #region Explorer Feture event

        //******************************************************************************
        // Purpose:  Represents the method that will handle the CancelClick event.
        //
        //******************************************************************************
        private void mobjExplorerService_CancelClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {
                m_oIGTTransactionManager.Rollback();
               // ExitCmd();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        //******************************************************************************
        // Purpose:  Represents the method that will handle the SaveAndContinueClick
        //           event.
        //
        //******************************************************************************
        private void mobjExplorerService_SaveAndContinueClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {

                // Transitions the state of the command to the next step in the placement work flow.

                m_oIGTTransactionManager.Rollback();

                // Catches any errors thrown to the error handler and displays a message box with the description.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }

        }


        //******************************************************************************
        // Purpose:  Represents the method that will handle the SaveClick event.
        //
        //******************************************************************************
        private void mobjExplorerService_SaveClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {

                // Transitions the state of the command to the next step in the placement work flow.
                m_oIGTTransactionManager.Rollback();

                // Catches any errors thrown to the error handler and displays a message box with the description.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }

        }
        #endregion
    }
}
