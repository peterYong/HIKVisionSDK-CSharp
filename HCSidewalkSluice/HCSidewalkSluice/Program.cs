using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HCSidewalkSluice
{
    class Program
    {
        static void Main(string[] args)
        {

            SetAlarm();
             SendDownCardID();

            Console.ReadLine();
        }


        /// <summary>
        /// 下发卡参数（二维码）
        /// </summary>
        public static void SendDownCardID()
        {
            //可能有多台道闸
            //string DVRIPAddress = ConfigurationManager.AppSettings["ip"];
            string ipes = ConfigurationManager.AppSettings["ips"];
            string[] ipsTemp = ipes.Split('、');
            foreach (var item in ipsTemp)
            {
                //初始化
                bool isInited = HKCommonSDK.Init();
                if (isInited)
                {
                    CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", false);
                    Console.WriteLine("NET_DVR_Init Success！");
                }
                else
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string strErr = "NET_DVR_Init failed, error code= " + iLastErr;
                    Console.WriteLine(strErr);
                }


                string[] ipport = item.Split(':');
                if (ipport.Length != 2)
                {
                    Console.WriteLine("ip地址配置有误！应该是ip1:port1、ip2:port2");
                }
                string DVRIPAddress = ipport[0];
                ushort DVRPortNumber = ushort.Parse(ipport[1]);
                string DVRUserName = ConfigurationManager.AppSettings["UserName"];
                string DVRPassword = ConfigurationManager.AppSettings["Password"];
                int m_lUserID = HKCommonSDK.LoginV30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword);
                if (m_lUserID == -1)
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string strErr = "NET_DVR_Login_V30 failed, error code= " + iLastErr;
                    Console.WriteLine(strErr);
                }
                else
                {
                    Console.WriteLine("NET_DVR_Login_V30 Success,ip:port=" + DVRIPAddress + ":" + DVRPortNumber + "! m_lUserID=" + m_lUserID);
                }


                //HKControlGateWay.ControlGateWayOpen(m_lUserID);

                //启动长连接配置
                int m_lSetCardCfgHandle = HKSendDownCardCfg.StartRemoteConfig(m_lUserID);
                if (m_lSetCardCfgHandle == -1)
                {
                    Console.WriteLine("NET_DVR_StartRemoteConfig fail!error code= " + CHCNetSDK.NET_DVR_GetLastError());
                    CHCNetSDK.NET_DVR_Logout(m_lUserID);
                    CHCNetSDK.NET_DVR_Cleanup();
                }
                else
                {
                    Console.WriteLine("NET_DVR_StartRemoteConfig Success! m_lSetCardCfgHandle=" + m_lSetCardCfgHandle);
                }

                //下发卡号
                byte valid = (byte)(1 == 1 ? 1 : 0);
                string cardid = DateTime.Now.ToString("yyyyMMddHHmmss");

                bool isSended = HKSendDownCardCfg.SendRemoteConfig(m_lUserID, m_lSetCardCfgHandle, valid, cardid, new DateTime(2018, 7, 4, 0, 0, 0), new DateTime(2028, 7, 4, 0, 0, 0));
                if (!isSended)
                {
                    Console.WriteLine("NET_DVR_SendRemoteConfig fail!error code= " + CHCNetSDK.NET_DVR_GetLastError());
                    CHCNetSDK.NET_DVR_StopRemoteConfig(m_lSetCardCfgHandle);
                    CHCNetSDK.NET_DVR_Logout(m_lUserID);
                    CHCNetSDK.NET_DVR_Cleanup();
                }
                else
                {
                    Console.WriteLine("NET_DVR_SendRemoteConfig Success!");
                }
                //退出
                //Thread.Sleep(5000);
                //CHCNetSDK.NET_DVR_Logout(m_lUserID);
                //CHCNetSDK.NET_DVR_Cleanup();

                Console.WriteLine("-----------------------------------");
            }
        }

        /// <summary>
        /// 远程控制道闸门开关
        /// </summary>
        public static void ControlGateWay()
        {
            //可能有多台道闸
            //string DVRIPAddress = ConfigurationManager.AppSettings["ip"];
            string ipes = ConfigurationManager.AppSettings["ips"];
            string[] ipsTemp = ipes.Split('、');
            foreach (var item in ipsTemp)
            {
                //初始化
                bool isInited = HKCommonSDK.Init();
                if (isInited)
                {
                    CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", false);
                    Console.WriteLine("NET_DVR_Init Success！");
                }
                else
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string strErr = "NET_DVR_Init failed, error code= " + iLastErr;
                    Console.WriteLine(strErr);
                }

                string[] ipport = item.Split(':');
                if (ipport.Length != 2)
                {
                    Console.WriteLine("ip地址配置有误！应该是ip1:port1、ip2:port2");
                }
                string DVRIPAddress = ipport[0];
                ushort DVRPortNumber = ushort.Parse(ipport[1]);
                string DVRUserName = ConfigurationManager.AppSettings["UserName"];
                string DVRPassword = ConfigurationManager.AppSettings["Password"];
                int m_lUserID = HKCommonSDK.LoginV30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword);
                if (m_lUserID == -1)
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string strErr = "NET_DVR_Login_V30 failed, error code= " + iLastErr;
                    Console.WriteLine(strErr);
                }
                else
                {
                    Console.WriteLine("NET_DVR_Login_V30 Success,ip:port=" + DVRIPAddress + ":" + DVRPortNumber + "! m_lUserID=" + m_lUserID);
                }

                HKControlGateWay.ControlGateWayOpen(m_lUserID);
            }
        }

        /// <summary>
        /// 布防获取门禁
        /// </summary>
        public static void SetAlarm()
        {
            //可能有多台道闸
            //string DVRIPAddress = ConfigurationManager.AppSettings["ip"];
            string ipes = ConfigurationManager.AppSettings["ips"];
            string[] ipsTemp = ipes.Split('、');
            foreach (var item in ipsTemp)
            {
                //初始化
                bool isInited = HKCommonSDK.Init();
                if (isInited)
                {
                    CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", false);
                    Console.WriteLine("NET_DVR_Init Success！");
                }
                else
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string strErr = "NET_DVR_Init failed, error code= " + iLastErr;
                    Console.WriteLine(strErr);
                }

                string[] ipport = item.Split(':');
                if (ipport.Length != 2)
                {
                    Console.WriteLine("ip地址配置有误！应该是ip1:port1、ip2:port2");
                }
                string DVRIPAddress = ipport[0];
                ushort DVRPortNumber = ushort.Parse(ipport[1]);
                string DVRUserName = ConfigurationManager.AppSettings["UserName"];
                string DVRPassword = ConfigurationManager.AppSettings["Password"];
                int m_lUserID = HKCommonSDK.LoginV30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword);
                if (m_lUserID == -1)
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string strErr = "NET_DVR_Login_V30 failed,ip:port=" + DVRIPAddress + ":" + DVRPortNumber + "! error code= " + iLastErr;
                    Console.WriteLine(strErr);
                }
                else
                {
                    Console.WriteLine("NET_DVR_Login_V30 Success,ip:port=" + DVRIPAddress + ":" + DVRPortNumber + "! m_lUserID=" + m_lUserID);
                }

                bool setAlarm = HKAlarm.SetDVRMessageCallBack_V31();
                if (!setAlarm)
                {
                    Console.WriteLine("NET_DVR_SetDVRMessageCallBack_V31 fail!error code= " + CHCNetSDK.NET_DVR_GetLastError());
                    CHCNetSDK.NET_DVR_Logout(m_lUserID);
                    CHCNetSDK.NET_DVR_Cleanup();
                }
                else
                {
                    Console.WriteLine("NET_DVR_SetDVRMessageCallBack_V31 Success! ");
                }

                ////启用布防
                CHCNetSDK.NET_DVR_SETUPALARM_PARAM struSetupParam = new CHCNetSDK.NET_DVR_SETUPALARM_PARAM();
                struSetupParam.dwSize = (uint)Marshal.SizeOf(struSetupParam);
                // struSetupParam.byLevel = (byte)1;
                struSetupParam.byDeployType = 1;
                int lHandle = CHCNetSDK.NET_DVR_SetupAlarmChan_V41(m_lUserID, ref struSetupParam);
                if (lHandle < 0)
                {
                    Console.WriteLine("NET_DVR_SetupAlarmChan_V41 fail!error code= " + CHCNetSDK.NET_DVR_GetLastError());
                    CHCNetSDK.NET_DVR_Logout(m_lUserID);
                    CHCNetSDK.NET_DVR_Cleanup();
                }
                ////等待 60s，等待接收设备上传报警
                //Thread.Sleep(600000);
                ////撤销布防
                //if (!CHCNetSDK.NET_DVR_CloseAlarmChan_V30(lHandle))
                //{
                //    Console.WriteLine("NET_DVR_CloseAlarmChan_V30 fail!error code= " + CHCNetSDK.NET_DVR_GetLastError());
                //    CHCNetSDK.NET_DVR_Logout(m_lUserID);
                //    CHCNetSDK.NET_DVR_Cleanup();
                //}
                //CHCNetSDK.NET_DVR_Logout(m_lUserID);
                //CHCNetSDK.NET_DVR_Cleanup();
                Console.WriteLine("-----------------------------------");
            }
        }





        /// <summary>
        /// 用户注册
        /// </summary>
        public void LoginV40()
        {
            //string DVRIPAddress = ConfigurationManager.AppSettings["ip"];
            //Int16 DVRPortNumber = Int16.Parse(ConfigurationManager.AppSettings["port"]);
            //string DVRUserName = ConfigurationManager.AppSettings["UserName"];
            //string DVRPassword = ConfigurationManager.AppSettings["Password"];

            //CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
            //m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref m_struDeviceInfo);
            //if (m_lUserID == -1)
            //{
            //    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
            //    strErr = "NET_DVR_Login_V30 failed, error code= " + iLastErr;
            //    //登录失败，输出错误号 Failed to login and output the error code
            //    Console.WriteLine(strErr);
            //}
            //else
            //{
            //    Console.WriteLine("Login Success!");
            //}
        }


        /// <summary>
        /// 对象转为byte[]
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public static Byte[] StructToBytes(Object structure)
        {
            Int32 size = Marshal.SizeOf(structure);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, buffer, false);
                Byte[] bytes = new Byte[size];
                Marshal.Copy(buffer, bytes, 0, size);

                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

    }
}
