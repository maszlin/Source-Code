using System.Collections;
using System.Windows.Forms;

public class clsTreeSorter : IComparer
{
    // Initialize the CaseInsensitiveComparer object
    private CaseInsensitiveComparer ObjectCompare;
    public clsTreeSorter()
    {
        ObjectCompare = new CaseInsensitiveComparer();
    }

    public int Compare(object x, object y)
    {
        TreeNode tx = (TreeNode)x;
        TreeNode ty = (TreeNode)y;
        
        int compareResult;
        //compareResult = ObjectCompare.Compare(tx.Tag.ToString(), ty.Tag.ToString());
        //return compareResult;
        compareResult = ObjectCompare.Compare(tx.Text, ty.Text);
        return compareResult;
    }
}

