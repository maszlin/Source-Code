using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;

namespace NEPS.GTechnology.NEPSDuctNestPlc
{
    public class DuctNest
    {
        private IGTKeyObject _oDuctnest = null;
        public IGTKeyObject Ductnest
        {
            set { _oDuctnest = value; }
            get { return _oDuctnest; }
        }

        private int _fid = 0;
        public int FID
        {
            set { _fid = value; }
            get { return _fid; }
        }

        private int _cond_fid = 0;
        public int PATH_FID
        {
            set { _cond_fid = value; }
            get { return _cond_fid; }
        }

        private int _from_wall = 0;
        public int From_Wall
        {
            set { _from_wall = value; }
            get { return _from_wall; }
        }

        private int _to_wall = 0;
        public int To_Wall
        {
            set { _to_wall = value; }
            get { return _to_wall; }
        }

        private int _from_rows = 0;
        public int From_Rows
        {
            set { _from_rows = value; }
            get { return _from_rows; }
        }

        private int _from_cols = 0;
        public int From_Cols
        {
            set { _from_cols = value; }
            get { return _from_cols; }
        }

        private int _to_rows = 0;
        public int To_Rows
        {
            set { _to_rows = value; }
            get { return _to_rows; }
        }

        private int _to_cols = 0;
        public int To_Cols
        {
            set { _to_cols = value; }
            get { return _to_cols; }
        }

        private int _total_duct = 0;
        public int Total_duct
        {
            get
            {
                if ((_from_cols * _from_rows) != (_to_cols * _to_rows))
                    _total_duct = 0;
                else
                    _total_duct = _from_cols * _from_rows;
                return _total_duct;
            }
        }

        private string _text = string.Empty;
        public string Text
        {
            set { _text = value; }
            get { return _text; }
        }

        private List<DuctCtrl> _fromDucts = null;
        public List<DuctCtrl> FromDucts
        {
            set { _fromDucts = value; }
            get { return _fromDucts; }
        }

        private List<DuctCtrl> _toDucts = null;
        public List<DuctCtrl> ToDucts
        {
            set { _toDucts = value; }
            get { return _toDucts; }
        }

        private Dictionary<string, string> _DuctMapping = null;
        public Dictionary<string, string> DuctMapping
        {
            set { _DuctMapping = value; }
            get { return _DuctMapping; }
        }

        public DuctCtrl GetDuctByName(string name, int from_to)
        {
            if (from_to == 0)
            {
                if (_fromDucts == null) return null;
                foreach (DuctCtrl duct in _fromDucts)
                {
                    if (duct.Text == name.ToUpper()) return duct;
                }
            }
            else
            {
                if (_toDucts == null) return null;
                foreach (DuctCtrl duct in _toDucts)
                {
                    if (duct.Text == name.ToUpper()) return duct;
                }
            }

            return null;
        }

        public DuctCtrl GetDuctByXY(int col, int row, int from_to)
        {
            if (from_to == 0)
            {
                if (_fromDucts == null) return null;
                foreach (DuctCtrl duct in _fromDucts)
                {
                    if ((duct.Col == col) && (duct.Row == row)) return duct;
                }
            }
            else
            {
                if (_toDucts == null) return null;
                foreach (DuctCtrl duct in _toDucts)
                {
                    if ((duct.Col == col) && (duct.Row == row)) return duct;
                }
            }

            return null;
        }

        public void EnableDucts()
        {
            EnableDucts(true, ConnectType.from);
            foreach (DuctCtrl _duct in _fromDucts)
            {
                if (_DuctMapping.ContainsKey(_duct.Text))
                    _duct.Enabled = false;
            }

            EnableDucts(true, ConnectType.to);
            foreach (DuctCtrl _duct in _toDucts)
            {
                if (_DuctMapping.ContainsValue(_duct.Text))
                    _duct.Enabled = false;
            }
        }

        public void EnableDucts(bool enabled, ConnectType type)
        {
            if (type == ConnectType.from)
            {
                foreach (DuctCtrl _duct in _fromDucts)
                {
                    _duct.Enabled = enabled;
                }
            }
            else
            {
                foreach (DuctCtrl _duct in _toDucts)
                {
                    _duct.Enabled = enabled;
                }
            }
        }

        public void AddMapping(string fromDuct, string toDuct)
        {
            if (_DuctMapping.ContainsKey(fromDuct))
            {
                _DuctMapping.Remove(fromDuct);
            }

            if (_DuctMapping.ContainsKey("(none)"))
            {
                _DuctMapping.Remove("(none)");
            }

            if (string.IsNullOrEmpty(fromDuct)) fromDuct = "(none)";
            if (string.IsNullOrEmpty(toDuct)) toDuct = "(none)";
            _DuctMapping.Add(fromDuct, toDuct);
        }


        public DuctNest()
        {
            _DuctMapping = new Dictionary<string, string>();
            _toDucts = new List<DuctCtrl>();
            _fromDucts = new List<DuctCtrl>();
            _from_cols = 0;
            _from_rows = 0;
            _to_cols = 0;
            _to_rows = 0;
        }

        public DuctNest(int cols, int rows)
        {
            _DuctMapping = new Dictionary<string, string>();
            _toDucts = new List<DuctCtrl>();
            _fromDucts = new List<DuctCtrl>();
            _from_cols = cols;
            _from_rows = rows;
            _to_cols = cols;
            _to_rows = rows;
            InitLayout(cols, rows, cols, rows);
        }

        public new event EventHandler DuctClick
        {
            add
            {
                foreach (DuctCtrl duct in _fromDucts)
                {
                    duct.Click += value;
                }
                foreach (DuctCtrl duct in _toDucts)
                {
                    duct.Click += value;
                }
            }
            remove
            {
                foreach (DuctCtrl duct in _fromDucts)
                {
                    duct.Click -= value;
                }
                foreach (DuctCtrl duct in _toDucts)
                {
                    duct.Click -= value;
                }
            }
        }

        public void InitLayout(int from_cols, int from_rows, int to_cols, int to_rows)
        {
            DuctCtrl duct = null;
            _from_cols = from_cols;
            _from_rows = from_rows;
            _to_cols = to_cols;
            _to_rows = to_rows;
            _toDucts.Clear();
            _fromDucts.Clear();

            for (int i = 0; i < _from_cols; i++)
            {
                for (int j = 0; j < _from_rows; j++)
                {
                    duct = new DuctCtrl(i, j, ConnectType.from);
                    duct.Top = 5 + j * (duct.Height + 2);
                    duct.Left = 5 + i * (duct.Width + 2);
                    _fromDucts.Add(duct);
                }
            }

            for (int i = 0; i < _to_cols; i++)
            {
                for (int j = 0; j < _to_rows; j++)
                {
                    duct = new DuctCtrl(i, j, ConnectType.to);
                    duct.Top = 5 + j * (duct.Height + 2);
                    duct.Left = 5 + i * (duct.Width + 2);
                    _toDucts.Add(duct);
                }
            }
        }

        public void InitLayout()
        {
            InitLayout(_from_cols, _from_rows, _to_cols, _to_rows);
        }
    }
}
