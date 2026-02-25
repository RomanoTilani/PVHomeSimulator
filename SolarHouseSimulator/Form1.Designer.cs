namespace SolarHouseSimulator
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.formsPlot = new ScottPlot.WinForms.FormsPlot();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownBat = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPowerDay = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownPower = new System.Windows.Forms.NumericUpDown();
            this.buttonLoadData = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPower)).BeginInit();
            this.SuspendLayout();
            // 
            // formsPlot
            // 
            this.formsPlot.DisplayScale = 0F;
            this.formsPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formsPlot.Location = new System.Drawing.Point(0, 0);
            this.formsPlot.Name = "formsPlot";
            this.formsPlot.Size = new System.Drawing.Size(1294, 835);
            this.formsPlot.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.numericUpDownBat);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxPowerDay);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.numericUpDownPower);
            this.splitContainer1.Panel1.Controls.Add(this.buttonLoadData);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.formsPlot);
            this.splitContainer1.Size = new System.Drawing.Size(1471, 835);
            this.splitContainer1.SplitterDistance = 173;
            this.splitContainer1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mean Power Usage (W/h)";
            // 
            // numericUpDownBat
            // 
            this.numericUpDownBat.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownBat.Location = new System.Drawing.Point(17, 108);
            this.numericUpDownBat.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.numericUpDownBat.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownBat.Name = "numericUpDownBat";
            this.numericUpDownBat.Size = new System.Drawing.Size(118, 20);
            this.numericUpDownBat.TabIndex = 5;
            this.numericUpDownBat.Value = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Power per Day (W)";
            // 
            // textBoxPowerDay
            // 
            this.textBoxPowerDay.Location = new System.Drawing.Point(17, 205);
            this.textBoxPowerDay.Name = "textBoxPowerDay";
            this.textBoxPowerDay.ReadOnly = true;
            this.textBoxPowerDay.Size = new System.Drawing.Size(118, 20);
            this.textBoxPowerDay.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Battery Size (Wh)";
            // 
            // numericUpDownPower
            // 
            this.numericUpDownPower.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPower.Location = new System.Drawing.Point(17, 155);
            this.numericUpDownPower.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownPower.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPower.Name = "numericUpDownPower";
            this.numericUpDownPower.Size = new System.Drawing.Size(118, 20);
            this.numericUpDownPower.TabIndex = 1;
            this.numericUpDownPower.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericUpDownPower.ValueChanged += new System.EventHandler(this.numericUpDownPower_ValueChanged);
            // 
            // buttonLoadData
            // 
            this.buttonLoadData.Location = new System.Drawing.Point(17, 16);
            this.buttonLoadData.Name = "buttonLoadData";
            this.buttonLoadData.Size = new System.Drawing.Size(126, 40);
            this.buttonLoadData.TabIndex = 0;
            this.buttonLoadData.Text = "Load Data";
            this.buttonLoadData.UseVisualStyleBackColor = true;
            this.buttonLoadData.Click += new System.EventHandler(this.buttonLoadData_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1471, 835);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Solar House Simulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPower)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ScottPlot.WinForms.FormsPlot formsPlot;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonLoadData;
        private System.Windows.Forms.NumericUpDown numericUpDownPower;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPowerDay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownBat;
    }
}

