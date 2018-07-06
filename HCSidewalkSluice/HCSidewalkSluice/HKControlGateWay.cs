/** 
* 命名空间: HCSidewalkSluice  
* 类 名： HKControlGateWay 
* 
* Ver      作者     变更日期                   变更内容 
* ────────────────────────────────────────────────
* V1.0     huy        2018/6/28 21:59:32                 初版  
* 
* Copyright © 2017 GENVICT. All rights reserved.  
* ────────────────────────────────────────────────
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HCSidewalkSluice
{
    public class HKControlGateWay
    {
        /// <summary>
        /// 远程开门控制
        /// </summary>
        public static void ControlGateWayOpen(int m_lUserID)
        {
            //开门，以门 1 为例
            bool bRet;
            int lGatewayIndex = 1;  //门禁序号，从 1 开始， -1 表示对所有门进行操作
            uint dwStaic = 1;      //命令值： 0-关闭， 1-打开， 2-常开， 3-常关
            bRet = CHCNetSDK.NET_DVR_ControlGateway(m_lUserID, lGatewayIndex, dwStaic);
            if (!bRet)
            {
                Console.WriteLine("NET_DVR_ControlGateway failed, error:%d\n", CHCNetSDK.NET_DVR_GetLastError());
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
                CHCNetSDK.NET_DVR_Cleanup();
                return;
            }
            //---------------------------------------
            //两秒钟之后就控制关
            //Thread.Sleep(2000);
            //dwStaic = 0;
            //bRet = CHCNetSDK.NET_DVR_ControlGateway(m_lUserID, lGatewayIndex, dwStaic);
            //if (!bRet)
            //{
            //    Console.WriteLine("NET_DVR_ControlGateway failed, error:%d\n", CHCNetSDK.NET_DVR_GetLastError());
            //    CHCNetSDK.NET_DVR_Logout(m_lUserID);
            //    CHCNetSDK.NET_DVR_Cleanup();
            //    return;
            //}

            Thread.Sleep(5000);
            CHCNetSDK.NET_DVR_Logout(m_lUserID);
            CHCNetSDK.NET_DVR_Cleanup();
            return;
        }
    }
}
