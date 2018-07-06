/** 
* 命名空间: HCSidewalkSluice  
* 类 名： HKAlarm 
* 
* Ver      作者     变更日期                   变更内容 
* ────────────────────────────────────────────────
* V1.0     huy        2018/6/30 13:57:20                 初版  
* 
* Copyright © 2017 GENVICT. All rights reserved.  
* ────────────────────────────────────────────────
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HCSidewalkSluice
{
    class HKAlarm
    {
        /// <summary>
        /// 设置布防门禁事件
        /// </summary>
        public static bool SetDVRMessageCallBack_V31()
        {
            //设置报警回调函数，刷卡等事件都会触发报警回调函数
            return CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(MSesGCallback, IntPtr.Zero);
        }

        /// <summary>
        /// 报警回调函数，接收设备报警消息等。
        /// </summary>
        /// <param name="lCommand"></param>
        /// <param name="pAlarmer"></param>
        /// <param name="pAlarmInfo"></param>
        /// <param name="dwBufLen"></param>
        /// <param name="pUser"></param>
        /// <returns></returns>
        private static bool MSesGCallback(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //回调函数中不可有耗时较长的操作，不能调用该 SDK（HCNetSDK.dll）本身的接口。
            //以下代码仅供参考，实际应用中不建议在回调函数中直接处理数据保存文件，
            //例如可以使用消息的方式(PostMessage)在消息响应函数里进行处理。
            switch (lCommand)
            {
                case CHCNetSDK.COMM_ALARM_ACS://门禁主机报警信息
                    {
                        CHCNetSDK.NET_DVR_ACS_ALARM_INFO struAcsAlarmInfo = new CHCNetSDK.NET_DVR_ACS_ALARM_INFO();
                        //Marshal.PtrToStructure(pAlarmInfo, struAcsAlarmInfo);
                        var result = Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_ACS_ALARM_INFO));
                        struAcsAlarmInfo = (CHCNetSDK.NET_DVR_ACS_ALARM_INFO)result;
                        switch (struAcsAlarmInfo.dwMajor)
                        {
                            case CHCNetSDK.MAJOR_ALARM:
                                Console.WriteLine("报警");
                                break;
                            case CHCNetSDK.MAJOR_EXCEPTION:
                                Console.WriteLine("异常");
                                break;
                            case CHCNetSDK.MAJOR_OPERATION:
                                Console.WriteLine("操作");
                                break;
                            case CHCNetSDK.MAJOR_EVENT:
                                Console.WriteLine("事件");
                                break;
                            default:
                                break;
                        }

                        //按需处理报警信息结构体中的信息......
                        break;
                    }
                case CHCNetSDK.COMM_ID_INFO_ALARM://门禁身份证刷卡信息
                    {
                        CHCNetSDK.NET_DVR_ID_CARD_INFO_ALARM struID_CardInfo = new CHCNetSDK.NET_DVR_ID_CARD_INFO_ALARM();
                        Marshal.PtrToStructure(pAlarmInfo, struID_CardInfo);
                        //按需处理报警信息结构体中的信息......
                        break;
                    }
                case CHCNetSDK.COMM_PASSNUM_INFO_ALARM://门禁通行人数信息
                    {
                        CHCNetSDK.NET_DVR_PASSNUM_INFO_ALARM struPassnumInfo = new CHCNetSDK.NET_DVR_PASSNUM_INFO_ALARM();
                        Marshal.PtrToStructure(pAlarmInfo, struPassnumInfo);
                        //按需处理报警信息结构体中的信息......
                        break;
                    }
                default:
                    break;
            }
            return true;
        }

        ///// <summary>
        ///// 报警布防。建立报警上传通道，获取报警等信息。
        ///// </summary>
        ///// <param name="iUserID">NET_DVR_Login或者NET_DVR_Login_V30的返回值</param>
        ///// <param name="alarmParam">报警布防参数</param>
        ///// <returns>-1表示失败，其他值作为NET_DVR_CloseAlarmChan_V30函数的句柄参数。</returns>
        //public static int SetupAlarmChanV41(int iUserID, ref CHCNetSDK.NET_DVR_SETUPALARM_PARAM alarmParam)
        //{
        //    return CHCNetSDK.NET_DVR_SetupAlarmChan_V41(iUserID, ref alarmParam);
        //}


    }
}
