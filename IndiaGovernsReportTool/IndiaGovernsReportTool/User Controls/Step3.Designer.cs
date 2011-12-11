namespace IndiaGovernsReportTool
{
    partial class Step3
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chart1Column1 = new System.Windows.Forms.ComboBox();
            this.chart2Column = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chart1Column2 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Step 3: Select Chart Columns";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Chart 1 Column";
            // 
            // chart1Column1
            // 
            this.chart1Column1.FormattingEnabled = true;
            this.chart1Column1.Location = new System.Drawing.Point(306, 88);
            this.chart1Column1.Name = "chart1Column1";
            this.chart1Column1.Size = new System.Drawing.Size(121, 21);
            this.chart1Column1.TabIndex = 8;
            // 
            // chart2Column
            // 
            this.chart2Column.FormattingEnabled = true;
            this.chart2Column.Location = new System.Drawing.Point(306, 149);
            this.chart2Column.Name = "chart2Column";
            this.chart2Column.Size = new System.Drawing.Size(121, 21);
            this.chart2Column.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Chart 2 Column";
            // 
            // chart1Column2
            // 
            this.chart1Column2.FormattingEnabled = true;
            this.chart1Column2.Location = new System.Drawing.Point(442, 88);
            this.chart1Column2.Name = "chart1Column2";
            this.chart1Column2.Size = new System.Drawing.Size(121, 21);
            this.chart1Column2.TabIndex = 11;
            // 
            // Step3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chart1Column2);
            this.Controls.Add(this.chart2Column);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chart1Column1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Step3";
            this.Size = new System.Drawing.Size(640, 395);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox chart1Column1;
        private System.Windows.Forms.ComboBox chart2Column;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox chart1Column2;
    }
}
