using JNKJ.Dto.Results;
using JNKJ.Dto.UserCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Services.UserCenter.Interface
{
    public interface IRelationshipService
    {
        #region Methods
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse AddRelationship(RelationshipRequest request);
        /// <summary>
        /// 根据Id修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse UpdateRelationshipById(RelationshipRequest request);
        /// <summary>
        /// 根据企业Id和用户Id修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse UpdateRelationship(RelationshipRequest request);
        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse DeleteRelationshipById(RelationshipRequest request);
        /// <summary>
        /// 根据企业Id和用户Id删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse DeleteRelationship(RelationshipRequest request);
        #endregion
    }
}
