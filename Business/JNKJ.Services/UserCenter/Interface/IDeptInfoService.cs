using JNKJ.Domain;
using JNKJ.Dto.Results;
using JNKJ.Dto.UserCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Services.UserCenter.Interface
{
    /// <summary>
    /// 组织架构的接口
    /// </summary>
    public interface IDeptInfoService
    {
        /// <summary>
        /// 添加组织架构
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse AddDeptInfo(DeptInfoRequest request);

        /// <summary>
        /// 修改组织架构 ---- 当前只做重命名操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse ModityDeptInfo(DeptInfoRequest request);

        /// <summary>
        /// 删除组织架构
        /// </summary>
        /// <param name="Id">组织架构Id</param>
        /// <returns></returns>
        JsonResponse DeleteDeptInfo(Guid Id);

        /// <summary>
        /// 根据条件获取组织架构信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IPagedList<DeptInfoResponse> GetDeptInfo(GetDeptInfoRequest request);

        /// <summary>
        /// 根据企业ID获取组织架构
        /// </summary>
        /// <param name="EnterpriseID">企业ID</param>
        /// <returns></returns>
        JsonResponse GetOrganization(Guid? EnterpriseID);

        /// <summary>
        /// 根据企业ID获取部门信息，下拉框的值 ---- PC端
        /// 返回部门ID和名字
        /// </summary>
        /// <param name="EnterpriseID">企业ID</param>
        /// <returns></returns>
        JsonResponse GetSelectDeptInfo(Guid? EnterpriseID);
    }
}
