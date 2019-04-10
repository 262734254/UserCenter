using System;
using System.Collections;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Authority;
using JNKJ.Dto.Results;

namespace JNKJ.Services.Authority.Interface
{
    public interface IMenuOrButtonService
    {

        /// <summary>
        /// 获取所有菜单集合
        /// 用于PC,角色管理--设置权限--生成Tree
        /// 根据type区分安卓端权限和PC端权限
        /// </summary>
        /// <returns></returns>
        string GetMenuList(int type);

        /// <summary>
        /// 根据菜单Id获取按钮集合
        /// 用于PC,点击菜单获取操作按钮
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        IList GetButtonByMenuId(Guid menuId);



        /// <summary>
        /// 根据菜单类型获取所有菜单
        /// </summary>
        /// <returns></returns>
        string GetMenuListByType(int type);


        /// <summary>
        /// 根据菜单类型获取所有菜单
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        IPagedList<Menu> GetMenuListByType(int type, int pageIndex);


        /// <summary>
        /// 根据菜单类型获取所有菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        IPagedList<Buttons> GetButtons(Guid menuId, int pageIndex, int pageSize);


        /// <summary>
        /// 根据菜单ID获取菜单所有的操作按钮
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        IPagedList<Menu> GetSubMenus(Guid menuId, int pageIndex, int pageSize);


        /// <summary>
        /// 判断菜单是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool IsExists(MenuRequest request);


        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool AddMenu(MenuRequest request);


        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool ModifyMenu(MenuRequest request);


        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        bool DeleteMenu(Guid roleId);


        /// <summary>
        /// 当前菜单是否正在使用
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        bool IsBeingUsed(Guid menuId);


        /// <summary>
        /// 添加操作按钮
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool AddButton(ButtongRequest request);

        /// <summary>
        /// 修改操作按钮
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool ModifyButton(ButtongRequest request);


        /// <summary>
        /// 删除操作按钮
        /// </summary>
        /// <param name="buttonId"></param>
        /// <returns></returns>
        bool DeleteButton(Guid buttonId);


        /// <summary>
        /// 判断按钮是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool IsExistsButton(ButtongRequest request);


        /// <summary>
        /// 当前按钮是否正在使用
        /// </summary>
        /// <param name="buttonId"></param>
        /// <returns></returns>
        bool IsBeingUsedButton(Guid buttonId);
    }
}
