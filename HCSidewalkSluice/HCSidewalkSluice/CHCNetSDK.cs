/** 
* 命名空间: HCSidewalkSluice  
* 类 名： CHCNetSDK 
* 
* Ver      作者     变更日期                   变更内容 
* ────────────────────────────────────────────────
* V1.0     huy        2018/6/22 17:54:46                 初版  
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
    class CHCNetSDK
    {
        #region SDK常量，这些常量在多个接口中共用

        /// <summary>
        /// //序列号长度
        /// </summary>
        public const int SERIALNO_LEN = 48;
        /// <summary>
        /// 卡号长度
        /// </summary>
        public const int ACS_CARD_NO_LEN = 32;

        public const int MAX_DOOR_NUM = 256;
        public const int MAX_GROUP_NUM = 128;
        /// <summary>
        /// 卡密码长度
        /// </summary>
        public const int CARD_PASSWORD_LEN = 8;
        public const int MAX_CARD_RIGHT_PLAN_NUM = 4;

        public const int MAX_LOCK_CODE_LEN = 8;
        public const int MAX_DOOR_CODE_LEN = 8;
        /// <summary>
        /// 设备名字长度
        /// </summary>
        public const int NAME_LEN = 32;

        public const int MAX_NAMELEN = 16;

        //配置命令dwCommand 
        /// <summary>
        /// 获取卡参数
        /// </summary>
        public const int NET_DVR_GET_CARD_CFG_V50 = 2178;
        /// <summary>
        /// 设置卡参数
        /// </summary>
        public const int NET_DVR_SET_CARD_CFG_V50 = 2179;

        /// <summary>
        /// 门禁主机数据类型
        /// </summary>
        public const int ENUM_ACS_SEND_DATA = 0x3;
        #endregion

        #region 海康常用SDK

        /// <summary>
        /// 初始化SDK，调用其他SDK函数的前提。
        /// </summary>
        /// <returns>TRUE表示成功，FALSE表示失败。</returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern bool NET_DVR_Init();

        /// <summary>
        /// 用户注销。
        /// </summary>
        /// <param name="iUserID">登录接口的返回值</param>
        /// <returns></returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern bool NET_DVR_Logout(int iUserID);


        /// <summary>
        /// 释放SDK资源，在结束之前最后调用
        /// </summary>
        /// <returns></returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern bool NET_DVR_Cleanup();

        /// <summary>
        /// 返回最后操作的错误码。
        /// </summary>
        /// <returns>0:没有错误。;41:SDK资源分配错误。;53:获得本地PC的IP地址或物理地址失败。</returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern uint NET_DVR_GetLastError();

        /// <summary>
        /// 启用日志文件写入接口
        /// </summary>
        /// <param name="nLogLevel">日志的等级（默认为0）：0-表示关闭日志，1-表示只输出ERROR错误日志，2-输出ERROR错误信息和DEBUG调试信息，3-输出ERROR错误信息、DEBUG调试信息和INFO普通信息等所有信息 </param>
        /// <param name="strLogDir">日志文件的路径，windows默认值为"C:\\SdkLog\\"；linux默认值"/home/sdklog/"</param>
        /// <param name="bAutoDel">是否删除超出的文件数，默认值为TRUE ..为TRUE时表示覆盖模式，日志文件个数超过SDK限制个数时将会自动删除超出的文件。SDK限制个数默认为10个</param>
        /// <returns></returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern bool NET_DVR_SetLogToFile(int nLogLevel, string strLogDir, bool bAutoDel);

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="sDVRIP"> 设备IP地址 </param>
        /// <param name="wDVRPort">设备端口号</param>
        /// <param name="sUserName">登录的用户名</param>
        /// <param name="sPassword">用户密码</param>
        /// <param name="lpDeviceInfo">设备信息</param>
        /// <returns>-1表示失败，其他值表示返回的用户ID值</returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern int NET_DVR_Login_V30(string sDVRIP, ushort wDVRPort, string sUserName, string sPassword, ref NET_DVR_DEVICEINFO_V30 lpDeviceInfo);


        /// <summary>
        /// NET_DVR_Login_V30()参数结构
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;  //序列号
            public byte byAlarmInPortNum;		        //报警输入个数
            public byte byAlarmOutPortNum;		        //报警输出个数
            public byte byDiskNum;				    //硬盘个数
            public byte byDVRType;				    //设备类型, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;				    //模拟通道个数
            public byte byStartChan;			        //起始通道号,例如DVS-1,DVR - 1
            public byte byAudioChanNum;                //语音通道数
            public byte byIPChanNum;					//最大数字通道个数，低位  
            public byte byZeroChanNum;			//零通道编码个数 //2010-01-16
            public byte byMainProto;			//主码流传输协议类型 0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySubProto;				//子码流传输协议类型0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySupport;        //能力，位与结果为0表示不支持，1表示支持，
                                          //bySupport & 0x1, 表示是否支持智能搜索
                                          //bySupport & 0x2, 表示是否支持备份
                                          //bySupport & 0x4, 表示是否支持压缩参数能力获取
                                          //bySupport & 0x8, 表示是否支持多网卡
                                          //bySupport & 0x10, 表示支持远程SADP
                                          //bySupport & 0x20, 表示支持Raid卡功能
                                          //bySupport & 0x40, 表示支持IPSAN 目录查找
                                          //bySupport & 0x80, 表示支持rtp over rtsp
            public byte bySupport1;        // 能力集扩充，位与结果为0表示不支持，1表示支持
                                           //bySupport1 & 0x1, 表示是否支持snmp v30
                                           //bySupport1 & 0x2, 支持区分回放和下载
                                           //bySupport1 & 0x4, 是否支持布防优先级	
                                           //bySupport1 & 0x8, 智能设备是否支持布防时间段扩展
                                           //bySupport1 & 0x10, 表示是否支持多磁盘数（超过33个）
                                           //bySupport1 & 0x20, 表示是否支持rtsp over http	
                                           //bySupport1 & 0x80, 表示是否支持车牌新报警信息2012-9-28, 且还表示是否支持NET_DVR_IPPARACFG_V40结构体
            public byte bySupport2; /*能力，位与结果为0表示不支持，非0表示支持							
							bySupport2 & 0x1, 表示解码器是否支持通过URL取流解码
							bySupport2 & 0x2,  表示支持FTPV40
							bySupport2 & 0x4,  表示支持ANR
							bySupport2 & 0x8,  表示支持CCD的通道参数配置
							bySupport2 & 0x10,  表示支持布防报警回传信息（仅支持抓拍机报警 新老报警结构）
							bySupport2 & 0x20,  表示是否支持单独获取设备状态子项
							bySupport2 & 0x40,  表示是否是码流加密设备*/
            public ushort wDevType;              //设备型号
            public byte bySupport3; //能力集扩展，位与结果为0表示不支持，1表示支持
                                    //bySupport3 & 0x1, 表示是否多码流
                                    // bySupport3 & 0x4 表示支持按组配置， 具体包含 通道图像参数、报警输入参数、IP报警输入、输出接入参数、
                                    // 用户参数、设备工作状态、JPEG抓图、定时和时间抓图、硬盘盘组管理 
                                    //bySupport3 & 0x8为1 表示支持使用TCP预览、UDP预览、多播预览中的"延时预览"字段来请求延时预览（后续都将使用这种方式请求延时预览）。而当bySupport3 & 0x8为0时，将使用 "私有延时预览"协议。
                                    //bySupport3 & 0x10 表示支持"获取报警主机主要状态（V40）"。
                                    //bySupport3 & 0x20 表示是否支持通过DDNS域名解析取流

            public byte byMultiStreamProto;//是否支持多码流,按位表示,0-不支持,1-支持,bit1-码流3,bit2-码流4,bit7-主码流，bit-8子码流
            public byte byStartDChan;		//起始数字通道号,0表示无效
            public byte byStartDTalkChan;	//起始数字对讲通道号，区别于模拟对讲通道号，0表示无效
            public byte byHighDChanNum;		//数字通道个数，高位
            public byte bySupport4;
            public byte byLanguageType;// 支持语种能力,按位表示,每一位0-不支持,1-支持  
                                       //  byLanguageType 等于0 表示 老设备
                                       //  byLanguageType & 0x1表示支持中文
                                       //  byLanguageType & 0x2表示支持英文
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;		//保留
        }

        #endregion

        #region 获取和下发卡号相关联参数流程

        /// <summary>
        /// 远程配置回调函数
        /// </summary>
        /// <param name="dwType">配置状态</param>
        /// <param name="lpBuffer">存放数据的缓冲区指针，具体内容跟dwType相关..
        /// <param name="dwBufLen">缓冲区大小</param>
        /// <param name="pUserData">用户数据</param>
        public delegate void fRemoteConfigCallback(uint dvType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData);


        /// <summary>
        /// 启动远程配置
        /// </summary>
        /// <param name="lUserID">登录接口的返回值 </param>
        /// <param name="dwCommand">配置命令，不同的功能对应不同的命令号(dwCommand)，lpInBuffer等参数也对应不同的内容， </param>
        /// <param name="lpInBuffer">输入参数，具体内容跟配置命令相关[NET_DVR_CARD_CFG_COND]</param>
        /// <param name="dwInBufferLen">输入缓冲的大小</param>
        /// <param name="cbStateCallback">状态回调函数</param>
        /// <param name="pUserData">用户数据</param>
        /// <returns></returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern Int32 NET_DVR_StartRemoteConfig(int lUserID, uint dwCommand, IntPtr lpInBuffer, uint dwInBufferLen, fRemoteConfigCallback cbStateCallback, IntPtr pUserData);

        /// <summary>
        /// 卡参数配置条件结构体。
        /// 设置卡参数（下发卡参数）时，如果将byCheckCardNo置为0，那么设备将不校验应用层下发的卡号信息，直接写入本地存储，可以一定程度提高卡号下发的速度，但是需要上层应用自己保证卡号信息不重复（整型值不能重复，比如，不能同时含有1和01这两种卡号）。
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CARD_CFG_COND
        {
            /// <summary>
            /// 结构体大小
            /// </summary>
            public uint dwSize;
            /// <summary>
            /// 设置或获取卡数量，获取时置为0xffffffff表示获取所有卡信息 
            /// </summary>
            public uint dwCardNum;
            /// <summary>
            /// 设备是否进行卡号校验：0- 不校验，1- 校验
            /// </summary>
            public byte byCheckCardNo;
            /// <summary>
            /// 保留，置为0
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            /// <summary>
            /// 就地控制器序号，表示往就地控制器下发离线卡参数，0代表是门禁主机
            /// </summary>
            public ushort wLocalControllerID;
            /// <summary>
            /// 保留，置为0
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            /// <summary>
            /// 锁ID 
            /// </summary>
            public uint dwLockID;
            /// <summary>
            /// 保留，置为0
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
        }


        /// <summary>
        /// 配置状态
        /// </summary>
        public enum NET_SDK_CALLBACK_TYPE : int
        {
            /// <summary>
            /// 回调状态值
            /// </summary>
            NET_SDK_CALLBACK_TYPE_STATUS = 0,
            /// <summary>
            /// 回调进度值(暂不支持)
            /// </summary>
            NET_SDK_CALLBACK_TYPE_PROGRESS,
            /// <summary>
            /// 回调数据内容 
            /// </summary>
            NET_SDK_CALLBACK_TYPE_DATA
        }

        /// <summary>
        /// 回调状态值=NET_SDK_CALLBACK_TYPE_STATUS时的，lpBuffer
        /// </summary>
        public enum NET_SDK_CALLBACK_STATUS_NORMAL : int
        {
            /// <summary>
            /// 成功,表示获取和配置成功并且结束；
            /// </summary>
            NET_SDK_CALLBACK_STATUS_SUCCESS = 1000,
            /// <summary>
            /// 处理中。lpBuffer：4字节状态 + 32字节卡号；
            /// </summary>
            NET_SDK_CALLBACK_STATUS_PROCESSING,
            /// <summary>
            /// 失败。lpBuffer：4字节状态 + 4字节错误码 + 32字节卡号；
            /// </summary>
            NET_SDK_CALLBACK_STATUS_FAILED,
            NET_SDK_CALLBACK_STATUS_EXCEPTION
        }



        /// <summary>
        /// 发送长连接数据。
        /// </summary>
        /// <param name="lHandle">长连接句柄，NET_DVR_StartRemoteConfig的返回值</param>
        /// <param name="dwDataType">数据类型，跟长连接接口NET_DVR_StartRemoteConfig的命令参数（dwCommand）有关，详见“Remarks”说明 </param>
        /// <param name="pSendBuf">保存发送数据的缓冲区，与dwDataType有关，详见“Remarks”说明</param>
        /// <param name="dwBufSize">发送数据的长度</param>
        /// <returns>TRUE表示成功，FALSE表示失败</returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern bool NET_DVR_SendRemoteConfig(int lHandle, uint dwDataType, IntPtr pSendBuf, uint dwBufSize);


        /// <summary>
        /// 卡参数配置结构体
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_CARD_CFG_V50
        {
            /// <summary>
            /// 结构体大小
            /// </summary>
            public uint dwSize;
            /// <summary>
            /// 需要修改的卡参数（设置卡参数时有效），按位表示，每位代表一种参数，值：0- 不修改，1- 需要修改
            /// </summary>
            public uint dwModifyParamType;
            /// <summary>
            /// 卡号
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = ACS_CARD_NO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byCardNo;
            /// <summary>
            /// 卡是否有效：0- 无效，1- 有效（用于删除卡，设置时置为0进行删除，获取时此字段始终为1） 
            /// </summary>
            public byte byCardValid;
            //卡类型：1- 普通卡（默认），2- 残疾人卡，3- 黑名单卡，4- 巡更卡，5- 胁迫卡，6- 超级卡，7- 来宾卡，8- 解除卡，9- 员工卡，10- 应急卡，11- 应急管理卡
            public byte byCardType;
            /// <summary>
            /// 是否为首卡：1- 是，0- 否 
            /// </summary>
            public byte byLeaderCard;
            /// <summary>
            /// 用户类型：0 – 普通用户1- 管理员用户
            /// </summary>
            public byte byUserType;
            /// <summary>
            /// 门权限（梯控的楼层权限），按字节表示，1-为有权限，0-为无权限，从低位到高位依次表示对门（或者梯控楼层）1-N是否有权限
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOOR_NUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byDoorRight;
            /// <summary>
            /// 有效期参数
            /// </summary>
            public NET_DVR_VALID_PERIOD_CFG struValid;
            /// <summary>
            /// 所属群组，按字节表示，1-属于，0-不属于，从低位到高位表示是否从属群组1~N
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_GROUP_NUM, ArraySubType = UnmanagedType.I1)]
            public byte[] byBelongGroup;
            /// <summary>
            /// 卡密码
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = CARD_PASSWORD_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byCardPassword;
            /// <summary>
            /// 卡权限计划，取值为计划模板编号，同个门不同计划模板采用权限或的方式处理
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOOR_NUM * MAX_CARD_RIGHT_PLAN_NUM, ArraySubType = UnmanagedType.I1)]
            public ushort[,] wCardRightPlan;
            /// <summary>
            /// 最大刷卡次数，0为无次数限制
            /// </summary>
            public uint dwMaxSwipeTime;
            /// <summary>
            /// 已刷卡次数
            /// </summary>
            public uint dwSwipeTime;
            /// <summary>
            /// 房间号 
            /// </summary>
            public ushort wRoomNumber;
            /// <summary>
            /// 层号
            /// </summary>
            public short wFloorNumber;
            /// <summary>
            /// 工号
            /// </summary>
            public uint dwEmployeeNo;
            /// <summary>
            /// 姓名
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byName;
            /// <summary>
            /// 部门编号
            /// </summary>
            public ushort wDepartmentNo;
            /// <summary>
            /// 排班计划编号
            /// </summary>
            public ushort wSchedulePlanNo;
            /// <summary>
            /// 排班计划类型：0- 无意义，1- 个人，2- 部门 
            /// </summary>
            public byte bySchedulePlanType;
            /// <summary>
            /// 保留，置为0
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            /// <summary>
            /// 锁ID 
            /// </summary>
            public uint dwLockID;
            /// <summary>
            /// 锁代码
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_LOCK_CODE_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byLockCode;
            /// <summary>
            /// 房间代码
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DOOR_CODE_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byRoomCode;
            /// <summary>
            /// 卡权限
            /// </summary>
            public uint dwCardRight;
            /// <summary>
            /// 计划模板(每天)各时间段是否启用，按位表示，0--不启用，1-启用 
            /// </summary>
            public uint dwPlanTemplate;
            /// <summary>
            /// 持卡人ID 
            /// </summary>
            public uint dwCardUserId;
            /// <summary>
            /// 卡模式类型，0-空，1- MIFARE S50，2- MIFARE S70，3- FM1208 CPU卡，4- FM1216 CPU卡，5-国密CPU卡，6-身份证，7- NFC 
            /// </summary>
            public byte byCardModelType;
            /// <summary>
            /// 保留，置为0
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 83, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
        }

        /// <summary>
        /// 有效期参数结构体
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_VALID_PERIOD_CFG
        {
            /// <summary>
            /// 是否启用该有效期：0- 不启用，1- 启用
            /// </summary>
            public byte byEnable;
            /// <summary>
            /// 是否限制起始时间的标志，0-不限制，1-限制
            /// </summary>
            public byte byBeginTimeFlag;
            /// <summary>
            /// 是否限制终止时间的标志，0-不限制，1-限制 
            /// </summary>
            public byte byEnableTimeFlag;
            /// <summary>
            /// 有效期索引,从0开始（时间段通过SDK设置给锁，后续在制卡时，只需要传递有效期索引即可，以减少数据量..
            /// 即一个用户一个卡可以设置多个 有效期索引。。而不用多张卡
            /// </summary>
            public byte byTimeDurationNo;
            /// <summary>
            /// 有效期起始时间
            /// </summary>
            public NET_DVR_TIME_EX struBeginTime;
            /// <summary>
            /// 有效期结束时间
            /// </summary>
            public NET_DVR_TIME_EX struEndTime;
            /// <summary>
            /// 保留，置为0
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        /// <summary>
        /// 时间参数结构体
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_TIME_EX
        {
            public ushort wYear;
            public byte byMonth;
            public byte byDay;
            public byte byHour;
            public byte byMinute;
            public byte bySecond;
            /// <summary>
            /// 保留
            /// </summary>
            public byte byRes;
        }


        /// <summary>
        /// 停止注册。关闭长连接配置接口所创建的句柄，释放资源。所有主机范围内探测器完成注册后调用接口NET_DVR_StopRemoteConfig释放资源。
        /// </summary>
        /// <param name="lHandle">句柄，NET_DVR_StartRemoteConfig的返回值 </param>
        /// <returns>TRUE表示成功，FALSE表示失败。</returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern bool NET_DVR_StopRemoteConfig(int lHandle);

        #endregion

        #region 远程门控制开关

        /// <summary>
        /// 远程门禁控制或梯控控制。
        /// </summary>
        /// <param name="lUserID">登录接口的返回值</param>
        /// <param name="lGatewayIndex">门禁序号（楼层编号），从1开始，-1表示对所有门（或者梯控的所有楼层）进行操作</param>
        /// <param name="dwStaic">命令值：0- 关闭（对于梯控，表示受控），1- 打开（对于梯控，表示开门），2- 常开（对于梯控，表示自由），3- 常关（对于梯控，表示禁用），4- 恢复（梯控），5- 访客呼梯（梯控），6- 住户呼梯（梯控）</param>
        /// <returns></returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern bool NET_DVR_ControlGateway(int lUserID, int lGatewayIndex, uint dwStaic);
        #endregion

        #region 布防获取门禁事件
        /// <summary>
        /// MAC地址长度
        /// </summary>
        public const int MACADDR_LEN = 6;

        /// <summary>
        /// 门禁主机报警信息
        /// </summary>
        public const int COMM_ALARM_ACS = 0x5002;
        /// <summary>
        /// 门禁身份证刷卡信息
        /// </summary>
        public const int COMM_ID_INFO_ALARM = 0x5200;
        /// <summary>
        /// 门禁通行人数信息
        /// </summary>
        public const int COMM_PASSNUM_INFO_ALARM = 0x5201;

        /// <summary>
        /// 注册回调函数，接收设备报警消息等。
        /// </summary>
        /// <param name="fMessageCallBack">回调函数 </param>
        /// <param name="pUser">用户数据</param>
        /// <returns>TRUE表示成功，FALSE表示失败。</returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRMessageCallBack_V31(MSGCallBack_V31 fMessageCallBack, IntPtr pUser);


        /// <summary>
        /// 报警回调函数..有返回值，接收到数据需要返回TRUE。该接口中回调函数的第一个参数（lCommand）和第三个参数（pAlarmInfo）是密切关联的，
        /// </summary>
        /// <param name="lCommand">上传的消息类型，不同的报警信息对应不同的类型，通过类型区分是什么报警信息，详见“Remarks”中列表</param>
        /// <param name="pAlarmer">报警设备信息，包括设备序列号、IP地址、登录IUserID句柄等</param>
        /// <param name="pAlarmInfo">报警信息，通过lCommand值判断pAlarmer对应的结构体，详见“Remarks”中列表</param>
        /// <param name="dwBufLen">报警信息缓存大小</param>
        /// <param name="pUser">用户数据</param>
        /// <returns></returns>
        public delegate bool MSGCallBack_V31(int lCommand, ref NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser);


        /// <summary>
        /// 报警设备信息
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_ALARMER
        {
            /// <summary>
            /// /* userid是否有效 0-无效，1-有效 */
            /// </summary>
            public byte byUserIDValid;
            /// <summary>
            /// /* 序列号是否有效 0-无效，1-有效 */
            /// </summary>
            public byte bySerialValid;
            /// <summary>
            /// /* 版本号是否有效 0-无效，1-有效 */
            /// </summary>
            public byte byVersionValid;
            /// <summary>
            /// /* 设备名字是否有效 0-无效，1-有效 */
            /// </summary>
            public byte byDeviceNameValid;
            /// <summary>
            /// /* MAC地址是否有效 0-无效，1-有效 */
            /// </summary>
            public byte byMacAddrValid;
            /// <summary>
            /// /* login端口是否有效 0-无效，1-有效 */
            /// </summary>
            public byte byLinkPortValid;
            /// <summary>
            /// /* 设备IP是否有效 0-无效，1-有效 */
            /// </summary>
            public byte byDeviceIPValid;
            /// <summary>
            /// /* socket ip是否有效 0-无效，1-有效 */
            /// </summary>
            public byte bySocketIPValid;
            /// <summary>
            /// /* NET_DVR_Login()返回值, 布防时有效 */
            /// </summary>
            public int lUserID;
            /// <summary>
            /// /* 序列号 */
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;
            /// <summary>
            /// /* 版本信息 高16位表示主版本，低16位表示次版本*/
            /// </summary>
            public uint dwDeviceVersion;
            /// <summary>
            /// /* 设备名字 */
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sDeviceName;
            /// <summary>
            /// /* MAC地址 */
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMacAddr;
            /// <summary>
            /// /* link port */
            /// </summary>
            public ushort wLinkPort;
            /// <summary>
            /// /* IP地址 */
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sDeviceIP;
            /// <summary>
            /// /* 报警主动上传时的socket IP地址 */
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sSocketIP;
            /// <summary>
            /// /* Ip协议 0-IPV4, 1-IPV6 */
            /// </summary>
            public byte byIpProtocol;
            /// <summary>
            /// 保留，置为0
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }


        /// <summary>
        /// 门禁主机报警信息结构体
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_ACS_ALARM_INFO
        {
            /// <summary>
            /// 结构体大小
            /// </summary>
            public uint dwSize;
            /// <summary>
            /// 报警主类型，具体定义见“Remarks”说明
            /// </summary>
            public uint dwMajor;
            /// <summary>
            /// 报警次类型，次类型含义根据主类型不同而不同，具体定义见“Remarks”说明
            /// </summary>
            public uint dwMinor;
            /// <summary>
            /// 报警时间 
            /// </summary>
            NET_DVR_TIME struTime;
            /// <summary>
            /// 网络操作的用户名
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NAMELEN, ArraySubType = UnmanagedType.I1)]
            byte[] sNetUser;
            /// <summary>
            /// 远程主机地址
            /// </summary>
            NET_DVR_IPADDR struRemoteHostAddr;
            /// <summary>
            /// 报警信息详细参数 
            /// </summary>
            NET_DVR_ACS_EVENT_INFO struAcsEventInfo;
            /// <summary>
            /// 图片数据大小，不为0是表示后面带数据
            /// </summary>
            public uint dwPicDataLen;
            /// <summary>
            /// 图片数据缓冲区
            /// </summary>
            IntPtr pPicData;
            /// <summary>
            /// 保留，置为0
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 24, ArraySubType = UnmanagedType.I1)]
            byte[] byRes;
        }

        /// <summary>
        /// 报警
        /// </summary>
        public const int MAJOR_ALARM = 0x01;
        /// <summary>
        /// 异常
        /// </summary>
        public const int MAJOR_EXCEPTION = 0x02;
        /// <summary>
        /// 操作
        /// </summary>
        public const int MAJOR_OPERATION = 0x03;
        /// <summary>
        /// 事件
        /// </summary>
        public const int MAJOR_EVENT = 0x04;
        

        /// <summary>
        /// 时间参数结构体
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_TIME
        {
            public uint dwYear;
            public uint dwMonth;
            public uint dwDay;
            public uint dwHour;
            public uint dwMinute;
            public uint dwSecond;
        }

        /// <summary>
        /// IP地址结构体。
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_IPADDR
        {
            /// <summary>
            /// 设备IPv4地址
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string sIpV4;

            /// <summary>
            /// 设备IPv6地址
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;


        }

        /// <summary>
        /// 门禁主机事件信息。
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_ACS_EVENT_INFO
        {
            /// <summary>
            /// 结构体大小 
            /// </summary>
            public uint dwSize;
            /// <summary>
            /// 卡号
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = ACS_CARD_NO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byCardNo;
            /// <summary>
            /// 卡类型：1- 普通卡，2- 残疾人卡，3- 黑名单卡，4- 巡更卡，5- 胁迫卡，6- 超级卡，7- 来宾卡，8- 解除卡，为0表示无效 
            /// </summary>
            public byte byCardType;
            /// <summary>
            /// 白名单单号，取值范围：1~8，0表示无效 
            /// </summary>
            public byte byWhiteListNo;
            /// <summary>
            /// 报告上传通道：1- 布防上传，2- 中心组1上传，3- 中心组2上传，0表示无效 
            /// </summary>
            public byte byReportChannel;
            /// <summary>
            /// 读卡器类型：0- 无效，1- IC读卡器，2- 身份证读卡器，3- 二维码读卡器，4- 指纹头 
            /// </summary>
            public byte byCardReaderKind;
            /// <summary>
            /// 读卡器编号，为0表示无效
            /// </summary>
            public uint dwCardReaderNo;
            /// <summary>
            /// 门编号（或者梯控的楼层编号），为0表示无效（当接的设备为人员通道设备时，门1为进方向，门2为出方向） 
            /// </summary>
            public uint dwDoorNo;
            /// <summary>
            /// 多重卡认证序号，为0表示无效
            /// </summary>
            public uint dwVerifyNo;
            /// <summary>
            /// 报警输入号，为0表示无效 
            /// </summary>
            public uint dwAlarmInNo;
            /// <summary>
            /// 报警输出号，为0表示无效 
            /// </summary>
            public uint dwAlarmOutNo;
            /// <summary>
            /// 事件触发器编号 
            /// </summary>
            public uint dwCaseSensorNo;
            /// <summary>
            /// RS485通道号，为0表示无效 
            /// </summary>
            public uint dwRs485No;
            /// <summary>
            /// 群组编号
            /// </summary>
            public uint dwMultiCardGroupNo;
            /// <summary>
            /// 人员通道号 
            /// </summary>
            public ushort wAccessChannel;
            /// <summary>
            /// 设备编号，为0表示无效
            /// </summary>
            public byte byDeviceNo;
            /// <summary>
            /// 分控器编号，为0表示无效
            /// </summary>
            public byte byDistractControlNo;
            /// <summary>
            /// 工号，为0无效 
            /// </summary>
            public uint dwEmployeeNo;
            /// <summary>
            /// 就地控制器编号，0-门禁主机，1-255代表就地控制器
            /// </summary>
            public ushort wLocalControllerID;
            /// <summary>
            /// 网口ID：（1-上行网口1,2-上行网口2,3-下行网口1）
            /// </summary>
            public byte byInternetAccess;
            /// <summary>
            /// 防区类型，0:即时防区,1-24小时防区,2-延时防区,3-内部防区,4-钥匙防区,5-火警防区,6-周界防区,7-24小时无声防区,8-24小时辅助防区,9-24小时震动防区,10-门禁紧急开门防区,11-门禁紧急关门防区，0xff-无 
            /// </summary>
            public byte byType;
            /// <summary>
            /// 物理地址，为0无效 
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMACAddr;
            /// <summary>
            /// 刷卡类型，0-无效，1-二维码
            /// </summary>
            public byte bySwipeCardType;
            /// <summary>
            /// 保留，置为0 
            /// </summary>
            public byte byRes2;
            /// <summary>
            /// 事件流水号，为0无效
            /// </summary>
            public uint dwSerialNo;
            /// <summary>
            /// 通道控制器ID，为0无效，1-主通道控制器，2-从通道控制器 
            /// </summary>
            public byte byChannelControllerID;
            /// <summary>
            /// 通道控制器灯板ID，为0无效（有效范围1-255）
            /// </summary>
            public byte byChannelControllerLampID;
            /// <summary>
            /// 通道控制器红外转接板ID，为0无效（有效范围1-255）
            /// </summary>
            public byte byChannelControllerIRAdaptorID;
            /// <summary>
            /// 通道控制器红外对射ID，为0无效（有效范围1-255）
            /// </summary>
            public byte byChannelControllerIREmitterID;
            /// <summary>
            /// 保留，置为0 
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

        }




        /// <summary>
        /// 身份证刷卡信息上传结构体。
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct  NET_DVR_ID_CARD_INFO_ALARM
        {
            //DWORD dwSize;
            //NET_DVR_ID_CARD_INFO struIDCardCfg;
            //DWORD dwMajor;
            //DWORD dwMinor;
            //NET_DVR_TIME_V30 struSwipeTime;
            //BYTE byNetUser[MAX_NAMELEN];
            //NET_DVR_IPADDR struRemoteHostAddr;
            //DWORD dwCardReaderNo;
            //DWORD dwDoorNo;
            //DWORD dwPicDataLen;
            //char* pPicData;
            //BYTE byCardType;
            //BYTE byDeviceNo;
            //BYTE byRes2[2];
            //DWORD dwFingerPrintDataLen;
            //char* pFingerPrintData;
            //DWORD dwCapturePicDataLen;
            //char* pCapturePicData;
            //BYTE byRes[188];

        }


        /// <summary>
        /// 通行人数信息上传结构体。
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_PASSNUM_INFO_ALARM
        {
            //DWORD dwSize;
            //DWORD dwAccessChannel;
            //NET_DVR_TIME_V30 struSwipeTime;
            //BYTE byNetUser[MAX_NAMELEN];
            //NET_DVR_IPADDR struRemoteHostAddr;
            //DWORD dwEntryTimes;
            //DWORD dwExitTimes;
            //DWORD dwTotalTimes;
            //BYTE byRes[300];

        }


        /// <summary>
        /// 报警布防。建立报警上传通道，获取报警等信息。
        /// </summary>
        /// <param name="lUserID">NET_DVR_Login或者NET_DVR_Login_V30的返回值</param>
        /// <param name="lpSetupParam">报警布防参数</param>
        /// <returns>-1表示失败，其他值作为NET_DVR_CloseAlarmChan_V30函数的句柄参数。</returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern int NET_DVR_SetupAlarmChan_V41(int lUserID, ref NET_DVR_SETUPALARM_PARAM lpSetupParam);


        /// <summary>
        /// 报警布防参数结构体
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SETUPALARM_PARAM
        {
            public uint dwSize;
            public byte byLevel;//布防优先级：0- 一等级（高），1- 二等级（中），2- 三等级（低，保留）
            public byte byAlarmInfoType;//上传报警信息类型（智能交通摄像机支持）：0- 老报警信息（NET_DVR_PLATE_RESULT），1- 新报警信息(NET_ITS_PLATE_RESULT) 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//这里保留音频的压缩参数 
        }


        /// <summary>
        /// 撤销报警上传通道
        /// </summary>
        /// <param name="lAlarmHandle">NET_DVR_SetupAlarmChan_V30或者NET_DVR_SetupAlarmChan_V41的返回值 </param>
        /// <returns>TRUE表示成功，FALSE表示失败</returns>
        [DllImport(@"..\bin\HCNetSDK.dll")]
        public static extern bool NET_DVR_CloseAlarmChan_V30(int lAlarmHandle);


        #endregion
    }
}
