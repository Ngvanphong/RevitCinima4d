namespace RevitToCinima
{
    partial class frmRevitToCinima
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
            this.btnExportCinima = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExportCinima
            // 
            this.btnExportCinima.Location = new System.Drawing.Point(488, 402);
            this.btnExportCinima.Name = "btnExportCinima";
            this.btnExportCinima.Size = new System.Drawing.Size(75, 23);
            this.btnExportCinima.TabIndex = 0;
            this.btnExportCinima.Text = "Export";
            this.btnExportCinima.UseVisualStyleBackColor = true;
            this.btnExportCinima.Click += new System.EventHandler(this.btnExportCinima_Click);
            // 
            // frmRevitToCinima
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 437);
            this.Controls.Add(this.btnExportCinima);
            this.MinimizeBox = false;
            this.Name = "frmRevitToCinima";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmRevitToCinima";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmRevitToCinima_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button btnExportCinima;
    }
}