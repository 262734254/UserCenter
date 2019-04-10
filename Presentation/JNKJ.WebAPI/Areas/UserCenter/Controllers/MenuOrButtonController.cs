using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Authority;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Services.Authority.Interface;

namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{
    public class MenuOrButtonController : BaseController
    {
        #region Fields

        private readonly IMenuOrButtonService _menuOrButtonService;

        #endregion

        #region Ctor

        public MenuOrButtonController(IMenuOrButtonService menuOrButtonService)
        {
            _menuOrButtonService = menuOrButtonService;
        }

        #endregion


        /// <summary>
        /// 获取所有的菜单和按钮集合
        /// </summary>
        /// <param name="type">3=android,2=pc</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_projectmenus(int type)
        {
            return toJson(_menuOrButtonService.GetMenuList(type));
        }



        /// <summary>
        /// 根据菜单Id获取按钮集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_projectbutton_by_menuid(Guid menuId)
        {
            return toJson(_menuOrButtonService.GetButtonByMenuId(menuId));
        }


        /// <summary>
        /// 根据菜单类型获取所有菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_projectmenus_by_type(int type)
        {
            return toJson(_menuOrButtonService.GetMenuListByType(type));
        }



        /// <summary>
        /// 根据菜单ID获取子菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_project_submenus(Guid menuId, int pageIndex, int pageSize)
        {
            var result = _menuOrButtonService.GetSubMenus(menuId, pageIndex, pageSize);
            var list = new PageList<Menu>()
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
        /// 根据菜单ID获取菜单所有的操作按钮
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_project_buttons(Guid menuId, int pageIndex, int pageSize)
        {
            var result = _menuOrButtonService.GetButtons(menuId, pageIndex, pageSize);
            var list = new PageList<Buttons>()
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
        /// 根据菜单类型获取所有菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage get_projectmenus_by_type(int type, int pageIndex)
        {
            var result = _menuOrButtonService.GetMenuListByType(type, pageIndex);
            var list = new PageList<Menu>()
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
        /// 添加菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_projectmenu(MenuRequest request)
        {
            if (request == null)
            {
                return toJson(null, OperatingState.CheckDataFail, "未传入需要的条件");
            }
            //判断是否存在
            if (_menuOrButtonService.IsExists(request))
            {
                return toJson(null, OperatingState.CheckDataFail, "已经存在该菜单");
            }

            var result = _menuOrButtonService.AddMenu(request);

            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage modify_projectmenu(MenuRequest request)
        {
            if (request?.Id == null)
            {
                return toJson(null, OperatingState.CheckDataFail, "未传入需要的条件");
            }

            //判断是否存在
            if (_menuOrButtonService.IsExists(request))
            {
                return toJson(null, OperatingState.CheckDataFail, "已经存在该菜单");
            }

            var result = _menuOrButtonService.ModifyMenu(request);

            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage delete_projectmenu([FromBody]Guid menuId)
        {
            if (_menuOrButtonService.IsBeingUsed(menuId))
            {
                return toJson(null, OperatingState.CheckDataFail, "该角色已在使用中,不能删除");
            }
            var result = _menuOrButtonService.DeleteMenu(menuId);

            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }


        /// <summary>
        /// 添加操作按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_projectbutton(ButtongRequest request)
        {
            if (request == null)
            {
                return toJson(null, OperatingState.CheckDataFail, "未传入需要的条件");
            }
            //判断是否存在
            if (_menuOrButtonService.IsExistsButton(request))
            {
                return toJson(null, OperatingState.CheckDataFail, "已经存在该操作按钮");
            }

            var result = _menuOrButtonService.AddButton(request);

            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }

        /// <summary>
        /// 修改操作按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage modify_projectbutton(ButtongRequest request)
        {
            if (request?.Id == null)
            {
                return toJson(null, OperatingState.CheckDataFail, "未传入需要的条件");
            }
            var result = _menuOrButtonService.ModifyButton(request);

            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }

        /// <summary>
        /// 删除操作按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage delete_projectbutton([FromBody]Guid buttonId)
        {
            if (_menuOrButtonService.IsBeingUsedButton(buttonId))
            {
                return toJson(null, OperatingState.CheckDataFail, "该操作按钮已在使用中,不能删除");
            }
            var result = _menuOrButtonService.DeleteButton(buttonId);

            return result ? toJson(null, OperatingState.Success, "操作成功") : toJson(null, OperatingState.Failure, "操作失败");
        }
    }
}
