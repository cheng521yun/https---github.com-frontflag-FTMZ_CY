﻿using UI.Ctrl.TextBox;

namespace UI.UC.福利中心.老人退住管理
{
    partial class 老人退住管理
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txt姓名 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt身份证 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt申请时间 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt入住时间 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt退住级别 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt入住级别 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmb性别 = new UI.Ctrl.Cmb.性别();
            this.txt年龄 = new UI.Ctrl.TextBox.NumberTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "户主姓名：";
            // 
            // txt姓名
            // 
            this.txt姓名.Location = new System.Drawing.Point(101, 36);
            this.txt姓名.Name = "txt姓名";
            this.txt姓名.Size = new System.Drawing.Size(100, 21);
            this.txt姓名.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(55, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "年龄：";
            // 
            // txt身份证
            // 
            this.txt身份证.Location = new System.Drawing.Point(101, 100);
            this.txt身份证.Name = "txt身份证";
            this.txt身份证.Size = new System.Drawing.Size(100, 21);
            this.txt身份证.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "身份证：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(233, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "性别：";
            // 
            // txt申请时间
            // 
            this.txt申请时间.Location = new System.Drawing.Point(101, 164);
            this.txt申请时间.Name = "txt申请时间";
            this.txt申请时间.Size = new System.Drawing.Size(100, 21);
            this.txt申请时间.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "申请时间：";
            // 
            // txt入住时间
            // 
            this.txt入住时间.Location = new System.Drawing.Point(101, 132);
            this.txt入住时间.Name = "txt入住时间";
            this.txt入住时间.Size = new System.Drawing.Size(100, 21);
            this.txt入住时间.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(31, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "入住时间：";
            // 
            // txt退住级别
            // 
            this.txt退住级别.Location = new System.Drawing.Point(101, 228);
            this.txt退住级别.Name = "txt退住级别";
            this.txt退住级别.Size = new System.Drawing.Size(255, 21);
            this.txt退住级别.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 232);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "退住级别：";
            // 
            // txt入住级别
            // 
            this.txt入住级别.Location = new System.Drawing.Point(101, 196);
            this.txt入住级别.Name = "txt入住级别";
            this.txt入住级别.Size = new System.Drawing.Size(100, 21);
            this.txt入住级别.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(31, 200);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 14;
            this.label9.Text = "入住级别：";
            // 
            // cmb性别
            // 
            this.cmb性别.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb性别.DropDownWidth = 80;
            this.cmb性别.FormattingEnabled = true;
            this.cmb性别.Items.AddRange(new object[] {
            " ",
            "男",
            "女"});
            this.cmb性别.Location = new System.Drawing.Point(276, 36);
            this.cmb性别.Name = "cmb性别";
            this.cmb性别.Size = new System.Drawing.Size(80, 20);
            this.cmb性别.TabIndex = 9;
            this.cmb性别.Value = "False";
            // 
            // txt年龄
            // 
            this.txt年龄.Location = new System.Drawing.Point(101, 68);
            this.txt年龄.Name = "txt年龄";
            this.txt年龄.Size = new System.Drawing.Size(100, 21);
            this.txt年龄.TabIndex = 5;
            // 
            // 老人退住管理
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.txt退住级别);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txt入住级别);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txt申请时间);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txt入住时间);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cmb性别);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt身份证);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txt年龄);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt姓名);
            this.Controls.Add(this.label1);
            this.Name = "老人退住管理";
            this.Size = new System.Drawing.Size(398, 297);
            this.Load += new System.EventHandler(this.老人退住管理_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt姓名;
        private NumberTextBox txt年龄;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt身份证;
        private System.Windows.Forms.Label label4;
        private UI.Ctrl.Cmb.性别 cmb性别;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt申请时间;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt入住时间;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt退住级别;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt入住级别;
        private System.Windows.Forms.Label label9;
    }
}
