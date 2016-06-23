﻿using System.Web;
using System.Web.Mvc;

namespace ContosoUniversity
{
    using Infrastructure;
    using Infrastructure.DataAccess;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ValidatorActionFilter());
            filters.Add(new MvcTransactionFilter());
        }
    }
}
