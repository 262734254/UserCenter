using JNKJ.Core;
using JNKJ.Core.Data;
using JNKJ.Core.Infrastructure;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.Qiniu;
using JNKJ.Services.UserCenter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Services.UserCenter.Realize
{
    /// <summary>
    /// 组织架构的服务
    /// </summary>
    public class DeptInfoService : IDeptInfoService
    {
        #region Fields
        private readonly IRepository<DeptInfo> _deptInfoRepository;
        private readonly IRepository<UserInfo> _userInfoRepository;
        private readonly IRepository<EnterpriseInfo> _enterpriseInfoRepository;
        private readonly IRepository<Relationship> _relationshipRepository;
        #endregion
        #region Ctor
        public DeptInfoService(
            IRepository<DeptInfo> deptInfoRepository,
           IRepository<UserInfo> userInfoRepository,
           IRepository<EnterpriseInfo> enterpriseInfoRepository,
            IRepository<Relationship> relationshipRepository)
        {
            _deptInfoRepository = deptInfoRepository;
            _userInfoRepository = userInfoRepository;
            _enterpriseInfoRepository = enterpriseInfoRepository;
            _relationshipRepository = relationshipRepository;
        }
        #endregion
        /// <summary>
        /// 添加组织架构
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse AddDeptInfo(DeptInfoRequest request)
        {
            if (string.IsNullOrEmpty(request?.Name) || request?.EnterpriseID == null)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的条件");
            }
            try
            {
                var dbEnterpriseInfo = _enterpriseInfoRepository.GetById(request?.EnterpriseID);
                if (dbEnterpriseInfo == null || dbEnterpriseInfo.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "所属企业不存在或已被删除");
                }
                if (request.Sort >= 5)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "部门最多添加到5级");
                }
                //if (_deptInfoRepository.Table.Any(t => t.DeletedState != (int)DeletedStates.Deleted && t.Name == request.Name && t.EnterpriseID == request.EnterpriseID && t.Sort == request.Sort && t.ParentID == request.ParentID))
                if (_deptInfoRepository.Table.Any(t => t.DeletedState != (int)DeletedStates.Deleted && t.Name == request.Name && t.EnterpriseID == request.EnterpriseID))
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "已存在相同部门名称，请重新输入部门名称");
                }
                var dbDeptInfo = new DeptInfo
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    EnterpriseID = request.EnterpriseID,
                    ParentID = request.ParentID,
                    Sort = request.Sort,
                    CreateUserId = request.CreateUserId,
                    CreateTime = DateTime.Now,
                    DeletedState = (int)DeletedStates.Normal,
                    DeletedTime = null
                };
                _deptInfoRepository.Insert(dbDeptInfo);
                return new JsonResponse(OperatingState.Success, "组织架构部门添加成功");
            }
            catch (Exception e)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(e.Message, e);
                return new JsonResponse(OperatingState.Failure, "添加失败 原因：" + e.Message);
            }

        }

        /// <summary>
        /// 修改组织架构 ---- 当前只做重命名操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse ModityDeptInfo(DeptInfoRequest request)
        {
            if (request?.Id == null || request?.EnterpriseID == null)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的条件");
            }
            try
            {
                var dbDeptInfo = _deptInfoRepository.GetById(request?.Id);
                if (dbDeptInfo == null || dbDeptInfo.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "不存在该组织架构信息");
                }
                //if (_deptInfoRepository.Table.Any(t => t.DeletedState != (int)DeletedStates.Deleted && t.Name == request.Name && t.EnterpriseID == request.EnterpriseID && t.Sort == request.Sort && t.ParentID == request.ParentID))
                if (_deptInfoRepository.Table.Any(t => t.DeletedState != (int)DeletedStates.Deleted && t.Name == request.Name && t.EnterpriseID == request.EnterpriseID))
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "已存在相同部门名称，请重新输入部门名称");
                }
                dbDeptInfo.Name = request.Name;
                _deptInfoRepository.Update(dbDeptInfo);
                return new JsonResponse(OperatingState.Success, "组织架构修改成功");
            }
            catch (Exception ex)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(ex.Message, ex);
                return new JsonResponse(OperatingState.Failure, "修改时出现异常 原因：" + ex.Message);
            }

        }

        /// <summary>
        /// 删除组织架构
        /// </summary>
        /// <param name="Id">组织架构Id</param>
        /// <returns></returns>
        public JsonResponse DeleteDeptInfo(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入要删除的组织架构Id");
            }
            try
            {
                var dbDeptInfo = _deptInfoRepository.GetById(Id);
                if (dbDeptInfo == null || dbDeptInfo.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "不存在该组织架构信息");
                }
                if (_deptInfoRepository.Table.Any(t => t.DeletedState != (int)DeletedStates.Deleted && t.ParentID == dbDeptInfo.Id))
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "该组织架构下存在子级组织架构信息，无法删除");
                }
                if (_relationshipRepository.Table.Any(t => t.DeletedState != (int)DeletedStates.Deleted && t.DeptInfoId == dbDeptInfo.Id))
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "该组织架构下存在用户信息信息，无法删除");
                }
                var level = dbDeptInfo.Sort + 1;
                dbDeptInfo.DeletedState = (int)DeletedStates.Deleted;
                dbDeptInfo.DeletedTime = DateTime.Now;
                _deptInfoRepository.Update(dbDeptInfo);
                return new JsonResponse(OperatingState.Success, level + "级部门删除成功");
            }
            catch (Exception ex)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(ex.Message, ex);
                return new JsonResponse(OperatingState.Failure, "修改时出现异常 原因：" + ex.Message);
            }

        }

        /// <summary>
        /// 根据条件获取组织架构信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IPagedList<DeptInfoResponse> GetDeptInfo(GetDeptInfoRequest request)
        {
            if (request == null) { return new PagedList<DeptInfoResponse>(new List<DeptInfoResponse>(), 0, request.pageSize.Value); }
            try
            {
                Expression<Func<DeptInfo, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted;
                if (request.Id != null)
                {
                    wheres = wheres.And(c => c.Id == request.Id);
                }
                if (request.EnterpriseID != null)
                {
                    var dbEnterpriseInfo = _enterpriseInfoRepository.GetById(request?.EnterpriseID);
                    if (dbEnterpriseInfo == null || dbEnterpriseInfo.DeletedState == (int)DeletedStates.Deleted)
                    {
                        return new PagedList<DeptInfoResponse>(new List<DeptInfoResponse>(), 0, request.pageSize.Value);
                    }
                    wheres = wheres.And(c => c.EnterpriseID == request.EnterpriseID);
                }
                if (request.ParentID != null)
                {
                    wheres = wheres.And(c => c.ParentID == request.ParentID);
                }
                var result = (from d in _deptInfoRepository.Table.Where(wheres)
                              join e in _enterpriseInfoRepository.Table.Where(t => t.DeletedState != (int)DeletedStates.Deleted) on d.EnterpriseID equals e.Id into eList
                              join u in _userInfoRepository.Table.Where(t => t.DeletedState != (int)DeletedStates.Deleted) on d.CreateUserId equals u.Id into uList
                              join d1 in _deptInfoRepository.Table.Where(t => t.DeletedState != (int)DeletedStates.Deleted) on d.ParentID equals d1.Id into dList
                              select new DeptInfoResponse
                              {
                                  Id = d.Id,
                                  Name = d.Name,
                                  EnterpriseID = d.EnterpriseID,
                                  EnterpriseName = eList.Any() ? eList.FirstOrDefault().Name : "",
                                  ParentID = d.ParentID,
                                  ParentName = dList.Any() ? dList.FirstOrDefault().Name : "",
                                  CreateUserId = d.CreateUserId,
                                  UserName = uList.Any() ? uList.FirstOrDefault().ChsName : "",
                                  CreateTime = d.CreateTime,
                              }).OrderByDescending(T => T.CreateTime).ToList();

                if (!string.IsNullOrWhiteSpace(request.keyWord))
                {
                    result = (from p in result
                              where (p.Name != null && p.Name.Contains(request.keyWord)) || (p.ParentName != null && p.ParentName.Contains(request.keyWord)) || (p.EnterpriseName != null && p.EnterpriseName.Contains(request.keyWord)) || (p.UserName != null && p.UserName.Contains(request.keyWord))
                              select p).ToList();
                }
                if (result.Count > 0)
                {
                    var qiniu = EngineContext.Current.Resolve<IQiniuService>();
                    result.ForEach(t => t.ImgUrl = qiniu.DownLoadPrivateUrl(t.ImgUrl));
                }
                return new PagedList<DeptInfoResponse>(result, request.pageIndex.Value - 1, request.pageSize.Value);
            }
            catch (Exception ex)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(ex.Message, ex);
                return new PagedList<DeptInfoResponse>(new List<DeptInfoResponse>(), 0, 0);
            }


        }

        /// <summary>
        /// 根据企业ID获取组织架构
        /// </summary>
        /// <param name="EnterpriseID">企业ID</param>
        /// <returns></returns>
        public JsonResponse GetOrganization(Guid? EnterpriseID)
        {
            try
            {
                if (EnterpriseID == null)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "未传入企业ID");
                }
                List<GetOrganizationResponse> result = new List<GetOrganizationResponse>();
                //根据不同参数拼接筛选条件
                Expression<Func<DeptInfo, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted && (c.ParentID == null || c.Sort == 0);

                var dbEnterpriseInfo = _enterpriseInfoRepository.GetById(EnterpriseID);
                if (dbEnterpriseInfo == null && dbEnterpriseInfo.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "当前企业不存在或已被删除");
                }
                else
                {
                    wheres = wheres.And(c => c.EnterpriseID == EnterpriseID);
                }

                result.Add(new GetOrganizationResponse()
                {
                    id = dbEnterpriseInfo.Id,
                    label = dbEnterpriseInfo.Name,
                    level = 0,
                    children = _deptInfoRepository.Table.Where(wheres).OrderByDescending(t => t.CreateTime).Select(d => new GetOrganizationResponse()
                    {
                        id = d.Id,
                        label = d.Name,
                        level = 1,
                        children = _deptInfoRepository.Table.Where(t => t.DeletedState != (int)DeletedStates.Deleted && t.ParentID == d.Id && t.Sort == 1).OrderByDescending(t => t.CreateTime)
                          .Select(d1 => new GetOrganizationResponse
                          {
                              id = d1.Id,
                              label = d1.Name,
                              level = 2,
                              children = _deptInfoRepository.Table.Where(t => t.DeletedState != (int)DeletedStates.Deleted && t.ParentID == d1.Id && t.Sort == 2).OrderByDescending(t => t.CreateTime)
                              .Select(d2 => new GetOrganizationResponse
                              {
                                  id = d2.Id,
                                  label = d2.Name,
                                  level = 3,
                                  children = _deptInfoRepository.Table.Where(t => t.DeletedState != (int)DeletedStates.Deleted && t.ParentID == d2.Id && t.Sort == 3).OrderByDescending(t => t.CreateTime)
                                  .Select(d3 => new GetOrganizationResponse
                                  {
                                      id = d3.Id,
                                      label = d3.Name,
                                      level = 4,
                                      children = _deptInfoRepository.Table.Where(t => t.DeletedState != (int)DeletedStates.Deleted && t.ParentID == d3.Id && t.Sort == 4).OrderByDescending(t => t.CreateTime)
                                      .Select(d4 => new
                                      {
                                          id = d4.Id,
                                          label = d4.Name,
                                          level = 5,
                                      })
                                  })
                              })
                          })
                    })
                });
                return new JsonResponse(OperatingState.Success, "获取组织架构信息成功", result);
            }
            catch (Exception e)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(e.Message, e);
                return new JsonResponse(OperatingState.Failure, "获取组织架构数据失败", e.Message);
            }

        }

        /// <summary>
        /// 根据企业ID获取部门信息，下拉框的值 ---- PC端
        /// 返回部门ID和名字
        /// </summary>
        /// <param name="EnterpriseID">企业ID</param>
        /// <returns></returns>
        public JsonResponse GetSelectDeptInfo(Guid? EnterpriseID)
        {
            try
            {
                if (EnterpriseID == null)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "未传入企业ID");
                }
                //根据不同参数拼接筛选条件
                Expression<Func<DeptInfo, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted;

                var dbEnterpriseInfo = _enterpriseInfoRepository.GetById(EnterpriseID);
                if (dbEnterpriseInfo == null && dbEnterpriseInfo.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "当前企业不存在或已被删除");
                }
                else
                {
                    wheres = wheres.And(c => c.EnterpriseID == EnterpriseID);
                }

                return new JsonResponse(OperatingState.Success, "获取部门信息成功", _deptInfoRepository.Table.Where(wheres).OrderByDescending(t => t.CreateTime).Select(t => new
                {
                    Id = t.Id,
                    Name = t.Name,
                    Sort = t.Sort
                }));
            }
            catch (Exception e)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(e.Message, e);
                return new JsonResponse(OperatingState.Failure, "获取部门数据失败", e.Message);
            }

        }

    }
}
