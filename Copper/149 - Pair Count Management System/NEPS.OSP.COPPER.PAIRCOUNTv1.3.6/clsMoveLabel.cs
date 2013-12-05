using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.PAIRCOUNT
{
    class clsMoveLabel
    {
        //first define temporary geometry service---
        public IGTGeometryEditService mobjEditServiceTemp;
        private int cblFID = 0;
        private short cblCNO = 0;
        private string cblCount = "";
        private bool flagMove = false;
        private int mi_styleID = 0;
        private GTAlignmentConstants m_textAlignment = GTAlignmentConstants.gtalCenterCenter;
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
                    mi_styleID = int.Parse(rs.Fields[0].Value.ToString());
                else
                    mi_styleID = 7032025;

                cblCNO = (short)(mobjEditServiceTemp.TargetMapWindow.DetailID > 0 ? 7033 : 7032);
            }
            else
            {
                mi_styleID = 7034011;
                cblCNO = (short)(mobjEditServiceTemp.TargetMapWindow.DetailID > 0 ? 7035 : 7034);
            }
            m_textAlignment = TextAlignment();
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
            double dRotation = 0;
            objPoint = GTClassFactory.Create<IGTPoint>();
            objPoint.X = WorldPoint.X;
            objPoint.Y = WorldPoint.Y;
            oOrPointGeom = GTClassFactory.Create<IGTTextPointGeometry>();
            oOrPointGeom.Origin = objPoint;
            oOrPointGeom.Text = cblCount;
            oOrPointGeom.Rotation = 0;//angle in radian
            oOrPointGeom.Alignment = m_textAlignment; // GTAlignmentConstants.gtalBottomLeft;
            //---------------------
            //--add newly created geometry to temporary geom service, second parameter SNO from g3e_style table for label that moving	
            mobjEditServiceTemp.AddGeometry(oOrPointGeom, mi_styleID); //SNO 7032011 for Cable Geo Count Strike Through BL Label MOD
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
                GTPairCount.m_oIGTTransactionManager.Begin("Count");
                GTPairCount.m_gtapp.BeginWaitCursor();

                IGTKeyObject oFeature = null;
                oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, cblFID);
                oFeature.Components.GetComponent(cblCNO).Geometry = mobjEditServiceTemp.GetGeometry(1);
                GTPairCount.m_oIGTTransactionManager.Commit();

            }
            catch (Exception ex)
            {
                GTPairCount.m_oIGTTransactionManager.Rollback();
                GTPairCount.m_CustomForm.TopMost = false;
                //   MessageBox.Show("Fail saving record to NEPS\r\n" + ex.Message);
            }
            finally
            {
                GTPairCount.m_oIGTTransactionManager.RefreshDatabaseChanges();
                GTPairCount.m_gtapp.EndWaitCursor();
                GTPairCount.m_CustomForm.TopMost = true;
            }

        }

        #region Get Alignment - 2013-03-27 @ m.zam
        private GTAlignmentConstants TextAlignment()
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            IGTKeyObject oFeature = null;
            oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, cblFID);
            rs = oFeature.Components.GetComponent(cblCNO).Recordset;

            //// notes : gc_cblcnt_tl_t = 7034 : dgc_cblcnt_tl_t = 7035 : gc_cblcnt_bl_t = 7032 : dgc_cblcnt_bl_t = 7033
            //string ssql = "SELECT G3E_ALIGNMENT FROM " + TableName(CNO) + " WHERE G3E_FID = " + FID;
            //rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(string.Format(ssql, classType),
            //    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                rs.MoveLast();
                switch (myUtil.ParseInt(rs.Fields["G3E_ALIGNMENT"].Value.ToString()))
                {
                    case 0: return GTAlignmentConstants.gtalCenterCenter;
                    case 1: return GTAlignmentConstants.gtalCenterLeft;
                    case 2: return GTAlignmentConstants.gtalCenterRight;
                    case 4: return GTAlignmentConstants.gtalTopCenter;
                    case 5: return GTAlignmentConstants.gtalTopLeft;
                    case 6: return GTAlignmentConstants.gtalTopRight;
                    case 8: return GTAlignmentConstants.gtalBottomCenter;
                    case 9: return GTAlignmentConstants.gtalBottomLeft;
                    case 10: return GTAlignmentConstants.gtalBottomRight;
                    default: return GTAlignmentConstants.gtalCenterCenter;
                }
            }
            else
                return GTAlignmentConstants.gtalCenterCenter;
        }



        #endregion
    }
}
