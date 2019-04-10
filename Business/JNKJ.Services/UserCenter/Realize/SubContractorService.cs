using System;
using System.Linq;
using JNKJ.Core.Data;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Services.General;
using JNKJ.Services.UserCenter.Interface;
namespace JNKJ.Services.UserCenter.Realize
{
    public class SubContractorService : ISubContractor
    {
        #region Fields
        private readonly IRepository<SubContractor> _subContractorRepository;
        #endregion
        #region Ctor
        public SubContractorService(IRepository<SubContractor> subContractorRepository)
        {
            _subContractorRepository = subContractorRepository;
        }
        #endregion
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
        public IPagedList<SubContractor> GetSubContractors(bool isAdmin, Guid subContractorId, DateTime? establishDate, string companyName = null,
            string organizationCode = null, string businessStatus = null,
            int pageIndex = 0, int pageSize = 100)
        {
            if (pageIndex <= ConstKeys.DEFAULT_PAGEINDEX) { pageIndex = ConstKeys.DEFAULT_PAGEINDEX; }
            if (pageSize >= ConstKeys.DEFAULT_MAX_PAGESIZE || pageSize <= ConstKeys.ZERO_INT) { pageSize = ConstKeys.DEFAULT_PAGESIZE; }
            var query = _subContractorRepository.Table;
            if (establishDate.HasValue)
            {
                query = query.Where(c => establishDate.Value == c.EstablishDate);
            }
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                query = query.Where(c => c.CompanyName.Contains(companyName));
            }
            if (!string.IsNullOrWhiteSpace(organizationCode))
            {
                query = query.Where(c => c.OrganizationCode.Contains(organizationCode));
            }
            if (!string.IsNullOrWhiteSpace(businessStatus))
            {
                query = query.Where(c => c.BusinessStatus.Contains(businessStatus));
            }
            query = query.OrderByDescending(c => c.EstablishDate);
            var list = new PagedList<SubContractor>(query, pageIndex - 1, pageSize);
            return list;
        }
        /// <summary>
        ///  Get the SubContractor by id
        /// </summary>
        /// <param name="subContractorId"></param>
        /// <returns></returns>
        public SubContractor GetSubContractorById(Guid subContractorId)
        {
            if (Guid.Empty == subContractorId) { return null; }
            var customer = _subContractorRepository.GetById(subContractorId);
            return customer;
        }
        /// <summary>
        /// Insert the SubContractor
        /// </summary>
        /// <param name="subContractor"></param>
        /// <returns></returns>
        public bool InsertSubContractor(SubContractor subContractor)
        {
            if (subContractor == null) { throw new ArgumentNullException("subContractor is null"); }
            bool result = _subContractorRepository.Insert(subContractor);
            return result;
        }
        /// <summary>
        /// Updates the SubContractor
        /// </summary>
        /// <param name="subContractor"></param>
        /// <returns></returns>
        public bool UpdateSubContractor(SubContractor subContractor)
        {
            if (subContractor == null) { throw new ArgumentNullException("subContractor is null"); }
            bool result = _subContractorRepository.SingleUpdate(subContractor);
            return result;
        }
        /// <summary>
        /// Delete the SubContractor
        /// </summary>
        /// <param name="subContractor"></param>
        /// <returns></returns>
        public bool DeleteSubContractor(SubContractor subContractor)
        {
            if (subContractor == null) { throw new ArgumentNullException("subContractor is null"); }
            bool result = _subContractorRepository.Delete(subContractor);
            return result;
        }
    }
}
