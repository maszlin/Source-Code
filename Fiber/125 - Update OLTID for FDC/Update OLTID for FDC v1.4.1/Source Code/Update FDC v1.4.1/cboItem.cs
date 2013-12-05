using System;
using System.Collections.Generic;
using System.Text;

namespace NEPS.GTechnology.UpdateFDC
{
    class cboItem
    {
        private string _value;

        private string _name;
        public string Value

        {
        get { return _value; }set { _value = value; }

        }
        public string Name

        {

        get { return _name; }
        set { _name = value; }

        }
        public cboItem(string name, string value)

        {

        _name = name;

        _value = value;

        }
        public override string ToString()

        {
        return _name;

        }
    }
}
