using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FrontFlag
{
    public class FILE
    {
        /// <summary>
        /// ѡ��һ���ļ���
        /// </summary>
        /// <returns></returns>
        public string SelectPath ()
        {
            FolderBrowserDialog Dlg = new FolderBrowserDialog();
            DialogResult ret = Dlg.ShowDialog();

            if (ret != DialogResult.OK)
                return "" ;

            return Dlg.SelectedPath;
        }

        /// <summary>
        /// ��ϵͳ�Ի���ѡ��һ���ļ���
        /// </summary>
        /// <param name="strFileType">�ļ����ͺ�׺����Ҫ���㡣eg:"jpg"</param>
        /// <returns></returns>
        public string SelectFile( string strFileType )
        {
            string strCurPath = Environment.CurrentDirectory;

            OpenFileDialog Dlg = new System.Windows.Forms.OpenFileDialog();

            string strFieTypeCause = "";
            if (strFileType.Trim() != String.Empty )
                strFieTypeCause = String.Format("{0} files (*.{0})|*.{0}|", strFileType);

            Dlg.DefaultExt = "*.*";
            Dlg.Filter = string.Format( "{0}All files (*.*)|*.*", strFieTypeCause);
            Dlg.InitialDirectory = Application.StartupPath;
            Dlg.Multiselect = true;

            DialogResult ret = Dlg.ShowDialog();

            Environment.CurrentDirectory = strCurPath;

            if (ret != DialogResult.OK)
                return "";

            return Dlg.FileName;
        }


        /// <summary>
        /// ѡ���������͵��ļ�
        /// </summary>
        /// <returns></returns>
        public string SelectFile()
        {
            return SelectFile("");
        }

        /// <summary>
        /// ��ϵͳ�Ի���ѡ��һ�������ļ���
        /// </summary>
        /// <param name="strFileType">�ļ����ͺ�׺����Ҫ���㡣eg:"jpg"</param>
        /// <returns></returns>
        public string SelectSaveFile(string strFileType)
        {
            string strCurPath = Environment.CurrentDirectory;

            SaveFileDialog Dlg = new System.Windows.Forms.SaveFileDialog();

            string strFieTypeCause = "";
            if (strFileType.Trim() != String.Empty)
                strFieTypeCause = String.Format("{0} files (*.{0})|*.{0}|", strFileType);

            Dlg.DefaultExt = "*.*";
            Dlg.Filter = string.Format("{0}All files (*.*)|*.*", strFieTypeCause);
            Dlg.InitialDirectory = Application.StartupPath;

            DialogResult ret = Dlg.ShowDialog();

            Environment.CurrentDirectory = strCurPath;

            if (ret != DialogResult.OK)
                return "";

            return Dlg.FileName;
        }

        public string Read ( string strFile )
        {
            string strRet = "";
            try
            {
                using ( StreamReader sr = new StreamReader ( strFile ) )
                {
                    String strline;
                    while ( ( strline = sr.ReadLine ( ) ) != null )
                    {
                        strRet += strline;
                    }
                }
            }
            catch ( Exception e )
            {
                FF.Ctrl.MsgBox.ShowWarn ( "CANNOT Open the file : " + strFile );
            }

            return strRet;
        }
    }
}
