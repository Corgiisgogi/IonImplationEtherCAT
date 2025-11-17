using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IonImplationEtherCAT
{
    public partial class RecipeView : UserControl
    {
        int currentRecipe = 1;
        List<object> currentPm1RecipeList = new List<object>() { null, null, "", "", "", "", "" };
        List<object> currentPm2RecipeList = new List<object>() { null, null, "", "", "", "", "" };
        List<object> currentPm3RecipeList = new List<object>() { "", "", "" };

        public RecipeView()
        {
            InitializeComponent();
            this.ActivateToggle(false);
        }

        public void ActivateToggle(bool activation)
        {
            panelPm1.Enabled = activation;
            panelPm2.Enabled = activation;
            panelPm3.Enabled = activation;
            btnRecipePm1.Enabled = activation;
            btnRecipePm2.Enabled = activation;
            btnRecipePm3.Enabled = activation;
            btnLoad.Enabled = activation;
            btnSave.Enabled = activation;
        }

        public void RecipeApply()
        {
            switch (currentRecipe)
            {
                case 1:
                    this.panelPm1.Visible = true;
                    this.panelPm2.Visible = false;
                    this.panelPm3.Visible = false;
                    break;

                case 2:
                    this.panelPm1.Visible = false;
                    this.panelPm2.Visible = true;
                    this.panelPm3.Visible = false;
                    break;

                case 3:
                    this.panelPm1.Visible = false;
                    this.panelPm2.Visible = false;
                    this.panelPm3.Visible = true;
                    break;
            }
        }

        private void btnRecipePm1_Click(object sender, EventArgs e)
        {
            currentRecipe = 1;
            RecipeApply();
        }

        private void btnRecipePm2_Click(object sender, EventArgs e)
        {
            currentRecipe = 2;
            RecipeApply();
        }

        private void btnRecipePm3_Click(object sender, EventArgs e)
        {
            currentRecipe = 3;
            RecipeApply();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            switch (currentRecipe)
            {
                case 1:
                    if (currentPm1RecipeList[0] != null) { currentPm1RecipeList[0] = listBoxIonGas.SelectedItem; }
                    if (currentPm1RecipeList[1] != null) { currentPm1RecipeList[1] = listBoxGas.SelectedItem; }
                    currentPm1RecipeList[2] = textBoxDose.Text;
                    currentPm1RecipeList[3] = textBoxVoltage.Text;
                    currentPm1RecipeList[4] = textBoxCurrent.Text;
                    currentPm1RecipeList[5] = textBoxMotor.Text;
                    currentPm1RecipeList[6] = textBoxScannerVoltage.Text;
                    MessageBox.Show("적용이 완료되었습니다.");
                    break;

                case 2:
                    if (currentPm2RecipeList[0] != null) { currentPm2RecipeList[0] = listBoxIonGas.SelectedItem; }
                    if (currentPm2RecipeList[1] != null) { currentPm2RecipeList[1] = listBoxGas.SelectedItem; }
                    currentPm2RecipeList[2] = textBoxDose.Text;
                    currentPm2RecipeList[3] = textBoxVoltage.Text;
                    currentPm2RecipeList[4] = textBoxCurrent.Text;
                    currentPm2RecipeList[5] = textBoxMotor.Text;
                    currentPm2RecipeList[6] = textBoxScannerVoltage.Text;
                    MessageBox.Show("적용이 완료되었습니다.");
                    break;

                case 3:
                    break;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            switch (currentRecipe)
            {
                case 1:
                    if (currentPm1RecipeList[0] != null) { listBoxIonGas.SelectedItem = currentPm1RecipeList[0]; }
                    if (currentPm1RecipeList[1] != null) { listBoxGas.SelectedItem = currentPm1RecipeList[1]; }
                    textBoxDose.Text = currentPm1RecipeList[2].ToString();
                    textBoxVoltage.Text = currentPm1RecipeList[3].ToString();
                    textBoxCurrent.Text = currentPm1RecipeList[4].ToString();
                    textBoxMotor.Text = currentPm1RecipeList[5].ToString();
                    textBoxScannerVoltage.Text = currentPm1RecipeList[6].ToString();
                    MessageBox.Show("불러오기가 완료되었습니다.");
                    break;

                case 2:
                    if (currentPm2RecipeList[0] != null) { listBoxIonGas.SelectedItem = currentPm1RecipeList[0]; }
                    if (currentPm2RecipeList[1] != null) { listBoxGas.SelectedItem = currentPm1RecipeList[1]; }
                    textBoxDose.Text = currentPm2RecipeList[2].ToString();
                    textBoxVoltage.Text = currentPm2RecipeList[3].ToString();
                    textBoxCurrent.Text = currentPm2RecipeList[4].ToString();
                    textBoxMotor.Text = currentPm2RecipeList[5].ToString();
                    textBoxScannerVoltage.Text = currentPm2RecipeList[6].ToString();
                    MessageBox.Show("불러오기가 완료되었습니다.");
                    break;

                case 3:
                    break;
            }
        }
    }
}
