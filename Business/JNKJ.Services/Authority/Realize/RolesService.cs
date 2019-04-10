using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JNKJ.Core.Data;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Authority;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Services.Authority.Interface;
using JNKJ.Core;
using JNKJ.Data;

namespace JNKJ.Services.Authority.Realize
{
    public class RolesService : IRolesService
    {
        private readonly IRepository<Roles> _roles;
        private readonly IRepository<RolesRights> _rolesRights;
        private readonly IRepository<RolesRightsButtons> _rolesRightsButtons;
        private readonly IRepository<Menu> _menu;
        private readonly IRepository<Buttons> _buttons;
        private readonly IRepository<Relationship> _relationship;




        /// <summary>
        /// 声明依赖项
        /// </summary>
        public RolesService(IRepository<Roles> roles,
            IRepository<RolesRights> rolesRights,
            IRepository<RolesRightsButtons> rolesRightsButtons,
            IRepository<Buttons> buttons,
            IRepository<Menu> menu, IRepository<Relationship> relationship)
        {
            _roles = roles;
            _rolesRights = rolesRights;
            _rolesRightsButtons = rolesRightsButtons;
            _buttons = buttons;
            _menu = menu;
            _relationship = relationship;
        }


        #region Operation


        /// <summary>
        /// 批量添加角色权限（同步添加角色的页面权限和操作权限）
        /// </summary>
        /// <param name="roleRightList"></param>
        public JsonResponse BatchAddRolesRights(List<RoleRightRequest> roleRightList)
        {
            if (roleRightList == null || roleRightList.Count == 0)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的条件");
            }

            try
            {
                //用SelectMany替代二重foreach循环
                foreach (var item in roleRightList.SelectMany(menu => menu.roleRightRequestDTOList))
                {
                    RolesRights rolesRight;
                    var result = AddRoleRight(item, out rolesRight);
                    if (result)
                    {
                        AddButtons(item, rolesRight.Id);
                    }
                    else
                    {
                        return new JsonResponse(OperatingState.Failure, "数据添加失败");
                    }
                }
                return new JsonResponse(OperatingState.Success, "数据添加成功");
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Failure, "数据添加失败", e.Message);
            }
        }


        /// <summary>
        /// 批量修改角色权限（同步修改角色的页面权限和操作权限）
        /// </summary>
        /// <param name="roleRightList"></param>
        /// <returns></returns>
        public JsonResponse ModifyRolesRights(List<RoleRightRequest> roleRightList)
        {
            if (roleRightList == null || roleRightList.Count == 0)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的条件");
            }

            try
            {
                foreach (var menu in roleRightList)
                {
                    foreach (var item in menu.roleRightRequestDTOList)
                    {
                        var tranRolesRight = _rolesRights.Context.Database.BeginTransaction();//开启事务
                        try
                        {
                            //按条件删除角色权限
                            var obj = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == item.ProjectRolesID && s.ProjectMenuID == item.ProjectMenuID);
                            _rolesRights.Delete(obj);


                            RolesRights rolesRight;
                            var result = AddRoleRight(item, out rolesRight);
                            if (result)
                            {
                                //按条件删除操作权限
                                _rolesRightsButtons.DeleteList(_rolesRightsButtons.Table.Where(s => s.ProjectRolesRightsID == obj.Id).ToList());
                                AddButtons(item, rolesRight.Id);
                            }
                            else
                            {
                                tranRolesRight.Rollback();
                                return new JsonResponse(OperatingState.Failure, "数据修改失败,数据已回滚");
                            }

                            tranRolesRight.Commit();
                        }
                        catch (Exception)
                        {
                            tranRolesRight.Rollback();
                            return new JsonResponse(OperatingState.Failure, "数据修改失败,数据已回滚");
                        }

                    }
                }
                return new JsonResponse(OperatingState.Success, "数据修改成功");
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Failure, "修改时出现异常");
            }

        }


        /// <summary>
        /// 批量添加Button
        /// </summary>
        /// <param name="item"></param>
        /// <param name="roleRightId"></param>
        private void AddButtons(RoleRightRequestDTO item, Guid roleRightId)
        {
            var buttonList = new List<RolesRightsButtons>();

            //解析当前页面下的操作按钮集合
            foreach (var btn in item.buttonList)
            {
                var isExist = _rolesRightsButtons.Table.Any(s => s.ProjectRolesRightsID == roleRightId && s.ProjectButtonsID == btn.Id);
                if (!isExist)
                {
                    buttonList.Add(new RolesRightsButtons()
                    {
                        Id = Guid.NewGuid(),
                        ProjectRolesRightsID = roleRightId,
                        ProjectButtonsID = btn.Id,
                        ButtonName = btn.ButtonName,
                        ButtonKey = btn.ButtonKey,
                        CreatedTime = DateTime.Now,
                        DisplayNo = btn.DisplayNo,
                        DeletedState = 0,
                        DeletedTime = null
                    });
                }
            }
            if (buttonList.Count <= 0) return;
            //批量添加操作按钮
            _rolesRightsButtons.AddRange(buttonList);
            _rolesRightsButtons.SaveChanges();
        }



        /// <summary>
        /// 添加页面权限
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rolesRight"></param>
        /// <returns></returns>
        private bool AddRoleRight(RoleRightRequestDTO item, out RolesRights rolesRight)
        {
            bool result;

            //判断是否已经存在记录
            var obj = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == item.ProjectRolesID && s.ProjectMenuID == item.ProjectMenuID);

            if (obj != null)
            {
                rolesRight = obj;
                result = true;
            }
            else
            {
                //角色权限（页面菜单）
                rolesRight = new RolesRights
                {
                    Id = Guid.NewGuid(),
                    ProjectRolesID = item.ProjectRolesID,
                    ProjectMenuID = item.ProjectMenuID,
                    ProjectMenuParendID = item.ProjectMenuParendID,
                    CreatedTime = DateTime.Now,
                    DeletedState = 0,
                    DeletedTime = null,
                    FunctionName = item.FunctionName,
                    DisplayNo = item.DisplayNo,
                    IsDefault = item.IsDefault
                };

                //添加当前角色的页面权限
                _rolesRights.PreInsert(rolesRight);
                result = _rolesRights.SaveChanges();
            }


            return result;
        }



        /// <summary>
        /// 获取角色信息列表
        /// PC + 安卓
        /// </summary>
        /// <returns></returns>
        public List<Roles> GetRoleList()
        {
            var roleId = Guid.Parse("6F7EF19C-CBBB-466A-9744-AF5818BC4C49");//过滤系统管理员
            return _roles.Table.Where(s => s.DeletedState != (int)DeletedStates.Deleted).Where(s => s.Id != roleId).ToList();
        }



        /// <summary>
        /// 获取角色信息列表_后台筛选使用
        /// PC
        /// </summary>
        /// <returns></returns>
        public IPagedList<Roles> GetRoleLists(RolesGetRequest request)
        {

            //根据不同参数拼接筛选条件
            Expression<Func<Roles, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted;

            if (!string.IsNullOrEmpty(request.RoleName))
            {
                wheres = wheres.And(c => c.RoleName.Contains(request.RoleName.Trim()));
            }

            //根据条件获取工种
            var query = _roles.Table.Where(wheres).OrderByDescending(t => t.CreatedTime).ToList();

            return new PagedList<Roles>(query, (request.pageIndex - 1), request.pageSize);
        }

        public string GetRoleNameById(Guid roleId)
        {
            var query = _roles.GetById(roleId);
            return query != null ? query.RoleName : "";
        }


        /// <summary>
        /// 根据角色ID获取角色详情和所有权限
        /// </summary>
        /// <returns></returns>
        public RolesPostResponse GetRoleAndRoleRightsByRoleId(RolesPostRequest roleRequest)
        {
            //角色响应类
            var prpr = new RolesPostResponse();

            //获取角色详情
            var role = _roles.GetById(roleRequest.ID);
            prpr.RoleName = role.RoleName;
            prpr.Remark = role.Remark;
            prpr.CreatedTime = role.CreatedTime;

            //获取角色权限列表
            //var prrsList = _rolesRightsService.GetAllRoleRightsByRoleId(roleRequest.ID);
            //prpr.projectRoleRightList = prrsList.projectRoleRightList;

            return prpr;
        }


        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public Roles GetRoleByName(string roleName)
        {
            return _roles.Table.FirstOrDefault(s => s.RoleName == roleName && s.DeletedState != (int)DeletedStates.Deleted);
        }


        /// <summary>
        /// 添加项目角色
        /// PC
        /// </summary>
        /// <param name="roleRequest"></param>
        /// <returns></returns>
        public bool AddRoles(RolesPostRequest roleRequest)
        {
            var role = new Roles
            {
                Id = Guid.NewGuid(),
                CreatedTime = DateTime.Now,
                DeletedTime = null,
                DeletedState = 0,
                RoleName = roleRequest.RoleName,
                Remark = roleRequest.Remark
            };
            _roles.PreInsert(role);
            return _roles.SaveChanges();
        }


        /// <summary>
        /// 修改项目角色
        /// PC
        /// </summary>
        /// <param name="roleRequest"></param>
        /// <returns></returns>
        public bool ModifyRoles(RolesPostRequest roleRequest)
        {
            var dbRole = _roles.GetById(roleRequest.ID);
            dbRole.RoleName = roleRequest.RoleName;
            dbRole.Remark = roleRequest.Remark;
            return _roles.SaveChanges();
        }

        /// <summary>
        /// 判断当前角色是否已经存在权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool GetRolesRightCount(Guid? roleId)
        {
            return _rolesRights.Table.Count(s => s.ProjectRolesID == roleId) > 0;
        }

        /// <summary>
        /// 删除项目角色
        /// PC
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool DeleteRoles(Guid roleId)
        {
            var dbRole = _roles.GetById(roleId);
            dbRole.DeletedState = (int)DeletedStates.Deleted;
            dbRole.DeletedTime = DateTime.Now;
            return _roles.SaveChanges();
        }

        #endregion


        #region PC

        #region PC端设置角色权限--设置PC权限

        /// <summary>
        /// 根据角色ID、菜单ID获取 操作权限
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string GetAllButtonRightsByMenuId(Guid? menuId, Guid? roleId)
        {
            try
            {
                var query = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == roleId && s.ProjectMenuID == menuId && s.DeletedState == (int)DeletedStates.Normal);

                if (query == null) { return string.Empty; }

                var projectRolesRightsButtonses = _rolesRightsButtons.Table.Where(s => s.ProjectRolesRightsID == query.Id);

                var list = new List<object>();

                foreach (var item in projectRolesRightsButtonses)
                {
                    list.Add(item.ProjectButtonsID);
                }

                var result = JsonHelper.SerializeToJson(list);

                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// 根据角色ID、菜单ID获取 操作权限
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IList<RolesRightsButtons> GetAllButtonsByMenuId(Guid? menuId, Guid? roleId)
        {
            if (menuId == null || roleId == null) return new List<RolesRightsButtons>();

            var projectRolesRights = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == roleId && s.ProjectMenuID == menuId);

            if (projectRolesRights == null) return new List<RolesRightsButtons>();

            var buttons = _rolesRightsButtons.Table.Where(s => s.ProjectRolesRightsID == projectRolesRights.Id);

            return new List<RolesRightsButtons>(buttons);
        }



        /// <summary>
        /// 添加、修改操作按钮权限
        /// </summary>
        /// <returns></returns>
        public bool AddButtonRight(AddButtonRightsRequest request)
        {
            try
            {
                //获取当前操作的权限菜单
                var projectRolesRightse = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == request.roleId && s.ProjectMenuID == request.menuId);

                //如果存在，则删除
                var btnList = _rolesRightsButtons.Table.Where(s => s.ProjectRolesRightsID == projectRolesRightse.Id && s.ProjectButtonsID == request.btnId).ToList();
                if (btnList.Any())
                {
                    _rolesRightsButtons.DeleteList(btnList);
                    return true;
                }

                //获取当前选择的按钮
                var btnObj = _buttons.GetById(request.btnId);

                var obj = new RolesRightsButtons
                {
                    Id = Guid.NewGuid(),
                    CreatedTime = DateTime.Now,
                    DeletedTime = null,
                    DeletedState = 0,
                    ProjectRolesRightsID = projectRolesRightse.Id,
                    ProjectButtonsID = btnObj.Id,
                    ButtonName = btnObj.ButtonName,
                    ButtonKey = btnObj.ButtonKey,
                    DisplayNo = btnObj.DisplayNo
                };

                _rolesRightsButtons.Insert(obj);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }




        /// <summary>
        /// 根据角色ID获取 页面权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string GetAllRoleRightsByRoleId(Guid? roleId)
        {
            try
            {
                var query = _rolesRights.Table.Where(s => s.ProjectRolesID == roleId && s.ProjectMenuParendID != null && s.FunctionType == 2 && s.DeletedState == (int)DeletedStates.Normal).Select(s => s.ProjectMenuID);

                var result = JsonHelper.SerializeToJson(query);

                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// 添加、修改角色页面权限
        /// 用于PC,角色管理--权限设置--重新设置页面权限
        /// 停用
        /// </summary>
        /// <returns></returns>
        public JsonResponse AddRoleRight(AddRoleRightsStrRequest request)
        {
            var rolesRights = new List<RolesRights>();
            var rolesRightsButtons = new List<RolesRightsButtons>();

            try
            {
                //当前选中的菜单集合，过滤按钮集合
                foreach (var item in request.data.Where(s => s.lv != 3))
                {
                    //获取当前操作的菜单
                    var menu = _menu.GetById(item.id);
                    if (menu == null) continue;

                    //添加到角色权限表
                    var result = new RolesRights()
                    {
                        Id = Guid.NewGuid(),
                        CreatedTime = DateTime.Now,
                        DeletedTime = null,
                        DeletedState = 0,
                        IsDefault = 0,
                        ProjectRolesID = request.RoleId,
                        ProjectMenuID = menu.Id,
                        ProjectMenuParendID = menu.ParentID,
                        FunctionKey = menu.FunctionKey,
                        FunctionName = menu.FunctionName,
                        DisplayNo = menu.DisplayNo,
                        ImgUrl = menu.FunctionUrl,
                        FunctionType = menu.FunctionType,
                        Icon = menu.Icon
                    };
                    rolesRights.Add(result);

                    //同步添加当前菜单下的操作按钮（筛选当前菜单下存在的操作按钮集合）
                    rolesRightsButtons.AddRange(request.data.Where(s => s.lv == 3 && s.parentId == item.id)
                        .Select(c => new RolesRightsButtons
                        {
                            Id = Guid.NewGuid(),
                            CreatedTime = DateTime.Now,
                            DeletedTime = null,
                            DeletedState = 0,
                            ProjectRolesRightsID = result.Id,
                            ProjectButtonsID = c.id,
                            ButtonName = c.label,
                            DisplayNo = c.displayNo
                        }));
                }


                //开启事务
                var tranRolesRight = _rolesRights.Context.Database.BeginTransaction();

                try
                {
                    //优先删除该角色的菜单权限
                    var roleRights = _rolesRights.Table.Where(s => s.ProjectRolesID == request.RoleId && s.FunctionType == 2).ToList();
                    if (roleRights.Count > 0)
                    {
                        _rolesRights.DeleteList(roleRights);
                    }
                    //再删除该角色的操作权限
                    foreach (var roleRightButtons in roleRights.Select(item => _rolesRightsButtons.Table.Where(s => s.ProjectRolesRightsID == item.Id)).Where(roleRightButtons => roleRightButtons.ToList().Count > 0))
                    {
                        _rolesRightsButtons.DeleteList(roleRightButtons.ToList());
                    }

                    //再添加该角色的菜单权限
                    _rolesRights.AddRange(rolesRights);
                    //再添加该角色的操作权限
                    _rolesRightsButtons.AddRange(rolesRightsButtons);


                    _rolesRights.SaveChanges();
                    //事务提交
                    tranRolesRight.Commit();
                }
                catch (Exception)
                {
                    //事务回滚
                    tranRolesRight.Rollback();
                }

                return new JsonResponse(OperatingState.Success, "数据添加成功");
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Failure, "数据添加失败", e.Message);
            }
        }



        /// <summary>
        /// 添加、修改角色页面权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool AddRoleRight(AddRightsRequest request)
        {
            try
            {
                //获取当前操作的权限菜单
                var projectRolesRightse = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == request.roleId && s.ProjectMenuID == request.menuId);

                //如果存在，则删除
                if (projectRolesRightse != null)
                {
                    return DeleteRight(request, projectRolesRightse);
                }

                AddRight(request);

                _rolesRights.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 当权限不存在时，添加权限
        /// </summary>
        /// <param name="request"></param>
        private void AddRight(AddRightsRequest request)
        {
            //如果不存在，则添加
            var menuObj = _menu.GetById(request.menuId);

            //添加页面权限
            var obj = new RolesRights
            {
                Id = Guid.NewGuid(),
                CreatedTime = DateTime.Now,
                DeletedTime = null,
                DeletedState = 0,
                IsDefault = 0,
                ProjectRolesID = request.roleId,
                ProjectMenuID = menuObj.Id,
                FunctionKey = menuObj.FunctionKey,
                FunctionName = menuObj.FunctionName,
                ImgUrl = menuObj.FunctionUrl,
                ProjectMenuParendID = menuObj.ParentID,
                DisplayNo = menuObj.DisplayNo,
                Icon = menuObj.Icon,
                FunctionType = menuObj.FunctionType
            };
            _rolesRights.Insert(obj);

            //表示添加的是一级页面
            if (obj.ProjectMenuParendID == null)
            {
                var rights = new List<RolesRights>();
                //同步添加二级页面
                var projectMenus = _menu.Table.Where(s => s.ParentID == obj.ProjectMenuID);
                foreach (var projectMenu in projectMenus)
                {
                    rights.Add(new RolesRights()
                    {
                        Id = Guid.NewGuid(),
                        CreatedTime = DateTime.Now,
                        DeletedTime = null,
                        DeletedState = 0,
                        IsDefault = 0,
                        ProjectRolesID = request.roleId,
                        ProjectMenuID = projectMenu.Id,
                        FunctionKey = projectMenu.FunctionKey,
                        FunctionName = projectMenu.FunctionName,
                        ImgUrl = projectMenu.FunctionUrl,
                        ProjectMenuParendID = projectMenu.ParentID,
                        DisplayNo = projectMenu.DisplayNo,
                        Icon = projectMenu.Icon,
                        FunctionType = projectMenu.FunctionType
                    });
                }
                _rolesRights.AddRange(rights);
            }
            //表示添加的是二级页面
            else
            {
                //判断是否存在一级页面
                var parendObj = _rolesRights.Table.Where(s => s.ProjectRolesID == request.roleId && s.ProjectMenuID == obj.ProjectMenuParendID).ToList();
                if (parendObj.Any()) return;

                //不存在则同步添加
                var parendMenu = _menu.GetById(obj.ProjectMenuParendID);
                _rolesRights.Insert(new RolesRights
                {
                    Id = Guid.NewGuid(),
                    CreatedTime = DateTime.Now,
                    DeletedTime = null,
                    DeletedState = 0,
                    IsDefault = 0,
                    ProjectRolesID = request.roleId,
                    ProjectMenuID = parendMenu.Id,
                    FunctionKey = parendMenu.FunctionKey,
                    FunctionName = parendMenu.FunctionName,
                    ImgUrl = parendMenu.FunctionUrl,
                    ProjectMenuParendID = parendMenu.ParentID,
                    DisplayNo = parendMenu.DisplayNo,
                    Icon = parendMenu.Icon,
                    FunctionType = parendMenu.FunctionType
                });
            }
        }

        /// <summary>
        /// 当权限存在时，删除权限
        /// </summary>
        /// <param name="request"></param>
        /// <param name="projectRolesRightse"></param>
        /// <returns></returns>
        private bool DeleteRight(AddRightsRequest request, RolesRights projectRolesRightse)
        {
            //表示删除的是一级页面
            if (projectRolesRightse.ProjectMenuParendID == null)
            {
                _rolesRights.Delete(projectRolesRightse);
                //同步删除所有的二级页面
                var projectRolesRightses = _rolesRights.Table.Where(s => s.ProjectRolesID == request.roleId && s.ProjectMenuParendID == projectRolesRightse.ProjectMenuID).ToList();
                if (projectRolesRightses.Count > 0)
                {
                    _rolesRights.DeleteList(projectRolesRightses);
                    //foreach (var btnList in projectRolesRightses.Select(item => _rolesRightsButtons.Table.Where(s => s.RolesRightsID == item.Id).ToList()).Where(btnList => btnList.Any()))
                    //{
                    //    _rolesRightsButtons.DeleteList(btnList);
                    //}
                    foreach (var item in projectRolesRightses)
                    {
                        //删除所有二级页面的操作权限
                        var btnList = _rolesRightsButtons.Table.Where(s => s.ProjectRolesRightsID == item.Id).ToList();
                        if (btnList.Any())
                        {
                            _rolesRightsButtons.DeleteList(btnList);
                        }
                    }
                }
            }
            //表示删除的是二级页面
            else
            {
                //删除页面权限
                _rolesRights.Delete(projectRolesRightse);

                //删除操作权限
                var btnList = _rolesRightsButtons.Table.Where(s => s.ProjectRolesRightsID == projectRolesRightse.Id).ToList();
                if (btnList.Any())
                {
                    _rolesRightsButtons.DeleteList(btnList);
                }

                //获取同级页面
                var projectRolesRightses = _rolesRights.Table.Where(s => s.ProjectRolesID == request.roleId && s.ProjectMenuParendID == projectRolesRightse.ProjectMenuParendID).ToList();
                //如果不存在同级页面，则删除父级页面
                if (!projectRolesRightses.Any())
                {
                    var projectRolesRights = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == request.roleId && s.ProjectMenuID == projectRolesRightse.ProjectMenuParendID);
                    if (projectRolesRights != null)
                    {
                        _rolesRights.Delete(projectRolesRights);
                    }
                }
            }
            _rolesRightsButtons.SaveChanges();

            return true;
        }


        #endregion


        #region PC端设置角色权限--设置安卓权限


        /// <summary>
        /// 根据角色ID获取 页面权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public JsonResponse GetRoleRightListByRoleId(Guid? roleId)
        {
            if (roleId == null)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的条件");
            }

            try
            {
                //一级
                var query = _rolesRights.Table.Where(s => s.ProjectRolesID == roleId && s.DeletedState != (int)DeletedStates.Deleted && s.FunctionType == 2 && s.ProjectMenuParendID == null).OrderBy(s => s.DisplayNo).ToList();

                var list = new List<object>();
                foreach (var item in query)
                {
                    //递归菜单
                    list.Add(new RoleRightResponse()
                    {
                        path = "/" + item.FunctionKey,
                        icon = item.Icon,
                        title = item.FunctionName,
                        name = item.FunctionKey,
                        leaf = false,
                        access = "1",
                        component = item.ImgUrl,
                        children = GetChildren(item.ProjectMenuID, roleId).ToList()
                    });
                }
                var result = JsonHelper.SerializeToJson(list);

                return new JsonResponse(OperatingState.Success, "获取成功", result);
            }
            catch (Exception ex)
            {
                return new JsonResponse(OperatingState.Failure, "获取失败", ex.Message);
            }
        }

        /// <summary>
        /// 递归获取子菜单集合
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private IEnumerable<RoleRightChildrenResponse> GetChildren(Guid? pId, Guid? roleId)
        {
            var query = _rolesRights.Table.Where(s => s.ProjectRolesID == roleId && s.DeletedState != (int)DeletedStates.Deleted && s.FunctionType == 2 && s.ProjectMenuParendID == pId).OrderBy(s => s.DisplayNo).ToList();
            return query.Select(s => new RoleRightChildrenResponse()
            {
                path = s.FunctionKey,
                icon = s.Icon,
                title = s.FunctionName,
                name = s.FunctionKey,
                leaf = true,
                component = s.ImgUrl,
                //获取当前菜单的所有操作按钮
                meta = new { btnPermissions = _rolesRightsButtons.Table.Where(b => b.ProjectRolesRightsID == s.Id).Select(b => new { b.ButtonKey, b.ButtonName }) }
            });
        }
        /// <summary>
        /// 自定义PC左侧导航父菜单实体类
        /// </summary>
        internal class RoleRightResponse
        {
            public string path { get; set; }

            public string access { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
            public string title { get; set; }
            public string component { get; set; }

            public bool leaf { get; set; }

            public IEnumerable<RoleRightChildrenResponse> children { get; set; }
        }
        /// <summary>
        /// 自定义PC左侧导航子菜单实体类
        /// </summary>
        internal class RoleRightChildrenResponse
        {
            public string path { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
            public string title { get; set; }
            public string component { get; set; }
            public object meta { get; set; }
            public bool leaf { get; set; }

        }



        /// <summary>
        /// 根据角色ID获取 页面权限菜单列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string GetAllRoleRightsAndroid(Guid? roleId)
        {
            try
            {
                var query = _rolesRights.Table.Where(s => s.ProjectRolesID == roleId && s.FunctionType == 3 && s.DeletedState == (int)DeletedStates.Normal).ToList();

                var list = new List<Guid?>();

                foreach (var item in query)
                {
                    switch (GetLevelByMenu(item.ProjectMenuID))
                    {
                        case 1:
                        case 2:
                            if (!_rolesRights.Table.Any(s => s.ProjectMenuParendID == item.ProjectMenuID))
                            {
                                list.Add(item.ProjectMenuID);
                            }
                            break;
                        case 3:
                            list.Add(item.ProjectMenuID);
                            break;
                    }
                }

                var result = JsonHelper.SerializeToJson(list);

                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 添加、删除角色页面权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool AddRoleRight_Android(AddRightsRequest request)
        {
            try
            {
                //表示在Tree中选中了全部取消
                if (!request.Mark)
                {
                    return DeleteRight_Android(request);
                }

                AddRight_Android(request);

                _rolesRights.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 当权限存在时，删除权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool DeleteRight_Android(AddRightsRequest request)
        {
            var projectRolesRights = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == request.roleId && s.ProjectMenuID == request.menuId);
            if (projectRolesRights != null)
            {
                //删除选中项
                _rolesRights.Delete(projectRolesRights);

                //如果不存在同级页面，则同步删除父级页面
                if (!_rolesRights.Table.Any(s => s.ProjectRolesID == request.roleId && s.ProjectMenuParendID == projectRolesRights.ProjectMenuParendID))
                {
                    var parent = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == request.roleId && s.ProjectMenuID == projectRolesRights.ProjectMenuParendID);
                    if (parent != null) { _rolesRights.Delete(parent); }
                }

                //同步删除所有子集(包括子集的子集)
                var childs = GetChildrens(request.menuId, request.roleId).ToList();
                if (childs.Any()) { _rolesRights.DeleteList(childs); }

                _rolesRightsButtons.SaveChanges();
            }
            return true;
        }
        private IEnumerable<RolesRights> GetChildrens(Guid? pId, Guid? roleId)
        {
            var query = _rolesRights.Table.Where(s => s.ProjectRolesID == roleId && s.ProjectMenuParendID == pId).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetChildrens(t.ProjectMenuID, roleId)));
        }



        /// <summary>
        /// 当权限不存在时，添加权限
        /// </summary>
        /// <param name="request"></param>
        private void AddRight_Android(AddRightsRequest request)
        {
            //判断当前选中项是否已经存在，避免重复添加
            var parendObj = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == request.roleId && s.ProjectMenuID == request.menuId);
            if (parendObj == null)
            {
                var menuObj = _menu.GetById(request.menuId);
                parendObj = new RolesRights
                {
                    Id = Guid.NewGuid(),
                    CreatedTime = DateTime.Now,
                    DeletedTime = null,
                    DeletedState = 0,
                    IsDefault = 0,
                    ProjectRolesID = request.roleId,
                    ProjectMenuID = menuObj.Id,
                    FunctionKey = menuObj.FunctionKey,
                    FunctionName = menuObj.FunctionName,
                    ImgUrl = menuObj.FunctionUrl,
                    ProjectMenuParendID = menuObj.ParentID,
                    DisplayNo = menuObj.DisplayNo,
                    Icon = menuObj.Icon,
                    FunctionType = menuObj.FunctionType
                };
                _rolesRights.Insert(parendObj);
            }

            AddChildOrParend(request, parendObj.ProjectMenuParendID);
        }

        /// <summary>
        /// 同步添加子集菜单和父级菜单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pId"></param>
        private void AddChildOrParend(AddRightsRequest request, Guid? pId)
        {
            //同步添加子集菜单
            var childs = GetMenuChilds(request.menuId);
            foreach (var item in childs.Where(item => !_rolesRights.Table.Any(s => s.ProjectRolesID == request.roleId && s.ProjectMenuID == item.Id)))
            {
                _rolesRights.Insert(new RolesRights()
                {
                    Id = Guid.NewGuid(),
                    CreatedTime = DateTime.Now,
                    DeletedTime = null,
                    DeletedState = 0,
                    IsDefault = 0,
                    ProjectRolesID = request.roleId,
                    ProjectMenuID = item.Id,
                    FunctionKey = item.FunctionKey,
                    FunctionName = item.FunctionName,
                    ImgUrl = item.FunctionUrl,
                    ProjectMenuParendID = item.ParentID,
                    DisplayNo = item.DisplayNo,
                    Icon = item.Icon,
                    FunctionType = item.FunctionType
                });
            }

            //如果当前选择项不是一级菜单，则还需要同步添加父级菜单
            if (GetLevelByMenu(request.menuId) == 2 || GetLevelByMenu(request.menuId) == 3)
            {
                //同步添加父级菜单
                AddParendMenu(request, pId);
            }
        }

        /// <summary>
        /// 根据条件递归添加子集菜单
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Menu> GetMenuChilds(Guid? pId)
        {
            var query = _menu.Table.Where(s => s.ParentID == pId).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetMenuChilds(t.Id)));
        }

        /// <summary>
        /// 根据条件递归添加父级菜单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="menuId"></param>
        private void AddParendMenu(AddRightsRequest request, Guid? menuId)
        {
            //判断是否存在上级页面
            var parendObj = _rolesRights.Table.Where(s => s.ProjectRolesID == request.roleId && s.ProjectMenuID == menuId).ToList();
            if (parendObj.Any()) return;

            var parendMenu = _menu.GetById(menuId);
            if (parendMenu != null)
            {
                _rolesRights.Insert(new RolesRights
                {
                    Id = Guid.NewGuid(),
                    CreatedTime = DateTime.Now,
                    DeletedTime = null,
                    DeletedState = 0,
                    IsDefault = 0,
                    ProjectRolesID = request.roleId,
                    ProjectMenuID = parendMenu.Id,
                    FunctionKey = parendMenu.FunctionKey,
                    FunctionName = parendMenu.FunctionName,
                    ImgUrl = parendMenu.FunctionUrl,
                    ProjectMenuParendID = parendMenu.ParentID,
                    DisplayNo = parendMenu.DisplayNo,
                    Icon = parendMenu.Icon,
                    FunctionType = parendMenu.FunctionType
                });
                AddParendMenu(request, parendMenu.ParentID);
            }
        }



        /// <summary>
        /// 获取当前菜单的级别
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public int GetLevelByMenu(Guid? menuId)
        {
            var menuObj = _menu.GetById(menuId);
            if (menuObj.ParentID == null) { return 1; }

            var mObj = _menu.GetById(menuObj.ParentID);
            return mObj?.ParentID == null ? 2 : 3;
        }

        #endregion

        #endregion


        #region 安卓

        /// <summary>
        /// 根据用户ID获取当前用户所属角色的一级页面权限
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        public JsonResponse GetRoleRightsByUserId(Guid? userId, Guid? enterpriseId)
        {
            var prrResponse = new JsonResponse();

            if (userId == null)
            {
                prrResponse.State = OperatingState.CheckDataFail;
                prrResponse.Message = "未传入需要的条件";
                return prrResponse;
            }

            //var user = _userInfo.GetById(userId);
            var user = _relationship.Table.FirstOrDefault(s => s.UserId == userId && s.EnterpriseID == enterpriseId);

            if (user?.RoleId != null)
            {
                //获取一级菜单页面
                var query = _rolesRights.Table.Where(s => s.ProjectRolesID == user.RoleId
                && s.DeletedState == (int)DeletedStates.Normal && s.ProjectMenuParendID == null && s.FunctionType == 3);

                prrResponse.DataModel = query.OrderBy(s => s.DisplayNo).ToList();
            }
            else
            {
                prrResponse.DataModel = new List<RolesRights>();
            }

            prrResponse.State = OperatingState.Success;
            prrResponse.Message = "获取信息成功";
            return prrResponse;
        }


        /// <summary>
        /// 根据菜单ID获取当前菜单的子级页面权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse GetRoleRightsByMenuId(RoleRightsRequest request)
        {
            if (request == null)
            {
                return new JsonResponse { State = OperatingState.CheckDataFail, Message = "未传入需要的条件" };
            }

            //获取用户角色
            //var user = _userInfo.GetById(request.UserId);
            var user = _relationship.Table.FirstOrDefault(s => s.UserId == request.UserId && s.EnterpriseID == request.EnterpriseId);

            if (user?.RoleId == null)
            {
                return new JsonResponse { State = OperatingState.Failure, Message = "获取用户角色信息失败" };
            }

            //根据页面ID、用户角色ID获取子菜单
            var roleRights = _rolesRights.Table.Where(s => s.ProjectMenuParendID == request.MenuId && s.ProjectRolesID == user.RoleId);


            //获取按钮权限
            var buttonses = new List<RolesRightsButtons>();
            if (request.MenuId != null && user.RoleId != null)
            {
                var projectRolesRights = _rolesRights.Table.FirstOrDefault(s => s.ProjectRolesID == user.RoleId && s.ProjectMenuID == request.MenuId);
                if (projectRolesRights != null)
                {
                    buttonses = _rolesRightsButtons.Table.Where(s => s.ProjectRolesRightsID == projectRolesRights.Id).OrderBy(s => s.DisplayNo).ToList();
                }
            }

            return new JsonResponse
            {
                DataModel = roleRights.Any() ? roleRights.OrderBy(s => s.DisplayNo).ToList() : new List<RolesRights>(),
                State = OperatingState.Success,
                Message = "获取信息成功",
                //ButtonList = buttonses
            };
        }


        /// <summary>
        /// 根据菜单ID获取当前菜单的操作权限
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse GetRoleRightButtonsByMenuId(RoleRightsRequest request)
        {
            if (request == null)
            {
                return new JsonResponse() { State = OperatingState.CheckDataFail, Message = "未传入需要的条件" };
            }
            //获取用户角色
            //var user = _userInfo.GetById(request.UserId);
            var user = _relationship.Table.FirstOrDefault(s => s.UserId == request.UserId && s.EnterpriseID == request.EnterpriseId);

            if (user?.RoleId == null)
            {
                return new JsonResponse { State = OperatingState.Failure, Message = "获取用户角色信息失败" };
            }

            //获取当前选中项的页面权限ID
            var roleRights = _rolesRights.Table.FirstOrDefault(s => s.ProjectMenuID == request.MenuId && s.ProjectRolesID == user.RoleId);
            //根据页面权限ID获取操作权限集合
            var roleRightButtons = _rolesRightsButtons.Table.Where(s => s.ProjectRolesRightsID == roleRights.Id);

            return new JsonResponse
            {
                State = OperatingState.Success,
                Message = "获取信息成功",
                DataModel = roleRightButtons.Any() ? roleRightButtons.OrderBy(s => s.DisplayNo).ToList() : new List<RolesRightsButtons>()
            };
        }


        #endregion
    }
}
