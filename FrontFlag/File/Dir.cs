using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FrontFlag
{
    public class DIR
    {
        //��ֱ�ӽ������Ŀ¼,���� c:\a\b\c 
        public bool CreateDir ( string strDir )
        {
            char [ ] chsInvalid = new char [ ] { '/' , ':' , '*' , '?' , '"' , '<' , '>' , '|' };   //\�������˷ָ��������Բ����ˡ�
            string strtmp;
            foreach ( char c in chsInvalid )
            {
                strtmp = String.Format ( "{0}" , c );
                if ( strDir.Contains ( strtmp ) )
                    return false;
            }

            strDir = strDir.Replace ( @"\\" , "?" );    //Ϊ��֧�־�������������·����ͷ��\\,��ᱻ�����Split�ֽ����

            char [ ] chs = new char [ ] { '\\' };
            string [ ] strsDir = strDir.Split ( chs );

            if ( strsDir.Length > 0 )
                strsDir [ 0 ] = strsDir [ 0 ].Replace ( "?" , @"\\" );    //��ԭ������·������ͷ\\,

            try
            {
                string strPath = "";
                foreach ( string str in strsDir )
                {
                    if ( str.Trim () == String.Empty )
                        continue;

                    strPath = Path.Combine ( strPath , str );

                    if ( str.Contains ( @"\\" ) )       //�Ǿ�����·������ͷ�������жϣ���Ϊ���Ƿ���false, Exists()���ܶԾ������ĸ�·���б𡣴ӵڶ���Ŀ¼��ʼ�жϡ�Ĭ�ϴ���\\xx.xx.xx.xx����ȷ�ġ�
                        continue;

                    if ( !Directory.Exists ( strPath ) )
                        Directory.CreateDirectory ( strPath );
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
