using StudentExams.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentExams.Controllers
{
    public class UserController : Controller
    {
        public UserController()
        {
            if (!Authentication.IsAdmin)
                throw new UnauthorizedAccessException();

            ViewBag.StudentGroups = GetStudentGroups();
            ViewBag.UserGroups = GetUserGroups();
        }

        public ActionResult Index()
        {
            ViewBag.Items = GetListItems();
            return View("Index");
        }


        private IList GetListItems()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.User.AsNoTracking()
                    .OrderBy(t => t.UserName)
                    .Select(t => new
                    {
                        Id = t.UserId,
                        UserName = t.UserName,
                        UserLogin = t.UserLogin,
                        GroupName = t.UserGroup.Name
                    })
                    .ToList();
            }
        }


        private IList GetUserGroups()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.UserGroup.AsNoTracking()
                    .OrderBy(t => t.Name)
                    .ToList();
            }
        }

        private IList GetStudentGroups()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.StudentGroup.AsNoTracking()
                    .OrderBy(t => t.Name)
                    .ToList();
            }
        }


        public ActionResult Edit(int? id)
        {
            try
            {
                User model;
                if (id != null)
                {
                    using (DataContext ctx = new DataContext())
                    {
                        model = ctx.User.First(t => t.UserId ==id);
                    }
                }
                else
                {
                    model = new User();
                    model.InitNewObject();
                }

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Index();
            }
        }        

        public ActionResult Save(User model, string Password, string PasswordConfirm)
        {
            try
            {
                string msg = model.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    ViewBag.Message = msg;
                    return View("Edit", model);
                }
                if (model.UserId == 0 && string.IsNullOrEmpty(Password))
                {
                    ViewBag.Message = "Необходимо задать пароль.";
                    return View("Edit", model);
                }
                if (!string.IsNullOrEmpty(Password))
                {
                    if (Password != PasswordConfirm)
                    {
                        ViewBag.Message = "Пароль и повтор пароля не совпадают.";
                        return View("Edit", model);
                    }
                }
                using (DataContext ctx = new DataContext())
                {
                    if (model.UserId != 0)
                    {
                        var item = ctx.User.FirstOrDefault(t => t.UserId == model.UserId);
                        item.UserName = model.UserName;
                        item.UserLogin = model.UserLogin;
                        item.IsActive = model.IsActive;
                        item.StudentGroupId = model.StudentGroupId;
                        item.UserGroupId = model.UserGroupId;
                        if (!string.IsNullOrEmpty(Password))
                        {
                            item.PasswordHash = Authentication.ComputePasswordHash(Password);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Password))
                        {
                            model.PasswordHash = Authentication.ComputePasswordHash(Password);
                        }
                        ctx.User.Add(model);
                    }

                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Edit(model.UserId);
            }
        }


        public ActionResult Delete(int id)
        {
            try
            {
                using (DataContext ctx = new DataContext())
                {
                    var item = ctx.User.FirstOrDefault(t => t.UserId == id);
                    if (item != null)
                    {
                        string msg = item.CheckCanDelete();
                        if (string.IsNullOrEmpty(msg))
                        {
                            ctx.User.Remove(item);
                            ctx.SaveChanges();
                        }
                        else
                        {
                            ViewBag.Message = msg;
                            return Index();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Index();
            }
        }
    }
}