using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace FrontFlag
{
    public class XML
    {
        XmlDocument doc = new XmlDocument ();

        public void FilterInvalidCh ( ref string strOrg )
        {
            strOrg = strOrg.Replace ( '(' , '_' );
            strOrg = strOrg.Replace ( ')' , '_' );
            strOrg = strOrg.Replace ( ' ' , '_' );
        }

        public bool ReadParam ( string strFileName , string strParams , ref string strValue )
        {
            string strXml = "";
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader ( strFileName , System.Text.Encoding.Default );
                strXml = sr.ReadToEnd ();
                sr.Close ();
            }
            catch ( Exception e )
            {
                //string strMsg = String.Format("�޷���ȷ�� {0} �ļ�!\nԭ��:{1}", strFileName, e.Message);
                //FF.Ctrl.MsgBox.ShowWarn ( strMsg );
                return false;
            }

            string strNextPrams = "";
            string strCurParam = "";
            bool bFindChid = GetParam ( strParams , ref strNextPrams , ref strCurParam );

            //XmlDocument doc = new XmlDocument ( );  //ΪʲĩҪ����?
            doc.LoadXml ( strXml );
            XmlNodeList nodeList = doc.GetElementsByTagName ( strCurParam );

            XmlElement xe = ( XmlElement ) nodeList [ 0 ];

            return GetValue ( xe , strNextPrams , ref strValue );
        }

        bool GetValue ( XmlElement xeFather , string strParams , ref string strValue )
        {
            strValue = "";

            string strNextPrams = "";
            string strCurParam = "";
            bool bFindChid = GetParam ( strParams , ref strNextPrams , ref strCurParam );

            XmlNodeList nodeList = xeFather.GetElementsByTagName ( strCurParam );

            foreach ( XmlNode xn in nodeList ) //���������ӽڵ� 
            {
                XmlElement xe = ( XmlElement ) xn; //���ӽڵ�����ת��ΪXmlElement���� 
                if ( xe.Name == strCurParam )
                {
                    if ( !bFindChid )
                    {
                        strValue = xe.InnerText;
                        return true;
                    }
                    else
                    {
                        //xe.GetElementsByTagName
                        //strXml = xe.InnerXml;
                        return GetValue ( xe , strNextPrams , ref strValue );
                    }
                }
            }
            return false;
        }

        //
        public bool WriteParam ( string strFileName , string strParams , string strValue )
        {
            string strXml = "";
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader ( strFileName , System.Text.Encoding.Default );
                strXml = sr.ReadToEnd ();
                sr.Close ();
            }
            catch ( Exception e )
            {
                //string strMsg = String.Format("�޷���ȷ�� {0} �ļ�!\nԭ��:{1}", strFileName , e.Message );
                //FF.Ctrl.MsgBox.ShowWarn(strMsg);
                return false;
            }

            string strNextPrams = "";
            string strCurParam = "";
            bool bFindChid = GetParam ( strParams , ref strNextPrams , ref strCurParam );

            doc.LoadXml ( strXml );
            XmlNodeList nodeList = doc.GetElementsByTagName ( strCurParam );

            XmlElement xe = ( XmlElement ) nodeList [ 0 ];
            bool bRet = SetValue ( ref xe , strNextPrams , strValue );

            if ( bRet )
                doc.Save ( strFileName );

            return bRet;
        }

        bool SetValue ( ref XmlElement xeFather , string strParams , string strValue )
        {
            string strNextPrams = "";
            string strCurParam = "";
            bool bFindChid = GetParam ( strParams , ref strNextPrams , ref strCurParam );

            XmlNodeList nodeList = xeFather.GetElementsByTagName ( strCurParam );

            bool bExsit = false;
            XmlElement xe = null;
            foreach ( XmlNode xn in nodeList ) //���������ӽڵ� 
            {
                xe = ( XmlElement ) xn; //���ӽڵ�����ת��ΪXmlElement���� 
                if ( xe.Name == strCurParam )
                {
                    bExsit = true;
                    if ( !bFindChid )
                    {
                        xe.InnerText = strValue;
                        return true;
                    }
                    else
                    {
                        return SetValue ( ref xe , strNextPrams , strValue );
                    }
                }
            }

            if ( !bExsit )
            { 
                if ( strCurParam != "" )
                {
                    XmlElement xNew = doc.CreateElement ( strCurParam );//����һ���ڵ�   
                    xeFather.AppendChild ( xNew );

                    //SetValue ( ref xNew , strNextPrams , strValue );
                    SetValue ( ref xeFather , strParams , strValue );

                    return true;
                }
            }

            return false;
        }

        #region Private

        //return true = �����ӽڵ� ; false=��û���ӽڵ��ˡ�
        bool GetParam ( string strParams , ref string strNextParams , ref string strCurParam )
        {
            bool bFindChid = false;
            strNextParams = "";
            strCurParam = "";

            char [ ] chs = new char [ ] { '/' };
            string [ ] strs = strParams.Split ( chs );
            if ( strs.Length <= 0 )
                return bFindChid;

            strCurParam = strs [ 0 ];

            if ( strs.Length > 1 )
            {
                bFindChid = true;
                for ( int i = 1 ; i < strs.Length ; i++ )
                {
                    if ( i > 1 )
                        strNextParams += "/";
                    strNextParams += strs [ i ];
                }
            }

            return bFindChid;
        }

        #endregion
    }
}
