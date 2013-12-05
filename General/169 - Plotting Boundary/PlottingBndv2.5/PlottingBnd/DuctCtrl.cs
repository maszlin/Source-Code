using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.API;

namespace AG.GTechnology.DuctSpaceReport
{
    public enum ConnectType
    {
        from = 0,
        to = 1
    }

    public partial class DuctCtrl : UserControl
    {
        public DuctCtrl()
        {
            InitializeComponent();
            btnDuct.Text = this.Text;
            btnDuct.Tag = this;
        }

        public DuctCtrl(int col, int row, ConnectType type)
        {
            InitializeComponent();
            _row = row;
            _col = col;
            _type = type;
            btnDuct.Text = this.Text;
            btnDuct.Tag = this;
        }

        private IGTKeyObject _oDuct = null;
        public IGTKeyObject Duct
        {
            set { _oDuct = value; }
            get { return _oDuct; }
        }

        private int _fid = 0;
        public int FID
        {
            set { _fid = value; }
            get { return _fid; }
        }

        private int _row = 0;
        public int Row
        {
            set { _row = value; }
            get { return _row; }
        }

        private int _col = 0;
        public int Col
        {
            set { _col = value; }
            get { return _col; }
        }

        private double _x = 0;
        public double X
        {
            set { _x = value; }
            get { return _x; }
        }

        private double _y = 0;
        public double Y
        {
            set { _y = value; }
            get { return _y; }
        }

        private ConnectType _type = ConnectType.from;
        public ConnectType Type
        {
            set { _type = value; }
            get { return _type; }
        }

        private bool _enable = true;
        public bool Enabled
        {
            set 
            { 
                _enable = value;
                btnDuct.Enabled = _enable;
                base.Enabled = _enable;
                if (_enable) 
                    btnDuct.BackColor = Color.ForestGreen; 
                else 
                    btnDuct.BackColor = Color.Red;
            }
            get { return btnDuct.Enabled; }
        }

        private string _text = string.Empty;
        public string Text
        {
            get 
            {
                _text = char.ConvertFromUtf32(65 + Row);
                _text = _text + (Col + 1).ToString();
                return _text; 
            }
        }

        public new event EventHandler Click
        {
            add
            {
                btnDuct.Click += value;
                //base.Click += value;
                //foreach (Control control in Controls)
                //{
                //    control.Click += value;
                //}
            }
            remove
            {
                btnDuct.Click -= value;
                //base.Click -= value;
                //foreach (Control control in Controls)
                //{
                //    control.Click -= value;
                //}
            }
        }
    }
}
