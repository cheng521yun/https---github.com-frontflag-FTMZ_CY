﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Def.Delegate;
using FrontFlag.Control;
using Global;

namespace 福田民政.Forms.数据管理.民管局
{
    public partial class Wnd : XForm
    {
        public Wnd()
        {
            InitializeComponent();
        }

        #region Event

        private void Wnd_Load( object sender, EventArgs e )
        {
            LoadForm();
        }

        #endregion

        private void LoadForm()
        {
            InitForm();
            Act.Entry_数据管理_民管局 += Call;
        }

        void InitForm()
        {
            AddFormList( typeof( 福田民政.Forms.数据管理.民管局.登记备案 ), Def.FormId.数据管理_民管局_登记备案 );
            AddFormList( typeof( 福田民政.Forms.数据管理.民管局.台账列表 ), Def.FormId.数据管理_民管局_台账列表 );
            AddFormList( typeof( 福田民政.Forms.数据管理.民管局.台账统计 ), Def.FormId.数据管理_民管局_台账统计 );

        }

        private void Call( object obj )
        {
            ShowDefaultForm();
        }

        private void ShowDefaultForm()
        {
            GL.Command.Excute( Def.CommandId.数据管理_民管局_登记备案 );
        }

    }
}
