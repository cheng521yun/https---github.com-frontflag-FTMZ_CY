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

namespace 福田民政.Forms.Work.数据管理.福利中心.老人入住.Tab.监护人
{
    public partial class Edit : EditDlg //Form
    {
        /*
        DB.Stru.事务科.低收入居民_发放记录 _stru = new DB.Stru.事务科.低收入居民_发放记录();

        福田民政.Forms.Work.数据管理.事务科.社会救助.Tab.发放记录 _list = new 福田民政.Forms.Work.数据管理.事务科.社会救助.Tab.发放记录();

        public Edit()
        {
            InitializeComponent();
        }

        private void Edit_Load(object sender, EventArgs e)
        {
            LoadForm();
        }

        #region 属性

        public DB.Stru.事务科.低收入居民_发放记录 stru
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
            Caption = "低收入居民 发放记录";

            uc.stru = _stru;
            _list.Find();
        }

        protected override void Save()
        {
            _stru = uc.stru;

            bool result = BL.事务科.低收入居民_发放记录.Save(_stru);

            if (result)
                FF.Ctrl.MsgBox.Show(Def.Str.Msg.Save_OK);
            else
                FF.Ctrl.MsgBox.Show(Def.Str.Msg.Save_Err);

            _list.Find();

        }
        public void Delete(string id)
        {
            uc.stru.ID = id;
            Delete();
        }
        protected override void Delete()
        {
            _stru = uc.stru;

            if (_stru.IsNotValid())
            {
                return;
            }
            bool result = BL.事务科.低收入居民_发放记录.Delete(_stru);

            if (result)
                FF.Ctrl.MsgBox.Show(Def.Str.Msg.Delete_OK);
            else
                FF.Ctrl.MsgBox.Show(Def.Str.Msg.Delete_Err);

            _list.Find();
        }*/
    }
}
