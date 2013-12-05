using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Windows.Forms;

public class ComboItem
{
    private int id;
    private string text;

    public ComboItem(int ID, string Text)
    {
        this.id = ID;
        this.text = Text;
    }

    public int ID
    { get { return this.id; } }

    public string Text
    { get { return this.text; } }
}

class myCombo
{
    /// <summary>
    /// Add items from dataset into combobox
    /// items added as ComboItem (value + id)
    /// the combobox is set with autocomplete mode
    /// </summary>
    /// <param name="cmb">related combo box</param>
    /// <param name="ds">dataset result from database select statement</param>
    /// <param name="col_item">column name for the item</param>
    /// <param name="col_key">column name for the key</param>
    public static void AddItems(ComboBox cmb, ADODB.Recordset rs, string colID, string colVal1, string colVal2)
    {
        List<ComboItem> items = new List<ComboItem>();
        cmb.DataSource = null;
        cmb.Items.Clear();
        cmb.Text = "";
        try
        {
            if (rs != null)
            {
                rs.MoveFirst();
                do
                {
                    string txt = rs.Fields[colVal1].Value.ToString();
                   // if (colVal2.Length > 0) txt += " [" + rs.Fields[colVal2].Value.ToString() + "]";
                    items.Add(new ComboItem(int.Parse(rs.Fields[colID].Value.ToString()),        // id
                                         txt));   // value
                    rs.MoveNext();
                }
                while (!rs.EOF);
                rs = null;
            }
            cmb.ValueMember = "ID";
            cmb.DisplayMember = "Text";
            cmb.DataSource = items;
           // cmb.Sorted = true;
            if (cmb.Items.Count > 0) cmb.SelectedIndex = cmb.Items.Count - 1;

          //  cmb.AutoCompleteMode = AutoCompleteMode.Append;
          //  cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
        }
        catch
        {
        }
    }

    /// <summary>
    /// Insert item to combobox while keeping existing data
    /// This function is required as the combobox datasource is binded to comboitem list
    /// </summary>
    /// <param name="cmb">related ComboxBox</param>
    /// <param name="index">location to insert new item</param>
    /// <param name="item">the item to be inserted</param>
    public static void InsertItem(ComboBox cmb, int index, ComboItem item)
    {
        List<ComboItem> newlist = new List<ComboItem>();
        newlist = (List<ComboItem>)cmb.DataSource;
        newlist.Insert(0, item);
        // unbind current datasouce 
        cmb.DataSource = null;
        // bind new list to combobox
        cmb.ValueMember = "ID";
        cmb.DisplayMember = "Text";
        cmb.DataSource = newlist;

        cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        cmb.AutoCompleteSource = AutoCompleteSource.ListItems;

    }
}