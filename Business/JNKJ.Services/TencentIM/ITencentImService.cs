using JNKJ.Dto.TencentIM;
namespace JNKJ.Services.TencentIM
{
    /// <summary>
    /// 腾讯IM云通信接口
    /// </summary>
    public interface ITencentImService
    {
        /// <summary>
        /// 账号导入
        /// </summary>
        /// <returns></returns>
        RequestResult AccountImport(AccountImportRequest request);
        /// <summary>
        /// 设置用户资料
        /// </summary>
        /// <returns></returns>
        RequestResult SetPortrait(SetPortraitRequest request);
        /// <summary>
        /// 根据用户名获取SIG
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        string Generate_Sig(string identifier);
        /// <summary>
        /// 创建群组
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        RequestResult CreateGroup(CreateGroupRequest request);
        /// <summary>
        /// 解散群组
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        RequestResult DestroyGroup(DestroyGroupRequest request);
        /// <summary>
        /// 修改群组基础资料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        RequestResult ModifyGroupBasenfo(ModifyGroupBasenfoRequest request);
        /// <summary>
        /// 增加群组成员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        RequestResult AddGroupMember(AddGroupMemberRequest request);
        /// <summary>
        /// 批量添加好友
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        RequestResult FriendAdd(FriendAddRequest request);
        /// <summary>
        /// 删除群组成员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        RequestResult DeleteGroupMember(DeleteGroupMemberRequest request);
        /// <summary>
        /// 在群组中发送系统通知
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        RequestResult SendGroupSystemNotification(SendGroupSystemNotificationRequest request);
    }
}
