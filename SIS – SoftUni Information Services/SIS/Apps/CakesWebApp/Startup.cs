﻿using System;
using SIS.MvcFramework;
using SIS.MvcFramework.Logger;
using SIS.MvcFramework.Services;

namespace CakesWebApp
{
    public class Startup : IMvcApplication
    {
        public void Configure()
        {

        }

        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IHashService, HashService>();
            collection.AddService<IUserCookieService, UserCookieService>();
            collection.AddService<ILogger,FileLogger>();
            collection.AddService<ILogger>((() => new FileLogger($"log_{DateTime.Now:yyyy-dd-M}.txt")));
        }
    }
}
