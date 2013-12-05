using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.Cable_Joint
{
    class clsMoveLabel
    {
        //first define temporary geometry service---
        public IGTGeometryEditService mobjEditServiceTemp;
        private int cblFID = 0;
        private string cblCount = "";
        private bool flagMove = false;
        private int styleID = 0;
        private string classType;

        public clsMoveLabel(int FID, string label, string classtype)
        {
            cblFID = FID;
            cblCount = label;
            classType = classtype;
            flagMove = true;
        }
        public int CableFID
        {
            get { return cblFID; }
            set { cblFID = value; }
        }
        public string CableCount
        {
            get { return cblCount; }
            set { cblCount = value; }
        }
        public bool FlagMove
        {
            get { return flagMove; }
            set { flagMove = value; }
        }
        public void InitTempLabel(IGTMapWindow ActiveMapWindow)
        {
            mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceTemp.TargetMapWindow = ActiveMapWindow;

            if (cblCount.IndexOf("T") > -1)
            {
                string ssql = "SELECT G3E_SNO FROM G3E_STYLERULE WHERE G3E_SRNO = 703201 AND G3E_FILTER LIKE '%PPF%{0}%'";
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(string.Format(ssql, classType),
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.RecordCount > 0)
                {
                    rs.MoveFirst();
                    styleID = int.Parse(rs.Fields[0].Value.ToString());
                }
                //styleID = 7032025;
            }
            else
                styleID = 7034011;
        }

        public void MoveLabel(IGTPoint WorldPoint, IGTMapWindow ActiveMapWindow)
        {
            if (mobjEditServiceTemp == null) InitTempLabel(ActiveMapWindow);

            if (mobjEditServiceTemp.GeometryCount > 0)
                mobjEditServiceTemp.RemoveAllGeometries();//delete all previous temp geom if any
            mobjEditServiceTemp.TargetMapWindow = ActiveMapWindow;

            //-------create new IGTTextPointGeometry , set all paramters include content, alighment and rotation
            //--------------------------
            IGTPoint objPoint;
            IGTTextPointGeometry oOrPointGeom = null;
            objPoint = GTClassFactory.Create<IGTPoint>();
            objPoint.X = WorldPoint.X;
            objPoint.Y = WorldPoint.Y;
            oOrPointGeom = GTClassFactory.Create<IGTTextPointGeometry>();
            oOrPointGeom.Origin = objPoint;
            oOrPointGeom.Text = cblCount;
            oOrPointGeom.Rotation = 0;//angle in radian
            oOrPointGeom.Alignment = GTAlignmentConstants.gtalBottomLeft;
            //oOrPointGeom.Alignment = GTAlignmentConstants.gtalTopLeft;
            //---------------------
            //--add newly created geometry to temporary geom service, second parameter SNO from g3e_style table for label that moving	
            mobjEditServiceTemp.AddGeometry(oOrPointGeom, styleID); //SNO 7032011 for Cable Geo Count Strike Through BL Label MOD
            //--------
            return;
        }

        public void CommitMove()
        {
            SaveLabel();

            if (mobjEditServiceTemp.GeometryCount > 0)
                mobjEditServiceTemp.RemoveAllGeometries();//delete all previous temp geom if any
            mobjEditServiceTemp = null;

            flagMove = false;
        }

        private void SaveLabel()
        {
            try
            {
                GTCable_Joint.m_oIGTTransactionManager.Begin("Count");
                GTCable_Joint.m_gtapp.BeginWaitCursor();

                IGTKeyObject oFeature = null;
                oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, cblFID);

                // notes : gc_cblcnt_tl_t = 7034 : dgc_cblcnt_tl_t = 7035 : gc_cblcnt_bl_t = 7032 : dgc_cblcnt_bl_t = 7033
                short CNO;
                if (cblCount.IndexOf("T") > -1)
                    CNO = (short)(mobjEditServiceTemp.TargetMapWindow.DetailID > 0 ? 7033 : 7032);
                else
                    CNO = (short)(mobjEditServiceTemp.TargetMapWindow.DetailID > 0 ? 7035 : 7034);

                oFeature.Components.GetComponent(CNO).Geometry = mobjEditServiceTemp.GetGeometry(1);
                GTCable_Joint.m_oIGTTransactionManager.Commit();

            }
            catch (Exception ex)
            {
                GTCable_Joint.m_oIGTTransactionManager.Rollback();
                //   MessageBox.Show("Fail saving record to NEPS\r\n" + ex.Message);
            }
            finally
            {
                GTCable_Joint.m_oIGTTransactionManager.RefreshDatabaseChanges();
                GTCable_Joint.m_gtapp.EndWaitCursor();
            }


        }
    }
}
