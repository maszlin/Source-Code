using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;

namespace NEPS.GTechnology.AssignJob
{
    public class FeatureItem
    {
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

	    public int FID
	    {
		    get { return originalFeature.FID;}
	    }

        public short FNO
	    {
            get { return originalFeature.FNO; }
	    }

        public IGTKeyObject originalFeature;

        public override string ToString()
        {
            return string.Format("{0}", FID);
        }

        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

    }
}
