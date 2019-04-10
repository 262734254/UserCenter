using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcyg.Domain.Dto.Customers
{
    /// <summary>
    /// 企业信息请求类
    /// </summary>
    public class EnterpriseInfoRequest
    {
        /// <summary>
        /// ID，新增时可为空
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// 企业名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 省代码
        /// </summary>
        public string LocationProvinceCode { get; set; }
        /// <summary>
        /// 市代码
        /// </summary>
        public string LocationCityCode { get; set; }
        /// <summary>
        /// 区代码
        /// </summary>
        public string LocationDistrictCode { get; set; }
        /// <summary>
        /// 企业地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 企业联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 企业类型名
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 安全生产许可证
        /// </summary>
        public string Safety { get; set; }
        /// <summary>
        /// 安全生产许可证_待审核字段，用于修改敏感信息审核通过后使用
        /// </summary>
        public string SafetyToAudit { get; set; }
        /// <summary>
        /// 法定代表人
        /// </summary>
        public string RepresentativePerson { get; set; }
        /// <summary>
        /// 法定代表电话
        /// </summary>
        public string RepresentativeTel { get; set; }
        /// <summary>
        /// 社会统一信用代码
        /// </summary>
        public string CreditCode { get; set; }
        /// <summary>
        /// 社会统一信用代码_待审核字段，用于修改敏感信息审核通过后使用
        /// </summary>
        public string CreditCodeToAudit { get; set; }
        /// <summary>
        /// 营业执照注册号
        /// </summary>
        public string BusinessLicenseNum { get; set; }
        /// <summary>
        /// 营业执照注册号_待审核字段，用于修改敏感信息审核通过后使用
        /// </summary>
        public string BusinessLicenseNumToAudit { get; set; }
        /// <summary>
        /// 企业邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 注册资金
        /// </summary>
        public decimal RegisteredCapital { get; set; }
        /// <summary>
        /// 注册资金_待审核字段，用于修改敏感信息审核通过后使用
        /// </summary>
        public decimal RegisteredCapitalToAudit { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string OpenAnAccount { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 资质证书号
        /// </summary>
        public string AptitudeCertificate { get; set; }
        /// <summary>
        /// 资质证书号_待审核字段，用于修改敏感信息审核通过后使用
        /// </summary>
        public string AptitudeCertificateToAudit { get; set; }
        /// <summary>
        /// 状态：0-未审核，1-审核通过，2-审核未通过
        /// </summary>
        public int? State { get; set; }
        /// <summary>
        /// 单位性质
        /// </summary>
        public string Nature { get; set; }

        /// <summary>
        /// 创建者用户ID。关联注册用户信息表（EnterpriseUserInfo）
        /// </summary>
        public Guid RegisterUserID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? RepresentativeIDCardType { get; set; }
    }



    /// <summary>
    /// 分页查询请求类
    /// </summary>
    public class EnterpriseInfoGetRequest
    {
        public Guid? Id { get; set; }
       
        /// <summary>
        /// 企业名称
        /// </summary>
        public string Name { get; set; }



        /// <summary>
        /// 企业状态
        /// </summary>
        public int? State { get; set; }




        /// <summary>
        /// 注册时间(开始时间与结束时间)
        /// </summary>
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }




        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int pageSize { get; set; }

    }


    /// <summary>
    /// 审核请求类
    /// </summary>
    public class EnterpriseInfoAuditRequest
    {
        /// <summary>
        /// 企业ID
        /// </summary>
        public Guid? EnterpriseID { get; set; }


        /// <summary>
        /// 是否审核通过
        /// </summary>
        public bool IsAuditPass { get; set; }
    }
}
