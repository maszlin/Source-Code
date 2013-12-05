using System;
using System.Collections.Generic;
using System.Text;

namespace AG.GTechnology.ManholePlacement
{
    class ManholeBnd
    {
        private string _type = string.Empty;
        public string ManholeType
        {
            set { _type = value; }
            get { return _type; }
        }

    }
}
