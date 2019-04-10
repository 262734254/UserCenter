using JNKJ.Core;
using JNKJ.Core.Data;
using JNKJ.Core.Infrastructure;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Dto.TencentIM;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.Face;
using JNKJ.Services.General;
using JNKJ.Services.Qiniu;
using JNKJ.Services.TencentIM;
using JNKJ.Services.UserCenter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace JNKJ.Services.UserCenter.Realize
{
    public class UserInfoService : IUserInfo
    {
        #region Fields
        private readonly IRepository<UserInfo> _userInfoRepository;
        private readonly IRepository<Relationship> _relationship;
        private readonly IRepository<Roles> _roleRepository;
        private readonly IRepository<UserInfoLog> _userInfoLog;
        #endregion

        #region Ctor
        public UserInfoService(IRepository<UserInfo> userInfoRepository,
            IRepository<Relationship> relationship,
             IRepository<UserInfoLog> userInfoLog,
            IRepository<Roles> roleRepository)
        {
            _userInfoRepository = userInfoRepository;
            _relationship = relationship;
            _roleRepository = roleRepository;
            _userInfoLog = userInfoLog;
        }
        #endregion

        #region UserCenter Methods
        /// <summary>
        /// 修改用户信息 --用于内部使用
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public JsonResponse UpdateUserInfo(UserInfoRequest Request)
        {
            try
            {
                var userInfo = _userInfoRepository.GetById(Request.Id);
                if (userInfo == null || userInfo.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "不存在该用户信息");
                }

                userInfo.Remark = Request.Remark == null ? userInfo.Remark : Request.Remark;
                userInfo.Sort = Request.Sort == null ? userInfo.Sort : Request.Sort;
                userInfo.PostalAddress = Request.PostalAddress == null ? userInfo.PostalAddress : Request.PostalAddress;
                userInfo.DetailAddress = Request.DetailAddress == null ? userInfo.DetailAddress : Request.DetailAddress;
                userInfo.NickName = Request.NickName == null ? userInfo.NickName : Request.NickName;
                userInfo.ChsName = Request.ChsName == null ? userInfo.ChsName : Request.ChsName;
                userInfo.ModifiedTime = DateTime.Now;
                userInfo.RolesID = Request.RolesID == null ? userInfo.RolesID : Request.RolesID; //系统管理员可以分配新用户的所属角色
                userInfo.EnterpriseInfoID = Request.EnterpriseInfoID == null ? userInfo.EnterpriseInfoID : Request.EnterpriseInfoID;
                userInfo.DefaultProjectID = Request.DefaultProjectID == null ? userInfo.DefaultProjectID : Request.DefaultProjectID;

                _userInfoRepository.Update(userInfo);
                _userInfoRepository.SaveChanges();
                //修改用户，企业，关系表
                var relationship = _relationship.Table.FirstOrDefault(t => t.UserId == Request.Id && t.DeletedState != (int)DeletedStates.Deleted);
                if (relationship != null)
                {
                    relationship.RoleId = Request.RolesID == null ? relationship.RoleId : Request.RolesID;
                    relationship.EnterpriseID = Request.EnterpriseInfoID == null ? relationship.EnterpriseID : Request.EnterpriseInfoID;
                    relationship.State = Request.State == null ? relationship.State : (int)Request.State;
                    relationship.DeptInfoId = Request.DeptInfoId == null ? relationship.DeptInfoId : Request.DeptInfoId;

                    _relationship.Update(relationship);
                    _relationship.SaveChanges();
                }

                return new JsonResponse(OperatingState.Success, "修改成功");
            }
            catch (Exception ex)
            {
                return new JsonResponse(OperatingState.Failure, "修改时出现异常！" + ex.Message);
            }
        }

        public JsonResponse GetUserInfoByPhone(string phone)
        {
            var userInfo = _userInfoRepository.Table.FirstOrDefault(s => s.Phone == phone);

            return new JsonResponse(OperatingState.Success, "操作成功", userInfo);
        }



        #endregion

        #region GCYG Methods

        /// <summary>
        /// AddEnterpriseUserInfo
        /// 添加用户
        /// </summary>
        /// <param name="userInfoRequest"></param>
        /// <returns></returns>
        public JsonResponse AddUserInfo(UserInfoRequest userInfoRequest)
        {
            var begin = _userInfoRepository.Context.Database.BeginTransaction();//开启事务
            try
            {
                if (string.IsNullOrEmpty(userInfoRequest?.Phone))//|| userInfoRequest.RolesID == null
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的条件");
                }

                var userInfo = new UserInfo()
                {
                    Id = Guid.NewGuid(),
                    DeletedState = 0,
                    DeletedTime = null,
                    ChsName = userInfoRequest.ChsName,
                    Phone = userInfoRequest.Phone,
                    UserName = userInfoRequest.Phone,
                    SubTime = DateTime.Now,
                    AccountState = (int)UserInfoState.New,
                    DetailAddress = userInfoRequest.DetailAddress,
                    IsFirstUserOnApp = false,
                    ModifiedTime = null,
                    NickName = userInfoRequest.NickName,
                    PostalAddress = userInfoRequest.PostalAddress,
                    RolesID = userInfoRequest.RolesID,//管理员添加用户时，可以选择角色
                    Sort = userInfoRequest.Sort,
                    UserImgUrl = "DefaultUserImg.jpg",   //默认头像
                    UserTypeEnum = (int)UserTypeEnum.Admin,
                    EnterpriseInfoID = userInfoRequest.EnterpriseInfoID,
                    Remark = userInfoRequest.Remark,
                    Password = userInfoRequest.Password,
                };

                var oldUser = _userInfoRepository.Table.FirstOrDefault(s => s.Phone == userInfoRequest.Phone && s.DeletedState == (int)DeletedStates.Deleted);
                if (oldUser != null)
                {
                    oldUser.DeletedState = (int)DeletedStates.Normal;
                    oldUser.DeletedTime = null;
                    return new JsonResponse(OperatingState.Success, "用户信息添加成功");
                }
                //根据手机号判断是否存在该用户
                else if (_userInfoRepository.Table.Any(s => s.Phone == userInfoRequest.Phone))
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "该手机号码已被注册");
                }
                else
                {
                    _userInfoRepository.Insert(userInfo);
                    _userInfoRepository.SaveChanges();
                }

                if (userInfoRequest.EnterpriseInfoID != null && userInfoRequest.RolesID != null)
                {
                    //添加用户，企业，角色关系
                    var relService = EngineContext.Current.Resolve<IRelationshipService>();
                    relService.AddRelationship(new RelationshipRequest
                    {
                        RoleId = userInfoRequest.RolesID,
                        EnterpriseID = userInfoRequest.EnterpriseInfoID,
                        DeptInfoId = userInfoRequest.DeptInfoId,
                        UserId = userInfo.Id,
                        State = (int)RelationshipState.Onjob,
                    });
                }
                //帐号同步到TIM
                var result = RegisterTim(userInfo);
                begin.Commit();
                return new JsonResponse(OperatingState.Success, "用户信息添加成功，审核通过后生效" + result.ErrorInfo, result);
            }
            catch (Exception e)
            {
                begin.Rollback();
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(e.Message, e);
                //return new ResponseBase(OperatingState.Failure, "添加数据失败，原因：" + e.Message);
                return new JsonResponse(OperatingState.Failure, "添加失败 原因：" + e.Message);
            }
        }


        /// <summary>
        /// ModityEnterpriseUserImg
        /// 修改用户的基本信息_安卓
        /// </summary>
        /// <param name="euRequest">修改用户的基本信息的请求</param>
        /// <returns></returns>
        public JsonResponse ModityUserImgForAndriod(UserInfoRequest Request)
        {
            if (Request?.Id == null || string.IsNullOrEmpty(Request.UserImgUrl))
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的条件");
            }
            try
            {
                var userInfo = _userInfoRepository.GetById(Request.Id);
                if (userInfo == null || userInfo.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "不存在该用户信息");
                }
                userInfo.UserImgUrl = Request.UserImgUrl;
                _userInfoRepository.SaveChanges();
                var qiniu = EngineContext.Current.Resolve<IQiniuService>();
                return new JsonResponse(OperatingState.Success, "修改成功", qiniu.DownLoadPrivateUrl(Request.UserImgUrl));
            }
            catch (Exception ex)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(ex.Message, ex);
                return new JsonResponse(OperatingState.Failure, "修改时出现异常 原因：" + ex.Message);
            }

        }

        /// <summary>
        /// ModityEnterpriseUserInfo
        /// 修改用户的基本信息
        /// PC
        /// </summary>
        /// <param name="euRequest">修改用户的基本信息的请求</param>
        /// <returns></returns>
        public JsonResponse ModityUserInfoForPC(UserInfoRequest Request)
        {
            var begin = _userInfoRepository.Context.Database.BeginTransaction();

            if (Request?.Id == null || Request?.EnterpriseInfoID == null)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的条件");
            }
            try
            {
                var userInfolog = new UserInfoLog();
                var userInfo = _userInfoRepository.GetById(Request.Id);
                if (userInfo == null || userInfo.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "不存在该用户信息");
                }
                //修改用户所属角色时，同步修改用工信息中的所属角色
                //if (Request.RolesID != userInfo.RolesID)
                //{
                //    foreach (var projectEmployment in _projectEmployment.Table.Where(s => s.RegisterUserID == euRequest.Id))
                //    {
                //        var pEmployment = _projectEmployment.GetById(projectEmployment.Id);
                //        pEmployment.ProjectRolesID = euRequest.ProjectRolesID;
                //    }
                //}
                userInfolog.UserInfo_Id = Request.Id;
                userInfolog.AccountState = (int)UserInfoState.Normal;
                userInfolog.Sort = userInfo.Sort;
                userInfolog.Remark = userInfo.Remark;
                userInfolog.PostalAddress = userInfo.PostalAddress;
                userInfolog.DetailAddress = userInfo.DetailAddress;
                userInfolog.NickName = userInfo.NickName;
                userInfolog.ChsName = userInfo.ChsName;
                userInfolog.ModifiedTime = userInfo.ModifiedTime;
                userInfolog.UserInfo_EnterpriseInfoID = userInfo.EnterpriseInfoID;
                userInfolog.UserInfo_RolesID = userInfo.RolesID;
                userInfolog.DeletedState = 0;

                userInfo.AccountState = (int)UserInfoState.Modity;
                userInfo.Remark = Request.Remark;
                userInfo.Sort = Request.Sort;
                userInfo.PostalAddress = Request.PostalAddress;
                userInfo.DetailAddress = Request.DetailAddress;
                userInfo.NickName = Request.NickName;
                userInfo.ChsName = Request.ChsName;
                userInfo.ModifiedTime = DateTime.Now;
                userInfo.RolesID = Request.RolesID; //系统管理员可以分配新用户的所属角色
                userInfo.EnterpriseInfoID = Request.EnterpriseInfoID;//系统管理员可以分配新用户的所属企业

                _userInfoRepository.SaveChanges();

                //修改用户，企业，关系表
                var relationship = _relationship.Table.FirstOrDefault(t => t.UserId == Request.Id && t.EnterpriseID == Request.EnterpriseInfoID && t.DeletedState != (int)DeletedStates.Deleted);
                if (relationship != null)
                {
                    relationship.RoleId = Request.RolesID == null ? relationship.RoleId : Request.RolesID;
                    relationship.EnterpriseID = Request.EnterpriseInfoID == null ? relationship.EnterpriseID : Request.EnterpriseInfoID;
                    relationship.State = Request.State == null ? relationship.State : (int)Request.State;
                    relationship.DeptInfoId = Request.DeptInfoId == null ? relationship.DeptInfoId : Request.DeptInfoId;

                    _relationship.SaveChanges();

                    userInfolog.Relationship_Id = relationship.Id;
                    userInfolog.Relationship_RolesID = relationship.RoleId;
                    userInfolog.Relationship_EnterpriseID = relationship.EnterpriseID;
                    userInfolog.State = relationship.State;
                    userInfolog.DeptInfoId = relationship.DeptInfoId;
                }

                //记录表是否有过修改记录
                var AnyBy = _userInfoLog.Table.Any();
                if (!AnyBy)
                {
                    _userInfoLog.Insert(userInfolog);
                }
                _userInfoLog.SaveChanges();


                begin.Commit();

                return new JsonResponse(OperatingState.Success, "修改成功");
            }
            catch (Exception ex)
            {
                begin.Rollback();
                return new JsonResponse(OperatingState.Failure, "修改时出现异常！" + ex.Message);
            }
        }

        /// <summary>
        /// AuditEnterpriseUserInfo
        /// 审核用户信息
        /// PC
        /// </summary>
        /// <param name="userInfoAuditRequest"></param>
        /// <returns></returns>
        public JsonResponse AuditUserInfo(UserInfoAuditRequest userInfoAuditRequest)
        {
            var begin = _userInfoRepository.Context.Database.BeginTransaction();
            var msg = string.Empty;
            var enterprise = _userInfoRepository.GetById(userInfoAuditRequest.UserInfoId);
            if (enterprise == null)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未发现该用户或已被删除");
            }
            try
            {
                if (userInfoAuditRequest.IsAuditPass)
                {
                    switch (enterprise.AccountState)
                    {
                        case (int)UserInfoState.New:
                            enterprise.AccountState = (int)UserInfoState.Normal;
                            msg = "审核通过，数据生效";
                            break;
                        case (int)UserInfoState.Modity:
                            enterprise.AccountState = (int)UserInfoState.Normal;
                            msg = "审核通过，新的数据已生效";
                            break;
                    }
                }
                else
                {
                    switch (enterprise.AccountState)
                    {
                        case (int)UserInfoState.New:
                            enterprise.AccountState = (int)UserInfoState.Invalid;
                            msg = "审核不通过，数据不生效";
                            break;
                        case (int)UserInfoState.Modity:
                            enterprise.AccountState = (int)UserInfoState.Normal;
                            msg = "审核不通过，数据不生效";

                            //不通过的话将之前修改的数据还原
                            var userInfoLog = _userInfoLog.Table.Where(t => t.UserInfo_Id == enterprise.Id).FirstOrDefault();
                            enterprise.Remark = userInfoLog.Remark;
                            enterprise.Sort = userInfoLog.Sort;
                            enterprise.PostalAddress = userInfoLog.PostalAddress;
                            enterprise.DetailAddress = userInfoLog.DetailAddress;
                            enterprise.NickName = userInfoLog.NickName;
                            enterprise.ChsName = userInfoLog.ChsName;
                            enterprise.ModifiedTime = userInfoLog.ModifiedTime;
                            enterprise.RolesID = userInfoLog.UserInfo_EnterpriseInfoID;
                            enterprise.EnterpriseInfoID = userInfoLog.UserInfo_RolesID;

                            var relationship = _relationship.Table.Where(t => t.UserId == enterprise.Id).FirstOrDefault();
                            if (relationship != null)
                            {
                                relationship.RoleId = userInfoLog.Relationship_RolesID;
                                relationship.EnterpriseID = userInfoLog.Relationship_EnterpriseID;
                                relationship.State = userInfoLog.State;
                                relationship.DeptInfoId = userInfoLog.DeptInfoId;

                                _relationship.SaveChanges();
                            }

                            break;
                    }
                }
                _userInfoRepository.SaveChanges();
                begin.Commit();
            }
            catch (Exception ex)
            {
                begin.Rollback();
                return new JsonResponse(OperatingState.CheckDataFail, "修改失败：" + ex.Message);
            }



            return new JsonResponse(OperatingState.Success, msg);
        }


        /// <summary>
        /// DeleteEnterpriseUserInfo
        /// 删除用户信息
        /// PC
        /// </summary>
        /// <param name="userInfoId"></param>
        /// <returns></returns>
        public JsonResponse DeleteUserInfo(Guid? userInfoId)
        {
            var enterpriseInfo = _userInfoRepository.GetById(userInfoId);
            if (enterpriseInfo == null || enterpriseInfo.DeletedState == (int)DeletedStates.Deleted)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未发现该用户或已被删除");
            }
            try
            {
                enterpriseInfo.DeletedState = (int)DeletedStates.Deleted;
                enterpriseInfo.DeletedTime = DateTime.Now;
                _userInfoRepository.Update(enterpriseInfo);

                //同步删除用户关系表中的数据
                var relService = EngineContext.Current.Resolve<IRelationshipService>();
                //request?.EnterpriseID == Guid.Empty || request?.UserId == Guid.Empty
                relService.DeleteRelationship(new RelationshipRequest()
                {
                    EnterpriseID = enterpriseInfo.EnterpriseInfoID,
                    UserId = enterpriseInfo.Id
                });

                return new JsonResponse(OperatingState.Success, "删除成功");
            }
            catch (Exception e)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(e.Message, e);
                return new JsonResponse(OperatingState.Failure, "删除时出现异常！");
            }
        }

        /// <summary>
        /// EnterpriseUserCertificationUpdate
        /// 用户实名认证
        /// </summary>
        /// <param name="eucRequest">用户实名认证请求类</param>
        /// <returns></returns>
        public JsonResponse UserInfoCertification(UserInfoCertificationRequest eucRequest)
        {
            if (eucRequest?.UserID == null)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入必要的数据");
            }
            var userInfo = _userInfoRepository.GetById(eucRequest.UserID);
            if (userInfo == null || userInfo.DeletedState == (int)DeletedStates.Deleted)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "不存在该用户或已被删除");
            }

            userInfo.FrontUserIdCardImgUrl = eucRequest.FrontUserIdCardImgUrl;
            userInfo.BehindUserIdCardImgUrl = eucRequest.BehindUserIdCardImgUrl;
            userInfo.IdCardAddress = eucRequest.IdCardAddress;
            userInfo.IdCardFullName = eucRequest.IdCardFullName;
            userInfo.IdCardNo = eucRequest.IdCardNo;
            userInfo.Sex = eucRequest.Sex;
            userInfo.AccountState = (int)UserInfoState.RealNameAuthenticationed;
            userInfo.ModifiedTime = DateTime.Now;

            //同步所有的名称
            userInfo.NickName = eucRequest.IdCardFullName;
            userInfo.ChsName = eucRequest.IdCardFullName;

            try
            {
                _userInfoRepository.SaveChanges();

                //修改TIM昵称
                var tim = EngineContext.Current.Resolve<ITencentImService>();
                var list = new List<object>
                {
                   new { Tag = "Tag_Profile_IM_Nick",Value=eucRequest.IdCardFullName }
                };
                tim.SetPortrait(new SetPortraitRequest
                {
                    From_Account = userInfo.Phone,
                    ProfileItem = list
                });

                //将身份证信息同步注册到人脸库
                var face = EngineContext.Current.Resolve<IFaceIdService>();//IFaceIDService
                var qiniu = EngineContext.Current.Resolve<IQiniuService>();
                var result = face.UserAdd(new FaceRequest() { userId = userInfo.Phone, image = qiniu.DownLoadPrivateUrl(eucRequest.FrontUserIdCardImgUrl) });
                if (result.State == OperatingState.Success)
                {
                    return new JsonResponse(OperatingState.Success, "实名认证成功", userInfo);
                }
                else
                {
                    return new JsonResponse(OperatingState.Failure, result.Message);
                }

            }
            catch (Exception ex)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(ex.Message, ex);
                return new JsonResponse(OperatingState.Failure, "保存时出现异常 原因：" + ex.Message);
            }
        }

        /// <summary>
        /// GetEnterpriseUserInfos
        /// 获取用户的基本信息
        /// PC
        /// </summary>
        /// <param name="EnterpriseID">企业ID</param>
        /// <param name="Type">类型Id 1=选择项目负责人（角色不为“游客”和“注册工匠”）</param>
        /// <returns></returns>
        public List<UserCenterInfoResponse> GetUserInfosForPC(Guid? EnterpriseID, int? Type)
        {
            try
            {
                //Expression<Func<UserInfo>,bool>> where =
                Expression<Func<UserInfo, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted;
                if (EnterpriseID == null)
                {
                    return null;
                }
                if (Type != null && Type == 1)
                {
                    //选择项目负责人（角色不为“游客”和“注册工匠”）

                    //“游客”角色的ID
                    var tourist = Guid.Parse("C4147198-7785-4F54-8B26-1671DA15C876");
                    //“注册工匠”角色的ID
                    var RegisteredCraftsmen = Guid.Parse("63C43B08-B107-48F8-94B3-D5C20293C7C0");
                    //“乙方”角色的ID
                    var yifang = Guid.Parse("BE3ABA61-5456-4CBD-B6B6-B839501969AE");

                    wheres = wheres.And(c => c.EnterpriseInfoID == EnterpriseID && c.RolesID != tourist && c.RolesID != RegisteredCraftsmen && c.RolesID != yifang);
                }
                var query = _userInfoRepository.Table.Where(wheres).ToList();
                var qiniu = EngineContext.Current.Resolve<IQiniuService>();

                JavaScriptSerializer SerializerResult = new JavaScriptSerializer();
                var userInfo = SerializerResult.Deserialize<List<UserCenterInfoResponse>>(JsonConvert.SerializeObject(query));
                List<UserCenterInfoResponse> newUserInfo = new List<UserCenterInfoResponse>();
                foreach (var item in userInfo)
                {
                    var rel = _relationship.Table.FirstOrDefault(t => t.UserId == item.Id && t.EnterpriseID == item.EnterpriseInfoID && t.DeletedState != (int)DeletedStates.Deleted);
                    if (rel != null)
                    {
                        item.DeptInfoId = rel.DeptInfoId;
                        item.State = rel.State;
                        item.RolesID = item.RolesID == null ? rel.RoleId : item.RolesID;
                    }
                    item.RolesName = _roleRepository.GetById(item.RolesID) == null ? null : _roleRepository.GetById(item.RolesID).RoleName;
                    item.UserImgUrl = qiniu.DownLoadPrivateUrl(item.UserImgUrl);
                    item.BehindUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(item.BehindUserIdCardImgUrl);
                    item.FrontUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(item.FrontUserIdCardImgUrl);
                    newUserInfo.Add(item);
                }
                return newUserInfo;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 获取没有用工登记的用户信息 --用于工程易管系统查询
        /// </summary>
        /// <param name="Ids">用户Id集合</param>
        /// <returns></returns>
        public List<UserCenterInfoResponse> GetUserInfosForEmployment(UserInfosForEmploymentRequest request)
        {
            try
            {
                Expression<Func<UserInfo, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted;
                if (request.Ids == null || request.Ids.Length <= 0)
                {
                    return null;
                }
                string sqlWhere = "where Id not in (" + request.Ids + ")";
                if (request.EnterpriseInfoID != Guid.Empty)
                {
                    sqlWhere = sqlWhere + " and EnterpriseInfoID='" + request.EnterpriseInfoID + "'";
                }
                //查询除ids 外的所有用户
                var query = _userInfoRepository.Context.Database.SqlQuery<UserInfo>(
                 "select * from  [dbo].[UserInfo] " + sqlWhere + " order by SubTime desc").ToList();

                //var query = _userInfoRepository.Context.Database.SqlQuery<UserInfo>(
                //"select * from  [dbo].[UserInfo] where Id not in (" + Ids + ") order by SubTime desc").ToList();

                List<UserCenterInfoResponse> newUserInfo = new List<UserCenterInfoResponse>();
                var qiniu = EngineContext.Current.Resolve<IQiniuService>();
                if (query.ToList().Count > 0)
                {

                    JavaScriptSerializer SerializerResult = new JavaScriptSerializer();
                    var userInfo = SerializerResult.Deserialize<List<UserCenterInfoResponse>>(JsonConvert.SerializeObject(query));

                    foreach (var item in userInfo)
                    {
                        var rel = _relationship.Table.FirstOrDefault(t => t.UserId == item.Id && t.EnterpriseID == item.EnterpriseInfoID && t.DeletedState != (int)DeletedStates.Deleted);
                        if (rel != null)
                        {
                            item.DeptInfoId = rel.DeptInfoId;
                            item.State = rel.State;
                            item.RolesID = item.RolesID == null ? rel.RoleId : item.RolesID;
                        }
                        item.RolesName = _roleRepository.GetById(item.RolesID) == null ? null : _roleRepository.GetById(item.RolesID).RoleName;
                        item.UserImgUrl = qiniu.DownLoadPrivateUrl(item.UserImgUrl);
                        item.BehindUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(item.BehindUserIdCardImgUrl);
                        item.FrontUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(item.FrontUserIdCardImgUrl);
                        newUserInfo.Add(item);
                    }
                }
                return newUserInfo;
            }
            catch (Exception)
            {
                return null;
            }

        }
        /// <summary>
        /// GetEnterpriseUserInfo
        /// 获取用户基本信息
        /// </summary>
        /// <returns></returns>
        public IPagedList<UserCenterInfoResponse> GetUserInfo(UserInfoGetRequest eugRequest)
        {
            try
            {
                if (eugRequest == null)
                {
                    return new PagedList<UserCenterInfoResponse>(new List<UserCenterInfoResponse>(), 0, ConstKeys.DEFAULT_PAGESIZE);
                }
                if (eugRequest.pageIndex == null || eugRequest.pageIndex <= ConstKeys.DEFAULT_PAGEINDEX) { eugRequest.pageIndex = ConstKeys.DEFAULT_PAGEINDEX; }

                //根据不同参数拼接筛选条件
                Expression<Func<UserInfo, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted;
                if (!string.IsNullOrEmpty(eugRequest.ChsName))
                {
                    wheres = wheres.And(c => c.ChsName.Contains(eugRequest.ChsName));
                }
                if (!string.IsNullOrEmpty(eugRequest.Phone))
                {
                    wheres = wheres.And(c => c.Phone.Contains(eugRequest.Phone));
                }
                if (eugRequest.AccountState != (int)UserInfoState.None)
                {
                    wheres = wheres.And(c => c.AccountState == eugRequest.AccountState);
                }
                if (eugRequest.SubStartTime != null && eugRequest.SubEndTime != null)
                {
                    var start = eugRequest.SubStartTime;
                    var end = Convert.ToDateTime(eugRequest.SubEndTime).AddDays(1).AddSeconds(-1);
                    wheres = wheres.And(c => c.SubTime > start && c.SubTime < end);
                }
                var query = _userInfoRepository.Table.Where(wheres).OrderByDescending(s => s.SubTime);

                var qiniu = EngineContext.Current.Resolve<IQiniuService>();

                JavaScriptSerializer SerializerResult = new JavaScriptSerializer();
                var userInfo = SerializerResult.Deserialize<List<UserCenterInfoResponse>>(JsonConvert.SerializeObject(query));
                List<UserCenterInfoResponse> newUserInfo = new List<UserCenterInfoResponse>();

                foreach (var item in userInfo)
                {
                    var rel = _relationship.Table.FirstOrDefault(t => t.UserId == item.Id && t.EnterpriseID == item.EnterpriseInfoID && t.DeletedState != (int)DeletedStates.Deleted);
                    if (rel != null)
                    {

                        item.DeptInfoId = rel.DeptInfoId;
                        item.State = rel.State;
                        item.RolesID = item.RolesID == null ? rel.RoleId : item.RolesID;
                    }
                    item.RolesName = _roleRepository.GetById(item.RolesID) == null ? null : _roleRepository.GetById(item.RolesID).RoleName;
                    item.UserImgUrl = qiniu.DownLoadPrivateUrl(item.UserImgUrl);
                    item.BehindUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(item.BehindUserIdCardImgUrl);
                    item.FrontUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(item.FrontUserIdCardImgUrl);
                    newUserInfo.Add(item);
                }
                //query.ToList().ForEach(s => s.UserImgUrl = s.UserImgUrl == null ? null : qiniu.DownLoadPrivateUrl(s.UserImgUrl));
                //query.ToList().ForEach(s => s.BehindUserIdCardImgUrl = s.BehindUserIdCardImgUrl == null ? null : qiniu.DownLoadPrivateUrl(s.BehindUserIdCardImgUrl));
                //query.ToList().ForEach(s => s.FrontUserIdCardImgUrl = s.FrontUserIdCardImgUrl == null ? null : qiniu.DownLoadPrivateUrl(s.FrontUserIdCardImgUrl));

                return new PagedList<UserCenterInfoResponse>(newUserInfo.ToList(), eugRequest.pageIndex.Value - 1, ConstKeys.DEFAULT_PAGESIZE);
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// GetUserInfos
        /// 获取用户基本信息
        /// PC
        /// </summary>
        /// <returns></returns>
        public IPagedList<UserCenterInfoResponse> GetUserInfos(UserInfoGetRequest request)
        {
            if (request.pageIndex == null || request.pageIndex <= ConstKeys.DEFAULT_PAGEINDEX) { request.pageIndex = ConstKeys.DEFAULT_PAGEINDEX; }

            //根据不同参数拼接筛选条件
            Expression<Func<UserInfo, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted;

            Expression<Func<UserCenterInfoResponse, bool>> where1 = c => c.DeletedState != (int)DeletedStates.Deleted;
            if (!string.IsNullOrEmpty(request.keyword))
            {
                wheres = wheres.And(c => c.ChsName.Contains(request.keyword) || c.Phone.Contains(request.keyword) || c.ChsName.Contains(request.keyword) || c.NickName.Contains(request.keyword) || c.UserName.Contains(request.keyword));
            }

            //匹配没有企业Id的用户
            if (!string.IsNullOrWhiteSpace(request.selectNoEnID.ToString()) && request.selectNoEnID)
            {
                wheres = wheres.And(c => c.EnterpriseInfoID == null);
            }

            //区分是否为 审核页面
            if (!string.IsNullOrWhiteSpace(request.accountType) && request.accountType.Equals("Audit"))
            {
                if (request.selectEnterpriseInfo != null)
                {
                    wheres = wheres.And(c => c.EnterpriseInfoID == request.selectEnterpriseInfo);
                }
                //查询新增待审核、修改待审核的数据
                wheres = wheres.And(c => c.AccountState == (int)UserInfoState.New || c.AccountState == (int)UserInfoState.Modity
                 || c.AccountState == (int)UserInfoState.NameAuthenticationedApproving);
            }
            else
            {
                if (request.selectValue != null)
                {
                    wheres = wheres.And(c => c.AccountState == request.selectValue);
                }

                if (request.selectEnterpriseInfo != null)
                {
                    wheres = wheres.And(c => c.EnterpriseInfoID == request.selectEnterpriseInfo);
                }

                if (request.selectRoleId != null)
                {
                    wheres = wheres.And(c => c.RolesID == request.selectRoleId);
                    //where1 = where1.And(c => c.RolesID == request.selectRoleId);
                }
            }

            var query = _userInfoRepository.Table.Where(wheres).OrderByDescending(s => s.SubTime);

            JavaScriptSerializer SerializerResult = new JavaScriptSerializer();
            var userInfo = SerializerResult.Deserialize<List<UserCenterInfoResponse>>(JsonConvert.SerializeObject(query));
            var qiniu = EngineContext.Current.Resolve<IQiniuService>();
            List<UserCenterInfoResponse> userList = new List<UserCenterInfoResponse>();
            foreach (var item in userInfo)
            {
                var rel = _relationship.Table.FirstOrDefault(t => t.UserId == item.Id && t.EnterpriseID == item.EnterpriseInfoID && t.DeletedState != (int)DeletedStates.Deleted);
                if (rel != null)
                {

                    item.DeptInfoId = rel.DeptInfoId;
                    item.State = rel.State;
                    item.RolesID = item.RolesID == null ? rel.RoleId : item.RolesID;
                }
                item.RolesName = _roleRepository.GetById(item.RolesID) == null ? null : _roleRepository.GetById(item.RolesID).RoleName;
                item.UserImgUrl = qiniu.DownLoadPrivateUrl(item.UserImgUrl);
                item.BehindUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(item.BehindUserIdCardImgUrl);
                item.FrontUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(item.FrontUserIdCardImgUrl);
                userList.Add(item);
            }

            if (request.deptInfoId != Guid.Empty && request.deptInfoId != null)
            {
                userList = userList.ToList().Where(t => t.DeptInfoId == request.deptInfoId).ToList();
            }
            if (request.state != null)
            {
                userList = userList.ToList().Where(t => t.State == request.state).ToList();
            }

            var roleId = Guid.Parse("6F7EF19C-CBBB-466A-9744-AF5818BC4C49");//过滤系统管理员
            return new PagedList<UserCenterInfoResponse>(userList.Where(s => s.RolesID != roleId).ToList(), request.pageIndex.Value - 1, ConstKeys.DEFAULT_PAGESIZE);
        }

        /// <summary>
        /// GetEnterpriseUserInfo
        /// 通过用户ID获取用户的基本信息  
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public JsonResponse GetUserInfo(Guid UserId)
        {
            if (UserId == null)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入必要的参数");
            }
            var user = _userInfoRepository.GetById(UserId);
            if (user == null || user.DeletedState == (int)DeletedStates.Deleted)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "该用户为空或已被删除");
            }
            var qiniu = EngineContext.Current.Resolve<IQiniuService>();
            try
            {
                JavaScriptSerializer SerializerResult = new JavaScriptSerializer();
                var userInfo = SerializerResult.Deserialize<UserCenterInfoResponse>(JsonConvert.SerializeObject(user));
                userInfo.UserImgUrl = qiniu.DownLoadPrivateUrl(user.UserImgUrl);
                userInfo.BehindUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(user.BehindUserIdCardImgUrl);
                userInfo.FrontUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(user.FrontUserIdCardImgUrl);

                var rel = _relationship.Table.FirstOrDefault(t => t.UserId == user.Id && t.EnterpriseID == user.EnterpriseInfoID && t.DeletedState != (int)DeletedStates.Deleted);
                if (rel != null)
                {
                    userInfo.DeptInfoId = rel.DeptInfoId;
                    userInfo.State = rel.State;
                    userInfo.RolesID = userInfo.RolesID == null ? rel.RoleId : userInfo.RolesID;
                    userInfo.RolesName = _roleRepository.GetById(userInfo.RolesID) != null ? _roleRepository.GetById(userInfo.RolesID).RoleName : "";
                }
                return new JsonResponse(OperatingState.Success, "获取成功", userInfo);
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "获取时出现异常 " + e.Message);
            }
        }

        /// <summary>
        /// GetEnterpriseUserInfo
        /// 通过用户ID和企业ID获取用户的基本信息 
        /// 企业ID用于查询《用户-企业-角色关系表》中的指定企业下用户角色
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="EnterpriseID"></param>
        /// <returns></returns>
        public JsonResponse GetUserInfoByEnId(Guid UserId, Guid? EnterpriseID)
        {
            if (UserId == Guid.Empty)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入必要的参数");
            }
            var user = _userInfoRepository.GetById(UserId);
            if (user == null || user.DeletedState == (int)DeletedStates.Deleted)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "该用户为空或已被删除");
            }
            var qiniu = EngineContext.Current.Resolve<IQiniuService>();
            try
            {
                JavaScriptSerializer SerializerResult = new JavaScriptSerializer();
                var userInfo = SerializerResult.Deserialize<UserCenterInfoResponse>(JsonConvert.SerializeObject(user));
                userInfo.UserImgUrl = qiniu.DownLoadPrivateUrl(user.UserImgUrl);
                userInfo.BehindUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(user.BehindUserIdCardImgUrl);
                userInfo.FrontUserIdCardImgUrl = qiniu.DownLoadPrivateUrl(user.FrontUserIdCardImgUrl);

                var enID = EnterpriseID == null || EnterpriseID == Guid.Empty ? userInfo.EnterpriseInfoID : EnterpriseID;
                var rel = _relationship.Table.FirstOrDefault(t => t.UserId == user.Id && t.EnterpriseID == enID && t.DeletedState != (int)DeletedStates.Deleted);
                if (rel != null)
                {
                    userInfo.DeptInfoId = rel.DeptInfoId;
                    userInfo.EnterpriseInfoID = enID;
                    userInfo.State = rel.State;
                    userInfo.RolesID = userInfo.RolesID == null ? rel.RoleId : userInfo.RolesID;
                    userInfo.RolesName = _roleRepository.GetById(userInfo.RolesID) != null ? _roleRepository.GetById(userInfo.RolesID).RoleName : "";
                }
                return new JsonResponse(OperatingState.Success, "获取成功", userInfo);
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "获取时出现异常 " + e.Message);
            }
        }


        /// <summary>
        /// 导入帐号到TIM
        /// </summary>
        /// <param name="userInfoList"></param>
        /// <returns></returns>
        public JsonResponse ImportUserInfo(List<AccountImportRequest> userInfoList)
        {
            try
            {
                //帐号同步到TIM
                var tim = EngineContext.Current.Resolve<ITencentImService>();
                if (userInfoList == null || userInfoList.Count <= 0) return new JsonResponse(OperatingState.Failure, "导入帐号失败 传入的数据为空");

                var sb = new StringBuilder();

                foreach (var obj in userInfoList.Select(userInfo => tim.AccountImport(userInfo)))
                {
                    sb.Append(obj.Identifier + "：" + (obj.ErrorInfo == "" ? "导入完成" : obj.ErrorInfo) + "<br />");
                }
                return new JsonResponse(OperatingState.Success, "导入帐号完成", sb.ToString());
            }
            catch (Exception e)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(e.Message, e);
                return new JsonResponse(OperatingState.Failure, "添加数据失败 失败原因：" + e.Message);
            }
        }

        /// <summary>
        /// 同步到人脸库
        /// </summary>
        /// <param name="userInfoList"></param>
        /// <returns></returns>
        public JsonResponse SyncFaceUserInfo(List<FaceRequest> userInfoList)
        {
            try
            {
                if (userInfoList == null || userInfoList.Count <= 0) return new JsonResponse(OperatingState.Failure, "导入帐号失败 传入的数据为空");
                //将身份证信息同步注册到人脸库
                var face = EngineContext.Current.Resolve<IFaceIdService>();

                var sb = new StringBuilder();

                foreach (var obj in userInfoList.Select(userInfo => face.UserAdd(userInfo)))
                {
                    sb.Append(obj + "<br />");
                }
                return new JsonResponse(OperatingState.Success, "导入帐号完成", sb.ToString());
            }
            catch (Exception e)
            {
                //var logger = EngineContext.Current.Resolve<ILogger>();
                //logger.Error(e.Message, e);
                return new JsonResponse(OperatingState.Failure, "导入帐号失败", e.Message);
            }
        }


        /// <summary>
        /// 设置用户的播放语音开关
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isPlay"></param>
        /// <returns></returns>
        public JsonResponse SetIsPlay(Guid userId, string isPlay)
        {
            var userInfo = _userInfoRepository.GetById(userId);

            userInfo.IsPlay = isPlay == "1";

            _userInfoRepository.SaveChanges();

            return new JsonResponse(OperatingState.Success, "设置成功");
        }

        #endregion

        #region Credit Methods

        public JsonResponse UpUserImage(UserInfoRequest request)
        {

            if (request.Id == null)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入参数！");
            }

            var userInfo = _userInfoRepository.GetById(request.Id);
            if (userInfo != null)
            {
                userInfo.UserImgUrl = request.UserImgUrl == null ? userInfo.UserImgUrl : request.UserImgUrl;
                userInfo.NickName = request.NickName == null ? userInfo.NickName : request.NickName;
                _userInfoRepository.Update(userInfo);
                return new JsonResponse(OperatingState.Success, "修改成功");
            }
            else
            {
                return new JsonResponse(OperatingState.Failure, "未找到当前用户的相关信息！");
            }

        }

        public UserInfo GetUserInfoForCredit(UserInfoRequest request)
        {

            if (request.UserName == null) { return null; }
            try
            {
                var userInfo = _userInfoRepository.Table.FirstOrDefault(t => t.UserName == request.UserName);
                if (userInfo != null)
                {
                    return userInfo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }


        #endregion


        #region private
        private RequestResult RegisterTim(UserInfo enteUserEntity)
        {
            try
            {
                //帐号同步到TIM
                var tim = EngineContext.Current.Resolve<ITencentImService>();
                //同步帐号到TIM,更新用户信息UserSig
                var obj = tim.AccountImport(new AccountImportRequest()
                {
                    FaceUrl = string.IsNullOrEmpty(enteUserEntity.UserImgUrl) ? "" : enteUserEntity.UserImgUrl,
                    Identifier = enteUserEntity.Phone,
                    Nick = string.IsNullOrEmpty(enteUserEntity.ChsName) ? "" : enteUserEntity.ChsName,
                    Type = 0
                });
                if (obj.ActionStatus.Equals("OK"))
                {
                    obj.ErrorInfo = "，同步腾讯IM帐号成功";

                    var userInfo = _userInfoRepository.GetById(enteUserEntity.Id);
                    userInfo.UserSig = obj.UserSig;
                    _userInfoRepository.SaveChanges();
                }
                else
                {
                    obj.ErrorInfo = "，同步腾讯IM帐号失败，原因：" + obj.ErrorInfo;
                }
                return obj;
            }
            catch (Exception e)
            {
                return new RequestResult { ErrorInfo = "，同步腾讯IM帐号失败，原因：" + e.Message };
            }
        }

        public JsonResponse GetUserLoginByPasswork(LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest.UserName == null)
                {
                    return new JsonResponse() { DataModel = "", Message = "手机号不能为空", State = OperatingState.CheckDataFail };
                }
                else
                {
                    var userinfo = _userInfoRepository.Table.Where(t => t.UserName == loginRequest.UserName).FirstOrDefault();
                    if (userinfo == null)
                    {
                        return new JsonResponse() { DataModel = "", Message = "当前用户不存在，请通知管理员为您注册账号！", State = OperatingState.CheckDataFail };
                    }
                    else if (userinfo.DeletedState == 1)
                    {
                        return new JsonResponse() { DataModel = "", Message = "您的帐号已经被禁用，无法登录！", State = OperatingState.CheckDataFail };
                    }
                    else if (userinfo.AccountState == (int)UserInfoState.Modity)
                    {
                        return new JsonResponse() { DataModel = "", Message = "您的帐号等待审核中，无法登录！", State = OperatingState.CheckDataFail };
                    }
                    else if (userinfo.AccountState == (int)UserInfoState.Invalid)
                    {
                        return new JsonResponse() { DataModel = "", Message = "您的帐号未通过审核，无法登录！", State = OperatingState.CheckDataFail };
                    }
                    else if(userinfo.AccountState == (int)UserInfoState.Normal || userinfo.AccountState == null)
                    {
                        ////密码转换为md5加密格式
                        string cl = loginRequest.Password;
                        string pwd = "";
                        MD5 md5 = MD5.Create();//实例化一个md5对像
                        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
                        // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                        for (int i = 0; i < s.Length; i++)
                        {
                            pwd = pwd + s[i].ToString("X");
                        }

                        var IsExists = _userInfoRepository.Table.Any(t => t.Password == pwd);
                        if (IsExists)
                        {
                            //调用token接口，存放cookie
                            var userid = _userInfoRepository.Table.Where(t => t.UserName == loginRequest.UserName).Select(t => t.Id).FirstOrDefault();
                            var tokenService = EngineContext.Current.Resolve<ITokenService>();
                            var tokenInfo = tokenService.GenerateToken(new GenerateTokenRequest
                            {
                                UserId = userid,
                                IsApp = loginRequest.IsApp
                            });
                            var tokenInfoViewModel = new TokenInfoViewModel()
                            {
                                UserId = userid,
                                Message = tokenInfo.Message,
                                Token = tokenInfo.Token,
                                Success = tokenInfo.Success
                            };
                            return new JsonResponse() { DataModel = tokenInfoViewModel, Message = "成功！", State = OperatingState.Success };
                        }
                        else
                        {
                            return new JsonResponse() { DataModel = "", Message = "密码错误", State = OperatingState.CheckDataFail };
                        }
                    } 
                    else
                    {
                        return new JsonResponse() { DataModel = "", Message = "手机号异常或不存在", State = OperatingState.CheckDataFail };

                    }
                }
            }
            catch (Exception e)
            {
                return new JsonResponse() { DataModel = "", Message = "错误" + e.Message.ToString(), State = OperatingState.CheckDataFail };
            }
        }
        #endregion
    }
}
