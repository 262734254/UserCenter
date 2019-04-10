using JNKJ.Dto.UserCenter;
using JNKJ.Services.UserCenter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace JNKJ.WebAPI.Areas.UserCenter.Controllers
{
    public class RelationshipController : BaseController
    {
        private readonly IRelationshipService _relationshipService;
        public RelationshipController(IRelationshipService relationshipService)
        {
            _relationshipService = relationshipService;
        }

        #region Methods
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage add_relationship(RelationshipRequest request)
        {
            return toJson(_relationshipService.AddRelationship(request));
        }
        /// <summary>
        /// 根据Id修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage update_relationship_by_Id(RelationshipRequest request)
        {
            return toJson(_relationshipService.UpdateRelationshipById(request));
        }
        /// <summary>
        /// 根据企业Id和用户Id修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage update_relationship(RelationshipRequest request)
        {
            return toJson(_relationshipService.UpdateRelationship(request));
        }
        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage delete_relationship_by_Id(RelationshipRequest request)
        {
            return toJson(_relationshipService.DeleteRelationshipById(request));
        }
        /// <summary>
        /// 根据企业Id和用户Id删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage delete_relationship(RelationshipRequest request)
        {
            return toJson(_relationshipService.DeleteRelationship(request));
        }


        #endregion


    }
}