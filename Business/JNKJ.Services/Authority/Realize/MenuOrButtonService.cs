using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JNKJ.Core.Data;
using JNKJ.Data;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Authority;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Services.Authority.Interface;
using JNKJ.Services.General;

namespace JNKJ.Services.Authority.Realize
{
    public class MenuOrButtonService : IMenuOrButtonService
    {
        private readonly IRepository<Menu> _projectMenuRepository;
        private readonly IRepository<Buttons> _projectButtonsRepository;
        private readonly IRepository<RolesRights> _projectRolesRightsRepository;
        private readonly IRepository<RolesRightsButtons> _projectRolesRightsButtonsRepository;

        public MenuOrButtonService(IRepository<Menu> projectMenu,
            IRepository<Buttons> projectButtonsRepository,
            IRepository<RolesRights> projectRolesRightsRepository,
            IRepository<RolesRightsButtons> projectRolesRightsButtonsRepository)
        {
            _projectMenuRepository = projectMenu;
            _projectButtonsRepository = projectButtonsRepository;
            _projectRolesRightsRepository = projectRolesRightsRepository;
            _projectRolesRightsButtonsRepository = projectRolesRightsButtonsRepository;
        }

        #region PC菜单基础操作

        /// <summary>
        /// 添加操作按钮
        /// PC
        /// </summary>
        /// <returns></returns>
        public bool AddButton(ButtongRequest request)
        {
            var obj = new Buttons
            {
                Id = Guid.NewGuid(),
                CreatedTime = DateTime.Now,
                DeletedTime = null,
                DeletedState = 0,
                ButtonKey = request.ButtonKey,
                ButtonName = request.ButtonName,
                ButtonUrl = request.ButtonUrl,
                ProjectMenuID = request.ProjectMenuId
            };
            _projectButtonsRepository.PreInsert(obj);
            return _projectButtonsRepository.SaveChanges();
        }

        /// <summary>
        /// 判断按钮是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool IsExistsButton(ButtongRequest request)
        {
            return _projectButtonsRepository.Table.Any(s => s.ButtonName.Contains(request.ButtonName) && s.ButtonKey == request.ButtonKey);
        }


        /// <summary>
        /// 修改操作按钮
        /// PC
        /// </summary>
        /// <returns></returns>
        public bool ModifyButton(ButtongRequest request)
        {
            var obj = _projectButtonsRepository.GetById(request.Id);
            obj.ButtonName = request.ButtonName;
            obj.ButtonKey = request.ButtonKey;
            obj.ButtonUrl = request.ButtonUrl;
            return _projectButtonsRepository.SaveChanges();
        }

        /// <summary>
        /// 当前按钮是否正在使用
        /// </summary>
        /// <param name="buttonId"></param>
        /// <returns></returns>
        public bool IsBeingUsedButton(Guid buttonId)
        {
            return _projectRolesRightsButtonsRepository.Table.Count(s => s.ProjectButtonsID == buttonId) > 0;
        }

        /// <summary>
        /// 删除操作按钮
        /// PC
        /// </summary>
        /// <param name="buttonId"></param>
        /// <returns></returns>
        public bool DeleteButton(Guid buttonId)
        {
            var menu = _projectButtonsRepository.GetById(buttonId);
            menu.DeletedState = (int)DeletedStates.Deleted;
            menu.DeletedTime = DateTime.Now;
            return _projectButtonsRepository.SaveChanges();
        }


        /// <summary>
        /// 添加菜单
        /// PC
        /// </summary>
        /// <returns></returns>
        public bool AddMenu(MenuRequest request)
        {
            var obj = new Menu
            {
                Id = Guid.NewGuid(),
                CreatedTime = DateTime.Now,
                DeletedTime = null,
                DeletedState = 0,
                FunctionKey = request.FunctionKey,
                FunctionName = request.FunctionName,
                FunctionType = request.FunctionType,
                FunctionUrl = request.FunctionUrl,
                Icon = request.Icon,
                DisplayNo = request.DisplayNo,
                ParentID = request.ParentID
            };
            _projectMenuRepository.PreInsert(obj);
            return _projectMenuRepository.SaveChanges();
        }

        /// <summary>
        /// 判断菜单是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool IsExists(MenuRequest request)
        {
            return _projectMenuRepository.Table.Any(s => s.FunctionKey == request.FunctionKey);
            //return _projectMenuRepository.Table.Any(s => s.FunctionName.Contains(request.FunctionName) || s.FunctionKey == request.FunctionKey);
        }


        /// <summary>
        /// 修改菜单
        /// PC
        /// </summary>
        /// <returns></returns>
        public bool ModifyMenu(MenuRequest request)
        {
            var obj = _projectMenuRepository.GetById(request.Id);
            obj.FunctionName = request.FunctionName;
            obj.FunctionKey = request.FunctionKey;
            obj.FunctionUrl = request.FunctionUrl;
            obj.Icon = request.Icon;
            obj.DisplayNo = request.DisplayNo;
            return _projectMenuRepository.SaveChanges();
        }


        /// <summary>
        /// 删除菜单
        /// PC
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool DeleteMenu(Guid roleId)
        {
            var menu = _projectMenuRepository.GetById(roleId);
            //menu.DeletedState = (int)DeletedStates.Deleted;
            //menu.DeletedTime = DateTime.Now;
            //return _projectMenuRepository.SaveChanges();
            return _projectMenuRepository.Delete(menu);
        }

        /// <summary>
        /// 当前菜单是否正在使用
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsBeingUsed(Guid roleId)
        {
            return _projectRolesRightsRepository.Table.Count(s => s.ProjectMenuID == roleId) > 0;
        }

        #endregion


        /// <summary>
        /// 根据菜单类型获取所有菜单
        /// 用于PC,菜单管理--根据选择的PC/安卓筛选
        /// </summary>
        /// <returns></returns>
        public IPagedList<Menu> GetMenuListByType(int type, int pageIndex)
        {
            //获取一级菜单
            var projectMenus = _projectMenuRepository.Table.Where(s => s.DeletedState != (int)DeletedStates.Deleted && s.FunctionType == type && s.ParentID == null).OrderBy(s => s.DisplayNo);

            var query = (from a in projectMenus
                         join b in _projectMenuRepository.Table.Where(s => s.DeletedState != (int)DeletedStates.Deleted) on a.Id equals b.ParentID into subList
                         select new MenuResponse
                         {
                             Id = a.Id,
                             FunctionKey = a.FunctionKey,
                             FunctionName = a.FunctionName,
                             FunctionUrl = a.FunctionUrl,
                             FunctionType = a.FunctionType,
                             ParentID = a.ParentID,
                             CreatedTime = a.CreatedTime,
                             SubmenuList = subList
                         }).OrderBy(s => s.DisplayNo);
            return new PagedList<Menu>(query, pageIndex - 1, ConstKeys.DEFAULT_PAGESIZE);
        }



        /// <summary>
        /// 根据菜单ID获取子菜单
        /// 用于PC,菜单管理--选择菜单--获取子菜单列表
        /// </summary>
        /// <returns></returns>
        public IPagedList<Menu> GetSubMenus(Guid menuId, int pageIndex, int pageSize)
        {
            var projectMenus = _projectMenuRepository.Table.Where(c => c.ParentID == menuId && c.DeletedState != (int)DeletedStates.Deleted).OrderByDescending(s => s.CreatedTime).ToList();

            return new PagedList<Menu>(projectMenus, pageIndex - 1, pageSize);
        }


        /// <summary>
        /// 根据菜单ID获取按钮
        /// 用于PC,菜单管理--选择菜单--获取按钮列表
        /// </summary>
        /// <returns></returns>
        public IPagedList<Buttons> GetButtons(Guid menuId, int pageIndex, int pageSize)
        {
            var buttons = _projectButtonsRepository.Table.Where(c => c.ProjectMenuID == menuId && c.DeletedState != (int)DeletedStates.Deleted).OrderByDescending(s => s.CreatedTime).ToList();

            return new PagedList<Buttons>(buttons, pageIndex - 1, pageSize);
        }


        /// <summary>
        /// 根据菜单Id获取按钮集合
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public IList GetButtonByMenuId(Guid menuId)
        {
            return _projectButtonsRepository.Table.Where(c => c.ProjectMenuID == menuId && c.DeletedState != (int)DeletedStates.Deleted).ToList().Select(s => new
            {
                id = s.Id,
                label = s.ButtonName,
            }).ToList();
        }


        /// <summary>
        /// 根据菜单类型获取所有菜单
        /// 用于PC,生成菜单管理Tree
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetMenuListByType(int type)
        {
            //获取一级菜单
            var projectMenus = _projectMenuRepository.Table.Where(s => s.DeletedState != (int)DeletedStates.Deleted && s.FunctionType == type && s.ParentID == null).OrderBy(s => s.DisplayNo).ToList();

            var list = projectMenus.Select(item => new MenuAndButtonResponse()
            {
                id = item.Id,
                label = item.FunctionName,
                children = GetChildrenByType(item.Id, type).ToList()
            }).ToList();

            return JsonHelper.SerializeToJson(list);
        }
        /// <summary>
        /// 递归获取子菜单集合
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<MenuAndButton> GetChildrenByType(Guid? pId, int type)
        {
            var query = _projectMenuRepository.Table.Where(s => s.DeletedState != (int)DeletedStates.Deleted && s.FunctionType == type && s.ParentID == pId).OrderBy(s => s.DisplayNo).ToList();

            return query.Select(s => new MenuAndButton()
            {
                id = s.Id,
                label = s.FunctionName,
                children = GetChildrenByType(s.Id, type)
            });
        }
        /// <summary>
        /// 菜单Tree实体类
        /// </summary>
        public class MenuAndButton
        {
            /// <summary>
            /// 菜单Id
            /// </summary>
            public Guid id { get; set; }
            /// <summary>
            /// 菜单名称
            /// </summary>
            public string label { get; set; }
            /// <summary>
            /// 子菜单集合
            /// </summary>
            public IEnumerable<object> children { get; set; }
        }







        /// <summary>
        /// 获取所有菜单集合
        /// </summary>
        /// <returns></returns>
        public string GetMenuList(int type)
        {
            string result;
            try
            {
                //获取一级菜单
                var projectMenus = _projectMenuRepository.Table.Where(s => s.DeletedState != (int)DeletedStates.Deleted && s.FunctionType == type && s.ParentID == null).OrderBy(s => s.DisplayNo).ToList();

                var list = projectMenus.Select(item => new MenuAndButtonResponse()
                {
                    id = item.Id,
                    lv = 1,
                    parentId = item.ParentID,
                    label = item.FunctionName,
                    children = GetChildren(item.Id, type).ToList()
                }).ToList();

                result = JsonHelper.SerializeToJson(list);
            }
            catch (Exception)
            {
                return "";
            }
            return result;
        }
        /// <summary>
        /// 递归获取子菜单集合
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<MenuAndButtonResponse> GetChildren(Guid? pId, int type)
        {
            var query = _projectMenuRepository.Table.Where(s => s.DeletedState != (int)DeletedStates.Deleted && s.FunctionType == type && s.ParentID == pId).OrderBy(s => s.DisplayNo).ToList();
            return query.Select(s => new MenuAndButtonResponse()
            {
                id = s.Id,
                lv = 2,
                label = s.FunctionName,
                parentId = s.ParentID,
                children = GetChildren(s.Id, type)
            });
        }
        /// <summary>
        /// 根据tree需要的数据格式，自定义实体
        /// </summary>
        public class MenuAndButtonResponse
        {
            /// <summary>
            /// 菜单Id
            /// </summary>
            public Guid id { get; set; }
            /// <summary>
            /// 菜单名称
            /// </summary>
            public string label { get; set; }
            /// <summary>
            /// 菜单的级别
            /// </summary>
            public int lv { get; set; }
            /// <summary>
            /// 菜单父级Id
            /// </summary>
            public Guid? parentId { get; set; }
            /// <summary>
            /// 子菜单集合
            /// </summary>
            public IEnumerable<object> children { get; set; }
        }
    }
}
