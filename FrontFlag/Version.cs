using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms ;

namespace FrontFlag
{
    #region �汾��ʷ
    /*
     * 2013-06-19 0.83 ���������ֶ�ʱ����ǿ��ת��Ϊ"YYY-MM-dd HH;MM:ss".(����Ĭ��ת��ʱ����������ڵ���Ϣ)
     * 
     * 2013-03-27 0.82 ����� COMBO.SetSelectText_NoCaseSens(). 
     * 
     * 2012-06-20 0.81 �����SQLITE�ࡣ  
     * 
     * 2012-01-08 0.80 �����Sys.CheckEnv�ࡣ  
     * 
     * 2011-06-14 0.79 ����GetPageSql() ��desc������£���ȡ��0ҳ�棬����ʾΪ0ҳ�����ݵ����⡣
     * 
     * 2010-12-12 0.78 FindList������� OnCellFormatting ���ԡ����ڴ���AfterFill�� 
     * 
     * 2010-10-12 0.76 ���FrontFlag.Control.DataGridView.XEditView �ࡣ 
     * 
     * 2010-05-28 0.72 FileĿ¼�����EXCEL�࣬����֧�ֵ���Excel�ļ���Table�� 
     * 
     * 2010-03-25 0.71 XEditListView�࣬�����SetColorTag()����������ָ���ض��е�Tag�л��ض�����ɫ��
     * 
     * 2010-03-23 0.70 �����UserCtrl/FindList ��GridView�ؼ���.  
     * 
     * 2010-03-08 0.69 XListView ����� SaveDtToExcel(), ���Ե���dt�� Excel��.  
     * 
     * 2009-06-03 0.68 Ctrol������� SetReadOnly_WhiteBK() 
     * 
     * 2009-04-27 0.67 XEditListView��DateTimePicker���͸�ΪMyDatePick�Զ���ؼ�������Clearѡ������ڡ�
     * 
     * 2009-04-07 0.66 XEditListView���DateTimePicker���͡�
     * 
     * 2009-02-13 0.65 SQL��� CopyRow() ������
     * 
     * 2009-02-11 0.64 XListView�ؼ���,money�����Զ���ȷ�����2λ�����4λС���㡣
     * 
     * 2009-01-23 0.63 ��ȷ��С�����4λ������С����2λ��
     * 
     * 2009-01-19 0.62 XListView�� ��� ItemUp(),ItemDown() ������
     * 
     * 2008-11-22 0.61 SQL�� ��� Save()��
     * 
     * 2008-11-04 0.51 ����ListView�������п��Ը������������⡣
     * 
     * 2008-04-12 0.49 SQL���ExecProcedureRet()������
     * 
     * 2007-12-20 0.48 �޸�XML, ����������λ������ӱ�����
     *                XListView ������XML�б����п�ȡ�  
     * 
     * 2007-12-06 0.47 ��� FUN.DATE.GetMonthLastDay ( int nYear , int nMonth ) ������
     * 
     * 2007-10-15 0.46 һ���С�޸ġ�
     * 
     * 2007-10-01 XListView��"yy-MM-dd HH:mm"��ʽ��ʾ���ڡ�
     * 
     * 2007-10-31 SQL.InsertRow2DB() ֧�ֲ������͡�
     * 
     * 2007-10-30 SQL.UpdateRow2DB() ֧�ֲ������͡�
     * 
     * 2007-10-05 L2A() ���׷��".00"��
     * 
     * 2007-09-25 ������ DataTable2List()�� dt=null �ǻ���������⡣
     * 
     * 2007-09-15 ������XEditList����������У�����󣬻��ԭ����EditBox�����ݸ����������� ������.
     * 
     * 
    */
    #endregion

    public class VERSION
    {
        public string strDate = "2013-06-19";
        public int nMainVersion = 0;
        public int nSubVersion = 83;
        public string strVersionType = "M";     //General,ͨ�ð汾.

        //0.1
        public string GetVerStr () 
        {
            return String.Format ( "{0}{1}.{2}" , strVersionType , nMainVersion , nSubVersion );
        }

        //Ver 0.1
        public string GetVerString ()
        {
            return String.Format ( "Ver {0}{1}.{2}" , strVersionType , nMainVersion , nSubVersion );
        }

        public bool CheckVer(string strType, int nNeedCheckMain, int nNeedCheckSub)
        {
            if ( strType != strVersionType )
                return false ;

            if (nMainVersion < nNeedCheckMain)
                return false ;

            if (nSubVersion < nNeedCheckSub)
                return false;

            return true;
        }

        public bool CheckVer ( string strVer )
        {
            STRING STR = new STRING (); 

            string strType = "";

            char [] chs = { '.' } ;
            string [] strs = null ;

            strs = strVer.Split ( chs ) ;
            
            if ( strs == null || strs.Length < 2 )
                return false ;

            strType = STR.SubStr.GetPreChar ( strs [ 0 ] );
            int nMain = FF.Fun.MyConvert.Str2Int ( strs [ 0 ] ) ;
            int nSub = FF.Fun.MyConvert.Str2Int ( strs [ 1 ] ) ;

            if ( !CheckVer ( strType , nMain , nSub ) )
                return false ;

            return true;
        }
    }
}
