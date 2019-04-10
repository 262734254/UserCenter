using System;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Services.General;

namespace JNKJ.Services.UserCenter.Interface
{
    /// <summary>
    /// 企业信息--企业信息
    /// </summary>
    public interface ISubContractor
    {
        /// <summary>
        /// Get the SubContractor paged list
        /// </summary>
        /// <param name="establishDate">成立日期 : 为空忽略</param>
        /// <param name="companyName">企业名称 : 为空忽略</param>
        /// <param name="organizationCode">组织机构编号 : 为空忽略</param>
        /// <param name="businessStatus">企业经营状态 : 为空忽略</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        IPagedList<SubContractor> GetSubContractors(bool isAdmin, Guid subContractorId, DateTime? establishDate, string companyName = null, string organizationCode = null, string businessStatus = null, int pageIndex = ConstKeys.DEFAULT_PAGEINDEX, int pageSize = ConstKeys.DEFAULT_MAX_PAGESIZE);


        /// <summary>
        ///  Get the SubContractor by id
        /// </summary>
        /// <param name="subContractorId"></param>
        /// <returns></returns>
        SubContractor GetSubContractorById(Guid subContractorId);


        /// <summary>
        /// Insert the SubContractor
        /// </summary>
        /// <param name="subContractor"></param>
        /// <returns></returns>
        bool InsertSubContractor(SubContractor subContractor);


        /// <summary>
        /// Updates the SubContractor
        /// </summary>
        /// <param name="subContractor"></param>
        /// <returns></returns>
        bool UpdateSubContractor(SubContractor subContractor);


        /// <summary>
        /// Delete the SubContractor
        /// </summary>
        /// <param name="subContractor"></param>
        /// <returns></returns>
        bool DeleteSubContractor(SubContractor subContractor);

    }
}
