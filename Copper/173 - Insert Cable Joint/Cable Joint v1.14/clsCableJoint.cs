using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;

using ADODB;

/// select G3E_FIELD, g3e_username from g3e_attribute where g3e_cno = 51;

namespace NEPS.GTechnology.Cable_Joint
{
    class clsCableJoint
    {
        public static int v_G3E_DETAILID = 0;
        public static short LINE_FNO = 7000;
        public static short LINE_GEO_CNO = 7010;
        public static short LINE_DET_CNO = 7011;

        public static short JOINT_FNO = 10800;
        public static short JOINT_CNO = 10801;
        public static short JOINT_GEO_CNO = 10820;
        public static short JOINT_DET_CNO = 10821;
        public static int JOINT_FID = 0;

        public static IGTDDCKeyObject m_SelectedLine;
        public static IGTPolylineGeometry m_Joints;
        public static IGTKeyObject JOINT_Feature;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;

        public static string closureType;
        public static string jointType;
        public static string mm;

        internal static bool LineSelected(IGTSelectedObjects selectedObj)
        {
            m_SelectedLine = null;
            switch (selectedObj.FeatureCount)
            {
                case 0:
                    return false;
                case 1:     // one feature selected

                    foreach (IGTDDCKeyObject oDDC in selectedObj.GetObjects())
                        if (oDDC.FNO == LINE_FNO)
                        {
                            m_SelectedLine = oDDC;
                            return true;
                        }
                    return false;
                default: // more than one features selected
                    return false;
            }//switch
        }

        internal static void ReadJoints()
        {
            string ssql = "SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) AS XY FROM " +
            (v_G3E_DETAILID > 0 ? "DGC_CBL_L" : "GC_CBL_L") +
            " WHERE G3E_FID = " + m_SelectedLine.FID.ToString();


            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            m_Joints = GTClassFactory.Create<IGTPolylineGeometry>();
            if (!rs.EOF)
            {

                string val = myUtil.CellValue(rs.Fields[0].Value).Trim();
                if (val.Length > 0)
                {
                    string[] XYs = val.Split('|');
                    for (int i = 0; i < XYs.GetUpperBound(0); i += 2)
                    {
                        IGTPoint p = GTClassFactory.Create<IGTPoint>();
                        p.X = double.Parse(XYs[i]);
                        p.Y = double.Parse(XYs[i + 1]);
                        m_Joints.Points.Add(p);
                    }
                }
            }
            rs.Close();
            rs = null;
        }

        internal static bool AddingJoint(IGTApplication app, IGTPoint jointPoint)
        {
            try
            {
                GTCable_Joint.m_gtapp.BeginWaitCursor();

                short iCNO;
                int jointPos = 0;

                string oSpliceClass = "";
                string cableCode = "";
                IGTKeyObject o1stFeature;
                IGTKeyObject o2ndFeature;
                IGTKeyObject oJntFeature;

                IGTTextPointGeometry oTextGeom;
                IGTPolylineGeometry oPointLine1 = GTClassFactory.Create<IGTPolylineGeometry>();
                IGTPolylineGeometry oPointLine2 = GTClassFactory.Create<IGTPolylineGeometry>();
                IGTPointGeometry oJoint = GTClassFactory.Create<IGTPointGeometry>();

                ADODB.Recordset rs1 = new ADODB.Recordset();
                ADODB.Recordset rs2 = new ADODB.Recordset();

              
                GTCable_Joint.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "INSERT JOINT : breaking cable at joint position");
                #region Breaking Cable Point into two segment at the joint position
                // get the joint position index
                jointPos = JointPosition(jointPoint, m_Joints) + 1;
                // create point for 1st segment of the cable
                for (int i = 0; i < jointPos; i++)
                    oPointLine1.Points.Add(m_Joints.Points[i]);
                // add joint point to the end of 1st segment and start of 2nd segment
                oPointLine1.Points.Add(jointPoint);
                oPointLine2.Points.Add(jointPoint);
                // create point for 2nd segment of the cable
                for (int i = jointPos; i < m_Joints.Points.Count; i++)
                    oPointLine2.Points.Add(m_Joints.Points[i]);
                // create point geometry for joint;
                oJoint.Origin = jointPoint;

                #endregion

                GTCable_Joint.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "INSERT JOINT : insert new joint");
                #region JOINT - 19-JUN-2012
                GTCable_Joint.m_oIGTTransactionManager.Begin("Insert Join");

                #region Insert Joint // this will be last - then we open the feature explorer
                iCNO = (v_G3E_DETAILID > 0 ? JOINT_DET_CNO : JOINT_GEO_CNO);

                oJntFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(JOINT_FNO);
                JOINT_FID = oJntFeature.FID;

                if (oJntFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oJntFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", oJntFeature.FID);
                    oJntFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", oJntFeature.FNO);
                    oJntFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);

                    if (v_G3E_DETAILID > 0) // this is for detail window only
                        oJntFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", v_G3E_DETAILID);
                }
                else
                {
                    oJntFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oJntFeature.Components.GetComponent(iCNO).Geometry = oJoint;

                #endregion

                #region Joint_Netelem
                iCNO = 51;
                if (oJntFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oJntFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", oJntFeature.FID);
                    oJntFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", oJntFeature.FNO);
                    oJntFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                }
                else
                {
                    oJntFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }
                oJntFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "PROPOSED");
                oJntFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                System.Diagnostics.Debug.WriteLine(mm);
                oJntFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", '-');
                oJntFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_PLACED", DateTime.Now.ToString("yyyy"));

                #endregion

                //GTCable_Joint.m_oIGTTransactionManager.Commit();
                //GTCable_Joint.m_oIGTTransactionManager.RefreshDatabaseChanges();
                #endregion

                GTCable_Joint.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "INSERT JOINT : configuring the cables");

                Cursor.Current = Cursors.WaitCursor;

                #region CABLE : 19-JUN-2012
                //GTCable_Joint.m_oIGTTransactionManager.Begin("Update Cable");

                iCNO = (v_G3E_DETAILID > 0 ? LINE_DET_CNO : LINE_GEO_CNO);

                #region Update 1st Segment Of The Cable
                o1stFeature = app.DataContext.OpenFeature(m_SelectedLine.FNO, m_SelectedLine.FID);
                o1stFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                o1stFeature.Components.GetComponent(iCNO).Geometry = oPointLine1;
                #endregion

                #region Create 2nd Segment As New Cable

                o2ndFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(LINE_FNO);

                if (o2ndFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", o2ndFeature.FID);
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", o2ndFeature.FNO);
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    if (v_G3E_DETAILID > 0)
                        o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", v_G3E_DETAILID);
                }
                else
                {
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }
                o2ndFeature.Components.GetComponent(iCNO).Geometry = oPointLine2;
                
                #endregion

                #region Positioning Text Label

                iCNO = (short)(v_G3E_DETAILID > 0 ? 7031 : 7030);

                if (o2ndFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", o2ndFeature.FID);
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", o2ndFeature.FNO);
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    if (v_G3E_DETAILID > 0)
                        o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", v_G3E_DETAILID);
                }
                else
                {
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                if (!o1stFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    o1stFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    o2ndFeature.Components.GetComponent(iCNO).Geometry = o1stFeature.Components.GetComponent(iCNO).Geometry;

                    bool flag_1 = 
                        myUtil.IsInBetween(oPointLine1, o1stFeature.Components.GetComponent(iCNO).Geometry.FirstPoint);
                    bool flag_2 =
                        myUtil.IsInBetween(oPointLine2, o1stFeature.Components.GetComponent(iCNO).Geometry.FirstPoint);

                    if (!flag_2 || flag_1)
                    {
                        //IGTOrientedPointGeometry o2ndLabel = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        IGTTextPointGeometry o2ndLabel = GTClassFactory.Create<IGTTextPointGeometry>();
                        //o2ndLabel = (IGTTextPointGeometry)o2ndFeature.Components.GetComponent(iCNO).Geometry; 
                        o2ndLabel.Rotation = GetRotationAngle(oPointLine2, 0, 0.5);
                        o2ndLabel.Origin = myUtil.GetPointInBetween(oPointLine2, 0, 0.5);
                        o2ndLabel.Alignment = GTAlignmentConstants.gtalCenterCenter;
                        o2ndFeature.Components.GetComponent(iCNO).Geometry = o2ndLabel;
                    }
                    if (!flag_1 || flag_2)
                    {
                        IGTOrientedPointGeometry o1stLabel = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        o1stLabel = (IGTOrientedPointGeometry)o1stFeature.Components.GetComponent(iCNO).Geometry;
                        o1stLabel.Origin = myUtil.GetPointInBetween(oPointLine1, -1, 0.5);
                        o1stFeature.Components.GetComponent(iCNO).Geometry = o1stLabel;
                    }
                }
                else
                {
                    o1stFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", o1stFeature.FID);
                    o1stFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", o1stFeature.FNO);
                    o1stFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    if (v_G3E_DETAILID > 0)
                        o1stFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", v_G3E_DETAILID);

                    IGTOrientedPointGeometry cblLabel1 = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    cblLabel1.Origin = myUtil.GetPointInBetween(oPointLine1, -1, 0.5);
                    o1stFeature.Components.GetComponent(iCNO).Geometry = cblLabel1;

                    IGTOrientedPointGeometry cblLabel2 = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    cblLabel2.Origin = myUtil.GetPointInBetween(oPointLine2, 0, 0.5);
                    o2ndFeature.Components.GetComponent(iCNO).Geometry = cblLabel2;
                }


                #endregion

                #region GC_Netelem
                iCNO = 51; //Netelem
                // copy existing netelem from 1st segment to 2nd segment
                rs1 = o1stFeature.Components.GetComponent(iCNO).Recordset;

                if (o2ndFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", o2ndFeature.FID);
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", o2ndFeature.FNO);
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                }
                else
                {
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                OpenJointAttribute(o1stFeature.FID, oJntFeature.FID);
                System.Diagnostics.Debug.WriteLine(mm);

                string featureState = o1stFeature.Components.GetComponent(iCNO).Recordset.Fields["FEATURE_STATE"].Value.ToString();
                if (featureState == "ASB") featureState = "MOD";
                o1stFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", featureState);
                o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", featureState);
                o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "PROPOSED");
                o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", mm);
                
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "EXC_ABB");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "FEATURE_STATE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "BILLING_RATE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "IMAP_FEATURE_ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "JOB_ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "JOB_STATE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "MIC");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "OWNERSHIP");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "PLAN_ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "SAP_WRK_ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "SCHEME_NAME");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "SEGMENT");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "SERVICE_CODE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "SWITCH_CENTRE_CLLI");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "YEAR_PLACED");

                #endregion

                #region Attributes

                iCNO = 7001;
                rs1 = o1stFeature.Components.GetComponent(iCNO).Recordset;

                o1stFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                double cablelen = myUtil.CableLength(oPointLine1);
                o1stFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", cablelen);

                if (o2ndFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", o2ndFeature.FID);
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", o2ndFeature.FNO);
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                }
                else
                {
                    o2ndFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                o2ndFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", myUtil.CableLength(oPointLine2));
                
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "ALPHA_CODE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "ARMOUR");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "BLOCK_NUM");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "CABLE_CLASS");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "CABLE_CODE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "CMP_NUMBER_OF_COAX_TUBES");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "COMPOSITION");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "COPPER_SIZE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "COUNT_ANNOTATION");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "CTYPE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "CUSAGE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "DESIGN_TYPE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "DIAMETER");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "EFFECTIVE_PAIRS");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "FIBER_MODE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "FIBER_SIZE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "FIBER_TAG_ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "G3E_PAIRCOUNTPREFIX");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "GAUGE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "HI_PR");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "ITFACE_CODE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "LO_PR");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "MDF_NUM");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "NUMBER_OF_COAX_TUBES");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "NUMBER_OF_VIDEO_PAIRS");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "NUMCABLES");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "ORIGINAL_USER");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "OTHER_ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "PERCENT_AERIAL");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "PERCENT_BURIED");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "PLACEMENT");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "ROUTE_DETAIL");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "RT_CODE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "SHEATH");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "SOURCE_ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "SOURCE_TYPE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "STUB_LABEL");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "SUB_TERMCODE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "TERMINATION_ID");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "TERMINATION_TYPE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "TEXT_FORMAT");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "TEXT_VALUE");
                //                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "TOTAL_LENGTH");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "TOTAL_SIZE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "USAGE");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "VERT_BLOCK_HI");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "VERT_BLOCK_LO");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "VERT_NUM");
                myUtil.CopyFields(o2ndFeature, rs1, iCNO, "VERT_TYPE");

                cableCode = rs1.Fields["CABLE_CODE"].Value.ToString();

                switch (rs1.Fields["CABLE_CLASS"].Value.ToString())
                {
                    case "D-CABLE": 
                        oSpliceClass = "JOINT D-SIDE"; break;
                    case "STUB D-SIDE" :
                        rs1.Update("CABLE_CLASS", "D-CABLE");
                        oSpliceClass = "JOINT D-SIDE"; break;
                    case "E-CABLE":
                        oSpliceClass = "JOINT E-SIDE"; break;
                    case "STUB E-SIDE":
                        rs1.Update("CABLE_CLASS", "E-CABLE");
                        oSpliceClass = "JOINT E-SIDE"; break;

                }
                #endregion

                #region NR_Connect
                iCNO = 53;
                rs1 = o1stFeature.Components.GetComponent(iCNO).Recordset;
                rs2 = o2ndFeature.Components.GetComponent(iCNO).Recordset;

                if (rs2.EOF)
                {
                    rs2.AddNew("G3E_FID", o2ndFeature.FID);
                    rs2.Update("G3E_FNO", o2ndFeature.FNO);
                    rs2.Update("G3E_CID", 1);
                }
                else
                {
                    rs2.MoveLast();
                }
                rs2.Update("IN_FNO", oJntFeature.FNO);
                rs2.Update("IN_FID", oJntFeature.FID);
                rs2.Update("OUT_FNO", short.Parse(rs1.Fields["OUT_FNO"].Value.ToString()));
                rs2.Update("OUT_FID", int.Parse(rs1.Fields["OUT_FID"].Value.ToString()));


                rs1.MoveLast();
                rs1.Update("OUT_FNO", oJntFeature.FNO);
                rs1.Update("OUT_FID", oJntFeature.FID);

                #endregion

                //GTCable_Joint.m_oIGTTransactionManager.Commit();
                //GTCable_Joint.m_oIGTTransactionManager.RefreshDatabaseChanges();
                #endregion

                //GTCable_Joint.m_oIGTTransactionManager.Begin(oSpliceClass);

                GTCable_Joint.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "INSERT JOINT : configuring joint attribute");
                #region Joint Attributes
                iCNO = JOINT_CNO;

                oJntFeature = app.DataContext.OpenFeature(oJntFeature.FNO, oJntFeature.FID);
                if (oJntFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oJntFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", oJntFeature.FID);
                    oJntFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", oJntFeature.FNO);
                    oJntFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                }
                else
                {
                    oJntFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oJntFeature.Components.GetComponent(iCNO).Recordset.Update("SPLICE_CLASS", oSpliceClass);
                oJntFeature.Components.GetComponent(iCNO).Recordset.Update("JOINT_TYPE", jointType);
                oJntFeature.Components.GetComponent(iCNO).Recordset.Update("CLOSURE_TYPE", closureType);
                oJntFeature.Components.GetComponent(iCNO).Recordset.Update("DIST_FROM_EXC", cablelen);
                oJntFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CODE", cableCode);

                #endregion

                GTCable_Joint.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "INSERT JOINT : copy joint attribute");
                #region Copy Joint Elemenet
                CopyJointElement(oJntFeature, o1stFeature.Components.GetComponent(53).Recordset);
                //clsOwnership.ParentOwnershipID(oJntFeature.FID, oJntFeature.FNO);
                #endregion

                JOINT_Feature = oJntFeature;

                GTCable_Joint.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "INSERT JOINT : saving data to database");
                GTCable_Joint.m_oIGTTransactionManager.Commit();
                GTCable_Joint.m_oIGTTransactionManager.RefreshDatabaseChanges();

              //  OpenJointAttribute(o1stFeature.FID, oJntFeature.FID);

                UpdateCableCode(o1stFeature.FID, cableCode);
                UpdateCableCode(o2ndFeature.FID, cableCode);
                Cursor.Current = Cursors.Default;
                return true;
            }
            catch (Exception ex)
            {
                GTCable_Joint.m_oIGTTransactionManager.Rollback();
                MessageBox.Show("Error placement\r\n" + ex.Message);
                return false;
            }
            finally
            {
                m_SelectedLine = null;
                GTCable_Joint.m_gtapp.EndWaitCursor();
            }
        }

        #region Local Methods

        private static int JointPosition(IGTPoint p, IGTPolylineGeometry pnt)
        {
            try
            {
                Debug.WriteLine("X : " + p.X.ToString() + ", Y : " + p.Y.ToString());

                p.X = Math.Round(p.X, 3, MidpointRounding.AwayFromZero);
                p.Y = Math.Round(p.Y, 3, MidpointRounding.AwayFromZero);
                Debug.WriteLine("X : " + p.X.ToString() + ", Y : " + p.Y.ToString());

                for (int i = 0; i < pnt.Points.Count - 1; i++)
                {

                    if (p.X == pnt.Points[i].X && p.Y == pnt.Points[i].Y)
                    {
                        Debug.WriteLine(" TRUE : " + i.ToString());
                        return i;
                    }
                    else if (InBetween(p.X, pnt.Points[i].X, pnt.Points[i + 1].X) &&
                        InBetween(p.Y, pnt.Points[i].Y, pnt.Points[i + 1].Y))
                    {
                        Debug.WriteLine(" TRUE : " + i.ToString());
                        return i;
                    }
                }
                Debug.WriteLine(" FALSE : -1 ");
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        private static bool InBetween(double P, double P1, double P2)
        {
            Debug.WriteLine("InBetween : " + P1.ToString() + " - " + P.ToString() + " - " + P2.ToString());
            if (P >= P1 && P <= P2)
                return true;
            else if (P >= P2 && P <= P1)
                return true;
            else
                return false;
        }

        #endregion

        #region Update 13-07-2012


        private static void CopyJointElement(IGTKeyObject newJoint, ADODB.Recordset rsCable)
        {
            try
            {
                rsCable.MoveLast();
                int iFID = int.Parse(rsCable.Fields["IN_FID"].Value.ToString());
                short iFNO = short.Parse(rsCable.Fields["IN_FNO"].Value.ToString());

                IGTKeyObject oldJoint = GTCable_Joint.m_gtapp.DataContext.OpenFeature(iFNO, iFID);

                // copy min material from existing joint
                short iCNO = 51; // netelem
                oldJoint.Components.GetComponent(iCNO).Recordset.MoveLast();
               // myUtil.CopyFields(newJoint, oldJoint.Components.GetComponent(iCNO).Recordset, iCNO, "MIN_MATERIAL");
                newJoint.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", mm);
                // copy attribute from oldjoint
                iCNO = JOINT_CNO;
                ADODB.Recordset rs = oldJoint.Components.GetComponent(iCNO).Recordset;
                rs.MoveLast();

                // myUtil.CopyFields(newJoint, rs, iCNO, "CABLE_CODE");
                myUtil.CopyFields(newJoint, rs, iCNO, "CASE_TYPE");
                // myUtil.CopyFields(newJoint, rs, iCNO, "CLOSURE_TYPE");
                // myUtil.CopyFields(newJoint, rs, iCNO, "DBLOSS");
                // myUtil.CopyFields(newJoint, rs, iCNO, "DCR");
                myUtil.CopyFields(newJoint, rs, iCNO, "ID");
                myUtil.CopyFields(newJoint, rs, iCNO, "ITFACE_CODE");
                myUtil.CopyFields(newJoint, rs, iCNO, "MODEL");
                myUtil.CopyFields(newJoint, rs, iCNO, "ORIGINAL_USER");
                myUtil.CopyFields(newJoint, rs, iCNO, "RT_CODE");
                myUtil.CopyFields(newJoint, rs, iCNO, "SPLICE_ADMIN_TYPE");
                myUtil.CopyFields(newJoint, rs, iCNO, "SPLICE_CONNECTION_TYPE");
                myUtil.CopyFields(newJoint, rs, iCNO, "SPLICE_LOSS");
                myUtil.CopyFields(newJoint, rs, iCNO, "SPLICE_NOTE");
                myUtil.CopyFields(newJoint, rs, iCNO, "SPLICE_PHYSICAL_TYPE");

                newJoint.Components.GetComponent(iCNO).Recordset.Update("DIST_FROM_EXC",
                    short.Parse(myUtil.rsField(newJoint.Components.GetComponent(iCNO).Recordset, "DIST_FROM_EXC")) +
                    short.Parse(myUtil.rsField(rs, "DIST_FROM_EXC")));

            }
            catch
            { }
        }

        #endregion

        #region Update 10-10-2012

        internal static void OpenJointAttribute(int cFID, int jFID)
        {
            try
            {
                // get joint FID from gc_splice
                string[] cableAttr = GetCableAttribute(cFID);
                if (cableAttr != null)
                {
                    frmJointAttribute f = new frmJointAttribute(jFID, cableAttr);
                    f.ShowDialog();
                    closureType = f.closure_type;
                    jointType = f.joint_type;
                    mm = f.minmaterial;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        internal static void UpdateCableCode(int cFID, string cableCode)
        {
            try
            {
                string ssql = "UPDATE GC_CBL SET CABLE_CODE = '" + cableCode + "' WHERE G3E_FID = " + cFID.ToString();

                ADODB.Recordset rsSQL = new ADODB.Recordset();
                int iR;
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            }
            catch
            { }
        }

        #region myMethod

        internal static string[] GetCableAttribute(int cableFID)
        {
            string ssql = "SELECT * FROM GC_CBL WHERE G3E_FID = " + cableFID.ToString();
            ADODB.Recordset rsSQL = new ADODB.Recordset();

            rsSQL = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);

            if (rsSQL.RecordCount > 0)
            {
                rsSQL.MoveFirst();
                string[] attr = new string[] {
                    rsSQL.Fields["G3E_FID"].Value.ToString(),
                    rsSQL.Fields["ITFACE_CODE"].Value.ToString(),
                    rsSQL.Fields["RT_CODE"].Value.ToString(),
                    rsSQL.Fields["CABLE_CODE"].Value.ToString(),
                    rsSQL.Fields["CTYPE"].Value.ToString(),
                    rsSQL.Fields["TOTAL_SIZE"].Value.ToString(),
                    rsSQL.Fields["GAUGE"].Value.ToString()      
                };
                return attr;
            }
            else
            {
                return null;
            }
        }


        #endregion


        #endregion


        #region Label Management
        internal static double GetRotationAngle(IGTPolylineGeometry geoPoint, int startPoint, double percentage)
        {
            IGTPoint newPoint = GTClassFactory.Create<IGTPoint>();
            if (startPoint == -1) startPoint = geoPoint.Points.Count - 2;

            double x1 = geoPoint.Points[startPoint].X;
            double y1 = geoPoint.Points[startPoint].Y;

            double x2 = geoPoint.Points[startPoint + 1].X;
            double y2 = geoPoint.Points[startPoint + 1].Y;

            double angle = PGeoLib.GetAngle(x1, y1, x2, y2);
            return angle;
        }

        #endregion
    }
}
