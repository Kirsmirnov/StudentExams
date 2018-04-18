using StudentExams.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentExams.Controllers
{
    public class StudentsController : Controller
    {
        public ActionResult Index(int id)
        {
            if (!Authentication.IsAdmin && !Authentication.IsTeacher)
                throw new UnauthorizedAccessException();

            StudentGroupId = id;
            PrepareViewBag();
            return View("Index");
        }

        private void PrepareViewBag()
        {
            ViewBag.Items = GetListItems();
            ViewBag.StudentGroups = GetStudentGroups();
            ViewBag.GroupName = GetStudentGroup(StudentGroupId).Name;
            ViewBag.StudentGroupId = StudentGroupId;
        }

        private int StudentGroupId
        {
            get
            {
                return Convert.ToInt32(Session["StudentGroupId"]);
            }
            set
            {
                Session["StudentGroupId"] = value;
            }
        }

        private StudentGroup GetStudentGroup(int id)
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.StudentGroup
                    .AsNoTracking()
                    .First(t => t.Id == id);
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

        private IList GetListItems()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.User.AsNoTracking()
                    .Where(t => t.StudentGroupId == StudentGroupId)
                    .OrderBy(t => t.UserName)
                    .Select(t => new
                    {
                        Id = t.UserId,
                        UserLogin = t.UserLogin,
                        UserName = t.UserName,
                        GroupName = t.StudentGroup.Name
                    })
                    .ToList();
            }
        }


        public ActionResult Edit(int? id)
        {
            User model;
            if (id != null)
            {
                using (DataContext ctx = new DataContext())
                {
                    model = ctx.User.First(t => t.UserId == id && t.UserGroupId == 6 && t.StudentGroupId == StudentGroupId);
                }
            }
            else
            {
                model = new User();
                model.InitNewObject();
                model.UserGroupId = 6;
                model.StudentGroupId = StudentGroupId;
            }
            PrepareViewBag();
            return View("Edit", model);
        }


        public ActionResult Save(User model)
        {
            try
            {
                model.UserGroupId = 6;
                string msg = model.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    ViewBag.Message = msg;
                    PrepareViewBag();
                    return View("Edit", model);
                }
                using (DataContext ctx = new DataContext())
                {
                    if (model.UserId != 0)
                    {
                        var item = ctx.User.FirstOrDefault(t => t.UserId == model.UserId);
                        item.UserName = model.UserName;
                        item.UserLogin = model.UserLogin;
                        item.StudentGroupId = model.StudentGroupId;
                    }
                    else
                    {
                        ctx.User.Add(model);
                    }

                    ctx.SaveChanges();
                    return RedirectToAction("Index", new { id = model.StudentGroupId });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                PrepareViewBag();
                return Edit(model.UserId);
            }
        }


        public ActionResult Delete(int id)
        {
            using (DataContext ctx = new DataContext())
            {
                var item = ctx.User.First(t => t.UserId == id);
                ctx.User.Remove(item);
                ctx.SaveChanges();
                return RedirectToAction("Index", new { id = StudentGroupId });
            }
        }
    }
}