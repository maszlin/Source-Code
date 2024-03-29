using System;
using System.Windows.Forms;
using System.Drawing;


namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    class clsCheckedListBox : CheckedListBox
    {
        /// <summary>
        /// This is a CheckedListBox that allows the item's text color to be different for each of the 3 states of the corresponding checkbox's value.
        /// Like the base CheckedListBox control, you must handle setting of the indeterminate checkbox state yourself.
        /// Note also that this control doesn't allow highlighting of the selected item since that obscures the item's special text color which has the special meaning.  But 
        /// the selected item is still known to the user by the focus rectangle it will have surrounding it, like usual.
        /// </summary>
        public Color UncheckedColor;
        public Color CheckedColor;
        public Color IndeterminateColor;

        public int UncheckedCount;
        public int CheckedCount;
        public int IndeterminateCount;

        /// <summary>
        /// Parameterless Constructor
        /// </summary>
        public clsCheckedListBox()
        {
            UncheckedColor = Color.Blue;
            CheckedColor = Color.Purple;
            IndeterminateColor = Color.Orange;

            UncheckedCount = 0;
            CheckedCount = 0;
            IndeterminateCount = 0;
        }

        /// <summary>
        /// Constructor that allows setting of item colors when checkbox has one of 3 states.
        /// </summary>
        /// <param name="uncheckedColor">The text color of the items that are unchecked.</param>
        /// <param name="checkedColor">The text color of the items that are checked.</param>
        /// <param name="indeterminateColor">The text color of the items that are indeterminate.</param>
        public clsCheckedListBox(Color uncheckedColor, Color checkedColor, Color indeterminateColor)
        {
            UncheckedColor = uncheckedColor;
            CheckedColor = checkedColor;
            IndeterminateColor = indeterminateColor;
        }

        /// <summary>
        /// Overriden draw method that doesn't allow highlighting of the selected item since that obscures the item's text color which has desired meaning.  But the 
        /// selected item is still known to the user by the focus rectangle being displayed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (this.DesignMode)
            {
                base.OnDrawItem(e);
            }
            else
            {
                Color textColor = this.GetItemCheckState(e.Index) == CheckState.Unchecked ? UncheckedColor : (this.GetItemCheckState(e.Index) == CheckState.Checked ? CheckedColor : IndeterminateColor);

                DrawItemEventArgs e2 = new DrawItemEventArgs
                    (e.Graphics, e.Font,
                    new Rectangle(e.Bounds.Location, e.Bounds.Size), e.Index,
                    (e.State & DrawItemState.Focus) == DrawItemState.Focus ? DrawItemState.Focus : DrawItemState.None, /* Remove 'selected' state so that the base.OnDrawItem doesn't obliterate the work we are doing here. */
                    textColor, this.BackColor);

                base.OnDrawItem(e2);
            }
        }
    }
}
