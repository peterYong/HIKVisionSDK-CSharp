/** 
* 命名空间: HCSidewalkSluice  
* 类 名： HKSendDownCardCfg 
* 
* Ver      作者     变更日期                   变更内容 
* ────────────────────────────────────────────────
* V1.0     huy        2018/6/28 18:05:23                 初版  
* 
* Copyright © 2017 GENVICT. All rights reserved.  
* ────────────────────────────────────────────────
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HCSidewalkSluice
{
    /// <summary>
    /// 下发卡号相关参数
    /// </summary>
    public class HKSendDownCardCfg
    {
        /// <summary>
        /// 启动长连接配置，设置卡参数
        /// </summary>
        /// <param name="m_lUserID">NET_DVR_Login_V40等登录接口的返回值 </param>
        /// <returns>NET_DVR_StartRemoteConfig的返回值</returns>
        public static int StartRemoteConfig(int m_lUserID)
        {
            //准备参数:(int lUserID, uint dwCommand, IntPtr lpInBuffer, uint dwInBufferLen,  RemoteConfigStateCALLBACK cbStateCallback, IntPtr pUserData);
            CHCNetSDK.NET_DVR_CARD_CFG_COND cardCfg = new CHCNetSDK.NET_DVR_CARD_CFG_COND();
            cardCfg.dwSize = (uint)Marshal.SizeOf(cardCfg);
            cardCfg.dwCardNum = 1;
            cardCfg.byCheckCardNo = 1;
            cardCfg.wLocalControllerID = 0;
            CHCNetSDK.fRemoteConfigCallback setGatewayCardCallback = new CHCNetSDK.fRemoteConfigCallback(SetGatewayCardCallback);//回调函数

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(cardCfg));
            int m_lSetCardCfgHandle = 0;
            try
            {
                Marshal.StructureToPtr(cardCfg, ptr, false);
                //IntPtr pUserData = Marshal.AllocHGlobal(Marshal.SizeOf(cardCfg));
                m_lSetCardCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig(m_lUserID, CHCNetSDK.NET_DVR_SET_CARD_CFG_V50, ptr, (uint)Marshal.SizeOf(cardCfg), setGatewayCardCallback, IntPtr.Zero);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //Marshal.FreeHGlobal(ptr);
            }
            return m_lSetCardCfgHandle;
        }

        /// <summary>
        /// 设置卡参数时的回调函数（输出函数、输出参数）【此函数定义不能修改】
        /// </summary>
        /// <param name="dwType">配置状态</param>
        /// <param name="lpBuffer">存放数据的缓冲区指针，具体内容跟dwType相关</param>
        /// <param name="dwBufLen">缓冲区大小【输出】 </param>
        /// <param name="pUserData">用户数据 </param>
        private static void SetGatewayCardCallback(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData)
        {
            string resultCallBack = "";
            if (dwType != (int)CHCNetSDK.NET_SDK_CALLBACK_TYPE.NET_SDK_CALLBACK_TYPE_STATUS)  //下发卡时只会返回状态
            {
                resultCallBack = "返回错误，dwType=" + dwType;
                Console.WriteLine(resultCallBack);
                return;
            }

            byte[] lpBufferBytes = new byte[dwBufLen];  //缓冲区的大小由 参数dwBufLen决定
            Marshal.Copy(lpBuffer, lpBufferBytes, 0, (int)dwBufLen);
            //1、取IntPtr中的前4个字节
            uint dwStatus = BitConverter.ToUInt32(lpBufferBytes, 0);  //[0]-[3]共4个字节。。eg： 0 0 3 233（分别是[3] [2] [1] [0]）

            if (dwStatus == (int)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_PROCESSING)//发送中
            {
                //2、取IntPtr中的[4,36]字节
                char[] szCardNumber = new char[CHCNetSDK.ACS_CARD_NO_LEN + 1];  // 每一位都是0 或者'\0'【二者等价】
                szCardNumber = Encoding.UTF8.GetChars(lpBufferBytes, 4, (int)dwBufLen - 4);

                string cardidStr = new string(szCardNumber);
                resultCallBack = "SetCard PROCESSING,CardNo:" + cardidStr;
            }
            else if (dwStatus == (int)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_FAILED)//发送失败
            {
                //错误码
                uint dwErrCode = Convert.ToUInt32(Encoding.UTF8.GetChars(lpBufferBytes, 4, 4));
                //char[] dwErrCode = new char[4+1];
                //dwErrCode = Encoding.UTF8.GetChars(lpBufferBtyes, 8, 4);
                //uint dwErrCode = BitConverter.ToUInt32(lpBufferBtyes, 4);   //后 4 个字节为错误码

                //卡号
                char[] szCardNumber = new char[CHCNetSDK.ACS_CARD_NO_LEN + 1];  // 每一位都是0 或者'\0'【二者等价】
                szCardNumber = Encoding.UTF8.GetChars(lpBufferBytes, 8, (int)dwBufLen - 8);

                resultCallBack = string.Format("SetCard Err:{0},CardNo:{1}", dwErrCode, new string(szCardNumber));
            }
            //下面两个关闭长连接
            else if (dwStatus == (int)CHCNetSDK.NET_SDK_CALLBACK_STATUS_NORMAL.NET_SDK_CALLBACK_STATUS_SUCCESS)//发送成功
            {
                resultCallBack = "SetCard SUCCESS!";
            }
            Console.WriteLine(resultCallBack);
        }

        /// <summary>
        /// 发送长连接数据
        /// </summary>
        /// <param name="m_lUserID">NET_DVR_Login_V40等登录接口的返回值</param>
        /// <param name="m_lSetCardCfgHandle">长连接句柄，NET_DVR_StartRemoteConfig的返回值</param>
        ///  <param name="cardValid">卡是否有效： 0-无效(删除)， 1-有效</param>
        /// <param name="cardID">下发的卡号</param>
        /// <param name="dtStart">有效开始时间</param>
        /// <param name="dtEnd">有效接收时间</param>
        /// <returns>TRUE表示成功，FALSE表示失败</returns>
        public static bool SendRemoteConfig(int m_lUserID, int m_lSetCardCfgHandle, byte cardValid, string cardID, DateTime dtStart, DateTime dtEnd)
        {
            //设置卡参数
            CHCNetSDK.NET_DVR_CARD_CFG_V50 cardCfg = new CHCNetSDK.NET_DVR_CARD_CFG_V50();
            cardCfg.dwSize = (uint)Marshal.SizeOf(cardCfg);
            cardCfg.dwModifyParamType = 0x000003FF;//需要修改的卡参数，按位表示，每位代表一种参数，值： 0-不修改， 1 - 修改。。此值表示修改除了（工号、姓名、部门编号、排班计划编号、排班计划类型、用户类型）以为的参数。

            cardCfg.byCardNo = Encoding.Default.GetBytes(cardID.Trim().PadRight(32, '\0').ToCharArray());
            cardCfg.byCardValid = cardValid;//卡是否有效： 0-无效， 1-有效

            cardCfg.byCardType = 1;//卡类型： 1-普通卡（默认）
            cardCfg.byLeaderCard = 0;//是否为首卡： 1-是， 0-否
            cardCfg.byDoorRight = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                cardCfg.byDoorRight[i] = 1;
            }
            //cardCfg.byDoorRight[0] = 1; //byDoorRight[0]~byDoorRight[255]对应门 1-256，值： 1-有权限， 0-无权限
            //cardCfg.byDoorRight[1] = 1;//门 1、门 2 有权限
            //cardCfg.byDoorRight[2] = 1; //byDoorRight[0]~byDoorRight[255]对应门 1-256，值： 1-有权限， 0-无权限
            //cardCfg.byDoorRight[3] = 1;//门 1、门 2 有权限

            cardCfg.byBelongGroup = new byte[128];
            for (int i = 0; i < 128; i++)
            {
                cardCfg.byBelongGroup[i] = 1;
            }
            //cardCfg.byBelongGroup[0] = 1;//byBelongGroup[0]~byBelongGroup[127]对应群组 1-128，值： 1-属于， 0-不属于
            string m_csCardPassword = "12345678";//卡密码
            cardCfg.byCardPassword = Encoding.Default.GetBytes(m_csCardPassword.Trim().PadRight(8, '\0').ToCharArray());
            //卡权限计划模板需先配置好

            //cardCfg.wCardRightPlan = new ushort[256 * 4];
            //cardCfg.wCardRightPlan[0] = 1;//该卡在门 1 具有卡权限计划模板 1 和 2 的权限
            //cardCfg.wCardRightPlan[1] = 2;
            //cardCfg.wCardRightPlan[2] = 3;//该卡在门 2 具有卡权限计划模板 3 和 4 的权限
            //cardCfg.wCardRightPlan[3] = 4;
            cardCfg.wCardRightPlan = new ushort[256, 4];
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    cardCfg.wCardRightPlan[i, j] = 1;
                }
            }

            cardCfg.dwMaxSwipeTime = 0;//最大刷卡次数， 0 无限次数
            cardCfg.dwSwipeTime = 0;//已刷卡次数
            cardCfg.struValid.byEnable = 1;//使能有效期

            cardCfg.struValid.struBeginTime.wYear = (ushort)dtStart.Year;  //开始时间： 2018-06-28 00:00:00
            cardCfg.struValid.struBeginTime.byMonth = (byte)dtStart.Month;
            cardCfg.struValid.struBeginTime.byDay = (byte)dtStart.Day;
            cardCfg.struValid.struBeginTime.byHour = (byte)dtStart.Hour;
            cardCfg.struValid.struBeginTime.byMinute = (byte)dtStart.Minute;
            cardCfg.struValid.struBeginTime.bySecond = (byte)dtStart.Second;
            cardCfg.struValid.struEndTime.wYear = (ushort)dtEnd.Year;    //结束时间： 2018-07-28 00:00:00
            cardCfg.struValid.struEndTime.byMonth = (byte)dtEnd.Month;
            cardCfg.struValid.struEndTime.byDay = (byte)dtEnd.Day;
            cardCfg.struValid.struEndTime.byHour = (byte)dtEnd.Hour;
            cardCfg.struValid.struEndTime.byMinute = (byte)dtEnd.Minute;
            cardCfg.struValid.struEndTime.bySecond = (byte)dtEnd.Second;
            //发送长连接数据                     
            int CardCfgLen = Marshal.SizeOf(cardCfg);
            IntPtr ptr = Marshal.AllocHGlobal(CardCfgLen);
            bool isSend = false;
            try
            {
                Marshal.StructureToPtr(cardCfg, ptr, false);
                isSend = CHCNetSDK.NET_DVR_SendRemoteConfig(m_lSetCardCfgHandle, CHCNetSDK.ENUM_ACS_SEND_DATA, ptr, (uint)CardCfgLen);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return isSend;
        }


    }
}
