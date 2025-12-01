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
        // 현재 선택된 레시피 (1=PM1, 2=PM2, 3=PM3)
        int currentRecipe = 1;

        // 레시피 세트 (JSON으로 저장/불러오기)
        private RecipeSet recipeSet;

        public RecipeView()
        {
            InitializeComponent();
            this.ActivateToggle(false);

            // 저장된 레시피 불러오기 (없으면 기본값 생성)
            recipeSet = RecipeManager.LoadRecipe();

            // UI에 레시피 데이터 표시
            LoadRecipeToUI();
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

        /// <summary>
        /// 현재 선택된 PM 패널만 표시
        /// </summary>
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

        /// <summary>
        /// 레시피 저장 버튼 클릭
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // 모든 PM의 UI 값을 레시피에 반영
            SavePM1RecipeFromUI();
            SavePM2RecipeFromUI();
            SavePM3RecipeFromUI();

            // JSON 파일로 저장
            if (RecipeManager.SaveRecipe(recipeSet))
            {
                MessageBox.Show("레시피가 저장되었습니다.", "저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("레시피 저장에 실패했습니다.", "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 레시피 불러오기 버튼 클릭
        /// </summary>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            // JSON 파일에서 불러오기
            recipeSet = RecipeManager.LoadRecipe();

            // UI에 표시
            LoadRecipeToUI();

            MessageBox.Show("레시피를 불러왔습니다.", "불러오기 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region PM1 레시피 UI 처리

        /// <summary>
        /// PM1 UI에서 레시피 데이터 저장
        /// </summary>
        private void SavePM1RecipeFromUI()
        {
            var recipe = recipeSet.PM1Recipe;

            recipe.IonGas = listBoxIonGas.SelectedItem?.ToString() ?? "";

            double.TryParse(textBoxDose.Text, out double dose);
            recipe.Dose = dose;

            double.TryParse(textBoxVoltage.Text, out double voltage);
            recipe.Voltage = voltage;

            double.TryParse(textBoxCurrent.Text, out double current);
            recipe.Current = current;

            double.TryParse(textBoxMotor.Text, out double motor);
            recipe.Motor = motor;

            double.TryParse(textBoxScannerVoltage.Text, out double scannerVoltage);
            recipe.ScannerVoltage = scannerVoltage;
        }

        /// <summary>
        /// PM1 레시피 데이터를 UI에 표시
        /// </summary>
        private void LoadPM1RecipeToUI()
        {
            var recipe = recipeSet.PM1Recipe;

            if (!string.IsNullOrEmpty(recipe.IonGas))
            {
                for (int i = 0; i < listBoxIonGas.Items.Count; i++)
                {
                    if (listBoxIonGas.Items[i].ToString() == recipe.IonGas)
                    {
                        listBoxIonGas.SelectedIndex = i;
                        break;
                    }
                }
            }

            textBoxDose.Text = recipe.Dose > 0 ? recipe.Dose.ToString() : "";
            textBoxVoltage.Text = recipe.Voltage > 0 ? recipe.Voltage.ToString() : "";
            textBoxCurrent.Text = recipe.Current > 0 ? recipe.Current.ToString() : "";
            textBoxMotor.Text = recipe.Motor > 0 ? recipe.Motor.ToString() : "";
            textBoxScannerVoltage.Text = recipe.ScannerVoltage > 0 ? recipe.ScannerVoltage.ToString() : "";
        }

        #endregion

        #region PM2 레시피 UI 처리

        /// <summary>
        /// PM2 UI에서 레시피 데이터 저장
        /// </summary>
        private void SavePM2RecipeFromUI()
        {
            var recipe = recipeSet.PM2Recipe;

            recipe.IonGas = listBoxIonGas2.SelectedItem?.ToString() ?? "";

            double.TryParse(textBoxDose2.Text, out double dose);
            recipe.Dose = dose;

            double.TryParse(textBoxVoltage2.Text, out double voltage);
            recipe.Voltage = voltage;

            double.TryParse(textBoxCurrent2.Text, out double current);
            recipe.Current = current;

            double.TryParse(textBoxMotor2.Text, out double motor);
            recipe.Motor = motor;

            double.TryParse(textBoxScannerVoltage2.Text, out double scannerVoltage);
            recipe.ScannerVoltage = scannerVoltage;
        }

        /// <summary>
        /// PM2 레시피 데이터를 UI에 표시
        /// </summary>
        private void LoadPM2RecipeToUI()
        {
            var recipe = recipeSet.PM2Recipe;

            if (!string.IsNullOrEmpty(recipe.IonGas))
            {
                for (int i = 0; i < listBoxIonGas2.Items.Count; i++)
                {
                    if (listBoxIonGas2.Items[i].ToString() == recipe.IonGas)
                    {
                        listBoxIonGas2.SelectedIndex = i;
                        break;
                    }
                }
            }

            textBoxDose2.Text = recipe.Dose > 0 ? recipe.Dose.ToString() : "";
            textBoxVoltage2.Text = recipe.Voltage > 0 ? recipe.Voltage.ToString() : "";
            textBoxCurrent2.Text = recipe.Current > 0 ? recipe.Current.ToString() : "";
            textBoxMotor2.Text = recipe.Motor > 0 ? recipe.Motor.ToString() : "";
            textBoxScannerVoltage2.Text = recipe.ScannerVoltage > 0 ? recipe.ScannerVoltage.ToString() : "";
        }

        #endregion

        #region PM3 레시피 UI 처리

        /// <summary>
        /// PM3 UI에서 레시피 데이터 저장
        /// </summary>
        private void SavePM3RecipeFromUI()
        {
            var recipe = recipeSet.PM3Recipe;

            double.TryParse(textBoxVaccum.Text, out double vacuum);
            recipe.Vacuum = vacuum;
            // Vacuum 값을 목표 압력으로도 설정 (시뮬레이션용)
            if (vacuum > 0)
            {
                recipe.TargetPressure = vacuum;
            }

            double.TryParse(textBoxTemperature.Text, out double temperature);
            recipe.Temperature = temperature;
            // 온도 입력값을 목표 온도로도 설정 (시뮬레이션용)
            if (temperature > 0)
            {
                recipe.TargetTemperature = temperature;
            }

            // ProcessTime은 MainView에서 별도 설정하므로 여기서 제거
        }

        /// <summary>
        /// PM3 레시피 데이터를 UI에 표시
        /// </summary>
        private void LoadPM3RecipeToUI()
        {
            var recipe = recipeSet.PM3Recipe;

            textBoxVaccum.Text = recipe.Vacuum > 0 ? recipe.Vacuum.ToString() : "";
            textBoxTemperature.Text = recipe.Temperature > 0 ? recipe.Temperature.ToString() : "";
            // ProcessTime은 MainView에서 별도 설정하므로 표시하지 않음
        }

        #endregion

        /// <summary>
        /// 모든 레시피 데이터를 UI에 로드
        /// </summary>
        private void LoadRecipeToUI()
        {
            LoadPM1RecipeToUI();
            LoadPM2RecipeToUI();
            LoadPM3RecipeToUI();
        }

        /// <summary>
        /// 현재 레시피 세트 반환 (MainView에서 사용)
        /// </summary>
        public RecipeSet GetCurrentRecipeSet()
        {
            // 최신 UI 값을 레시피에 반영
            SavePM1RecipeFromUI();
            SavePM2RecipeFromUI();
            SavePM3RecipeFromUI();

            return recipeSet;
        }

        /// <summary>
        /// 특정 PM의 레시피 반환
        /// </summary>
        public IonImplantRecipe GetPM1Recipe()
        {
            SavePM1RecipeFromUI();
            return recipeSet.PM1Recipe;
        }

        public IonImplantRecipe GetPM2Recipe()
        {
            SavePM2RecipeFromUI();
            return recipeSet.PM2Recipe;
        }

        public AnnealingRecipe GetPM3Recipe()
        {
            SavePM3RecipeFromUI();
            return recipeSet.PM3Recipe;
        }
    }
}
