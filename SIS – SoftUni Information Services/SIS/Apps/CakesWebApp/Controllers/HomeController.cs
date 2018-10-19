﻿using System.Collections.Generic;
using CakesWebApp.Models;
using CakesWebApp.ViewModels.Home;
using SIS.Http.Enums;
using SIS.Http.Requests.Contracts;
using SIS.Http.Responses.Contracts;
using SIS.MvcFramework;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet("/")]
        public IHttpResponse Index()
        {
            var viewModel = new DisplayCakesViewModel();
            viewModel.Cakes = this.Db.Products;
            return this.View("Index", viewModel);
        }

        [HttpGet("/hello")]
        public IHttpResponse HelloUser()
        {
            return this.View("HelloUser", new HelloUserViewModel { Username = this.User });
        }
    }

    public class DisplayCakesViewModel
    {
        public IEnumerable<Product> Cakes { get; set; }
    }
}
