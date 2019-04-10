using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Authority;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Services.Authority.Interface;
using JNKJ.Services.General;

namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{
    public class RolesController : BaseController
    {
        #region Fields

        private readonly IRolesService _rolesService;

        #endregion

        #region Ctor

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        #endregion


        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage get_role()
        {
            return toJson(_rolesService.GetRoleList(), OperatingState.Success, "获取成功");
        }


        /// <summary>
        /// 获取角色信息列表_后台筛选使用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_roles(string roleName, int pageIndex, int pageSize)
        {
            var result = _rolesService.GetRoleLists(new RolesGetRequest { RoleName = roleName, pageIndex = pageIndex, pageSize = ConstKeys.DEFAULT_PAGESIZE });

            var list = new PageList<Roles>()
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
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_rolename_by_id(Guid roleId)
        {
            return toJson(_rolesService.GetRoleNameById(roleId), OperatingState.Success, "获取成功");
        }


        /// <summary>
        /// 根据角色ID获取角色详情和角色权限
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage get_role_roleright_by_roleid(RolesPostRequest roleRequest)
        {
            return toJson(_rolesService.GetRoleAndRoleRightsByRoleId(roleRequest));
        }


        /// <summary>
        /// 添加项目的角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_role(RolesPostRequest roleRequest)
        {
            if (roleRequest == null)
            {
                return toJson(null, OperatingState.CheckDataFail, "未传入需要的条件");
            }

            //判断是否存在该角色名称
            if (_rolesService.GetRoleByName(roleRequest.RoleName) != null)
            {
                return toJson(null, OperatingState.CheckDataFail, "已经存在该角色");
            }

            var result = _rolesService.AddRoles(roleRequest);

            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }


        /// <summary>
        /// 删除项目角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage delete_role([FromBody]Guid roleId)
        {
            if (_rolesService.GetRolesRightCount(roleId))
            {
                return toJson(null, OperatingState.CheckDataFail, "该角色已在使用中,不能删除");
            }


            var result = _rolesService.DeleteRoles(roleId);

            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }


        /// <summary>
        /// 修改项目角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage edit_role(RolesPostRequest editRequest)
        {
            if (editRequest.ID == null)
            {
                return toJson(null, OperatingState.CheckDataFail, "未传入需要的条件");
            }
            //判断是否存在该角色名称
            if (_rolesService.GetRoleByName(editRequest.RoleName) != null)
            {
                return toJson(null, OperatingState.CheckDataFail, "已经存在该角色");
            }

            var result = _rolesService.ModifyRoles(editRequest);

            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }


        #region 权限相关

        /// <summary>
        /// 批量添加角色权限（同步添加角色的页面权限和操作权限）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_rolerights(List<RoleRightRequest> roleRightList)
        {
            return toJson(_rolesService.BatchAddRolesRights(roleRightList));
        }



        /// <summary>
        /// 修改项目角色权限（同步修改角色的页面权限和操作权限）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage modify_rolesRights(List<RoleRightRequest> roleRightList)
        {
            return toJson(_rolesService.ModifyRolesRights(roleRightList));
        }



        /// <summary>
        /// 添加、修改角色页面权限
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_roleright(AddRoleRightsStrRequest request)
        {
            return toJson(_rolesService.AddRoleRight(request));
        }


        /// <summary>
        /// 添加、修改角色页面权限
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_menuright(AddRightsRequest request)
        {
            var result = _rolesService.AddRoleRight(request);
            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }


        /// <summary>
        /// 添加、修改角色页面权限
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_menuright_android(AddRightsRequest request)
        {
            var result = _rolesService.AddRoleRight_Android(request);
            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }



        /// <summary>
        /// 根据角色ID获取用户权限列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_rolerightlist_by_roleid(Guid? roleId)
        {
            return toJson(_rolesService.GetRoleRightListByRoleId(roleId));
        }

        /// <summary>
        /// 根据角色ID获取 页面权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_allroleright(Guid? roleId)
        {
            return toJson(_rolesService.GetAllRoleRightsByRoleId(roleId));
        }


        /// <summary>
        /// 绑定当前角色的所有权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_allroleright_android(Guid? roleId)
        {
            return toJson(_rolesService.GetAllRoleRightsAndroid(roleId));
        }


        /// <summary>
        /// 根据角色ID、菜单ID获取 操作权限
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_allbuttonrights_by_menuid(Guid? menuId, Guid? roleId)
        {
            return toJson(_rolesService.GetAllButtonRightsByMenuId(menuId, roleId));
        }



        /// <summary>
        /// 根据角色ID、菜单ID获取 操作权限
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_buttons_by_menuid(Guid? menuId, Guid? roleId)
        {
            return toJson(_rolesService.GetAllButtonsByMenuId(menuId, roleId));
        }



        /// <summary>
        /// 根据用户ID获取当前用户所属角色的一级页面权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_rolerights_userid(Guid? userId, Guid? enterpriseId)
        {
            return toJson(_rolesService.GetRoleRightsByUserId(userId, enterpriseId));
        }


        /// <summary>
        /// 根据菜单ID获取当前菜单的子级页面权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage get_rolerights_menuid(RoleRightsRequest request)
        {
            return toJson(_rolesService.GetRoleRightsByMenuId(request));
        }


        /// <summary>
        /// 根据菜单ID获取当前菜单的操作权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage get_roleright_button_menuId(RoleRightsRequest request)
        {
            return toJson(_rolesService.GetRoleRightButtonsByMenuId(request));
        }

        /// <summary>
        /// 添加、修改操作按钮权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_buttonright(AddButtonRightsRequest request)
        {
            var result = _rolesService.AddButtonRight(request);
            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }

        #endregion
    }
}
