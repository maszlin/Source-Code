using System;
using System.Collections.Generic;
using System.Text;

namespace NEPS.GTechnology.PUTrigger
{
    class Class1
    {
        //#region cases
        //private void btnOK_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (addMode == 95)
        //        {
        //            PlaceSymbol();
        //        }
        //        else if (addMode == 90)
        //        {
        //            valpufType.Text = cmbpufType.SelectedItem.ToString();
        //            cmbpufType.Visible = false;
        //            valpufType.Visible = true;
        //            AddPUTrigger();
        //        }
        //        else
        //        {
        //            switch (valPlantUnitFeature.Text)
        //            {
        //                case "IB_CON":
        //                case "IB_E":
        //                    {
        //                        valpufType.Text = null;
        //                        cmbpufType.Items.Clear();
        //                        cmbpufType.Items.Add(new cboItem("CIVIL", "CIVIL"));
        //                        cmbpufType.Items.Add(new cboItem("COPPER", "COPPER"));
        //                        cmbpufType.SelectedIndex = 0;
        //                        lblpufType.Visible = true;
        //                        cmbpufType.Visible = true;
        //                        addMode = 90;
        //                        break;
        //                    }
        //                case "IB_T":
        //                    {
        //                        valpufType.Text = null;
        //                        cmbpufType.Items.Clear();
        //                        cmbpufType.Items.Add(new cboItem("CIVIL", "CIVIL"));
        //                        cmbpufType.Items.Add(new cboItem("COPPER", "COPPER"));
        //                        cmbpufType.Items.Add(new cboItem("FIBRE", "FIBRE"));
        //                        cmbpufType.SelectedIndex = 0;
        //                        lblpufType.Visible = true;
        //                        cmbpufType.Visible = true;
        //                        addMode = 90;
        //                        break;
        //                    }
        //                case "LD_IN":
        //                case "PILE":
        //                case "ANCHOR_IRON":
        //                case "GLAV_LADDER_HOOK":
        //                case "SUMHOLE_GRATING":
        //                case "LOCKING_PIN":
        //                case "FOUNDATION_BLOT":
        //                case "CABLE_BEARER_NO:1":
        //                case "CABLE_BEARER_NO:2":
        //                case "CABLE_BRACKET_NO:8":
        //                case "CABLE_BRACKET_NO:12":
        //                case "CABLE_BRACKET_NO:18":
        //                case "CABLE_BRACKET_NO:24":
        //                case "MANHOLE_LADDER_GALV":
        //                case "PLAT_A":
        //                case "PLAT_B":
        //                case "TRANS_POLE 50KM":
        //                case "TRANS_POLE 100KM":
        //                case "TRANS_POLE 150KM":
        //                case "TRANS_POLE 200KM":
        //                case "TRANS_POLE 201KM":
        //                case "STENCIL":
        //                    {
        //                        valpufType.Text = "CIVIL/COPPER";
        //                        lblpufType.Visible = true;
        //                        valpufType.Visible = true;
        //                        AddPUTrigger();
        //                        break;
        //                    }
        //                case "OFCW":
        //                    {
        //                        valpufType.Text = "FIBRE";
        //                        lblpufType.Visible = true;
        //                        valpufType.Visible = true;
        //                        AddPUTrigger();
        //                        break;
        //                    }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}


        //private void AddPUTrigger()
        //{
        //    try
        //    {
        //        cmbPlantUnitFeature.Visible = false;
        //        lblPlantUnitFeature.Text = lblPlantUnitFeature.Text + " :";
        //        valPlantUnitFeature.Visible = true;
        //        m_gtapp.Application.SelectedObjects.Clear();

        //        switch (valPlantUnitFeature.Text)
        //        {
        //            case "LD_IN":
        //                {
        //                    procMsg = "Identify ASB/PAD manhole for lead-in";
        //                    break;
        //                }
        //            case "IB_CON":
        //            case "IB_E":
        //            case "IB_T":
        //                {
        //                    procMsg = "Identify pole/node for " + valPlantUnitFeature.Text;
        //                    break;
        //                }
        //            case "PILE":
        //                {
        //                    break;
        //                }
        //            case "ANCHOR_IRON":
        //            case "GLAV_LADDER_HOOK":
        //            case "SUMHOLE_GRATING":
        //            case "LOCKING_PIN":
        //            case "FOUNDATION_BLOT":
        //            case "CABLE_BEARER_NO:1":
        //            case "CABLE_BEARER_NO:2":
        //            case "CABLE_BRACKET_NO:8":
        //            case "CABLE_BRACKET_NO:12":
        //            case "CABLE_BRACKET_NO:18":
        //            case "CABLE_BRACKET_NO:24":
        //            case "MANHOLE_LADDER_GALV":
        //                {
        //                    procMsg = "Identify ASB manhole for " + valPlantUnitFeature.Text;
        //                    break;
        //                }
        //            case "PLAT_A":
        //                {
        //                    procMsg = "Identify ASB/PAD manhole for " + valPlantUnitFeature.Text;
        //                    break;
        //                }
        //            case "PLAT_B":
        //                {
        //                    procMsg = "Identify ASB/PAD manhole/chamber for " + valPlantUnitFeature.Text;
        //                    break;
        //                }
        //            case "STENCIL":
        //                {
        //                    procMsg = "Identify ASB DP/Cabinet for " + valPlantUnitFeature.Text;
        //                    break;
        //                }
        //            case "TRANS_POLE 50KM":
        //            case "TRANS_POLE 100KM":
        //            case "TRANS_POLE 150KM":
        //            case "TRANS_POLE 200KM":
        //            case "TRANS_POLE 201KM":
        //                {
        //                    procMsg = "Identify pole/node for " + valPlantUnitFeature.Text;
        //                    break;
        //                }
        //            case "OFCW":
        //                {
        //                    procMsg = "Identify manhole for " + valPlantUnitFeature.Text;
        //                    break;
        //                }
        //        }

        //        if (valPlantUnitFeature.Text != "PILE")
        //        {
        //            procMsg = procMsg + " or Press Esc to cancel";
        //            hideDialog();
        //            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, procMsg);
        //            m_gtapp.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
        //            addMode = 91;
        //        }
        //        else
        //        {
        //            PUTriggerState = "ASB";
        //            ShowAttributes();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        m_gtapp.EndWaitCursor();
        //        showDialog();
        //    }
        //}


        //private void CheckFeature1()
        //{
        //    try
        //    {
        //        bool isValid = false;
        //        PUTriggerState = "NOT AVAILABLE";

        //        switch (valPlantUnitFeature.Text)
        //        {
        //            case "LD_IN":
        //                {
        //                    //procMsg = "Identify ASB/PAD manhole for lead-in";
        //                    if (iFNO1 == 2700)
        //                    {
        //                        if ((CheckState(iFNO1, iFID1, "ASB") == true) || (CheckState(iFNO1, iFID1, "PAD") == true))
        //                        {
        //                            isValid = true;
        //                        }
        //                        PUTriggerState = srcState;
        //                    }
        //                    break;
        //                }
        //            case "IB_CON":
        //            case "IB_E":
        //            case "IB_T":
        //                {
        //                    switch (valpufType.Text)
        //                    {
        //                        case "CIVIL":
        //                            {
        //                                //procMsg = "Identify pole/node for " + valPlantUnitFeature.Text;
        //                                if ((iFNO1 == 3000) || (iFNO1 == 2800))
        //                                {
        //                                    isValid = true;
        //                                    PUTriggerState = GetState(iFNO1, iFID1);
        //                                }
        //                                break;
        //                            }
        //                        case "COPPER":
        //                            {
        //                                //procMsg = "Identify pole/node for " + valPlantUnitFeature.Text;
        //                                if ((iFNO1 == 3000) || (iFNO1 == 19000))
        //                                {
        //                                    isValid = true;
        //                                    PUTriggerState = GetState(iFNO1, iFID1);
        //                                }
        //                                break;
        //                            }
        //                        case "FIBRE":
        //                            {
        //                                //procMsg = "Identify pole/node for " + valPlantUnitFeature.Text;
        //                                if ((iFNO1 == 3000) || (iFNO1 == 4800))
        //                                {
        //                                    isValid = true;
        //                                    PUTriggerState = GetState(iFNO1, iFID1);
        //                                }
        //                                break;
        //                            }
        //                    }
        //                    break;
        //                }
        //            case "PILE":
        //                {
        //                    break;
        //                }
        //            case "ANCHOR_IRON":
        //            case "GLAV_LADDER_HOOK":
        //            case "SUMHOLE_GRATING":
        //            case "LOCKING_PIN":
        //            case "FOUNDATION_BLOT":
        //            case "CABLE_BEARER_NO:1":
        //            case "CABLE_BEARER_NO:2":
        //            case "CABLE_BRACKET_NO:8":
        //            case "CABLE_BRACKET_NO:12":
        //            case "CABLE_BRACKET_NO:18":
        //            case "CABLE_BRACKET_NO:24":
        //            case "MANHOLE_LADDER_GALV":
        //                {
        //                    //procMsg = "Identify ASB manhole for " + valPlantUnitFeature.Text;
        //                    if (iFNO1 == 2700)
        //                    {
        //                        if (CheckState(iFNO1, iFID1, "ASB") == true)
        //                        {
        //                            isValid = true;
        //                        }
        //                        PUTriggerState = srcState;
        //                    }
        //                    break;
        //                }
        //            case "PLAT_A":
        //                {
        //                    //procMsg = "Identify ASB/PAD manhole for " + valPlantUnitFeature.Text;
        //                    if (iFNO1 == 2700)
        //                    {
        //                        if ((CheckState(iFNO1, iFID1, "ASB") == true) || (CheckState(iFNO1, iFID1, "PAD") == true))
        //                        {
        //                            isValid = true;
        //                        }
        //                        PUTriggerState = srcState;
        //                    }
        //                    break;
        //                }
        //            case "PLAT_B":
        //                {
        //                    //procMsg = "Identify ASB/PAD manhole/chamber for " + valPlantUnitFeature.Text;
        //                    if ((iFNO1 == 2700) || (iFNO1 == 3800))
        //                    {
        //                        if ((CheckState(iFNO1, iFID1, "ASB") == true) || (CheckState(iFNO1, iFID1, "PAD") == true))
        //                        {
        //                            isValid = true;
        //                        }
        //                        PUTriggerState = srcState;
        //                    }
        //                    break;
        //                }
        //            case "STENCIL":
        //                {
        //                    //procMsg = "Identify ASB DP/Cabinet for " + valPlantUnitFeature.Text;
        //                    if ((iFNO1 == 13000) || (iFNO1 == 10300))
        //                    {
        //                        if ((CheckState(iFNO1, iFID1, "ASB") == true))
        //                        {
        //                            isValid = true;
        //                        }
        //                        PUTriggerState = srcState;
        //                    }
        //                    break;
        //                }
        //            case "TRANS_POLE 50KM":
        //            case "TRANS_POLE 100KM":
        //            case "TRANS_POLE 150KM":
        //            case "TRANS_POLE 200KM":
        //            case "TRANS_POLE 201KM":
        //                {
        //                    //procMsg = "Identify pole/node for " + valPlantUnitFeature.Text;
        //                    if ((iFNO1 == 3000) || (iFNO1 == 4800))
        //                    {
        //                        isValid = true;
        //                        PUTriggerState = GetState(iFNO1, iFID1);
        //                    }
        //                    break;
        //                }
        //            case "OFCW":
        //                {
        //                    //procMsg = "Identify manhole for " + valPlantUnitFeature.Text;
        //                    if (iFNO1 == 2700)
        //                    {
        //                        isValid = true;
        //                        PUTriggerState = GetState(iFNO1, iFID1);
        //                    }
        //                    break;
        //                }
        //        }


        //        if (isValid)
        //        {
        //            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to accept or Press Esc to reject");
        //            m_gtapp.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
        //            addMode = 92;
        //        }
        //        else
        //        {
        //            m_gtapp.Application.SelectedObjects.Clear();
        //            iFNO1 = 0;
        //            iFID1 = 0;
        //            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, procMsg);
        //            m_gtapp.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
        //            addMode = 91;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        m_gtapp.EndWaitCursor();
        //        showDialog();
        //    }
        //}


        //private void ProcFeature1()
        //{
        //    try
        //    {


        //        switch (valPlantUnitFeature.Text)
        //        {
        //            case "LD_IN":
        //                {
        //                    ShowAttributes();
        //                    break;
        //                }
        //            case "PILE":
        //                {
        //                    break;
        //                }
        //            case "ANCHOR_IRON":
        //            case "GLAV_LADDER_HOOK":
        //            case "SUMHOLE_GRATING":
        //            case "LOCKING_PIN":
        //            case "FOUNDATION_BLOT":
        //            case "CABLE_BEARER_NO:1":
        //            case "CABLE_BEARER_NO:2":
        //            case "CABLE_BRACKET_NO:8":
        //            case "CABLE_BRACKET_NO:12":
        //            case "CABLE_BRACKET_NO:18":
        //            case "CABLE_BRACKET_NO:24":
        //            case "MANHOLE_LADDER_GALV":
        //            case "PLAT_A":
        //            case "PLAT_B":
        //            case "STENCIL":
        //            case "TRANS_POLE 50KM":
        //            case "TRANS_POLE 100KM":
        //            case "TRANS_POLE 150KM":
        //            case "TRANS_POLE 200KM":
        //            case "TRANS_POLE 201KM":
        //                {
        //                    PlaceSymbol();
        //                    break;
        //                }

        //            case "IB_CON":
        //            case "IB_E":
        //            case "IB_T":
        //                {
        //                    procMsg = "Identify cable";
        //                    procMsg = procMsg + " or Press Esc to cancel";
        //                    hideDialog();
        //                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, procMsg);
        //                    m_gtapp.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
        //                    addMode = 93;
        //                    break;
        //                }
        //            case "OFCW":
        //                {
        //                    procMsg = "Identify fibre cable";
        //                    procMsg = procMsg + " or Press Esc to cancel";
        //                    hideDialog();
        //                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, procMsg);
        //                    m_gtapp.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
        //                    addMode = 93;
        //                    break;
        //                }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        m_gtapp.EndWaitCursor();
        //        showDialog();
        //    }
        //}





        //private void ShowAttributes()
        //{
        //    try
        //    {
        //        string sSql = null;
        //        Recordset rsComp = null;
        //        int recordsAffected = 0;

        //        showDialog();
        //        EnableAllButtons();

        //        m_gtapp.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;

        //        switch (valPlantUnitFeature.Text)
        //        {
        //            case "LD_IN":
        //                {
        //                    lblTitle.Text = "LEAD-IN PU-TRIGGER";
        //                    lblAttribute1.Text = "NUMBER OF WAYS";
        //                    lblAttribute2.Text = "BILLING RATE";

        //                    txtAttribute1.Text = "1";

        //                    sSql = "SELECT PL_VALUE FROM REF_COM_BILLRATE ORDER BY PL_NUM";
        //                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                    if (rsComp != null)
        //                    {
        //                        if (!rsComp.EOF)
        //                        {
        //                            cmbAttribute2.Items.Clear();
        //                            cmbAttribute2.Items.Add(new cboItem("***", "***"));

        //                            while (!rsComp.EOF)
        //                            {
        //                                cmbAttribute2.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                rsComp.MoveNext();
        //                            }
        //                        }
        //                    }

        //                    rsComp = null;
        //                    cmbAttribute2.SelectedIndex = 1;

        //                    lblTitle.Visible = true;
        //                    lblAttribute1.Visible = true;
        //                    lblAttribute2.Visible = true;
        //                    txtAttribute1.Visible = true;
        //                    cmbAttribute2.Visible = true;

        //                    addMode = 95;
        //                    break;
        //                }
        //            case "IB_CON":
        //                {
        //                    lblTitle.Text = "INTEGRAL BEARER CONNECT PU-TRIGGER";
        //                    lblAttribute1.Text = "IB WIRE SIZE";
        //                    lblAttribute2.Text = "BILLING RATE";

        //                    sSql = "SELECT PL_VALUE FROM REF_IB_WIRE_SIZE ORDER BY PL_NUM";
        //                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                    if (rsComp != null)
        //                    {
        //                        if (!rsComp.EOF)
        //                        {
        //                            cmbAttribute1.Items.Clear();

        //                            while (!rsComp.EOF)
        //                            {
        //                                cmbAttribute1.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                rsComp.MoveNext();
        //                            }
        //                        }
        //                    }

        //                    rsComp = null;
        //                    cmbAttribute1.SelectedIndex = 0;

        //                    sSql = "SELECT PL_VALUE FROM REF_COM_BILLRATE ORDER BY PL_NUM";
        //                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                    if (rsComp != null)
        //                    {
        //                        if (!rsComp.EOF)
        //                        {
        //                            cmbAttribute2.Items.Clear();
        //                            cmbAttribute2.Items.Add(new cboItem("***", "***"));

        //                            while (!rsComp.EOF)
        //                            {
        //                                cmbAttribute2.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                rsComp.MoveNext();
        //                            }
        //                        }
        //                    }

        //                    rsComp = null;
        //                    cmbAttribute2.SelectedIndex = 1;

        //                    lblTitle.Visible = true;
        //                    lblAttribute1.Visible = true;
        //                    lblAttribute2.Visible = true;
        //                    cmbAttribute1.Visible = true;
        //                    cmbAttribute2.Visible = true;

        //                    addMode = 95;
        //                    break;
        //                }
        //            case "IB_E":
        //                {
        //                    lblTitle.Text = "INTEGRAL BEARER EARTH PU-TRIGGER";
        //                    lblAttribute1.Text = "CONNECTION";
        //                    lblAttribute2.Text = "IB WIRE SIZE";
        //                    lblAttribute3.Text = "BILLING RATE";

        //                    sSql = "SELECT PL_VALUE FROM REF_IBE_CONNECTION ORDER BY PL_NUM";
        //                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                    if (rsComp != null)
        //                    {
        //                        if (!rsComp.EOF)
        //                        {
        //                            cmbAttribute1.Items.Clear();

        //                            while (!rsComp.EOF)
        //                            {
        //                                cmbAttribute1.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                rsComp.MoveNext();
        //                            }
        //                        }
        //                    }

        //                    rsComp = null;
        //                    cmbAttribute1.SelectedIndex = 0;

        //                    sSql = "SELECT PL_VALUE FROM REF_IB_WIRE_SIZE ORDER BY PL_NUM";
        //                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                    if (rsComp != null)
        //                    {
        //                        if (!rsComp.EOF)
        //                        {
        //                            cmbAttribute2.Items.Clear();

        //                            while (!rsComp.EOF)
        //                            {
        //                                cmbAttribute2.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                rsComp.MoveNext();
        //                            }
        //                        }
        //                    }

        //                    rsComp = null;
        //                    cmbAttribute2.SelectedIndex = 0;

        //                    sSql = "SELECT PL_VALUE FROM REF_COM_BILLRATE ORDER BY PL_NUM";
        //                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                    if (rsComp != null)
        //                    {
        //                        if (!rsComp.EOF)
        //                        {
        //                            cmbAttribute3.Items.Clear();
        //                            cmbAttribute3.Items.Add(new cboItem("***", "***"));

        //                            while (!rsComp.EOF)
        //                            {
        //                                cmbAttribute3.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                rsComp.MoveNext();
        //                            }
        //                        }
        //                    }

        //                    rsComp = null;
        //                    cmbAttribute3.SelectedIndex = 1;

        //                    lblTitle.Visible = true;
        //                    lblAttribute1.Visible = true;
        //                    lblAttribute2.Visible = true;
        //                    lblAttribute3.Visible = true;
        //                    cmbAttribute1.Visible = true;
        //                    cmbAttribute2.Visible = true;
        //                    cmbAttribute3.Visible = true;

        //                    addMode = 95;
        //                    break;
        //                }
        //            case "IB_T":
        //                {
        //                    switch (valpufType.Text)
        //                    {
        //                        case "CIVIL":
        //                        case "COPPER":
        //                            {
        //                                lblTitle.Text = "INTEGRAL BEARER TERMINATE PU-TRIGGER";
        //                                lblAttribute1.Text = "TERMINATION TYPE";
        //                                lblAttribute2.Text = "LOCATION OR POLE TYPE";
        //                                lblAttribute3.Text = "CABLE SIZE";
        //                                lblAttribute4.Text = "CABLE GAUAGE";
        //                                lblAttribute5.Text = "BILLING RATE";

        //                                sSql = "SELECT PL_VALUE FROM REF_IBT_TYPE WHERE PL_TYPE LIKE '%CIVIL/COPPER%' ORDER BY PL_NUM";
        //                                rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                if (rsComp != null)
        //                                {
        //                                    if (!rsComp.EOF)
        //                                    {
        //                                        cmbAttribute1.Items.Clear();

        //                                        while (!rsComp.EOF)
        //                                        {
        //                                            cmbAttribute1.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                            rsComp.MoveNext();
        //                                        }
        //                                    }
        //                                }

        //                                rsComp = null;
        //                                cmbAttribute1.SelectedIndex = 0;

        //                                sSql = "SELECT PL_VALUE FROM REF_IBT_LOCATION ORDER BY PL_NUM";
        //                                rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                if (rsComp != null)
        //                                {
        //                                    if (!rsComp.EOF)
        //                                    {
        //                                        cmbAttribute2.Items.Clear();

        //                                        while (!rsComp.EOF)
        //                                        {
        //                                            cmbAttribute2.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                            rsComp.MoveNext();
        //                                        }
        //                                    }
        //                                }

        //                                rsComp = null;
        //                                cmbAttribute2.SelectedIndex = 0;

        //                                sSql = "SELECT PL_VALUE FROM REF_IBT_CABLE_SIZE ORDER BY PL_NUM";
        //                                rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                if (rsComp != null)
        //                                {
        //                                    if (!rsComp.EOF)
        //                                    {
        //                                        cmbAttribute3.Items.Clear();

        //                                        while (!rsComp.EOF)
        //                                        {
        //                                            cmbAttribute3.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                            rsComp.MoveNext();
        //                                        }
        //                                    }
        //                                }

        //                                rsComp = null;
        //                                cmbAttribute3.SelectedIndex = 0;

        //                                sSql = "SELECT PL_VALUE FROM REF_IBT_CABLE_GAUGE ORDER BY PL_NUM";
        //                                rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                if (rsComp != null)
        //                                {
        //                                    if (!rsComp.EOF)
        //                                    {
        //                                        cmbAttribute4.Items.Clear();

        //                                        while (!rsComp.EOF)
        //                                        {
        //                                            cmbAttribute4.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                            rsComp.MoveNext();
        //                                        }
        //                                    }
        //                                }

        //                                rsComp = null;
        //                                cmbAttribute4.SelectedIndex = 0;

        //                                sSql = "SELECT PL_VALUE FROM REF_COM_BILLRATE ORDER BY PL_NUM";
        //                                rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                if (rsComp != null)
        //                                {
        //                                    if (!rsComp.EOF)
        //                                    {
        //                                        cmbAttribute5.Items.Clear();
        //                                        cmbAttribute5.Items.Add(new cboItem("***", "***"));

        //                                        while (!rsComp.EOF)
        //                                        {
        //                                            cmbAttribute5.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                            rsComp.MoveNext();
        //                                        }
        //                                    }
        //                                }

        //                                rsComp = null;
        //                                cmbAttribute5.SelectedIndex = 1;

        //                                lblTitle.Visible = true;
        //                                lblAttribute1.Visible = true;
        //                                lblAttribute2.Visible = true;
        //                                lblAttribute3.Visible = true;
        //                                lblAttribute4.Visible = true;
        //                                lblAttribute5.Visible = true;
        //                                cmbAttribute1.Visible = true;
        //                                cmbAttribute2.Visible = true;
        //                                cmbAttribute3.Visible = true;
        //                                cmbAttribute4.Visible = true;
        //                                cmbAttribute5.Visible = true;

        //                                addMode = 95;
        //                                break;
        //                            }
        //                        case "FIBRE":
        //                            {
        //                                lblTitle.Text = "INTEGRAL BEARER TERMINATE PU-TRIGGER";
        //                                lblAttribute1.Text = "TERMINATION TYPE";
        //                                lblAttribute2.Text = "LOCATION OR POLE TYPE";
        //                                lblAttribute3.Text = "ONLY FOR POLE - POLE ATTACHMENT";
        //                                lblAttribute4.Text = "CABLE SIZE";
        //                                lblAttribute5.Text = "BILLING RATE";


        //                                sSql = "SELECT PL_VALUE FROM REF_IBT_TYPE WHERE PL_TYPE LIKE '%FIBRE%' ORDER BY PL_NUM";
        //                                rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                if (rsComp != null)
        //                                {
        //                                    if (!rsComp.EOF)
        //                                    {
        //                                        cmbAttribute1.Items.Clear();

        //                                        while (!rsComp.EOF)
        //                                        {
        //                                            cmbAttribute1.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                            rsComp.MoveNext();
        //                                        }
        //                                    }
        //                                }

        //                                rsComp = null;
        //                                cmbAttribute1.SelectedIndex = 0;

        //                                valAttribute2.Text = "-";

        //                                if (iFNO1 == 3000)
        //                                {
        //                                    sSql = "SELECT POLE_TYPE FROM GC_POLE WHERE G3E_FID = " + iFNO1.ToString();

        //                                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                    if (rsComp != null)
        //                                    {
        //                                        if (!rsComp.EOF)
        //                                        {
        //                                            switch (rsComp.Fields["POLE_TYPE"].Value.ToString())
        //                                            {
        //                                                case "CONCRETE":
        //                                                    {
        //                                                        valAttribute2.Text = "C/P";
        //                                                        break;
        //                                                    }
        //                                                case "BESI":
        //                                                    {
        //                                                        valAttribute2.Text = "I/P";
        //                                                        break;
        //                                                    }
        //                                                case "KAYU":
        //                                                    {
        //                                                        valAttribute2.Text = "W/P";
        //                                                        break;
        //                                                    }
        //                                                default:
        //                                                    {
        //                                                        valAttribute2.Text = "WALL";
        //                                                        break;
        //                                                    }
        //                                            }
        //                                        }
        //                                    }

        //                                    rsComp = null;

        //                                    sSql = "SELECT PL_VALUE FROM REF_POLE_ATTACHMENT_TYPE ORDER BY PL_NUM";
        //                                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                    if (rsComp != null)
        //                                    {
        //                                        if (!rsComp.EOF)
        //                                        {
        //                                            cmbAttribute3.Items.Clear();

        //                                            while (!rsComp.EOF)
        //                                            {
        //                                                cmbAttribute3.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                                rsComp.MoveNext();
        //                                            }
        //                                        }
        //                                    }

        //                                    rsComp = null;
        //                                    cmbAttribute3.SelectedIndex = 0;
        //                                }
        //                                else
        //                                {
        //                                    valAttribute3.Text = "-";
        //                                }

        //                                sSql = "SELECT PL_VALUE FROM REF_IBT_CABLE_SIZE ORDER BY PL_NUM";
        //                                rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                if (rsComp != null)
        //                                {
        //                                    if (!rsComp.EOF)
        //                                    {
        //                                        cmbAttribute4.Items.Clear();

        //                                        while (!rsComp.EOF)
        //                                        {
        //                                            cmbAttribute4.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                            rsComp.MoveNext();
        //                                        }
        //                                    }
        //                                }

        //                                rsComp = null;
        //                                cmbAttribute4.SelectedIndex = 0;

        //                                sSql = "SELECT PL_VALUE FROM REF_COM_BILLRATE ORDER BY PL_NUM";

        //                                rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                                if (rsComp != null)
        //                                {
        //                                    if (!rsComp.EOF)
        //                                    {
        //                                        cmbAttribute5.Items.Clear();
        //                                        cmbAttribute5.Items.Add(new cboItem("***", "***"));

        //                                        while (!rsComp.EOF)
        //                                        {
        //                                            cmbAttribute5.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                            rsComp.MoveNext();
        //                                        }
        //                                    }
        //                                }

        //                                rsComp = null;
        //                                cmbAttribute5.SelectedIndex = 1;

        //                                lblTitle.Visible = true;
        //                                lblAttribute1.Visible = true;
        //                                lblAttribute2.Visible = true;
        //                                lblAttribute3.Visible = true;
        //                                lblAttribute4.Visible = true;
        //                                lblAttribute5.Visible = true;
        //                                cmbAttribute1.Visible = true;
        //                                valAttribute2.Visible = true;

        //                                if (iFNO1 == 3000)
        //                                {
        //                                    cmbAttribute3.Visible = true;
        //                                }
        //                                else
        //                                {
        //                                    valAttribute3.Visible = true;
        //                                }
        //                                cmbAttribute4.Visible = true;
        //                                cmbAttribute5.Visible = true;

        //                                addMode = 95;
        //                                break;
        //                            }
        //                    }

        //                    break;
        //                }
        //            case "PILE":
        //                {
        //                    lblTitle.Text = "PILING PU-TRIGGER";
        //                    lblAttribute1.Text = "TYPE";
        //                    lblAttribute2.Text = "SIZE";
        //                    lblAttribute3.Text = "NO. OF NORMAL PILING";
        //                    lblAttribute4.Text = "NO. OF EXTRA PILING";
        //                    lblAttribute5.Text = "BILLING RATE";

        //                    txtAttribute3.Text = "1";
        //                    txtAttribute4.Text = "0";

        //                    sSql = "SELECT PL_VALUE FROM REF_PILE_TYPE ORDER BY PL_NUM";

        //                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                    if (rsComp != null)
        //                    {
        //                        if (!rsComp.EOF)
        //                        {
        //                            cmbAttribute1.Items.Clear();

        //                            while (!rsComp.EOF)
        //                            {
        //                                cmbAttribute1.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                rsComp.MoveNext();
        //                            }
        //                        }
        //                    }

        //                    rsComp = null;
        //                    cmbAttribute1.SelectedIndex = 0;

        //                    sSql = "SELECT PL_VALUE FROM REF_PILE_SIZE ORDER BY PL_NUM";

        //                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                    if (rsComp != null)
        //                    {
        //                        if (!rsComp.EOF)
        //                        {
        //                            cmbAttribute2.Items.Clear();

        //                            while (!rsComp.EOF)
        //                            {
        //                                cmbAttribute2.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                rsComp.MoveNext();
        //                            }
        //                        }
        //                    }

        //                    rsComp = null;
        //                    cmbAttribute2.SelectedIndex = 0;

        //                    sSql = "SELECT PL_VALUE FROM REF_COM_BILLRATE ORDER BY PL_NUM";

        //                    rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //                    if (rsComp != null)
        //                    {
        //                        if (!rsComp.EOF)
        //                        {
        //                            cmbAttribute5.Items.Clear();
        //                            cmbAttribute5.Items.Add(new cboItem("***", "***"));

        //                            while (!rsComp.EOF)
        //                            {
        //                                cmbAttribute5.Items.Add(new cboItem(rsComp.Fields["PL_VALUE"].Value.ToString(), rsComp.Fields["PL_VALUE"].Value.ToString()));
        //                                rsComp.MoveNext();
        //                            }
        //                        }
        //                    }

        //                    rsComp = null;
        //                    cmbAttribute5.SelectedIndex = 1;

        //                    lblTitle.Visible = true;
        //                    lblAttribute1.Visible = true;
        //                    lblAttribute2.Visible = true;
        //                    lblAttribute3.Visible = true;
        //                    lblAttribute4.Visible = true;
        //                    lblAttribute5.Visible = true;
        //                    cmbAttribute1.Visible = true;
        //                    cmbAttribute2.Visible = true;
        //                    txtAttribute3.Visible = true;
        //                    txtAttribute4.Visible = true;
        //                    cmbAttribute5.Visible = true;

        //                    addMode = 95;
        //                    break;
        //                }
        //            case "ANCHOR_IRON":
        //            case "GLAV_LADDER_HOOK":
        //            case "SUMHOLE_GRATING":
        //            case "LOCKING_PIN":
        //            case "FOUNDATION_BLOT":
        //            case "CABLE_BEARER_NO:1":
        //            case "CABLE_BEARER_NO:2":
        //            case "CABLE_BRACKET_NO:8":
        //            case "CABLE_BRACKET_NO:12":
        //            case "CABLE_BRACKET_NO:18":
        //            case "CABLE_BRACKET_NO:24":
        //            case "MANHOLE_LADDER_GALV":
        //            case "PLAT_A":
        //            case "PLAT_B":
        //            case "TRANS_POLE 50KM":
        //            case "TRANS_POLE 100KM":
        //            case "TRANS_POLE 150KM":
        //            case "TRANS_POLE 200KM":
        //            case "TRANS_POLE 201KM":
        //            case "STENCIL":
        //            case "OFCW":
        //                {
        //                    break;
        //                }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        m_gtapp.EndWaitCursor();
        //        showDialog();
        //    }
        //}


        //private void PlaceSymbol()
        //{
        //    try
        //    {
        //        hideDialog();
        //        //MessageBox.Show("PLACE SYMBOL");
        //        m_gtapp.Application.SelectedObjects.Clear();

        //        switch (valPlantUnitFeature.Text)
        //        {
        //            case "LD_IN":
        //            case "ANCHOR_IRON":
        //            case "GLAV_LADDER_HOOK":
        //            case "SUMHOLE_GRATING":
        //            case "LOCKING_PIN":
        //            case "FOUNDATION_BLOT":
        //            case "CABLE_BEARER_NO:1":
        //            case "CABLE_BEARER_NO:2":
        //            case "CABLE_BRACKET_NO:8":
        //            case "CABLE_BRACKET_NO:12":
        //            case "CABLE_BRACKET_NO:18":
        //            case "CABLE_BRACKET_NO:24":
        //            case "MANHOLE_LADDER_GALV":
        //            case "OFCW":
        //                {
        //                    procMsg = "Place near manhole";
        //                    break;
        //                }
        //            case "IB_CON":
        //            case "IB_E":
        //            case "IB_T":
        //            case "TRANS_POLE 50KM":
        //            case "TRANS_POLE 100KM":
        //            case "TRANS_POLE 150KM":
        //            case "TRANS_POLE 200KM":
        //            case "TRANS_POLE 201KM":
        //                {
        //                    procMsg = "Place near pole/node";
        //                    break;
        //                }
        //            case "PILE":
        //                {
        //                    procMsg = "Place near ductPath/manhole/cabinet";
        //                    break;
        //                }
        //            case "PLAT_A":
        //            case "PLAT_B":
        //                {
        //                    procMsg = "Place near manhole/chamber";
        //                    break;
        //                }
        //            case "STENCIL":
        //                {
        //                    procMsg = "TBD";
        //                    break;
        //                }
        //        }


        //        procMsg = procMsg + " or Press Esc to cancel";
        //        hideDialog();
        //        SymbolPoints.Clear();
        //        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, procMsg);
        //        m_gtapp.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
        //        addMode = 97;

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        m_gtapp.EndWaitCursor();
        //        showDialog();
        //    }
        //}



        //private void ProcFeature2()
        //{
        //    try
        //    {
        //        switch (valPlantUnitFeature.Text)
        //        {
        //            case "LD_IN":
        //            case "PILE":
        //            case "ANCHOR_IRON":
        //            case "GLAV_LADDER_HOOK":
        //            case "SUMHOLE_GRATING":
        //            case "LOCKING_PIN":
        //            case "FOUNDATION_BLOT":
        //            case "CABLE_BEARER_NO:1":
        //            case "CABLE_BEARER_NO:2":
        //            case "CABLE_BRACKET_NO:8":
        //            case "CABLE_BRACKET_NO:12":
        //            case "CABLE_BRACKET_NO:18":
        //            case "CABLE_BRACKET_NO:24":
        //            case "MANHOLE_LADDER_GALV":
        //            case "PLAT_A":
        //            case "PLAT_B":
        //            case "STENCIL":
        //            case "TRANS_POLE 50KM":
        //            case "TRANS_POLE 100KM":
        //            case "TRANS_POLE 150KM":
        //            case "TRANS_POLE 200KM":
        //            case "TRANS_POLE 201KM":
        //                {
        //                    break;
        //                }
        //            case "IB_CON":
        //            case "IB_E":
        //            case "IB_T":
        //                {
        //                    ShowAttributes();
        //                    break;
        //                }
        //            case "OFCW":
        //                {
        //                    PlaceSymbol();
        //                    break;
        //                }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        m_gtapp.EndWaitCursor();
        //        showDialog();
        //    }
        //}
        //#endregion
    }
}
