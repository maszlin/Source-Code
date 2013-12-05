using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;
using Intergraph.GTechnology.Diagnostics;

namespace NEPS.AssignPlantUnit
{
    public class GTAssignPlantUnit : Intergraph.GTechnology.Interfaces.IGTFunctional
    {
        GTDiagnostics m_Diag = new GTDiagnostics(GTDiagSS.IDotNetCustomCmd, GTDiagMaskWord.IDotNetCustomCmd, "GTFibJointAssignPlantUnit.cs"); 

        public const int CNO_FSplice = 11801;
       
        public void Execute()
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogEnter("Execute"); 

            if (components.Count == 0)
                return;

            IGTComponent oComp = components.GetComponent(CNO_FSplice);
            oComp.Recordset.MoveFirst();
            int fspliceFID = int.Parse(oComp.Recordset.Fields["G3E_FID"].Value.ToString());

            AssignmentForm form = new AssignmentForm();
            form.componentList = Components;
            form.fspliceFID = fspliceFID;
            form.dc = DataContext;
            form.ShowDialog();

            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogExit("Execute"); 

        }

        #region IGTFunctional Members

        GTArguments arguments = null;
        public GTArguments Arguments
        {
            get
            {
                return arguments;
            }
            set
            {
                arguments = value;
            }
        }

        string componentName = "";
        public string ComponentName
        {
            get
            {
                return componentName;
            }
            set
            {
                componentName = value;
            }
        }

        IGTComponents components = null;
        public Intergraph.GTechnology.API.IGTComponents Components
        {
            get
            {
                return components;
            }
            set
            {
                components = value;
            }
        }

        IGTDataContext dataContext = null;
        public Intergraph.GTechnology.API.IGTDataContext DataContext
        {
            get
            {
                return dataContext;
            }
            set
            {
                dataContext = value;
            }
        }

        public void Delete()
        {
           // MessageBox.Show("Delete.");
        }

        string fieldName = "";
        public string FieldName
        {
            get
            {
                return fieldName;
            }
            set
            {
                fieldName = value;
            }
        }

        IGTFieldValue fieldValueBeforeChange = null;
        public Intergraph.GTechnology.API.IGTFieldValue FieldValueBeforeChange
        {
            get
            {
                return fieldValueBeforeChange;
            }
            set
            {
                fieldValueBeforeChange = value;
            }
        }

        GTFunctionalTypeConstants typeConstant;
        public Intergraph.GTechnology.Interfaces.GTFunctionalTypeConstants Type
        {
            get
            {
                return typeConstant;
            }
            set
            {
                typeConstant = value;
            }
        }

        public void Validate(out string[] ErrorPriorityArray, out string[] ErrorMessageArray)
        {
            ErrorPriorityArray = new List<string>().ToArray();
            ErrorMessageArray = ErrorPriorityArray;
           // MessageBox.Show("Validate.");
        }

        #endregion
    }
}
