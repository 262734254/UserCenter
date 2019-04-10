using System;
using System.Collections.Generic;
using JNKJ.Domain.UserCenter;

namespace JNKJ.Dto.Authority
{
    public class MenuResponse : Menu
    {
        public IEnumerable<Menu> SubmenuList { get; set; }
    }

    /// <summary>
    /// 菜单请求类
    /// </summary>
    public class MenuRequest
    {

        public Guid? Id { get; set; }

        /// <summary>
        /// 菜单Key
        /// </summary>
        public string FunctionKey { get; set; }


        /// <summary>
        /// 菜单名称
        /// </summary>
        public string FunctionName { get; set; }


        /// <summary>
        /// 菜单Url
        /// </summary>
        public string FunctionUrl { get; set; }


        /// <summary>
        /// 菜单Icon
        /// </summary>
        public string Icon { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayNo { get; set; }


        /// <summary>
        /// 菜单类型
        /// </summary>
        public int? FunctionType { get; set; }


        /// <summary>
        /// 直接父级ID
        /// </summary>
        public Guid? ParentID { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }
    }


    /// <summary>
    /// 菜单操作按钮请求类
    /// </summary>
    public class ButtongRequest
    {

        public Guid? Id { get; set; }

        /// <summary>
        /// 所属菜单ID
        /// </summary>
        public Guid? ProjectMenuId { get; set; }


        /// <summary>
        /// 按钮Url
        /// </summary>
        public string ButtonUrl { get; set; }


        /// <summary>
        /// 按钮文本
        /// </summary>
        public string ButtonName { get; set; }

        /// <summary>
        /// 按钮Key
        /// </summary>
        public string ButtonKey { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int DisplayNo { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }
    }
}
