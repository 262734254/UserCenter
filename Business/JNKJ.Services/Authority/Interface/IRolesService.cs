using System;
using System.Collections.Generic;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Authority;
using JNKJ.Dto.Results;

namespace JNKJ.Services.Authority.Interface
{
    public interface IRolesService
    {
        /// <summary>
        /// 获取角色信息列表
        /// </summary>
        /// <returns></returns>
        List<Roles> GetRoleList();

        /// <summary>
        /// 获取角色信息列表_后台筛选使用
        /// </summary>
        /// <returns></returns>
        IPagedList<Roles> GetRoleLists(RolesGetRequest request);


        string GetRoleNameById(Guid roleId);


        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Roles GetRoleByName(string roleName);

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="roleRequest"></param>
        /// <returns></returns>
        bool AddRoles(RolesPostRequest roleRequest);

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="roleRequest"></param>
        /// <returns></returns>
        bool ModifyRoles(RolesPostRequest roleRequest);


        /// <summary>
        /// 判断当前角色是否已经存在权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        bool GetRolesRightCount(Guid? roleId);


        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        bool DeleteRoles(Guid roleId);


        /// <summary>
        /// 根据角色ID获取角色详情和所有权限
        /// </summary>
        /// <param name="roleRequest"></param>
        /// <returns></returns>
        RolesPostResponse GetRoleAndRoleRightsByRoleId(RolesPostRequest roleRequest);


        /// <summary>
        /// 批量添加角色权限（同步添加角色的页面权限和操作权限）
        /// </summary>
        /// <param name="roleRightList"></param>
        /// <returns></returns>
        JsonResponse BatchAddRolesRights(List<RoleRightRequest> roleRightList);


        /// <summary>
        /// 修改角色权限（同步修改角色的页面权限和操作权限）
        /// </summary>
        /// <param name="roleRightList"></param>
        /// <returns></returns>
        JsonResponse ModifyRolesRights(List<RoleRightRequest> roleRightList);


        /// <summary>
        /// 添加、修改角色页面权限
        /// 用于PC,角色管理--权限设置--重新设置页面权限
        /// [Mark]ajax请求webApi,参数为集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse AddRoleRight(AddRoleRightsStrRequest request);


        /// <summary>
        /// 根据角色ID获取 页面权限
        /// PC-登录获取角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        JsonResponse GetRoleRightListByRoleId(Guid? roleId);


        /// <summary>
        /// 根据用户ID获取当前用户所属角色的一级页面权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        JsonResponse GetRoleRightsByUserId(Guid? userId,Guid? enterpriseId);

        /// <summary>
        /// 根据菜单ID获取当前菜单的子级页面权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse GetRoleRightsByMenuId(RoleRightsRequest request);

        /// <summary>
        /// 根据菜单ID获取当前菜单的操作权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse GetRoleRightButtonsByMenuId(RoleRightsRequest request);


        /// <summary>
        /// 添加、修改角色页面权限
        /// 用于PC,角色管理--权限设置--重新设置页面权限
        /// [Mark]ajax请求webApi,参数为集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool AddRoleRight(AddRightsRequest request);


        /// <summary>
        /// 添加、删除角色页面权限
        /// 用于PC,,角色管理--权限设置--设置安卓权限
        /// [Mark]ajax请求webApi,参数为集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool AddRoleRight_Android(AddRightsRequest request);


        /// <summary>
        /// 根据角色ID获取 页面权限
        /// 用于PC,角色管理--权限设置--绑定页面权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        string GetAllRoleRightsByRoleId(Guid? roleId);


        /// <summary>
        /// 根据角色ID获取 页面权限菜单列表
        /// 用于PC,角色管理--权限设置--绑定页面权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        string GetAllRoleRightsAndroid(Guid? roleId);


        /// <summary>
        /// 根据角色ID、菜单ID获取 操作权限
        /// 用于PC,角色管理--权限设置--绑定操作权限
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        string GetAllButtonRightsByMenuId(Guid? menuId, Guid? roleId);


        /// <summary>
        /// 根据角色ID、菜单ID获取 操作权限集合
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        IList<RolesRightsButtons> GetAllButtonsByMenuId(Guid? menuId, Guid? roleId);



        /// <summary>
        /// 添加、修改操作按钮权限
        /// 用于PC,角色管理--权限设置--重新设置操作权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool AddButtonRight(AddButtonRightsRequest request);

    }
}
