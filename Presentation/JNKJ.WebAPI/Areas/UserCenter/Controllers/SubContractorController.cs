using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.UserCenter.Interface;
namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{
    public class SubContractorController : BaseController
    {
        #region Fields
        private readonly ISubContractor _subContractorService;
        #endregion
        #region Ctor
        public SubContractorController(ISubContractor subContractorService)
        {
            _subContractorService = subContractorService;
        }
        #endregion
        [HttpGet]
        [ActionName("get_subcontractors")]
        public HttpResponseMessage GetSubContractors([FromUri]SubContractorRequest request)
        {
            var result = _subContractorService.GetSubContractors(request.isAdmin, request.subContractorId, request.establishDate, request.companyName, request.organizationCode, request.businessStatus, request.pageIndex, request.pageSize);
            var list = new PageList<SubContractor>()
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
        [ActionName("get_subcontractor_by_id")]
        public HttpResponseMessage GetSubContractorById([FromUri]Guid subContractorId)
        {
            var result = _subContractorService.GetSubContractorById(subContractorId);
            return toJson(result, OperatingState.Success, "获取成功");
        }
        [HttpPost]
        [ActionName("delete_subcontractor")]
        public HttpResponseMessage DeleteSubContractor([FromBody]Guid subContractorId)
        {
            var obj = _subContractorService.GetSubContractorById(subContractorId);
            var result = _subContractorService.DeleteSubContractor(obj);
            return result ? toJson(null, OperatingState.Success, "删除成功") : toJson(null, OperatingState.Failure, "删除失败");
        }
        [HttpPost]
        [ActionName("insert_subcontractor")]
        public HttpResponseMessage InsertSubContractor(SubContractor subContractor)
        {
            subContractor.Id = Guid.NewGuid();
            var result = _subContractorService.InsertSubContractor(subContractor);
            return toJson(null, OperatingState.Success, result ? "添加成功" : "添加失败");
        }
        [HttpPost]
        [ActionName("update_subcontractor")]
        public HttpResponseMessage UpdateSubContractor(SubContractor subContractor)
        {
            var result = _subContractorService.UpdateSubContractor(subContractor);
            return result ? toJson(null, OperatingState.Success, "修改成功") : toJson(null, OperatingState.Failure, "修改失败");
        }
    }
}
