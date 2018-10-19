﻿using System.Collections.Generic;
using System.Text;
using SIS.Http.Enums;
using SIS.Http.Headers;
using SIS.Http.Requests.Contracts;
using SIS.Http.Responses;
using SIS.Http.Responses.Contracts;
using SIS.MvcFramework.Services;
using SIS.MvcFramework.ViewEngine;

namespace SIS.MvcFramework
{
    public abstract class Controller
    {
        protected Controller()
        {
            this.Response = new HttpResponse { StatusCode = HttpResponseStatusCode.OK };
        }

        public IHttpRequest Request { get; set; }

        public IHttpResponse Response { get; set; }

        public IViewEngine ViewEngine { get; set; }

        public IUserCookieService UserCookieService { get; internal set; }

        protected string User
        {
            get
            {
                if (!this.Request.Cookies.ContainsCookie(".auth-cakes"))
                {
                    return null;
                }
                //.auth-IRunes
                //.auth-cakes
                var cookie = this.Request.Cookies.GetCookie(".auth-cakes");
                var cookieContent = cookie.Value;
                var userName = this.UserCookieService.GetUserData(cookieContent);
                return userName;
            }
        }


        protected IHttpResponse View(string viewName)
        {
            var allContent = this.GetViewContent(viewName, (object) null);
            this.PrepareHtmlResult(allContent);
            return this.Response;
        }

        protected IHttpResponse View<T>(string viewName, T model = null)
            where T : class
        {
            var allContent = this.GetViewContent(viewName, model);
            this.PrepareHtmlResult(allContent);
            return this.Response;
        }

        protected IHttpResponse ViewLoggedOut(string viewName)
        {
            var allContent = this.GetViewContentLoggedOut(viewName, (object) null);
            this.PrepareHtmlResult(allContent);
            return this.Response;
        }

        protected IHttpResponse ViewLoggedOut<T>(string viewName, T model = null)
            where T : class
        {
            var allContent = this.GetViewContentLoggedOut(viewName, model);
            this.PrepareHtmlResult(allContent);
            return this.Response;
        }


        protected IHttpResponse File(byte[] content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentLength, content.Length.ToString()));
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentDisposition, "inline"));
            this.Response.Content = content;
            return this.Response;
        }

        protected IHttpResponse Redirect(string location)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.Location, location));
            this.Response.StatusCode = HttpResponseStatusCode.SeeOther; // TODO: Found better?
            return this.Response;
        }

        protected IHttpResponse Text(string content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/plain; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
            return this.Response;
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            var viewModel = new BadRequestViewModel()
            {
                ErrorMessage = errorMessage,
            };
            var allContent = this.GetViewContent("Error", viewModel);
            this.PrepareHtmlResult(allContent);
            this.Response.StatusCode = HttpResponseStatusCode.BadRequest;
            return this.Response;
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            var viewBag = new Dictionary<string, string>();
            viewBag.Add("Error", errorMessage);
            var allContent = this.GetViewContent("Error", viewBag);
            this.PrepareHtmlResult(allContent);
            this.Response.StatusCode = HttpResponseStatusCode.InternalServerError;
            return this.Response;
        }

        private string GetViewContent<T>(string viewName, T model)
        {
            var content = this.ViewEngine.GetHtml(viewName, System.IO.File.ReadAllText("Views/" + viewName + ".html"), model);
            var layoutFileContent = System.IO.File.ReadAllText("Views/_Layout.html");
            var allContent = layoutFileContent.Replace("@RenderBody()", content);
            var layoutContent = this.ViewEngine.GetHtml("_Layout", allContent, model);
            return layoutContent;
        }

        private string GetViewContentLoggedOut<T>(string viewName, T model)
        {
            var content = this.ViewEngine.GetHtml(viewName, System.IO.File.ReadAllText("Views/" + viewName + ".html"), model);
            var layoutFileContent = System.IO.File.ReadAllText("Views/_Layout_LoggedOut.html");
            var allContent = layoutFileContent.Replace("@RenderBody()", content);
            var layoutContent = this.ViewEngine.GetHtml("_Layout_LoggedOut", allContent, model);
            return layoutContent;
        }


        private void PrepareHtmlResult(string content)
        {
            this.Response.Headers.Add(new HttpHeader(HttpHeader.ContentType, "text/html; charset=utf-8"));
            this.Response.Content = Encoding.UTF8.GetBytes(content);
        }
    }

    public class BadRequestViewModel
    {
        public string ErrorMessage { get; set; }
    }
}
