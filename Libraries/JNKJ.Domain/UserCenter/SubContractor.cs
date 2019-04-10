using System;
namespace JNKJ.Domain.UserCenter
{
    ///<summary>
    /// 企业信息
    ///</summary>
    public class SubContractor : BaseEntity
    {
        ///<summary>
        ///企业名称
        ///</summary>
        public string CompanyName { set; get; }
        ///<summary>
        ///单位性质，参建单位性质字典表
        ///</summary>
        public string OrganizationType { set; get; }
        ///<summary>
        ///组织机构编号
        ///</summary>
        public string OrganizationCode { set; get; }
        ///<summary>
        ///工商营业执照注册号
        ///</summary>
        public string BizRegisterCode { set; get; }
        ///<summary>
        ///社会统一信用代码，与营业执照号两者中必有一项为必填
        ///</summary>
        public string SocialCreditNumber { set; get; }
        ///<summary>
        ///注册地区编码，参见地区字典数据
        ///</summary>
        public string AreaCode { set; get; }
        ///<summary>
        ///企业营业地址
        ///</summary>
        public string Address { set; get; }
        ///<summary>
        ///邮政编码
        ///</summary>
        public string ZipCode { set; get; }
        ///<summary>
        ///法定代表人姓名
        ///</summary>
        public string RepresentativeName { set; get; }
        ///<summary>
        ///法定代表人证件类型，参见人员证件类型字典表
        ///</summary>
        public int RepresentativeIDCardType { set; get; }
        ///<summary>
        ///法定代表人证件号码
        ///</summary>
        public string RepresentativeIDCardNumber { set; get; }
        ///<summary>
        ///注册资本（万元）
        ///</summary>
        public Decimal RegistrationCapital { set; get; }
        ///<summary>
        ///注册资本币种,见币种字典表
        ///</summary>
        public string CapitalCurrency { set; get; }
        ///<summary>
        ///成立日期,精确到日，格式:2014-02-05
        ///</summary>
        public DateTime EstablishDate { set; get; }
        ///<summary>
        ///办公电话
        ///</summary>
        public string PhoneNumber { set; get; }
        ///<summary>
        ///传真号码
        ///</summary>
        public string FaxNumber { set; get; }
        ///<summary>
        ///联系人姓名
        ///</summary>
        public string ContactPeopleName { set; get; }
        ///<summary>
        ///联系人办公电话
        ///</summary>
        public string ContactPeoplePhone { set; get; }
        ///<summary>
        ///联系人手机号码
        ///</summary>
        public string ContactPeopleCellPhone { set; get; }
        ///<summary>
        ///企业联系邮箱
        ///</summary>
        public string Email { set; get; }
        ///<summary>
        ///企业网址
        ///</summary>
        public string HomeWebsiteUrl { set; get; }
        ///<summary>
        ///企业经营状态：0: 注销；1：正常
        ///</summary>
        public string BusinessStatus { set; get; }
        ///<summary>
        ///企业备注
        ///</summary>
        public string Memo { set; get; }
    }
}
