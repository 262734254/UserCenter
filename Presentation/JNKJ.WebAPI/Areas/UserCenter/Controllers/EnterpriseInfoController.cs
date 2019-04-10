using Gcyg.Domain.Dto.Customers;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Services.UserCenter.Interface;
using JNKJ.WebAPI.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{
    //[ApiAuthorize]

    public class EnterpriseInfoController : BaseController
    {
        #region Fields

        private readonly IEnterpriseInfoService _enterpriseInfo;

        #endregion

        #region Ctor

        public EnterpriseInfoController(IEnterpriseInfoService enterpriseInfo)
        {
            _enterpriseInfo = enterpriseInfo;
        }

        #endregion

        /// <summary>
        /// 获取系统内所有企业列表
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        //[IsLoginAuthorize]
        [ActionName("get_all_enterpriseinfo")]
        public HttpResponseMessage get_all_enterpriseinfo(int pageIndex, int pageSize)
        {
            var result = _enterpriseInfo.GetAllEnterpriseInfo(pageIndex, pageSize);

            var list = new PageList<EnterpriseInfo>()
            {
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Data = result.ToList()
            };
            return toListJson(list, OperatingState.Success, "获取成功");
        }




        [HttpGet]
        [ActionName("get_enterpriseinfo_by_rns")]
        public HttpResponseMessage GetEnterpriseInfoByRns(bool isAdmin, Guid? subContractorId, DateTime? establishDate,
           string companyName, string organizationCode, string businessStatus, int pageIndex, int pageSize)
        {
            var result = _enterpriseInfo.GetEnterpriseInfoByRns(isAdmin, subContractorId, establishDate, companyName, organizationCode, businessStatus, pageIndex, pageSize);

            var list = new PageList<EnterpriseInfo>()
            {
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Data = result.ToList()
            };
            return toJson(list);
        }



        /// <summary>
        /// 获取系统内所有企业列表
        /// </summary>
        /// <returns></returns>
        //[IsLoginFilter]
        [HttpGet]
        [ActionName("get_all_enterpriseinfo")]
        public HttpResponseMessage get_all_enterpriseinfo()
        {
            return toJson(_enterpriseInfo.GetAllEnterpriseInfo());
        }

        /// <summary>
        /// 获取系统内所有企业列表
        /// PC
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("get_all_enterpriseinfo_pc")]
        public HttpResponseMessage get_all_enterpriseinfo_pc(string keyValue, int pageIndex, int pageSize)
        {
            var result = _enterpriseInfo.GetEnterpriseInfo(keyValue, pageIndex, pageSize);
            var list = new PageList<EnterpriseInfo>()
            {
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Data = result.ToList()
            };
            return toListJson(list, OperatingState.Success, "获取成功");
        }

        /// <summary>
        /// 根据条件获取企业信息
        /// </summary>
        /// <param name="enterprise"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("get_enterpriseinfo_by_EnterpriseInfo")]
        public HttpResponseMessage get_enterpriseinfo_by_EnterpriseInfo(EnterpriseInfoGetRequest enterprise)
        {
            var result = _enterpriseInfo.GetEnterpriseInfo(enterprise);
            var list = new PageList<EnterpriseInfo>()
            {
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Data = result.ToList()
            };
            return toListJson(list, OperatingState.Success, "获取成功");
        }

        /// <summary>
        /// 根据企业ID获取企业信息
        /// </summary>
        /// <param name="EnterpriseID">企业ID</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("get_enterpriseinfo_by_id")]
        public HttpResponseMessage get_enterpriseinfo_by_id(Guid EnterpriseID)
        {
            return toJson(_enterpriseInfo.GetEnterpriseInfo(EnterpriseID));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="enterprise"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("add_enterpriseinfo")]
        public HttpResponseMessage add_enterpriseinfo(EnterpriseInfoRequest enterprise)
        {
            return toJson(_enterpriseInfo.AddEnterpriseInfo(enterprise));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("delete_enterpriseinfo")]
        public HttpResponseMessage delete_enterpriseinfo(Guid? enterpriseId)
        {
            return toJson(_enterpriseInfo.DeleteEnterpriseInfo(enterpriseId));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="enterprise"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("modify_enterpriseinfo")]
        public HttpResponseMessage modify_enterpriseinfo(EnterpriseInfoRequest enterprise)
        {
            return toJson(_enterpriseInfo.ModifyEnterpriseInfo(enterprise));
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("modify_enterpriseinfo_status")]
        public HttpResponseMessage modify_enterpriseinfo_status(EnterpriseInfoAuditRequest request)
        {
            return toJson(_enterpriseInfo.ModifyEnterpriseStatus(request));
        }
    }
}
