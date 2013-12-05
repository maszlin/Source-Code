using ADODB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using AG.GTechnology.Utilities;

namespace AG.GTechnology.OffsetPointSPT_Cbl
{
    class GTOffsetPointSPT_Cbl : Intergraph.GTechnology.Interfaces.IGTPlacementTechnique
    {
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        private IGTDataContext oDataContext = application.DataContext;

        private IGTGeometryEditService mobjEditService = null;
        private Intergraph.GTechnology.API.IGTPlacementTechniqueHelper m_PTHelper = null;
        private Intergraph.GTechnology.API.IGTGraphicComponents m_GComps = null;
        private Intergraph.GTechnology.API.IGTGraphicComponent m_GComp = null;
        private Intergraph.GTechnology.API.IGTKeyObject m_KeyObject = null;
        private Intergraph.GTechnology.API.IGTKeyObjects m_KeyObjectCollection = null;
        private bool m_bSilent = false;

        private IGTPoint m_pRotateAnchor = null;
        private double m_dAngle = 0.0;
        private double m_dAngleDeg = 0.0;
        private bool m_bRotateMode = false;

        int mintState = 0;

        private List<short> iOwnerFNO = new List<short>();
        private int iOwnerFID = 0;
        private short mintContainRelationshipNumber = 3;
        List<short> lstRelativeCNO = new List<short>();
        List<short> lstRelativeCID = new List<short>();
        string strOwnerName = string.Empty;
        private IGTPoint OwnerPoint = null;
        private IGTPolylineGeometry CableGeom = null;
        private IGTTextPointGeometry LabelCode = null;
        
        private double dOffsetX = 0;
        private double dOffsetY = 0;
        private int iAngle = 0;
        private int iLocation = 1;
        private int iOffsetFlag = 1;

        private frmODF m_ODFForm;

        #region IGTPlacementTechnique Members

        public void AbortPlacement()
        {
            mobjEditService = null;
            this.m_PTHelper.AbortPlacement();
        }

        public Intergraph.GTechnology.API.IGTGraphicComponent GraphicComponent
        {
            get
            {
                return this.m_GComp;
            }
            set
            {
                this.m_GComp = value;
            }
        }

        public Intergraph.GTechnology.API.IGTGraphicComponents GraphicComponents
        {
            get
            {
                return this.m_GComps;
            }
            set
            {
                this.m_GComps = value;
            }
        }

        private string GetExchange()
        {
            ADODB.Recordset rsExcAbb = null;
            string sSql = null;
            int recordsAffected = 0;

            try
            {
                sSql = "SELECT EXC_ABB FROM G3E_JOB WHERE G3E_IDENTIFIER = '" + application.DataContext.ActiveJob + "'";
                rsExcAbb = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                if (!rsExcAbb.EOF)
                {
                    return rsExcAbb.Fields["EXC_ABB"].Value.ToString();
                }
                rsExcAbb = null;
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private IGTGeometry CreatePointGeometry(IGTPoint UserPoint, double dAngle)
        {
            double a = (3.1415926535897931 * dAngle) / 180.0;
            double num3 = 1.5707963267948966;
            double num2 = 3.1415926535897931;
            double num5 = Math.Sin(a);
            double num4 = Math.Cos(a);

            IGTOrientedPointGeometry geometry = GTClassFactory.Create<IGTOrientedPointGeometry>();
            IGTVector vector = GTClassFactory.Create<IGTVector>();
            vector.I = Math.Cos(dAngle);
            vector.J = Math.Sin(dAngle);

            geometry.Origin = UserPoint;
            geometry.Orientation = vector;

            return geometry;
        }

        public void KeyUp(Intergraph.GTechnology.API.IGTMapWindow MapWindow, int KeyCode, int ShiftState, Intergraph.GTechnology.API.IGTPlacementTechniqueEventMode EventMode)
        {
            if (!m_bSilent)
            {
                // TODO::Process event
            }
        }

        public void MouseClick(Intergraph.GTechnology.API.IGTMapWindow MapWindow, Intergraph.GTechnology.API.IGTPoint UserPoint, int Button, int ShiftState, Intergraph.GTechnology.API.IGTDDCKeyObjects LocatedObjects, Intergraph.GTechnology.API.IGTPlacementTechniqueEventMode EventMode)
        {
            EventMode.RelationshipProcessingEnabled = false;

           // this.m_PTHelper.MouseClick(UserPoint, Button, ShiftState);
            if (mintState == 1)
            {
                this.m_PTHelper.SetGeometry(this.LabelCode);
                this.m_PTHelper.EndPlacement();

            }
        }

        public void MouseDblClick(Intergraph.GTechnology.API.IGTMapWindow MapWindow, Intergraph.GTechnology.API.IGTPoint UserPoint, int ShiftState, Intergraph.GTechnology.API.IGTDDCKeyObjects LocatedObjects)
        {
           // m_PTHelper.MouseDblClick(UserPoint, ShiftState);
        }

        public void MouseMove(Intergraph.GTechnology.API.IGTMapWindow MapWindow, Intergraph.GTechnology.API.IGTPoint UserPoint, int ShiftState, Intergraph.GTechnology.API.IGTDDCKeyObjects LocatedObjects, Intergraph.GTechnology.API.IGTPlacementTechniqueEventMode EventMode)
        {
            //this.m_PTHelper.MouseMove(UserPoint, ShiftState);
            if (mintState == 1)
            {
                
                    EventMode.RelationshipProcessingEnabled = false;
                    

                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to choose new position of Cable Code Label");
                                IGTTextPointGeometry temp = GTClassFactory.Create<IGTTextPointGeometry>();

                                IGTPoint projectPoint = PointOnConduit(UserPoint.X, UserPoint.Y, this.CableGeom);

                                if (projectPoint.X == 0 && projectPoint.Y == 0)
                                {
                                    return;
                                }
                                else
                                {
                                    int length = int.Parse(projectPoint.Z.ToString());
                                    IGTPoint endPoint = PointOnConduit(this.CableGeom.LastPoint.X, this.CableGeom.LastPoint.Y, this.CableGeom);
                                    if (length > int.Parse(endPoint.Z.ToString()))
                                    {
                                        return;
                                    }
                                    temp.Origin = projectPoint;
                                    temp.Rotation = TakeRotationOfSegmentPolyline(length, this.CableGeom);// OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);
                                    temp.Text = "[DESCRIPTION]";
                                    temp.Alignment = GTAlignmentConstants.gtalBottomLeft;                                    
                                    this.m_PTHelper.SetGeometry(temp);
                                    this.LabelCode = temp;
                                }
            }
        }

        public void StartPlacement(Intergraph.GTechnology.API.IGTPlacementTechniqueHelper PTHelper, Intergraph.GTechnology.API.IGTKeyObject KeyObject, Intergraph.GTechnology.API.IGTKeyObjects KeyObjectCollection)
        {
            try
            {
                m_PTHelper = PTHelper;
                m_KeyObject = KeyObject;
                m_KeyObjectCollection = KeyObjectCollection;
                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();

                m_bRotateMode = false;
                // Disable construction aids and status bar prompts for silent placement techinque.
                m_PTHelper.ConstructionAidsEnabled = GTConstructionAidsEnabledConstants.gtptConstructionAidsNone;
                m_PTHelper.ConstructionAidDynamicsEnabled = false;
                m_PTHelper.StatusBarPromptsEnabled = false;
                m_PTHelper.StartPlacement(m_KeyObject, m_KeyObjectCollection);

                if (m_GComp.Arguments != null)
                {
                    if (m_GComp.Arguments.Count > 0)
                    {
                        string strFNO = m_GComp.Arguments.GetArgument(0).ToString();
                        if (!string.IsNullOrEmpty(strFNO))
                        {
                            string[] sArray = strFNO.Split('/');

                            for (int i = 0; i < sArray.Length; i+=2)
                            {
                                lstRelativeCNO.Add(Convert.ToInt16(sArray[i]));
                                lstRelativeCID.Add(Convert.ToInt16(sArray[i + 1]));
                            }
                        }
                        string strconnFNO = m_GComp.Arguments.GetArgument(1).ToString();
                       // strconnFNO = "5500/";
                        if (!string.IsNullOrEmpty(strconnFNO))
                        {
                            string[] sArray = strconnFNO.Split(',');

                            for (int i = 0; i < sArray.Length; i++)
                            {
                                iOwnerFNO.Add(short.Parse( sArray[i].Trim()));
                                //if (!string.IsNullOrEmpty(strOwnerName)) strOwnerName = strOwnerName + "/";
                                //string sName = GetFeatureName(Convert.ToInt16(sArray[i]));
                                //if (!string.IsNullOrEmpty(sName)) strOwnerName = strOwnerName + sName;
                                //lstOwnerFNO.Add(Convert.ToInt16(sArray[i]));
                            }
                        }
                    }
                }

                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source ODF...");

                m_ODFForm = new frmODF();
                m_ODFForm.EXC_ABB = GetExchange();
                m_ODFForm.ODF_FNO = iOwnerFNO;
                m_ODFForm.ShowDialog();

               

                iOwnerFNO[0] =  m_ODFForm.ODF_FNO[0];
                iOwnerFID = m_ODFForm.ODF_FID;
                m_ODFForm.Close();

                IGTGeometry oLine = null;
                for (int i = 0; i < m_GComps.Count; i++)
                {
                    for (int j = 0; j < lstRelativeCNO.Count; j++)
                    {
                        if (m_GComps[i].CNO == lstRelativeCNO[j])
                            oLine = m_GComps[i].Geometry;
                    }
                }

                if (oLine == null)
                {
                    m_PTHelper.AbortPlacement();
                    return;
                }
                this.CableGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                try
                {
                    this.CableGeom = (IGTPolylineGeometry)((IGTCompositePolylineGeometry)oLine).ExtractGeometry(((IGTCompositePolylineGeometry)oLine).FirstPoint, ((IGTCompositePolylineGeometry)oLine).LastPoint, false);
              
                }catch (Exception ex){}
                
                IGTTextPointGeometry temp = GTClassFactory.Create<IGTTextPointGeometry>();
                temp.Origin = this.CableGeom.FirstPoint;
                temp.Rotation = TakeRotationOfSegmentPolyline(0, this.CableGeom);// OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);
                temp.Text = "[DESCRIPTION]";
                temp.Alignment = GTAlignmentConstants.gtalBottomLeft;
                this.m_PTHelper.SetGeometry(temp);
                this.LabelCode = GTClassFactory.Create<IGTTextPointGeometry>();
                this.LabelCode = temp;

                short mintContainRelationshipNumber ;

                mintContainRelationshipNumber = m_ODFForm.NR_DIRECTION;

                if (iOwnerFID > 0)
                {
                    IGTRelationshipService mobjRelationshipService;
                    IGTKeyObject oOwner = application.DataContext.OpenFeature(iOwnerFNO[0], iOwnerFID);

                    mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>();
                    mobjRelationshipService.DataContext = application.DataContext;

                    //Ownership        
                    mobjRelationshipService.ActiveFeature = m_KeyObject;
                    if (mobjRelationshipService.AllowSilentEstablish(oOwner))
                    {
                        mobjRelationshipService.SilentEstablish(mintContainRelationshipNumber, oOwner);
                    }
                }
                if (m_ODFForm.DialogResult != DialogResult.OK)
                {
                    m_PTHelper.EndPlacement();
                    return;
                }else
                    mintState = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(("StartPlacement:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                m_PTHelper.AbortPlacement();
            }
        }
        #region Coordinate for Projected Point on Conduit line
        public IGTPoint PointOnConduit(double Xslash, double Yslash, IGTPolylineGeometry DuctPathLineGeom)
        {

            IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
            projectPoint.X = 0;
            projectPoint.Y = 0;
            projectPoint.Z = 0;
            List<IGTPoint> ProjectedPoints = new List<IGTPoint>();
            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {
                ProjectedPoints.Add(ProjectedPointOnConduit(DuctPathLineGeom.Points[i].X,
                DuctPathLineGeom.Points[i].Y,
                DuctPathLineGeom.Points[i + 1].X,
                DuctPathLineGeom.Points[i + 1].Y,
                Xslash, Yslash));
            }
            double min = 0;
            int sectnum = 0;
            for (int i = 0; i < ProjectedPoints.Count; i++)
            {
                double difX = ProjectedPoints[i].X - Xslash;
                double difY = ProjectedPoints[i].Y - Yslash;

                if (difX < 0) difX *= -1;
                if (difY < 0) difY *= -1;
                double disMin = difY + difX;
                if (min == 0) min = disMin + 1;
                if (min > disMin)
                {
                    projectPoint.X = ProjectedPoints[i].X;
                    projectPoint.Y = ProjectedPoints[i].Y;
                    projectPoint.Z = 0;
                    min = disMin;
                    sectnum = i + 1;
                }

            }
            //checking if mouse click too far from conduit
            if (min > 3)
            {
                projectPoint.X = 0;
                projectPoint.Y = 0;
                projectPoint.Z = 0;
                sectnum = 0;
            }
            if (sectnum > 0)
            {

                int length = 0;
                for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
                {
                    if (sectnum == i + 1)
                    {
                        length += LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                        DuctPathLineGeom.Points[i].Y, Xslash, Yslash);
                        break;
                    }
                    else
                        length += LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                            DuctPathLineGeom.Points[i].Y,
                            DuctPathLineGeom.Points[i + 1].X,
                            DuctPathLineGeom.Points[i + 1].Y);
                }
                projectPoint.Z = double.Parse(length.ToString());
            }
            return projectPoint;

        }
        #endregion
        #region Coordinate for Projected Point on SEGMENT of Conduit line
        public IGTPoint ProjectedPointOnConduit(double stX, double stY, double endX, double endY, double slashX, double slashY)
        {
            IGTSegment sectT = GTClassFactory.Create<IGTSegment>();
            IGTSegmentPoint slashT = GTClassFactory.Create<IGTSegmentPoint>();
            IGTPoint clickT = GTClassFactory.Create<IGTPoint>();
            IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
            sectT.Point1.X = stX;
            sectT.Point1.Y = stY;
            sectT.Point1.Z = 0;
            sectT.Point2.X = endX;
            sectT.Point2.Y = endY;
            sectT.Point2.Z = 0;

            clickT.X = slashX;
            clickT.Y = slashY;
            clickT.Z = 0;

            slashT = clickT.ProjectTo(sectT);

            projectPoint.X = slashT.Point.X;
            projectPoint.Y = slashT.Point.Y;
            projectPoint.Z = 0;

            return projectPoint;
        }
             #endregion
        #region Calculate Rotation for Along plc lable
        public double TakeRotationOfSegmentPolyline(int GraphicLength, IGTPolylineGeometry DuctPathLineGeom)
        {
            int lengthTemp = 0;
            double Rotat = 0.0;
            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {

                int temp = LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                    DuctPathLineGeom.Points[i].Y,
                    DuctPathLineGeom.Points[i + 1].X,
                    DuctPathLineGeom.Points[i + 1].Y);
                lengthTemp += temp;
                if (lengthTemp >= GraphicLength)
                {
                    if (DuctPathLineGeom.Points[i + 1].Y < DuctPathLineGeom.Points[i].Y &&
                        DuctPathLineGeom.Points[i + 1].X <= DuctPathLineGeom.Points[i].X
                        || DuctPathLineGeom.Points[i + 1].Y > DuctPathLineGeom.Points[i].Y &&
                        DuctPathLineGeom.Points[i + 1].X <= DuctPathLineGeom.Points[i].X)
                        Rotat = AngleBtwPoint(DuctPathLineGeom.Points[i + 1].X, DuctPathLineGeom.Points[i + 1].Y,
                            DuctPathLineGeom.Points[i].X, DuctPathLineGeom.Points[i].Y);
                    else
                        Rotat = AngleBtwPoint(DuctPathLineGeom.Points[i].X, DuctPathLineGeom.Points[i].Y,
                                          DuctPathLineGeom.Points[i + 1].X, DuctPathLineGeom.Points[i + 1].Y);
                    break;
                }
            }
            return Rotat;
        }
         #endregion
        #region Angle between segment and OX by start and end's points on segment
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
        private int LegthBtwTwoPoints(double startPointX, double startPointY, double endPointX, double endPointY)
        {
            return Convert.ToInt32(Math.Round(Math.Sqrt(Math.Pow((endPointX - startPointX), 2) + Math.Pow((endPointY - startPointY), 2)), 0));
        }
        #endregion

        void m_PTHelper_ConstructionAidComplete(object sender, GTConstructionAidCompleteEventArgs e)
        {
            bool bAutomatic = true;
            if (bAutomatic)
            {
                e.AutomaticallyAppend = true;
            }
            else
            {
                // We do not want Construction Aids to add any geometry, we will do it ourselves.
                e.AutomaticallyAppend = false;
                // The Construction Aids finished. There should be a point we can use, so append/process it.
                IGTPointGeometry UserPoint = (IGTPointGeometry)e.ConstructedGeometry;
                m_PTHelper.AppendGeometry(UserPoint);
            }
        }

        void m_PTHelper_ArcComplete(object sender, EventArgs e)
        {
        }

        #endregion

    }
}
