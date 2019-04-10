using JNKJ.Domain.UserCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Dto.UserCenter
{
    /// <summary>
    /// 用户请求类
    /// </summary>
    public class UserInfoRequest : UserInfo
    {
        #region 修改时用到的字段 （备注：目前用于PC端的方法-->ModityUserInfoForPC）
        /// <summary>
        /// 组织架构Id
        /// </summary>
        public Guid? DeptInfoId { get; set; }
        /// <summary>
        /// 用户状态 0为在职。1为离职
        /// </summary>
        public int? State { get; set; }
        #endregion
    }
    /// <summary>
    /// 审核请求类
    /// </summary>
    public class UserInfoAuditRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserInfoId { get; set; }
        /// <summary>
        /// 用户状态
        /// </summary>
        public int? UserState { get; set; }
        /// <summary>
        /// 是否审核通过
        /// </summary>
        public bool IsAuditPass { get; set; }
    }

    /// <summary>
    /// 用户实名认证请求类
    /// </summary>
    public class UserInfoCertificationRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 正面身份证照片URL
        /// </summary>
        public string FrontUserIdCardImgUrl { get; set; }

        /// <summary>
        /// 反面身份证照片URL
        /// </summary>
        public string BehindUserIdCardImgUrl { get; set; }

        /// <summary>
        /// 身份证住址
        /// </summary>
        public string IdCardAddress { get; set; }

        /// <summary>
        /// 身份证真实姓名
        /// </summary>
        public string IdCardFullName { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCardNo { get; set; }

        /// <summary>
        /// 性别。NULL或0为男，1为女。
        /// </summary>
        public int? Sex { get; set; }

    }

    /// <summary>
    /// 获取用户基本信息请求类
    /// </summary>
    public class UserInfoGetRequest
    {

        /// <summary>
        /// 帐号状态
        /// </summary>
        public int? AccountState { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string ChsName { get; set; }

        /// <summary>
        /// 注册开始时间
        /// </summary>
        public DateTime? SubStartTime { get; set; }

        /// <summary>
        /// 注册结束时间
        /// </summary>
        public DateTime? SubEndTime { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int? pageIndex { get; set; }

        public int? pageSize { get; set; }

        #region GetUserInfos（PC端接口使用字段）
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 账号状态
        /// </summary>
        public int? selectValue { get; set; }
        /// <summary>
        /// 查询的企业Id
        /// </summary>
        public Guid? selectEnterpriseInfo { get; set; }
        /// <summary>
        /// 查询的角色Id
        /// </summary>
        public Guid? selectRoleId { get; set; }
        /// <summary>
        /// 页面类型 用于区分审核页面还是用户信息页面
        /// </summary>
        public string accountType { get; set; }
        /// <summary>
        /// 组织架构部门Id
        /// </summary>
        public Guid? deptInfoId { get; set; }
        /// <summary>
        /// 用户状态 0为在职。1为离职
        /// </summary>
        public int? state { get; set; }
        //int pageIndex

        /// <summary>
        /// 用于查询没有企业Id是用户
        /// </summary>
        public bool selectNoEnID { get; set; }
        #endregion
    }
    /// <summary>
    /// 用户中心响应类
    /// </summary>
    public class UserCenterInfoResponse : UserInfo {
        
        /// <summary>
        /// 组织架构Id
        /// </summary>
        public Guid? DeptInfoId { set; get; }
        /// <summary>
        /// 用户状态 0为在职。1为离职
        /// </summary>
        public int? State { set; get; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RolesName { get; set; }

    }

    public class UserCenterInfoRequest{

        ///<summary>
        ///企业id
        ///</summary>
        public Guid? EnterpriseInfoID { set; get; }

        ///<summary>
        ///手机号
        ///</summary>
        public string Phone { set; get; }

        /// <summary>
        /// 组织架构Id
        /// </summary>
        public Guid? DeptInfoId { get; set; }

        /// <summary>
        /// 用户状态 0为在职。1为离职
        /// </summary>
        public int? State { get; set; }

        /// <summary>
        /// 帐号状态
        /// </summary>
        public int? AccountState { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string ChsName { get; set; }
        /// <summary>
        /// 关键字查询
        /// </summary>
        public string Keyword { get; set; }

        public string AccountType { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid? RolesID { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int? pageIndex { get; set; }
    }

    public class UserInfosForEmploymentRequest {
        public string Ids { get; set; }

        public Guid? EnterpriseInfoID { get; set; }
    }


}
