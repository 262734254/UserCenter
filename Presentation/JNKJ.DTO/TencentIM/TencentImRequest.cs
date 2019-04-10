using System.Collections;
namespace JNKJ.Dto.TencentIM
{
    /// <summary>
    /// 腾讯云通信_请求类
    /// </summary>
    public class TencentImRequest
    {
    }
    /// <summary>
    /// 批量发单聊消息
    /// </summary>
    public class BatchSendmsgRequest
    {
        /// <summary>
        /// 用户名
        /// 必填，获取当前登录角色的用户名
        /// 用于生成当前用户的SIG
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 1：把消息同步到From_Account在线终端和漫游上；
        /// 2：消息不同步至 From_Account；
        /// 若不填写默认情况下会将消息存 From_Account 漫游
        /// 选填
        /// </summary>
        public int SyncOtherMachine { get; set; }
        /// <summary>
        /// 消息发送方帐号
        /// 管理员指定消息发送方帐号
        /// 选填
        /// </summary>
        public string From_Account { get; set; }
        /// <summary>
        /// 消息接收方帐号
        /// 必填
        /// </summary>
        public string To_Account { get; set; }
        /// <summary>
        /// 消息随机数,由随机函数产生。
        /// 用作消息去重
        /// 必填
        /// </summary>
        public int MsgRandom { get; set; }
        /// <summary>
        /// 消息内容，具体格式请参考 消息格式描述。
        /// 注意，一条消息可包括多种消息元素，MsgBody 为 Array 类型
        /// 必填
        /// </summary>
        public object MsgBody { get; set; }
        /// <summary>
        /// TIM消息对象类型
        /// 目前支持的消息对象包括： TIMTextElem(文本消息),TIMFaceElem(表情消息)， TIMLocationElem(位置消息)，TIMCustomElem(自定义消息)。
        /// 必填
        /// </summary>
        public string MsgType { get; set; }
        /// <summary>
        /// 对于每种 MsgType 用不同的 MsgContent 格式
        /// 必填
        /// </summary>
        public object MsgContent { get; set; }
        /// <summary>
        /// 离线推送信息配置，具体可参考 消息格式描述。
        /// 选填
        /// </summary>
        public object OfflinePushInfo { get; set; }
    }
    /// <summary>
    /// 单发单聊消息_请求类
    /// </summary>
    public class SendmsgRequest
    {
        /// <summary>
        /// 用户名
        /// 必填，获取当前登录角色的用户名
        /// 用于生成当前用户的SIG
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 1：把消息同步到 From_Account 在线终端和漫游上；
        /// 2：消息不同步至 From_Account；
        /// 若不填写默认情况下会将消息存 From_Account 漫游
        /// 选填
        /// </summary>
        public int SyncOtherMachine { get; set; }
        /// <summary>
        /// 消息发送方帐号
        /// （用于指定发送消息方帐号）
        /// 选填
        /// </summary>
        public string From_Account { get; set; }
        /// <summary>
        /// 消息接收方帐号
        /// 必填
        /// </summary>
        public string To_Account { get; set; }
        /// <summary>
        /// 消息离线保存时长（秒数），最长为 7 天（604800s）。
        /// 若消息只发在线用户，不想保存离线，则该字段填 0。若不填，则默认保存 7 天
        /// 选填
        /// </summary>
        public int MsgLifeTime { get; set; }
        /// <summary>
        /// 消息随机数,由随机函数产生。
        /// 用作消息去重
        /// 必填
        /// </summary>
        public int MsgRandom { get; set; }
        /// <summary>
        /// 消息时间戳，unix 时间戳。
        /// 选填
        /// </summary>
        public int MsgTimeStamp { get; set; }
        /// <summary>
        /// 消息内容，具体格式请参考 消息格式描述。
        /// 注意，一条消息可包括多种消息元素，MsgBody 为 Array 类型
        /// 必填
        /// </summary>
        public object MsgBody { get; set; }
        /// <summary>
        /// TIM消息对象类型
        /// 目前支持的消息对象包括： TIMTextElem(文本消息),TIMFaceElem(表情消息)， TIMLocationElem(位置消息)，TIMCustomElem(自定义消息)。
        /// 必填
        /// </summary>
        public string MsgType { get; set; }
        /// <summary>
        /// 对于每种 MsgType 用不同的 MsgContent 格式
        /// 必填
        /// </summary>
        public object MsgContent { get; set; }
        /// <summary>
        /// 离线推送信息配置，具体可参考 消息格式描述。
        /// 选填
        /// </summary>
        public object OfflinePushInfo { get; set; }
    }
    /// <summary>
    /// 在群组中发送普通消息
    /// </summary>
    public class SendGroupSystemNotificationRequest
    {
        /// <summary>
        /// 向哪个群组发送消息
        /// 必填
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 32 位随机数。如果 5分钟内两条消息的随机值相同，后一条消息将被当做重复消息而丢弃。
        /// </summary>
        public int Random { get; set; }
        /// <summary>
        /// 消息体
        /// </summary>
        public object MsgBody { get; set; }
    }
    /// <summary>
    /// 批量添加好友_请求类
    /// </summary>
    public class FriendAddRequest
    {
        ///// <summary>
        ///// 用户名
        ///// 必填，获取当前登录角色的用户名
        ///// 用于生成当前用户的SIG
        ///// </summary>
        //public string Identifier { get; set; }
        /// <summary>
        /// 需要为该 Identifier 添加好友。
        /// 必填
        /// </summary>
        public string From_Account { get; set; }
        /// <summary>
        /// 好友结构体对象
        /// 必填
        /// </summary>
        public IList AddFriendItem { get; set; }
        /// <summary>
        /// 加好友方式（默认双向加好友方式）："Add_Type_Single" 表示单向加好友；"Add_Type_Both" 表示双向加好友。
        /// </summary>
        public  string AddType { get; set; }
        /// <summary>
        /// 管理员强制加好友标记：1 表示强制加好友；0 表示常规加好友方式。
        /// </summary>
        public int ForceAddFlags { get; set; }
    }
    /// <summary>
    /// 删除群组成员_请求类
    /// </summary>
    public class DeleteGroupMemberRequest
    {
        /// <summary>
        /// 用户名
        /// 必填，获取当前登录角色的用户名
        /// 用于生成当前用户的SIG
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 操作的群ID
        /// 必填
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 是否静默加人
        /// 选填，是否静默加人。0：非静默加人；1：静默加人。不填该字段默认为0。
        /// </summary>
        public int Silence { get; set; }
        /// <summary>
        /// 踢出用户原因
        /// 选填
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 待删除的群成员
        /// 必填
        /// </summary>
        public IList MemberToDel_Account { get; set; }
    }
    /// <summary>
    /// 增加群组成员_请求类
    /// </summary>
    public class AddGroupMemberRequest
    {
        /// <summary>
        /// 操作的群ID
        /// 必填
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 是否静默加人
        /// 选填，是否静默加人。0：非静默加人；1：静默加人。不填该字段默认为0。
        /// </summary>
        public int Silence { get; set; }
        /// <summary>
        /// 待添加的群成员数组
        /// 必填
        /// </summary>
        public IList MemberList { get; set; }
        /// <summary>
        /// 待添加的群成员帐号
        /// 必填
        /// </summary>
        public string Member_Account { get; set; }
    }
    /// <summary>
    /// 创建群组_请求类
    /// </summary>
    public class CreateGroupRequest
    {
        /// <summary>
        /// 用户名
        /// 必填，获取当前登录角色的用户名
        /// 用于生成当前用户的SIG
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 为了使得群组ID更加简单，便于记忆传播
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 群主id
        /// 选填，自动添加到群成员中。如果不填，群没有群主。
        /// </summary>
        public string Owner_Account { get; set; }
        /// <summary>
        /// 群组形态
        /// 必填，包括Public（公开群），Private（私密群），ChatRoom（聊天室），AVChatRoom（互动直播聊天室），BChatRoom（在线成员广播大群）。
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 群名称
        /// 必填，最长30字节。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 群简介
        /// 选填，最长240字节。
        /// </summary>
        public string Introduction { get; set; }
        /// <summary>
        /// 群公告
        /// 选填，最长300字节。
        /// </summary>
        public string Notification { get; set; }
        /// <summary>
        /// 群头像URL
        /// 选填，最长100字节。
        /// </summary>
        public string FaceUrl { get; set; }
        /// <summary>
        /// 最大群成员数量
        /// 选填，缺省时的默认值：私有群是200，公开群是2000，聊天室是10000，互动直播聊天室和在线成员广播大群无限制。
        /// </summary>
        public int MaxMemberCount { get; set; }
        /// <summary>
        /// 申请加群处理方式
        /// 选填，包含FreeAccess（自由加入），NeedPermission（需要验证），DisableApply（禁止加群），不填默认为NeedPermission（需要验证）。
        /// </summary>
        public string ApplyJoinOption { get; set; }
    }
    /// <summary>
    /// 解散群组_请求类
    /// </summary>
    public class DestroyGroupRequest
    {
        /// <summary>
        /// 操作的群ID。
        /// 必填
        /// </summary>
        public string GroupId { get; set; }
    }
    /// <summary>
    /// 修改群组基础资料_请求类
    /// </summary>
    public class ModifyGroupBasenfoRequest
    {
        /// <summary>
        /// 需要修改基础信息的群组的ID。
        /// 必填
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 群名称，最长30字节
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 设置用户资料_请求类
    /// </summary>
    public class SetPortraitRequest
    {
        /// <summary>
        /// 需要设置该Identifier的资料。
        /// 必填
        /// </summary>
        public string From_Account { get; set; }
        /// <summary>
        /// 待设置的用户的资料对象数组，数组中每一个对象都包含了Tag和Value。
        /// 必填
        /// </summary>
        public IList ProfileItem { get; set; }
        ///// <summary>
        ///// 指定要设置的资料字段的名称
        ///// 必填
        ///// </summary>
        //public string Tag { get; set; }
        ///// <summary>
        ///// 待设置的资料字段的值
        ///// 必填
        ///// </summary>
        //public string Value { get; set; }
    }
    /// <summary>
    /// 账号导入_请求类
    /// </summary>
    public class AccountImportRequest
    {
        /// <summary>
        /// 用户名
        /// 必填，长度不超过 32 字节
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 用户昵称
        /// 选填
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 用户头像URL
        /// 选填
        /// </summary>
        public string FaceUrl { get; set; }
        /// <summary>
        /// 帐号类型
        /// 选填，开发者默认无需填写，值0表示普通帐号，1表示机器人帐号。
        /// </summary>
        public int Type { get; set; }
    }
    /// <summary>
    /// 执行状态
    /// </summary>
    public class RequestResult
    {
        /// <summary>
        /// 请求处理的结果
        /// OK 表示处理成功，FAIL 表示失败
        /// </summary>
        public string ActionStatus { get; set; }
        /// <summary>
        /// 错误码
        /// 0 为成功，其他为失败
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string ErrorInfo { get; set; }
        /// <summary>
        /// IM用户名
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// IMUserSig
        /// </summary>
        public string UserSig { get; set; }
    }
}
