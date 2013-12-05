namespace NEPS.GTechnology.InsertSplice
{
    using Intergraph.GTechnology.API;
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    internal class clsSnap
    {
        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
        [DllImport("user32.dll", CallingConvention=CallingConvention.StdCall)]
        private static extern void SetCursorPos(int X, int Y);
        internal static IGTPoint SnapPointer(IGTGeometry geo, IntPtr hWnd, IGTPoint worldPoint)
        {
            IGTPoint point;
            IGTSnapService service = GTClassFactory.Create<IGTSnapService>();
            service.SnapTolerance = 100000.0;
            service.SnapTypesEnabled = GTSnapTypesEnabledConstants.gtssSnapToElement;
            if (service.SnapToGeometry(geo, worldPoint, out point) != GTSnapTypesConstants.gtssNotSnapped)
            {
                IGTPoint point2 = GTClassFactory.Create<IGTApplication>().ActiveMapWindow.WorldToWindow(point);
                Point lpPoint = new Point((int) point2.X,(int) point2.Y);
                ClientToScreen(hWnd, ref lpPoint);
                SetCursorPos(lpPoint.X, lpPoint.Y);
                return point;
            }
            return worldPoint;
        }
    }
}

