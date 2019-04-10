using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using JNKJ.Data;
using JNKJ.Dto.TencentIM;
namespace JNKJ.Services.TencentIM
{
    /// <summary>
    /// 腾讯IM_接口实现
    /// </summary>
    public class TencentImService : ITencentImService
    {
        #region const
        //云通讯服务中申请的APPID
        private readonly uint Sdkappid = Convert.ToUInt32(ConfigurationSettings.AppSettings["Sdkappid"]);
        //系统生成的公私钥便于开发者快速开发
        public readonly string KeyName = ConfigurationSettings.AppSettings["KeyName"];
        //REST API 的 URL
        private const string Url = "https://console.tim.qq.com/v4/";
        #endregion
        #region REST API 的 URL 的 方法名
        public string AccountImportStr => "im_open_login_svc/account_import";              //账号导入
        public string CreateGroupStr => "group_open_http_svc/create_group";                //创建群组
        public string AddGroupMemberStr => "group_open_http_svc/add_group_member";         //增加群组成员
        public string DeleteGroupMemberStr => "group_open_http_svc/delete_group_member";   //删除群组成员
        public string SendmsgStr => "openim/sendmsg";                                      //单发单聊消息
        public string BatchSendmsgStr => "openim/batchsendmsg";                            //批量发单聊消息
        public string FriendAddStr => "sns/friend_add";                                    //批量添加好友
        public string SendGroupSystemNotificationStr => "group_open_http_svc/send_group_msg";          //在群组中发送系统通知
        public string SetPortraitStr => "profile/portrait_set";                            //设置资料
        public string DestroyGroupStr => "group_open_http_svc/destroy_group";              //解散群组
        public string ModifyGroupBasenfoStr => "group_open_http_svc/modify_group_base_info";     //修改群组基础资料
        #endregion
        #region operation
        /// <summary>
        /// 账号导入
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestResult AccountImport(AccountImportRequest request)
        {
            return RetString(request, AccountImportStr, request.Identifier);
        }
        /// <summary>
        /// 设置资料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestResult SetPortrait(SetPortraitRequest request)
        {
            return RetString(request, SetPortraitStr, "");
        }
        /// <summary>
        /// 创建群组
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestResult CreateGroup(CreateGroupRequest request)
        {
            return RetString(request, CreateGroupStr, request.Identifier);
        }
        /// <summary>
        /// 解散群组
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestResult DestroyGroup(DestroyGroupRequest request)
        {
            return RetString(request, DestroyGroupStr, "");
        }
        /// <summary>
        /// 修改群组基础资料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestResult ModifyGroupBasenfo(ModifyGroupBasenfoRequest request)
        {
            return RetString(request, ModifyGroupBasenfoStr, "");
        }
        /// <summary>
        /// 增加群组成员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestResult AddGroupMember(AddGroupMemberRequest request)
        {
            return RetString(request, AddGroupMemberStr, "");
        }
        /// <summary>
        /// 删除群主成员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestResult DeleteGroupMember(DeleteGroupMemberRequest request)
        {
            return RetString(request, DeleteGroupMemberStr,"");
        }
        /// <summary>
        /// 批量添加好友
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestResult FriendAdd(FriendAddRequest request)
        {
            return RetString(request, FriendAddStr, "");
        }
        /// <summary>
        /// 在群组中发送系统通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestResult SendGroupSystemNotification(SendGroupSystemNotificationRequest request)
        {
            return RetString(request, SendGroupSystemNotificationStr, "");
        }
        /// <summary>
        /// 请求网络API
        /// </summary>
        /// <param name="request">请求类</param>
        /// <param name="functionStr">接口方法名</param>
        /// <param name="identifier">TIM用户名</param>
        /// <returns></returns>
        private RequestResult RetString<T>(T request, string functionStr, string identifier)
        {
            //获取管理员的userSig
            var userSig = Generate_Sig("admin");
            //设置网络API地址
            var serviceAddress = $"{Url}{functionStr}?usersig={userSig}&identifier=admin&sdkappid={Sdkappid}&contenttype=json";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(serviceAddress);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            using (var dataStream = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                dataStream.Write(ToJson(request));
                dataStream.Close();
            }
            var response = (HttpWebResponse)httpWebRequest.GetResponse();
            var reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            var result = reader.ReadToEnd();
            //反序列化
            var requestResult = JsonHelper.DeserializeToModel<RequestResult>(result);
            if (!string.IsNullOrEmpty(identifier))
            {
                requestResult.UserSig = Generate_Sig(identifier);
                requestResult.Identifier = identifier;
            }
            return requestResult;
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string ToJson(object obj)
        {
            string str;
            if (obj is string || obj is char)
            {
                str = obj.ToString();
            }
            else
            {
                str = JsonHelper.SerializeToJson(obj);
            }
            return str;
        }
        #endregion
        #region TIM生成SIG接口
        /// <summary>
        /// 映射一个虚拟路径到物理路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string MapPath(string path)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }
        /// <summary>
        /// 生成SIG
        /// </summary>
        /// <returns></returns>
        public string Generate_Sig(string identifier)
        {
            var priKeyPath = Path.Combine(MapPath("~/App_Data/"), KeyName);
            var file = new FileStream(priKeyPath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(file);
            var b = new byte[file.Length];
            reader.Read(b, 0, b.Length);
            var priKey = Encoding.Default.GetString(b);
            var sig = new StringBuilder(4096);
            var errMsg = new StringBuilder(4096);
            var ret = tls_gen_sig_ex(
                Sdkappid,
                identifier,
                sig,
                4096,
                priKey,
                (uint)priKey.Length,
                errMsg,
                4096);
            if (0 != ret)
            {
                return errMsg.ToString();
            }
            return sig.ToString();
        }
        [DllImport("sigcheck.dll", EntryPoint = "tls_gen_sig_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tls_gen_sig_ex(
            UInt32 sdkappid,
            string identifier,
            StringBuilder sig,
            UInt32 sig_buff_len,
            string pri_key,
            UInt32 pri_key_len,
            StringBuilder err_msg,
            UInt32 err_msg_buff_len
        );
        #endregion
    }
}
