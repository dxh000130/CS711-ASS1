namespace Cache
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
            this.btnStart = new System.Windows.Forms.Button();
            this.lblCacheStatus = new System.Windows.Forms.Label();
            this.Server_IP_label = new System.Windows.Forms.Label();
            this.Client_IP_label = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.Open_Log = new System.Windows.Forms.Button();
            this.Open_Detail = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(665, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(123, 41);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Cache";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // lblCacheStatus
            // 
            this.lblCacheStatus.Location = new System.Drawing.Point(22, 148);
            this.lblCacheStatus.Name = "lblCacheStatus";
            this.lblCacheStatus.Size = new System.Drawing.Size(625, 251);
            this.lblCacheStatus.TabIndex = 3;
            this.lblCacheStatus.Text = "Cache Server Status";
            // 
            // Server_IP_label
            // 
            this.Server_IP_label.Location = new System.Drawing.Point(12, 8);
            this.Server_IP_label.Name = "Server_IP_label";
            this.Server_IP_label.Size = new System.Drawing.Size(511, 45);
            this.Server_IP_label.TabIndex = 4;
            // 
            // Client_IP_label
            // 
            this.Client_IP_label.Location = new System.Drawing.Point(13, 67);
            this.Client_IP_label.Name = "Client_IP_label";
            this.Client_IP_label.Size = new System.Drawing.Size(509, 48);
            this.Client_IP_label.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(665, 70);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 45);
            this.button1.TabIndex = 6;
            this.button1.Text = "clear button";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Clear_Button);
            // 
            // Open_Log
            // 
            this.Open_Log.Location = new System.Drawing.Point(665, 138);
            this.Open_Log.Name = "Open_Log";
            this.Open_Log.Size = new System.Drawing.Size(122, 47);
            this.Open_Log.TabIndex = 7;
            this.Open_Log.Text = "Open Log";
            this.Open_Log.UseVisualStyleBackColor = true;
            this.Open_Log.Click += new System.EventHandler(this.Open_Log_f);
            // 
            // Open_Detail
            // 
            this.Open_Detail.Location = new System.Drawing.Point(667, 219);
            this.Open_Detail.Name = "Open_Detail";
            this.Open_Detail.Size = new System.Drawing.Size(119, 49);
            this.Open_Detail.TabIndex = 8;
            this.Open_Detail.Text = "Open Detail log";
            this.Open_Detail.UseVisualStyleBackColor = true;
            this.Open_Detail.Click += new System.EventHandler(this.Open_DLog_f);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Open_Detail);
            this.Controls.Add(this.Open_Log);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Client_IP_label);
            this.Controls.Add(this.Server_IP_label);
            this.Controls.Add(this.lblCacheStatus);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button Open_Log;
        private System.Windows.Forms.Button Open_Detail;

        private System.Windows.Forms.Button button1;

        private System.Windows.Forms.Label Client_IP_label;

        private System.Windows.Forms.Label Server_IP_label;

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblCacheStatus;

        #endregion
    }
}