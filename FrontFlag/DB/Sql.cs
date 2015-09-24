using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
//using NLog;

namespace FrontFlag
{
    public class SQL
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        private SqlConnection _conn = null;

        string strReplaceChar = "��";       //�����滻�ֶ���Σ�յ�'�ַ���


        #region ���캯��

        public SQL ()
        {
            _conn = null;
        }

        public SQL ( string strIP , string strDB , string strUser , string strPass )
        {
            string strConn = String.Format ( "data source={0};initial catalog={1};uid={2};pwd={3};Connect Timeout={4}" , strIP , strDB , strUser , strPass , 30 );
            try
            {
                _conn = new SqlConnection ( strConn );
            }
            catch ( Exception e )
            {
                FF.Ctrl.MsgBox.ShowWarn ( String.Format ( "connect SQL serevr is ERR !\n{0}" , e.Message ) );
            }
        }

        public SQL ( string strConn )
        {
            try
            {
                _conn = new SqlConnection ( strConn );
            }
            catch ( Exception e )
            {
                FF.Ctrl.MsgBox.ShowWarn ( String.Format ( "connect SQL serevr is ERR !\n{0}" , e.Message ) );
            }
        }

        #endregion

        #region ��������

        public void Close ()
        {
            if ( _conn != null )
            {
                _conn.Close ();
                _conn = null;
            }
        }    

        #endregion

        #region ״̬����

        bool IsOK ()
        { 
            return ( _conn != null ) ? true : false ;
        }

        #endregion

        #region ���Ժ���

        public SqlConnection Conn 
        {
            get { return _conn ; } 
        }

        #endregion

        void Log2Test( string strSqlString, DataSet ds)
        {
            char [] chs = new char[] {' '};
            string[] strs = strSqlString.Split(chs);
            if ( strs.Length <= 0 )
                return;

            string strVerb = strs[0].ToLower().Trim();

            string[] strsTab = new string[] { "_test_log", "_rolepopedom2form" };
            foreach (var tab in strsTab)
            {
                if (strSqlString.ToLower().Contains(tab))
                    return;
            }

            if (strVerb == "select")
            {
                string strMsg = String.Format("ExecDataSet(): {0}", strSqlString);
                try
                {
                    int nCount = ds.Tables[0].Rows.Count;
                    strMsg += " RetCount=" + nCount.ToString();
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

        public bool Exec ( string strSqlString )
        {
            try
            {
                _conn.Open ( );

                strSqlString = GetSafeStr ( strSqlString );
                SqlCommand cmd = new SqlCommand ( strSqlString , _conn );
                cmd.ExecuteNonQuery ( );
            }
            catch ( Exception e )
            {
                string str = String.Format( "SQL Error ! \n{0}\n{1}", e.Message, strSqlString );
                MessageBox.Show ( str );
                _conn.Close ( );
                return false;
            }

            _conn.Close ( );
            return true;
        }

        public void Exec ( string strSqlString , SqlParameter [ ] Params )
        {
            try
            {
                _conn.Open ( );

                strSqlString = GetSafeStr ( strSqlString );
                SqlCommand cmd = new SqlCommand ( strSqlString , _conn );
                cmd.Parameters.AddRange ( Params );

                cmd.ExecuteNonQuery ( );
            }
            catch ( Exception e )
            {
                string str = String.Format( "SQL Error ! \n{0}\n{1}", e.Message, strSqlString );
                MessageBox.Show ( str );
            }

            _conn.Close ( );
        }

        public DataSet ExecDataSet ( string strSqlString )
        {
            DataSet ds = new DataSet ( );

            try
            {
                _conn.Open ( );

                strSqlString = GetSafeStr ( strSqlString );
                SqlCommand cmd = new SqlCommand ( strSqlString , _conn );
                SqlDataReader dr = cmd.ExecuteReader ( );
                ds = DataReader2DataSet ( dr );

                dr.Close ( );

            }
            catch ( Exception e )
            {
                string strErr = String.Format ( "CSqlAdo Error ! " + e.Message + "\nSql�����: " + strSqlString );
                //FF.Debug.TimeLog(strErr);
                //MessageBox.Show ( strErr );
            }

            _conn.Close ( );

            //set Tablename to dataset.
            strSqlString = strSqlString.ToLower ( );
            int n = strSqlString.IndexOf ( "from" );
            if ( n >= 0 && ds != null && ds.Tables.Count > 0 )
            {
                string str = strSqlString.Substring ( n ).Trim ( );  //str= from tablename where xxxx
                n = str.IndexOf ( ' ' );
                str = str.Substring ( n ).Trim ( );  //str= tablename xxxxx
                n = str.IndexOf ( ' ' );
                if ( n >= 0 )                        //if have not Where , n =-1 .
                    str = str.Substring ( 0 , n ).Trim ( ); //str = tablename

                ds.Tables [ 0 ].TableName = str;
            }

            return ds;
        }

        public DataTable ExecDataTable ( string strSqlString )
        {
            DataSet ds = new DataSet ( );
            strSqlString = GetSafeStr ( strSqlString );
            ds = ExecDataSet ( strSqlString );
            if ( ds != null && ds.Tables.Count > 0 )
                return ds.Tables [ 0 ];
            else
                return null;
        }

        public string ExecInsertRet ( string strInsert )
        {
            strInsert = GetSafeStr ( strInsert );
            DataTable dt = new DataTable ( );
            string strSql = "SET NOCOUNT ON ; " + strInsert + " ; SELECT @@IDENTITY ";
            dt = ExecDataTable ( strSql );
            if ( dt == null || dt.Rows.Count <= 0 )
                return "";
            return dt.Rows [ 0 ] [ 0 ].ToString ( );
        }

        public void ExecProcedure ( string strProcName , SqlParameter [ ] Param )
        {
            try
            {
                _conn.Open ( );

                strProcName = GetSafeStr ( strProcName );
                SqlCommand cmd = new SqlCommand ( strProcName , _conn );
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange ( Param );

                cmd.ExecuteNonQuery ( );
            }
            catch ( Exception e )
            {
                string str = String.Format( "SQL Error ! \n{0}\n{1}", e.Message, strProcName );
                MessageBox.Show ( str );
            }

            _conn.Close ( );
        }

        public DataTable ExecProcedureRet ( string strProcName , SqlParameter[] Param )
        {
            DataSet ds = new DataSet ();

            try
            {
                _conn.Open ();

                strProcName = GetSafeStr ( strProcName );
                SqlCommand cmd = new SqlCommand ( strProcName , _conn );
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange ( Param );

                SqlDataReader dr = cmd.ExecuteReader ();
                ds = DataReader2DataSet ( dr );
            }
            catch ( Exception e )
            {
                string str = String.Format( "SQL Error ! \n{0}\n{1}", e.Message, strProcName );
                MessageBox.Show ( str );
            }

            _conn.Close ();

            DataTable dt = new DataTable () ;

            if ( ds.Tables.Count > 0 && ds.Tables[ 0 ] != null )
                dt = ds.Tables[ 0 ];   

            return dt;
        }

        public string [ ] ExecStrAry ( string strSqlString )
        {
            ArrayList ary = new ArrayList ( );

            try
            {
                _conn.Open ( );

                strSqlString = GetSafeStr ( strSqlString );
                SqlCommand cmd = new SqlCommand ( strSqlString , _conn );
                SqlDataReader dr = cmd.ExecuteReader ( );

                string str;
                while ( dr.Read ( ) )
                {
                    str = ( ( string ) dr.GetValue ( 0 ) ).Trim ( );
                    ary.Add ( str );
                }

                dr.Close ( );

            }
            catch ( Exception e )
            {
                string strErr = String.Format ( "CSqlAdo Error ! " + e.Message + "\nSql�����: " + strSqlString );
                MessageBox.Show ( strErr );
            }

            _conn.Close ( );

            //
            string [ ] strary = new string [ ary.Count ];
            for ( int i = 0 ; i < ary.Count ; i++ )
                strary [ i ] = ( string ) ary [ i ];

            return strary;
        }

        //��ȡ���е������ֶ����ơ�
        public string [ ] GetTableFldNames ( string strTable )
        {
            DataTable dt = new DataTable ( );
            string strSql = String.Format ( "SELECT columns.name from syscolumns columns,sysobjects objects where objects.name='{0}' and columns.id=objects.id order by colid " , strTable );
            dt = ExecDataTable ( strSql );

            int n = 0;
            string str;
            string [ ] strs = new string [ dt.Rows.Count ];
            foreach ( DataRow dr in dt.Rows )
            {
                str = dr [ 0 ].ToString ( ).Trim ( );
                strs [ n++ ] = str;
            }
            return strs;
        }

        public DataSet DataReader2DataSet ( SqlDataReader reader )
        {
            DataSet dataSet = new DataSet ( );
            do
            {
                // Create new data table

                DataTable schemaTable = reader.GetSchemaTable ( );
                DataTable dataTable = new DataTable ( );

                if ( schemaTable != null )
                {
                    // A query returning records was executed

                    for ( int i = 0 ; i < schemaTable.Rows.Count ; i++ )
                    {
                        DataRow dataRow = schemaTable.Rows [ i ];
                        // Create a column name that is unique in the data table
                        string columnName = ( string ) dataRow [ "ColumnName" ];
                        // Add the column definition to the data table
                        DataColumn column = new DataColumn ( columnName , ( Type ) dataRow [ "DataType" ] );
                        dataTable.Columns.Add ( column );
                    }

                    dataSet.Tables.Add ( dataTable );

                    // Fill the data table we just created

                    while ( reader.Read ( ) )
                    {
                        DataRow dataRow = dataTable.NewRow ( );

                        for ( int i = 0 ; i < reader.FieldCount ; i++ )
                            dataRow [ i ] = reader.GetValue ( i );

                        dataTable.Rows.Add ( dataRow );
                    }
                }
                else
                {
                    // No records were returned

                    DataColumn column = new DataColumn ( "RowsAffected" );
                    dataTable.Columns.Add ( column );
                    dataSet.Tables.Add ( dataTable );
                    DataRow dataRow = dataTable.NewRow ( );
                    dataRow [ 0 ] = reader.RecordsAffected;
                    dataTable.Rows.Add ( dataRow );
                }
            }
            while ( reader.NextResult ( ) );
            return dataSet;
        }

        public ArrayList GetSelect2strary ( string strSqlString )
        {
            string str;

            DataTable dt = new DataTable ( );
            ArrayList arystr = new ArrayList ( );

            //_conn.Open();

            //SqlCommand cmd = new SqlCommand( strSql, _conn);
            //SqlDataReader dr = cmd.ExecuteReader();
            strSqlString = GetSafeStr ( strSqlString );
            dt = ExecDataTable ( strSqlString );

            foreach ( DataRow dr in dt.Rows )
            {
                str = dr [ 0 ].ToString ( ).Trim ( );
                arystr.Add ( str );
            }

            //dr.Close () ;
            //_conn.Close();

            return arystr;
        }

        #region DataSet

        public bool UpdateDataSet2DB ( ref DataSet ds , string [ ] strKeys )
        {
            DataTable dt = ds.Tables [ 0 ];
            //            DataRow[] currentRows = dt.Select ( null , null , DataViewRowState.ModifiedCurrent );
            DataRow [ ] currentRows = dt.Select ( null , null , DataViewRowState.CurrentRows );

            if ( currentRows.Length < 1 )
                return false;

            int n = 0;
            string strTable = dt.TableName;
            string str = "" , strSet , strSql , strWhere , strText;
            bool bRet = true;

            //foreach ( DataRow dr in dt.Rows )  
            foreach ( DataRow dr in currentRows )
            {
                //create Where string
                n = 0;
                strWhere = "";
                foreach ( string strKey in strKeys )
                {
                    if ( n > 0 )
                        strWhere += " AND ";
                    str = String.Format ( "{0}='{1}'" , strKey , dr [ strKey ].ToString ().Trim () );
                    strWhere += str;
                    n++;
                }

                //create set( x=y ,) string
                n = 0;
                strSet = "";
                foreach ( DataColumn dc in dt.Columns )
                {
                    Type t = dr [ dc ].GetType ();

                    strText = dr [ dc ].ToString ().Trim ();
                    strText = strText.Replace ( "'" , strReplaceChar );

                    if ( t == typeof ( System.String ) )
                        str = String.Format ( " {0}='{1}' " , dc.ColumnName , strText );
                    //else if ( t == typeof ( System.Byte[] ) )
                    //  str = String.Format ( " {0}={1} " , dc.ColumnName , dr[dc] ) ;
                    else
                        continue;

                    if ( n > 0 )
                        strSet += ", ";
                    strSet += str;
                    n++;
                }
                strSql = String.Format ( "UPDATE {0} SET {1} WHERE {2}" , strTable , strSet , strWhere );
                bRet &= Exec ( strSql );
            }

            return bRet;
        }

        #endregion

        #region DataTable

        public string InsertTable2DB ( ref DataTable dt , string [ ] strsNotSaveFld )
        {
            if ( dt.Rows.Count < 0 )
                return "";

            string strRetID = "";

            string [ ] strsAutoAddFld = GetFlagFld ( dt.TableName );
            string strTable = dt.TableName;

            int n = 0;
            string strSql , str1 = "" , str2 = "" , strFld = "" , strValue = "" , strText ;
            Type t;
            bool bEndFlag = false , bRet = true ;

            foreach ( DataRow dr in dt.Rows )
            {
                n = 0;
                str1 = "";
                str2 = "";

                foreach ( DataColumn dc in dt.Columns )
                {
                    bEndFlag = false;

                    if ( strsAutoAddFld != null )
                    {
                        foreach ( string s in strsAutoAddFld )   //����Ƿ����������У� Ҫ�ǣ��Ͳ���������ˡ�
                        {
                            if ( s == dc.ColumnName )
                                bEndFlag = true;
                        }
                    }

                    if ( strsNotSaveFld != null )
                    {
                        foreach ( string s2 in strsNotSaveFld ) //����Ƿ�ָ���Ĳ�������У� Ҫ�ǣ��Ͳ���������ˡ�
                        {
                            if ( s2 == dc.ColumnName )
                                bEndFlag = true;
                        }
                    }

                    if ( bEndFlag )
                        continue;

                    strFld = dc.ColumnName;
                    t = dc.DataType;

                    strText = dr [ dc ].ToString ().Trim () ;
                    strText = strText.Replace ( "'" , strReplaceChar );

                    if ( t == typeof ( System.String ) )
                        strValue = String.Format ( " '{0}' " , strText );
                    else if ( t == typeof ( System.Int32 ) )
                        strValue = String.Format ( " {0} " , Convert.IsDBNull ( dr [ dc ] ) ? 0 : dr [ dc ] );
                    else if ( t == typeof ( System.Boolean ) )
                        strValue = String.Format( " '{0}' ", ( strText.ToLower() == "true" ) ? "True":"False" );
                    else if ( t == typeof ( System.Byte ) )
                        strValue = String.Format ( " {0} " , dr [ dc ] );
                    else if (t == typeof(System.DateTime))
                    {
                        try
                        {
                            strText = Convert.ToDateTime( dr[dc] ).ToString( "yyyy-MM-dd HH:mm:dd" );
                        }
                        catch ( Exception )
                        {
                            strText = String.Empty;
                        }

                        if (strText == "")
                            strValue = String.Format(" {0} ", "NULL");
                        else
                            strValue = String.Format(" '{0}' ", strText);
                    }
                    else if (t == typeof(System.Decimal))
                        strValue = String.Format(" {0} ", strText);
                    else if (t == typeof(System.Double))
                        strValue = String.Format(" {0} ", strText);
                    else
                        //continue;
                        strValue = String.Format(" '{0}' ", strText);

                    if ( n > 0 )
                    {
                        str1 += ", ";
                        str2 += ", ";
                    }

                    str1 += strFld;
                    str2 += strValue;

                    n++;
                }

                strSql = String.Format ( "INSERT INTO {0} ( {1} ) VALUES ( {2} ) " , strTable , str1 , str2 );
                strRetID = ExecInsertRet ( strSql );
            }

            return strRetID;
        }
        public string InsertTable2DB ( ref DataTable dt )
        {
            string [ ] strsNotSaveFld = null;
            return InsertTable2DB ( ref dt , strsNotSaveFld );
        }

        public bool UpdateTable2DB ( ref DataTable dt , string [ ] strKeys , string [ ] strNotSaveFld )
        {
            //            DataRow[] currentRows = dt.Select ( null , null , DataViewRowState.ModifiedCurrent );
            DataRow [ ] currentRows = dt.Select ( null , null , DataViewRowState.CurrentRows );

            if ( currentRows.Length < 1 )
                return false;

            int n = 0;
            string strTable = dt.TableName;
            string str = "" , strFld , strSet , strSql , strWhere , strText ;
            Type t;
            bool bEndFlag;
            bool bRet = true;

            string [ ] strsAutoAddFld = GetFlagFld ( dt.TableName );

            //foreach ( DataRow dr in dt.Rows )  
            foreach ( DataRow dr in currentRows )
            {
                //create Where string
                n = 0;
                strWhere = "";
                foreach ( string strKey in strKeys )
                {
                    if ( n > 0 )
                        strWhere += " AND ";

                    strText = dr [ strKey ].ToString ( ).Trim ( ) ;
                    strText = strText.Replace ( "'" , strReplaceChar );

                    str = String.Format ( "{0}='{1}'" , strKey , strText );
                    strWhere += str;
                    n++;
                }

                //create set( x=y ,) string
                n = 0;
                strSet = "";
                foreach ( DataColumn dc in dt.Columns )
                {
                    bEndFlag = false;
                    strFld = dc.ColumnName;

                    foreach ( string s in strsAutoAddFld )   //����Ƿ����������У� Ҫ�ǣ��Ͳ���������ˡ�
                    {
                        if ( s == strFld )
                            bEndFlag = true;
                    }

                    if ( bEndFlag )
                        continue;

                    t = dr [ dc ].GetType ( );

                    foreach ( string strNot in strNotSaveFld )
                    {
                        if ( strNot == strFld )
                            bEndFlag = true;
                    }

                    if ( bEndFlag )
                        continue;

                    strText = dr [ dc ].ToString ( ).Trim ( ) ;
                    strText = strText.Replace ( "'" , strReplaceChar );

                    if ( t == typeof ( System.String ) )
                        str = String.Format ( " {0}='{1}' " , strFld , strText );
                    else if ( t == typeof ( System.Int32 ) )
                        str = String.Format ( " {0}={1} " , strFld , dr [ dc ] );
                    else if (t == typeof(System.DateTime))
                    {
                        try
                        {
                            strText = Convert.ToDateTime( dr[dc] ).ToString( "yyyy-MM-dd HH:mm:dd" );
                        }
                        catch ( Exception )
                        {
                            strText = String.Empty;
                        }

                        if ( strText == "" )
                            str = String.Format(" {0}={1} ", strFld, "NULL");
                        else
                            str = String.Format(" {0}='{1}' ", strFld, strText);
                    }
                    else if (t == typeof(System.Decimal))
                        str = String.Format(" {0}={1} ", strFld, dr[dc]);
                    else if (t == typeof(System.Double))
                        str = String.Format(" {0}={1} ", strFld, dr[dc]);
                    else if (t == typeof(System.DBNull))
                        str = String.Format(" {0}={1} ", strFld, "NULL");
                    else
                        continue;

                    if ( n > 0 )
                        strSet += ", ";
                    strSet += str;

                    n++;
                }

                strSql = String.Format ( "UPDATE {0} SET {1} WHERE {2}" , strTable , strSet , strWhere );
                bRet &= Exec ( strSql );
            }

            return bRet;
        }
        public bool UpdateTable2DB ( ref DataTable dt , string [ ] strKeys )
        {
            string [ ] strNotFld = new string [ 1 ] { "" };
            return UpdateTable2DB ( ref dt , strKeys , strNotFld );
        }

        public string Save(ref DataTable dt, string[] strsNotSaveFld)
        {
            string[] strsAutoAddFld = GetFlagFld(dt.TableName);

            //û���������С�
            if (strsAutoAddFld.Length <= 0)
                return "";

            //һ��Ĭ�ϵ�һ���������У�������id
            string strID = strsAutoAddFld[0];
            string strRetID = "";  //Ҫ���ص�������ID.

            foreach (DataRow dr in dt.Rows)
            {
                DataRow drtmp = dr;

                if (dr[strID].ToString().Trim() == "")       //û��id,��ʾ�������ӵ��С�
                {
                    strRetID = InsertRow2DB(ref drtmp, strsNotSaveFld);
                }
                else
                {
                    UpdateRow2DB(ref drtmp, strsAutoAddFld, strsNotSaveFld);     //�޸�ԭ�����е��С�
                    strRetID = dr[strID].ToString().Trim();
                }
            }

            return strRetID;
        }
        public string Save(ref DataTable dt)
        {
            string[] strsNotSaveFld = new string[1] { "" };
            return Save(ref dt, strsNotSaveFld);
        }

        #endregion 

        #region DataRow

        public string InsertRow2DB ( ref DataRow dr , string [ ] strsNotSaveFld )
        {
            if ( dr == null )
                return "";

            string strRetID = "";

            string [ ] strsAutoAddFld = GetFlagFld ( dr.Table.TableName );
            string strTable = dr.Table.TableName;

            int n = 0;
            string strSql , str1 = "" , str2 = "" , strFld = "" , strValue = "" , strText;
            Type t;
            bool bEndFlag = false , bRet = true;

            n = 0;
            str1 = "";
            str2 = "";

            foreach ( DataColumn dc in dr.Table.Columns )
            {
                bEndFlag = false;

                if ( strsAutoAddFld != null )
                {
                    foreach ( string s in strsAutoAddFld )   //����Ƿ����������У� Ҫ�ǣ��Ͳ���������ˡ�
                    {
                        if ( s == dc.ColumnName )
                            bEndFlag = true;
                    }
                }

                if ( strsNotSaveFld != null )
                {
                    foreach ( string s2 in strsNotSaveFld ) //����Ƿ�ָ���Ĳ�������У� Ҫ�ǣ��Ͳ���������ˡ�
                    {
                        if ( s2 == dc.ColumnName )
                            bEndFlag = true;
                    }
                }

                if ( bEndFlag )
                    continue;

                strFld = dc.ColumnName;
                t = dc.DataType;

                strText = dr [ dc ].ToString ().Trim ();
                strText = strText.Replace ( "'" , strReplaceChar );

                if ( t == typeof ( System.String ) )
                    strValue = String.Format ( " '{0}' " , strText );
                else if ( t == typeof ( System.Int32 ) )
                    strValue = String.Format ( " {0} " , Convert.IsDBNull ( dr [ dc ] ) ? 0 : dr [ dc ] );
                else if ( t == typeof ( System.Boolean ) )
                    strValue = String.Format ( " {0} " , ( strText.ToLower() == "true" ) ? '1' : '0' );
                else if ( t == typeof ( System.Byte ) )
                    strValue = String.Format ( " {0} " , dr [ dc ] );
                else if (t == typeof(System.DateTime))
                {
                    try
                    {
                        strText = Convert.ToDateTime( dr[dc] ).ToString( "yyyy-MM-dd HH:mm:dd" );
                    }
                    catch ( Exception )
                    {
                        strText = String.Empty;
                    }

                    if (strText == "")
                        strValue = String.Format(" {0} ", "NULL");
                    else
                        strValue = String.Format(" '{0}' ", strText);
                }
                else if (t == typeof (System.Decimal))
                {
                    if (String.IsNullOrEmpty(strText.Trim()))
                        strText = "0";
                    strValue = String.Format(" {0} ", strText);
                }
                else if (t == typeof (System.Double))
                {
                    if ( String.IsNullOrEmpty( strText.Trim() ) )
                        strText = "0";
                    strValue = String.Format(" {0} ", strText);
                }
                else
                    strValue = String.Format(" '{0}' ", strText);

                if ( n > 0 )
                {
                    str1 += ", ";
                    str2 += ", ";
                }

                str1 += strFld;
                str2 += strValue;

                n++;
            }

            strSql = String.Format ( "INSERT INTO {0} ( {1} ) VALUES ( {2} ) " , strTable , str1 , str2 );

            //logger.Info( "FF/Sql.cs/InsertRow2DB(), strSql= " + strSql );

            try
            {
                strRetID = ExecInsertRet( strSql );
            }
            catch (Exception e)
            {
                //logger.Info( "FF/Sql.cs/InsertRow2DB(), Err= " + e.Message );
                //FF.Ctrl.MsgBox.Show( e.Message );
            }
            
            return strRetID;
        }
        public string InsertRow2DB ( ref DataRow dr )
        {
            string [ ] strsNotSaveFld = null;
            return InsertRow2DB ( ref dr , strsNotSaveFld );
        }

        public bool UpdateRow2DB ( ref DataRow dr , string [ ] strKeys , string [ ] strNotSaveFld )
        {
            if ( dr == null || strKeys == null || strKeys.Length <= 0 )
                return false;

            int n = 0;
            string strTable = dr.Table.TableName;
            string str = "" , strFld , strSet , strSql , strWhere , strText;
            Type t;
            bool bEndFlag;
            bool bRet = true;

            string [ ] strsAutoAddFld = GetFlagFld ( dr.Table.TableName );

            n = 0;
            strWhere = "";
            foreach ( string strKey in strKeys )
            {
                if ( n > 0 )
                    strWhere += " AND ";

                strText = dr [ strKey ].ToString ().Trim ();
                strText = strText.Replace ( "'" , strReplaceChar );

                str = String.Format ( "{0}='{1}'" , strKey , strText );
                strWhere += str;
                n++;
            }

            //create set( x=y ,) string
            n = 0;
            strSet = "";
            foreach ( DataColumn dc in dr.Table.Columns )
            {
                bEndFlag = false;
                strFld = dc.ColumnName;

                foreach ( string s in strsAutoAddFld )   //����Ƿ����������У� Ҫ�ǣ��Ͳ���������ˡ�
                {
                    if ( s == strFld )
                        bEndFlag = true;
                }

                if ( bEndFlag )
                    continue;

                t = dr [ dc ].GetType ();

                foreach ( string strNot in strNotSaveFld )
                {
                    if ( strNot == strFld )
                        bEndFlag = true;
                }

                if ( bEndFlag )
                    continue;

                strText = dr [ dc ].ToString ().Trim ();
                strText = strText.Replace ( "'" , strReplaceChar );

                if ( t == typeof ( System.String ) )
                    str = String.Format ( " {0}='{1}' " , strFld , strText );
                else if ( t == typeof ( System.Int32 ) )
                    str = String.Format ( " {0}={1} " , strFld , dr [ dc ] );
                else if (t == typeof(System.DateTime))
                {
                    try
                    {
                        strText = Convert.ToDateTime( dr[dc] ).ToString( "yyyy-MM-dd HH:mm:dd" );
                    }
                    catch ( Exception )
                    {
                        strText = String.Empty;
                    }

                    if ( strText == "" )
                        str = String.Format( " {0}={1} ", strFld, "NULL" );
                    else
                    {
                        str = String.Format(" {0}='{1}' ", strFld, strText);
                    }
                }
                else if (t == typeof(System.Decimal))
                    str = String.Format(" {0}={1} ", strFld, strText);
                else if (t == typeof(System.Double))
                    str = String.Format(" {0}={1} ", strFld, strText);
                else if (t == typeof(System.Boolean))
                    str = String.Format(" {0}='{1}' ", strFld, (strText == "True") ? '1' : '0');
                else if (t == typeof(System.DBNull))
                    str = String.Format(" {0}={1} ", strFld, "NULL");
                else
                    str = String.Format(" {0}='{1}' ", strFld, strText);

                if ( n > 0 )
                    strSet += ", ";
                strSet += str;

                n++;
            }

            strSql = String.Format ( "UPDATE {0} SET {1} WHERE {2}" , strTable , strSet , strWhere );
            bRet &= Exec ( strSql );

            ////test 2013
            //logger.Info( strSql );
            //FF.Debug.Log( strSql );

            return bRet;
        }
        //�� strKeys Ϊ�����ֶΣ� ������dr. ���� strKeys��ָ����һ�������ֶ� ������ Update�е� Where �ִ���
        public bool UpdateRow2DB ( ref DataRow dr , string [ ] strKeys )
        {
            string [ ] strNotFld = { "" };
            return UpdateRow2DB ( ref dr , strKeys , strNotFld );
        }
        //Ĭ�� dr ���Ѿ���һ���������ֶΡ����Ǹ��������ֶ���Ϊ strKeys. ����dr�����е�ֵ��
        public bool UpdateRow2DB ( ref DataRow dr )
        {
            if ( dr == null )
                return false;

            int n = 0;
            string strTable = dr.Table.TableName;
            string str = "" , strFld , strSet , strSql , strWhere , strText;
            Type t;
            bool bEndFlag;
            bool bRet = true;

            string [ ] strsAutoAddFld = GetFlagFld ( dr.Table.TableName );

            n = 0;
            strWhere = "";
            foreach ( string strKey in strsAutoAddFld )
            {
                if ( n > 0 )
                    strWhere += " AND ";

                strText = dr [ strKey ].ToString ().Trim ();
                strText = strText.Replace ( "'" , strReplaceChar );

                str = String.Format ( "{0}='{1}'" , strKey , strText );
                strWhere += str;
                n++;
            }

            //create set( x=y ,) string
            n = 0;
            strSet = "";
            foreach ( DataColumn dc in dr.Table.Columns )
            {
                bEndFlag = false;
                strFld = dc.ColumnName;

                foreach ( string s in strsAutoAddFld )   //����Ƿ����������У� Ҫ�ǣ��Ͳ���������ˡ�
                {
                    if ( s == strFld )
                        bEndFlag = true;
                }

                if ( bEndFlag )
                    continue;

                t = dr [ dc ].GetType ();

                if ( bEndFlag )
                    continue;

                strText = dr [ dc ].ToString ().Trim ();
                strText = strText.Replace ( "'" , strReplaceChar );

                if ( t == typeof ( System.String ) )
                    str = String.Format ( " {0}='{1}' " , strFld , strText );
                else if ( t == typeof ( System.Int32 ) )
                    str = String.Format ( " {0}={1} " , strFld , dr [ dc ] );
                else if (t == typeof(System.DateTime))
                {
                    try
                    {
                        strText = Convert.ToDateTime( dr[dc] ).ToString( "yyyy-MM-dd HH:mm:dd" );
                    }
                    catch ( Exception )
                    {
                        strText = String.Empty;
                    }

                    if (strText == "")
                        str = String.Format(" {0}={1} ", strFld, "NULL");
                    else
                        str = String.Format(" {0}='{1}' ", strFld, strText);
                }
                else if (t == typeof(System.Decimal))
                    str = String.Format(" {0}={1} ", strFld, dr[dc]);
                else if (t == typeof(System.Double))
                    str = String.Format(" {0}={1} ", strFld, dr[dc]);
                else if ( t == typeof ( System.DBNull ) )
                    str = String.Format(" {0}={1} ", strFld, "NULL");
                else
                    continue;

                if ( n > 0 )
                    strSet += ", ";
                strSet += str;

                n++;
            }

            strSql = String.Format ( "UPDATE {0} SET {1} WHERE {2}" , strTable , strSet , strWhere );
            bRet &= Exec ( strSql );

            return bRet;
        }

        //Delete
        public bool DeleteRow ( ref DataRow dr , string [ ] strKeys )
        {
            if ( dr == null || strKeys == null || strKeys.Length <= 0 )
                return false;

            int n = 0;
            string strTable = dr.Table.TableName;
            string str = "" , strFld , strSet , strSql , strWhere , strText;
            Type t;
            bool bEndFlag;
            bool bRet = true;

            string [ ] strsAutoAddFld = GetFlagFld ( dr.Table.TableName );

            n = 0;
            strWhere = "";
            foreach ( string strKey in strKeys )
            {
                if ( n > 0 )
                    strWhere += " AND ";

                strText = dr [ strKey ].ToString ().Trim ();
                strText = strText.Replace ( "'" , strReplaceChar );

                str = String.Format ( "{0}='{1}'" , strKey , strText );
                strWhere += str;
                n++;
            }

            strSql = String.Format ( "delete from {0} WHERE {1}" , strTable , strWhere );
            bRet &= Exec ( strSql );

            return bRet;

        }

        //Ĭ�� dr ���Ѿ���һ���������ֶΡ�
        public bool DeleteRow ( ref DataRow dr )
        {
            if ( dr == null )
                return false;

            int n = 0;
            string strTable = dr.Table.TableName;
            string str = "" , strFld , strSet , strSql , strWhere , strText;
            Type t;
            bool bEndFlag;
            bool bRet = true;

            string [ ] strsAutoAddFld = GetFlagFld ( dr.Table.TableName );

            n = 0;
            strWhere = "";
            foreach ( string strKey in strsAutoAddFld )
            {
                if ( n > 0 )
                    strWhere += " AND ";

                strText = dr [ strKey ].ToString ().Trim ();
                strText = strText.Replace ( "'" , strReplaceChar );

                str = String.Format ( "{0}='{1}'" , strKey , strText );
                strWhere += str;
                n++;
            }

            strSql = String.Format ( "Delete from {0} WHERE {1}" , strTable , strWhere );
            bRet &= Exec ( strSql );

            return bRet;
        }

        //�õ������û���������ֶ���Ϣ��
        public DataTable GetTablesInfo ()
        {
            string strSql = "SELECT TableName = o.name, OWNER = USER_NAME(o.uid),       TableDescription = ISNULL(ptb.value, N''), FieldId = c.colid, FieldName = c.name,       FieldType = QUOTENAME(t .name) + CASE WHEN t .name IN (N'decimal', N'numeric')       THEN N'(' + CAST(c.prec AS varchar) + N',' + CAST(c.scale AS varchar)       + N')' WHEN t .name = N'float' OR      t .name LIKE N'%char' OR      t .name LIKE N'%binary' THEN N'(' + CAST(c.prec AS varchar)       + N')' ELSE N'' END + CASE WHEN c.isnullable = 1 THEN N'' ELSE N' NOT' END + N' NULL',       FieldDescription = ISNULL(pfd.value, ''), DefileLength = c.length,       FieldDefault = ISNULL(df.text, N''), IsIDENTITY = COLUMNPROPERTY(o.id, c.name,       N'IsIdentity'), IsComputed = COLUMNPROPERTY(o.id, c.name, N'IsComputed'),       IsROWGUID = COLUMNPROPERTY(o.id, c.name, N'IsRowGuidCol'),       IsPrimaryKey = CASE WHEN opk.xtype IS NULL THEN 0 ELSE 1 END FROM sysobjects o JOIN       syscolumns c ON c.id = o.id AND OBJECTPROPERTY(o.id, N'IsUserTable') = 1 JOIN       systypes t ON t .xusertype = c.xusertype LEFT JOIN       syscomments df ON df.id = c.cdefault LEFT JOIN       sysproperties ptb ON ptb.id = o.id AND ptb.smallid = 0 LEFT JOIN       sysproperties pfd ON pfd.id = o.id AND pfd.smallid = c.colid LEFT JOIN       sysindexkeys idxk ON idxk.id = o.id AND idxk.colid = c.colid LEFT JOIN       sysindexes idx ON idx.indid = idxk.indid AND idx.id = idxk.id AND        idx.indid NOT IN (0, 255) LEFT JOIN       sysobjects opk ON opk.parent_obj = o.id AND opk.name = idx.name AND        OBJECTPROPERTY(opk.id, N'IsPrimaryKey') = 1 ORDER BY o.name, c.colid";
            return ExecDataTable ( strSql );
        }

        #endregion

        #region Static

        static public bool IsValid ( ref DataSet ds )
        {
            if ( ds == null || ds.Tables.Count <= 0 )
                return false;

            if ( ds.Tables [ 0 ] == null || ds.Tables [ 0 ].Rows.Count <= 0 )
                return false;

            return true;
        }
        static public bool IsValid ( ref DataTable dt )
        {
            if ( dt == null || dt.Rows.Count <= 0 )
                return false;
            return true;
        }
        static public bool IsNotValid ( ref DataTable dt )
        {
            return !IsValid ( ref dt );
        }

        static public bool HaveVisibleRow ( DataTable dt )
        {
            DataRow [ ] drs = dt.Select ( "" , "" , DataViewRowState.CurrentRows );
            if ( drs == null || drs.Length <= 0 )
                return false;
            else
                return true;
        }
        static public int GetVisibleRowNum ( ref DataTable dt )
        {
            DataRow [ ] drs = dt.Select ( "" , "" , DataViewRowState.CurrentRows );
            if ( drs == null || drs.Length <= 0 )
                return 0;
            else
                return drs.Length;
        }

        static public string GetFld ( ref DataRow dr , string strFld )
        {
            if ( dr == null )
                return "";

            string strRet = "";
            try
            {
                strRet = dr [ strFld ].ToString ().Trim ();
            }
            catch ( Exception e )
            {
                FF.Ctrl.MsgBox.ShowWarn (  String.Format ( "Err in SQL.GetFld() !\n{0}" , e.Message ) );
            }

            return strRet;
        }

        static public bool SetFld ( ref DataRow dr , string strFld , string strText )
        {
            if ( dr == null )
                return false;

            try
            {
                dr [ strFld ] = strText.Trim ();
            }
            catch ( Exception e )
            {
                FF.Ctrl.MsgBox.ShowWarn ( String.Format ( "Err in SQL.SetFld() !\n{0}" , e.Message ) );
                return false;
            }

            return true ;
        }

        //dt1 �� dt2 �ĽṹҪһ����
        static public void AppendDt(ref DataTable dt1, ref DataTable dt2)
        {
            object[] obj = new object[dt2.Columns.Count];

            foreach (DataRow dr2 in dt2.Rows)
            {
                dr2.ItemArray.CopyTo(obj, 0);
                dt1.Rows.Add(obj); ; 
            }
        }

        //dt1 �� dt2 �ĽṹҪһ����
        static public void CopyRow ( DataRow drSrc , ref DataRow drDest )
        {
            object [] obj = new object [drSrc.Table.Columns.Count];

            drSrc.ItemArray.CopyTo ( obj , 0 );
            drDest.ItemArray = obj;
      
        }

        #endregion

        //
        public DataTable GetNewTable ( ref DataTable table , string strSelect )
        {
            DataTable NewTable = table.Clone ();

            strSelect = GetSafeStr ( strSelect );
            DataRow [ ] drs = table.Select ( strSelect );

            foreach ( DataRow row in drs )
            {
                DataRow dr = NewTable.NewRow ();

                for ( int i = 0 ; i < NewTable.Columns.Count ; i++ )
                {
                    dr [ i ] = row [ i ];
                }
                NewTable.Rows.Add ( dr );
            }

            return NewTable;
        }

        public DataTable GetBlankDt ( string strTableName )
        {
            string strSql = String.Format ( "select * from {0} where 1<>1" , strTableName );

            DataTable dt = new DataTable ();
            dt = ExecDataTable ( strSql );

            if ( dt == null  )
                return null ;

            DataRow dr = dt.NewRow ();
            dt.Rows.Add ( dr );

            return dt;
        }

        public DataRow GetBlankDr ( string strTableName )
        {
            DataTable dt = GetBlankDt ( strTableName );

            if ( SQL.IsNotValid ( ref dt ) )
                return null;

            DataRow dr = dt.Rows [ 0 ];
            return dr;
        }

        static public DataRow AddNewRow ( ref DataTable dt )
        {
            DataRow dr = dt.NewRow ( );
            dt.Rows.Add ( dr );
            return dr;
        }

        /*
        static public DataRow AddRow(ref DataTable dt , DataRow dr )
        {
            DataRow drNew = dt.NewRow();
            drNew = dr;
            dt.Rows.Add(drNew);
            return drNew;
        }
         * */

        public void DataSetClear ( ref DataSet ds )
        {
            if ( ds.Tables.Count <= 0 )
                return;

            string [ ] strsFlagFld = GetFlagFld ( ds.Tables [ 0 ].TableName );

            DataTable dt = ds.Tables [ 0 ];

            foreach ( DataRow dr in dt.Rows )
            {
                foreach ( DataColumn dc in dt.Columns )
                {
                    bool bFlag = true;
                    foreach ( string s in strsFlagFld )
                    {
                        if ( s == dc.ColumnName )
                            bFlag = false;
                    }

                    if ( bFlag )
                        dr [ dc ] = "";
                }
            }
        }

        //get �����������ֶΡ�
        public string [ ] GetFlagFld ( string strTabeName )
        {
            DataTable dt = new DataTable ();

            try
            {
                string strSql = String.Format ( "Select [name] From sysColumns Where id=object_id(N'{0}') and Status=128" , strTabeName );
                dt = ExecDataTable ( strSql );
            }
            catch(Exception e)   //�������û���������ֶλ��׳��쳣��
            {
                string [] strs = new string [ 0 ];
                return strs;
            }

            string [] strary = new string [ dt.Rows.Count ];
            int i = 0;
            foreach ( DataRow dr in dt.Rows )
            {
                strary [ i ] = dr [ 0 ].ToString ( ).Trim ( );
                i++;
            }
            return strary;
        }

        public bool IsFlagFld(DataRow dr, string strFldName)
        {
            string strTableName = dr.Table.TableName;
            string[] strFld = GetFlagFld( strTableName );

            foreach (string s in strFld)
            {
                if (s.ToLower() == strFldName.ToLower())
                    return true;
            }

            return false;
        }
        
        #region Function
        
        public string InsertRet ( string strInset )
        {
            string strSql;
            DataTable dt = new DataTable ();

            strSql = "SET NOCOUNT ON ; " + strInset + " ; SELECT @@IDENTITY ";
            dt = ExecDataTable ( strSql );
            if ( dt == null || dt.Rows.Count <= 0 )
                return "";
            return dt.Rows [ 0 ] [ 0 ].ToString ();
        }

        public int GetPageMaxPage ( String strTab , String strWhere , ref int nRecCount , int nPageSize )
        {
            string strSql;
            DataTable dt = new DataTable ();

            nRecCount = 0;
            int nPageNum;

            strWhere = GetSafeStr ( strWhere );
            if ( String.IsNullOrEmpty ( strWhere ) )
                strSql = String.Format ( "select count(*) from {0} " , strTab );
            else
                strSql = String.Format ( "select count(*) from {0} where {1} " , strTab , strWhere );

 
            dt = ExecDataTable ( strSql );
            if ( dt == null || dt.Rows.Count <= 0 )
                return 0;

            try
            {
                nRecCount = int.Parse ( dt.Rows [ 0 ] [ 0 ].ToString ().Trim () );
            }
            catch
            {
                return 0;
            }

            nPageNum = ( nRecCount / nPageSize );
            if ( nRecCount % nPageSize != 0 )
                nPageNum++;

            return nPageNum;
        }

        public int GetPageMaxPage ( String strTab , ref int nRecCount , int nPageSize )
        {
            return GetPageMaxPage ( strTab , "" , ref nRecCount , nPageSize );
        }

        public int GetPageMaxPage_SqlStr( String strSql, ref int nRecCount, int nPageSize )
        {
            DataTable dt = new DataTable();

            nRecCount = 0;
            int nPageNum;


            dt = ExecDataTable( strSql );
            if ( dt == null || dt.Rows.Count <= 0 )
                return 0;

            try
            {
                nRecCount = int.Parse( dt.Rows[ 0 ][ 0 ].ToString().Trim() );
            }
            catch
            {
                return 0;
            }

            nPageNum = ( nRecCount / nPageSize );
            if ( nRecCount % nPageSize != 0 )
                nPageNum++;

            return nPageNum;
        }

        public static string GetPageSql ( string strTable , int nPage , int nPageSize , string strID , string strWhere , bool bAsc )
        {
            string strOrder = (bAsc) ? "asc" : "desc";

            int PageNum_Size = nPage * nPageSize;
            string strSql = "";

            strWhere = GetSafeStr ( strWhere );
            strWhere = strWhere.Trim ();
            if ( strWhere != "" )
            {
                strSql = String.Format ( "SELECT TOP {2} * FROM {0} WHERE ( {3} NOT IN ( SELECT TOP {5} {3} FROM {0} WHERE {4} ORDER BY {3} {6})) AND {4} ORDER BY {3} {6}", strTable, nPage, nPageSize, strID, strWhere, PageNum_Size, strOrder );
            }
            else
            {
                strSql = String.Format ( "SELECT TOP {2} * FROM {0} WHERE ( {3} NOT IN ( SELECT TOP {5} {3} FROM {0}  ORDER BY {3} {6})) ORDER BY {3} {6}", strTable, nPage, nPageSize, strID, "", PageNum_Size, strOrder );
            }

            return strSql;
        }
        public static string GetPageSql ( string strTable, int nPage, int nPageSize, string strID, string strWhere )
        {
            return GetPageSql(strTable, nPage, nPageSize, strID, strWhere, true );
        }
        public static string GetPageSql_Desc ( string strTable, int nPage, int nPageSize, string strID, string strWhere )
        {
            return GetPageSql ( strTable, nPage, nPageSize, strID, strWhere, false );
        }

        public static string GetPageSql ( string strAllCol , string strTab1 , string strKey1 , string strTab2 , string strKey2 , int nPage , int nPageSize , string strID , string strWhere )
        {
            int PageNum_Size = nPage * nPageSize;
            string strSql = "";

            strWhere = GetSafeStr ( strWhere );
            strWhere = strWhere.Trim ();
            if ( strWhere != "" )
            {
                strSql = String.Format ( "SELECT TOP {2} {6} FROM {7} INNER JOIN {9} ON {7}.{8} = {9}.{10} WHERE ( {3} NOT IN ( SELECT TOP {5} {6} FROM {7} INNER JOIN {9} ON {7}.{8} = {9}.{10} WHERE {4} ORDER BY {3})) AND {4} ORDER BY {3} " , "" , nPage , nPageSize , strID , strWhere , PageNum_Size , strAllCol , strTab1 , strKey1 , strTab2 , strKey2 );
            }
            else
            {
                strSql = String.Format ( "SELECT TOP {2} {6} FROM {7} INNER JOIN {9} ON {7}.{8} = {9}.{10} WHERE ( {3} NOT IN ( SELECT TOP {5} {6} FROM {7} INNER JOIN {9} ON {7}.{8} = {9}.{10}  ORDER BY {3})) ORDER BY {3} " , nPage , nPageSize , strID , "" , PageNum_Size , strAllCol , strTab1 , strKey1 , strTab2 , strKey2 );
            }

            return strSql;
        }

        public static string GetPageSql2005(string strTable, int nPage, int nPageSize, string strKeyFld, string strWhere, bool bAsc)
        {
            int nStrat = nPage * nPageSize + 1;  //ROW_NUMBER ����1��ʼ�ġ�
            int nEnd = nStrat + nPageSize;
            string strOrder = (bAsc) ? "asc" : "desc";
            strWhere = (strWhere.Trim() == "") ? "" : "Where " + strWhere;

            string strSql = String.Format("select * from ( SELECT  ROW_NUMBER() OVER (ORDER BY {0} {1}) as RowNum, * FROM {2} {3} ) as T where RowNum >= {4} AND RowNum < {5}", strKeyFld, strOrder, strTable, strWhere, nStrat, nEnd);
            return strSql;
        }
        public static string GetPageSql2005(string strTable, int nPage, int nPageSize, string strKeyFld, string strWhere)  //Ĭ���ǽ�������
        {
            return GetPageSql2005(strTable, nPage, nPageSize, strKeyFld, strWhere, false);
        }

        #endregion

        #region private

        static string GetSafeStr ( string str )
        {
            return str;

            /*
            string strRet = str.Replace ( "'" , "''" ) ;
            return strRet;
             */ 
        }

        #endregion
    }

    public class SQLPARAM
    {
        private bool bLocal = false;
        private string LocalServerName = "MSDE";   //���ڶ�ȡ����MSDE���ݿ⣻�������ڱ��غ�Զ�����ݿ��л������粻�ȶ�ʱ���л������ء�

        private string strIP = "";
        private string strDB = "";
        private string strUser = "";
        private string strPass = "";
        private string strConnect = "";


        public bool LOCAL
        {
            set { bLocal = value; }
            get { return bLocal; }
        }

        public string LOCALSERVERNAME
        {
            set { LocalServerName = value; }
            get { return LocalServerName; }

        }

        public string IP
        {
            set { strIP = value; }
            get { return strIP; }

        }

        public string DB
        {
            set { strDB = value; }
            get { return strDB; }

        }

        public string User
        {
            set { strUser = value; }
            get { return strUser; }

        }

        public string Pass
        {
            set { strPass = value; }
            get { return strPass; }

        }

        public string ConnectStr
        {
            get
            {
                if ( bLocal )
                    strConnect = String.Format ( "data source=(local)\\{0};initial catalog={1};uid={2};pwd={3};Connect Timeout={4}", LocalServerName, strDB, strUser, strPass, 120 );   //ԭ����30s,��CustomerList����2ҳ��ʱ�����1�ֶ��ӡ�
                //strConnect = String.Format("data source=(local);initial catalog={1};uid={2};pwd={3};Connect Timeout={4}", LocalServerName, strDB, strUser, strPass, 120);   //ԭ����30s,��CustomerList����2ҳ��ʱ�����1�ֶ��ӡ�
                else
                    strConnect = String.Format ( "data source={0};initial catalog={1};uid={2};pwd={3};Connect Timeout={4}", strIP, strDB, strUser, strPass, 120 );   //ԭ����30s,��CustomerList����2ҳ��ʱ�����1�ֶ��ӡ�

                return strConnect;
            }
        }

        /// <summary>
        /// ȡ��ͬһSQL������һ�����ݿ�������ִ������ڿ���ȡ�����ݡ���һ����Ŀ��ʹ�ö�����ݿ⣩
        /// </summary>
        /// <param name="strOtherDB"></param>
        /// <returns></returns>
        public string GetConnectStr ( string strOtherDB )
        {
            string str = "";

            if ( bLocal )
                str = String.Format ( "data source=(local)\\{0};initial catalog={1};uid={2};pwd={3};Connect Timeout={4}", LocalServerName, strOtherDB, strUser, strPass, 120 );   //ԭ����30s,��CustomerList����2ҳ��ʱ�����1�ֶ��ӡ�
            //str = String.Format("data source=(local);initial catalog={1};uid={2};pwd={3};Connect Timeout={4}", LocalServerName, strOtherDB, strUser, strPass, 120);   //ԭ����30s,��CustomerList����2ҳ��ʱ�����1�ֶ��ӡ�
            else
                str = String.Format ( "data source={0};initial catalog={1};uid={2};pwd={3};Connect Timeout={4}", strIP, strOtherDB, strUser, strPass, 120 );   //ԭ����30s,��CustomerList����2ҳ��ʱ�����1�ֶ��ӡ�

            return str;
        }

    }

    public class SQLITEPARAM
    {
        private string _strDBFile = "";
        private string _strPassword = "_mogultech_key_";

        #region ����

        public string DBFile
        {
            set { _strDBFile = value; }
            get { return _strDBFile; }
        }

        public string Password
        {
            get { return _strPassword; }
        }

        #endregion
    }

}
