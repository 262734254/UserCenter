
namespace JNKJ.Services.General
{
    public class ConstKeys
    {
        /// <summary>
        /// 缓存时间
        /// </summary>
        public const int CACHETIME = 60 * 60 * 24;
        /// <summary>
        /// 零
        /// </summary>
        public const int ZERO_INT = 0;

        public const int NEGATIVE_INT = -1;
        /// <summary>
        /// 不启用
        /// </summary>
        public const int DISABLE = 0;
        /// <summary>
        /// 启用
        /// </summary>
        public const int ENABLE = 1;
        /// <summary>
        /// 忽略
        /// </summary>
        public const int IGNORE = 0;
        /// <summary>
        /// 正常的
        /// </summary>
        public const int NORMAL = 1;
        /// <summary>
        /// 新增
        /// </summary>
        public const int NEWLY = 2;
        /// <summary>
        /// 审核中
        /// </summary>
        public const int AUDITING = 3;
        /// <summary>
        /// 隐藏
        /// </summary>
        public const int HIDDEN = 4;
        /// <summary>
        /// 被锁定
        /// </summary>
        public const int LOCKED = 5;
        /// <summary>
        /// 被拒绝
        /// </summary>
        public const int BENY = 6;
        /// <summary>
        /// 无效的
        /// </summary>
        public const int INVALID = 7;
        /// <summary>
        /// 已删除
        /// </summary>
        public const int DELETED = 99;
        /// <summary>
        /// 数据不存在
        /// </summary>
        public const int ERROR_NONEXISTENT = 1;
        /// <summary>
        /// 鉴权失败
        /// </summary>
        public const int ERROR_AUTHENTICATIONFAIL = 2;
        /// <summary>
        /// 数据已存在
        /// </summary>
        public const int ERROR_EXISTENT = 3;
        /// <summary>
        /// 数据已被锁定
        /// </summary>
        public const int ERROR_LOCKED = 4;
        /// <summary>
        /// 失败
        /// </summary>
        public const int ERROR_FAIL = 99;
        /// <summary>
        /// 
        /// </summary>
        public const string ADMIN_REDIRECT_URL = "/admin/systems/ManagerRoleList";
        /// <summary>
        /// 
        /// </summary>
        public const string SYSTEMS_KEY = "Systems";
        /// <summary>
        /// 
        /// </summary>
        public const string SELLER_KEY = "Seller";
        /// <summary>
        /// 
        /// </summary>
        public const string BUYER_KEY = "Buyer";
        /// <summary>
        /// 
        /// </summary>
        public const string USER_KEY = "User";
        /// <summary>
        /// 默认的站点ID
        /// </summary>
        public const int DEFAULT_STORE = 0;
        /// <summary>
        /// 默认的页面大小
        /// </summary>
        public const int DEFAULT_PAGESIZE = 10;
        public const int DEFAULT_MAX_PAGESIZE = 100;
        /// <summary>
        /// 默认的页码
        /// </summary>
        public const int DEFAULT_PAGEINDEX = 1;
        /// <summary>
        /// 用户登陆成功
        /// </summary>
        public const int LOGIN_SUCCESSFUL = 1;
        /// <summary>
        /// 用户不存在
        /// </summary>
        public const int LOGIN_NOTEXIST = 2;
        /// <summary>
        /// 用户密码错误
        /// </summary>
        public const int LOGIN_WRONGPASSWORD = 3;
        /// <summary>
        /// 用户未激活
        /// </summary>
        public const int LOGIN_NOTACTIVE = 4;
        /// <summary>
        /// 用户已经删除
        /// </summary>
        public const int LOGIN_DELETED = 5;
        public const string LAYER_ONE = "";
        public const string LAYER_TWO = "----|--";
        public const string LAYER_THREE = "--------|--";

    }
}
