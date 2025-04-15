namespace RuTube_downloader
{
    partial class FrameRuTubeVideo
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblVideoTitle = new System.Windows.Forms.Label();
            this.lblDateUploaded = new System.Windows.Forms.Label();
            this.lblDatePublished = new System.Windows.Forms.Label();
            this.lblChannelName = new System.Windows.Forms.Label();
            this.btnDownload = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBarDownload = new System.Windows.Forms.ProgressBar();
            this.pictureBoxVideoThumbnail = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoThumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // lblVideoTitle
            // 
            this.lblVideoTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVideoTitle.BackColor = System.Drawing.Color.Gainsboro;
            this.lblVideoTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblVideoTitle.Location = new System.Drawing.Point(184, 3);
            this.lblVideoTitle.Name = "lblVideoTitle";
            this.lblVideoTitle.Size = new System.Drawing.Size(305, 53);
            this.lblVideoTitle.TabIndex = 0;
            this.lblVideoTitle.Text = "lblVideoTitle";
            // 
            // lblDateUploaded
            // 
            this.lblDateUploaded.AutoSize = true;
            this.lblDateUploaded.BackColor = System.Drawing.Color.Gainsboro;
            this.lblDateUploaded.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDateUploaded.Location = new System.Drawing.Point(184, 77);
            this.lblDateUploaded.Name = "lblDateUploaded";
            this.lblDateUploaded.Size = new System.Drawing.Size(111, 16);
            this.lblDateUploaded.TabIndex = 1;
            this.lblDateUploaded.Text = "lblDateUploaded";
            // 
            // lblDatePublished
            // 
            this.lblDatePublished.AutoSize = true;
            this.lblDatePublished.BackColor = System.Drawing.Color.Gainsboro;
            this.lblDatePublished.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDatePublished.Location = new System.Drawing.Point(184, 93);
            this.lblDatePublished.Name = "lblDatePublished";
            this.lblDatePublished.Size = new System.Drawing.Size(110, 16);
            this.lblDatePublished.TabIndex = 2;
            this.lblDatePublished.Text = "lblDatePublished";
            // 
            // lblChannelName
            // 
            this.lblChannelName.AutoSize = true;
            this.lblChannelName.BackColor = System.Drawing.Color.Gainsboro;
            this.lblChannelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblChannelName.Location = new System.Drawing.Point(184, 60);
            this.lblChannelName.Name = "lblChannelName";
            this.lblChannelName.Size = new System.Drawing.Size(111, 17);
            this.lblChannelName.TabIndex = 3;
            this.lblChannelName.Text = "lblChannelName";
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownload.Location = new System.Drawing.Point(414, 129);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 4;
            this.btnDownload.Text = "Скачать";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProgress.AutoSize = true;
            this.lblProgress.BackColor = System.Drawing.Color.Gainsboro;
            this.lblProgress.Location = new System.Drawing.Point(3, 113);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(58, 13);
            this.lblProgress.TabIndex = 5;
            this.lblProgress.Text = "lblProgress";
            // 
            // progressBarDownload
            // 
            this.progressBarDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarDownload.Location = new System.Drawing.Point(3, 129);
            this.progressBarDownload.Name = "progressBarDownload";
            this.progressBarDownload.Size = new System.Drawing.Size(405, 23);
            this.progressBarDownload.TabIndex = 6;
            // 
            // pictureBoxVideoThumbnail
            // 
            this.pictureBoxVideoThumbnail.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pictureBoxVideoThumbnail.Location = new System.Drawing.Point(6, 3);
            this.pictureBoxVideoThumbnail.Name = "pictureBoxVideoThumbnail";
            this.pictureBoxVideoThumbnail.Size = new System.Drawing.Size(172, 106);
            this.pictureBoxVideoThumbnail.TabIndex = 7;
            this.pictureBoxVideoThumbnail.TabStop = false;
            this.pictureBoxVideoThumbnail.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxVideoThumbnail_Paint);
            // 
            // FrameRuTubeVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.pictureBoxVideoThumbnail);
            this.Controls.Add(this.progressBarDownload);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.lblChannelName);
            this.Controls.Add(this.lblDatePublished);
            this.Controls.Add(this.lblDateUploaded);
            this.Controls.Add(this.lblVideoTitle);
            this.Name = "FrameRuTubeVideo";
            this.Size = new System.Drawing.Size(492, 155);
            this.Load += new System.EventHandler(this.FrameRuTubeVideo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoThumbnail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVideoTitle;
        private System.Windows.Forms.Label lblDateUploaded;
        private System.Windows.Forms.Label lblDatePublished;
        private System.Windows.Forms.Label lblChannelName;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressBarDownload;
        private System.Windows.Forms.PictureBox pictureBoxVideoThumbnail;
    }
}
