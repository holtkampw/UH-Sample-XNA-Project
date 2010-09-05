namespace AStartTest
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
            this.findPathBtn = new System.Windows.Forms.Button();
            this.timelbl = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // findPathBtn
            // 
            this.findPathBtn.Location = new System.Drawing.Point(513, 213);
            this.findPathBtn.Name = "findPathBtn";
            this.findPathBtn.Size = new System.Drawing.Size(75, 23);
            this.findPathBtn.TabIndex = 42;
            this.findPathBtn.Text = "Find Path";
            this.findPathBtn.UseVisualStyleBackColor = true;
            this.findPathBtn.Click += new System.EventHandler(this.findPathBtn_Click);
            // 
            // timelbl
            // 
            this.timelbl.AutoSize = true;
            this.timelbl.Location = new System.Drawing.Point(447, 253);
            this.timelbl.Name = "timelbl";
            this.timelbl.Size = new System.Drawing.Size(33, 13);
            this.timelbl.TabIndex = 43;
            this.timelbl.Text = "Time:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(513, 133);
            this.button1.Name = "button1";
            this.button1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 44;
            this.button1.Text = "Iterate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 391);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.timelbl);
            this.Controls.Add(this.findPathBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button findPathBtn;
        private System.Windows.Forms.Label timelbl;
        private System.Windows.Forms.Button button1;
    }
}

