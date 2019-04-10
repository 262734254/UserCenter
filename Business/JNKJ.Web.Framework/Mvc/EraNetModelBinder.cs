﻿using System.Web.Mvc;
namespace JNKJ.Web.Framework.Mvc
{
    public class JNKJModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if (model is BaseJNKJModel)
            {
                ((BaseJNKJModel)model).BindModel(controllerContext, bindingContext);
            }
            return model;
        }
        protected override System.ComponentModel.PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            bindingContext.PropertyFilter = new System.Predicate<string>(pred);
            var values = base.GetModelProperties(controllerContext, bindingContext);
            return values;
        }
        protected bool pred(string target)
        {
            return true;
        }
    }
}
