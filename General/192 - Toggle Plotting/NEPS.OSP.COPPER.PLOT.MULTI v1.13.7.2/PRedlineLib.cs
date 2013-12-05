using Intergraph.GTechnology.API;
using System.Collections.Generic;
using System.Text;
using System;

/////////////////////////////
// La Viet Phuong - Jan 2012
////////////////////////////
namespace AG.GTechnology.Utilities
{
    public class PRedlineLib
    {
        private static double Pi = 3.141592653589793;
        private static int MapMinX = 0;
        private static int MapMinY = 0;
        private static int MapMaxX = 0;
        private static int MapMaxY = 0;
        private static int MapRatioX = 0;
        private static int MapRatioY = 0;
        private static int PageSize = 0;

        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();

        #region Math - calculation functions
        private static IGTGeometry SetRatio(IGTGeometry inGeom, int iPage)
        {
            IGTPoint _pnt = null;
            switch (inGeom.Type)
            {
                case "TextPointGeometry":
                    IGTTextPointGeometry oTempText = (IGTTextPointGeometry)inGeom;
                    IGTTextPointGeometry oNewText = GTClassFactory.Create<IGTTextPointGeometry>();
                    _pnt = GTClassFactory.Create<IGTPoint>();
                    //_pnt = new GTPoint();
                    _pnt.X = MapMinX + oTempText.Origin.X * MapRatioX;
                    _pnt.Y = MapMinY + (MapMaxY - oTempText.Origin.Y * MapRatioY) - iPage * PageSize * MapRatioY;
                    oNewText.Origin = _pnt;
                    oNewText.Rotation = oTempText.Rotation;
                    oNewText.Text = oTempText.Text;
                    oNewText.Alignment = oTempText.Alignment;
                    return oNewText;
                case "OrientedPointGeometry":
                    IGTOrientedPointGeometry oTempPoint = (IGTOrientedPointGeometry)inGeom;
                    IGTOrientedPointGeometry oNewPoint = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    _pnt = GTClassFactory.Create<IGTPoint>();
                    //_pnt = new GTPoint();
                    _pnt.X = MapMinX + oTempPoint.Origin.X * MapRatioX;
                    _pnt.Y = MapMinY + (MapMaxY - oTempPoint.Origin.Y * MapRatioY) - iPage * PageSize * MapRatioY;
                    oNewPoint.Origin = _pnt;
                    oNewPoint.Orientation = oTempPoint.Orientation;
                    return oNewPoint;
                case "PolylineGeometry":
                    IGTPolylineGeometry oTempLine = (IGTPolylineGeometry)inGeom;
                    IGTPolylineGeometry oNewLine = GTClassFactory.Create<IGTPolylineGeometry>();
                    for (int i = 0; i < oTempLine.Points.Count; i++)
                    {
                        _pnt = GTClassFactory.Create<IGTPoint>();
                        //_pnt = new GTPoint();
                        _pnt.X = MapMinX + oTempLine.Points[i].X * MapRatioX;
                        _pnt.Y = MapMinY + (MapMaxY - oTempLine.Points[i].Y * MapRatioY) - iPage * PageSize * MapRatioY;
                        oNewLine.Points.Add(_pnt);
                    }
                    return oNewLine;
            }
            return null;
        }

        #endregion

        #region Add redline functions
        public static void AddText(IGTPlotWindow _PlotWindow, int lStyleID, double X, double Y, string text)
        {
            IGTGeometry oGeom = PGeoLib.CreateTextGeom(X, Y, text, 0, GTAlignmentConstants.gtalCenterLeft);

            IGTPlotRedline oGTPlotRedline = _PlotWindow.NamedPlot.NewRedline(oGeom, lStyleID);

        }

        public static void AddPoint(IGTPlotWindow _PlotWindow, int lStyleID, double X, double Y)
        {
            IGTGeometry oGeom = PGeoLib.CreatePointGeom(X, Y);

            IGTPlotRedline oGTPlotRedline = _PlotWindow.NamedPlot.NewRedline(oGeom, lStyleID);
        }

        public static void AddLine(IGTPlotWindow _PlotWindow, int lStyleID, double X1, double Y1, double X2, double Y2)
        {
            IGTPolylineGeometry oPolylineGeometry = GTClassFactory.Create<IGTPolylineGeometry>();

            IGTPoint oGTPoint = GTClassFactory.Create<IGTPoint>();
            oGTPoint.X = X1;
            oGTPoint.Y = Y1;
            oGTPoint.Z = 0;
            oPolylineGeometry.Points.Add(oGTPoint);

            oGTPoint = GTClassFactory.Create<IGTPoint>();
            oGTPoint.X = X2;
            oGTPoint.Y = Y2;
            oGTPoint.Z = 0;
            oPolylineGeometry.Points.Add(oGTPoint);

            //IGTPlotRedline oGTPlotRedline = application.NamedPlots["DSR_1"].NewRedline(oPolylineGeometry, 5010);
            IGTPlotRedline oGTPlotRedline = _PlotWindow.NamedPlot.NewRedline(oPolylineGeometry, lStyleID);
        }

        public static void AddGeometry(IGTPlotWindow _PlotWindow, IGTGeometry oGeom, int lStyleID)
        {
            AddGeometry(_PlotWindow, oGeom, lStyleID, 0);
        }
        
        public static void AddGeometry(IGTPlotWindow _PlotWindow, IGTGeometry oGeom, int lStyleID, int iSize)
        {
            IGTPlotRedline oGTPlotRedline = _PlotWindow.NamedPlot.NewRedline(oGeom, lStyleID);

            if ((oGeom.Type == "OrientedPointGeometry") && (iSize > 0))
            {
                IGTSymbology overrideStyle = oGTPlotRedline.Symbology;
                overrideStyle.Size = iSize * 72;
                oGTPlotRedline.Symbology = overrideStyle;
            }
        }

        public static void AddMapwindow(IGTPlotWindow _PlotWindow, IGTMapWindow _MapWindow, double X1, double Y1, double X2, double Y2, Dictionary<string, string> ListFilter, IGTDDCKeyObjects oManhole)
        {
            IGTPlotMap oGTPlotMap = GTClassFactory.Create <IGTPlotMap>();
            IGTPaperRange oGTLocation = GTClassFactory.Create <IGTPaperRange>();

            IGTPoint _pnt = GTClassFactory.Create<IGTPoint>();
            _pnt.X = X1;
            _pnt.Y = Y1;
            oGTLocation.TopLeft=_pnt;

            _pnt = GTClassFactory.Create<IGTPoint>();
            _pnt.X = X2;
            _pnt.Y = Y2;
            oGTLocation.BottomRight = _pnt;

            oGTPlotMap = _PlotWindow.InsertMap(oGTLocation);
            oGTPlotMap.DisplayService.ReplaceLegend(_MapWindow.LegendName);
            oGTPlotMap.Frame.Activate();
            //oGTPlotMap.FitSelectedObjects(1);
            //oGTPlotMap.ZoomArea(_MapWindow.GetRange());
            application.SelectedObjects.Clear();
            for (int i = 0; i < oManhole.Count; i++)
                application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllComponentsOfFeature, oManhole[i]);
            oGTPlotMap.FitSelectedObjects(1.5);
            application.RefreshWindows();

            IGTDisplayControlNodes toNodes = oGTPlotMap.DisplayService.GetDisplayControlNodes();
            for (int i = 0; i < toNodes.Count; i++)
            {
                IGTDisplayControlNode toNode = toNodes[i];
                string NodeName = toNode.DisplayName;
                if (toNode.LegendEntry != null)
                {
                    if (NodeName.Contains("Manhole"))
                    {
                        toNode.LegendEntry.Displayable = true;
                        if (ListFilter.ContainsKey("MANHOLE")) 
                            toNode.LegendEntry.Filter = "G3E_FID=" + ListFilter["MANHOLE"];
                    }
                    else if (NodeName.Contains("Duct Nest"))
                    {
                        toNode.LegendEntry.Displayable = true;
                        if (ListFilter.ContainsKey("FORMATION"))
                            if (!string.IsNullOrEmpty(ListFilter["FORMATION"]))
                                toNode.LegendEntry.Filter = "G3E_FID in (" + ListFilter["FORMATION"] + ")";
                    }
                    else if (NodeName.StartsWith("Duct From") ||
                        NodeName.StartsWith("Duct To"))
                    {
                        toNode.LegendEntry.Displayable = true;
                        if (ListFilter.ContainsKey("DUCT"))
                            if (!string.IsNullOrEmpty(ListFilter["DUCT"]))
                                toNode.LegendEntry.Filter = "G3E_FID in (" + ListFilter["DUCT"] + ")";
                    }
                    else if (NodeName.StartsWith("Sub Duct From") ||
                        NodeName.StartsWith("Sub Duct To"))
                    {
                        toNode.LegendEntry.Displayable = true;
                        if (ListFilter.ContainsKey("SUBDUCT"))
                            if (!string.IsNullOrEmpty(ListFilter["SUBDUCT"]))
                                toNode.LegendEntry.Filter = "G3E_FID in (" + ListFilter["SUBDUCT"] + ")";
                    }
                    else if (NodeName.StartsWith("Inner Duct From") ||
                            NodeName.StartsWith("Inner Duct To"))
                    {
                        toNode.LegendEntry.Displayable = true;
                        if (ListFilter.ContainsKey("INNERDUCT"))
                            if (!string.IsNullOrEmpty(ListFilter["INNERDUCT"]))
                                toNode.LegendEntry.Filter = "G3E_FID in (" + ListFilter["INNERDUCT"] + ")";
                    }
                    else
                        toNode.LegendEntry.Displayable = false;
                }
            }

            //IGTDisplayControlNodes fromNodes = _MapWindow.DisplayService.GetDisplayControlNodes();
            //IGTDisplayControlNodes toNodes = oGTPlotMap.DisplayService.GetDisplayControlNodes();
            //for (int i = 0; i < fromNodes.Count; i++)
            //{
            //    IGTDisplayControlNode fromNode = fromNodes[i];
            //    string NodeName = fromNode.DisplayPathName;

            //    IGTDisplayControlNode toNode = toNodes[i];
            //    if (toNode.DisplayPathName == NodeName)
            //    {
            //        if (toNode.LegendEntry != null)
            //        {
            //            if (toNode.LegendEntry.Displayable != fromNode.LegendEntry.Displayable)
            //                toNode.LegendEntry.Displayable = fromNode.LegendEntry.Displayable;

            //            //if (toNode.LegendEntry.DisplayScaleMode != fromNode.LegendEntry.DisplayScaleMode)
            //            //    toNode.LegendEntry.DisplayScaleMode = fromNode.LegendEntry.DisplayScaleMode;

            //            if (toNode.LegendEntry.Locatable != fromNode.LegendEntry.Locatable)
            //                toNode.LegendEntry.Locatable = fromNode.LegendEntry.Locatable;

            //            if (toNode.LegendEntry.Filter != fromNode.LegendEntry.Filter)
            //                toNode.LegendEntry.Filter = fromNode.LegendEntry.Filter;
            //        }
            //    }
            //}

            //oGTPlotMap.FitAll();
            oGTPlotMap.Frame.Deactivate();
            application.RefreshWindows();
            //oGTPlotMap.CenterSelectedObjects();
        }

        public static void AddRedlines(IGTPlotWindow _PlotWindow, List<IGTGeometry> arrGeom, List<object> arrStyle, int iGroupNumber)
        {
            AddRedlines(_PlotWindow, arrGeom, arrStyle, iGroupNumber, -1);
        }

        public static void AddRedlines(IGTPlotWindow _PlotWindow, List<IGTGeometry> arrGeom, List<object> arrStyle)
        {
            AddRedlines(_PlotWindow, arrGeom, arrStyle, 0);
        }

        public static void AddRedlines(IGTPlotWindow _PlotWindow, List<IGTGeometry> arrGeom, List<object> arrStyle, int iGroupNumber, int iSize)
        {
            int lStyleID;
            IGTGeometry oGeom = null;
            IGTPlotRedline oGTPlotRedline = null;
            int iRLGroup = -1;
            for (int i = 0; i < arrGeom.Count; i++)
            {
                if (iGroupNumber != 0)
                    iRLGroup = -1;
                else
                    iRLGroup = 0;

                lStyleID = (int)arrStyle[i];
                oGeom = arrGeom[i];
                //oGeom = SetRatio(arrGeom[i].Geom, iPage);

                if (_PlotWindow != null)
                {
                    if (iRLGroup != 0)
                    {
                        oGTPlotRedline = _PlotWindow.NamedPlot.NewRedline(oGeom, lStyleID, iRLGroup);
                        iRLGroup = oGTPlotRedline.GroupNumber;
                    }
                    else
                    {
                        oGTPlotRedline = _PlotWindow.NamedPlot.NewRedline(oGeom, lStyleID);
                        iRLGroup = oGTPlotRedline.GroupNumber;
                    }

                    if ((oGeom.Type == "OrientedPointGeometry") && (iSize > 0))
                    {
                        IGTSymbology overrideStyle = oGTPlotRedline.Symbology;
                        overrideStyle.Size = iSize * 72;
                        oGTPlotRedline.Symbology = overrideStyle;
                    }
                }
            }
        }
        #endregion

        #region PlotWindow functions
        public static IGTPlotWindow CreatePlotWindow(string sName)
        {
            string genName = sName;
            bool bFound = false;

            IGTPlotWindows oWins;
            oWins = application.GetPlotWindows();

            IGTNamedPlots oPlots = application.NamedPlots;

            for (int j = 1; j < 999; j++)
            {
                genName = sName + "_" + j.ToString();
                bFound = false;
                for (int i = oPlots.Count - 1; i >= 0; i--)
                {
                    if (genName == oPlots[i].Name)
                    {
                        bFound = true;
                    }
                }
                if (!bFound) break;
            }
            oWins = null;

            IGTNamedPlot oNamedPlot = null;
            if (!bFound)
            {
                //application.NamedPlots.Remove(genName);
                oNamedPlot = application.NewNamedPlot(genName);
                oNamedPlot.PaperHeight = 42000.0;
                oNamedPlot.PaperWidth = 29700.0;
            }
            return application.NewPlotWindow(oNamedPlot);
        }

        #endregion


    }
}
