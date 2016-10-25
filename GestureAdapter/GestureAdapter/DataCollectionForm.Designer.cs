namespace GestureAdapter
{
    partial class DataCollectionForm
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
            this.components = new System.ComponentModel.Container();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 80F);
            this.lblTime.Location = new System.Drawing.Point(536, 85);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(340, 243);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "10";
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 80F);
            this.lblAction.Location = new System.Drawing.Point(263, 361);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(650, 243);
            this.lblAction.TabIndex = 3;
            this.lblAction.Text = "Relax";
            // 
            // btnStartStop
            // 
            this.btnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartStop.Location = new System.Drawing.Point(1180, 724);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(212, 52);
            this.btnStartStop.TabIndex = 4;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // timer
            // 
            this.timer.Interval = 600;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(962, 724);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(212, 52);
            this.button2.TabIndex = 5;
            this.button2.Text = "Bad Flag";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DataCollectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1404, 788);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.lblAction);
            this.Controls.Add(this.lblTime);
            this.Name = "DataCollectionForm";
            this.Text = "Prompt User";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button button2;
    }
}