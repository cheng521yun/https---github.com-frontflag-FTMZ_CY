﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Buss;
using FrontFlag;
using UI.Dlg;

namespace 福田民政.Forms.Work.数据管理.基层科.民主管理
{
    public partial class FEdit : EditDlg
    {
        DB.Stru.基层科.民主管理信息管理 _stru = new DB.Stru.基层科.民主管理信息管理();  //todo  change 民主管理

        public FEdit()
        {
            InitializeComponent();
        }

        private void Edit_Load( object sender, EventArgs e )
        {
            LoadForm();
        }

        #region 属性

        public DB.Stru.基层科.民主管理信息管理 stru
        {
            set
            {
                _stru = value;
            }
            get
            {
                return _stru;
            }
        }

        #endregion

        private void LoadForm()
        {
            Caption = "民主管理";

            uc.stru = _stru;
        }

        protected override void Save()
        {
            _stru = uc.stru;

            _stru = BL.基层科.民主管理信息管理.Save(_stru);

            if ( _stru.IsValid() )
                FF.Ctrl.MsgBox.Show( Def.Str.Msg.Save_OK );
            else
                FF.Ctrl.MsgBox.Show( Def.Str.Msg.Save_Err );
        }

        protected override void Delete()
        {
            _stru = uc.stru;

            if ( _stru.IsNotValid() )
            {
                return;
            }

            _stru = BL.基层科.民主管理信息管理.Delete(_stru);

            if ( !_stru.IsValid() )
                FF.Ctrl.MsgBox.Show( Def.Str.Msg.Delete_OK );
            else
                FF.Ctrl.MsgBox.Show( Def.Str.Msg.Delete_Err );
        }
    }
}
