﻿namespace ContosoUniversity.Infrastructure.DataAccess
{
    using System.Web.Mvc;
    using App_Start;
    using DAL;

    public class MvcTransactionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Logger.Instance.Verbose("MvcTransactionFilter::OnActionExecuting");
            var context = StructuremapMvc.ParentScope.CurrentNestedContainer.GetInstance<SchoolContext>();
            context.BeginTransaction();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Logger.Instance.Verbose("MvcTransactionFilter::OnActionExecuted");
            var instance = StructuremapMvc.ParentScope.CurrentNestedContainer.GetInstance<SchoolContext>();
            instance.CloseTransaction(filterContext.Exception);
        }
    }
}