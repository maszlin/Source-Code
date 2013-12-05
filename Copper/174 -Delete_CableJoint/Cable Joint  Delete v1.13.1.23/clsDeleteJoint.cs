using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;

using ADODB;

namespace NEPS.GTechnology.Delete_Joint
{
    class clsDeleteJoint
    {
        public short LINE_FNO = 7000;
        public short LINE_GEO_CNO = 7010;
        public short LINE_DET_CNO = 7011;

        public short JOINT_FNO = 10800;
        public short JOINT_CNO = 10801;
        public short JOINT_GEO_CNO = 10820;
        public short JOINT_DET_CNO = 10821;
        public int JOINT_FID = 0;
        private int JOINT_InFID = 0;
        private int JOINT_OutFID = 0;


        public IGTDDCKeyObject m_SelectedObj;
        public IGTPolylineGeometry m_Joints;
        public Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;

        public int v_G3E_DETAILID = 0;

        public bool JointSelected(IGTSelectedObjects selectedObj)
        {
            m_SelectedObj = null;
            switch (selectedObj.FeatureCount)
            {
                case 0:
                    return false;
                case 1:     // one feature selected

                    foreach (IGTDDCKeyObject oDDC in selectedObj.GetObjects())
                        if (oDDC.FNO == JOINT_FNO)
                        {
                            m_SelectedObj = oDDC;
                            return true;
                        }
                    return false;
                default: // more than one features selected
                    return false;
            }//switch
        }

        public bool RemovingJoint()
        {
            IGTKeyObject InFeature;
            IGTKeyObject OutFeature;

            short iCNO;
            if (GetConnectedCable()) // make sure only two cable connected to the joint
            {

                if (!CheckFeatureState(LINE_FNO, JOINT_InFID) || !CheckFeatureState(LINE_FNO, JOINT_OutFID))
                    return false;

                if (CompareInOutCable()) // make sure in and out cables are of the same type
                {
                    CombineJoints(); // combine joints of both cable

                    GTDelete_Joint.m_oIGTTransactionManager.Begin("Delete Join");

                    #region Update NR-Connect
                    iCNO = 53;

                    InFeature = GTDelete_Joint.m_gtapp.DataContext.OpenFeature(LINE_FNO, JOINT_InFID);
                    OutFeature = GTDelete_Joint.m_gtapp.DataContext.OpenFeature(LINE_FNO, JOINT_OutFID);

                    Update_NRConnect
                        (InFeature.Components.GetComponent(iCNO).Recordset,
                        OutFeature.Components.GetComponent(iCNO).Recordset);

                    #endregion

                    #region Update In Cable - combine geometry with OutCable
                    iCNO = (v_G3E_DETAILID > 0 ? LINE_DET_CNO : LINE_GEO_CNO);
                    InFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    InFeature.Components.GetComponent(iCNO).Geometry = m_Joints;
                    #endregion

                    #region Attributes
                    iCNO = 7001;
                    InFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    InFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", myUtil.CableLength(m_Joints));
                    #endregion

                    #region Update FeatureState MOD => ASB

                    iCNO = 51; //Netelem
                    InFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    string featureState = InFeature.Components.GetComponent(iCNO).Recordset.Fields["FEATURE_STATE"].Value.ToString();
                    if (featureState == "MOD")
                        InFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "ASB");

                    #endregion

                    #region Delete Out Label
                    iCNO = (short)(v_G3E_DETAILID > 0 ? 7031 : 7030);
                    if (!OutFeature.Components.GetComponent(iCNO).Recordset.EOF)
                        OutFeature.Components.GetComponent(iCNO).Recordset.Delete(AffectEnum.adAffectCurrent);
                    #endregion

                    #region Delete Out Cable
                    //DeleteFeature(LINE_FNO, JOINT_OutFID);
                    DeleteCable(JOINT_OutFID);
                    #endregion

                    #region Delete Joint
                    DeleteFeature(m_SelectedObj.FNO, m_SelectedObj.FID);
                    #endregion

                    GTDelete_Joint.m_oIGTTransactionManager.Commit();
                    GTDelete_Joint.m_oIGTTransactionManager.RefreshDatabaseChanges();

                    return true;
                }
            }
            return false;
        }

        #region  Local Method
        private bool GetConnectedCable()
        {
            try
            {
                JOINT_FID = m_SelectedObj.FID;
                JOINT_InFID = -1;
                JOINT_OutFID = -1;

                string ssql = "SELECT * FROM GC_NR_CONNECT ";
                ssql += "WHERE IN_FID = " + m_SelectedObj.FID.ToString() + " ";
                ssql += "OR OUT_FID = " + m_SelectedObj.FID.ToString();

                ADODB.Recordset rs = new ADODB.Recordset();

                rs = GTDelete_Joint.m_gtapp.DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.EOF)
                {
                    MessageBox.Show("No cable connected to this joint");
                    return true;
                }
                else if (rs.RecordCount > 2)
                {
                    MessageBox.Show("Can not delete this joint\r\nMore than two cables connected to the joint");
                    return false;
                }
                else
                {
                    while (!rs.EOF)
                    {
                        if (myUtil.ParseInt(rs.Fields["IN_FID"].Value.ToString()) == m_SelectedObj.FID)
                        {
                            JOINT_OutFID = myUtil.ParseInt(rs.Fields["G3E_FID"].Value.ToString());
                        }
                        else
                        {
                            JOINT_InFID = myUtil.ParseInt(rs.Fields["G3E_FID"].Value.ToString());
                        }
                        rs.MoveNext();
                    }
                }
                rs.Close();
                rs = null;
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fail to read connected cable from this joint\r\n" + ex.Message);
                return false;
            }
        }

        private bool CheckFeatureState(short iFNO, int iFID)
        {
            short iCNO = (short)51; //Netelem            
            IGTKeyObject f = GTDelete_Joint.m_gtapp.DataContext.OpenFeature(iFNO, iFID);
            string fstate = f.Components.GetComponent(iCNO).Recordset.Fields["FEATURE_STATE"].Value.ToString();
            if (fstate == "PPF" || fstate == "PAD")
                return true;
            else
            {
                MessageBox.Show("One or both of cable feature state not PPF nor PAD");
                return false;
            }
        }

        private bool CompareInOutCable()
        {
            try
            {
                string[] keys = { "CABLE_CLASS", "COPPER_SIZE", "CABLE_CODE", 
                    "EFFECTIVE_PAIRS", "CTYPE", "CUSAGE", "GAUGE", "USAGE" };

                ADODB.Recordset rsIn = new ADODB.Recordset();
                ADODB.Recordset rsOut = new ADODB.Recordset();

                //"SELECT * FROM " + (v_G3E_DETAILID > 0 ? "DGC_CBL" : "GC_CBL") +

                string ssql = "SELECT * FROM GC_CBL WHERE G3E_FID = ";

                rsIn = GTDelete_Joint.m_gtapp.DataContext.OpenRecordset(ssql + JOINT_InFID,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                rsOut = GTDelete_Joint.m_gtapp.DataContext.OpenRecordset(ssql + JOINT_OutFID,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rsIn.EOF)
                    MessageBox.Show("Records for in cable not found");
                else if (rsOut.EOF)
                    MessageBox.Show("Records for out cable not found");
                else
                {
                    for (int i = 0; i < keys.GetUpperBound(0); i++)
                    {
                        if (rsIn.Fields[keys[i]].Value.ToString().Trim() != rsOut.Fields[keys[i]].Value.ToString().Trim())
                        {
                            Debug.WriteLine("IN : " + rsIn.Fields[keys[i]].Value.ToString());
                            Debug.WriteLine("OUT : " + rsOut.Fields[keys[i]].Value.ToString());
                            MessageBox.Show(keys[i] + " between the two connected cable not equal");
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fail to compare cables\r\n" + ex.Message);
                return false;
            }
        }

        private void CombineJoints()
        {
            // combine joint point of In Cable and Out Cable in to globa variable m_Joints
            m_Joints = GTClassFactory.Create<IGTPolylineGeometry>();
            m_Joints.Points.Clear();

            m_Joints = ReadJoints(JOINT_InFID, 0);
            IGTPolylineGeometry p = GTClassFactory.Create<IGTPolylineGeometry>();
            p = ReadJoints(JOINT_OutFID, 0);
            int i1 = m_Joints.Points.Count - 1;
            int i2 = p.Points.Count - 1;
            if (p.Points[0].Equals(m_Joints.Points[i1]))
                for (int i = 1; i < p.Points.Count; i++)
                    m_Joints.Points.Add(p.Points[i]);
            else if (p.Points[i2].Equals(m_Joints.Points[0]))
                for (int i = i2 - 1; i >= 0; i--)
                    m_Joints.Points.Insert(0, p.Points[i]);
            else if (p.Points[i2].Equals(m_Joints.Points[i1]))
                for (int i = i2 - 1; i >= 0; i--)
                    m_Joints.Points.Add(p.Points[i]);
            else
                for (int i = 1; i < p.Points.Count; i++)
                    m_Joints.Points.Add(p.Points[i]);
        }

        private IGTPolylineGeometry ReadJoints(int iFID, int start_index)
        {
            Debug.WriteLine("Read Joints : " + iFID);

            IGTPolylineGeometry pXY = GTClassFactory.Create<IGTPolylineGeometry>();
            string ssql = "SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) AS XY FROM " +
                        (v_G3E_DETAILID > 0 ? "DGC_CBL_L" : "GC_CBL_L") +
                        " WHERE G3E_FID = " + iFID;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                string val = myUtil.CellValue(rs.Fields[0].Value).Trim();
                if (val.Length > 0)
                {
                    string[] XYs = val.Split('|');
                    for (int i = start_index; i < XYs.GetUpperBound(0); i += 2)
                    {
                        IGTPoint p = GTClassFactory.Create<IGTPoint>();
                        p.X = double.Parse(XYs[i]);
                        p.Y = double.Parse(XYs[i + 1]);
                        pXY.Points.Add(p);
                        Debug.WriteLine("X : " + p.X + ", Y : " + p.Y);
                    }
                }
            }
            rs.Close();
            rs = null;
            return pXY;
        }

        private void DeleteCable(int iFID)
        {
            GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
            try
            {
                //GTDelete_Joint.m_oIGTTransactionManager.Begin("Delete Transfer Cable");
                IGTKeyObject cblFeature = GTDelete_Joint.m_gtapp.DataContext.OpenFeature(LINE_FNO, iFID);

                DeleteFeatureCNO(cblFeature, 51); // GC_NETELEM
                DeleteFeatureCNO(cblFeature, LINE_GEO_CNO); // GC_CBL_L
                DeleteFeatureCNO(cblFeature, 7030); // GC_CBL_T : Text Label
                DeleteFeatureCNO(cblFeature, 7001); // GC_CBL : Attribute
                DeleteFeatureCNO(cblFeature, 53); // NR_CONNECT

                //GTDelete_Joint.m_oIGTTransactionManager.Commit();
                //GTDelete_Joint.m_oIGTTransactionManager.RefreshDatabaseChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Deleting Cable\r\n" + ex.Message);
            }
            finally
            {
                GTClassFactory.Create<IGTApplication>().EndWaitCursor();
            }
        }

        private void DeleteFeatureCNO(IGTKeyObject feature, short cno)
        {
            try
            {
                if (!feature.Components.GetComponent(cno).Recordset.EOF && !feature.Components.GetComponent(cno).Recordset.BOF)
                {
                    feature.Components.GetComponent(cno).Recordset.MoveLast();
                    feature.Components.GetComponent(cno).Recordset.Delete(AffectEnum.adAffectCurrent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("delete feature - fid:" + feature.FID + " fno:" + feature.FNO + " cno:" + cno + " >>> " + ex.Message);
            }
        }

        private bool DeleteFeature(short iFNO, int iFID)
        {
            int iR; // out variable from adodb.sql 
            Debug.WriteLine("Delete Features " + iFNO.ToString() + "[" + iFID.ToString() + "]");
            try
            {
                string ssql = "SELECT g3e_table FROM g3e_component WHERE g3e_cno IN " +
                    "(SELECT g3e_cno FROM g3e_featurecomponent WHERE g3e_fno = " + iFNO.ToString() + ")";
                string dsql;

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                    (ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                while (!rs.EOF)
                {
                    try
                    {
                        string val = myUtil.CellValue(rs.Fields[0].Value).Trim();
                        if (val.Length > 0)
                        {
                            dsql = "DELETE FROM " + val + " WHERE G3E_FID = " + iFID.ToString();
                            Debug.Write(dsql + " >>> ");

                            GTClassFactory.Create<IGTApplication>().DataContext.Execute
                                (dsql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                            Debug.WriteLine(" OK");
                            Application.DoEvents();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    rs.MoveNext();
                }
                rs.Close();
                rs = null;

                GTClassFactory.Create<IGTApplication>().DataContext.Execute
                    ("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                GTClassFactory.Create<IGTApplication>().DataContext.Execute
                    ("ROLLBACK", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                return false;
            }
        }

        private void Update_NRConnect(ADODB.Recordset rsIn, ADODB.Recordset rsOut)
        {
            rsIn.MoveLast();
            rsOut.MoveLast();

            rsIn.Update("OUT_FNO", short.Parse(rsOut.Fields["OUT_FNO"].Value.ToString()));
            rsIn.Update("OUT_FID", int.Parse(rsOut.Fields["OUT_FID"].Value.ToString()));
        }
        #endregion

        #region Utilities Method
        #endregion
    }
}
