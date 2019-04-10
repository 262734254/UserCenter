using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Authority;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Services.Authority.Interface;
using JNKJ.WebAPI.App_Start;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.UserCenter.Interface;
using JNKJ.Services.General;

namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{
    /// <summary>
    /// 组织架构管理的控制器
    /// </summary>
    [ApiAuthorize]
    public class DeptInfoController : BaseController
    {
        private readonly IDeptInfoService _deptInfoService;
        public DeptInfoController(IDeptInfoService deptInfoService)
        {
            _deptInfoService = deptInfoService;
        }
        // GET: UserCenter/DeptInfo

        /// <summary>
        /// 添加组织架构
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_deptinfo(DeptInfoRequest request)
        {
            return toJson(_deptInfoService.AddDeptInfo(request));
        }

        /// <summary>
        /// 修改组织架构 ---- 当前只做重命名操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage modity_deptinfo(DeptInfoRequest request)
        {
            return toJson(_deptInfoService.ModityDeptInfo(request));
        }

        /// <summary>
        /// 删除组织架构
        /// </summary>
        /// <param name="Id">组织架构Id</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage delete_deptinfo([FromBody]Guid Id)
        {
            return toJson(_deptInfoService.DeleteDeptInfo(Id));
        }

        /// <summary>
        /// 根据条件获取组织架构信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_deptinfo([FromUri]GetDeptInfoRequest request)
        {
            request.pageSize = request.pageSize ?? ConstKeys.DEFAULT_PAGEINDEX; //默认继承全局每页显示的记录数，后期可以改这个参数
            request.pageIndex = request.pageIndex ?? 1; //页码，从1开始
            var model = _deptInfoService.GetDeptInfo(request);
            var list = new PageList<DeptInfoResponse>()
            {
                PageIndex = request.pageIndex.Value,
                PageSize = request.pageIndex.Value,
                TotalCount = model.TotalCount,
                TotalPages = model.TotalPages,
                Data = model.ToList()
            };
            return toListJson(list, OperatingState.Success, "获取成功");
        }

        /// <summary>
        /// 根据企业ID获取组织架构 ---- PC端
        /// </summary>
        /// <param name="EnterpriseID">企业ID</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_organization(Guid? EnterpriseID)
        {
            return toJson(_deptInfoService.GetOrganization(EnterpriseID));
        }

        /// <summary>
        /// 根据企业ID获取部门信息，下拉框的值 ---- PC端
        /// 返回部门ID和名字
        /// </summary>
        /// <param name="EnterpriseID">企业ID</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_select_deptinfo(Guid? EnterpriseID)
        {
            return toJson(_deptInfoService.GetSelectDeptInfo(EnterpriseID));
        }

    }
}