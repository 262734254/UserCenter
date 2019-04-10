using JNKJ.Domain.UserCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Dto.UserCenter
{
    /// <summary>
    /// 组织架构请求类
    /// </summary>
    public class DeptInfoRequest : DeptInfo
    {
    }

    /// <summary>
    /// 获取组织架构请求类
    /// </summary>
    public class GetDeptInfoRequest
    {
        /// <summary>
        ///组织架构Id
        /// </summary>
        public Guid? Id { get; set; }
        ///<summary>
        ///企业表Id
        ///</summary>
        public Guid? EnterpriseID { set; get; }
        ///<summary>
        ///上级部门Id。null为无
        ///</summary>
        public Guid? ParentID { set; get; }

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        public int? pageIndex { get; set; }

        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        public int? pageSize { get; set; }

        /// <summary>
        /// 关键字 --- 部门名字/企业名称/创建人名称
        /// </summary>
        public string keyWord { get; set; }
    }

    /// <summary>
    /// 获取组织架构返回类
    /// </summary>
    public class DeptInfoResponse
    {
        /// <summary>
        ///组织架构Id
        /// </summary>
        public Guid Id { get; set; }
        ///<summary>
        ///部门名字
        ///</summary>
        public string Name { set; get; }
        ///<summary>
        ///企业表Id
        ///</summary>
        public Guid? EnterpriseID { set; get; }
        ///<summary>
        ///企业名称
        ///</summary>
        public string EnterpriseName { set; get; }
        ///<summary>
        ///上级部门Id。null为无
        ///</summary>
        public Guid? ParentID { set; get; }
        ///<summary>
        ///上级部门名称
        ///</summary>
        public string ParentName { set; get; }
        ///<summary>
        ///创建人，对应用户表Id
        ///</summary>
        public Guid? CreateUserId { set; get; }
        ///<summary>
        ///创建人名称
        ///</summary>
        public string UserName { set; get; }

        ///<summary>
        ///创建人头像
        ///</summary>
        public string ImgUrl { set; get; }

        ///<summary>
        ///创建时间
        ///</summary>
        public DateTime CreateTime { set; get; }

    }

    /// <summary>
    /// 获取组织架构响应类 ---- PC端树形图数据
    /// </summary>
    public class GetOrganizationResponse
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 子节点数据
        /// </summary>
        public object children { get; set; }
    }
}
