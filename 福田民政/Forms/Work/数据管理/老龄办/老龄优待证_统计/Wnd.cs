﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FrontFlag.Control;
using UI.Dlg;
using 福田民政.Forms.Comm.人员;
using 福田民政.Forms.Work.数据管理.老龄办.老龄优待证_统计;

namespace 福田民政.Forms.数据管理.老龄办
{
    public partial class 老龄优待证_统计 : WorkForm
    {
        Find fFind = new Find();
        列表 fList = new 列表();


        public 老龄优待证_统计()
        {
            InitializeComponent();
        }

        protected override void Init()
        {
            strCaption = "老龄优待证 统计";
            Def.Act.Menu.Entry_数据管理_老龄办_老龄优待证_统计 += Call;

            InitForm();
            InitAction();
        }

        private void InitForm()
        {
            fFind.SetParent( pnlTop );
            
            fList.SetParent( pnlList );
        }

        private void InitAction()
        {
            //Def.Act.数据管理.老龄办.老龄津贴_发放 += Call老龄津贴_发放;
        }

        public override void ShowWnd()
        {
            Show();

            fList.统计();
        }

    }
}
