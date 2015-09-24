﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FrontFlag.Control;

namespace 福田民政.Forms.Work.数据管理.基层科.数据统计.Tab
{
    public partial class 社区基础服务设施情况 : XForm
    {

        public 社区基础服务设施情况()
        {
            InitializeComponent();
        }

        private void 成员信息_Load( object sender, EventArgs e )
        {
            LoadForm();
        }

        private void LoadForm()
        {
            InitList();
            InitTool();
        }

        private void InitList()
        {
            fl.SetGrdFld( new COLHEAD[] 
                { 
                    new COLHEAD("面积统计（按街道）	","",160),
                    new COLHEAD("面积统计（按社区）	","",160),
                } );

            fl.strIDCol = "ID";
        }

        private void InitTool()
        {
            //ButnTool fTool = new ButnTool();
            //Form f = (Form)fTool;
            //fl.AddFootPnl( ref f );

            //fTool.dlgtAdd = Add;
            //fTool.dlgtDelete = Delete;

        }

        private void Add()
        {
            MessageBox.Show("Add");
        }

        private void Delete()
        {
            MessageBox.Show( "Delete" );
        }
    }
}
