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
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxNetz = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownBat = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPowerDay = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownPower = new System.Windows.Forms.NumericUpDown();
            this.buttonLoadData = new System.Windows.Forms.Button();
            this.buttonRun = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPellets = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxOverpower = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxMining = new System.Windows.Forms.TextBox();
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
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxMining);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxOverpower);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxPellets);
            this.splitContainer1.Panel1.Controls.Add(this.buttonRun);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxNetz);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 285);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Ofen (kWh)";
            // 
            // textBoxNetz
            // 
            this.textBoxNetz.Location = new System.Drawing.Point(14, 301);
            this.textBoxNetz.Name = "textBoxNetz";
            this.textBoxNetz.ReadOnly = true;
            this.textBoxNetz.Size = new System.Drawing.Size(118, 20);
            this.textBoxNetz.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mean Power Usage (W/h)";
            // 
            // numericUpDownBat
            // 
            this.numericUpDownBat.Location = new System.Drawing.Point(14, 155);
            this.numericUpDownBat.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numericUpDownBat.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownBat.Name = "numericUpDownBat";
            this.numericUpDownBat.Size = new System.Drawing.Size(118, 20);
            this.numericUpDownBat.TabIndex = 5;
            this.numericUpDownBat.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Power per Day (W)";
            // 
            // textBoxPowerDay
            // 
            this.textBoxPowerDay.Location = new System.Drawing.Point(14, 252);
            this.textBoxPowerDay.Name = "textBoxPowerDay";
            this.textBoxPowerDay.ReadOnly = true;
            this.textBoxPowerDay.Size = new System.Drawing.Size(118, 20);
            this.textBoxPowerDay.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Battery Size (kWh)";
            // 
            // numericUpDownPower
            // 
            this.numericUpDownPower.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPower.Location = new System.Drawing.Point(14, 202);
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
            // buttonRun
            // 
            this.buttonRun.Enabled = false;
            this.buttonRun.Location = new System.Drawing.Point(17, 78);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(126, 40);
            this.buttonRun.TabIndex = 9;
            this.buttonRun.Text = "Simulate";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 324);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Pellets (kg)";
            // 
            // textBoxPellets
            // 
            this.textBoxPellets.Location = new System.Drawing.Point(14, 340);
            this.textBoxPellets.Name = "textBoxPellets";
            this.textBoxPellets.ReadOnly = true;
            this.textBoxPellets.Size = new System.Drawing.Size(118, 20);
            this.textBoxPellets.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 363);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Überschuss (kWh)";
            // 
            // textBoxOverpower
            // 
            this.textBoxOverpower.Location = new System.Drawing.Point(12, 379);
            this.textBoxOverpower.Name = "textBoxOverpower";
            this.textBoxOverpower.ReadOnly = true;
            this.textBoxOverpower.Size = new System.Drawing.Size(118, 20);
            this.textBoxOverpower.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 402);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Mining (kWh)";
            // 
            // textBoxMining
            // 
            this.textBoxMining.Location = new System.Drawing.Point(14, 418);
            this.textBoxMining.Name = "textBoxMining";
            this.textBoxMining.ReadOnly = true;
            this.textBoxMining.Size = new System.Drawing.Size(118, 20);
            this.textBoxMining.TabIndex = 14;
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxNetz;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxPellets;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxOverpower;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxMining;
    }
}

