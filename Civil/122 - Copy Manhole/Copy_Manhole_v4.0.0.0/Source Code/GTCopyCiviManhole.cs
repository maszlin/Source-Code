using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.NEPSCopyManhole
{
    class GTCopyCivilManhole : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        public static Intergraph.GTechnology.API.IGTDataContext m_IGTDataContext = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditServiceRotate = null;
        public static Intergraph.GTechnology.API.IGTRelationshipService mobjRelationshipService = null;
        IGTLocateService mobjLocateService = null;
            #region Manhole
            public static int startplc=0;
            public static int SourceFeatureFID = 0;
            public static double SourceFeatureX = 0;
            public static double SourceFeatureY = 0;
            public static int CopiedFeatureFID = 0;
            public static double CopiedFeatureX = 0;
            public static double CopiedFeatureY = 0;
            public static IGTVector CopiedFeatureOrientation = null;
            public static double CopiedFeatureRotation= 0;
        public class MHStyle
        {
            public int SymbolStyle = 0;
            public int WallNumStyle = 0;
            public int LabelIDStyle = 0;
            public int LabelDescStyle = 0;
            public GTAlignmentConstants LabelAlignID = GTAlignmentConstants.gtalTopCenter;
            public GTAlignmentConstants LabelAlignDesc = GTAlignmentConstants.gtalTopCenter;
            public string LabelIDContent = "???";
            public string LabelDescContent = "MHType";
            public int LineStyle = 0;
            public int LeaderLineStyle = 0;
        };
        MHStyle ManhlStyle = new MHStyle();
            public class GeometryDesc
            {
                public int FID = 0;
                public string viewname = "";
                public double[] X;
                public double[] Y;
                public IGTVector Orientation = null;
                public double Rotation = 0;
                public string type = "";
                public int styleId = 0;
                public string lblText = "";
                public double[] diffX;
                public double[] diffY;
                public double[] dictCentre;
                public int Alighment;
            };
            public static List<GeometryDesc> SourceFeature = null;
            public static int ManholeStyleId = 0;
            public static Distance DistForm = null;
            public static bool FormClosing = false;
            public static bool Precision = false;
            #endregion
  
            #region Duct Path
            public IGTPoint SelGeom = null;
            GTWindowsForm_DuctPathPlc m_CustomForm = null;
            public static IGTTextPointGeometry LabelAlongLine = null;
            public static IGTTextPointGeometry oTextGeomLabel = null;
            public static int startdraw = 7;
            private int middledraw = 0;
            public static IGTPoint PointForLeaderLine = null;
            public static IGTPoint StartDrawPoint = null;
            public static IGTPoint EndDrawPoint = null;
            private int styleid = 14500;
            #endregion
        #endregion

        #region Mouse Click
            void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            try
            {
                IGTPoint WorldPoint = e.WorldPoint;

                if (e.Button != 2)//left button
                {
                 #region MANHOLE

                    #region selecting source MH
                    if (startplc == 10000) //select source Manhole feature to start copying
                    {
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source Manhole feature! Right Click  to exit.");
                        DistForm.MessageHelpChange(0);
                        
                        IGTDDCKeyObjects feat1 = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectAll);
                        IGTDDCKeyObjects feat = GTClassFactory.Create<IGTDDCKeyObjects>();
                        foreach (IGTDDCKeyObject oDDCKeyObject in feat1)
                        {
                            feat = m_gtapp.DataContext.GetDDCKeyObjects(oDDCKeyObject.FNO, oDDCKeyObject.FID, GTComponentGeometryConstants.gtddcgAllGeographic);
                            break;
                        }
                     
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
                       
                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {

                            if (SourceFeature == null)
                                SourceFeature = new List<GeometryDesc>();

                            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                            {

                                if (oDDCKeyObject.FNO != 2700)
                                {
                                    DistForm.Hide();
                                    MessageBox.Show("Please select a Manhole!", "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    DistForm.Show();
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source Manhole feature! Right Click  to exit.");
                                    return;
                                }
                                /////////////////////
                                ////////////////////
                                /////////////////////
                                //////////////////////
                                //take style for copying mh
                                IGTKeyObject mhfeature = m_gtapp.DataContext.OpenFeature(oDDCKeyObject.FNO, oDDCKeyObject.FID);
                               
                                string ManholeType = "JC9";
                                string MH_MIX = "";
                                if (!mhfeature.Components.GetComponent(2701).Recordset.EOF)
                                {
                                    ManholeType = mhfeature.Components.GetComponent(2701).Recordset.Fields["FEATURE_TYPE"].Value.ToString();
                                    MH_MIX = mhfeature.Components.GetComponent(2701).Recordset.Fields["MH_MIX"].Value.ToString();
                                }
                          

                                IGTHelperService helper = GTClassFactory.Create<IGTHelperService>();
                                helper.DataContext = m_gtapp.DataContext;

                                ManhlStyle.SymbolStyle = helper.GetComponentStyleID(mhfeature, 2720);
                              
                                string manholeLabel = "[ManholeLabel]";
                                GTAlignmentConstants ManholeLabelAlignment = GTAlignmentConstants.gtalBottomLeft;

                                ManhlStyle.LabelIDStyle = helper.GetComponentStyleID(mhfeature, 2730);
                                helper.GetComponentLabel(mhfeature, 2730, ref manholeLabel, ref ManholeLabelAlignment);
                                ManhlStyle.LabelAlignID = ManholeLabelAlignment;
                                ManhlStyle.LabelIDContent = "???";
                                helper.GetComponentLabel(mhfeature, 2734, ref manholeLabel, ref ManholeLabelAlignment);
                                ManhlStyle.LabelAlignDesc = ManholeLabelAlignment;
                                ManhlStyle.LabelDescStyle = helper.GetComponentStyleID(mhfeature, 2734);
                                string decription = "";
                                if (MH_MIX.ToUpper() == "PRE-FAB")
                                    decription = ManholeType + "/(P)";
                                else if (MH_MIX.ToUpper() == "PRE-FAB (SHUTTERING)")
                                    decription = ManholeType + "/(P/S)";
                                else if (MH_MIX.ToUpper() == "SITE")
                                    decription = ManholeType;
                                else decription = ManholeType + MH_MIX;

                                // REPLACE  "NEW LINE" MORE THAN 1 WITH 1 "NEW LINE"
                                //  decription=decription.Replace("\s{2,}","\n");


                                ManhlStyle.LabelDescContent = decription;
                                ManhlStyle.WallNumStyle = helper.GetComponentStyleID(mhfeature, 2732);
                                ManhlStyle.LineStyle = helper.GetComponentStyleID(mhfeature, 2710);
                                ManhlStyle.LeaderLineStyle = helper.GetComponentStyleID(mhfeature, 2712);
                                ///////////////////
                                ///////////////////

                                GeometryDesc temp = new GeometryDesc();
                                temp.FID = oDDCKeyObject.FID;
                                temp.viewname = oDDCKeyObject.ComponentViewName;
                                temp.type = oDDCKeyObject.Geometry.Type;
                                if (temp.type != "CompositePolylineGeometry")
                                {
                                    temp.X = new double[2];
                                    temp.Y = new double[2];
                                    temp.dictCentre = new double[2];
                                    temp.diffX = new double[2];
                                    temp.diffY = new double[2];
                                    temp.X[0] = oDDCKeyObject.Geometry.FirstPoint.X;
                                    temp.Y[0] = oDDCKeyObject.Geometry.FirstPoint.Y;
                                    temp.X[1] = -1;
                                    temp.Y[1] = -1;
                                    temp.Orientation = ((IGTOrientedPointGeometry)oDDCKeyObject.Geometry).Orientation;
                                    temp.Rotation = AngleBtwPoint(temp.Orientation.I, temp.Orientation.J);
                                }
                                else
                                {
                                    IGTPolylineGeometry tt = (IGTPolylineGeometry)((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).ExtractGeometry(((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).FirstPoint, ((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).LastPoint, false);

                                    int count = tt.Points.Count + 1;
                                    temp.X = new double[count];
                                    temp.Y = new double[count];
                                    temp.dictCentre = new double[count];
                                    temp.diffX = new double[count];
                                    temp.diffY = new double[count];
                                }
                                if (oDDCKeyObject.ComponentViewName == "VGC_MANHL_S")
                                {

                                    SourceFeatureFID = oDDCKeyObject.FID;
                                    SourceFeatureX = oDDCKeyObject.Geometry.FirstPoint.X;
                                    SourceFeatureY = oDDCKeyObject.Geometry.FirstPoint.Y;
                                   // string type = Get_Value("select FEATURE_TYPE from GC_MANHL where G3E_FID=" + SourceFeatureFID);
                                   //// type = type;// +" PPF";
                                   // string tt = "select  MIN(G3E_SNO) from G3E_STYLERULE where G3E_SRNO=272001 " +
                                   //             " and upper(G3E_FILTER) like upper('%PPF%') " +
                                   //             " and upper(G3E_FILTER) like upper('%" + type + "%') ";
                                   // string styletmp = Get_Value(tt);

                                   // //  IGTHelperService helperService = GTClassFactory.Create<IGTHelperService>();
                                   // //  helperService.DataContext = m_IGTDataContext;
                                   // //  ManholeStyleId = helperService.GetComponentStyleID(m_IGTDataContext.OpenFeature(2700, SourceFeatureFID), 2720);

                                   // if (styletmp == "")
                                   //     ManholeStyleId = 2720044;
                                   // else ManholeStyleId = int.Parse(styletmp);
                                    ManholeStyleId = ManhlStyle.SymbolStyle;
                                    temp.styleId = ManholeStyleId;
                                }
                                else
                                    if (oDDCKeyObject.ComponentViewName == "VGC_MANHLW_T")
                                    {
                                        temp.styleId = ManhlStyle.WallNumStyle;// 30600;
                                        for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                                        {
                                            if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "WALL_NUM")
                                            {
                                                temp.lblText = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                                              
                                            }
                                            if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "G3E_ALIGNMENT")
                                            {
                                                temp.Alighment = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                              
                                            }
                                        }

                                    }
                                    else
                                        if (oDDCKeyObject.ComponentViewName == "VGC_MANHL_T")
                                        {
                                            temp.styleId = ManhlStyle.LabelIDStyle;//27300102;
                                            //string FEATURE_TYPE = "";
                                            //for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                                            //{

                                            //    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "FEATURE_TYPE")
                                            //    {
                                            //        FEATURE_TYPE = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                                            //    }
                                            //    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "G3E_ALIGNMENT")
                                            //    {
                                            //        temp.Alighment = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                                  
                                            //    }
                                            //}
                                            temp.Alighment =(int)ManhlStyle.LabelAlignID;
                                            temp.lblText = ManhlStyle.LabelIDContent;//"???\n" + FEATURE_TYPE;

                                        }
                                        else
                                            if (oDDCKeyObject.ComponentViewName == "VGC_MANHL_T2")
                                            {
                                                temp.styleId = ManhlStyle.LabelDescStyle; //30600;
                                                //string FEATURE_TYPE = "";
                                                //string MH_MIX = "";
                                                //for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                                                //{

                                                //    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "FEATURE_TYPE")
                                                //    {
                                                //        FEATURE_TYPE = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                                                //    }
                                                //    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "MH_MIX")
                                                //    {
                                                //        MH_MIX = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                                                //    }
                                                //    if (FEATURE_TYPE != "" && MH_MIX != "")
                                                //        break;
                                                //}
                                                temp.lblText = ManhlStyle.LabelDescContent;//"???\n" + FEATURE_TYPE + "/" + MH_MIX;
                                                temp.Alighment = (int)ManhlStyle.LabelAlignDesc;
                                            }
                                            else
                                                if (oDDCKeyObject.ComponentViewName == "VGC_MANHL_L")
                                                {
                                                    temp.styleId = ManhlStyle.LineStyle;//13508;
                                                    IGTPolylineGeometry tt = (IGTPolylineGeometry)((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).ExtractGeometry(((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).FirstPoint, ((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).LastPoint, false);

                                                    int count = tt.Points.Count;
                                                    for (int i = 0; i < count; i++)
                                                    {
                                                        temp.X[i] = tt.Points[i].X;
                                                        temp.Y[i] = tt.Points[i].Y;
                                                    }
                                                    temp.X[count] = -1;
                                                    temp.Y[count] = -1;
                                                    temp.lblText = "line";

                                                }
                                                else
                                                    if (oDDCKeyObject.ComponentViewName == "VGC_MANHLLDR_L")
                                                    {
                                                        temp.styleId = ManhlStyle.LeaderLineStyle;//13501;
                                                        IGTPolylineGeometry tt = (IGTPolylineGeometry)((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).ExtractGeometry(((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).FirstPoint, ((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).LastPoint, false);

                                                        int count = tt.Points.Count;
                                                        for (int i = 0; i < count; i++)
                                                        {
                                                            temp.X[i] = tt.Points[i].X;
                                                            temp.Y[i] = tt.Points[i].Y;
                                                        }
                                                        temp.X[count] = -1;
                                                        temp.Y[count] = -1;
                                                        temp.lblText = "line";
                                                    }

                                SourceFeature.Add(temp);
                            }
                            for (int i = 0; i < SourceFeature.Count; i++)
                            {
                                int j = 0;
                                while (SourceFeature[i].X[j] != -1)
                                {
                                    SourceFeature[i].diffX[j] = SourceFeature[i].X[j] - SourceFeatureX;
                                    SourceFeature[i].diffY[j] = SourceFeature[i].Y[j] - SourceFeatureY;
                                    SourceFeature[i].dictCentre[j] = LengthBtwTwoPoints(SourceFeatureX, SourceFeatureY, SourceFeature[i].X[j], SourceFeature[i].Y[j]);
                                    j++;
                                }
                                SourceFeature[i].diffX[j] = -1;
                                SourceFeature[i].diffY[j] = -1;
                                SourceFeature[i].dictCentre[j] = -1;

                                IGTPoint temp = GTClassFactory.Create<IGTPoint>();
                                temp.X = SourceFeature[i].X[0];
                                temp.Y = SourceFeature[i].Y[0];
                                temp.Z = 0;

                                if (SourceFeature[i].lblText != "" && SourceFeature[i].lblText != "line")
                                {
                                    IGTTextPointGeometry textTemp = GTClassFactory.Create<IGTTextPointGeometry>();

                                    textTemp.Origin = temp;
                                    textTemp.RichText = SourceFeature[i].lblText;
                                    textTemp.Rotation = SourceFeature[i].Rotation;
                                    textTemp.Alignment = 0;
                                    mobjEditService.AddGeometry(textTemp, SourceFeature[i].styleId);
                                }
                                else if (SourceFeature[i].lblText != "line")
                                {
                                    IGTOrientedPointGeometry symPoint = GTClassFactory.Create<IGTOrientedPointGeometry>();
                                    symPoint.Origin = temp;
                                    symPoint.Orientation = SourceFeature[i].Orientation;
                                    mobjEditService.AddGeometry(symPoint, SourceFeature[i].styleId);

                                }
                                else if (SourceFeature[i].lblText == "line")
                                {
                                    IGTPolylineGeometry line = GTClassFactory.Create<IGTPolylineGeometry>();
                                    int k = 0;
                                    while (SourceFeature[i].X[k] != -1)
                                    {
                                        IGTPoint Pline = GTClassFactory.Create<IGTPoint>();
                                        Pline.X = SourceFeature[i].X[k];
                                        Pline.Y = SourceFeature[i].Y[k];
                                        Pline.Z = 0;
                                        line.Points.Add(Pline);
                                        k++;
                                    }

                                    mobjEditService.AddGeometry(line, SourceFeature[i].styleId);
                                }

                            }

                            startplc = 1000;
                            DistForm.MessageHelpChange(1);
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confrim location! Right Click  to exit.");

                            return;
                        }
                        else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                        {
                            DistForm.Hide();
                            MessageBox.Show("Please select only one Manhole at once!", "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            m_gtapp.SelectedObjects.Clear();
                            DistForm.MessageHelpChange(0);
                            DistForm.Show();
                            m_gtapp.ActiveMapWindow.Activate();
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source Manhole feature! Right Click  to exit.");
                            return;
                        }
                        return;
                    }
                    #endregion

                    if (startplc == 1000)//confirm position of MANHOLE
                    {

                        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        if (!Precision) //in NON-precision mode
                        {
                            tempp.X = e.WorldPoint.X;
                            tempp.Y = e.WorldPoint.Y;
                            tempp.Z = 0.0;
                        }
                        //in precision mode
                        else tempp = PointBasedOnEnteredLength(double.Parse(DistForm.txtDistance.Value.ToString()),
                                     SourceFeatureX, SourceFeatureY, e.WorldPoint.X, e.WorldPoint.Y);

                        CopiedFeatureX = tempp.X;
                        CopiedFeatureY = tempp.Y;


                        startplc = 2000;
                        DistForm.MessageHelpChange(3);
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete copying! Right Click  to change location.");
                        return;
                    }
                    #region rotation MH
                    if (startplc == 2000)//confirm rotation of MANHOLE
                    {
                        
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;

                    IGTPoint tempp2 = GTClassFactory.Create<IGTPoint>();
                    tempp2.X = CopiedFeatureX;
                    tempp2.Y = CopiedFeatureY;
                    tempp2.Z = 0.0;
                    double Alfa = AngleBtwPoint(CopiedFeatureX, CopiedFeatureY, e.WorldPoint.X, e.WorldPoint.Y);
                    //  double len = LengthBtwTwoPoints(CopiedFeatureX, CopiedFeatureY, e.WorldPoint.X, e.WorldPoint.Y);
                    IGTVector Orientation = GTClassFactory.Create<IGTVector>();

                    if (mobjEditServiceRotate.GeometryCount > 0)
                        mobjEditServiceRotate.RemoveAllGeometries();

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

                    if (CopiedFeatureOrientation == null)
                        CopiedFeatureOrientation = GTClassFactory.Create<IGTVector>();

                    CopiedFeatureOrientation = Orientation.BuildVector(tempp, tempp2);
                    CopiedFeatureRotation = Alfa;

                    for (int i = 0; i < SourceFeature.Count; i++)
                    {
                        IGTPoint temp = GTClassFactory.Create<IGTPoint>();
                        temp.X = CopiedFeatureX + SourceFeature[i].diffX[0];
                        temp.Y = CopiedFeatureY + SourceFeature[i].diffY[0];
                        temp.Z = 0;

                        double Beta = AngleBtwPoint(CopiedFeatureX, CopiedFeatureY, temp.X, temp.Y);
                        IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
                        projectPoint = PointBasedOnEnteredLength(SourceFeature[i].dictCentre[0], Alfa + Beta, CopiedFeatureX, CopiedFeatureY);
                        if (SourceFeature[i].viewname == "VGC_MANHL_T" || SourceFeature[i].viewname == "VGC_MANHL_T2")
                        {
                            IGTPoint tempp4 = GTClassFactory.Create<IGTPoint>();
                            tempp4.X = CopiedFeatureX + SourceFeature[i].diffX[0];
                            tempp4.Y = CopiedFeatureY + SourceFeature[i].diffY[0];
                            tempp4.Z = 0.0;
                            IGTTextPointGeometry textTemp = GTClassFactory.Create<IGTTextPointGeometry>();
                            textTemp.Origin = tempp4;
                            textTemp.Text = SourceFeature[i].lblText;
                            textTemp.Rotation = SourceFeature[i].Rotation;
                            //textTemp.Alignment = GTAlignmentConstants.gtalBottomRight;
                            textTemp.Alignment = (Intergraph.GTechnology.API.GTAlignmentConstants)SourceFeature[i].Alighment;// GTAlignmentConstants.gtalBottomRight; ;
                           
                            mobjEditService.AddGeometry(textTemp, SourceFeature[i].styleId);
                        }
                        else
                        if (SourceFeature[i].lblText != "" && SourceFeature[i].lblText != "line")
                        {
                            IGTTextPointGeometry textTemp = GTClassFactory.Create<IGTTextPointGeometry>();
                            textTemp.Origin = projectPoint;
                            textTemp.Text = SourceFeature[i].lblText;
                            textTemp.Rotation = Alfa + SourceFeature[i].Rotation;
                            textTemp.Alignment = 0;
                            mobjEditService.AddGeometry(textTemp, SourceFeature[i].styleId);

                        }
                        else if (SourceFeature[i].lblText != "line")
                        {
                            IGTPoint PointForOrient = GTClassFactory.Create<IGTPoint>();
                            PointForOrient = PointBasedOnEnteredLength(1, Alfa + SourceFeature[i].Rotation, CopiedFeatureX, CopiedFeatureY);
                            IGTOrientedPointGeometry symPoint = GTClassFactory.Create<IGTOrientedPointGeometry>();
                            symPoint.Origin = projectPoint;
                               symPoint.Orientation = Orientation.BuildVector( projectPoint,PointForOrient); //Orientation.AddVectors(SourceFeature[i].Orientation,Orientation.BuildVector(tempp,tempp2));//
                           mobjEditService.AddGeometry(symPoint, SourceFeature[i].styleId);
                            CopiedFeatureOrientation = symPoint.Orientation;
                        }
                        else if (SourceFeature[i].lblText == "line")
                        {

                            IGTPolylineGeometry line = GTClassFactory.Create<IGTPolylineGeometry>();
                            int k = 0;
                            while (SourceFeature[i].X[k] != -1)
                            {
                                double Beta1 = AngleBtwPoint(CopiedFeatureX, CopiedFeatureY, CopiedFeatureX + SourceFeature[i].diffX[k], CopiedFeatureY + SourceFeature[i].diffY[k]);
                                IGTPoint Pline = GTClassFactory.Create<IGTPoint>();
                                Pline = PointBasedOnEnteredLength(SourceFeature[i].dictCentre[k], Alfa + Beta1, CopiedFeatureX, CopiedFeatureY);

                                line.Points.Add(Pline);
                                k++;
                            }
                            mobjEditService.AddGeometry(line, SourceFeature[i].styleId);
                        }

                    }
                        
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete copying! Right Click  to change location.");
                    DistForm.MessageHelpChange(3);
                    startplc = 5000;
                    return;
                }
                    #endregion
                 #endregion

                #region DUCT PATH
                #region For Conduit Line Placement
                if (startdraw == 1 && StartDrawPoint != null)
                    {

                        //   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                        IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                        Point1.X = e.WorldPoint.X;
                        Point1.Y = e.WorldPoint.Y;
                        Point1.Z = 0.0;
                        oLineGeom.Points.Add(StartDrawPoint);
                        oLineGeom.Points.Add(Point1);
                        if (middledraw == 0)
                        {
                            if (mobjEditService.GeometryCount > 0)
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);// .RemoveAllGeometries();
                        }

                        mobjEditService.AddGeometry(oLineGeom, styleid);

                        StartDrawPoint.X = e.WorldPoint.X;
                        StartDrawPoint.Y = e.WorldPoint.Y;
                        StartDrawPoint.Z = 0.0;
                        middledraw = 1;
                        //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                    }
                    #endregion

                    #region For Leader Line Section Label Placement
                    if (startdraw == 35)
                    {
                        if (PointForLeaderLine == null)
                        {
                            PointForLeaderLine = GTClassFactory.Create<IGTPoint>();
                        }
                        PointForLeaderLine.X = e.WorldPoint.X;
                        PointForLeaderLine.Y = e.WorldPoint.Y;
                        PointForLeaderLine.Z = 0.0;
                        middledraw = 1;
                    }
                    #endregion

                    #region For Section Label Placement

                    //fix position after rotation
                    if (startdraw == 5)
                    {
                        IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint Point2 = GTClassFactory.Create<IGTPoint>();
                        IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                        Point1.X = e.WorldPoint.X;
                        Point1.Y = e.WorldPoint.Y;
                        Point1.Z = 0.0;
                        Point2.X = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.X;
                        Point2.Y = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.Y;
                        Point2.Z = 0.0;
                        oTextGeom.Origin = Point2;
                        oTextGeom.Rotation = m_CustomForm.AngleBtwPoint(Point2.X, Point2.Y, Point1.X, Point1.Y);
                        oTextGeom.Text = m_CustomForm.SectLabel;
                        oTextGeom.Alignment = 0;
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.AddGeometry(oTextGeom, 32400);
                        oTextGeomLabel.Origin = Point2;
                        startdraw = 35;
                        return;
                    }

                    if (startdraw == 3)
                    {
                        startdraw = 5;
                        return;
                    }
                    #endregion

                    #region For Section Slash plc
                    if (startdraw == 2 || startdraw == 20)
                    {
                        IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        tempp.X = e.WorldPoint.X;
                        tempp.Y = e.WorldPoint.Y;
                        tempp.Z = 0.0;
                        temp.Origin = tempp;
                        IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);

                        if (projectPoint.X == 0 && projectPoint.Y == 0)
                        {
                            MessageBox.Show("Please, place Slash Point on creating Conduit Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            temp.Origin = projectPoint;
                            //IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                            //temp.Orientation = Orientation.BuildVector( projectPoint,tempp);
                            int length = int.Parse(projectPoint.Z.ToString());
                            if (m_CustomForm.SectSlashes.Count > 0)
                            {
                                if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                {
                                    MessageBox.Show("Not allowed place Slash for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                            }

                            temp.Orientation = m_CustomForm.OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);

                            if (mobjEditService.GeometryCount > 0)
                            {
                                if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                    mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            }
                            mobjEditService.AddGeometry(temp, 2220004);
                        }
                        startdraw = 20;
                        return;
                    }
                    #endregion

                    #region Get Selected Source Wall OR Term Wall of Device
                    if (startdraw == 300 || startdraw == 400)
                    {
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {
                            if ((startdraw == 300 && m_CustomForm.GetDeviceWithWall(true))//source
                                || (startdraw == 400 && m_CustomForm.GetDeviceWithWall(false)))//term
                            {
                                startdraw = 0;
                                m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                                return;
                            }
                            else return;
                        }

                    }
                    #endregion

                    #region For Moving label along line plc


                    if (startdraw == 39)
                    {
                        IGTTextPointGeometry temp = GTClassFactory.Create<IGTTextPointGeometry>();
                        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        tempp.X = e.WorldPoint.X;
                        tempp.Y = e.WorldPoint.Y;
                        tempp.Z = 0.0;
                        temp.Origin = tempp;
                        IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);

                        if (projectPoint.X == 0 && projectPoint.Y == 0)
                        {
                            MessageBox.Show("Please, place Label on creating Conduit Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            temp.Origin = projectPoint;
                            //IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                            //temp.Orientation = Orientation.BuildVector( projectPoint,tempp);
                            int length = int.Parse(projectPoint.Z.ToString());
                            if (m_CustomForm.SectSlashes.Count > 0)
                            {
                                if (m_CustomForm.LastSection != 1)
                                    if (length >= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                    {
                                        MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                if (m_CustomForm.SectSlashes.Count > 1)
                                {
                                    if (m_CustomForm.LastSection == 1)
                                    {
                                        if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                        {
                                            MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                    }
                                    else
                                        if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 2].length)
                                        {
                                            MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                }
                            }

                            temp.Rotation = m_CustomForm.TakeRotationOfSegmentPolyline(length);// OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);
                            temp.Text = LabelAlongLine.Text;
                            temp.Alignment = 0;
                            if (mobjEditService.GeometryCount > 0)
                            {
                                if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                                    mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            }
                            mobjEditService.AddGeometry(temp, 32400);
                            oTextGeomLabel.Origin = temp.Origin;
                            oTextGeomLabel.Text = temp.Text;
                            oTextGeomLabel.Rotation = temp.Rotation;
                            oTextGeomLabel.Alignment = 0;
                        }
                        startdraw = 38;
                        return;
                    }
                    #endregion

                   
                 #endregion

                }
                else if (e.Button == 2)//right button
                {

                    #region MANHOLE


                    if ( startplc == 5000)//start moving MANHOLE  again,  5- while waiting for final placement confirmation
                    {
                        DistForm.MessageHelpChange(1);
                        startplc = 1000;
                        return;
                    }
                    if (startplc == 2000 )//skip rotaion rotation
                    {
                        DistForm.MessageHelpChange(3);
                        CopiedFeatureRotation = 0;
                        startplc = 5000;
                        return;
                    }

                    if (startplc == 10000 || startplc == 1000)//exiting while moving MANHOLE or before selected source MANHOLE feature
                    {
                        if (DistForm != null)
                            DistForm.Hide();
                        DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Copy Manhole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

                    #endregion

                    #region DUCT PATH
                    #region start drawing with mouse moving Conduit Line Placement
                    if (startdraw == 101)
                    {
                        if (mobjEditService.GeometryCount > 0)
                            mobjEditService.RemoveAllGeometries();
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        startdraw = 1;
                        //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                    }
                     #endregion

                    #region Cancelation for Wall or Device Selection
                    if (startdraw == 300 || startdraw == 400 || startdraw == 500 || startdraw == 600)
                    {
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                        startdraw = 0;
                        // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                    }
                    #endregion

                    #region Cancellation of placement for section's slash
                    if (startdraw == 2)
                    {
                        if (m_CustomForm.LastSection == 0)
                        {
                            m_gtapp.SelectedObjects.Clear();
                            if (mobjEditService.GeometryCount > 0)
                                if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                    mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            startdraw = 0;
                            m_CustomForm.LastSection = 1;
                            // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cancellation of placement for section's slash! Next section will be the last");
                            m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                            m_CustomForm.ConfSelBtnIsEnable = true;
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                            return;
                        }

                    }

                    if (startdraw == 20)
                    {
                        startdraw = 2;
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        return;
                    }
                    #endregion

                    #region Placement for section's label along the line
                    if (startdraw == 3 || startdraw == 32 || startdraw == 39 || (startdraw == 30 && oTextGeomLabel.Rotation != LabelAlongLine.Rotation))
                    {
                        if (mobjEditService.GeometryCount > 0)
                        {
                            while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "PolylineGeometry" &&
                                mobjEditService.GeometryCount != 1)
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        LabelAlongLine.Alignment = 0;
                        mobjEditService.AddGeometry(LabelAlongLine, 32400);

                        oTextGeomLabel.Origin = LabelAlongLine.Origin;
                        oTextGeomLabel.Text = m_CustomForm.SectLabel;
                        oTextGeomLabel.Rotation = LabelAlongLine.Rotation;
                        oTextGeomLabel.Alignment = 0;
                        startdraw = 30;
                        return;
                    }
                    //start moving along line
                    if (startdraw == 30 && oTextGeomLabel.Rotation == LabelAlongLine.Rotation)
                    {
                        startdraw = 39;
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        return;
                    }
                    //start moving again
                    if (startdraw == 38)
                    {
                        startdraw = 3;
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        return;
                    }
                    #endregion
                    #region Skip rotation
                    if (startdraw == 5)
                    {
                        IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint Point2 = GTClassFactory.Create<IGTPoint>();
                        IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                        Point1.X = e.WorldPoint.X;
                        Point1.Y = e.WorldPoint.Y;
                        Point1.Z = 0.0;
                        Point2.X = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.X;
                        Point2.Y = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.Y;
                        Point2.Z = 0.0;
                        oTextGeom.Origin = Point2;
                        oTextGeom.Rotation = LabelAlongLine.Rotation;// m_CustomForm.AngleBtwPoint(Point2.X, Point2.Y, Point1.X, Point1.Y);
                        oTextGeom.Text = m_CustomForm.SectLabel;
                        oTextGeom.Alignment = 0;
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.AddGeometry(oTextGeom, 32400);
                        oTextGeomLabel.Origin = Point2;
                        oTextGeomLabel.Text = m_CustomForm.SectLabel;
                        oTextGeomLabel.Rotation = LabelAlongLine.Rotation;
                        oTextGeomLabel.Alignment = 0;
                        startdraw = 35;
                        return;
                    }
                     #endregion
                    #region Start drawing leader line from beginning
                    if (startdraw == 35)
                    {
                        //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                        if (mobjEditService.GeometryCount > 0)
                        {
                            while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        PointForLeaderLine = null;
                        middledraw = 0;
                        return;
                    }
                    //after stopping drawing before final acception
                    if (startdraw == 31)
                    {
                        //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                        if (mobjEditService.GeometryCount > 0)
                        {
                            while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        PointForLeaderLine = null;
                        middledraw = 0;
                        startdraw = 32;
                        return;
                    }
                    //start  drawing again
                    //if (startdraw == 32)
                    //{
                    //    startdraw = 35;
                    //    return;
                    //}
                    #endregion
                    #endregion
                }
            }
            catch (Exception ex)
            {
                if(DistForm!=null)
                    DistForm.Hide();
                MessageBox.Show(ex.Message, "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        } 
        #endregion

        #region Mouse Move
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
      //      ExitCmd();

            try
            {

                #region MANHOLE
                if (DistForm == null)
                {
                    DistForm = new Distance();
                    DistForm.FormClosing += new FormClosingEventHandler(Distance_FormClosing);
                    DistForm.MouseUp+=new MouseEventHandler(DistForm_MouseUp);
                    DistForm.Show();
                   // m_gtapp.
                    m_gtapp.ActiveMapWindow.Activate();
                }

                #region moving MH
                if (startplc == 1000 )//moving position of manhole 
                {
                    DistForm.MessageHelpChange(1);
                    DistForm.Show();
                    m_gtapp.ActiveMapWindow.Activate(); 

                    if (mobjEditService.GeometryCount > 0)
                        mobjEditService.RemoveAllGeometries();
                    if (mobjEditServiceRotate.GeometryCount > 0)
                        mobjEditServiceRotate.RemoveAllGeometries();
                    m_CustomForm = null;
                    
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;

                    if (Precision)//in precision mode
                    {
                        tempp = PointBasedOnEnteredLength(double.Parse(DistForm.txtDistance.Value.ToString()),
                                     SourceFeatureX, SourceFeatureY, e.WorldPoint.X, e.WorldPoint.Y);

                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                        IGTPoint tempp2 = GTClassFactory.Create<IGTPoint>();
                        tempp2.X = SourceFeatureX;
                        tempp2.Y = SourceFeatureY;
                        tempp2.Z = 0.0;

                        oLineGeom.Points.Add(tempp2);
                        oLineGeom.Points.Add(tempp);
                        mobjEditService.AddGeometry(oLineGeom, 2410007);
                    }

                    for (int i = 0; i < SourceFeature.Count; i++)
                    {

                        IGTPoint temp = GTClassFactory.Create<IGTPoint>();
                        temp.X = tempp.X + SourceFeature[i].diffX[0];
                        temp.Y = tempp.Y + SourceFeature[i].diffY[0];
                        temp.Z = 0;

                        if (SourceFeature[i].lblText != "" && SourceFeature[i].lblText != "line")
                        {
                            IGTTextPointGeometry textTemp = GTClassFactory.Create<IGTTextPointGeometry>();
                            textTemp.Origin = temp;
                            textTemp.Text = SourceFeature[i].lblText;
                            textTemp.Rotation = SourceFeature[i].Rotation;
                            textTemp.Alignment = (Intergraph.GTechnology.API.GTAlignmentConstants)SourceFeature[i].Alighment;// GTAlignmentConstants.gtalBottomRight; ;
                           
                            mobjEditService.AddGeometry(textTemp, SourceFeature[i].styleId);

                        }
                        else if (SourceFeature[i].lblText != "line")
                        {
                            IGTOrientedPointGeometry symPoint = GTClassFactory.Create<IGTOrientedPointGeometry>();
                            symPoint.Origin = temp;
                            symPoint.Orientation = SourceFeature[i].Orientation;
                            mobjEditService.AddGeometry(symPoint, SourceFeature[i].styleId);

                        } 
                            else if (SourceFeature[i].lblText == "line")
                            {
                                IGTPolylineGeometry line = GTClassFactory.Create<IGTPolylineGeometry>();
                                int k = 0;
                                while (SourceFeature[i].X[k] != -1)
                                {
                                    IGTPoint Pline = GTClassFactory.Create<IGTPoint>();
                                    Pline.X = tempp.X + SourceFeature[i].diffX[k];
                                    Pline.Y = tempp.Y + SourceFeature[i].diffY[k];
                                    Pline.Z = 0;
                                    line.Points.Add(Pline);
                                    k++;
                                }
                                mobjEditService.AddGeometry(line, SourceFeature[i].styleId);
                            }
                    }


                    if (!Precision)//in non-precision mode
                    {
                        int distance = LengthBtwTwoPointsInt(SourceFeatureX, SourceFeatureY, tempp.X, tempp.Y);
                        if (distance<10000)
                            DistForm.txtDistance.Value = distance;
                        else MessageBox.Show("Copied Manhole too far from source feature!", "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    string addTostatus = " Distance  -  " + DistForm.txtDistance.Value +"m";
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location! Right Click  to exit." + addTostatus);
                    return;
                }
                #endregion

                #region rotating MH
                if (startplc == 2000)//rotating of manhole
                {
                    DistForm.MessageHelpChange(2); 
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;

                    IGTPoint tempp2 = GTClassFactory.Create<IGTPoint>();
                    tempp2.X = CopiedFeatureX;
                    tempp2.Y = CopiedFeatureY;
                    tempp2.Z = 0.0;
                    double Alfa = AngleBtwPoint(CopiedFeatureX, CopiedFeatureY, e.WorldPoint.X, e.WorldPoint.Y);
                    //double len = LengthBtwTwoPoints(CopiedFeatureX, CopiedFeatureY, e.WorldPoint.X, e.WorldPoint.Y);
                    IGTVector Orientation = GTClassFactory.Create<IGTVector>();

                    if (mobjEditServiceRotate.GeometryCount > 0)
                        mobjEditServiceRotate.RemoveAllGeometries();
                  

                    if (CopiedFeatureOrientation == null)
                        CopiedFeatureOrientation = GTClassFactory.Create<IGTVector>();

                    CopiedFeatureOrientation = Orientation.BuildVector(tempp, tempp2);
                    CopiedFeatureRotation = Alfa;

                    for (int i = 0; i < SourceFeature.Count; i++)
                    {
                        IGTPoint temp = GTClassFactory.Create<IGTPoint>();
                        temp.X = CopiedFeatureX + SourceFeature[i].diffX[0];
                        temp.Y = CopiedFeatureY + SourceFeature[i].diffY[0];
                        temp.Z = 0;

                        double Beta = AngleBtwPoint(CopiedFeatureX, CopiedFeatureY, temp.X, temp.Y);
                        IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
                        projectPoint = PointBasedOnEnteredLength(SourceFeature[i].dictCentre[0], Alfa + Beta, CopiedFeatureX, CopiedFeatureY);
                        if (SourceFeature[i].viewname == "VGC_MANHL_T" || SourceFeature[i].viewname == "VGC_MANHL_T2")
                        {
                            IGTPoint tempp3 = GTClassFactory.Create<IGTPoint>();
                            tempp3.X = CopiedFeatureX + SourceFeature[i].diffX[0];
                            tempp3.Y = CopiedFeatureY+ SourceFeature[i].diffY[0];
                            tempp3.Z = 0.0;
                            IGTTextPointGeometry textTemp = GTClassFactory.Create<IGTTextPointGeometry>();
                            textTemp.Origin = tempp3;
                            textTemp.Text = SourceFeature[i].lblText;
                            textTemp.Rotation = SourceFeature[i].Rotation;
                            textTemp.Alignment = (Intergraph.GTechnology.API.GTAlignmentConstants)SourceFeature[i].Alighment;// GTAlignmentConstants.gtalBottomRight; ;
                            mobjEditServiceRotate.AddGeometry(textTemp, SourceFeature[i].styleId);
                        }
                        else
                        if (SourceFeature[i].lblText != "" && SourceFeature[i].lblText != "line" )
                        {
                            IGTTextPointGeometry textTemp = GTClassFactory.Create<IGTTextPointGeometry>();
                            textTemp.Origin = projectPoint;
                            textTemp.Text = SourceFeature[i].lblText;
                            textTemp.Rotation = Alfa + SourceFeature[i].Rotation;
                            textTemp.Alignment = 0;
                            mobjEditServiceRotate.AddGeometry(textTemp, SourceFeature[i].styleId);

                        }
                        else if (SourceFeature[i].lblText != "line")
                        {
                            IGTPoint PointForOrient = GTClassFactory.Create<IGTPoint>();
                            PointForOrient = PointBasedOnEnteredLength(1, Alfa + SourceFeature[i].Rotation, CopiedFeatureX, CopiedFeatureY);
                            IGTOrientedPointGeometry symPoint = GTClassFactory.Create<IGTOrientedPointGeometry>();
                            symPoint.Origin = projectPoint; 
                            symPoint.Orientation = Orientation.BuildVector(projectPoint, PointForOrient); //Orientation.AddVectors(SourceFeature[i].Orientation,Orientation.BuildVector(tempp,tempp2));//
                            mobjEditServiceRotate.AddGeometry(symPoint, SourceFeature[i].styleId);
                            CopiedFeatureOrientation = symPoint.Orientation;
                        } 
                            else if (SourceFeature[i].lblText == "line")
                        {
                          
                                IGTPolylineGeometry line = GTClassFactory.Create<IGTPolylineGeometry>();
                                int k = 0;
                                while (SourceFeature[i].X[k] != -1)
                                {
                                    double Beta1 = AngleBtwPoint(CopiedFeatureX, CopiedFeatureY, CopiedFeatureX + SourceFeature[i].diffX[k], CopiedFeatureY + SourceFeature[i].diffY[k]);
                                    IGTPoint Pline = GTClassFactory.Create<IGTPoint>();
                                    Pline = PointBasedOnEnteredLength(SourceFeature[i].dictCentre[k], Alfa + Beta1, CopiedFeatureX, CopiedFeatureY);
                        
                                    line.Points.Add(Pline);
                                    k++;
                                }
                                mobjEditServiceRotate.AddGeometry(line, SourceFeature[i].styleId);
                            }
                      
                    }
                    
                    string addTostatus = " Distance  -  " + DistForm.txtDistance.Value.ToString() + "m";
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm rotation! Right Click  to skip rotation." + addTostatus);
                    return;
                }
                #endregion

                #region selecting source MH
                if (startplc == 10000) //select source Manhole feature to start copying
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source Manhole feature! Right Click  to exit.");
                    DistForm.MessageHelpChange(0);

                    if (m_gtapp.SelectedObjects.FeatureCount == 1)
                    {

                        if (SourceFeature == null)
                            SourceFeature = new List<GeometryDesc>();
                        
                        IGTDDCKeyObjects feat = GTClassFactory.Create<IGTDDCKeyObjects>();
                        foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                        {

                            if (oDDCKeyObject.FNO != 2700)
                            {
                                DistForm.Hide();
                                MessageBox.Show("Please select a Manhole!", "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                DistForm.Show();
                                m_gtapp.ActiveMapWindow.Activate();
                                m_gtapp.SelectedObjects.Clear();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source Manhole feature! Right Click  to exit.");
                                return;
                            }
                            feat = m_gtapp.DataContext.GetDDCKeyObjects(oDDCKeyObject.FNO, oDDCKeyObject.FID, GTComponentGeometryConstants.gtddcgAllGeographic);
                            break;
                           
                        }
                       
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
                       
                        foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                        {
                            /////////////////////
                            ////////////////////
                            /////////////////////
                            //////////////////////
                            //take style for copying mh
                            IGTKeyObject mhfeature = m_gtapp.DataContext.OpenFeature(oDDCKeyObject.FNO, oDDCKeyObject.FID);

                            string ManholeType = "JC9";
                            string MH_MIX = "";
                            if (!mhfeature.Components.GetComponent(2701).Recordset.EOF)
                            {
                                ManholeType = mhfeature.Components.GetComponent(2701).Recordset.Fields["FEATURE_TYPE"].Value.ToString();
                                MH_MIX = mhfeature.Components.GetComponent(2701).Recordset.Fields["MH_MIX"].Value.ToString();
                            }


                            IGTHelperService helper = GTClassFactory.Create<IGTHelperService>();
                            helper.DataContext = m_gtapp.DataContext;

                            ManhlStyle.SymbolStyle = helper.GetComponentStyleID(mhfeature, 2720);

                            string manholeLabel = "[ManholeLabel]";
                            GTAlignmentConstants ManholeLabelAlignment = GTAlignmentConstants.gtalBottomLeft;

                            ManhlStyle.LabelIDStyle = helper.GetComponentStyleID(mhfeature, 2730);
                            helper.GetComponentLabel(mhfeature, 2730, ref manholeLabel, ref ManholeLabelAlignment);
                            ManhlStyle.LabelAlignID = ManholeLabelAlignment;
                            ManhlStyle.LabelIDContent = "???";
                            helper.GetComponentLabel(mhfeature, 2734, ref manholeLabel, ref ManholeLabelAlignment);
                            ManhlStyle.LabelAlignDesc = ManholeLabelAlignment;
                            ManhlStyle.LabelDescStyle = helper.GetComponentStyleID(mhfeature, 2734);
                            string decription = "";
                            if (MH_MIX.ToUpper() == "PRE-FAB")
                                decription = ManholeType + "/(P)";
                            else if (MH_MIX.ToUpper() == "PRE-FAB (SHUTTERING)")
                                decription = ManholeType + "/(P/S)";
                            else if (MH_MIX.ToUpper() == "SITE")
                                decription = ManholeType;
                            else decription = ManholeType + MH_MIX;

                            // REPLACE  "NEW LINE" MORE THAN 1 WITH 1 "NEW LINE"
                            //  decription=decription.Replace("\s{2,}","\n");


                            ManhlStyle.LabelDescContent = decription;
                            ManhlStyle.WallNumStyle = helper.GetComponentStyleID(mhfeature, 2732);
                            ManhlStyle.LineStyle = helper.GetComponentStyleID(mhfeature, 2710);
                            ManhlStyle.LeaderLineStyle = helper.GetComponentStyleID(mhfeature, 2712);
                            ///////////////////
                            ///////////////////
                            
                            GeometryDesc temp = new GeometryDesc();
                            temp.FID = oDDCKeyObject.FID;
                            temp.viewname = oDDCKeyObject.ComponentViewName;
                            temp.type = oDDCKeyObject.Geometry.Type;
                            if (temp.type != "CompositePolylineGeometry")
                            {
                                temp.X = new double[2];
                                temp.Y = new double[2];
                                temp.dictCentre = new double[2];
                                temp.diffX = new double[2];
                                temp.diffY = new double[2];
                                temp.X[0] = oDDCKeyObject.Geometry.FirstPoint.X;
                                temp.Y[0] = oDDCKeyObject.Geometry.FirstPoint.Y;
                                temp.X[1] = -1;
                                temp.Y[1] = -1;                               
                                temp.Orientation = ((IGTOrientedPointGeometry)oDDCKeyObject.Geometry).Orientation;
                                temp.Rotation = AngleBtwPoint(temp.Orientation.I, temp.Orientation.J);
                            }
                            else
                            {
                                IGTPolylineGeometry tt = (IGTPolylineGeometry)((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).ExtractGeometry(((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).FirstPoint, ((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).LastPoint, false);
                           
                                int count = tt.Points.Count+1;
                                temp.X = new double[count];
                                temp.Y = new double[count];
                                temp.dictCentre = new double[count];
                                temp.diffX = new double[count];
                                temp.diffY = new double[count];
                            }
                             if (oDDCKeyObject.ComponentViewName == "VGC_MANHL_S")
                            {

                                SourceFeatureFID = oDDCKeyObject.FID;
                                SourceFeatureX = oDDCKeyObject.Geometry.FirstPoint.X;
                                SourceFeatureY = oDDCKeyObject.Geometry.FirstPoint.Y;
                              //  string type = Get_Value("select FEATURE_TYPE from GC_MANHL where G3E_FID=" + SourceFeatureFID);
                              //  //type = type;// +" PPF";
                              //  string tt = "select  MIN(G3E_SNO) from G3E_STYLERULE where G3E_SRNO=272001 " +
                              //              " and upper(G3E_FILTER) like upper('%PPF%') " +
                              //              " and upper(G3E_FILTER) like upper('%" + type + "%') ";
                              //  string styletmp = Get_Value(tt);
                         
                              ////  IGTHelperService helperService = GTClassFactory.Create<IGTHelperService>();
                              ////  helperService.DataContext = m_IGTDataContext;
                              ////  ManholeStyleId = helperService.GetComponentStyleID(m_IGTDataContext.OpenFeature(2700, SourceFeatureFID), 2720);

                              //  if (styletmp == "")
                              //      ManholeStyleId = 2720044;

                              //  else ManholeStyleId = int.Parse(styletmp);
                                ManholeStyleId = ManhlStyle.SymbolStyle;
                                temp.styleId = ManholeStyleId;
                            } else
                            if (oDDCKeyObject.ComponentViewName == "VGC_MANHLW_T")
                            {
                                temp.styleId = ManhlStyle.WallNumStyle;//30600;
                                for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                                {
                                    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "WALL_NUM")
                                    {
                                        temp.lblText = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                                        
                                    }
                                    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "G3E_ALIGNMENT")
                                    {
                                        temp.Alighment = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());
                                    }
                                }
                           
                            } else
                                if (oDDCKeyObject.ComponentViewName == "VGC_MANHL_T")
                                {
                                    temp.styleId = ManhlStyle.LabelIDStyle;//27300102;
                                    //string FEATURE_TYPE = "";
                                    //for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                                    //{

                                    //    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "FEATURE_TYPE")
                                    //    {
                                    //        FEATURE_TYPE = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                                    //    }
                                    //    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "G3E_ALIGNMENT")
                                    //    {
                                    //        temp.Alighment = int.Parse(oDDCKeyObject.Recordset.Fields[i].Value.ToString());

                                    //    }
                                    //}
                                    temp.Alighment = (int)ManhlStyle.LabelAlignID;
                                    temp.lblText = ManhlStyle.LabelIDContent;//"???\n" + FEATURE_TYPE;

                                }
                                else
                                    if (oDDCKeyObject.ComponentViewName == "VGC_MANHL_T2")
                                    {
                                        temp.styleId = ManhlStyle.LabelDescStyle; //30600;
                                        //string FEATURE_TYPE = "";
                                        //string MH_MIX = "";
                                        //for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                                        //{

                                        //    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "FEATURE_TYPE")
                                        //    {
                                        //        FEATURE_TYPE = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                                        //    }
                                        //    if (oDDCKeyObject.Recordset.Fields[i].Name.ToString() == "MH_MIX")
                                        //    {
                                        //        MH_MIX = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                                        //    }
                                        //    if (FEATURE_TYPE != "" && MH_MIX != "")
                                        //        break;
                                        //}
                                        temp.lblText = ManhlStyle.LabelDescContent;//"???\n" + FEATURE_TYPE + "/" + MH_MIX;
                                        temp.Alighment = (int)ManhlStyle.LabelAlignDesc;
                                    }
                            else
                                if (oDDCKeyObject.ComponentViewName == "VGC_MANHL_L")
                                {
                                    temp.styleId = ManhlStyle.LineStyle; //13508;
                                    IGTPolylineGeometry tt = (IGTPolylineGeometry)((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).ExtractGeometry(((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).FirstPoint, ((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).LastPoint, false);

                                    int count = tt.Points.Count;
                                    for (int i = 0; i < count; i++)
                                    {
                                        temp.X[i] = tt.Points[i].X;
                                        temp.Y[i] = tt.Points[i].Y;                                      
                                    }
                                    temp.X[count] = -1;
                                    temp.Y[count] = -1;
                                    temp.lblText = "line";

                                }
                           else
                                    if (oDDCKeyObject.ComponentViewName == "VGC_MANHLLDR_L")
                                    {
                                        temp.styleId = ManhlStyle.LeaderLineStyle; //13501;
                                        IGTPolylineGeometry tt = (IGTPolylineGeometry)((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).ExtractGeometry(((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).FirstPoint, ((IGTCompositePolylineGeometry)oDDCKeyObject.Geometry).LastPoint, false);

                                        int count = tt.Points.Count;
                                        for (int i = 0; i < count; i++)
                                        {
                                            temp.X[i] = tt.Points[i].X;
                                            temp.Y[i] = tt.Points[i].Y;
                                        }
                                        temp.X[count] = -1;
                                        temp.Y[count] = -1;
                                        temp.lblText = "line";
                                    }

                            SourceFeature.Add(temp); 
                        }
                        for (int i = 0; i < SourceFeature.Count; i++)
                        {
                            int j = 0;
                            while (SourceFeature[i].X[j] != -1)
                            {
                                SourceFeature[i].diffX[j] = SourceFeature[i].X[j] - SourceFeatureX;
                                SourceFeature[i].diffY[j] = SourceFeature[i].Y[j] - SourceFeatureY;
                                SourceFeature[i].dictCentre[j] = LengthBtwTwoPoints(SourceFeatureX, SourceFeatureY, SourceFeature[i].X[j], SourceFeature[i].Y[j]);
                                j++;
                            }
                            SourceFeature[i].diffX[j] = -1;
                            SourceFeature[i].diffY[j] = -1;
                            SourceFeature[i].dictCentre[j] = -1;

                            IGTPoint temp = GTClassFactory.Create<IGTPoint>();
                            temp.X = SourceFeature[i].X[0];
                            temp.Y = SourceFeature[i].Y[0];
                            temp.Z = 0;

                            if (SourceFeature[i].lblText != "" && SourceFeature[i].lblText != "line")
                            {
                                IGTTextPointGeometry textTemp = GTClassFactory.Create<IGTTextPointGeometry>();
                                
                                textTemp.Origin = temp;
                                textTemp.RichText = SourceFeature[i].lblText;
                                textTemp.Rotation = SourceFeature[i].Rotation;
                                textTemp.Alignment = 0;
                                mobjEditService.AddGeometry(textTemp, SourceFeature[i].styleId);                               
                            }
                            else if (SourceFeature[i].lblText != "line")
                            {
                                IGTOrientedPointGeometry symPoint = GTClassFactory.Create<IGTOrientedPointGeometry>();
                                symPoint.Origin = temp;
                                symPoint.Orientation = SourceFeature[i].Orientation;
                                mobjEditService.AddGeometry(symPoint, SourceFeature[i].styleId);

                            }
                            else if (SourceFeature[i].lblText == "line")
                            {
                                IGTPolylineGeometry line = GTClassFactory.Create<IGTPolylineGeometry>();
                                int k = 0;
                                while (SourceFeature[i].X[k] != -1)
                                {
                                    IGTPoint Pline = GTClassFactory.Create<IGTPoint>();
                                    Pline.X = SourceFeature[i].X[k];
                                    Pline.Y = SourceFeature[i].Y[k];
                                    Pline.Z = 0;
                                    line.Points.Add(Pline);
                                    k++;
                                }

                                mobjEditService.AddGeometry(line, SourceFeature[i].styleId);
                            }

                        }                      

                        startplc = 1000;
                        DistForm.MessageHelpChange(1);
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confrim location! Right Click  to exit.");  

                    }
                    else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                    {
                        DistForm.Hide();
                        MessageBox.Show("Please select only one Manhole at once!", "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_gtapp.SelectedObjects.Clear();
                        DistForm.MessageHelpChange(0);
                        DistForm.Show();
                        m_gtapp.ActiveMapWindow.Activate();                       
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source Manhole feature! Right Click  to exit."); 
                        return;
                    }
                    return;
                }
                #endregion

                #region waiting for final confirmation MH
                if (startplc == 5000)//update status bar while waiting for final confirmation for placement
                {
                    DistForm.MessageHelpChange(3);
                    if (mobjEditServiceRotate.GeometryCount > 0)
                        mobjEditServiceRotate.RemoveAllGeometries();
                    int distance = LengthBtwTwoPointsInt(SourceFeatureX, SourceFeatureY, CopiedFeatureX, CopiedFeatureY);
                    if (distance < 10000)
                        DistForm.txtDistance.Value = distance;
                    else MessageBox.Show("Copied Manhole too far from source feature!", "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                   
                    string addTostatus = " Distance  -  " + DistForm.txtDistance.Value + "m";
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete copying! Right Click  to change location." + addTostatus);
                    return;
                }
                #endregion

        #endregion

                #region DUCT PATH
               
                #region Get Selected Source Wall OR Term Wall of Device
                if (startdraw == 300 || startdraw == 400)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select wall of civil feature! Right Click to cancel re-selection.");

                    if (m_gtapp.SelectedObjects.FeatureCount == 1)
                    {
                        if ((startdraw == 300 && m_CustomForm.GetDeviceWithWall(true))//source
                            || (startdraw == 400 && m_CustomForm.GetDeviceWithWall(false)))//term
                        {
                            startdraw = 0;
                            m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                            return;
                        }
                        else return;
                    }

                }
                #endregion

                #region For Conduit Line Placement
                if (startdraw == 1 && StartDrawPoint != null)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm point of turn! Double Click to complete drawing.");

                    //   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                    IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                    IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    Point1.X = e.WorldPoint.X;
                    Point1.Y = e.WorldPoint.Y;
                    Point1.Z = 0.0;
                    oLineGeom.Points.Add(StartDrawPoint);
                    oLineGeom.Points.Add(Point1);
                    if (middledraw == 0)
                    {
                        if (mobjEditService.GeometryCount > 0)
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);// .RemoveAllGeometries();
                    }

                    mobjEditService.AddGeometry(oLineGeom, styleid);

                    if (middledraw == 1) middledraw = 0;
                    return;
                }
                if (startdraw == 1)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm point of turn! Double Click to complete drawing.");//"LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                    return;
                }
                if (startdraw == 101)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete drawing! Right Click to re-draw.");//"LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                    return;
                }
                #endregion
                #region For Section Slash plc

                if (startdraw == 2)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to move along line! Double Click to confirm location! Right Click to cancel slash placement.");

                    IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;
                    temp.Origin = tempp;
                    if (mobjEditService.GeometryCount > 0)
                    {
                        if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    mobjEditService.AddGeometry(temp, 2220004);
                    return;
                }
                if (startdraw == 20)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to move along line! Double Click to confirm location! Right Click to start moving follow cursor.");
                    return;
                }
                #endregion
                #region For Section Label Placement
                if (startdraw == 3)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location! Right Click to place along line.");

                    IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                    IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                    Point1.X = e.WorldPoint.X;
                    Point1.Y = e.WorldPoint.Y;
                    Point1.Z = 0.0;
                    oTextGeom.Origin = Point1;
                    oTextGeom.Alignment = 0;
                    oTextGeom.Text = m_CustomForm.SectLabel;
                    if (mobjEditService.GeometryCount > 0)
                    {
                        if (mobjEditService.GetGeometry(mobjEditService.GeometryCount - 1).Type == "PolylineGeometry"
                            && mobjEditService.GeometryCount != 2)
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    //leader line first
                    IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    oLineGeom.Points.Add(LabelAlongLine.FirstPoint);
                    oLineGeom.Points.Add(Point1);
                    mobjEditService.AddGeometry(oLineGeom, 11509);//14510 14509
                    //label
                    mobjEditService.AddGeometry(oTextGeom, 32400);
                    return;
                }
                if (startdraw == 30)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete!Right Click to start moving along section line.");
                    return;
                }
                if (startdraw == 38)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete!Right Click to start moving follow cursor.");
                    return;
                }
                #endregion
                #region For Moving label along line plc
                if (startdraw == 39)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to choose new position! Right Click to place in middle of section.");

                    IGTTextPointGeometry temp = GTClassFactory.Create<IGTTextPointGeometry>();
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;
                    temp.Origin = tempp;
                    IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);

                    if (projectPoint.X == 0 && projectPoint.Y == 0)
                    {
                        //MessageBox.Show("Please, place Label on creating Conduit Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        temp.Origin = projectPoint;
                        //IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                        //temp.Orientation = Orientation.BuildVector( projectPoint,tempp);
                        int length = int.Parse(projectPoint.Z.ToString());
                        if (m_CustomForm.SectSlashes.Count > 0)
                        {
                            if (m_CustomForm.LastSection != 1)
                                if (length >= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                {
                                    // MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                            if (m_CustomForm.SectSlashes.Count > 1)
                            {
                                if (m_CustomForm.LastSection == 1)
                                {
                                    if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                    {
                                        //   MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                }
                                else
                                    if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 2].length)
                                    {
                                        //  MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                            }
                        }

                        temp.Rotation = m_CustomForm.TakeRotationOfSegmentPolyline(length);// OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);
                        temp.Text = LabelAlongLine.Text;
                        temp.Alignment = 0;
                        if (mobjEditService.GeometryCount > 0)
                        {
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        mobjEditService.AddGeometry(temp, 32400);
                        // oTextGeomLabel.Origin = temp.Origin;
                        // oTextGeomLabel.Text = temp.Text;
                        // oTextGeomLabel.Rotation = temp.Rotation;
                        // oTextGeomLabel.Alignment = 0;
                    }
                    return;
                }
                #endregion
                #region Rotate Label
                if (startdraw == 5)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm rotation! Right Click to skip rotation.");

                    IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                    IGTPoint Point2 = GTClassFactory.Create<IGTPoint>();
                    IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                    Point1.X = e.WorldPoint.X;
                    Point1.Y = e.WorldPoint.Y;
                    Point1.Z = 0.0;
                    Point2.X = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.X;
                    Point2.Y = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.Y;
                    Point2.Z = 0.0;
                    if (mobjEditService.GeometryCount > 0)
                        while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    oTextGeom.Origin = Point2;
                    oTextGeom.Text = m_CustomForm.SectLabel;
                    oTextGeom.Alignment = 0;
                    mobjEditService.AddGeometry(oTextGeom, 32400);
                    oTextGeom.Rotation = m_CustomForm.AngleBtwPoint(Point2.X, Point2.Y, Point1.X, Point1.Y);
                    mobjEditService.AddGeometry(oTextGeom, 32400);
                    oTextGeomLabel.Origin = Point2;
                    oTextGeomLabel.Text = m_CustomForm.SectLabel;
                    oTextGeomLabel.Rotation = oTextGeom.Rotation;

                }
                #endregion
                #region For Leader Line Section Label Placement
                if (startdraw == 35)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm point of turn! Double Click to complete drawing! Right Click to re-draw.");

                    IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                    Point1.X = e.WorldPoint.X;
                    Point1.Y = e.WorldPoint.Y;
                    Point1.Z = 0.0;

                    if (PointForLeaderLine == null)
                    {
                        PointForLeaderLine = GTClassFactory.Create<IGTPoint>();
                        PointForLeaderLine.X = LabelAlongLine.FirstPoint.X;
                        PointForLeaderLine.Y = LabelAlongLine.FirstPoint.Y;
                        PointForLeaderLine.Z = LabelAlongLine.FirstPoint.Z;
                    }

                    oLineGeom.Points.Add(PointForLeaderLine);
                    oLineGeom.Points.Add(Point1);
                    if (middledraw == 0)
                    {
                        if (mobjEditService.GeometryCount > 0)
                        {
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "PolylineGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                    }

                    mobjEditService.AddGeometry(oLineGeom, 12501);

                    if (middledraw == 1) middledraw = 0;
                    return;
                }
                if (startdraw == 31)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete! Right Click to delete leader line.");
                    return;
                }
                if (startdraw == 32)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete! Right Click to place along line.");
                    return;
                }
                #endregion
         

                #endregion

            }
            catch (Exception ex)
            {
                if (DistForm != null)
                    DistForm.Hide();
                MessageBox.Show(ex.Message, "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }
        #endregion

        #region Press key ESC 
        void m_oIGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == 27)//button ESC
                {
                #region MANHOLE
               

                    if (startplc == 2000 || startplc == 5000)//start moving MANHOLE  again, 2- while choosing rotation, 5- while waiting for final placement confirmation
                    {
                        DistForm.MessageHelpChange(1);
                        startplc = 1000;
                        return;
                    }

                    if (startplc == 10000 || startplc == 1000)//exiting while moving MANHOLE or before selected source MANHOLE feature
                    {
                        if (DistForm != null)
                            DistForm.Hide();
                       DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Copy Manhole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                    
                #endregion

                #region DUCT PATH
                #region start drawing with mouse moving Conduit Line Placement
                if (startdraw == 101)
                {
                    if (mobjEditService.GeometryCount > 0)
                        mobjEditService.RemoveAllGeometries();
                    m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    startdraw = 1;
                    //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                }
                #endregion

                #region Cancelation for Wall or Device Selection
                if (startdraw == 300 || startdraw == 400 || startdraw == 500 || startdraw == 600)
                {
                    m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                    startdraw = 0;
                    // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                }
                #endregion

                #region Cancellation of placement for section's slash
                if (startdraw == 2)
                {
                    if (m_CustomForm.LastSection == 0)
                    {
                        m_gtapp.SelectedObjects.Clear();
                        if (mobjEditService.GeometryCount > 0)
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        startdraw = 0;
                        m_CustomForm.LastSection = 1;
                        // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cancellation of placement for section's slash! Next section will be the last");
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        m_CustomForm.ConfSelBtnIsEnable = true;
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                        return;
                    }

                }

                if (startdraw == 20)
                {
                    startdraw = 2;
                    m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    return;
                }
                #endregion

                #region Placement for section's label along the line
                if (startdraw == 3 || startdraw == 5 || startdraw == 32 || (startdraw == 30 && oTextGeomLabel.Rotation != LabelAlongLine.Rotation))
                {
                    if (mobjEditService.GeometryCount > 0)
                    {
                        while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        if (mobjEditService.GetGeometry(mobjEditService.GeometryCount ).Type == "PolylineGeometry" &&
                            mobjEditService.GeometryCount != 1)
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    mobjEditService.AddGeometry(LabelAlongLine, 32400);

                    oTextGeomLabel.Origin = LabelAlongLine.Origin;
                    oTextGeomLabel.Text = m_CustomForm.SectLabel;
                    oTextGeomLabel.Rotation = LabelAlongLine.Rotation;
                    startdraw = 30;
                    return;
                }
                //start moving again
                if (startdraw == 30 && oTextGeomLabel.Rotation == LabelAlongLine.Rotation)
                {
                    startdraw = 3;
                    m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    return;
                }
                #endregion

                #region Start drawing leader line from beginning
                if (startdraw == 35)
                {
                    //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                    if (mobjEditService.GeometryCount > 0)
                    {
                        while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    PointForLeaderLine = null;
                    middledraw = 0;
                    return;
                }
                //after stopping drawing before final acception
                if (startdraw == 31)
                {
                    //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                    if (mobjEditService.GeometryCount > 0)
                    {
                        while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    PointForLeaderLine = null;
                    middledraw = 0;
                    startdraw = 32;
                    return;
                }
                #endregion
                #endregion
                
                    return;

            }
            }
            catch (Exception ex)
            {
                if (DistForm != null)
                    DistForm.Hide();
                MessageBox.Show(ex.Message, "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }

        }

        #endregion

        #region Mouse DOuble Click
        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {

            try
            { 
           #region LEFT CLICK
                if (e.Button != 2)
                {
                #region MANHOLE

                if (startplc == 5000)//final confirmation for placement
                {
                    startplc = 0;
                    DistForm.Hide();
                    
                    Messages frmMsg = new Messages();
                    frmMsg.Message(1);
                    frmMsg.Show();

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, copying in process...");
                    m_gtapp.SetProgressBarRange(0, 100);
                    m_gtapp.SetProgressBarPosition(50);

                    if (!CopyManhole())//create manhole
                    {
                        frmMsg.Close();
                        ExitCmd();
                    }
                    frmMsg.Message(3);
                    CopiedFeatureOrientation=null;
                    if (mobjEditService.GeometryCount > 0)
                        mobjEditService.RemoveAllGeometries();
                    m_gtapp.SetProgressBarPosition(50);
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Copying successfully completed!");
                    m_gtapp.SetProgressBarRange(0, 0);
                    frmMsg.Close();
                    DialogResult retVal = MessageBox.Show("Do you want to place Duct Path between Source and Copied Manholes?", "Copy Manhole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        if (m_CustomForm == null)
                        {
                            m_CustomForm = new GTWindowsForm_DuctPathPlc();
                        }
                        startplc = 15000;
                        #region Get Selected Source and Termineted Devices
                          if (m_CustomForm.GetSourceDevice(2700, SourceFeatureFID) && m_CustomForm.GetTermDevice(2700, CopiedFeatureFID))
                                            {
                                                startdraw = 0;
                                                m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                                                
                                            }
                                            else
                                            {
                                                m_gtapp.SelectedObjects.Clear();
                                                
                                            }
                        #endregion 
                     }
                    else
                    {
                        startplc = 1000; 
                        DistForm.MessageHelpChange(1);
                        DistForm.Show();
                        m_gtapp.ActiveMapWindow.Activate();
                    }

                    SourceFeatureFID = CopiedFeatureFID;
                    SourceFeatureX = CopiedFeatureX;
                    SourceFeatureY = CopiedFeatureY;
                    CopiedFeatureFID = 0;
                    CopiedFeatureX = 0;
                    CopiedFeatureY = 0;
                    return;
                }
                #endregion

                #region DUCT PATH
                #region For Conduit Line plc
                if ((startdraw == 1 || startdraw == 101) && StartDrawPoint != null)
                {
                    IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    oLineGeom.Points.Add(StartDrawPoint);

                    if (startdraw == 1)
                    {
                        IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                        Point1.X = e.WorldPoint.X;
                        Point1.Y = e.WorldPoint.Y;
                        Point1.Z = 0.0;
                        oLineGeom.Points.Add(Point1);
                    }

                    oLineGeom.Points.Add(EndDrawPoint);
                    if (middledraw == 0)
                    {
                        if (mobjEditService.GeometryCount > 0)
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    mobjEditService.AddGeometry(oLineGeom, styleid);
                    StartDrawPoint.X = e.WorldPoint.X;
                    StartDrawPoint.Y = e.WorldPoint.Y;
                    StartDrawPoint.Z = 0.0;
                    if (middledraw == 1) middledraw = 0;
                    startdraw = 0;
                    //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "PRESS 'Start Drawing' BUTTON TO REDRAW GRAPHIC LINE OF CONDUIT OR 'Get Selected' BUTTON TO RESELECT TERMINATED DEVICE");
                    m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                }
                 #endregion

                #region first sttep of confirmation Leader Line Section Label Placement
                if (startdraw == 35)
                {
                    IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                    Point1.X = e.WorldPoint.X;
                    Point1.Y = e.WorldPoint.Y;
                    Point1.Z = 0.0;

                    if (PointForLeaderLine == null)
                    {
                        PointForLeaderLine = GTClassFactory.Create<IGTPoint>();
                        PointForLeaderLine.X = LabelAlongLine.FirstPoint.X;
                        PointForLeaderLine.Y = LabelAlongLine.FirstPoint.Y;
                        PointForLeaderLine.Z = LabelAlongLine.FirstPoint.Z;
                    }

                    oLineGeom.Points.Add(PointForLeaderLine);
                    oLineGeom.Points.Add(Point1);
                    if (middledraw == 0)
                    {
                        if (mobjEditService.GeometryCount > 0)
                        {
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "PolylineGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                    }

                    mobjEditService.AddGeometry(oLineGeom, 12501);

                    if (middledraw == 1) middledraw = 0;
                    startdraw = 31;
                    return;
                }
                #endregion

                #region For Section Slash plc

                if (startdraw == 20)
                {

                    //IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    //IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    //tempp.X = e.WorldPoint.X;// mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.X;
                    //tempp.Y = e.WorldPoint.Y; //mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.Y;
                    //tempp.Z = 0.0;
                    //temp.Origin = tempp;

                    //IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);
                    //int length = int.Parse(projectPoint.Z.ToString());
                    //temp.Orientation = m_CustomForm.OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length); 
                    //mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    //            mobjEditService.AddGeometry(temp, 2220006);
                    //            m_CustomForm.AddSectSlash(projectPoint.X, projectPoint.Y, length, temp.Orientation);
                    //            m_gtapp.SelectedObjects.Clear();
                    //            startdraw = 0;
                    //            m_CustomForm.ConfSelBtnIsEnable = true;
                    //            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();

                    IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;
                    temp.Origin = tempp;
                    IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);

                    if (projectPoint.X == 0 && projectPoint.Y == 0)
                    {
                        MessageBox.Show("Please, place Slash Point on creating Conduit Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        temp.Origin = projectPoint;
                        //IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                        //temp.Orientation = Orientation.BuildVector( projectPoint,tempp);
                        int length = int.Parse(projectPoint.Z.ToString());
                        if (m_CustomForm.SectSlashes.Count > 0)
                        {
                            if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                            {
                                MessageBox.Show("Not allowed place Slash for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }

                        temp.Orientation = m_CustomForm.OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);

                        if (mobjEditService.GeometryCount > 0)
                        {
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        mobjEditService.AddGeometry(temp, 2220006);
                        m_CustomForm.AddSectSlash(projectPoint.X, projectPoint.Y, length, temp.Orientation);
                        m_gtapp.SelectedObjects.Clear();
                        startdraw = 0;
                        m_CustomForm.ConfSelBtnIsEnable = true;
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                        return;
                    }


                }
                #endregion

                #region For Section Label Placement
                if (startdraw == 30 || startdraw == 32 || startdraw == 38)
                {
                    // IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                    if (mobjEditService.GeometryCount > 0)
                    {
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    mobjEditService.AddGeometry(oTextGeomLabel, 32500);
                    m_CustomForm.SectionLabelAdd(oTextGeomLabel.Origin.X, oTextGeomLabel.Origin.Y, oTextGeomLabel.Rotation, null);
                    oTextGeomLabel = null;
                    m_gtapp.SelectedObjects.Clear();

                    if (m_CustomForm.LastSection == 0)
                    {
                        m_gtapp.SelectedObjects.Clear();
                        startdraw = 2;
                    }
                    else
                    {
                        startdraw = 0;
                        m_gtapp.SelectedObjects.Clear();
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, ""); m_CustomForm.Show();
                        if (m_CustomForm.LastSection == 2)
                        {
                            m_CustomForm.ConfSelBtnIsEnable = true;
                            m_CustomForm.LastSection = 1;
                        }
                    }
                    return;

                }

                //label with leader line
                if (startdraw == 31)
                {

                    IGTPolylineGeometry LeaderLine = GTClassFactory.Create<IGTPolylineGeometry>();
                    IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                    if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                    {
                        geom = mobjEditService.GetGeometry(mobjEditService.GeometryCount);
                        LeaderLine.Points.Add(geom.FirstPoint);
                        for (int i = mobjEditService.GeometryCount; i >= 1; i--)
                        {
                            geom = mobjEditService.GetGeometry(i);
                            if (geom.Type != "TextPointGeometry")
                            {
                                LeaderLine.Points.Add(geom.FirstPoint);
                                mobjEditService.RemoveGeometry(i);
                            }
                            else break;
                        }
                    }

                    if (mobjEditService.GeometryCount > 0)
                    {
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }

                    mobjEditService.AddGeometry(LeaderLine, 13002);
                    mobjEditService.AddGeometry(oTextGeomLabel, 32500);

                    m_CustomForm.SectionLabelAdd(oTextGeomLabel.Origin.X, oTextGeomLabel.Origin.Y, oTextGeomLabel.Rotation, LeaderLine);
                    oTextGeomLabel = null;
                    LeaderLine = null;
                    PointForLeaderLine = null;
                    m_gtapp.SelectedObjects.Clear();

                    if (m_CustomForm.LastSection == 0)
                    {
                        m_gtapp.SelectedObjects.Clear();
                        startdraw = 2;
                    }
                    else
                    {
                        startdraw = 0;
                        m_gtapp.SelectedObjects.Clear();
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, ""); m_CustomForm.Show();
                        if (m_CustomForm.LastSection == 2)
                        {
                            m_CustomForm.ConfSelBtnIsEnable = true;
                            m_CustomForm.LastSection = 1;
                        }
                    }
                    return;
                }
                #endregion
                #endregion
                }
           #endregion
            }
            catch (Exception ex)
            {
                if (DistForm != null)
                    DistForm.Hide();
                MessageBox.Show(ex.Message, "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }

        #endregion

        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {

            m_gtapp = GTClassFactory.Create<IGTApplication>();
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Copy Manhole . . . ");
            m_oIGTCustomCommandHelper = CustomCommandHelper;
            mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>();
            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;
            mobjEditServiceRotate = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceRotate.TargetMapWindow = m_gtapp.ActiveMapWindow;
            m_IGTDataContext = m_gtapp.DataContext;
            mobjRelationshipService.DataContext = m_IGTDataContext; 
            mobjLocateService = m_gtapp.ActiveMapWindow.LocateService;

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
            }
            SubscribeEvents();

            startplc = 10000;
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source Manhole feature! Right Click  to exit.");

            
            
        }
                
       

        public bool CanTerminate
        {
            get
            {
                if (startdraw == 0 && startplc ==15000)
                    m_CustomForm.Hide();
                if (startplc != 15000)
                    DistForm.Hide();
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Copy Manhole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                    //  return true;
                }
                else
                {
                    if (startdraw == 0 && startplc == 15000)
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                    if( startplc !=15000)
                        DistForm.Show();
                    return false;
                }

                return false;
            }
        }

        public void Pause()
        {
            startplc += 50000;
            startdraw += 50000;
        }

        public void Resume()
        {
            startplc -= 50000;
            startdraw -= 50000;
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
           // m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
          //  m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
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
          //  m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
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

        #region Distance Form events
        private void Distance_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormClosing)
            {
                DistForm.Hide();
                DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Copy Manhole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        private void DistForm_MouseUp(object sender, MouseEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            if (e.Button == MouseButtons.Right)//right click
            {
                if (startplc == 2000 || startplc == 5000)//start moving pole symbol again, 2- while choosing rotation, 5- while waiting for final placement confirmation
                {

                    DistForm.MessageHelpChange(1);
                    startplc = 1000;
                    return;
                }

                if (startplc == 10000 || startplc == 1000)//exiting while moving pole symbol or before selected source pole feature
                {
                    if (DistForm != null)
                        DistForm.Hide();
                    DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Copy Manhole", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                
            }
        }

        #endregion

        #region Exit DuctPath Plc Form
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitCmd();
        }

        public void ExitCmd()
        {
            m_gtapp.SetProgressBarRange(0, 0);
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting..."); 
            m_gtapp.SelectedObjects.Clear();
            
            if (m_CustomForm != null)
            {
                m_CustomForm.CloseStatus = 10;
                m_CustomForm.Close();
                m_CustomForm.Dispose();
                m_CustomForm = null;

            }
            if (mobjLocateService != null)
                mobjLocateService = null;
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
            }
            if (mobjEditServiceRotate != null)
            {
                mobjEditServiceRotate.RemoveAllGeometries();
                mobjEditServiceRotate = null;
            }

            UnsubscribeEvents();
            if (SourceFeature != null)
            {
                SourceFeature.Clear();
                SourceFeature = null;
            }
           
            if (mobjRelationshipService != null)
            {
                mobjRelationshipService = null;
            }

            #region Manhole
            startplc=0;
            SourceFeatureFID = 0;
            SourceFeatureX = 0;
            SourceFeatureY = 0;
            CopiedFeatureFID = 0;
            CopiedFeatureX = 0;
            CopiedFeatureY = 0;
            if(CopiedFeatureOrientation!=null)
                CopiedFeatureOrientation = null;
            CopiedFeatureRotation= 0;
            if(SourceFeature !=null)
                SourceFeature = null;
            ManholeStyleId = 0;
            FormClosing = true;
            if (DistForm != null)
            {
                DistForm.Close();
                DistForm = null;
            }
            FormClosing = false;
             Precision = false;
            #endregion  
            #region Duct Path
            if(SelGeom!=null)
                SelGeom = null;
            if(LabelAlongLine!=null)
                LabelAlongLine = null;
            if(oTextGeomLabel!=null)
                oTextGeomLabel = null;
            startdraw = 7;
            middledraw = 0;
            if(PointForLeaderLine!=null)
                PointForLeaderLine = null;
            if (StartDrawPoint != null)
                StartDrawPoint = null;
            if (EndDrawPoint != null)
                EndDrawPoint = null;
            styleid = 14500;
            #endregion     

            m_oIGTCustomCommandHelper.Complete();

        }
        #endregion

        #region Copy Manhole
        private bool CopyManhole()
        {
            try
            {
               
                short iCNO;

                short iFNO = 2700;      //Manhole
                int iFID;
                // progressBar1.Value = 5;
                m_oIGTTransactionManager.Begin("CopyManhole");

                IGTKeyObject oCopyFeature = m_IGTDataContext.OpenFeature(iFNO, SourceFeatureFID);
                IGTKeyObject oNewFeature =  m_IGTDataContext.NewFeature(iFNO);
                 iFID = oNewFeature.FID;
                 CopiedFeatureFID = iFID;
                iCNO = 51; //GC_NETELEM
                CopyAttributes(oCopyFeature.Components.GetComponent(iCNO).Recordset, oNewFeature.Components.GetComponent(iCNO).Recordset);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");

                iCNO = 2701; // Attribute
                CopyAttributes(oCopyFeature.Components.GetComponent(iCNO).Recordset, oNewFeature.Components.GetComponent(iCNO).Recordset);
                // Geometry
//2710	GC_MANHL_L	Manhole Geo Line*
//--2711	DGC_MANHL_P	Manhole Detail Polygon
//2712	GC_MANHLLDR_L	Manhole Geo Leader Line*
//--2713	DGC_MANHLLDR_L	Manhole Detail Leader Line
//2720	GC_MANHL_S	Manhole Geo Symbol
//2730	GC_MANHL_T	Manhole Geo Label
//--2731	DGC_MANHL_T	Manhole Detail Label
//2732	GC_MANHLW_T	Manhole Geo Wall Label
//2734	GC_MANHL_T2	Manhole Geo ID Label
//2736	GC_MANHL_T3	Manhole Geo IPID Label

                for (int i = 0; i < SourceFeature.Count; i++)
                {
                    if (SourceFeature[i].viewname == "VGC_MANHL_S")
                        iCNO = 2720;
                    else if (SourceFeature[i].viewname == "VGC_MANHL_T")
                        iCNO = 2730;
                    else if (SourceFeature[i].viewname == "VGC_MANHLW_T")
                        iCNO = 2732;
                    else if (SourceFeature[i].viewname == "VGC_MANHL_T2")
                        iCNO = 2734;
                    else if (SourceFeature[i].viewname == "VGC_MANHL_T3")
                        iCNO = 2736;
                    else if (SourceFeature[i].viewname == "VGC_MANHL_L")
                        iCNO = 2710;
                    else if (SourceFeature[i].viewname == "VGC_MANHLLDR_L")
                        iCNO = 2712;
                   
                    if (iCNO != 2732)
                    {
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
                    }
                    else
                    {
                       // if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                       // {
                            oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", SourceFeature[i].lblText);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("WALL_NUM", SourceFeature[i].lblText);
                      //  }
                    }

                    IGTPoint PointForOrient = GTClassFactory.Create<IGTPoint>();
                    PointForOrient = PointBasedOnEnteredLength(1, CopiedFeatureRotation + SourceFeature[i].Rotation, CopiedFeatureX, CopiedFeatureY);
                    IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
                    int k = 0;
                    if (SourceFeature[i].X[1] == -1)
                    {
                        if (SourceFeature[i].viewname != "VGC_MANHL_T" && SourceFeature[i].viewname != "VGC_MANHL_T2")
                        {

                            double Beta = AngleBtwPoint(CopiedFeatureX, CopiedFeatureY, CopiedFeatureX + SourceFeature[i].diffX[k], CopiedFeatureY + SourceFeature[i].diffY[k]);

                            projectPoint = PointBasedOnEnteredLength(SourceFeature[i].dictCentre[k], CopiedFeatureRotation + Beta, CopiedFeatureX, CopiedFeatureY);
                            IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                            SourceFeature[i].Orientation = Orientation.BuildVector(projectPoint, PointForOrient);
                            SourceFeature[i].Rotation = CopiedFeatureRotation + SourceFeature[i].Rotation;
                            SourceFeature[i].X[k] = projectPoint.X;
                            SourceFeature[i].Y[k] = projectPoint.Y;
                            SourceFeature[i].diffX[k] = SourceFeature[i].X[k] - CopiedFeatureX;
                            SourceFeature[i].diffY[k] = SourceFeature[i].Y[k] - CopiedFeatureY;
                        }
                        else
                        {
                            SourceFeature[i].X[k] = CopiedFeatureX + SourceFeature[i].diffX[0];
                            SourceFeature[i].Y[k] = CopiedFeatureY + SourceFeature[i].diffY[0];
                        }
                    }
                    else
                    {
                        
                        while (SourceFeature[i].X[k] != -1)
                        {
                            double Beta = AngleBtwPoint(CopiedFeatureX, CopiedFeatureY, CopiedFeatureX + SourceFeature[i].diffX[k], CopiedFeatureY + SourceFeature[i].diffY[k]);
                            
                            projectPoint = PointBasedOnEnteredLength(SourceFeature[i].dictCentre[k], CopiedFeatureRotation + Beta, CopiedFeatureX, CopiedFeatureY);
                            IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                            SourceFeature[i].Orientation = Orientation.BuildVector(projectPoint, PointForOrient);
                            SourceFeature[i].Rotation = CopiedFeatureRotation + SourceFeature[i].Rotation;
                            SourceFeature[i].X[k] = projectPoint.X;
                            SourceFeature[i].Y[k] = projectPoint.Y;
                            SourceFeature[i].diffX[k] = SourceFeature[i].X[k] - CopiedFeatureX;
                            SourceFeature[i].diffY[k] = SourceFeature[i].Y[k] - CopiedFeatureY;
                            k++;
                        }
                    }

                    if (SourceFeature[i].viewname == "VGC_MANHL_T" || SourceFeature[i].viewname == "VGC_MANHL_T2" )
                    {
                        IGTPoint tempp5 = GTClassFactory.Create<IGTPoint>();
                        tempp5.X = SourceFeature[i].X[0];
                        tempp5.Y = SourceFeature[i].Y[0];
                        tempp5.Z = 0.0;
                        IGTTextPointGeometry textTemp = GTClassFactory.Create<IGTTextPointGeometry>();
                        textTemp.Origin = tempp5;
                        textTemp.Text = SourceFeature[i].lblText;
                        textTemp.Rotation = SourceFeature[i].Rotation;
                        textTemp.Alignment = (Intergraph.GTechnology.API.GTAlignmentConstants)SourceFeature[i].Alighment;// GTAlignmentConstants.gtalBottomRight; ;
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", SourceFeature[i].Alighment);
                        oNewFeature.Components.GetComponent(iCNO).Geometry = textTemp;
                    }
                    else
                    if (SourceFeature[i].lblText != "" && SourceFeature[i].lblText != "line")
                    {
                      //  string temp = SourceFeature[i].viewname;
                        IGTTextPointGeometry textTemp = GTClassFactory.Create<IGTTextPointGeometry>();
                        textTemp.Origin = projectPoint;
                        textTemp.Text = SourceFeature[i].lblText;
                        textTemp.Rotation = SourceFeature[i].Rotation;
                        textTemp.Alignment = (Intergraph.GTechnology.API.GTAlignmentConstants)SourceFeature[i].Alighment;// GTAlignmentConstants.gtalBottomRight; ;
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", SourceFeature[i].Alighment);
                        oNewFeature.Components.GetComponent(iCNO).Geometry = textTemp;
                          
                    }
                    else if (SourceFeature[i].lblText != "line")
                    {
                        IGTOrientedPointGeometry symPoint = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        symPoint.Origin = projectPoint;
                       // SourceFeature[i].Orientation = CopiedFeatureOrientation;

                        symPoint.Orientation =  SourceFeature[i].Orientation;
                        oNewFeature.Components.GetComponent(iCNO).Geometry = symPoint;
                    } 
                    else if (SourceFeature[i].lblText == "line")
                    {
                        IGTPolylineGeometry line = GTClassFactory.Create<IGTPolylineGeometry>();
                        int m = 0;
                        while (SourceFeature[i].X[m] != -1)
                        {
                            IGTPoint Pline = GTClassFactory.Create<IGTPoint>();
                            Pline.X = SourceFeature[i].X[m];
                            Pline.Y = SourceFeature[i].Y[m];
                            Pline.Z = 0;
                            line.Points.Add(Pline);
                            m++;
                        }
                        oNewFeature.Components.GetComponent(iCNO).Geometry = line;
                    }
                }

                m_oIGTTransactionManager.Commit();
                m_oIGTTransactionManager.RefreshDatabaseChanges();
                m_gtapp.RefreshWindows();
               

                return true;
               
            }
            catch (Exception ex)
            {
                m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Copy Manhole", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
                
            }
        }
        private void CopyAttributes(Recordset FromRec, Recordset ToRec)
        {
            if (!FromRec.EOF)
            {
                for (int i = 0; i < FromRec.Fields.Count; i++)
                {
                    if ((FromRec.Fields[i].Name != "G3E_FID") && (FromRec.Fields[i].Name != "G3E_ID") && (FromRec.Fields[i].Name != "MANHOLE_ID"))
                    {
                        ToRec.Update(FromRec.Fields[i].Name, FromRec.Fields[i].Value);
                    }
                }
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
        public double AngleBtwPoint(double diffX, double diffY)
        {
            double t1 = diffY;
            double t2 = diffX;

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
        private double LengthBtwTwoPoints(double startPointX, double startPointY, double endPointX, double endPointY)
        {
            return Math.Sqrt(Math.Pow((endPointX - startPointX), 2) + Math.Pow((endPointY - startPointY), 2));
        }
        private int LengthBtwTwoPointsInt(double startPointX, double startPointY, double endPointX, double endPointY)
        {
            return Convert.ToInt32(Math.Round(Math.Sqrt(Math.Pow((endPointX - startPointX), 2) + Math.Pow((endPointY - startPointY), 2)), 0));
        }
        #endregion

        #region Calculate Coord for point base on  length 
        public IGTPoint PointBasedOnEnteredLength(double len, double Angle, double startPointX, double startPointY)
        {
            IGTPoint NewPoint = GTClassFactory.Create<IGTPoint>();
            NewPoint.X = startPointX + len * Math.Cos(Angle * Math.PI / 180);
            NewPoint.Y = startPointY + len * Math.Sin(Angle * Math.PI / 180);
            NewPoint.Z = 0.0;

            return NewPoint;
        }

        public IGTPoint PointBasedOnEnteredLength(double len, double startPointX, double startPointY, double endPointX, double endPointY)
        {
            double Angle = AngleBtwPoint(startPointX, startPointY, endPointX, endPointY);

            IGTPoint NewPoint = GTClassFactory.Create<IGTPoint>();
            NewPoint.X = startPointX + len * Math.Cos(Angle * Math.PI / 180);
            NewPoint.Y = startPointY + len * Math.Sin(Angle * Math.PI / 180);
            NewPoint.Z = 0.0;           
            return NewPoint;
        }
          #endregion
    }
}
