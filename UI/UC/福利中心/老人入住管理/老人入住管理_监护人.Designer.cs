﻿using UI.Ctrl.TextBox;

namespace UI.UC.福利中心.老人入住管理
{
    partial class 老人入住管理_监护人
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
            this.txt联系电话 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt住址或工作单位 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt关系 = new UI.Ctrl.TextBox.NumberTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt姓名 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt联系电话
            // 
            this.txt联系电话.Location = new System.Drawing.Point(118, 123);
            this.txt联系电话.Name = "txt联系电话";
            this.txt联系电话.Size = new System.Drawing.Size(100, 21);
            this.txt联系电话.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(48, 127);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 18;
            this.label7.Text = "联系电话：";
            // 
            // txt住址或工作单位
            // 
            this.txt住址或工作单位.Location = new System.Drawing.Point(118, 91);
            this.txt住址或工作单位.Name = "txt住址或工作单位";
            this.txt住址或工作单位.Size = new System.Drawing.Size(100, 21);
            this.txt住址或工作单位.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "住址或工作单位：";
            // 
            // txt关系
            // 
            this.txt关系.Location = new System.Drawing.Point(118, 59);
            this.txt关系.Name = "txt关系";
            this.txt关系.Size = new System.Drawing.Size(100, 21);
            this.txt关系.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(72, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "关系：";
            // 
            // txt姓名
            // 
            this.txt姓名.Location = new System.Drawing.Point(118, 27);
            this.txt姓名.Name = "txt姓名";
            this.txt姓名.Size = new System.Drawing.Size(100, 21);
            this.txt姓名.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "姓名：";
            // 
            // 老人入住管理_监护人
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.txt联系电话);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txt住址或工作单位);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txt关系);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt姓名);
            this.Controls.Add(this.label1);
            this.Name = "老人入住管理_监护人";
            this.Size = new System.Drawing.Size(398, 204);
            this.Load += new System.EventHandler(this.老人入住管理_监护人_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt联系电话;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt住址或工作单位;
        private System.Windows.Forms.Label label4;
        private NumberTextBox txt关系;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt姓名;
        private System.Windows.Forms.Label label1;

    }
}
