using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using AG.GTechnology.Utilities;
using ADODB;

namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    //public static clsPlotProperties propPlot = new clsPlotProperties();
    class clsPlotProperties
    {
        #region Variables
         public static int m_iDRI_ID;
         public static string m_sType;
         public static string m_sMapScale;
         public static string m_sMapScaleCustom;
         public static string m_sMapScalePreDefined;
         public static int m_lMapScale;
         public static string m_sSheetName;
         public static string m_sSheetSize;
         public static string m_sSheetOrientation;
         public static int m_lSheetId;
         public static double m_dSheetHeight;
         public static double m_dSheetWidth;
         public static double m_dSheet_Inset;
         public static double m_dCaptionStampTLX;
         public static double m_dCaptionStampTLY;

         public static double m_dMapTLX;
         public static double m_dMapTLY;
         public static double m_dMapBRX;
         public static double m_dMapBRY;

         public static double m_dMapHeight;
         public static double m_dMapWidth;

         public static IGTPoint m_oMapTLPoint;
         public static IGTPoint m_oMapBRPoint;


         public static bool m_bPlaceComponment = false;
         public static double m_dMapHeightScaled;
         public static double m_dMapWidthScaled;
         public static bool load_first_time = true;

        #endregion


        public static IGTPrintProperties PrintProperties()
        {
                IGTPrintProperties prtProp = GTClassFactory.Create<IGTPrintProperties>();
                    prtProp.PaperHeight = m_dSheetHeight;
                prtProp.PaperWidth = m_dSheetWidth;
               // prtProp.PaperName = m_sSheetName;
               // prtProp.PaperSize = m_sSheetSize;

                return prtProp;
        }

        #region Properties
        public static double MapHeightScaled
        {
            get
            {
                return m_dMapHeightScaled;
            }
            set
            {
                m_dMapHeightScaled = value;
            }
        }

         public static double MapWidthScaled
        {
            get
            {
                return m_dMapWidthScaled;
            }
            set
            {
                m_dMapWidthScaled = value;
            }
        }
            
        #endregion
    }
}
