using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Results;
using JNKJ.Dto.TencentIM;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Services.UserCenter.Interface
{
    public interface IUserInfo
    {
        #region Methods
        /// <summary>
        /// 修改用户信息 --用于内部使用
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        JsonResponse UpdateUserInfo(UserInfoRequest Request);

        /// <summary>
        /// 根据手机号码获取用户
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        JsonResponse GetUserInfoByPhone(string phone);


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userInfoRequest"></param>
        /// <returns></returns>
        JsonResponse AddUserInfo(UserInfoRequest userInfoRequest);
        /// <summary>
        /// 修改用户的基本信息_安卓
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        JsonResponse ModityUserImgForAndriod(UserInfoRequest Request);
        /// <summary>
        /// 修改用户的基本信息_PC
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        JsonResponse ModityUserInfoForPC(UserInfoRequest Request);
        /// <summary>
        /// 审核用户信息_PC
        /// </summary>
        /// <param name="userInfoAuditRequest"></param>
        /// <returns></returns>
        JsonResponse AuditUserInfo(UserInfoAuditRequest userInfoAuditRequest);
        /// <summary>
        /// 删除用户信息_PC
        /// </summary>
        /// <param name="userInfoId"></param>
        /// <returns></returns>
        JsonResponse DeleteUserInfo(Guid? userInfoId);

        /// <summary>
        /// 用户实名认证
        /// </summary>
        /// <param name="eucRequest"></param>
        /// <returns></returns>
        JsonResponse UserInfoCertification(UserInfoCertificationRequest eucRequest);
        /// <summary>
        /// 获取用户的基本信息   ----- 选择项目负责人（角色不为“游客”和“注册工匠”）_PC
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        List<UserCenterInfoResponse> GetUserInfosForPC(Guid? EnterpriseID, int? Type);
        /// <summary>
        /// 获取没有用工登记的用户信息 --用于工程易管系统查询
        /// </summary>
        /// <param name="Ids">用户Id集合</param>
        /// <returns></returns>
        List<UserCenterInfoResponse> GetUserInfosForEmployment(UserInfosForEmploymentRequest request);
        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="eugRequest"></param>
        /// <returns></returns>
        IPagedList<UserCenterInfoResponse> GetUserInfo(UserInfoGetRequest eugRequest);
        /// <summary>
        /// 获取用户基本信息_PC
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="selectValue"></param>
        /// <param name="selectEnterpriseInfo"></param>
        /// <param name="selectRoleId"></param>
        /// <param name="accountType"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        IPagedList<UserCenterInfoResponse> GetUserInfos(UserInfoGetRequest request);
        /// <summary>
        /// 通过用户ID获取用户的基本信息  
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        JsonResponse GetUserInfo(Guid UserId);

        /// <summary>
        /// GetEnterpriseUserInfo
        /// 通过用户ID和企业ID获取用户的基本信息 
        /// 企业ID用于查询《用户-企业-角色关系表》中的指定企业下用户角色
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        JsonResponse GetUserInfoByEnId(Guid UserId, Guid? EnterpriseID);
        /// <summary>
        /// 导入帐号到TIM
        /// </summary>
        /// <param name="userInfoList"></param>
        /// <returns></returns>
        JsonResponse ImportUserInfo(List<AccountImportRequest> userInfoList);
        /// <summary>
        /// 同步到人脸库
        /// </summary>
        /// <param name="userInfoList"></param>
        /// <returns></returns>
        JsonResponse SyncFaceUserInfo(List<FaceRequest> userInfoList);
        /// <summary>
        /// 设置用户的播放语音开关
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isPlay"></param>
        /// <returns></returns>
        JsonResponse SetIsPlay(Guid userId, string isPlay);

        /// <summary>
        /// 密码登陆
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        JsonResponse GetUserLoginByPasswork(LoginRequest loginRequest);

        #endregion

        #region Credit Methods(信用体系系统调用方法)
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse UpUserImage(UserInfoRequest request);
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        UserInfo GetUserInfoForCredit(UserInfoRequest request);

        #endregion

    }
}
