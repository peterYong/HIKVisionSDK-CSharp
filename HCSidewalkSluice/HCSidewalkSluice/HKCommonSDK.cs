/** 
* 命名空间: HCSidewalkSluice  
* 类 名： HKCommonSDK 
* 
* Ver      作者     变更日期                   变更内容 
* ────────────────────────────────────────────────
* V1.0     huy        2018/6/28 21:07:39                 初版  
* 
* Copyright © 2017 GENVICT. All rights reserved.  
* ────────────────────────────────────────────────
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCSidewalkSluice
{
    /// <summary>
    /// 海康通用SDK（初始化、用户注册...）
    /// </summary>
    public class HKCommonSDK
    {
        /// <summary>
        /// 初始化SDK
        /// </summary>
        public static bool Init()
        {
            bool m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            return m_bInitSDK;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="DVRIPAddress">设备ip</param>
        /// <param name="DVRPortNumber">设备ip</param>
        /// <param name="DVRUserName">用户名</param>
        /// <param name="DVRPassword">用户密码</param>
        /// <returns>-1表示失败，其他值表示返回的用户ID值。该用户ID具有唯一性，后续对设备的操作都需要通过此ID实现。</returns>
        public static int LoginV30(string DVRIPAddress, ushort DVRPortNumber, string DVRUserName, string DVRPassword)
        {
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
            int m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref m_struDeviceInfo);
            return m_lUserID;
        }


    }
}
