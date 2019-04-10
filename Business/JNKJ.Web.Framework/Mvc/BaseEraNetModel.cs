using System.Collections.Generic;
using System.Web.Mvc;
namespace JNKJ.Web.Framework.Mvc
{
    /// <summary>
    /// Base JNKJCommerce model
    /// </summary>
    public partial class BaseJNKJModel
    {
        public BaseJNKJModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
        }
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }
        /// <summary>
        /// Use this property to store any custom value for your models. 
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }
    }
    /// <summary>
    /// Base JNKJCommerce entity model
    /// </summary>
    public partial class BaseJNKJEntityModel : BaseJNKJModel
    {
        public virtual int Id { get; set; }
    }
}
