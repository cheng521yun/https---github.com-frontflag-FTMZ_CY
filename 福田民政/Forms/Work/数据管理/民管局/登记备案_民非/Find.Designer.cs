﻿namespace 福田民政.Forms.Work.数据管理.民管局.登记备案
{
    partial class Find
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnTool = new UI.UC.Comm.ListTool();
            this.SuspendLayout();
            // 
            // BtnTool
            // 
            this.BtnTool.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnTool.Location = new System.Drawing.Point(266, 0);
            this.BtnTool.Name = "BtnTool";
            this.BtnTool.Size = new System.Drawing.Size(361, 40);
            this.BtnTool.TabIndex = 0;
            // 
            // Find
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 40);
            this.Controls.Add(this.BtnTool);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Find";
            this.Text = "Find";
            this.ResumeLayout(false);

        }

        #endregion

        public UI.UC.Comm.ListTool BtnTool;

    }
}