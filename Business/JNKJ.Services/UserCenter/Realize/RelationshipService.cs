using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JNKJ.Core.Data;
using JNKJ.Domain.UserCenter;
using JNKJ.Services.UserCenter.Interface;
using JNKJ.Dto.Results;
using JNKJ.Dto.UserCenter;
using JNKJ.Dto.Enums;

namespace JNKJ.Services.UserCenter.Realize
{
    public class RelationshipService : IRelationshipService
    {
        private readonly IRepository<Relationship> _relationship;

        public RelationshipService(IRepository<Relationship> relationship)
        {
            _relationship = relationship;
        }

        #region Methods
        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse AddRelationship(RelationshipRequest request)
        {
            if (request?.EnterpriseID == Guid.Empty || request?.UserId == Guid.Empty)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入必要的条件");
            }

            if (_relationship.Table.Any(c => c.EnterpriseID == request.EnterpriseID && c.UserId == request.UserId && c.DeletedState != (int)DeletedStates.Deleted))
            {
                return new JsonResponse(OperatingState.CheckDataFail, "已经存在该企业和用户");
            }
            var obj = new Relationship()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                EnterpriseID = request.EnterpriseID,
                RoleId = request.RoleId,
                DeptInfoId = request.DeptInfoId,
                State = request.State,
                IsEnterprise=request.IsEnterprise,
                CreateTime = DateTime.Now
            };
            try
            {
                _relationship.Insert(obj);
                _relationship.SaveChanges();
                return new JsonResponse(OperatingState.Success, "添加成功");
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Success, "操作时出现异常 " + e.Message);
            }

        }
        /// <summary>
        /// 修改操作  根据Id修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse UpdateRelationshipById(RelationshipRequest request)
        {
            if (request?.Id == Guid.Empty)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入必要的条件");
            }

            try
            {
                var obj = _relationship.GetById(request.Id);
                if (obj == null || obj.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "该数据为空或已被删除");
                }
                return UpdateOperating(obj,request);
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Success, "操作时出现异常 " + e.Message);
            }

        }
        /// <summary>
        /// 修改操作 根据用户id和企业id修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse UpdateRelationship(RelationshipRequest request)
        {
            if (request?.EnterpriseID == Guid.Empty || request?.UserId == Guid.Empty)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入必要的条件");
            }

            try
            {
                var obj = _relationship.Table.FirstOrDefault(t=>t.EnterpriseID==request.EnterpriseID&&t.UserId==request.UserId);
                if (obj == null || obj.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "该数据为空或已被删除");
                }

                return UpdateOperating(obj, request);
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Success, "操作时出现异常 " + e.Message);
            }

        }
        /// <summary>
        /// 删除操作 根据id 删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse DeleteRelationshipById(RelationshipRequest request)
        {
            if (request?.Id == Guid.Empty)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入必要的条件");
            }

            try
            {
                var obj = _relationship.GetById(request.Id);
                if (obj == null || obj.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "该数据为空或已被删除");
                }
                return DeleteOperating(obj);
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Success, "操作时出现异常 " + e.Message);
            }

        }
        /// <summary>
        /// 删除操作 根据用户id和企业id删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse DeleteRelationship(RelationshipRequest request)
        {
            if (request?.EnterpriseID == Guid.Empty || request?.UserId == Guid.Empty)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入必要的条件");
            }

            try
            {
                var obj = _relationship.Table.FirstOrDefault(t => t.EnterpriseID == request.EnterpriseID && t.UserId == request.UserId);
                if (obj == null || obj.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "该数据为空或已被删除");
                }

                return DeleteOperating(obj);
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Success, "操作时出现异常 " + e.Message);
            }

        }
        
        #endregion

        #region private
        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="obj">原数据</param>
        /// <param name="request">新数据</param>
        /// <returns></returns>
        private JsonResponse UpdateOperating(Relationship obj, RelationshipRequest request) {

            try
            {
                obj.RoleId = request.RoleId == null ? obj.RoleId : request.RoleId;
                obj.DeptInfoId = request.DeptInfoId == null ? obj.DeptInfoId : request.DeptInfoId;
                obj.State = request.State == null ? obj.State : request.State;
                obj.IsEnterprise = request.IsEnterprise == null ? obj.IsEnterprise : request.IsEnterprise;

                _relationship.Update(obj);
                _relationship.SaveChanges();

                return new JsonResponse(OperatingState.Success, "修改成功");
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Success, "操作时出现异常 " + e.Message);
            }
           
        }

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="obj">原数据</param>
        /// <param name="request">新数据</param>
        /// <returns></returns>
        private JsonResponse DeleteOperating(Relationship obj)
        {

            try
            {
                obj.DeletedState = (int)DeletedStates.Deleted;
                obj.DeletedTime = DateTime.Now;

                _relationship.Update(obj);
                _relationship.SaveChanges();

                return new JsonResponse(OperatingState.Success, "删除成功");
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Success, "操作时出现异常 " + e.Message);
            }

        }


        #endregion
    }
}
