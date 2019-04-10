using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
namespace JNKJ.Cache.Caching
{
    /// <summary>
    ///表示在HTTP请求之间缓存（长期缓存）的管理器。
    /// </summary>
    public partial class MemoryCacheManager : ICacheManager
    {
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }
        /// <summary>
        /// 获取或设置与指定键关联的值。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">获取键的值</param>
        /// <returns>返回指定键的值.</returns>
        public virtual T Get<T>(string key)
        {
            return (T)Cache[key];
        }
        /// <summary>
        /// 将指定的键和对象添加到缓存中。 
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">数据</param>
        /// <param name="cacheTime">缓存时间</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(cacheTime);
            Cache.Add(new CacheItem(key, data), policy);
        }
        /// <summary>
        /// 获取一个值，该值指示指定的键是否有被缓存。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回值</returns>
        public virtual bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }
        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }
        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysitemove = new List<string>();
            foreach (var item in Cache)
                if (regex.IsMatch(item.Key))
                    keysitemove.Add(item.Key);
            foreach (string key in keysitemove)
            {
                Remove(key);
            }
        }
        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }
    }
}