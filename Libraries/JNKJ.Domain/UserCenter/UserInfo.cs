using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Domain.UserCenter
{
    /// <summary>
    /// 用户表  备注:为避免工程易管系统报错，修改了用户表请同步修改工程易管中的UserInfo类
    /// </summary>
    public class UserInfo : BaseEntity
    {
        ///<summary>
        ///企业id
        ///</summary>
        public Guid? EnterpriseInfoID { set; get; }
        ///<summary>
        ///该用户设置的默认项目ID
        ///</summary>
        public Guid? DefaultProjectID { set; get; }
        ///<summary>
        ///用户名（默认手机号）
        ///</summary>
        public string UserName { set; get; }
        ///<summary>
        ///注册时间
        ///</summary>
        public DateTime? SubTime { set; get; }
        ///<summary>
        ///修改时间
        ///</summary>
        public DateTime? ModifiedTime { set; get; }
        ///<summary>
        ///备注
        ///</summary>
        public string Remark { set; get; }
        ///<summary>
        ///排序
        ///</summary>
        public int? Sort { set; get; }
        ///<summary>
        ///头像
        ///</summary>
        public string UserImgUrl { set; get; }
        ///<summary>
        ///用户类别。1=管理员，2=其它用户人员。
        ///</summary>
        public int? UserTypeEnum { set; get; }
        ///<summary>
        ///通讯地址
        ///</summary>
        public string PostalAddress { set; get; }
        ///<summary>
        ///详细地址
        ///</summary>
        public string DetailAddress { set; get; }
        ///<summary>
        ///手机号
        ///</summary>
        public string Phone { set; get; }
        ///<summary>
        ///是否是本APP的创建者。0或NULL表示否，1表示是。
        ///</summary>
        public bool? IsFirstUserOnApp { set; get; }
        ///<summary>
        ///昵称
        ///</summary>
        public string NickName { set; get; }
        ///<summary>
        ///姓名
        ///</summary>
        public string ChsName { set; get; }
        ///<summary>
        ///性别。NULL或0为男，1为女。
        ///</summary>
        public int? Sex { set; get; }
        ///<summary>
        ///帐号状态。0或NULL表示正常，1=已经被禁用。
        ///</summary>
        public int? AccountState { set; get; }
        ///<summary>
        ///正面身份证照片URL
        ///</summary>
        public string FrontUserIdCardImgUrl { set; get; }
        ///<summary>
        ///反面身份证照片URL
        ///</summary>
        public string BehindUserIdCardImgUrl { set; get; }
        ///<summary>
        ///身份证住址
        ///</summary>
        public string IdCardAddress { set; get; }
        ///<summary>
        ///身份证真实姓名
        ///</summary>
        public string IdCardFullName { set; get; }
        ///<summary>
        ///身份证号码
        ///</summary>
        public string IdCardNo { set; get; }
        ///<summary>
        ///角色Id
        ///</summary>
        public Guid? RolesID { set; get; }
        ///<summary>
        ///TIM_UserSig
        ///</summary>
        public string UserSig { set; get; }
        ///<summary>
        ///是否播放项目提示语（安卓上控制是否播放开关）
        ///</summary>
        public bool? IsPlay { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string FaceImg { set; get; }
        ///<summary>
        ///
        ///</summary>
        public string FaceToken { set; get; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
