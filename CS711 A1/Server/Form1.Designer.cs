namespace Server
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
            this.btnAddFile = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.StorageFilePathLabel = new System.Windows.Forms.Label();
            this.IPLable = new System.Windows.Forms.Label();
            this.FilesListbox = new System.Windows.Forms.ListBox();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAddFile
            // 
            this.btnAddFile.Location = new System.Drawing.Point(213, 597);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(172, 55);
            this.btnAddFile.TabIndex = 0;
            this.btnAddFile.Text = "Add File";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click_1);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(43, 597);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(122, 55);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start Server";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // StorageFilePathLabel
            // 
            this.StorageFilePathLabel.Location = new System.Drawing.Point(43, 44);
            this.StorageFilePathLabel.Name = "StorageFilePathLabel";
            this.StorageFilePathLabel.Size = new System.Drawing.Size(628, 44);
            this.StorageFilePathLabel.TabIndex = 2;
            this.StorageFilePathLabel.Text = "StorageFilePathLabel";
            // 
            // IPLable
            // 
            this.IPLable.Location = new System.Drawing.Point(12, 9);
            this.IPLable.Name = "IPLable";
            this.IPLable.Size = new System.Drawing.Size(799, 35);
            this.IPLable.TabIndex = 3;
            this.IPLable.Text = "IPLable";
            // 
            // FilesListbox
            // 
            this.FilesListbox.FormattingEnabled = true;
            this.FilesListbox.ItemHeight = 18;
            this.FilesListbox.Location = new System.Drawing.Point(60, 91);
            this.FilesListbox.Name = "FilesListbox";
            this.FilesListbox.Size = new System.Drawing.Size(995, 454);
            this.FilesListbox.TabIndex = 4;
            // 
            // StatusLabel
            // 
            this.StatusLabel.Location = new System.Drawing.Point(452, 546);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(666, 144);
            this.StatusLabel.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1231, 703);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.FilesListbox);
            this.Controls.Add(this.IPLable);
            this.Controls.Add(this.StorageFilePathLabel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.btnAddFile);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label StatusLabel;

        private System.Windows.Forms.ListBox FilesListbox;

        private System.Windows.Forms.Label IPLable;

        private System.Windows.Forms.Label StorageFilePathLabel;

        private System.Windows.Forms.Button startButton;

        private System.Windows.Forms.Button btnAddFile;

        #endregion
    }
}