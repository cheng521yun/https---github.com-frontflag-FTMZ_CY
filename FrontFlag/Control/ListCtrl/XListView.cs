//////////////////////////////////////////////////////////////////////////
//
//  ʹ��˵��: ��Ҫ���� ListView.Init () �� �Ա㼤����Ϣ��Ӧ������
//
//////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.ComponentModel;



using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

using Excel = Microsoft.Office.Interop.Excel;  

namespace FrontFlag.Control
{
    public class XListView : System.Windows.Forms.ListView
    {
        DEBUG Debug = new DEBUG (); 

        #region ί��

        public struct SENDMSGDATA
        {
            public enum FunNo { HoverCell } ;
            public int nFunNO;
            public string strMsg;
            public ListViewItem Item;
            public int nParam1;
            public int nParam2;
        } ;
        public delegate void entSendMsg ( SENDMSGDATA SendMsgData );
        public event entSendMsg SendMsg = null;   

        #endregion

        #region Windows Param

        [StructLayoutAttribute ( LayoutKind.Sequential )]
        public struct LV_ITEM
        {
            public UInt32 mask;
            public Int32 iItem;
            public Int32 iSubItem;
            public UInt32 state;
            public UInt32 stateMask;
            public String pszText;
            public Int32 cchTextMax;
            public Int32 iImage;
            public IntPtr lParam;
        }

        public const Int32 LVM_FIRST = 0x1000;
        public const Int32 LVM_GETITEM = LVM_FIRST + 5;
        public const Int32 LVM_SETITEM = LVM_FIRST + 6;
        public const Int32 LVIF_TEXT = 0x0001;
        public const Int32 LVIF_IMAGE = 0x0002;

        public const int LVW_FIRST = 0x1000;
        public const int LVM_GETEXTENDEDLISTVIEWSTYLE = LVW_FIRST + 54;

        public const int LVS_EX_GRIDLINES = 0x00000001;
        public const int LVS_EX_SUBITEMIMAGES = 0x00000002;
        public const int LVS_EX_CHECKBOXES = 0x00000004;
        public const int LVS_EX_TRACKSELECT = 0x00000008;
        public const int LVS_EX_HEADERDRAGDROP = 0x00000010;
        public const int LVS_EX_FULLROWSELECT = 0x00000020; // applies to report mode only
        public const int LVS_EX_ONECLICKACTIVATE = 0x00000040;

        #endregion

        bool _bCanSort = false;
        ListViewColumnSorter _sorter = new ListViewColumnSorter ();

        bool _bFilterColumnClick = false;

        //ArrayList _aryColHide = new ArrayList () ;      //��¼��Ҫ���ص��С�
        List<int> _lstColHide = new List<int>();          //��¼��Ҫ���ص��С�
        List<int> _listColNotW = new List<int>();         //��¼Xml�����п��=0���С�

        public FLDCOL FldCol = new FLDCOL () ;          //����ֱ����DataTable����ֵ��

        string _strXmlFile = "";                        //�����п�ȵ��ļ����ơ�
        string _strListName = "";                       //ÿ���б���Ҫ��XML�ļ������Լ���Ψһ�����ơ�
        bool   _bAllowSaveFldWidth = false;             //true=�����п�Ⱥ������ֵ���Զ����浽xml�ļ��С�

        string [] _strOrderValueType = null;
        int [] _nColWidths = null;                       //����ÿ�еĿ��,�Ա�HideCol/ShowCol֮���������ָ�ԭ�����п�

        public XListView()
        {
            Init ();
        }

        #region ����

        public List<int> listColNotW
        {
            get { return _listColNotW; }
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x4E: // WM_NOTIFY
                    //if (!this.HandleNotify(ref m))
                        base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private bool HandleNotify(ref Message m)
        {
            bool isMsgHandled = false;

            const int HDN_FIRST = (0 - 300);
            const int HDN_ITEMCHANGINGA = (HDN_FIRST - 0);
            const int HDN_ITEMCHANGINGW = (HDN_FIRST - 20);

            API.NMHDR nmhdr = (API.NMHDR)m.GetLParam(typeof(API.NMHDR));

            if (nmhdr.code == HDN_ITEMCHANGINGA || nmhdr.code == HDN_ITEMCHANGINGW)
            {
                //Debug.Log("in nmhdr.code == HDN_ITEMCHANGINGW");

                API.NMHEADER nmheader = (API.NMHEADER)m.GetLParam(typeof(API.NMHEADER));
                if (nmheader.iItem >= 0 && nmheader.iItem < this.Columns.Count)
                {
                    API.HDITEM hditem = (API.HDITEM)Marshal.PtrToStructure(
                        nmheader.pHDITEM, typeof(API.HDITEM));
                    //OLVColumn column = this.GetColumn(nmheader.iItem);
                    //if ( IsOutsideOfBounds ( nmheader.iItem ) )

                    //Debug.Log(String.Format("--- HandleNotify. col={0}", (int)nmheader.iItem));

                    //if (_aryColHide.Contains((int)nmheader.iItem))
                    if (_lstColHide.Contains((int)nmheader.iItem))
                    {
                        Columns[(int)nmheader.iItem].Width = 0;

                        m.Result = (IntPtr)1; // prevent the change
                        isMsgHandled = true;

                        //Debug.Log("**** HandleNotify");
                    }
                }
            }

            return isMsgHandled;
        }

        //private bool IsOutsideOfBounds(int col)
        //{
        //    // Check the mask to see if the width field is valid,
        //    if ((hditem.mask & 1) != 1)
        //        return false;

        //    // Now check that the value is in range
        //    //return ( hditem.cxy < col.MinimumWidth ||
        //    //       ( col.MaximumWidth != -1 && hditem.cxy > col.MaximumWidth ) );

        //    foreach (int nCol in _aryColHide)
        //    {
        //        if (e.ColumnIndex == nCol && e.NewWidth > 0)
        //        {
        //            Debug.Log("**** Do Hide col");

        //            e.NewWidth = 0;
        //            e.Cancel = true;
        //        }
        //    }
        //}

        public void Init ()
        {
            this.View = System.Windows.Forms.View.Details;
            MultiSelect = false;

            //AllowOrder ( false );     //Ĭ��������
            AllowOrder ( true );        //Ĭ��������
 
            //�趨������.
            ListViewItemSorter = _sorter;
            _sorter.Order = SortOrder.Ascending;

            //��Ϣ��Ӧ������
            ColumnClick += new System.Windows.Forms.ColumnClickEventHandler ( this.OnColumnClick );
            ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler ( this.OnColWidthChanging );
            ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler ( this.OnItemMouseHover );
            ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler ( this.OnColumnWidthChanged );

            //
            SetStyle ();

            RestoreColWidth ();   

            //
            SetDefaultAllOrderType ();
        }

        public void SetMultSetlst(bool bFlag)
        {
            MultiSelect = bFlag;
        }

        /// <summary>
        /// ����ÿ�е�Ĭ���������͡� Ĭ�����ַ����͡�
        /// </summary>
        public void SetDefaultAllOrderType ( )
        {
            int nNum = Columns.Count ;
            _strOrderValueType = new string [ nNum ];

            for ( int i = 0 ; i < nNum ; i++ )
                _strOrderValueType [ i ] = ListViewColumnSorter.DefValueType_String;
        }

        public void SetColOrderType ( int nCol , string strValueType )
        {
            if ( nCol >= 0 && nCol < Columns.Count && nCol < _strOrderValueType.Length )
                _strOrderValueType [ nCol ] = strValueType;
        }

        //��֪��Ϊʲĩ���Colum��ʱ�� �ᷢ2�� ColumnClick ��Ϣ����,����ֻ���ü��������˵���2�ε���Ϣ.
        public bool FilterColumnClick
        {
            set
            {
                _bFilterColumnClick = value;
            }

            get
            {
                return _bFilterColumnClick;
            }
        }

        public void AllowOrder ( bool bFlag )
        {
            _bCanSort = bFlag;

            if ( _bCanSort )
            {
                HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
                _sorter.Order = SortOrder.Ascending;
            }
            else
            {
                HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
                _sorter.Order = SortOrder.None;
            }
        }

        public void AllowSaveColWidth ( bool bFlag , string strXmlFile , string strListName )
        {
            //������������Ϊinit()����û��Ч���������п��Ա�������ColumnWidthChanged����������Ա�ǿ��ִ�С�
            ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.OnColumnWidthChanged);
            ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.OnColWidthChanging);


            _bAllowSaveFldWidth = bFlag;
            _strXmlFile = strXmlFile;
            _strListName = strListName;

            if ( _strXmlFile.Trim () == "" || strListName.Trim() == "" )
                _bAllowSaveFldWidth = false;
        }

        public void SetStyle ()      
        {
            System.Windows.Forms.Message m = new Message ();
            m.HWnd = this.Handle;
            m.Msg = LVM_GETEXTENDEDLISTVIEWSTYLE;
            m.LParam = ( IntPtr ) ( LVS_EX_GRIDLINES | LVS_EX_FULLROWSELECT | LVS_EX_SUBITEMIMAGES );
            m.WParam = IntPtr.Zero;
            this.WndProc ( ref m );
        }

        public void Order ( int colNum , SortOrder so )
        {
            _sorter.SortColumn = colNum;
            _sorter.Order = so ;
            Sort ();
        }

        public void Order ( int colNum , SortOrder so , string strValueType )
        {
            _sorter.SortColumn = colNum;
            _sorter.Order = so;
            _sorter.SortValueType = strValueType ; 
            Sort ();
        }

        //��������ͷ�¼���Ӵ���
        long nColumnClick = 0;
        private void OnColumnClick ( object sender , System.Windows.Forms.ColumnClickEventArgs e )
        {
            if ( !_bCanSort )
                return;

            if ( _bFilterColumnClick )
            {
                if ( nColumnClick++ % 2 == 1 )          //��֪��Ϊʲĩ���Colum��ʱ�� �ᷢ2�� ColumnClick ��Ϣ����,����ֻ���ü��������˵���2�ε���Ϣ.
                    return;

                if ( nColumnClick > 1000 )
                    nColumnClick = 0;
            }

            //������������͡�
            if ( e.Column < _strOrderValueType.Length )
                 _sorter.SortValueType = _strOrderValueType [ e.Column ];
            else   //Ĭ�����ַ������ͱȽϡ�
                _sorter.SortValueType = ListViewColumnSorter.DefValueType_String;

            //
            if ( e.Column == _sorter.SortColumn )
            {
                if ( _sorter.Order == SortOrder.Ascending )
                    _sorter.Order = SortOrder.Descending;
                else
                {
                    if ( _sorter.Order == SortOrder.Descending )
                        _sorter.Order = SortOrder.Ascending;
                    else
                        return;
                }
            }
            else
            {
                _sorter.SortColumn = e.Column;
            }

            Sort ();
        }

        #region ��ֵ

        public bool SetColFld ( string [ ] strsCol , string [ ] strsFld )
        {
            ArrayList ary = new ArrayList () ;  
            ary = GetColNameAry () ;

            foreach ( string str in strsCol )
            {
                if ( ! ary.Contains ( str ) )
                {
                    FF.Ctrl.MsgBox.ShowWarn ( String.Format ( "XListView.SetColFld(): �в�����ָ��������: {0} !" , str ) );
                    return false;
                }
            }

            return FldCol.Set ( strsCol , strsFld );
        }

        public bool DataTable2List ( ref DataTable dt )
        {
            FUN Fun = new FUN () ;

            Items.Clear ();

            if ( !FldCol.IsValid () || SQL.IsNotValid ( ref dt ) )
                return false;

            bool bRet = true;
            int ColMax = Columns.Count;
            string [ ] strsSubItem = new string [ ColMax ];
            int LineNum = dt.Rows.Count;
            ListViewItem [ ] lis = new ListViewItem [ LineNum ];
            Type t;
            DateTime dat;

            BeginUpdate ();
            SuspendLayout ();

            string strFld , strText;
            int i = 0 , j = 0;

            try 
            {
                foreach ( DataRow dr in dt.Rows )
                {
                    j = 0;

                    foreach ( ColumnHeader hd in Columns )
                    {
                        strFld = FldCol.GetFldName ( hd.Text );

                        if ( strFld == String.Empty )
                        {
                            strsSubItem [ j++ ] = "";       //����Ҫ��dt�ж����col, ����Ϊ��
                        }
                        else
                        {
                            t = dr [ strFld ].GetType ();

                            if ( t == typeof ( System.DateTime ) )
                            {
                                dat = FF.Fun.MyConvert.Str2Date ( dr [ strFld ].ToString () );
                                if( dat.Year == 1900 )
                                    //strsSubItem[j++] = String.Empty;   //2013-10-09 ����������������ʱ����
                                    strsSubItem[j++] = dat.ToString( "yyyy-MM-dd HH:mm" );   //ʹ��yyyy����yy, �ɱ���1900-01-01 ���ı�� 00-01-01�� ����ϵͳ�ᵱ�� 2000-01-01 ��
                                else
                                    strsSubItem [ j++ ] = dat.ToString ( "yy-MM-dd HH:mm" );
                            }
                            else if ( t == typeof ( System.Decimal ) )  //decimal ���� Money����
                            {
                                strText = dr [strFld].ToString ().Trim ();
                                strsSubItem [j++] = Fun.MyConvert.Decimal2StrDot4 ( strText ); //���Ƶ�2λС���㡣
                            }
                            else   //�������͵��ֶ� ��Ĭ��Ϊ�ı���ʽ��
                            {
                                strText = dr [strFld].ToString ().Trim ();
                                strsSubItem [j++] = strText;
                            }
                        }
                    }

                    lis [ i++ ] = new ListViewItem ( strsSubItem );
                }

                Items.AddRange ( lis );

            }
            catch ( Exception e )
            {
                bRet = false;
                string strMsg = String.Format ( "DataTable2List() Err!\nTable:{0}\n{1}" , dt.TableName , e.Message );
                FF.Ctrl.MsgBox.ShowWarn ( strMsg );
            }

            ResumeLayout ();
            EndUpdate ();

            return bRet;
        }
        public bool List2DataTable ( ref DataTable dt )
        {
            dt.Rows.Clear () ;

            bool bRet = true;
            string strFld , strText;
            int n = 0;

            try
            {
                foreach ( ListViewItem li in Items )
                {
                    SQL.AddNewRow ( ref dt );
                    DataRow dr = dt.Rows [ n ];

                    foreach ( ColumnHeader hd in Columns )
                    {
                        strFld = FldCol.GetFldName ( hd.Text );
                        if ( strFld == String.Empty )
                            continue;

                        strText = li.SubItems [ hd.Index ].Text;
                        dr [ strFld ] = strText;
                    }

                    n++;
                }
            }
            catch ( Exception e )
            {
                bRet = false;
                string strMsg = String.Format ( "List2DataTable() Err!\nTable:{0}\n{1}" , dt.TableName ,  e.Message );
                FF.Ctrl.MsgBox.ShowWarn ( strMsg );
            }

            return bRet;
        }
        /*public bool List2DataTable ( ref DataTable dt )
        {
            bool bRet = true;
            string strFld , strText;
            int n = 0;

            try
            {
                foreach ( ListViewItem li in Items )
                {
                    SQL.AddNewRow ( ref dt );
                    DataRow dr = dt.Rows [ n ];

                    foreach ( ColumnHeader hd in Columns )
                    {
                        strFld = FldCol.GetFldName ( hd.Text );
                        if ( strFld == String.Empty )
                            continue;

                        strText = li.SubItems [ hd.Index ].Text;
                        dr [ strFld ] = strText;
                    }

                    n++;
                }
            }
            catch ( Exception e )
            {
                bRet = false;
                string strMsg = String.Format ( "List2DataTable() Err!\nTable:{0}\n{1}" , dt.TableName , e.Message );
                FF.Ctrl.MsgBox.ShowWarn ( strMsg );
            }

            return bRet;
        }*/

        public bool DataRow2List ( int nLineNo , ref DataRow dr )
        {
            if ( nLineNo < 0 || nLineNo >= Items.Count )
                return false;

            if ( !FldCol.IsValid () || dr == null )
                return false;

            bool bRet = true;

            int ColMax = Columns.Count;
            string [ ] strsSubItem = new string [ ColMax ];

            BeginUpdate ();
            SuspendLayout ();

            string strFld , strText;
            int i = 0 , j = 0;

            try
            {
                foreach ( ColumnHeader hd in Columns )
                {
                    strFld = FldCol.GetFldName ( hd.Text );
                    if ( strFld == String.Empty )
                    {
                        strsSubItem [ j++ ] = "";       //����Ҫ��dr�ж����col, ����Ϊ��
                    }
                    else
                    {
                        strText = dr [ strFld ].ToString ().Trim ();
                        strsSubItem [ j++ ] = strText;
                    }
                }

                ListViewItem li = new ListViewItem ( strsSubItem );

                Items [ nLineNo ] = li;
            }
            catch ( Exception e )
            {
                bRet = false;
                string strMsg = String.Format ( "DataRow2List() Err!\nTable:{0}\n{1}" , dr.Table.TableName , e.Message ); 
                FF.Ctrl.MsgBox.ShowWarn ( strMsg ) ;
            }
            
            ResumeLayout ();
            EndUpdate ();

            return bRet;
        }
        public bool DataRow2List(int nLineNo, DataRow dr)
        {
            if (nLineNo < 0 || nLineNo >= Items.Count)
                return false;

            if (!FldCol.IsValid() || dr == null)
                return false;

            bool bRet = true;

            int ColMax = Columns.Count;
            string[] strsSubItem = new string[ColMax];

            BeginUpdate();
            SuspendLayout();

            string strFld, strText;
            int i = 0, j = 0;

            try
            {
                foreach (ColumnHeader hd in Columns)
                {
                    strFld = FldCol.GetFldName(hd.Text);
                    if (strFld == String.Empty)
                    {
                        strsSubItem[j++] = "";       //����Ҫ��dr�ж����col, ����Ϊ��
                    }
                    else
                    {
                        strText = dr[strFld].ToString().Trim();
                        strsSubItem[j++] = strText;
                    }
                }

                ListViewItem li = new ListViewItem(strsSubItem);

                Items[nLineNo] = li;
            }
            catch (Exception e)
            {
                bRet = false;
                string strMsg = String.Format("DataRow2List() Err!\nTable:{0}\n{1}", dr.Table.TableName, e.Message);
                FF.Ctrl.MsgBox.ShowWarn(strMsg);
            }

            ResumeLayout();
            EndUpdate();

            return bRet;
        }

        public bool List2DataRow ( int nLineNo , ref DataRow dr )
        {
            if ( nLineNo < 0 || nLineNo >= Items.Count )
                return false;

            if ( !FldCol.IsValid () )
                return false;

            bool bRet = true;
            string strFld , strText;
            int n = 0;

            try
            {
                foreach ( ColumnHeader hd in Columns )
                {
                    strFld = FldCol.GetFldName ( hd.Text );
                    if ( strFld == String.Empty )
                        continue;

                    strText = Items[ nLineNo ].SubItems [ hd.Index ].Text;
                    dr [ strFld ] = strText;
                }
            }
            catch ( Exception e )
            {
                bRet = false;
                string strMsg = String.Format ( "List2DataTable() Err!\nTable:{0}\n{1}" , dr.Table.TableName , e.Message );
                FF.Ctrl.MsgBox.ShowWarn ( strMsg );
            }

            return bRet;
        }

        #endregion

        #region ������

        public int GetColIndex ( string strColName )
        {
            foreach ( ColumnHeader hd in Columns )
            {
                if ( hd.Text.Trim () == strColName.Trim () )
                    return hd.Index;
            }
            return -1;
        }

        public void ClearHideCol ()
        {
            //_aryColHide.Clear ();
            _lstColHide.Clear();
        }

        public void HideCol ( string strColName )
        {
            int nCol = GetColIndex ( strColName );
            HideCol ( nCol );
        }
        public void ShowCol ( string strColName , int nColW )
        {
            int nCol = GetColIndex ( strColName );

            ShowCol(nCol);   

            if ( nColW <= 0 )
                nColW = 100;

            SetColWidth(nCol, nColW);   
        }
        public void ShowCol ( string strColName )
        {
            try
            {
                int nCol = GetColIndex ( strColName );

                ShowCol ( strColName , _nColWidths [ nCol ] );
            }
            catch ( Exception e )
            {
                ShowCol ( strColName , 100 );
            }
        }

        public void HideCol ( int nCol )
        {
            if ( nCol < 0 || nCol >= this.Columns.Count )
                return ;

            //_aryColHide.Add ( nCol ) ;
            _lstColHide.Add(nCol);
            SetColWidth ( nCol , 0 );
        }
        public void ShowCol ( int nCol , int nColW )
        {
            if ( nCol < 0 || nCol >= this.Columns.Count )
                return;

            try
            {
                //if ( _aryColHide.Contains ( nCol ) )
                //    _aryColHide.Remove ( nCol );
                if (_lstColHide.Contains(nCol))
                    _lstColHide.Remove(nCol);
            }
            catch ( Exception e )
            {
                MessageBox.Show ( e.Message );
            }

            SetColWidth ( nCol , nColW );
        }
        public void ShowCol ( int nCol )
        {
            try
            {
                ShowCol ( nCol , _nColWidths [ nCol ] );
            }
            catch ( Exception e )
            {
                ShowCol ( nCol , 100 ) ;
            }
        }

        public void SetColWidth ( string strColName , int Width )
        {
            int nCol = GetColIndex ( strColName );
            SetColWidth ( nCol , Width );
        }
        public void SetColWidth ( int nCol , int Width )
        { 
            if ( nCol < 0 || nCol >= this.Columns.Count || Width < 0 )
                return ;

            //FF.Debug.Log(String.Format("SetColWidth. Width={0}", Width));

            Columns [ nCol ].Width = Width;
        }

        //�ϲ������е�������
        public bool CombinColToDest ( string strColOrg1 , string strColOrg2 , string strColDest , string strSplit )
        {
            int nCol1 = GetColIndex ( strColOrg1 );
            int nCol2 = GetColIndex ( strColOrg2 );
            int nCol3 = GetColIndex ( strColDest );

            if ( nCol1 < 0 || nCol2 < 0 || nCol3 < 0 )
                return false ;
            
            string str1 , str2 ;
            foreach ( ListViewItem li in Items )
            {
                str1 = li.SubItems [ nCol1 ].Text;
                str2 = li.SubItems [ nCol2 ].Text;
                li.SubItems [ nCol3 ].Text = str1 + strSplit + str2;
            }

            return true;
        }
        public bool CombinColToDest ( string strColOrg1 , string strColOrg2 , string strColDest )
        {
            return CombinColToDest ( strColOrg1 , strColOrg2 , strColDest , " " );
        }

        //�ѵ�һ�кϲ����ڶ�����
        public bool CombinCol ( string strCol1 , string strCol2 , string strSplit )
        {
            return CombinColToDest ( strCol1 , strCol2 , strCol2 , strSplit );
        }
        public bool CombinCol ( string strCol1 , string strCol2 )
        {
            return CombinCol ( strCol1 , strCol2 , "" ) ;
        }

        //�϶�Head��ʱ�򣬴�����Ҫ���ص��С�
        void OnColWidthChanging ( object sender , ColumnWidthChangingEventArgs e )
        {
            //Debug.Log ( "===== in OnColWidthChanging()" );

            //foreach (int nCol in _aryColHide)
            foreach ( int nCol in _lstColHide )
            {
                //Debug.Log("foreach nCol in _aryColHide�� ");
                //Debug.Log(String.Format ( "e.ColumnIndex == nCol; {0} , {1}", e.ColumnIndex , nCol ));
                //Debug.Log( String.Format ("e.NewWidth > 0: {0}", e.NewWidth ) );

                if (e.ColumnIndex == nCol && e.NewWidth > 0)
                {
                    //Debug.Log("**** Do Hide col");

                    e.NewWidth = 0;
                    e.Cancel = true;
                }
            }
        }

        //����ƶ����ĸ�Item�ϡ�//��һ�����ƶ���������List�հ״��ƶ������ἤ�������Ϣ��
        void OnItemMouseHover ( object sender , ListViewItemMouseHoverEventArgs e )
        {
            if ( SendMsg == null )
                return;

            SENDMSGDATA data = new SENDMSGDATA ();
            data.nFunNO = ( int ) SENDMSGDATA.FunNo.HoverCell;
            data.Item = e.Item;

            SendMsg ( data );
        }

        //�����еĿ�Ⱥ� Ҫ��������ֵ��¼�������ļ��У� �Ա��´���ʾListʱ�����Ա��ָ����������ڵĿ�ȡ�
        void OnColumnWidthChanged ( object sender , ColumnWidthChangedEventArgs e )
        {
            #region ��ΪOnColumnWidthChangingû�����ã���ʱ����������������и�������
            foreach (int nCol in _lstColHide)
            {
                if (e.ColumnIndex == nCol && Columns[e.ColumnIndex].Width > 0)
                {
                    Columns[e.ColumnIndex].Width = 0;
                }
            }
            #endregion 

            if ( ! _bAllowSaveFldWidth )
                return;

            //return;   //2014-02-28 ����cancel, ��Ϊ�п�̬��ʾ����ȷ

            string strColName = Columns [ e.ColumnIndex ].Text;
            FF.Xml.FilterInvalidCh ( ref strColName ) ;
            int nWidth = Columns [ e.ColumnIndex ].Width;

            string Param = String.Format ( @"{0}/{1}" , _strListName , strColName );
            FF.Xml.WriteParam ( _strXmlFile , Param , nWidth.ToString () );
        }

        public void RestoreColWidth()
        {
            if ( !_bAllowSaveFldWidth )
                return;

            _listColNotW.Clear();

            string strCol, strValue = "";
            int nWidth = 0;

            _nColWidths = new int[ Columns.Count ];

            int i = 0;
            foreach ( ColumnHeader hd in Columns )
            {
                strCol = hd.Text;
                FF.Xml.FilterInvalidCh( ref strCol );

                string Param = String.Format( @"{0}/{1}", _strListName, strCol );
                FF.Xml.ReadParam( _strXmlFile, Param, ref strValue );

                if ( strValue == "" )
                {
                    _nColWidths[ i++ ] = hd.Width;
                    continue;
                }

                nWidth = FF.Fun.MyConvert.Str2Int( strValue );

                if ( nWidth == 0 )
                    _listColNotW.Add( hd.Index );   //��¼�¿��=0���С�

                if ( nWidth >= 0 )
                {
                    hd.Width = nWidth;
                    _nColWidths[ i++ ] = hd.Width;
                }
            }

        }


        #endregion

        #region ������

        public void SetLineHeight ( int H )
        {
            ImageList imgList = new ImageList ();
            imgList.ImageSize = new System.Drawing.Size ( 1 , H );
            SmallImageList = imgList;
        }

        public void ItemSelectedUp ()
        {
            if ( SelectedIndices.Count <= 0 )
                return;

            int nSel = SelectedIndices [0];
            if ( nSel == 0 )
                return;

            nSel = ItemUp ( nSel );

            SelectedIndices.Clear ();
            if ( nSel >= 0 )
                SelectedIndices.Add ( nSel );

        }

        public void ItemSelectedDown ()
        {
            if ( SelectedIndices.Count <= 0 )
                return;

            int nSel = SelectedIndices [0];
            if ( nSel == Items.Count -  1 )
                return;

            nSel = ItemDown ( nSel );

            SelectedIndices.Clear ();
            if ( nSel >= 0 )
                SelectedIndices.Add ( nSel );
        }

        public int ItemUp ( int nSel )
        {
            if ( nSel < 0 && nSel >= Items.Count )
                return -1;

            if ( nSel == 0 )
                return 0;

            ListViewItem ItemTmp = new ListViewItem ( Items [nSel].SubItems [0].Text );
            int nCount = Items [nSel].SubItems.Count ;
            for ( int i = 1 ; i < nCount ; i ++ )
            {
                ItemTmp.SubItems.Add ( Items [nSel].SubItems [ i ].Text );
            }

            ListViewItem ItemTmp2 =  Items.Insert ( nSel - 1 , ItemTmp );
            Items.RemoveAt ( nSel + 1 );

            return nSel - 1;
        }

        public int ItemDown ( int nSel )
        {
            if ( nSel < 0 && nSel >= Items.Count )
                return -1;

            if ( nSel == Items.Count -1 )
                return Items.Count - 1;

            ListViewItem ItemTmp = new ListViewItem ( Items [nSel].SubItems [0].Text );
            int nCount = Items [nSel].SubItems.Count;
            for ( int i = 1 ; i < nCount ; i++ )
            {
                ItemTmp.SubItems.Add ( Items [nSel].SubItems [i].Text );
            }

            ListViewItem ItemTmp2 = Items.Insert ( nSel + 2 , ItemTmp );
            Items.RemoveAt ( nSel );

            return nSel + 1;
        }

        #endregion

        #region ����Cell

        //
        public void SetCellImg ( int nLine , int nCol , int nImgIndex )
        {
            LV_ITEM lvi = new LV_ITEM ();

            lvi.mask = XListView.LVIF_IMAGE | XListView.LVIF_TEXT;
            lvi.iItem = nLine;
            lvi.iSubItem = nCol;
            lvi.iImage = nImgIndex;
            lvi.pszText = GetCellText ( nLine , nCol );
            SendMessage ( Handle , LVM_SETITEM , 0 , ref lvi );
        }
        public void SetCellImg ( ListViewItem li , int nCol , int nImgIndex )
        {
            int nLine = Items.IndexOf ( li );
            if ( nLine < 0 )
                return;
            SetCellImg ( nLine , nCol , nImgIndex ) ;
        }

        #region Get/Set Text

        public string GetCellText ( int nLine , int nCol )
        {
            if ( nLine < 0 || nLine >= Items.Count || nCol < 0 || nCol >= Columns.Count )
                return "";

            return Items [ nLine ].SubItems [ nCol ].Text;
        }
        public string GetCellText ( int nLine , string strColName )
        {
            int nCol = GetColIndex ( strColName );
            return GetCellText ( nLine , nCol );
        }

        public string GetSelectLineCellText ( int nCol )
        {
            int nLine = GetSelLine ();
            if ( nLine < 0 )
                return "";

            if ( nLine < 0 || nLine >= Items.Count || nCol < 0 || nCol >= Columns.Count )
                return "";

            return Items [ nLine ].SubItems [ nCol ].Text;
        }
        public string GetSelectLineCellText ( string strColName )
        {
            int nCol = GetColIndex ( strColName );
            return GetSelectLineCellText ( nCol );
        }

        public void SetCellText ( int nLine , int nCol , string strText )
        {
            if ( nLine < 0 || nLine >= Items.Count || nCol < 0 || nCol >= Columns.Count )
                return;

            Items [ nLine ].SubItems [ nCol ].Text = strText;
        }
        public void SetCellText ( int nLine , string strColName , string strText )
        {
            int nCol = GetColIndex ( strColName );
            SetCellText ( nLine , nCol , strText );
        }

        public bool SetSelectLineCellText ( int nCol , string strText )
        {
            int nLine = GetSelLine ();
            if ( nLine < 0 )
                return false;

            if ( nLine < 0 || nLine >= Items.Count || nCol < 0 || nCol >= Columns.Count )
                return false;

            Items [ nLine ].SubItems [ nCol ].Text = strText;
            return true;
        }
        public bool SetSelectLineCellText ( string strColName , string strText )
        {
            int nCol = GetColIndex ( strColName );
            return SetSelectLineCellText ( nCol , strText );
        }

        #endregion

        #endregion

        #region Select

        //ѡ����һ��.
        public int GetSelLine ()
        {
            if ( SelectedIndices.Count <= 0 )
                return -1;

            return SelectedIndices [ 0 ];
        }

        //��������һ����.
        public int GetSelCol ()
        {
            Point curPos = PointToClient ( Cursor.Position );
            ListViewItem lvwItem = GetItemAt ( curPos.X , curPos.Y );

            if ( lvwItem != null )
            {
                int W = 0;
                for ( int i = 0 ; i < Columns.Count ; i++ )
                {
                    W += Columns [ i ].Width;
                    if ( W > curPos.X )
                        return i;
                }
            }

            return -1;
        }

        public bool DeleteSelectLine ()
        {
            int nIndex = GetSelLine ();
            if ( nIndex < 0 )
                return false;

            Items.RemoveAt ( nIndex );
            return true;
        }

        public void SelectLastLine ()
        {
            int nLastLine = Items.Count - 1;
            SelectedIndices.Clear ();
            SelectedIndices.Add ( nLastLine );
        }

        //���������������.
        public int AddNewLine ()
        {
            try
            {
                int MaxCol = Columns.Count;
                ListViewItem li = new ListViewItem("");

                for (int i = 1; i < MaxCol; i++)
                    li.SubItems.Add("");

                Items.Add(li);

                SelectedIndices.Clear();
                li.Selected = true;                     //2010-03-14,�����mogulttech��˾��makePO��Save()֮���������Լ��� Try{}�������ⵯ������Ի���

                if (SelectedIndices.Count <= 0)
                    return -1;

                return SelectedIndices[0];
            }
            catch ( Exception e )
            {
                string strErr = e.Message;
                return -1;
            }

        }

        #endregion

        #region Private

        ArrayList GetColNameAry ()
        { 
            ArrayList ary = new ArrayList () ;
            foreach ( ColumnHeader hd in Columns )
                ary.Add ( hd.Text );
            return ary;
        }

        #endregion

        #region Windows API

        [DllImport ( "user32.dll" )]
        public static extern int SendMessage (
            IntPtr hWnd ,	// handle to destination window 
            int Msg ,		// message 
            IntPtr wParam ,	// first message parameter 
            IntPtr lParam	// second message parameter 
            );

        [DllImport ( "user32.dll" )]
        public static extern bool SendMessage (
            IntPtr hWnd ,		// handle to destination window 
            Int32 msg ,			// message 
            Int32 wParam ,
            ref LV_ITEM lParam );// pointer to struct of LV_ITEM

        #endregion


        /// <summary>
        /// ����������DataTable�е����ݵ�Excel�ļ� , �����ͬ�����ݽṹ��ʾ��һ��Excelҳ���С�  
        /// </summary>
        /// <remarks>  
        /// add com "Microsoft Excel 11.0 Object Library"  
        /// using Excel=Microsoft.Office.Interop.Excel;  
        /// using System.Reflection;  
        /// </remarks>  
        /// <param name="dgv">DataTable</param>
        /// <param name="strOutFileName">�趨�������ļ�����</param>
        /// <param name="strsHeadName">�������еĵ�HeadName, ���Ϊnull,�򵼳�ȫ����</param>
        /// <returns></returns>
        public static bool SaveDtToExcel(ArrayList aryParam)
        {
            if (aryParam == null || aryParam.Count == 0)
                return false;

            Excel.Application objExcel = null;
            Excel.Workbook objWorkbook = null;
            Excel.Worksheet objsheet = null;

            string fileNameString = "";     //Excel���ļ����ơ�

            int n = 0;          //������ǰ�ǵڼ������ݽṹ�� 
            int nLine = 1;      //ÿ�����ݽṹ�Ŀ�ʼ������
            int nJG = 5;        //ÿ�����ݽṹ֮�� ���������

            foreach (EXCELPARAM param in aryParam)
            {
                DataTable dt = param.dt;
                string strOutFileName = param.strOutFile;
                string[] strsHeadName = param.strsHeadName;
                string[] strsFldName = param.strsFldName;

                int[] nsLineNo = null;

                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("û�п��Ե��������ݣ�", "��ʾ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                //�����һ�����ݽṹ�� ѡ���ļ����ơ�
                if (n == 0)
                {

                    #region   ��֤�ɲ�����

                    //��������Ի���   
                    SaveFileDialog dlg = new SaveFileDialog();
                    //ĬȻ�ļ���׺   
                    dlg.FileName = strOutFileName;
                    dlg.DefaultExt = "xls ";
                    //�ļ���׺�б�   
                    dlg.Filter = "EXCEL�ļ�(*.XLS)|*.xls ";
                    //ĬȻ·����ϵͳ��ǰ·��   
                    dlg.InitialDirectory = Directory.GetCurrentDirectory();
                    //�򿪱���Ի���   
                    if (dlg.ShowDialog() == DialogResult.Cancel)
                        return false;

                    //�����ļ�·��   
                    fileNameString = dlg.FileName;
                    //��֤strFileName�Ƿ�Ϊ�ջ�ֵ��Ч   
                    if (fileNameString.Trim() == " ")
                        return false;

                    //��֤��fileNameString�������ļ��Ƿ���ڣ��������ɾ����   
                    FileInfo file = new FileInfo(fileNameString);
                    if (file.Exists)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception error)
                        {
                            MessageBox.Show(error.Message, "ɾ��ʧ�� ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    //��������   
                    objExcel = new Microsoft.Office.Interop.Excel.Application();
                    objWorkbook = objExcel.Workbooks.Add(Missing.Value);
                    objsheet = (Excel.Worksheet)objWorkbook.ActiveSheet;
                    //����EXCEL���ɼ�   
                    objExcel.Visible = false;

                }


                //�����������ݵ�����������   
                int rowscount = dt.Rows.Count;
                int colscount = dt.Columns.Count;
                //�����������0   
                if (rowscount <= 0)
                {
                    MessageBox.Show("û�����ݿɹ����� ", "��ʾ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                //�����������0   
                if (colscount <= 0)
                {
                    MessageBox.Show("û�����ݿɹ����� ", "��ʾ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                //���������Դ���65536   
                if (rowscount > 65536)
                {
                    MessageBox.Show("���ݼ�¼��̫��(��಻�ܳ���65536��)�����ܱ��� ", "��ʾ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                //���������Դ���255   
                if (colscount > 255)
                {
                    MessageBox.Show("���ݼ�¼����̫�࣬���ܱ��� ", "��ʾ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }


                    #endregion

                #region

                //ȡ��Ҫ�������е�HeadName.
                //���û��ָ���ض����У���Ĭ��ת�� �ɼ��� ���� ��HeadName ���С����ؼ��в��õ������ؼ���һ��û��HeadName��
                if (strsHeadName == null)
                {
                    MessageBox.Show("û��ָ���ֶ����ƣ�", "��ʾ ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                //ȡ������Ҫִ�е��У��������û�������Ҫ���������У�����ͨ��checkboxѡ��Ҫ�������У�
                //���û��ָ���ض����У���Ĭ��ת��ȫ������
                if (nsLineNo == null)
                {
                    nsLineNo = new int[dt.Rows.Count];
                    for (int row = 0; row <= dt.Rows.Count - 1; row++)
                    {
                        nsLineNo[row] = row;
                    }
                }

                #endregion


                try
                {
                    //��Excel��д����ı�ͷ   
                    Excel.Range rag = objsheet.get_Range(objsheet.Cells[nLine, 1], objsheet.Cells[nLine, strsHeadName.Length]);
                    rag.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    int displayColumnsCount = 1;
                    foreach (string strHeadName in strsHeadName)
                    {
                        objExcel.Cells[nLine, displayColumnsCount] = strHeadName;
                        displayColumnsCount++;
                    }

                    //����ÿ�е����ݸ�ʽ��
                    int nRow1 = nLine + 1, nRow2 = nRow1 + nsLineNo.Length;
                    int nColSel = 1;
                    foreach (string strFldName in strsFldName)
                    {
                        rag = objsheet.get_Range(objsheet.Cells[nRow1, nColSel], objsheet.Cells[nRow2, nColSel]);

                        switch (dt.Columns[strFldName].DataType.ToString())
                        {
                            case "System.String":
                                rag.NumberFormatLocal = "@";
                                break;

                            case "System.DateTime":
                                rag.NumberFormatLocal = "yyyy-MM-dd HH:mm";
                                break;

                            //���Ը����Լ�����Ҫ��չ��

                            default:
                                //rag.NumberFormatLocal = "G/ͨ�ø�ʽ";
                                break;
                        }

                        nColSel++;
                    }

                    //��Excel����������д�����е�����   
                    int nCol, row;
                    int nExcelCol, nExcelRow = nLine + 1; //Execl�����ж���1��ʼ���㡣��һ�������������ݴӵڶ��п�ʼд�롣

                    for (int i = 0; i <= nsLineNo.Length - 1; i++)
                    {
                        row = nsLineNo[i];
                        nExcelCol = 1;              //Execl�����ж���1��ʼ���㡣��һ�������������ݴӵڶ��п�ʼд�롣

                        foreach (string strFldName in strsFldName)
                        {
                            try
                            {
                                objExcel.Cells[nExcelRow, nExcelCol++] = dt.Rows[row][strFldName].ToString().Trim();
                            }
                            catch (Exception)
                            {
                            }
                        }

                        nExcelRow++;
                    }

                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message, "���� ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                //
                n++;
                nLine += nsLineNo.Length + nJG;
            }


            //�����ļ�   
            objWorkbook.SaveAs(fileNameString, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Excel.XlSaveAsAccessMode.xlShared, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value);


            //�ر�ExcelӦ��   
            if (objWorkbook != null) objWorkbook.Close(Missing.Value, Missing.Value, Missing.Value);
            if (objExcel.Workbooks != null) objExcel.Workbooks.Close();
            if (objExcel != null) objExcel.Quit();

            objsheet = null;
            objWorkbook = null;
            objExcel = null;


            MessageBox.Show(fileNameString + "\n\n�������! ", "��ʾ ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }
 
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class EXCELPARAM
    {
        public DataTable dt = new DataTable();
        public string strOutFile = "";
        public string[] strsHeadName = null;
        public string[] strsFldName = null;

        public string strDecimalFormat = String.Empty;   //С�����ʽ������String.Empty��Ĭ�ϴ����"0.00"

    }
     
    #region ListViewColumnSorter

    /// <summary>
    /// �̳���IComparer
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        static public string DefValueType_String = "String";
        static public string DefValueType_Num = "Number";
        static public string DefValueType_Date = "Date";

        string _strValueType = "String";    //ָ��Ҫ���������ʲĩ���ͣ� ������ "String","Num".

        /// <summary>
        /// ָ�������ĸ�������
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// ָ������ķ�ʽ
        /// </summary>
        private SortOrder OrderOfSort;
        /// <summary>
        /// ����CaseInsensitiveComparer�����
        /// �μ�ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.2052/cpref/html/frlrfSystemCollectionsCaseInsensitiveComparerClassTopic.htm
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// ���캯��
        /// </summary>
        public ListViewColumnSorter()
        {
            // Ĭ�ϰ���һ������
            ColumnToSort = 0;

            // ����ʽΪ������
            OrderOfSort = SortOrder.None;

            // ��ʼ��CaseInsensitiveComparer�����
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// ��дIComparer�ӿ�.
        /// </summary>
        /// <param name="x">Ҫ�Ƚϵĵ�һ������</param>
        /// <param name="y">Ҫ�Ƚϵĵڶ�������</param>
        /// <returns>�ȽϵĽ��.�����ȷ���0�����x����y����1�����xС��y����-1</returns>
        public int Compare( object x, object y )
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // ���Ƚ϶���ת��ΪListViewItem����
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            if ( ColumnToSort < 0 || ColumnToSort >= listviewX.SubItems.Count || ColumnToSort >= listviewY.SubItems.Count )
                return 0;

            // �Ƚ�
            if ( _strValueType == DefValueType_Num )
            {
                //double f1 = FF.Fun.MyConvert.Str2Double ( listviewX.SubItems [ ColumnToSort ].Text );
                //double f2 = FF.Fun.MyConvert.Str2Double ( listviewY.SubItems [ ColumnToSort ].Text );

                //Ҳ�������ڱȽ� "1234 USD" �������ִ�
                string str1 = listviewX.SubItems[ ColumnToSort ].Text;
                string str2 = listviewY.SubItems[ ColumnToSort ].Text;

                string[] strs1 = str1.Split( new char[] { ' ' } );
                if ( strs1.Length > 1 )
                    str1 = strs1[ 0 ];

                string[] strs2 = str2.Split( new char[] { ' ' } );
                if ( strs2.Length > 1 )
                    str2 = strs2[ 0 ];

                double f1 = FF.Fun.MyConvert.Str2Double( str1 );
                double f2 = FF.Fun.MyConvert.Str2Double( str2 );

                if ( f1 > f2 )
                    compareResult = 1;
                else if ( f1 == f2 )
                    compareResult = 0;
                else
                    compareResult = -1;
            }
            else if ( _strValueType == DefValueType_Date )
            {
                DateTime d1 = FF.Fun.MyConvert.Str2Date( listviewX.SubItems[ ColumnToSort ].Text );
                DateTime d2 = FF.Fun.MyConvert.Str2Date( listviewY.SubItems[ ColumnToSort ].Text );

                if ( d1 > d2 )
                    compareResult = 1;
                else if ( d1 == d2 )
                    compareResult = 0;
                else
                    compareResult = -1;
            }
            else  //Ĭ�ϵĶ����� "Char"�ı����� �Ƚ�
                compareResult = ObjectCompare.Compare( listviewX.SubItems[ ColumnToSort ].Text, listviewY.SubItems[ ColumnToSort ].Text );

            // ��������ıȽϽ��������ȷ�ıȽϽ��
            if ( OrderOfSort == SortOrder.Ascending )
            {
                // ��Ϊ��������������ֱ�ӷ��ؽ��
                return compareResult;
            }
            else if ( OrderOfSort == SortOrder.Descending )
            {
                // ����Ƿ�����������Ҫȡ��ֵ�ٷ���
                return ( -compareResult );
            }
            else
            {
                // �����ȷ���0
                return 0;
            }
        }

        /// <summary>
        /// ��ȡ�����ð�����һ������.
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// ��ȡ����������ʽ.
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

        /// <summary>
        /// Ҫ���������ʲĩ����. ������ �ַ����� ���֣���
        /// </summary>
        public string SortValueType
        {
            set
            {
                _strValueType = value;
            }
            get
            {
                return _strValueType;
            }
        }

    }

    #endregion

    #region FLDCOL

    ////����ֱ����DataTable����ֵ��
    //public class FLDCOL
    //{
    //    //��¼ÿ�ж�Ӧ��Table���ֶ����ƣ� ����ֱ�Ӵ�DataTable��List��ֵ����һ�θ�ֵ���ж��У�
    //    string [ ] _strColName = null;
    //    string [ ] _strFldName = null;

    //    public bool IsValid ()
    //    {
    //        if ( _strColName == null || _strFldName == null )
    //            return false;

    //        if ( _strColName.Length == 0 || _strFldName.Length == 0 || _strColName.Length != _strFldName.Length )
    //            return false;

    //        return true;
    //    }

    //    public bool Set ( string [] strsCol , string [] strsFld )
    //    {
    //        if ( strsCol.Length != strsFld.Length )
    //            return false;

    //        _strColName = strsCol;
    //        _strFldName = strsFld;

    //        return true;
    //    }

    //    public string GetFldName ( string strColName )  
    //    {
    //        if ( ! IsValid () )
    //            return String.Empty ;

    //        int i = 0;
    //        foreach ( string str in _strColName )
    //        {
    //            if ( str == strColName )
    //            {
    //                if ( i < _strFldName.Length )
    //                    return _strFldName [ i ];
    //            } 
    //            i++;
    //        }
    //        return "";
    //    }

    //    public int GetDefColNum ()
    //    {
    //        if ( !IsValid () )
    //            return 0 ;

    //        return _strColName.Length ;
    //    }

    //}

    #endregion
}
