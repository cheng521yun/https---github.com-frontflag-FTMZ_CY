﻿namespace 福田民政.Forms.Work.数据管理.事务科.专项救助数据.Tab.意外救助
{
    partial class Edit
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
            DB.Stru.事务科.专项救助_意外救助 专项救助_意外救助1 = new DB.Stru.事务科.专项救助_意外救助();
            this.uc = new UI.UC.事务科.专项救助.意外救助();
            this.SuspendLayout();
            // 
            // uc
            // 
            this.uc.BackColor = System.Drawing.Color.White;
            this.uc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uc.Location = new System.Drawing.Point(0, 40);
            this.uc.Name = "uc";
            this.uc.Size = new System.Drawing.Size(379, 310);
            专项救助_意外救助1.dat救助时间 = new System.DateTime(2015, 5, 11, 6, 44, 15, 883);
            专项救助_意外救助1.ID = "";
            专项救助_意外救助1.备注 = "";
            专项救助_意外救助1.姓名 = "";
            专项救助_意外救助1.性别 = "";
            专项救助_意外救助1.所属街道 = "";
            专项救助_意外救助1.救助原因 = "";
            专项救助_意外救助1.救助时间 = "";
            专项救助_意外救助1.类别 = "";
            专项救助_意外救助1.联系电话 = "";
            专项救助_意外救助1.金额 = "";
            this.uc.stru = 专项救助_意外救助1;
            this.uc.TabIndex = 0;
            // 
            // Edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 390);
            this.Controls.Add(this.uc);
            this.Name = "Edit";
            this.Text = "编辑";
            this.Controls.SetChildIndex(this.uc, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private UI.UC.事务科.专项救助.意外救助 uc;
    }
}