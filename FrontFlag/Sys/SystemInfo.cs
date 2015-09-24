using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Management;

namespace FrontFlag
{
    public class SYSTEMINFO
    {
        /*
        * LayoutKind.Automatic��Ϊ�����Ч����������̬�����ͳ�Ա��������
        * ע�⣺��Զ��Ҫʹ�����ѡ�������ò��ܹ�Ͻ�Ķ�̬���ӿ⺯����
        * LayoutKind.Explicit����ÿ������FieldOffset���Զ����ͳ�Ա����
        * LayoutKind.Sequential���Գ������ܹ�Ͻ���Ͷ���ط��Ĳ��ܹ�Ͻ�ڴ��е����ͳ�Ա��������
        */
        /// <summary>
        /// ����CPU����Ϣ�ṹ
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CpuInfo
        {
             /// <summary>
             /// OEM ID
             /// </summary>
             public uint dwOemId;
             /// <summary>
             /// ҳ���С
             /// </summary>
             public uint dwPageSize;
             public uint lpMinimumApplicationAddress;
             public uint lpMaximumApplicationAddress;
             public uint dwActiveProcessorMask;
             /// <summary>
             /// CPU����
             /// </summary>
             public uint dwNumberOfProcessors;
             /// <summary>
             /// CPU����
             /// </summary>
             public uint dwProcessorType;
             public uint dwAllocationGranularity;
             /// <summary>
             /// CPU�ȼ�
             /// </summary>
             public uint dwProcessorLevel;
             public uint dwProcessorRevision;
        }

        /// <summary>
        /// �����ڴ����Ϣ�ṹ
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryInfo
        {
             /// <summary>
             ///
             /// </summary>
             public uint dwLength;
             /// <summary>
             /// �Ѿ�ʹ�õ��ڴ�
             /// </summary>
             public uint dwMemoryLoad;
             /// <summary>
             /// �������ڴ��С
             /// </summary>
             public uint dwTotalPhys;
             /// <summary>
             /// ���������ڴ��С
             /// </summary>
             public uint dwAvailPhys;
             /// <summary>
             /// �����ļ��ܴ�С
             /// </summary>
             public uint dwTotalPageFile;
             /// <summary>
             /// ���ý����ļ���С
             /// </summary>
             public uint dwAvailPageFile;
             /// <summary>
             /// �������ڴ��С
             /// </summary>
             public uint dwTotalVirtual;
             /// <summary>
             /// ���������ڴ��С
             /// </summary>
             public uint dwAvailVirtual;
        }

        /// <summary>
        /// ����ϵͳʱ�����Ϣ�ṹ
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTimeInfo
        {
             /// <summary>
             /// ��
             /// </summary>
             public ushort wYear;
             /// <summary>
             /// ��
             /// </summary>
             public ushort wMonth;
             /// <summary>
             /// ����
             /// </summary>
             public ushort wDayOfWeek;
             /// <summary>
             /// ��
             /// </summary>
             public ushort wDay;
             /// <summary>
             /// Сʱ
             /// </summary>
             public ushort wHour;
             /// <summary>
             /// ����
             /// </summary>
             public ushort wMinute;
             /// <summary>
             /// ��
             /// </summary>
             public ushort wSecond;
             /// <summary>
             /// ����
             /// </summary>
             public ushort wMilliseconds;
        }

        private const int CHAR_COUNT = 128;

        public SYSTEMINFO ()
         {
         }

         [DllImport("kernel32")]
         private static extern void GetWindowsDirectory(StringBuilder WinDir, int count);

         [DllImport("kernel32")]
         private static extern void GetSystemDirectory(StringBuilder SysDir, int count);

         [DllImport("kernel32")]
         private static extern void GetSystemInfo(ref CpuInfo cpuInfo);

         [DllImport("kernel32")]
         private static extern void GlobalMemoryStatus(ref MemoryInfo memInfo);

         [DllImport("kernel32")]
         private static extern void GetSystemTime(ref SystemTimeInfo sysInfo);

         #region CPU

         /**/
         /// <summary>
         /// ��ѯCPU���
         /// </summary>
         /// <returns></returns>
         public string GetCpuId()
         {
             ManagementClass mClass = new ManagementClass("Win32_Processor");
             ManagementObjectCollection moc = mClass.GetInstances();
             string cpuId=null;
             foreach (ManagementObject mo in moc)
             {
                 cpuId = mo.Properties["ProcessorId"].Value.ToString();
                 break;
             }
             return cpuId;
         }

        //��ȡ�����CPU�ĵ�ǰʱ��Ƶ��
        public string GetCpuCurrentClockSpeed ()
        {
            ManagementClass mClass = new ManagementClass ( "Win32_Processor" );
            ManagementObjectCollection moc = mClass.GetInstances ();
            string strCurrentClockSpeed = null;
            foreach ( ManagementObject mo in moc )
            {
                float fHZ = ( ( float )Convert.ToInt16 ( mo.Properties [ "CurrentClockSpeed" ].Value ) ) / 1000 ;
                strCurrentClockSpeed = fHZ.ToString () + "GHz";
                break;
            }
            return strCurrentClockSpeed;
        }

        //��ȡ�����CPU�����ʱ��Ƶ��
        public string GetCpuMaxClockSpeed ()
        {
            ManagementClass mClass = new ManagementClass ( "Win32_Processor" );
            ManagementObjectCollection moc = mClass.GetInstances ();
            string strMaxClockSpeed = null;
            foreach ( ManagementObject mo in moc )
            {
                float fHZ = ( ( float ) Convert.ToInt16 ( mo.Properties [ "MaxClockSpeed" ].Value ) ) / 1000;
                strMaxClockSpeed = fHZ.ToString () + "GHz";
                break;
            }
            return strMaxClockSpeed;
        }

        //��ȡ�����CPU��ռ����
        public string GetCpuLoadPercentage ()
        {
            ManagementClass mClass = new ManagementClass ( "Win32_Processor" );
            ManagementObjectCollection moc = mClass.GetInstances ();
            string strLoadPercentage = null;
            foreach ( ManagementObject mo in moc )
            {
                strLoadPercentage = mo.Properties [ "LoadPercentage" ].Value.ToString () + "%";
                break;
            }
            return strLoadPercentage;
        }

        //��ȡ�����CPU�İ汾; eg:�ͺ�4 ����9
        public string GetCpuVersion ()
        {
            ManagementClass mClass = new ManagementClass ( "Win32_Processor" );
            ManagementObjectCollection moc = mClass.GetInstances ();
            string strVersion = null;
            foreach ( ManagementObject mo in moc )
            {
                strVersion = mo.Properties [ "Version" ].Value.ToString () ;
                break;
            }
            return strVersion;
        }

        //��ȡ�����CPU�Ĳ�Ʒ���� ; eg:"Intel(R) Pentium(R) 4 CPU 2.80GHz"
        public string GetCpuName ()
        {
            ManagementClass mClass = new ManagementClass ( "Win32_Processor" );
            ManagementObjectCollection moc = mClass.GetInstances ();
            string strName = null;
            foreach ( ManagementObject mo in moc )
            {
                strName = mo.Properties [ "Name" ].Value.ToString ();
                break;
            }
            return strName.Trim ();
        }

        //��ȡ�����CPU�������� 
        public string GetCpuManufacturer ()
        {
            ManagementClass mClass = new ManagementClass ( "Win32_Processor" );
            ManagementObjectCollection moc = mClass.GetInstances ();
            string strManufacturer = null;
            foreach ( ManagementObject mo in moc )
            {
                strManufacturer = mo.Properties [ "Manufacturer" ].Value.ToString ();
                break;
            }
            return strManufacturer;
        }

        //��ȡ�����CPU�ĵ�ǰ��ѹ 
        public string GetCpuCurrentVoltage ()
        {
            ManagementClass mClass = new ManagementClass ( "Win32_Processor" );
            ManagementObjectCollection moc = mClass.GetInstances ();
            string strCurrentVoltage = null;
            foreach ( ManagementObject mo in moc )
            {
                strCurrentVoltage = mo.Properties [ "CurrentVoltage" ].Value.ToString ();
                break;
            }
            return strCurrentVoltage;
        }

        #endregion

         #region Memo

        /**/
        /// <summary>
        /// ��ȡϵͳ�ڴ���Ϣ
        /// </summary>
        /// <returns></returns>
        public MemoryInfo GetMemoryInfo ()
        {
            MemoryInfo memoryInfo = new MemoryInfo ();
            GlobalMemoryStatus ( ref memoryInfo );
            return memoryInfo;
        }

        //��ȡ������������ڴ�
        public string GetMemoryTotalPhysical ()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher ( "Select * From Win32_LogicalMemoryConfiguration" );
            String strTotalPhysicalMemory = null;
            foreach ( ManagementObject mo in searcher.Get () )
            {
                strTotalPhysicalMemory = mo [ "TotalPhysicalMemory" ].ToString ().Trim ();
                break;
            }
            return  ( strTotalPhysicalMemory ) ;
        }

        //��ȡ������������ڴ�
        public string GetMemoryTotalVirtual ()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher ( "Select * From Win32_LogicalMemoryConfiguration" );
            String strTotalVirtualMemory = null;
            foreach ( ManagementObject mo in searcher.Get () )
            {
                strTotalVirtualMemory = mo [ "TotalVirtualMemory" ].ToString ().Trim ();
                break;
            }
            return  ( strTotalVirtualMemory );
        }

        //��ȡ�������ҳ���ļ�
        public string GetMemoryTotalPageFileSpace ()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher ( "Select * From Win32_LogicalMemoryConfiguration" );
            String strTotalPageFileSpace = null;
            foreach ( ManagementObject mo in searcher.Get () )
            {
                strTotalPageFileSpace = mo [ "TotalPageFileSpace" ].ToString ().Trim ();
                break;
            }
            return strTotalPageFileSpace;
        }

        #endregion

         #region Disk

        /**/
        /// <summary>
         /// ��ѯӲ�̱��
         /// </summary>
         /// <returns></returns>
         public string GetDiskId()
         {
             ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
             String hardDiskID=null;
             foreach (ManagementObject mo in searcher.Get())
             {
                 hardDiskID = mo["SerialNumber"].ToString().Trim();
                 break;
             }
             return hardDiskID;
         }

         //���ָ�����̵����� ; eg: param ="c:"
        public string GetDiskSize ( string strDiskSign )
        {
            String strDiskSize = null; 
            string strWSL = String.Format ( "select * from win32_logicaldisk where Name='{0}'" , strDiskSign );
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher ( strWSL );

                foreach ( ManagementObject mo in searcher.Get () )
                {
                    strDiskSize = mo [ "Size" ].ToString ().Trim ();
                    break;
                }
            }
            catch ( Exception e )
            {
                return e.Message;
            }

            return GetSizeUseUnit ( strDiskSize );
        }

        //���ָ�����̵�ʣ������ ; eg: param ="c:"
        public string GetDiskFreeSpace ( string strDiskSign )
        {
            String strDiskFreeSpace = null;
            string strWSL = String.Format ( "select * from win32_logicaldisk where Name='{0}'" , strDiskSign );
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher ( strWSL );

                foreach ( ManagementObject mo in searcher.Get () )
                {
                    strDiskFreeSpace = mo [ "FreeSpace" ].ToString ().Trim ();
                    break;
                }
            }
            catch ( Exception e )
            {
                return e.Message;
            }

            return GetSizeUseUnit ( strDiskFreeSpace );
        }

        #endregion

         #region Opreat

        /**/
        /// <summary>
        /// ��ȡϵͳ����
        /// </summary>
        /// <returns></returns>
        public string GetOperationSystemInName ()
        {
            OperatingSystem os = System.Environment.OSVersion;
            string osName = "UNKNOWN";
            switch ( os.Platform )
            {
                case PlatformID.Win32Windows:
                    switch ( os.Version.Minor )
                    {
                        case 0: osName = "Windows 95"; break;
                        case 10: osName = "Windows 98"; break;
                        case 90: osName = "Windows ME"; break;
                    }
                    break;
                case PlatformID.Win32NT:
                    switch ( os.Version.Major )
                    {
                        case 3: osName = "Windws NT 3.51"; break;
                        case 4: osName = "Windows NT 4"; break;
                        case 5: if ( os.Version.Minor == 0 )
                            {
                                osName = "Windows 2000";
                            }
                            else if ( os.Version.Minor == 1 )
                            {
                                osName = "Windows XP";
                            }
                            else if ( os.Version.Minor == 2 )
                            {
                                osName = "Windows Server 2003";
                            }
                            break;
                        case 6: osName = "Longhorn"; break;
                    }
                    break;
            }
            return String.Format ( "{0},{1}" , osName , os.Version.ToString () );
        }

        //��ȡ����ϵͳ����
        public string GetOperationSystem ()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher ( "SELECT * FROM Win32_OperatingSystem" );
            String strOperationSystem = null;
            foreach ( ManagementObject mo in searcher.Get () )
            {
                strOperationSystem = mo [ "Caption" ].ToString ().Trim ();
                break;
            }
            return strOperationSystem;
        }

        //��ȡ���������
        public string GetOperationPCName ()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher ( "SELECT * FROM Win32_OperatingSystem" );
            String strPCName = null;
            foreach ( ManagementObject mo in searcher.Get () )
            {
                strPCName = mo [ "csname" ].ToString ().Trim ();
                break;
            }
            return strPCName;
        }

        //��ȡ����ϵͳ�汾
        public string GetOperationVersion ()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher ( "SELECT * FROM Win32_OperatingSystem" );
            String strVersion = null;
            foreach ( ManagementObject mo in searcher.Get () )
            {
                strVersion = mo [ "Version" ].ToString ().Trim ();
                break;
            }
            return strVersion;
        }

        //��ȡ����ϵͳ����������
        public string GetOperationManufacturer ()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher ( "SELECT * FROM Win32_OperatingSystem" );
            String strManufacturer = null;
            foreach ( ManagementObject mo in searcher.Get () )
            {
                strManufacturer = mo [ "Manufacturer" ].ToString ().Trim ();
                break;
            }
            return strManufacturer;
        }
        #endregion

         #region Path

         /**/
         /// <summary>
         /// ��ȡWindowsĿ¼
         /// </summary>
         /// <returns></returns>
         public string GetWinDirectory()
         {
             StringBuilder sBuilder = new StringBuilder(CHAR_COUNT);
             GetWindowsDirectory(sBuilder, CHAR_COUNT);
             return sBuilder.ToString();
         }

         /**//// <summary>
         /// ��ȡϵͳĿ¼
         /// </summary>
         /// <returns></returns>
         public string GetSysDirectory()
         {
             StringBuilder sBuilder = new StringBuilder(CHAR_COUNT);
             GetSystemDirectory(sBuilder, CHAR_COUNT);
             return sBuilder.ToString();
         }

        /**//// <summary>
        /// ��ȡCPU��Ϣ
        /// </summary>
        /// <returns></returns>
         public CpuInfo GetCpuInfo()
         {
             CpuInfo cpuInfo = new CpuInfo();
             GetSystemInfo(ref cpuInfo);
             return cpuInfo;
         }

        #endregion

         #region Time

         /**/
         /// <summary>
         /// ��ȡϵͳʱ����Ϣ
         /// </summary>
         /// <returns></returns>
         public SystemTimeInfo GetSystemTimeInfo()
         {
             SystemTimeInfo systemTimeInfo = new SystemTimeInfo();
             GetSystemTime(ref systemTimeInfo);
             return systemTimeInfo;
         }

         #endregion

         #region private

        private string GetSizeUseUnit ( string size )
        {
            double dSpace = Convert.ToDouble ( size );
            string sSpace = dSpace.ToString ( "N" );
            string [ ] tmp;
            string rtnSize = "0";
            tmp = sSpace.Split ( ',' );
            switch ( tmp.GetUpperBound ( 0 ) )
            {
                case 0:
                    rtnSize = tmp [ 0 ] + " �ֽ�";
                    break;
                case 1:
                    rtnSize = tmp [ 0 ] + "." + tmp [ 1 ].Substring ( 0 , 2 ) + " K";
                    break;
                case 2:
                    rtnSize = tmp [ 0 ] + "." + tmp [ 1 ].Substring ( 0 , 2 ) + " M";
                    break;
                case 3:
                    rtnSize = tmp [ 0 ] + "." + tmp [ 1 ].Substring ( 0 , 2 ) + " G";
                    break;
                case 4:
                    rtnSize = tmp [ 0 ] + "." + tmp [ 1 ].Substring ( 0 , 2 ) + " T";
                    break;
            }
            return rtnSize;
        } 

        #endregion
     }
}
