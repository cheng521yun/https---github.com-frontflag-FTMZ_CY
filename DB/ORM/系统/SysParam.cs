﻿

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;

using FrontFlag;


namespace DB.Orm.系统
{
    public class SysParam
    {
        DB.DA.系统.SysParam da = new DB.DA.系统.SysParam();

        #region Get

        public DataTable GetBlank()
        {
            return da.GetBlank();
        }

        public List<DB.Stru.系统.SysParam> Get_ByWhere( string strWhere )
        {
            List<DB.Stru.系统.SysParam> lst = new List<DB.Stru.系统.SysParam>();

            DataTable dt = da.GetWhere( strWhere );

            if ( SQL.IsNotValid( ref dt ) )
                return lst;

            lst = DB.Stru.系统.SysParam.Dt2List( ref dt );

            return lst;
        }

        public DB.Stru.系统.SysParam GetFirst_ByWhere( string strWhere )
        {
            DB.Stru.系统.SysParam stru = new DB.Stru.系统.SysParam();
            List<DB.Stru.系统.SysParam> list = Get_ByWhere( strWhere );

            if ( list != null && list.Count > 0 )
                stru = list[ 0 ];

            return stru;
        }

        public DB.Stru.系统.SysParam Get_ByID( string strID )
        {
            string strWhere = String.Format( "{0}='{1}'", DB.Tab.系统.SysParam.ID, strID );
            return GetFirst_ByWhere( strWhere );
        }

        #endregion

        #region Split to Page

        //Get one Page data from all 
        public List<DB.Stru.系统.SysParam> GetPage( int nPageNo, string strWhere )
        {
            List<DB.Stru.系统.SysParam> lst = new List<DB.Stru.系统.SysParam>();

            DataTable dt = da.GetPage( nPageNo, strWhere );

            if ( SQL.IsNotValid( ref dt ) )
                return lst;

            lst = DB.Stru.系统.SysParam.Dt2List( ref dt );

            return lst;
        }

        //Get Page Number  
        public int GetPageMax( string strWhere )
        {
            return da.GetPageMax( strWhere );
        }

        #endregion

        #region Save
        public string Save( ref DataTable dt )
        {
            string strID = da.Save( ref dt );
            return strID;
        }

        public string Save( DB.Stru.系统.SysParam stru )
        {
            DataTable dt = GetBlank();
            DataRow dr = dt.NewRow();
            stru.Stru2Dr( ref dr );
            dt.Rows.Add( dr );
            string strID = da.Save( ref dt );
            return strID;
        }

        public bool Save( List<DB.Stru.系统.SysParam> lst )
        {
            bool bRet = true;
            string strID;
            foreach ( DB.Stru.系统.SysParam stru in lst )
            {
                strID = Save( stru );
                bRet &= ( strID == "" ) ? false : true;
            }
            return bRet;
        }

        #endregion

        #region Delete

        public void Delete( DB.Stru.系统.SysParam stru )
        {
            da.Delete_ByID( stru.ID );
        }

        public void Delete( List<string> lstDel )
        {
            foreach ( string strID in lstDel )
                da.Delete_ByID( strID );
        }

        public void Delete( List<DB.Stru.系统.SysParam> lst )
        {
            foreach ( DB.Stru.系统.SysParam stru in lst )
                da.Delete_ByID( stru.ID );
        }

        #endregion



        #region Update

        public void Update_ByID( string strID, string strFld, string strVal )
        {
            da.Update_ByID( strID, strFld, strVal );
        }

        #endregion

        #region Sum

        public int GetCount( string strWhere )
        {
            int nCount = 0;
            DataTable dt = da.GetCount( strWhere );

            if ( SQL.IsValid( ref dt ) )
            {
                string strCount = dt.Rows[ 0 ][ 0 ].ToString();
                nCount = FF.Fun.MyConvert.Str2Int( strCount );
            }

            return nCount;
        }

        public int GetSum( string strWhere )
        {
            int nCount = 0;
            DataTable dt = da.GetCount( strWhere );

            if ( SQL.IsValid( ref dt ) )
            {
                string strCount = dt.Rows[ 0 ][ 0 ].ToString();
                nCount = FF.Fun.MyConvert.Str2Int( strCount );
            }

            return nCount;
        }

        #endregion
    }


}


