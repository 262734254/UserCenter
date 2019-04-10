using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Results;

namespace JNKJ.Dto.Authority
{
    public class RolesRequest
    {
    }

    /// <summary>
    /// 项目角色请求类
    /// </summary>
    public class RolesPostRequest
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid? ID { get; set; }


        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }

    public class RolesGetRequest
    {

        /// <summary>
        /// 名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        public int pageSize { get; set; }
    }



    public class RolesPostResponse : JsonResponse
    {

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 权限集合
        /// </summary>
        public IQueryable<RoleRightResponseDTO> projectRoleRightList { get; set; }
    }

    /// <summary>
    /// 角色权限响应类
    /// </summary>
    public class RoleRightResponse : JsonResponse
    {
        /// <summary>
        /// 权限集合
        /// </summary>
        public IQueryable<RoleRightResponseDTO> roleRightList { get; set; }
    }


    /// <summary>
    /// 角色权限响应扩展类
    /// 用于安卓端，读取用户权限时，当前菜单下的子菜单集合
    /// </summary>
    public class RoleRightResponseDTO : RolesRights
    {
        /// <summary>
        /// 当前菜单的子菜单集合
        /// </summary>
        public IEnumerable<RolesRights> subMenuList { get; set; }

    }






    /// <summary>
    /// 角色权限请求类
    /// </summary>
    public class RoleRightRequest
    {
        /// <summary>
        /// 权限集合
        /// </summary>
        public IQueryable<RoleRightRequestDTO> roleRightRequestDTOList { get; set; }
    }


    /// <summary>
    /// 角色权限请求扩展类
    /// 用户web端，添加用户权限时，选择的每个菜单及菜单对应的按钮集合
    /// </summary>
    public class RoleRightRequestDTO : RolesRights
    {
        /// <summary>
        /// 当前菜单的操作按钮集合
        /// </summary>
        public IQueryable<Buttons> buttonList { get; set; }

    }

    /// <summary>
    /// 查询用户权限请求类
    /// </summary>
    public class RoleRightsRequest
    {
        /// <summary>
        /// 企业ID
        /// </summary>
        public Guid? EnterpriseId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 菜单ID
        /// </summary>
        public Guid? MenuId { get; set; }


    }


    /// <summary>
    /// 添加角色权限请求类
    /// </summary>
    public class AddRoleRightsStrRequest
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid RoleId { get; set; }
        /// <summary>
        /// 选中的数据集
        /// </summary>
        public List<AddRoleRightsStr> data { get; set; }
    }

    /// <summary>
    /// 自定义数据传输对象
    /// </summary>
    public class AddRoleRightsStr
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
        /// 级别
        /// </summary>
        public int lv { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public Guid? parentId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int displayNo { get; set; }

    }



    /// <summary>
    /// 添加、删除操作按钮权限
    /// </summary>
    public class AddButtonRightsRequest
    {

        public Guid? btnId { get; set; }

        public Guid? menuId { get; set; }

        public Guid? roleId { get; set; }
    }




    /// <summary>
    /// 添加、删除页面权限
    /// </summary>
    public class AddRightsRequest
    {
        public Guid? menuId { get; set; }

        public Guid? roleId { get; set; }

        /// <summary>
        /// 选中的菜单数是否为0
        /// </summary>
        public bool Mark { get; set; }

        /// <summary>
        /// 选中菜单的级别
        /// </summary>
        public int Level { get; set; }
    }


}


