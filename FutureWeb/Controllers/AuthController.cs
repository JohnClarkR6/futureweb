﻿using FutureWeb.Models;
using FutureWeb.ViewModels;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FutureWeb.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();          //signs out of formsAuthentication which removes authensicity token
            return RedirectToRoute("home");         //returns to the home route
        }

        public ActionResult Login()
        {
            return View();                                           //method used for GET requests
        }

        [HttpPost]                                                   //removed ambiguity between views
        public ActionResult Login(AuthLogin form, string returnUrl)                //login view using the ViewModel's data and will show what happens after data is used.
        {
            var user = Database.Session.Query<User>().FirstOrDefault(u => u.Username == form.Username);  //query database about user

            if (user == null)
                FutureWeb.Models.User.FakeHash();           //if no user name apply fake hash  to fix timing attack

            if (user == null || !user.CheckPassword(form.Password))
                ModelState.AddModelError("Username", "Username or password is incorrect");       

            if (!ModelState.IsValid)                                 //If the modelstate in Auth.Viewmodels if not valid i.e with [Required]   
                return View(form);                                   //Return the view with the form again. If valid return the valid view

            FormsAuthentication.SetAuthCookie(form.Username, true); //authentication to tell asp.net who a person is who he says he is. Not Authorization.
                                                                    //method used for POST requests   //form user example form.Username will = authlogins forms username.
            if (!string.IsNullOrWhiteSpace(returnUrl))              //if returnurl exists in the query string i.e .com/reutrnurl@posts@admin
                return Redirect(returnUrl);                         //then redirect the user to the retunUrl string

            return RedirectToRoute("home");                         //otherwise return back to home.                 
        }

        public ActionResult Register()
        {
            return View();                                           //method used for GET requests
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(AuthRegister form)
        {
            var user = new User();

            if (Database.Session.Query<User>().Any(u => u.Username == form.Username))     //querys the database for usernames, if it is unique it will continue
                ModelState.AddModelError("Username", "Username must be unique");

            if (!ModelState.IsValid)
                return View(form);

            user.Email = form.Email;
            user.Username = form.Username;
            user.SetPassword(form.Password);

            Database.Session.Save(user);

            return RedirectToRoute("home");                                              //method used for GET requests
        }

    }
}