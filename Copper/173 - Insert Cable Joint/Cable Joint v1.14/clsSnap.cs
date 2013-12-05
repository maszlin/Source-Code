/*
 * Class    : Snap
 * Desc     : snap mouse pointer to selected feature
 * Build    : m.zam@azza4u.com
 * Date     : 07-06-2012
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Runtime.InteropServices;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;

namespace NEPS.GTechnology.Cable_Joint
{
    class clsSnap
    {
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        
        internal static IGTPoint SnapPointer(IGTGeometry geo, IntPtr hWnd, IGTPoint worldPoint)
        {
            IGTSnapService oSnap = GTClassFactory.Create<IGTSnapService>();
            IGTPoint oSnapPoint;
            GTSnapTypesConstants snapType;
            oSnap.SnapTolerance = 100000; //1000000;

            oSnap.SnapTypesEnabled = GTSnapTypesEnabledConstants.gtssSnapToElement;

            snapType = oSnap.SnapToGeometry(geo, worldPoint, out oSnapPoint);
            if (snapType != GTSnapTypesConstants.gtssNotSnapped)
            {
                IGTPoint oWindowPoint = GTClassFactory.Create<IGTApplication>().ActiveMapWindow.WorldToWindow(oSnapPoint);

                Point CursorPoint = new Point();
                CursorPoint.X = (int)oWindowPoint.X;
                CursorPoint.Y = (int)oWindowPoint.Y;

                ClientToScreen(hWnd, ref CursorPoint); //This is relative to app not mapwindow
                SetCursorPos(CursorPoint.X, CursorPoint.Y);

                return oSnapPoint; // return the new worldpoint after snap
            }
            else
            {
                return worldPoint;
            }

        }

    }
}
