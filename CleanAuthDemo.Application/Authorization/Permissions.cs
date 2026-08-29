using System;
using System.Collections.Generic;
using System.Text;

namespace CleanAuthDemo.Application.Authorization
{
    public static class Permissions
    {
        public static class Products
        {
            public const string Read = "products.read";
            public const string Create = "products.create";
            public const string Update = "products.update";
            public const string Delete = "products.delete";
        }
    }
}
