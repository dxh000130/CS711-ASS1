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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.ItemHeight = 18;
            this.listBoxFiles.Location = new System.Drawing.Point(12, 56);
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
            this.buttonRefreshList.Text = "Refresh File List";
            this.buttonRefreshList.UseVisualStyleBackColor = true;
            this.buttonRefreshList.Click += new System.EventHandler(this.buttonRefreshList_Click);
            // 
            // buttonDownloadFile
            // 
            this.buttonDownloadFile.Location = new System.Drawing.Point(224, 17);
            this.buttonDownloadFile.Name = "buttonDownloadFile";
            this.buttonDownloadFile.Size = new System.Drawing.Size(131, 32);
            this.buttonDownloadFile.TabIndex = 2;
            this.buttonDownloadFile.Text = "Download File";
            this.buttonDownloadFile.UseVisualStyleBackColor = true;
            this.buttonDownloadFile.Click += new System.EventHandler(this.buttonDownloadFile_Click);
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(12, 246);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(776, 192);
            this.pictureBoxPreview.TabIndex = 3;
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.Click += new System.EventHandler(this.Open_Download_file);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(481, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(307, 171);
            this.label1.TabIndex = 4;
            this.label1.Text = "Status Label";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(411, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(126, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "Open Log";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Open_Log_f);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(590, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(171, 27);
            this.button2.TabIndex = 6;
            this.button2.Text = "Open Detail Log";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Open_DLog_f);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.buttonDownloadFile);
            this.Controls.Add(this.buttonRefreshList);
            this.Controls.Add(this.listBoxFiles);
            this.Name = "Form1";
            this.Text = "Client";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.PictureBox pictureBoxPreview;

        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button buttonRefreshList;
        private System.Windows.Forms.Button buttonDownloadFile;

        #endregion
    }
}