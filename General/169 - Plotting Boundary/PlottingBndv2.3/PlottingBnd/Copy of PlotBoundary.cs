using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.API;
using Intergraph.GTechnology.Interfaces;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;

namespace AG.GTechnology.Utilities
{
    [DefaultPropertyAttribute("PaperSize")]
    public class PlotBoundary //: IGTNamedPlot
    {

        private ICollection mo_Attributes;

        private int mlng_FNO;

        private int mlng_FID;

        private short mlng_CNO;

        private int mlng_CID;

        private int mlng_DetailID;

        private string mstr_Name;

        private string mstr_Type;

        private string mstr_PaperSize;

        private string mstr_PaperOrientation;

        private bool mbln_Adhoc;

        public ICollection Attributes
        {
            get
            {
                return mo_Attributes;
            }
            set
            {
                mo_Attributes = value;
            }
        }

        [CategoryAttribute("Plot Boundary")]
        [DisplayNameAttribute("Name")]
        [DescriptionAttribute("Plot Boundary Name")]
        [ReadOnlyAttribute(true)]
        public string Name
        {
            get
            {
                return mstr_Name;
            }
            set
            {
                mstr_Name = value;
            }
        }

        [CategoryAttribute("Plot Boundary")]
        [DisplayNameAttribute("Type")]
        [DescriptionAttribute("Plot Boundary Type - Used to drive the default Title Block and Legend used etc.")]
        [ReadOnlyAttribute(true)]
        public string Type
        {
            get
            {
                return mstr_Type;
            }
            set
            {
                mstr_Type = value;
            }
        }

        [CategoryAttribute("Plot Boundary")]
        [DisplayNameAttribute("Paper Size")]
        [DescriptionAttribute("Plot Boundary Paper Size")]
        [ReadOnlyAttribute(true)]
        public string PaperSize
        {
            get
            {
                return mstr_PaperSize;
            }
            set
            {
                mstr_PaperSize = value;
            }
        }

        [CategoryAttribute("Plot Boundary")]
        [DisplayNameAttribute("Paper Orientation")]
        [DescriptionAttribute("Plot Boundary Paper Orientation")]
        [ReadOnlyAttribute(true)]
        public string PaperOrientation
        {
            get
            {
                return mstr_PaperOrientation;
            }
            set
            {
                mstr_PaperOrientation = value;
            }
        }

        [BrowsableAttribute(false)]
        public bool Adhoc
        {
            get
            {
                return mbln_Adhoc;
            }
            set
            {
                mbln_Adhoc = value;
            }
        }

        [BrowsableAttribute(false)]
        public int FNO
        {
            get
            {
                return mlng_FNO;
            }
            set
            {
                mlng_FNO = value;
            }
        }

        [BrowsableAttribute(false)]
        public int FID
        {
            get
            {
                return mlng_FID;
            }
            set
            {
                mlng_FID = value;
            }
        }

        [BrowsableAttribute(false)]
        public short CNO
        {
            get
            {
                return mlng_CNO;
            }
            set
            {
                mlng_CNO = value;
            }
        }

        [BrowsableAttribute(false)]
        public int CID
        {
            get
            {
                return mlng_CID;
            }
            set
            {
                mlng_CID = value;
            }
        }

        [BrowsableAttribute(false)]
        public int DetailID
        {
            get
            {
                return mlng_DetailID;
            }
            set
            {
                mlng_DetailID = value;
            }
        }
    }
}