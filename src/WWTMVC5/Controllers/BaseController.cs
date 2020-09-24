using System.Web.Mvc;

namespace WWTMVC5.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //var @base = filterContext.Result as ViewResultBase;
            //if (@base != null)//Gets ViewResult and PartialViewResult
            //{
            //    object viewModel = @base.Model;

            //    if (viewModel is UserModel)
            //    {
            //        var model = viewModel as UserModel;
                    
            //    }
            //}
            base.OnActionExecuted(filterContext);
        }
	}
}