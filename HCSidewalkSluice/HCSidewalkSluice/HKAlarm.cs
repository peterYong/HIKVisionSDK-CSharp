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
        public static CHCNetSDK.MSGCallBack_V31 msgCall_delegate = null;
        /// <summary>
        /// 设置布防门禁事件
        /// </summary>
        public static bool SetDVRMessageCallBack_V31()
        {
            //设置报警回调函数，刷卡等事件都会触发报警回调函数
            // bool setOK= CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(MSesGCallback, IntPtr.Zero);
            //GC.KeepAlive(msgCall_delegate);//关键的一句， 如果没有KeepAlive，NET_DVR_SetDVRMessageCallBack_V31调用完之后，MSesGCallback就被GC.Collect掉了，以后如果还有调用的话就会抛异常。
            //return setOK;

            //或者
            msgCall_delegate = new CHCNetSDK.MSGCallBack_V31(MSesGCallback);
            bool setOK = CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(msgCall_delegate, IntPtr.Zero);
            return setOK;
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

                        Console.WriteLine("门禁主机报警信息：" + StrAlarmInfo(struAcsAlarmInfo));
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

                        var result = Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_PASSNUM_INFO_ALARM));
                        struPassnumInfo = (CHCNetSDK.NET_DVR_PASSNUM_INFO_ALARM)result;
                        //按需处理报警信息结构体中的信息......
                        Console.WriteLine("门禁通行人数信息:"+struPassnumInfo.ToString ());
                        break;
                    }
                default:
                    break;
            }
            return true;
        }

        #region 门禁主机报警信息
        /// <summary>
        /// 门禁主机报警信息
        /// </summary>
        /// <param name="sAlarm"></param>
        /// <returns></returns>
        public static string StrAlarmInfo(CHCNetSDK.NET_DVR_ACS_ALARM_INFO sAlarm)
        {
            DateTime dtime = new DateTime((int)sAlarm.struTime.dwYear, (int)sAlarm.struTime.dwMonth, (int)sAlarm.struTime.dwDay, (int)sAlarm.struTime.dwHour, (int)sAlarm.struTime.dwMinute, (int)sAlarm.struTime.dwSecond);
            string time = dtime.ToString("yyyy-MM-dd HH:mm:ss");
            string netUser = Encoding.UTF8.GetString(sAlarm.sNetUser).TrimEnd('\0');

            byte[] ipv6 = sAlarm.struRemoteHostAddr.sIpV6;
            string ipAddress = "ipV4=" + sAlarm.struRemoteHostAddr.sIpV4.TrimEnd('\0') + "，ipV6=" +
                Encoding.UTF8.GetString(ipv6).TrimEnd('\0');

            //string details = Encoding.UTF8.GetString(StructToBytes(sAlarm.struAcsEventInfo));
            string details = sAlarm.struAcsEventInfo.ToString();

            string result = (GetMajorType(sAlarm.dwMajor, sAlarm.dwMinor) + "，报警时间=" + time + "，网络操作的用户名=" + netUser + "，远程主机地址，" + ipAddress + "，报警信息详细参数，" + details);
            return result;
        }

        /// <summary>
        /// 获取主类型、次类型
        /// </summary>
        /// <param name="dwMajor"></param>
        /// <returns></returns>
        public static string GetMajorType(uint dwMajor, uint dwMinor)
        {
            string major = "";
            switch (dwMajor)
            {
                case CHCNetSDK.MAJOR_ALARM:
                    major = "主类型=报警，" + "次类型：" + dwMinor;
                    break;
                case CHCNetSDK.MAJOR_EXCEPTION:
                    major = "主类型=异常，" + "次类型：" + dwMinor;
                    break;
                case CHCNetSDK.MAJOR_OPERATION:
                    major = "主类型=操作，" + "次类型：" + GetMinorTypeAlarm(dwMinor);
                    break;
                case CHCNetSDK.MAJOR_EVENT:
                    major = "主类型=事件，" + "次类型：" + GetMinorTypeEvent(dwMinor);
                    break;
                default:
                    major = "主类型：" + dwMajor.ToString() + "次类型：" + dwMinor;
                    break;
            }
            return major;
        }

        /// <summary>
        /// 获取【操作】次类型
        /// </summary>
        /// <param name="dwMajor"></param>
        /// <returns></returns>
        public static string GetMinorTypeAlarm(uint dwMinor)
        {
            string minor = "";
            switch (dwMinor)
            {
                case CHCNetSDK.MINOR_LOCAL_LOGIN:
                    minor = "本地登录";
                    break;
                case CHCNetSDK.MINOR_LOCAL_UPGRADE:
                    minor = "本地升级";
                    break;
                case CHCNetSDK.MINOR_REMOTE_LOGIN:
                    minor = "远程登录";
                    break;
                case CHCNetSDK.MINOR_REMOTE_LOGOUT:
                    minor = "远程注销登陆";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ARM:
                    minor = "远程布防";
                    break;
                case CHCNetSDK.MINOR_REMOTE_DISARM:
                    minor = "远程撤防";
                    break;
                case CHCNetSDK.MINOR_REMOTE_REBOOT:
                    minor = "远程重启";
                    break;
                case CHCNetSDK.MINOR_REMOTE_UPGRADE:
                    minor = "远程升级";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CFGFILE_OUTPUT:
                    minor = "远程导出配置文件";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CFGFILE_INTPUT:
                    minor = "远程导入配置文件";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ALARMOUT_OPEN_MAN:
                    minor = "远程手动开启报警输出";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ALARMOUT_CLOSE_MAN:
                    minor = "远程手动关闭报警输出";
                    break;
                case CHCNetSDK.MINOR_REMOTE_OPEN_DOOR:
                    minor = "远程开门";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CLOSE_DOOR:
                    minor = "远程关门（对于梯控，表示受控）";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ALWAYS_OPEN:
                    minor = "远程常开（对于梯控，表示自由）";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ALWAYS_CLOSE:
                    minor = "远程常关（对于梯控，表示禁用）";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CHECK_TIME:
                    minor = "远程手动校时";
                    break;
                case CHCNetSDK.MINOR_NTP_CHECK_TIME:
                    minor = " NTP自动校时";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CLEAR_CARD:
                    minor = "远程清空卡号";
                    break;
                case CHCNetSDK.MINOR_REMOTE_RESTORE_CFG:
                    minor = "远程恢复默认参数";
                    break;
                case CHCNetSDK.MINOR_ALARMIN_ARM:
                    minor = "防区布防";
                    break;
                case CHCNetSDK.MINOR_ALARMIN_DISARM:
                    minor = "防区撤防";
                    break;
                case CHCNetSDK.MINOR_LOCAL_RESTORE_CFG:
                    minor = "本地恢复默认参数";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CAPTURE_PIC:
                    minor = "远程抓拍";
                    break;
                case CHCNetSDK.MINOR_MOD_NET_REPORT_CFG:
                    minor = "修改网络中心参数配置";
                    break;
                case CHCNetSDK.MINOR_MOD_GPRS_REPORT_PARAM:
                    minor = "修改GPRS中心参数配置";
                    break;
                case CHCNetSDK.MINOR_MOD_REPORT_GROUP_PARAM:
                    minor = "修改中心组参数配置";
                    break;
                case CHCNetSDK.MINOR_UNLOCK_PASSWORD_OPEN_DOOR:
                    minor = "解除码输入";
                    break;
                case CHCNetSDK.MINOR_AUTO_RENUMBER:
                    minor = "自动重新编号";
                    break;
                case CHCNetSDK.MINOR_AUTO_COMPLEMENT_NUMBER:
                    minor = "自动补充编号";
                    break;
                case CHCNetSDK.MINOR_NORMAL_CFGFILE_INPUT:
                    minor = "导入普通配置文件";
                    break;
                case CHCNetSDK.MINOR_NORMAL_CFGFILE_OUTTPUT:
                    minor = "导出普通配置文件";
                    break;
                case CHCNetSDK.MINOR_CARD_RIGHT_INPUT:
                    minor = "导入卡权限参数";
                    break;
                case CHCNetSDK.MINOR_CARD_RIGHT_OUTTPUT:
                    minor = "导出卡权限参数";
                    break;
                case CHCNetSDK.MINOR_LOCAL_USB_UPGRADE:
                    minor = "本地U盘升级";
                    break;
                case CHCNetSDK.MINOR_REMOTE_VISITOR_CALL_LADDER:
                    minor = "访客呼梯";
                    break;
                case CHCNetSDK.MINOR_REMOTE_HOUSEHOLD_CALL_LADDER:
                    minor = "住户呼梯";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ACTUAL_GUARD:
                    minor = "远程实时布防";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ACTUAL_UNGUARD:
                    minor = "远程实时撤防";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CONTROL_NOT_CODE_OPER_FAILED:
                    minor = "遥控器未对码操作失败";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CONTROL_CLOSE_DOOR:
                    minor = "遥控器关门";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CONTROL_OPEN_DOOR:
                    minor = "遥控器开门";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CONTROL_ALWAYS_OPEN_DOOR:
                    minor = "遥控器常开门";
                    break;
                default:
                    minor = dwMinor.ToString();
                    break;
            }
            return minor;
        }

        /// <summary>
        /// 获取【事件】次类型
        /// </summary>
        /// <param name="dwMajor"></param>
        /// <returns></returns>
        public static string GetMinorTypeEvent(uint dwMinor)
        {
            string minor = "";
            switch (dwMinor)
            {
                case CHCNetSDK.MINOR__LEGAL_CARD_PASS:
                    minor = "合法卡认证通过";
                    //记录通行日志
                    break;
                case CHCNetSDK.MINOR__CARD_AND_PSW_PASS:
                    minor = "刷卡加密码认证通过";
                    break;
                case CHCNetSDK.MINOR__CARD_AND_PSW_FAIL:
                    minor = "刷卡加密码认证失败";
                    break;
                case CHCNetSDK.MINOR__CARD_AND_PSW_TIMEOUT:
                    minor = "数卡加密码认证超时";
                    break;
                case CHCNetSDK.MINOR__CARD_AND_PSW_OVER_TIME:
                    minor = "刷卡加密码超次";
                    break;
                case CHCNetSDK.MINOR__CARD_NO_RIGHT:
                    minor = "未分配权限";
                    break;
                case CHCNetSDK.MINOR__CARD_INVALID_PERIOD:
                    minor = "无效时段";
                    break;
                case CHCNetSDK.MINOR__CARD_OUT_OF_DATE:
                    minor = "卡号过期";
                    break;
                case CHCNetSDK.MINOR__INVALID_CARD:
                    minor = "无此卡号";
                    break;
                case CHCNetSDK.MINOR__ANTI_SNEAK_FAIL:
                    minor = "反潜回认证失败";
                    break;
                case CHCNetSDK.MINOR__INTERLOCK_DOOR_NOT_CLOSE:
                    minor = "互锁门未关闭";
                    break;
                case CHCNetSDK.MINOR__LEADER_CARD_OPEN_BEGIN:
                    minor = "首卡开门开始";
                    break;
                case CHCNetSDK.MINOR__LEADER_CARD_OPEN_END:
                    minor = "首卡开门结束";
                    break;
                case CHCNetSDK.MINOR__ALWAYS_OPEN_BEGIN:
                    minor = "常开状态开始";
                    break;
                case CHCNetSDK.MINOR__ALWAYS_OPEN_END:
                    minor = "常开状态结束";
                    break;
                case CHCNetSDK.MINOR__LOCK_OPEN:
                    minor = "门锁打开";
                    break;
                case CHCNetSDK.MINOR__LOCK_CLOSE:
                    minor = "门锁关闭";
                    break;
                case CHCNetSDK.MINOR__DOOR_BUTTON_PRESS:
                    minor = " 开门按钮打开";
                    break;
                case CHCNetSDK.MINOR__DOOR_BUTTON_RELEASE:
                    minor = "开门按钮放开";
                    break;
                case CHCNetSDK.MINOR__DOOR_OPEN_NORMAL:
                    minor = "正常关门（门磁）";
                    break;
                case CHCNetSDK.MINOR__DOOR_OPEN_ABNORMAL:
                    minor = "门异常打开（门磁）";
                    break;
                case CHCNetSDK.MINOR__DOOR_OPEN_TIMEOUT:
                    minor = "门打开超时（门磁）";
                    break;
                case CHCNetSDK.MINOR__ALARMOUT_ON:
                    minor = "报警输出打开";
                    break;
                case CHCNetSDK.MINOR__ALARMOUT_OFF:
                    minor = "报警输出关闭";
                    break;
                case CHCNetSDK.MINOR__ALWAYS_CLOSE_BEGIN:
                    minor = "常关状态开始";
                    break;
                case CHCNetSDK.MINOR__ALWAYS_CLOSE_END:
                    minor = "常关状态结束";
                    break;
                case CHCNetSDK.MINOR__DOORBELL_RINGING:
                    minor = "门铃响";
                    break;
                case CHCNetSDK.MINOR__CARD_PLATFORM_VERIFY:
                    minor = "刷卡平台认证";
                    break;
                case CHCNetSDK.MINOR__CALL_CENTER:
                    minor = "呼叫中心事件";
                    break;
                case CHCNetSDK.MINOR__FIRE_RELAY_TURN_ON_DOOR_ALWAYS_OPEN:
                    minor = "消防继电器导通触发门常开";
                    break;
                case CHCNetSDK.MINOR__FIRE_RELAY_RECOVER_DOOR_RECOVER_NORMAL:
                    minor = "消防继电器恢复门恢复正常";
                    break;
                case CHCNetSDK.MINOR__FIRSTCARD_AUTHORIZE_BEGIN:
                    minor = "首卡授权开始";
                    break;
                case CHCNetSDK.MINOR__FIRSTCARD_AUTHORIZE_END:
                    minor = "首卡授权结束";
                    break;
                case CHCNetSDK.MINOR__CERTIFICATE_BLACK_LIST:
                    minor = "黑名单事件";
                    break;
                case CHCNetSDK.MINOR__DOOR_OPEN_OR_DORMANT_FAIL:
                    minor = "门状态常闭或休眠状态认证失败";
                    break;
                case CHCNetSDK.MINOR__AUTH_PLAN_DORMANT_FAIL:
                    minor = "认证计划休眠模式认证失败";
                    break;
                case CHCNetSDK.MINOR__CARD_ENCRYPT_VERIFY_FAIL:
                    minor = "卡加密校验失败";
                    break;
                case CHCNetSDK.MINOR__SUBMARINEBACK_REPLY_FAIL:
                    minor = "反潜回服务器应答失败";
                    break;
                case CHCNetSDK.MINOR__TRAILING:
                    minor = "尾随通行";
                    break;
                case CHCNetSDK.MINOR__REVERSE_ACCESS:
                    minor = "反向闯入";
                    break;
                case CHCNetSDK.MINOR__FORCE_ACCESS:
                    minor = "外力冲撞";
                    break;
                case CHCNetSDK.MINOR__PASSING_TIMEOUT:
                    minor = "翻越";
                    break;
                case CHCNetSDK.MINOR__INTRUSION_ALARM:
                    minor = "误闯报警";
                    break;
                case CHCNetSDK.MINOR__FREE_GATE_PASS_NOT_AUTH:
                    minor = "闸机自由通行时未认证通过";
                    break;
                case CHCNetSDK.MINOR__DROP_ARM_BLOCK:
                    minor = "摆臂被阻挡";
                    break;
                case CHCNetSDK.MINOR__DROP_ARM_BLOCK_RESUME:
                    minor = "摆臂阻挡消除";
                    break;
                default:
                    minor = dwMinor.ToString();
                    break;
            }
            return minor;
        }


        #endregion





        #region 门禁通行人数信息


        #endregion



        /// <summary>
        /// 将结构体转为byte[]
        /// </summary>
        /// <param name="structObj"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] StructToBytes(object structObj, int size = 0)
        {
            if (size == 0)
            {
                size = Marshal.SizeOf(structObj); //得到结构体大小
            }
            IntPtr buffer = Marshal.AllocHGlobal(size);  //开辟内存空间
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);   //填充内存空间
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);   //填充数组
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("struct to bytes error:" + ex);
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);   //释放内存
            }
        }


    }
}
