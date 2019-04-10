using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Dto.TencentIM;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.Face;
using JNKJ.Services.General;
using JNKJ.Services.UserCenter.Interface;
using JNKJ.WebAPI.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{
    public class UserInfoController : BaseController
    {
        #region Fields
        private readonly IUserInfo _userInfoService;
        #endregion

        #region Ctor
        public UserInfoController(IUserInfo userInfoService)
        {
            _userInfoService = userInfoService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 修改用户信息 --用于内部使用
        /// </summary>
        /// <param name="userInfoRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("update_userInfo")]
        public HttpResponseMessage UpdateUserInfo(UserInfoRequest userInfoRequest)
        {
            var result = _userInfoService.UpdateUserInfo(userInfoRequest);
            return toJson(null, result.State, result.Message);
        }

        [HttpGet]
        [ActionName("get_userinfo_by_phone")]
        public HttpResponseMessage GetUserInfoByPhone(string phone)
        {
            var result = _userInfoService.GetUserInfoByPhone(phone);
            return toJson(result.DataModel, result.State, result.Message);
        }


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userInfoRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("add_userInfo")]
        public HttpResponseMessage AddUserInfo(UserInfoRequest userInfoRequest)
        {
            //var token = System.Web.HttpContext.Current.Request.Headers["auth"];
            var result = _userInfoService.AddUserInfo(userInfoRequest);
            return toJson(null, result.State, result.Message);
        }
        /// <summary>
        /// 修改用户的基本信息_安卓
        /// </summary>
        /// <param name="userInfoRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("modity_userImg_for_andriod")]
        public HttpResponseMessage ModityUserImgForAndriod(UserInfoRequest userInfoRequest)
        {
            var result = _userInfoService.ModityUserImgForAndriod(userInfoRequest);
            return toJson(result.DataModel, result.State, result.Message);
        }
        /// <summary>
        /// 修改用户的基本信息_PC
        /// </summary>
        /// <param name="userInfoRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("modity_userImg_for_pc")]
        public HttpResponseMessage ModityUserInfoForPC(UserInfoRequest userInfoRequest)
        {
            var result = _userInfoService.ModityUserInfoForPC(userInfoRequest);
            return toJson(result.DataModel, result.State, result.Message);
        }
        /// <summary>
        /// 审核用户信息_PC
        /// </summary>
        /// <param name="userInfoAuditRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("audit_userInfo")]
        public HttpResponseMessage AuditUserInfo(UserInfoAuditRequest userInfoAuditRequest)
        {
            var result = _userInfoService.AuditUserInfo(userInfoAuditRequest);
            return toJson(result.DataModel, result.State, result.Message);
        }
        /// <summary>
        /// 删除用户信息_PC
        /// </summary>
        /// <param name="userInfoId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("delete_userInfo")]
        public HttpResponseMessage DeleteUserInfo([FromBody]Guid? userInfoId)
        {
            var result = _userInfoService.DeleteUserInfo(userInfoId);
            return toJson(result.DataModel, result.State, result.Message);
        }
        /// <summary>
        /// 用户实名认证
        /// </summary>
        /// <param name="eucRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("userInfo_certification")]
        public HttpResponseMessage UserInfoCertification(UserInfoCertificationRequest eucRequest)
        {
            var result = _userInfoService.UserInfoCertification(eucRequest);
            return toJson(result.DataModel, result.State, result.Message);
        }
        /// <summary>
        /// 获取用户的基本信息   ----- 选择项目负责人（角色不为“游客”和“注册工匠”）_PC
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("get_userInfos_for_pc")]
        public HttpResponseMessage GetUserInfosForPC([FromUri]Guid? EnterpriseID, int? Type)
        {
            return toJson(_userInfoService.GetUserInfosForPC(EnterpriseID, Type));
        }
        /// <summary>
        /// 获取没有用工登记的用户信息 --用于工程易管系统查询
        /// </summary>
        /// <param name="Ids">用户Id集合</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("get_userInfos_for_employment")]
        public HttpResponseMessage GetUserInfosForEmployment(UserInfosForEmploymentRequest request)
        {
            return toJson(_userInfoService.GetUserInfosForEmployment(request));
        }

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="eugRequest"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("get_userInfo")]
        public HttpResponseMessage GetUserInfo([FromUri]UserInfoGetRequest eugRequest)
        {
            var result = _userInfoService.GetUserInfo(eugRequest);
            var list = new PageList<UserCenterInfoResponse>()
            {
                PageIndex = eugRequest.pageIndex == null ? ConstKeys.DEFAULT_PAGEINDEX : eugRequest.pageIndex.Value,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Data = result.ToList()
            };
            return toListJson(list, OperatingState.Success, "获取成功");
        }
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
        [HttpGet]
        [ActionName("get_userInfos")]
        public HttpResponseMessage GetUserInfos([FromUri]UserInfoGetRequest request)
        {
            var result = _userInfoService.GetUserInfos(request);
            var list = new PageList<UserCenterInfoResponse>()
            {
                PageIndex = request.pageIndex == null ? ConstKeys.DEFAULT_PAGEINDEX : request.pageIndex.Value,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Data = result.ToList()
            };
            return toListJson(list, OperatingState.Success, "获取成功");
        }
        /// <summary>
        /// 通过用户ID获取用户的基本信息  
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("get_userInfo_by_Id")]
        public HttpResponseMessage GetUserInfo(Guid UserId)
        {
            var result = _userInfoService.GetUserInfo(UserId);
            return toJson(result.DataModel, result.State, result.Message);
        }
        /// <summary>
        /// GetEnterpriseUserInfo
        /// 通过用户ID和企业ID获取用户的基本信息 
        /// 企业ID用于查询《用户-企业-角色关系表》中的指定企业下用户角色
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("get_userInfo_by_enId")]
        public HttpResponseMessage GetUserInfoByEnId(Guid UserId, Guid? EnterpriseID)
        {
            var result = _userInfoService.GetUserInfoByEnId(UserId, EnterpriseID);
            return toJson(result.DataModel, result.State, result.Message);
        }
        /// <summary>
        /// 导入帐号到TIM
        /// </summary>
        /// <param name="userInfoList"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("import_userInfo")]
        public HttpResponseMessage ImportUserInfo([FromBody]List<AccountImportRequest> userInfoList)
        {
            var result = _userInfoService.ImportUserInfo(userInfoList);
            return toJson(null, result.State, result.Message);
        }
        /// <summary>
        /// 同步到人脸库
        /// </summary>
        /// <param name="userInfoList"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("syncface_userinfo")]
        public HttpResponseMessage SyncFaceUserInfo([FromBody]List<FaceRequest> userInfoList)
        {
            var result = _userInfoService.SyncFaceUserInfo(userInfoList);
            return toJson(null, result.State, result.Message);
        }
        /// <summary>
        /// 设置用户的播放语音开关
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isPlay"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("set_isplay")]
        public HttpResponseMessage SetIsPlay(Guid userId, string isPlay)
        {
            var result = _userInfoService.SetIsPlay(userId, isPlay);
            return toJson(null, result.State, result.Message);
        }

        #endregion

        #region Credit Methods(信用体系系统调用方法)
        [HttpPost]
        public HttpResponseMessage up_user_image(UserInfoRequest userInfoRequest)
        {
            var result = _userInfoService.UpUserImage(userInfoRequest);
            return toJson(null, result.State, result.Message);
        }

        [HttpGet]
        public HttpResponseMessage get_user_for_credit(UserInfoRequest userInfoRequest)
        {
            return toJson(_userInfoService.GetUserInfoForCredit(userInfoRequest));
        }
        #endregion


    }
}