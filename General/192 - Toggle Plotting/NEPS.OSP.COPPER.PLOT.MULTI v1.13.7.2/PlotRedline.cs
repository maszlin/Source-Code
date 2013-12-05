private static GTMapWindow mobjMapWindow = null;

arrManholeGeom = new List<GTGeometry>();
arrManholeStyle = new List<object>();
arrPipeGeom = new List<GTGeometry>();
arrPipeStyle = new List<object>();

if (mobjMapWindow != null) 
{
	mobjMapWindow.DisplayService.AppendTemporaryGeometries("SLD Diagram", "Manhole Schematic", arrManholeGeom.ToArray(), arrManholeStyle.ToArray());
	mobjMapWindow.DisplayService.AppendTemporaryGeometries("SLD Diagram", "Pipe Schematic", arrPipeGeom.ToArray(), arrPipeStyle.ToArray());
	mobjMapWindow.FitAll();
}

AddRedlines(arrManholeGeom, arrManholeStyle, -1, 8);
AddRedlines(arrPipeGeom, arrPipeStyle);

private static void AddRedlines(List<GTGeometry> arrGeom, List<object> arrStyle, int iGroupNumber)
{
	AddRedlines(arrGeom, arrStyle, iGroupNumber, -1);
}

private static void AddRedlines(List<GTGeometry> arrGeom, List<object> arrStyle)
{
	AddRedlines(arrGeom, arrStyle, 0);
}


private static void AddRedlines(List<GTGeometry> arrGeom, List<object> arrStyle, int iGroupNumber, int iSize)
{
	int lStyleID;
	GTGeometry oGeom = null;
	GTPlotWindow _PlotWindow = null;
	GTPlotRedline oGTPlotRedline = null;
	int iRLGroup = -1;
	int oldPage = -1;
	for (int i = 0; i < arrGeom.Count; i++)
	{
		int iPage = (int)((FeatMaxY - arrGeom[i].Range.BottomLeft.Y) / 1400);

		if (oldPage != iPage)
		{
			oldPage = iPage;
			if (iGroupNumber != 0)
				iRLGroup = -1;
			else
				iRLGroup = 0;
		}
		lStyleID = (int)arrStyle[i];
		oGeom = SetRatio(arrGeom[i], iPage);

		_PlotWindow = GetPage(iPage+1);
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
				GTSymbology overrideStyle = oGTPlotRedline.Symbology;
				overrideStyle.Size = iSize * 72;
				oGTPlotRedline.Symbology = overrideStyle;
			}
		}
	}
}