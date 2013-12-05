using System;
using System.Collections.Generic;
using System.Text;

namespace AG.GTechnology.DuctSpaceReport
{
    class DuctFormationCfg
    {
        private string _text = string.Empty;
        public string Text
        {
            set { _text = value; }
            get { return _text; }
        }

        private int _rows = 0;
        public int Rows
        {
            set { _rows = value; }
            get { return _rows; }
        }

        private int _cols = 0;
        public int Cols
        {
            set { _cols = value; }
            get { return _cols; }
        }

        public string ToString
        {
            get { return _text; }
        }
    }
}
