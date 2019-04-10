using Gcyg.Domain.Dto.Customers;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Results;
using JNKJ.Services.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Services.UserCenter.Interface
{
    public interface IEnterpriseInfoService
    {
        /// <summary>
        /// 获取系统内所有企业列表
        /// </summary>
        /// <returns></returns>
        IPagedList<EnterpriseInfo> GetAllEnterpriseInfo(int pageIndex = ConstKeys.DEFAULT_PAGEINDEX, int pageSize = ConstKeys.DEFAULT_MAX_PAGESIZE);


        /// <summary>
        /// 获取系统内所有企业列表
        /// </summary>
        /// <returns></returns>
        List<EnterpriseInfo> GetAllEnterpriseInfo();


        /// <summary>
        /// Get the SubContractor paged list
        /// </summary>
        /// <param name="subContractorId"></param>
        /// <param name="establishDate">成立日期 : 为空忽略</param>
        /// <param name="companyName">企业名称 : 为空忽略</param>
        /// <param name="organizationCode">组织机构编号 : 为空忽略</param>
        /// <param name="businessStatus">企业经营状态 : 为空忽略</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        IPagedList<EnterpriseInfo> GetEnterpriseInfoByRns(bool isAdmin, Guid? subContractorId, DateTime? establishDate,
            string companyName, string organizationCode, string businessStatus, int pageIndex, int pageSize);



        /// <summary>
        /// 根据条件获取企业信息
        /// PC
        /// </summary>
        /// <returns></returns>
        IPagedList<EnterpriseInfo> GetEnterpriseInfo(string keyValue, int pageIndex = ConstKeys.DEFAULT_PAGEINDEX, int pageSize = ConstKeys.DEFAULT_MAX_PAGESIZE);


        /// <summary>
        /// 根据条件获取企业信息
        /// </summary>
        /// <returns></returns>
        IPagedList<EnterpriseInfo> GetEnterpriseInfo(EnterpriseInfoGetRequest enterprise);

        /// <summary>
        /// 根据企业ID获取企业信息
        /// </summary>
        /// <returns></returns>
        JsonResponse GetEnterpriseInfo(Guid EnterpriseID);


        /// <summary>
        ///  修改企业信息
        /// </summary>
        /// <param name="enterprise"></param>
        /// <returns></returns>
        JsonResponse ModifyEnterpriseInfo(EnterpriseInfoRequest enterprise);


        /// <summary>
        /// 审核企业
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse ModifyEnterpriseStatus(EnterpriseInfoAuditRequest request);


        /// <summary>
        /// 添加企业信息
        /// </summary>
        /// <param name="enterprise"></param>
        /// <returns></returns>
        JsonResponse AddEnterpriseInfo(EnterpriseInfoRequest enterprise);


        /// <summary>
        /// 验证验证码是否正确，正确后注册企业帐户
        /// </summary>
        /// <param name="requestEntity"></param>
        /// <returns></returns>
        JsonResponse ValidateRegister(UserPreRegisterValidationSMSRequest requestEntity);


        /// <summary>
        /// 删除企业信息
        /// </summary>
        /// <param name="enterpriseInfoId"></param>
        /// <returns></returns>
        JsonResponse DeleteEnterpriseInfo(Guid? enterpriseInfoId);
    }
}
