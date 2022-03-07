
using System;

namespace CurrencyConverterGUI
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
            this.ConversionGroupBox = new System.Windows.Forms.GroupBox();
            this.FromLabel = new System.Windows.Forms.Label();
            this.ToLabel = new System.Windows.Forms.Label();
            this.AmountLabel = new System.Windows.Forms.Label();
            this.AmountTextBox = new System.Windows.Forms.TextBox();
            this.ConvertButton = new System.Windows.Forms.Button();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.ResultLabel = new System.Windows.Forms.Label();
            this.ResultTextBox = new System.Windows.Forms.TextBox();
            this.FromComboBox = new System.Windows.Forms.ComboBox();
            this.ToComboBox = new System.Windows.Forms.ComboBox();
            this.ConversionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConversionGroupBox
            // 
            this.ConversionGroupBox.Controls.Add(this.ToComboBox);
            this.ConversionGroupBox.Controls.Add(this.FromComboBox);
            this.ConversionGroupBox.Controls.Add(this.ResultTextBox);
            this.ConversionGroupBox.Controls.Add(this.ResultLabel);
            this.ConversionGroupBox.Controls.Add(this.UpdateButton);
            this.ConversionGroupBox.Controls.Add(this.ConvertButton);
            this.ConversionGroupBox.Controls.Add(this.AmountTextBox);
            this.ConversionGroupBox.Controls.Add(this.AmountLabel);
            this.ConversionGroupBox.Controls.Add(this.ToLabel);
            this.ConversionGroupBox.Controls.Add(this.FromLabel);
            this.ConversionGroupBox.Location = new System.Drawing.Point(-2, -1);
            this.ConversionGroupBox.Name = "ConversionGroupBox";
            this.ConversionGroupBox.Size = new System.Drawing.Size(360, 183);
            this.ConversionGroupBox.TabIndex = 0;
            this.ConversionGroupBox.TabStop = false;
            this.ConversionGroupBox.Text = "Convert Currency";
            // 
            // FromLabel
            // 
            this.FromLabel.AutoSize = true;
            this.FromLabel.Location = new System.Drawing.Point(16, 22);
            this.FromLabel.Name = "FromLabel";
            this.FromLabel.Size = new System.Drawing.Size(40, 17);
            this.FromLabel.TabIndex = 0;
            this.FromLabel.Text = "From";
            // 
            // ToLabel
            // 
            this.ToLabel.AutoSize = true;
            this.ToLabel.Location = new System.Drawing.Point(194, 22);
            this.ToLabel.Name = "ToLabel";
            this.ToLabel.Size = new System.Drawing.Size(25, 17);
            this.ToLabel.TabIndex = 1;
            this.ToLabel.Text = "To";
            // 
            // AmountLabel
            // 
            this.AmountLabel.AutoSize = true;
            this.AmountLabel.Location = new System.Drawing.Point(6, 48);
            this.AmountLabel.Name = "AmountLabel";
            this.AmountLabel.Size = new System.Drawing.Size(56, 17);
            this.AmountLabel.TabIndex = 4;
            this.AmountLabel.Text = "Amount";
            // 
            // AmountTextBox
            // 
            this.AmountTextBox.Location = new System.Drawing.Point(68, 48);
            this.AmountTextBox.Name = "AmountTextBox";
            this.AmountTextBox.Size = new System.Drawing.Size(122, 22);
            this.AmountTextBox.TabIndex = 5;
            // 
            // ConvertButton
            // 
            this.ConvertButton.Location = new System.Drawing.Point(224, 48);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(121, 24);
            this.ConvertButton.TabIndex = 6;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.UseVisualStyleBackColor = true;
            this.ConvertButton.Click += new System.EventHandler(this.ConvertButton_Click);
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(69, 106);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(277, 26);
            this.UpdateButton.TabIndex = 7;
            this.UpdateButton.Text = "Update Currencies";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // ResultLabel
            // 
            this.ResultLabel.AutoSize = true;
            this.ResultLabel.Location = new System.Drawing.Point(8, 78);
            this.ResultLabel.Name = "ResultLabel";
            this.ResultLabel.Size = new System.Drawing.Size(48, 17);
            this.ResultLabel.TabIndex = 8;
            this.ResultLabel.Text = "Result";
            // 
            // ResultTextBox
            // 
            this.ResultTextBox.Enabled = false;
            this.ResultTextBox.Location = new System.Drawing.Point(68, 78);
            this.ResultTextBox.Name = "ResultTextBox";
            this.ResultTextBox.Size = new System.Drawing.Size(277, 22);
            this.ResultTextBox.TabIndex = 9;
            // 
            // FromComboBox
            // 
            this.FromComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FromComboBox.FormattingEnabled = true;
            this.FromComboBox.Location = new System.Drawing.Point(69, 19);
            this.FromComboBox.Name = "FromComboBox";
            this.FromComboBox.Size = new System.Drawing.Size(121, 24);
            this.FromComboBox.TabIndex = 10;
            // 
            // ToComboBox
            // 
            this.ToComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToComboBox.FormattingEnabled = true;
            this.ToComboBox.Location = new System.Drawing.Point(224, 19);
            this.ToComboBox.Name = "ToComboBox";
            this.ToComboBox.Size = new System.Drawing.Size(121, 24);
            this.ToComboBox.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 136);
            this.Controls.Add(this.ConversionGroupBox);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ConversionGroupBox.ResumeLayout(false);
            this.ConversionGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.GroupBox ConversionGroupBox;
        private System.Windows.Forms.Button ConvertButton;
        private System.Windows.Forms.TextBox AmountTextBox;
        private System.Windows.Forms.Label AmountLabel;
        private System.Windows.Forms.Label ToLabel;
        private System.Windows.Forms.Label FromLabel;
        private System.Windows.Forms.TextBox ResultTextBox;
        private System.Windows.Forms.Label ResultLabel;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.ComboBox ToComboBox;
        private System.Windows.Forms.ComboBox FromComboBox;
    }
}

