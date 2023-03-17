namespace Client
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
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.buttonRefreshList = new System.Windows.Forms.Button();
            this.buttonDownloadFile = new System.Windows.Forms.Button();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.ItemHeight = 18;
            this.listBoxFiles.Location = new System.Drawing.Point(86, 56);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(453, 184);
            this.listBoxFiles.TabIndex = 0;
            // 
            // buttonRefreshList
            // 
            this.buttonRefreshList.Location = new System.Drawing.Point(20, 16);
            this.buttonRefreshList.Name = "buttonRefreshList";
            this.buttonRefreshList.Size = new System.Drawing.Size(151, 34);
            this.buttonRefreshList.TabIndex = 1;
            this.buttonRefreshList.Text = "buttonRefreshList";
            this.buttonRefreshList.UseVisualStyleBackColor = true;
            // 
            // buttonDownloadFile
            // 
            this.buttonDownloadFile.Location = new System.Drawing.Point(224, 17);
            this.buttonDownloadFile.Name = "buttonDownloadFile";
            this.buttonDownloadFile.Size = new System.Drawing.Size(131, 32);
            this.buttonDownloadFile.TabIndex = 2;
            this.buttonDownloadFile.Text = "buttonDownloadFile";
            this.buttonDownloadFile.UseVisualStyleBackColor = true;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(68, 246);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(524, 141);
            this.pictureBoxPreview.TabIndex = 3;
            this.pictureBoxPreview.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(23, 398);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(740, 39);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.buttonDownloadFile);
            this.Controls.Add(this.buttonRefreshList);
            this.Controls.Add(this.listBoxFiles);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.PictureBox pictureBoxPreview;

        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button buttonRefreshList;
        private System.Windows.Forms.Button buttonDownloadFile;

        #endregion
    }
}