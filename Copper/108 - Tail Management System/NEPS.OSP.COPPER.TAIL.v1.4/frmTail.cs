using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ADODB;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NEPS.OSP.COPPER.TAIL
{
    public partial class frmTail : Form
    {
        private IGTApplication m_gtapp = null;
        private IGTDataContext m_IGTDataContext = null;

        #region Variable to Manage TAIL
        // variable Globals
        private int initStep = 1;
        private double initX;
        private double initY;
        private string isAction = "CREATE";
        private double initDetailID;

        // variable for create tail
        private string v_EXC_ABB;
        private string v_MDF_NO;
        private string v_VERT_NO;

        // Result Step 1
        private int v_Joint_FID = 0;
        private string v_Joint_Cbl_Code = "";
        private double v_Joint_X;
        private double v_Joint_Y;

        // Result Step 2
        private int v_Vert_FID = 0;
        private int initPairPerBlock = 0;
        private int initTotalBlock = 0;
        private double v_FirstCord_Vert_X;
        private double v_FirstCord_Vert_Y;

        // Result Step 3
        private int v_Block_LO = 0;
        private double v_Block_LO_X;
        private double v_Block_LO_Y;

        // Result Step 4
        // txtLO

        // Result Step 5
        private int v_Block_HI = 0;
        private double v_Block_HI_X;
        private double v_Block_HI_Y;

        // Result Step 6
        // txtHI
        #endregion

        #region Variable for Temporary Geometry
        private List<IGTPoint> v_PointLine = new List<IGTPoint>();
        private List<IGTPoint> v_PointPoly = new List<IGTPoint>();
        private List<IGTPoint> v_PointText = new List<IGTPoint>();
        #endregion

        #region Form Event

        public frmTail()
        {
            try
            {
                InitializeComponent();
                this.FormClosing += new FormClosingEventHandler(this.frmTail_FormClosing);
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Tail Management System...");

                tabManage.SelectedIndex = 0;
                isAction = "CREATE";
                GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();
                Application.DoEvents();

                return;

                #region Tail Validation

                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount != 0)
                {
                    int initFNO = 0;
                    int initFID = 0;
                    Boolean initTail = false;
                    foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                    {
                        initFNO = oDDCKeyObject.FNO;
                        initFID = oDDCKeyObject.FID;
                        break;
                    }
                    // clear cahce
                    GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();
                    if (initFNO != 7000)
                    {
                        MessageBox.Show(this, "This application only for manage Tail", "Tail Management System");
                    }
                    else
                    {
                        ADODB.Recordset rsInfo = new ADODB.Recordset();
                        string strSQL = "SELECT CABLE_CLASS FROM GC_CBL WHERE G3E_FID = " + initFID;
                        rsInfo = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                        if (rsInfo.RecordCount > 0)
                        {
                            rsInfo.MoveFirst();
                            if (rsInfo.Fields[0].Value.ToString().ToUpper() == "TAIL")
                            {
                                initTail = true;
                            }
                        }
                        rsInfo = null;
                        strSQL = "";

                        if (initTail == false)
                        {
                            MessageBox.Show(this, "This application only for manage Tail", "Tail Management System");
                        }
                        else
                        {

                            tabManage.SelectedIndex = 1;
                            tabPageCreate.Dispose();
                            isAction = "MODIFY";
                            return;
                        }
                    }
                }


                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Running Tail Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmTail_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Tail Management System...");
        }

        private void frmTail_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Tail Management System...");
        }

        private void frmTail_Load(object sender, EventArgs e)
        {
            this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
        }

        private void frmTail_FormClosing(object sender, FormClosingEventArgs e)
        {
            //DialogResult retVal = MessageBox.Show("Are you sure to exit?", "Tail Management System", MessageBoxButtons.YesNo);
            //if (retVal == DialogResult.Yes)
            //{
            //    e.Cancel = false;
            //}
            //else
            //{
            //    e.Cancel = true;
            //}
        }

        #endregion

        #region General Function And Procedure

        public bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            int result;
            return int.TryParse(val, NumberStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        public double XPointGeom
        {
            set
            {
                initX = value;
            }
        }

        public double YPointGeom
        {
            set
            {
                initY = value;
            }
        }

        public int getInitStep
        {
            set
            {
                initStep = initStep + value;
            }
            get
            {
                return initStep;
            }
        }

        public string setLblPrcValue
        {
            set
            {
                lblProgress_7.Text = value;
            }
        }

        public List<IGTPoint> isPointPoly
        {
            get
            {
                return v_PointPoly;
            }
        }

        public List<IGTPoint> isPointLine
        {
            set
            {
                v_PointLine = value;
            }
        }

        #endregion

        public String manageTail
        {
            set
            {
                if (value != "0")
                {
                    //IGTGeometry isGeom = null;
                    int initFNO = 0;
                    int initCNO = 0;
                    int initFID = 0;
                    double initLoopY = 0;
                    double initLoopX = 0;

                    int initSelectedBlock = 0;
                    double initRangeSelected = 0;
                    double initRange = 0;
                    initDetailID = Convert.ToDouble(value.ToString());

                    if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                    {
                        return;
                    }
                    else
                    {
                        if (isAction == "CREATE")
                        {
                            if (initStep == 1)
                            {
                                #region Step 1 - Identify Main Joint
                                // Identify Main Joint
                                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                                {
                                    initFNO = oDDCKeyObject.FNO;
                                    if (initFNO == 10800)
                                    {
                                        v_Joint_FID = oDDCKeyObject.FID;
                                        v_Joint_X = oDDCKeyObject.Geometry.FirstPoint.X;
                                        v_Joint_Y = oDDCKeyObject.Geometry.FirstPoint.Y;
                                        v_Joint_Cbl_Code = oDDCKeyObject.Recordset.Fields["CABLE_CODE"].Value.ToString();
                                        txtCableCode.Text = v_Joint_Cbl_Code;
                                        v_EXC_ABB = oDDCKeyObject.Recordset.Fields["EXC_ABB"].Value.ToString();
                                        //break;
                                    }
                                }
                                // Clear Cahce
                                GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();

                                // go to Step 2
                                if (v_Joint_FID > 0)
                                {
                                    // next step
                                    initStep++;

                                    cmdStep_1.Text = "RESET";
                                    cmdStep_2.Enabled = true;

                                    lblState_1.Text = "PASSED";
                                    lblState_1.BackColor = System.Drawing.Color.Orange;
                                    lblState_1.ForeColor = System.Drawing.Color.Black;
                                    lblProgress_2.BackColor = System.Drawing.Color.Orange;
                                    lblProgress_2.ForeColor = System.Drawing.Color.Black;
                                }
                                else
                                {
                                    lblState_1.Text = "FAILED";
                                    lblState_1.BackColor = System.Drawing.Color.Red;
                                    lblState_1.ForeColor = System.Drawing.Color.Black;
                                }
                                #endregion
                            }
                            else if (initStep == 2)
                            {
                                #region Step 2 - Identify Vertical
                                // Identify Vertical
                                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                                {
                                    initFNO = oDDCKeyObject.FNO;
                                    if (initFNO == 13500)
                                    {
                                        initFID = oDDCKeyObject.FID;
                                        if (oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "13511")
                                        {
                                            v_FirstCord_Vert_X = oDDCKeyObject.Geometry.FirstPoint.X;
                                            v_FirstCord_Vert_Y = oDDCKeyObject.Geometry.FirstPoint.Y;
                                        }
                                        //break;
                                    }
                                }
                                // Clear Cahce
                                GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();

                                if (initFNO == 13500)
                                {
                                    // next step
                                    initStep++;
                                    v_Vert_FID = initFID;

                                    ADODB.Recordset rsVert = new ADODB.Recordset();
                                    string strSQL = "SELECT NUM_OF_BLKS, PAIRS_PER_BLK, MDF_NUM, VERT_NUM FROM GC_VERTICAL WHERE G3E_FID = " + v_Vert_FID;
                                    rsVert = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                                    if (rsVert.RecordCount > 0)
                                    {
                                        rsVert.MoveFirst();
                                        initTotalBlock = Convert.ToInt16(rsVert.Fields[0].Value.ToString());
                                        initPairPerBlock = Convert.ToInt16(rsVert.Fields[1].Value.ToString());
                                        v_MDF_NO = rsVert.Fields[2].Value.ToString();
                                        v_VERT_NO = rsVert.Fields[3].Value.ToString();
                                    }
                                    rsVert = null;
                                    strSQL = "";

                                    cmdStep_2.Text = "RESET";
                                    cmdStep_3.Enabled = true;

                                    lblState_2.Text = "PASSED";
                                    lblState_2.BackColor = System.Drawing.Color.Orange;
                                    lblState_2.ForeColor = System.Drawing.Color.Black;
                                    lblProgress_3.BackColor = System.Drawing.Color.Orange;
                                    lblProgress_3.ForeColor = System.Drawing.Color.Black;
                                }
                                else
                                {
                                    lblState_2.Text = "FAILED";
                                    lblState_2.BackColor = System.Drawing.Color.Red;
                                    lblState_2.ForeColor = System.Drawing.Color.Black;
                                }
                                #endregion
                            }
                            else if (initStep == 3)
                            {
                                #region Step 3 - Identify Low Vertical Block
                                // Identify Vertical Block
                                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                                {
                                    initFID = oDDCKeyObject.FID;
                                    if (initFID == v_Vert_FID)
                                    {
                                        initCNO = Convert.ToInt16(oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString());
                                        if (initCNO == 13533)
                                        {
                                            initLoopY = oDDCKeyObject.Geometry.FirstPoint.Y;
                                            initLoopX = oDDCKeyObject.Geometry.FirstPoint.X;

                                            initRange = initLoopY - initY;
                                            if (initRange < 0)
                                            {
                                                initRange = initRange * -1;
                                            }

                                            if (initSelectedBlock == 0)
                                            {
                                                initSelectedBlock = Convert.ToInt16(oDDCKeyObject.Recordset.Fields["G3E_CID"].Value.ToString());
                                                initRangeSelected = initRange;
                                            }
                                            else
                                            {
                                                if (initRangeSelected > initRange)
                                                {
                                                    initSelectedBlock = Convert.ToInt16(oDDCKeyObject.Recordset.Fields["G3E_CID"].Value.ToString());
                                                    initRangeSelected = initRange;
                                                }
                                            }
                                        }
                                    }
                                }
                                // Clear Cahce
                                GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();

                                if (initSelectedBlock > 0)
                                {
                                    // next step
                                    initStep++;
                                    v_Block_LO = initSelectedBlock;

                                    cmdStep_3.Text = "RESET";
                                    cmdStep_4.Enabled = true;
                                    txtLO.Enabled = true;
                                    int tempValue = ((v_Block_LO * initPairPerBlock) - initPairPerBlock) + 1;
                                    txtLO.Text = tempValue.ToString();
                                    v_Block_LO_X = initLoopX;
                                    v_Block_LO_Y = initLoopY;

                                    lblProgress_3.Text = "IDENTIFY LOW VERTICAL BLOCK (" + v_Block_LO + ")";
                                    lblState_3.Text = "PASSED";
                                    lblState_3.BackColor = System.Drawing.Color.Orange;
                                    lblState_3.ForeColor = System.Drawing.Color.Black;
                                    lblProgress_4.BackColor = System.Drawing.Color.Orange;
                                    lblProgress_4.ForeColor = System.Drawing.Color.Black;

                                    //initStep++;
                                    //cmdStep_4.Text = "RESET";
                                    //cmdStep_5.Enabled = true;
                                    //lblState_4.Text = "PASSED";
                                    //lblState_4.BackColor = System.Drawing.Color.Orange;
                                    //lblState_4.ForeColor = System.Drawing.Color.Black;
                                    //lblProgress_5.BackColor = System.Drawing.Color.Orange;
                                    //lblProgress_5.ForeColor = System.Drawing.Color.Black;
                                }
                                else
                                {
                                    lblState_3.Text = "FAILED";
                                    lblState_3.BackColor = System.Drawing.Color.Red;
                                    lblState_3.ForeColor = System.Drawing.Color.Black;
                                }
                                #endregion
                            }
                            else if (initStep == 4)
                            {
                                #region Step 4 - Manage LO PAIR
                                // handle by txtLO_KeyDown
                                #endregion
                            }
                            else if (initStep == 5)
                            {
                                #region Step 5 - Identify High Vertical Block
                                // Identify Vertical Block
                                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                                {
                                    initFID = oDDCKeyObject.FID;
                                    if (initFID == v_Vert_FID)
                                    {
                                        initCNO = Convert.ToInt16(oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString());
                                        if (initCNO == 13533)
                                        {
                                            initLoopY = oDDCKeyObject.Geometry.FirstPoint.Y;
                                            initLoopX = oDDCKeyObject.Geometry.FirstPoint.X;

                                            initRange = initLoopY - initY;
                                            if (initRange < 0)
                                            {
                                                initRange = initRange * -1;
                                            }

                                            if (initSelectedBlock == 0)
                                            {
                                                initSelectedBlock = Convert.ToInt16(oDDCKeyObject.Recordset.Fields["G3E_CID"].Value.ToString());
                                                initRangeSelected = initRange;
                                            }
                                            else
                                            {
                                                if (initRangeSelected > initRange)
                                                {
                                                    initSelectedBlock = Convert.ToInt16(oDDCKeyObject.Recordset.Fields["G3E_CID"].Value.ToString());
                                                    initRangeSelected = initRange;
                                                }
                                            }
                                        }
                                    }
                                }
                                // Clear Cahce
                                GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();

                                if (initSelectedBlock > 0 && initSelectedBlock >= v_Block_LO)
                                {
                                    // next step
                                    initStep++;
                                    v_Block_HI = initSelectedBlock;

                                    cmdStep_5.Text = "RESET";
                                    cmdStep_6.Enabled = true;
                                    txtHI.Enabled = true;
                                    int tempValue = (v_Block_HI * initPairPerBlock);
                                    if (initSelectedBlock == v_Block_LO)
                                    {
                                        txtHI.Text = txtLO.Text;
                                    }
                                    else
                                    {
                                        txtHI.Text = tempValue.ToString();
                                    }
                                    v_Block_HI_X = initLoopX;
                                    v_Block_HI_Y = initLoopY;

                                    lblProgress_5.Text = "IDENTIFY HIGH VERTICAL BLOCK (" + v_Block_HI + ")";
                                    lblState_5.Text = "PASSED";
                                    lblState_5.BackColor = System.Drawing.Color.Orange;
                                    lblState_5.ForeColor = System.Drawing.Color.Black;
                                    lblProgress_6.BackColor = System.Drawing.Color.Orange;
                                    lblProgress_6.ForeColor = System.Drawing.Color.Black;

                                    // next step
                                    //initStep++;

                                    //txtHI.Enabled = false;
                                    //cmdStep_6.Text = "RESET";
                                    //cmdStep_7.Enabled = true;
                                    //lblState_6.Text = "PASSED";
                                    //lblState_6.BackColor = System.Drawing.Color.Orange;
                                    //lblState_6.ForeColor = System.Drawing.Color.Black;
                                    //lblProgress_7.BackColor = System.Drawing.Color.Orange;
                                    //lblProgress_7.ForeColor = System.Drawing.Color.Black;
                                    //setPointPolyCbl();
                                }
                                else
                                {
                                    lblState_5.Text = "FAILED";
                                    lblState_5.BackColor = System.Drawing.Color.Red;
                                    lblState_5.ForeColor = System.Drawing.Color.Black;
                                }
                                #endregion
                            }
                            else if (initStep == 6)
                            {
                                #region Step 6 - Manage HI PAIR
                                // handle by txtHI_KeyDown
                                #endregion
                            }
                            else if (initStep == 7)
                            {
                                #region Step 7 - Create Tail
                                // handle by cmdStep_Done_Click
                                #endregion
                            }
                            else if (initStep == 8)
                            {
                                #region Step 8 - Create Line Tail
                                txtHI.Enabled = false;
                                cmdStep_7.Text = "RESET";
                                cmdStep_8.Enabled = true;
                                lblState_7.Text = "PASSED";
                                lblState_7.BackColor = System.Drawing.Color.Orange;
                                lblState_7.ForeColor = System.Drawing.Color.Black;
                                lblProgress_8.BackColor = System.Drawing.Color.Orange;
                                lblProgress_8.ForeColor = System.Drawing.Color.Black;
                                txtCableCode.Enabled = false; //!SharingMainJoint(v_Joint_FID);
                                txtCableCode.Text = v_Joint_Cbl_Code;
                                Application.DoEvents();
                                TestCableCode();
                                #endregion
                            }
                            else if (initStep == 9)
                            {
                                //Handle by Textbox Cable Code
                            }
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, "This application only working on detail of Exchange. Please try again.", "Tail Management System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        private void setPointPolyCbl()
        {
            IGTPoint oPntPoly0 = GTClassFactory.Create<IGTPoint>();
            oPntPoly0.X = v_FirstCord_Vert_X;
            oPntPoly0.Y = (v_FirstCord_Vert_Y - ((Convert.ToDouble(txtLO.Text) / (initPairPerBlock * initTotalBlock)) * 30)) + 30;
            v_PointPoly.Add(oPntPoly0);

            IGTPoint oPntPoly1 = GTClassFactory.Create<IGTPoint>();
            oPntPoly1.X = v_FirstCord_Vert_X;
            oPntPoly1.Y = (v_FirstCord_Vert_Y - ((Convert.ToDouble(txtHI.Text) / (initPairPerBlock * initTotalBlock)) * 30)) + 30;
            v_PointPoly.Add(oPntPoly1);

            IGTPoint oPntPoly2 = GTClassFactory.Create<IGTPoint>();
            oPntPoly2.X = v_FirstCord_Vert_X + 10;
            oPntPoly2.Y = (v_FirstCord_Vert_Y - ((Convert.ToDouble(txtHI.Text) / (initPairPerBlock * initTotalBlock)) * 30)) + 30;
            v_PointPoly.Add(oPntPoly2);

            IGTPoint oPntPoly3 = GTClassFactory.Create<IGTPoint>();
            oPntPoly3.X = v_FirstCord_Vert_X + 10;
            oPntPoly3.Y = (v_FirstCord_Vert_Y - ((Convert.ToDouble(txtLO.Text) / (initPairPerBlock * initTotalBlock)) * 30)) + 30;
            v_PointPoly.Add(oPntPoly3);
        }

        #region Command Create Tail
        void txtLO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                #region LO process
                if (isNumeric(txtLO.Text, System.Globalization.NumberStyles.Integer) == true)
                {
                    if ((Convert.ToInt16(txtLO.Text) <= (v_Block_LO * initPairPerBlock)) && (Convert.ToInt16(txtLO.Text) > ((v_Block_LO * initPairPerBlock) - initPairPerBlock)))
                    {
                        // next step
                        initStep++;

                        txtLO.Enabled = false;
                        cmdStep_4.Text = "RESET";
                        cmdStep_5.Enabled = true;
                        lblState_4.Text = "PASSED";
                        lblState_4.BackColor = System.Drawing.Color.Orange;
                        lblState_4.ForeColor = System.Drawing.Color.Black;
                        lblProgress_5.BackColor = System.Drawing.Color.Orange;
                        lblProgress_5.ForeColor = System.Drawing.Color.Black;
                    }
                    else
                    {
                        lblState_4.Text = "FAILED";
                        lblState_4.BackColor = System.Drawing.Color.Red;
                        lblState_4.ForeColor = System.Drawing.Color.Black;
                    }
                }
                #endregion
            }
        }

        void txtHI_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                #region HI process
                if (isNumeric(txtHI.Text, System.Globalization.NumberStyles.Integer) == true)
                {
                    if ((Convert.ToInt16(txtHI.Text) <= (v_Block_HI * initPairPerBlock)) && (Convert.ToInt16(txtHI.Text) > ((v_Block_HI * initPairPerBlock) - initPairPerBlock)) && (Convert.ToInt16(txtHI.Text) > Convert.ToInt16(txtLO.Text)))
                    {
                        // next step
                        initStep++;

                        txtHI.Enabled = false;
                        cmdStep_6.Text = "RESET";
                        cmdStep_7.Enabled = true;
                        lblState_6.Text = "PASSED";
                        lblState_6.BackColor = System.Drawing.Color.Orange;
                        lblState_6.ForeColor = System.Drawing.Color.Black;
                        lblProgress_7.BackColor = System.Drawing.Color.Orange;
                        lblProgress_7.ForeColor = System.Drawing.Color.Black;
                        setPointPolyCbl();
                    }
                    else
                    {
                        lblState_6.Text = "FAILED";
                        lblState_6.BackColor = System.Drawing.Color.Red;
                        lblState_6.ForeColor = System.Drawing.Color.Black;
                    }
                }
                #endregion
            }
        }

        private void txtCableCode_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            TestCableCode();
        }

        private void cmdStep_Done_Click(object sender, EventArgs e)
        {
            try
            {
                short iFNO = 7000;
                short iCNO;
                int iFID = 0;

                m_gtapp.BeginWaitCursor();
                Cursor.Current = Cursors.WaitCursor;

                #region Initial Coordinate for TAIL (POLYGON, LINE, TEXT)
                #region POLYGON
                IGTPolygonGeometry oCablePoly;
                oCablePoly = GTClassFactory.Create<IGTPolygonGeometry>();

                for (int i = 0; i < v_PointPoly.Count; i++)
                {
                    oCablePoly.Points.Add(v_PointPoly[i]);
                }

                oCablePoly.Points.Add(v_PointPoly[0]);
                #endregion

                #region LINE
                double initLength = 0;
                IGTPolylineGeometry oCableLine;
                oCableLine = GTClassFactory.Create<IGTPolylineGeometry>();

                for (int i = 0; i < v_PointLine.Count; i++)
                {
                    oCableLine.Points.Add(v_PointLine[i]);
                }

                initLength = 10;
                #endregion

                #region TEXT
                IGTTextPointGeometry oCableText;
                oCableText = GTClassFactory.Create<IGTTextPointGeometry>();

                IGTPoint oPntText = GTClassFactory.Create<IGTPoint>();
                oPntText.X = v_PointLine[2].X + 1;
                oPntText.Y = v_PointLine[2].Y + 1;

                oCableText.Origin = oPntText;
                oCableText.Rotation = 90;

                #endregion

                #region TEXT TL
                IGTTextPointGeometry oCableTextTL;
                oCableTextTL = GTClassFactory.Create<IGTTextPointGeometry>();

                IGTPoint oPntTextTL = GTClassFactory.Create<IGTPoint>();
                if (v_FirstCord_Vert_X < v_Joint_X)
                {
                    oPntTextTL.X = v_FirstCord_Vert_X + 10 + 1.8;
                    oPntTextTL.Y = v_FirstCord_Vert_Y - 30;
                }
                else
                {
                    oPntTextTL.X = v_FirstCord_Vert_X - 2.2;
                    oPntTextTL.Y = v_FirstCord_Vert_Y - 30;
                }

                oCableTextTL.Origin = oPntTextTL;
                oCableTextTL.Rotation = 90;
                #endregion

                #endregion

                #region Initial Support Information
                string v_EXC_ABB = "";
                double v_G3E_DETAILID = 0;
                string v_DETAIL_USERNAME = "";
                string v_VERT_NUM = "";
                string v_JOB_STATE = "";
                string v_FEATURE_STATE = "";
                string v_OWNERSHIP = "";
                ADODB.Recordset rsInfo = new ADODB.Recordset();
                string strSQL = "SELECT N.EXC_ABB, D.G3E_DETAILID, D.DETAIL_USERNAME, V.VERT_NUM, N.JOB_STATE, N.FEATURE_STATE, N.OWNERSHIP ";
                strSQL = strSQL + "FROM DGC_VERTICAL_T VT, GC_VERTICAL V, GC_DETAIL D, GC_NETELEM N ";
                strSQL = strSQL + "WHERE N.G3E_FNO = 6000 AND N.G3E_FID = D.G3E_FID AND D.G3E_DETAILID = VT.G3E_DETAILID AND VT.G3E_FID = V.G3E_FID AND V.G3E_FID = " + v_Vert_FID;
                rsInfo = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsInfo.RecordCount > 0)
                {
                    rsInfo.MoveFirst();
                    v_EXC_ABB = rsInfo.Fields[0].Value.ToString();
                    v_G3E_DETAILID = Convert.ToDouble(rsInfo.Fields[1].Value.ToString());
                    v_DETAIL_USERNAME = rsInfo.Fields[2].Value.ToString();
                    v_VERT_NUM = rsInfo.Fields[3].Value.ToString();
                    v_JOB_STATE = rsInfo.Fields[4].Value.ToString();
                    v_FEATURE_STATE = rsInfo.Fields[5].Value.ToString();
                    v_OWNERSHIP = rsInfo.Fields[6].Value.ToString();
                }
                rsInfo = null;
                strSQL = "";
                #endregion

                #region Initial Start Drawing
                GTTail.m_oIGTTransactionManager.Begin("DrawTail");
                IGTKeyObject oNewFeature;
                oNewFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(iFNO);
                iFID = oNewFeature.FID;
                #endregion

                #region Create Cable Netelem for Tail
                iCNO = 51;

                /*
                Name                 Null?    Type
                -------------------- -------- -------------
                G3E_ID               NOT NULL NUMBER(10)
                G3E_FNO              NOT NULL NUMBER(5)
                G3E_FID              NOT NULL NUMBER(10)
                G3E_CNO              NOT NULL NUMBER(5)
                G3E_CID              NOT NULL NUMBER(10)
                CREATED_BY                    VARCHAR2(30)
                CREATED_DATE                  DATE
                CREATED_HOST                  VARCHAR2(50)
                CREATED_IP_ADDRESS            VARCHAR2(30)
                CREATED_OS_USER               VARCHAR2(50)
                EXC_ABB                       VARCHAR2(10)
                FEATURE_STATE                 VARCHAR2(10)
                ID                            VARCHAR2(30)
                IMAP_FEATURE_ID               VARCHAR2(25)
                JOB_ID                        VARCHAR2(30)
                JOB_STATE                     VARCHAR2(10)
                MIC                           VARCHAR2(15)
                MIN_MATERIAL                  VARCHAR2(15)
                MODIFIED_BY                   VARCHAR2(30)
                MODIFIED_DATE                 DATE
                MODIFIED_HOST                 VARCHAR2(50)
                MODIFIED_IP_ADDRESS           VARCHAR2(30)
                MODIFIED_OS_USER              VARCHAR2(50)
                OWNERSHIP                     VARCHAR2(30)
                PLAN_ID                       VARCHAR2(25)
                SAP_WRK_ID                    VARCHAR2(30)
                SERVICE_CODE                  VARCHAR2(4)
                SWITCH_CENTRE_CLLI            VARCHAR2(25)
                YEAR_PLACED                   VARCHAR2(4)
                 */

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "PROPOSED");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "0");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("OWNERSHIP", v_OWNERSHIP);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "PROPOSED");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "0");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("OWNERSHIP", v_OWNERSHIP);
                }
                #endregion

                #region Create Cable Attribute for Tail
                iCNO = 7001;

                /*
                Name                        Null?    Type
                --------------------------- -------- --------------
                G3E_ID                      NOT NULL NUMBER(10)
                G3E_FNO                     NOT NULL NUMBER(5)
                G3E_FID                     NOT NULL NUMBER(10)
                G3E_CNO                     NOT NULL NUMBER(5)
                G3E_CID                     NOT NULL NUMBER(10)
                ALPHA_CODE                           VARCHAR2(20)
                ARMOUR                               VARCHAR2(20)
                CABLE_CODE                           VARCHAR2(3)
                CMP_NUMBER_OF_COAX_TUBES             NUMBER(4)
                COMPOSITION                          VARCHAR2(20)
                COPPER_SIZE                          NUMBER(38)
                COUNT_ANNOTATION                     VARCHAR2(2048)
                CREATED_BY                           VARCHAR2(30)
                CREATED_DATE                         DATE
                CREATED_HOST                         VARCHAR2(50)
                CREATED_IP_ADDRESS                   VARCHAR2(30)
                CREATED_OS_USER                      VARCHAR2(50)
                CTYPE                                NUMBER(22)
                CUSAGE                               NUMBER(22)
                DESIGN_TYPE                          VARCHAR2(16)
                DIAMETER                             NUMBER(10,2)
                EFFECTIVE_PAIRS                      NUMBER(22)
                FIBER_MODE                           VARCHAR2(8)
                FIBER_SIZE                           NUMBER(38)
                FIBER_TAG_ID                         VARCHAR2(24)
                G3E_PAIRCOUNTPREFIX                  NUMBER(1)
                GAUGE                                NUMBER(4)
                ID                                   VARCHAR2(50)
                MODIFIED_BY                          VARCHAR2(30)
                MODIFIED_DATE                        DATE
                MODIFIED_HOST                        VARCHAR2(50)
                MODIFIED_IP_ADDRESS                  VARCHAR2(30)
                MODIFIED_OS_USER                     VARCHAR2(50)
                NUMBER_OF_COAX_TUBES                 NUMBER(4)
                NUMBER_OF_VIDEO_PAIRS                NUMBER(4)
                ORIGINAL_USER                        VARCHAR2(20)
                OTHER_ID                             VARCHAR2(24)
                PERCENT_AERIAL                       NUMBER(3)
                PERCENT_BURIED                       NUMBER(3)
                PLACEMENT                            NUMBER(22)
                ROUTE_DETAIL                         VARCHAR2(4000)
                RT_CODE                              VARCHAR2(9)
                SHEATH                               VARCHAR2(35)
                STUB_LABEL                           CHAR(1)
                TERMINATION_ID                       NUMBER(22)
                TERMINATION_TYPE                     NUMBER(22)
                TEXT_FORMAT                          NUMBER(22)
                TOTAL_LENGTH                         NUMBER(38,10)
                TOTAL_SIZE                           NUMBER(38)
                USAGE                                VARCHAR2(8)
                */

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ALPHA_CODE", "COPPER");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CLASS", "TAIL");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CODE", txtCableCode.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMPOSITION", "COPPER");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COPPER_SIZE", (Convert.ToDouble(txtHI.Text) - Convert.ToDouble(txtLO.Text)) + 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DESIGN_TYPE", "N/A");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FIBER_MODE", "N/A");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FIBER_SIZE", 0);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GAUGE", 0.5);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ID", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM + "," + txtLO.Text + "-" + txtHI.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("STUB_LABEL", "N");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_SIZE", 100); //(Convert.ToDouble(txtHI.Text) - Convert.ToDouble(txtLO.Text)) + 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("USAGE", "EXCHANGE");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", initLength);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COUNT_ANNOTATION", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM + "," + txtLO.Text + "-" + txtHI.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SUB_TERMCODE", "***");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("NUMCABLES", (v_Block_HI - v_Block_LO) + 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MDF_NUM", v_MDF_NO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("VERT_NUM", v_VERT_NO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("LO_PR", txtLO.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("HI_PR", txtHI.Text);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ALPHA_CODE", "COPPER");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CLASS", "TAIL");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CODE", txtCableCode.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMPOSITION", "COPPER");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COPPER_SIZE", (Convert.ToDouble(txtHI.Text) - Convert.ToDouble(txtLO.Text)) + 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DESIGN_TYPE", "N/A");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FIBER_MODE", "N/A");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FIBER_SIZE", 0);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GAUGE", 0.5);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ID", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM + "," + txtLO.Text + "-" + txtHI.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("STUB_LABEL", "N");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_SIZE", 100); //(Convert.ToDouble(txtHI.Text) - Convert.ToDouble(txtLO.Text)) + 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("USAGE", "EXCHANGE");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", initLength);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COUNT_ANNOTATION", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM + "," + txtLO.Text + "-" + txtHI.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SUB_TERMCODE", "***");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("NUMCABLES", (v_Block_HI - v_Block_LO) + 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MDF_NUM", v_MDF_NO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("VERT_NUM", v_VERT_NO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("LO_PR", txtLO.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("HI_PR", txtHI.Text);
                }
                #endregion

                #region Create Cable Count for Tail
                iCNO = 55;
                /*
                Name                    Null?    Type
                ----------------------- -------- --------------
                G3E_ID                  NOT NULL NUMBER(10)
                G3E_FNO                 NOT NULL NUMBER(5)
                G3E_FID                 NOT NULL NUMBER(10)
                G3E_CNO                 NOT NULL NUMBER(5)
                G3E_CID                 NOT NULL NUMBER(10)
                CLASS                            VARCHAR2(50)
                COUNT_ANNOTATION                 VARCHAR2(2048)
                CREATED_DATE                     DATE
                CURRENT_DESIGNATION              VARCHAR2(128)
                CURRENT_FEED_DIR                 CHAR(1)
                CURRENT_HIGH                     NUMBER(38)
                CURRENT_LOW                      NUMBER(38)
                EHI_PR                           NUMBER(22)
                ELO_PR                           NUMBER(22)
                G3E_VALIDATE            NOT NULL NUMBER(1)
                HIGH_PORT                        NUMBER(38)
                ISP_FID                          NUMBER(38)
                ISP_FTYPE                        VARCHAR2(16)
                LOW_PORT                         NUMBER(38)
                MDF_UNIT                         NUMBER(22)
                PROPOSED_DESIGNATION             VARCHAR2(128)
                PROPOSED_FEED_DIR                CHAR(1)
                PROPOSED_HIGH                    NUMBER(38)
                PROPOSED_LOW                     NUMBER(38)
                RANGE_SEQ                        NUMBER(38)
                RIPPLE_WORKORDERS                VARCHAR2(255)
                SEQ                              NUMBER(38)
                VERTICAL_UNIT                    NUMBER(22)
                */

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CLASS", "Pair Count");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COUNT_ANNOTATION", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM + "," + txtLO.Text + "-" + txtHI.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CURRENT_DESIGNATION", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PROPOSED_DESIGNATION", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PROPOSED_FEED_DIR", "F");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PROPOSED_HIGH", txtHI.Text); //HIGH PAIR
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PROPOSED_LOW", txtLO.Text); //LOW PAIR
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CURRENT_FEED_DIR", "F");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CURRENT_HIGH", txtHI.Text); //HIGH PAIR
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CURRENT_LOW", txtLO.Text); //LOW PAIR
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SEQ", 1);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CLASS", "Pair Count");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COUNT_ANNOTATION", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM + "," + txtLO.Text + "-" + txtHI.Text);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CURRENT_DESIGNATION", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PROPOSED_DESIGNATION", v_EXC_ABB + "/" + v_DETAIL_USERNAME + "/" + v_VERT_NUM);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PROPOSED_FEED_DIR", "F");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PROPOSED_HIGH", txtHI.Text); //HIGH PAIR
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PROPOSED_LOW", txtLO.Text); //LOW PAIR
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CURRENT_FEED_DIR", "F");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CURRENT_HIGH", txtHI.Text); //HIGH PAIR
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CURRENT_LOW", txtLO.Text); //LOW PAIR
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SEQ", 1);
                }
                #endregion

                #region Create Cable Line for Tail
                iCNO = 7011;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", v_G3E_DETAILID);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oNewFeature.Components.GetComponent(iCNO).Geometry = oCableLine;
                #endregion

                #region Create Cable Polygon for Tail
                iCNO = 7015;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", v_G3E_DETAILID);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oNewFeature.Components.GetComponent(iCNO).Geometry = oCablePoly;
                #endregion

                #region Create Cable Text for Tail
                iCNO = 7031;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", v_G3E_DETAILID);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oNewFeature.Components.GetComponent(iCNO).Geometry = oCableText;
                #endregion

                #region Create Cable Text BL_T for Tail
                //iCNO = 7033;
                //if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", v_G3E_DETAILID);
                //}
                //else
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                //}

                //oNewFeature.Components.GetComponent(iCNO).Geometry = oCableTextTL;
                #endregion

                #region Create Relationship
                iCNO = 53;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("IN_FNO", 13500);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("IN_FID", v_Vert_FID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("OUT_FNO", 10800);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("OUT_FID", v_Joint_FID);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("IN_FNO", 13500);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("IN_FID", v_Vert_FID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("OUT_FNO", 10800);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("OUT_FID", v_Joint_FID);
                }
                #endregion

                #region Initial End Drawing & Refresh Metadata Changing
                GTTail.m_oIGTTransactionManager.Commit();
                GTTail.m_oIGTTransactionManager.RefreshDatabaseChanges();
                #endregion

                UpdateMainJoint(); // update main joint cable code if new

                #region Reset Step
                lblProgress_Done.Text = "PASSED";
                lblProgress_Done.BackColor = System.Drawing.Color.Orange;
                lblProgress_Done.ForeColor = System.Drawing.Color.Black;

                resetStep(8);
                resetStep(7);
                resetStep(6);
                resetStep(5);
                resetStep(4);
                resetStep(3);
                resetStep(2);
                resetStep(1);
                initStep = 1;
                #endregion

                #region Open Feature Explorer

                //this.Hide();
                Cursor.Current = Cursors.Default;
                m_gtapp.EndWaitCursor();
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                Application.DoEvents();
                GTTail.StartFeatureExplorer(oNewFeature);
                #endregion

            }
            catch (Exception ex)
            {
                GTTail.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(this, ex.Message, "Tail Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Reset Step
        private void resetStep(int isStep)
        {
            if (isStep == 1)
            #region reset step 1
            {
                // Result Step 1
                v_Joint_FID = 0;
                v_Joint_X = 0;
                v_Joint_Y = 0;

                cmdStep_1.Text = "STEP 1";
                cmdStep_2.Enabled = false;

                lblState_1.Text = "";
                lblState_1.BackColor = System.Drawing.SystemColors.Control;
                lblState_1.ForeColor = System.Drawing.SystemColors.GrayText;
                lblProgress_2.BackColor = System.Drawing.SystemColors.Control;
                lblProgress_2.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            #endregion
            else if (isStep == 2)
            #region reset step 2
            {
                // Result Step 2
                v_Vert_FID = 0;
                initPairPerBlock = 0;
                initTotalBlock = 0;
                v_FirstCord_Vert_X = 0;
                v_FirstCord_Vert_Y = 0;

                cmdStep_2.Text = "STEP 2";
                cmdStep_3.Enabled = false;

                lblState_2.Text = "";
                lblState_2.BackColor = System.Drawing.SystemColors.Control;
                lblState_2.ForeColor = System.Drawing.SystemColors.GrayText;
                lblProgress_3.BackColor = System.Drawing.SystemColors.Control;
                lblProgress_3.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            #endregion
            else if (isStep == 3)
            #region reset step 3
            {
                // Result Step 3
                v_Block_LO = 0;
                v_Block_LO_X = 0;
                v_Block_LO_Y = 0;

                txtLO.Enabled = false;
                txtLO.Text = "";

                cmdStep_3.Text = "STEP 3";
                cmdStep_4.Enabled = false;

                lblState_3.Text = "";
                lblState_3.BackColor = System.Drawing.SystemColors.Control;
                lblState_3.ForeColor = System.Drawing.SystemColors.GrayText;
                lblProgress_4.BackColor = System.Drawing.SystemColors.Control;
                lblProgress_4.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            #endregion
            else if (isStep == 4)
            #region reset step 4
            {
                // Result Step 4
                txtLO.Enabled = true;

                cmdStep_4.Text = "STEP 4";
                cmdStep_5.Enabled = false;

                lblState_4.Text = "";
                lblState_4.BackColor = System.Drawing.SystemColors.Control;
                lblState_4.ForeColor = System.Drawing.SystemColors.GrayText;
                lblProgress_5.BackColor = System.Drawing.SystemColors.Control;
                lblProgress_5.ForeColor = System.Drawing.SystemColors.GrayText;


            }
            #endregion
            else if (isStep == 5)
            #region reset step 5
            {
                txtHI.Text = "";
                txtHI.Enabled = false;

                // Result Step 5
                v_Block_HI = 0;
                v_Block_HI_X = 0;
                v_Block_HI_Y = 0;

                cmdStep_5.Text = "STEP 5";
                cmdStep_6.Enabled = false;

                lblState_5.Text = "";
                lblState_5.BackColor = System.Drawing.SystemColors.Control;
                lblState_5.ForeColor = System.Drawing.SystemColors.GrayText;
                lblProgress_6.BackColor = System.Drawing.SystemColors.Control;
                lblProgress_6.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            #endregion
            else if (isStep == 6)
            #region reset step 6
            {
                // Result Step 6
                txtHI.Enabled = true;

                cmdStep_6.Text = "STEP 6";
                cmdStep_7.Enabled = false;

                lblState_6.Text = "";
                lblState_6.BackColor = System.Drawing.SystemColors.Control;
                lblState_6.ForeColor = System.Drawing.SystemColors.GrayText;
                lblProgress_7.BackColor = System.Drawing.SystemColors.Control;
                lblProgress_7.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            #endregion
            else if (isStep == 7)
            #region reset step 7
            {
                // Result Step 6
                v_PointLine.Clear();
                v_PointPoly.Clear();
                v_PointText.Clear();

                txtHI.Enabled = true;
                txtCableCode.Enabled = false;
                txtCableCode.Text = "";

                cmdStep_7.Text = "STEP 7";
                cmdStep_8.Enabled = false;

                lblState_7.Text = "";
                lblState_7.BackColor = System.Drawing.SystemColors.Control;
                lblState_7.ForeColor = System.Drawing.SystemColors.GrayText;
                lblProgress_8.BackColor = System.Drawing.SystemColors.Control;
                lblProgress_8.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            #endregion
            else if (isStep == 8)
            #region reset step 8
            {
                txtCableCode.Enabled = true;
                cmdStep_8.Text = "STEP 8";
                cmdStep_Done.Enabled = false;

                lblProgress_8.BackColor = System.Drawing.SystemColors.Control;
                lblProgress_8.ForeColor = System.Drawing.SystemColors.GrayText;
                lblProgress_Done.BackColor = System.Drawing.SystemColors.Control;
                lblProgress_Done.ForeColor = System.Drawing.SystemColors.GrayText;

            }
            #endregion
        }

        private void cmdStep_1_Click(object sender, EventArgs e)
        {
            if (cmdStep_1.Text == "RESET")
            {
                resetStep(6);
                resetStep(5);
                resetStep(4);
                resetStep(3);
                resetStep(2);
                resetStep(1);
                initStep = 1;
            }
        }

        private void cmdStep_2_Click(object sender, EventArgs e)
        {
            if (cmdStep_2.Text == "RESET")
            {
                resetStep(6);
                resetStep(5);
                resetStep(4);
                resetStep(3);
                resetStep(2);
                initStep = 2;
            }
        }

        private void cmdStep_3_Click(object sender, EventArgs e)
        {
            if (cmdStep_3.Text == "RESET")
            {
                resetStep(6);
                resetStep(5);
                resetStep(4);
                resetStep(3);
                initStep = 3;
            }
        }

        private void cmdStep_4_Click(object sender, EventArgs e)
        {
            if (cmdStep_4.Text == "RESET")
            {
                resetStep(6);
                resetStep(5);
                resetStep(4);
                initStep = 4;
            }
        }

        private void cmdStep_5_Click(object sender, EventArgs e)
        {
            if (cmdStep_5.Text == "RESET")
            {
                resetStep(6);
                resetStep(5);
                initStep = 5;
            }
        }

        private void cmdStep_6_Click(object sender, EventArgs e)
        {
            if (cmdStep_6.Text == "RESET")
            {
                resetStep(6);
                initStep = 6;
            }
            else
            {
            }
        }

        private void cmdStep_7_Click(object sender, EventArgs e)
        {
            if (cmdStep_7.Text == "RESET")
            {
                resetStep(7);
                initStep = 7;
            }
        }

        #endregion

        #region edited by m.zam @ 26-08-2012
        private bool SharingMainJoint(int jFID)
        {
            // check if the main joint share with other tail
            ADODB.Recordset rsInfo = new ADODB.Recordset();
            string ssql = "SELECT G3E_FID FROM GC_NR_CONNECT WHERE OUT_FID = " + jFID;
            rsInfo = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rsInfo.RecordCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion

        #region CableCode by m.zam @ 04-10-2012

        private void TestCableCode()
        {
            txtCableCode.Text = txtCableCode.Text.Trim();
            bool isCableCodeOK = txtCableCode.Text.Length > 0;

            // make sure current cable code is not assigned to another MAIN JOINT
            string strCode = "SELECT A.G3E_FID FROM GC_SPLICE A, GC_NETELEM B WHERE A.G3E_FID <> " + v_Joint_FID;
            strCode += " AND A.CABLE_CODE = '" + txtCableCode.Text + "' AND UPPER(A.SPLICE_CLASS) = 'MAIN JOINT'";
            strCode += " AND A.G3E_FID = B.G3E_FID AND B.EXC_ABB = '" + v_EXC_ABB + "'";

            ADODB.Recordset rsCode = new ADODB.Recordset();
            rsCode = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strCode, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (!isCableCodeOK || rsCode.RecordCount > 0 && txtCableCode.Text != v_Joint_Cbl_Code)
            { // the query should not return any recordset else the cable code has also allocated to another MAIN JOINT.
                MessageBox.Show(this, "Cable code for selected main joint already used by others\r\nNew cable code will be assign to this tail and main joint");
                isCableCodeOK = false;
                txtCableCode.Text = GetNextCableCode();
                lblState_8.Text = "NEW";
                lblState_8.BackColor = System.Drawing.Color.Yellow;
                lblState_8.ForeColor = System.Drawing.Color.Black;
                cmdStep_8.Text = "RESET";
                Debug.WriteLine("FID : " + rsCode.Fields[0].Value.ToString());
                Application.DoEvents();
            }
            else
            {
                lblState_8.Text = "PASSED";
                lblState_8.BackColor = System.Drawing.Color.Orange;
                lblState_8.ForeColor = System.Drawing.Color.Black;
                cmdStep_8.Text = "RESET";
            }
            // next step
            initStep++;

            #region Step 9 - Provide Cable Code
            txtHI.Enabled = false;
            txtCableCode.Enabled = true;
            cmdStep_Done.Enabled = true;
            lblProgress_Done.BackColor = System.Drawing.Color.Orange;
            lblProgress_Done.ForeColor = System.Drawing.Color.Black;
            #endregion
            //            }
        }

        private string GetNextCableCode()
        {
            try
            {
                string cablecode = "";
                string ssql = "SELECT MAX(LPAD(UPPER(TRIM(CABLE_CODE)),7,' ')) FROM GC_SPLICE A, GC_NETELEM B ";
                ssql += "WHERE A.G3E_FID = B.G3E_FID AND UPPER(A.SPLICE_CLASS) = 'MAIN JOINT' ";
                ssql += "AND B.EXC_ABB = '" + v_EXC_ABB + "'";

                ADODB.Recordset rsCode = new ADODB.Recordset();
                rsCode = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsCode.RecordCount > 0)
                {
                    rsCode.MoveFirst();
                    string oldcode = rsCode.Fields[0].Value.ToString().Trim();

                    for (int i = oldcode.Length - 1; i > -1; i--)
                    {
                        char c = oldcode[i];
                        if (c == 'Z')
                        {
                            if (i == 0)
                            {
                                cablecode = "AA" + cablecode;
                                break;
                            }
                            else
                            {
                                cablecode = "A" + cablecode;
                            }
                        }
                        else
                        {
                            cablecode = oldcode.Remove(i) + (char)((int)c + 1) + cablecode;
                            break;
                        }
                    }
                    return cablecode;
                }
                else
                {
                    return "A";
                }

            }
            catch { return "A"; }

        }

        private void cmdStep_8_Click(object sender, EventArgs e)
        {
            if (cmdStep_8.Text == "STEP 8")
                TestCableCode();
            else
                resetStep(8);
        }

        private void txtCableCode_Leave(object sender, EventArgs e)
        {
            TestCableCode();
        }

        private void txtCableCode_EnabledChanged(object sender, EventArgs e)
        {
            // prevent txtCableCode to Enable if the main joint already assigned to other tail.
            if (SharingMainJoint(v_Joint_FID)) txtCableCode.Enabled = false;
        }

        private void UpdateMainJoint()
        {
            try
            {
                if (txtCableCode.Text != v_Joint_Cbl_Code)
                {
                    string ssql = "UPDATE GC_SPLICE SET CABLE_CODE = '" + txtCableCode.Text +
                    "' WHERE G3E_FID = " + v_Joint_FID.ToString();

                    ADODB.Recordset rsSQL = new ADODB.Recordset();
                    int iR;
                    GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                    GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                }
            }
            catch
            { }
        }
        #endregion

    }
}