using StudentExams.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace StudentExams.Controllers
{
    public class ExamListController : Controller
    {
        public ActionResult Index()
        {
            if (!Authentication.IsAdmin && !Authentication.IsTeacher)
                throw new UnauthorizedAccessException();

            PrepareViewBag();
            return View("Index");
        }


        private void PrepareViewBag()
        {
            ViewBag.Items = GetListItems();
            ViewBag.Predmets = GetPredmets();
            ViewBag.Groups = GetGroups();
            ViewBag.ExamTypes = GetExamTypes();
        }


        private IList GetListItems()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.ExamList
                    .AsNoTracking()
                    .Where(t => t.TeacherId == Authentication.User.UserId || Authentication.IsAdmin)
                    .OrderByDescending(t => t.ExamDate)
                    .Select(t => new
                    {
                        Id = t.Id,
                        ExamDate = t.ExamDate,
                        ExamType = t.ExamType.Name,
                        TeacherName = t.Teacher.UserName,
                        PredmetName = t.Predmet.Name,
                        StudentGroupName = t.StudentGroup.Name
                    })
                    .ToList();
            }
        }


        private IList GetPredmets()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.Predmet.AsNoTracking()
                    .OrderBy(t => t.Name)
                    .ToList();
            }
        }


        private IList GetGroups()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.StudentGroup.AsNoTracking()
                    .OrderBy(t => t.Name)
                    .ToList();
            }
        }

        private IList GetExamTypes()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.ExamType.AsNoTracking()
                    .OrderBy(t => t.Name)
                    .ToList();
            }
        }

        public ActionResult Create()
        {
            try
            {
                ExamList model = new ExamList();
                model.InitNewObject();
                PrepareViewBag();
                return View("Create", model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Index();
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                ExamList model;
                using (DataContext ctx = new DataContext())
                {
                    model = ctx.ExamList
                        .Include(t => t.Teacher)
                        .Include(t => t.ExamType)
                        .Include(t => t.Predmet)
                        .Include(t => t.StudentGroup)
                        .Include(t => t.ExamListDetail)
                        .Include("ExamListDetail.Student")
                        .AsNoTracking()
                        .First(t => t.Id == id);
                }
                PrepareViewBag();
                return View("Edit", model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Index();
            }
        }


        public ActionResult SaveCreated(ExamList model)
        {
            try
            {
                model.InitNewObject();
                string msg = model.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    ViewBag.Message = msg;
                    PrepareViewBag();
                    return View("Create", model);
                }
                using (DataContext ctx = new DataContext())
                {
                    var students = ctx.StudentGroup
                        .First(t => t.Id == model.StudentGroupId)
                        .Students;

                    foreach (User student in students)
                    {
                        model.ExamListDetail.Add(new ExamListDetail()
                        {
                            Student = student,
                            ExamList = model
                        });
                    }

                    model.ExamDate = DateTime.Now;
                    ctx.ExamList.Add(model);

                    ctx.SaveChanges();
                    return RedirectToAction("Edit", new { id = model.Id });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                PrepareViewBag();
                return View("Create", model);
            }
        }


        public ActionResult SaveExisting(ExamList model)
        {
            try
            {
                using (DataContext ctx = new DataContext())
                {
                    var item = ctx.ExamList.First(t => t.Id == model.Id);

                    foreach (ExamListDetail detail in model.ExamListDetail)
                    {
                        var detailItem = ctx.ExamListDetail.First(t => t.StudentId == detail.StudentId && t.ExamListId == model.Id);
                        if (detailItem != null)
                        {
                            detailItem.ExamResult = detail.ExamResult;
                        }
                    }

                    ctx.SaveChanges();
                    return Index();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                PrepareViewBag();
                return Edit(model.Id);
            }
        }


        public ActionResult Delete(int id)
        {
            try
            {
                using (DataContext ctx = new DataContext())
                {
                    var item = ctx.ExamList.FirstOrDefault(t => t.Id == id);
                    if (item != null)
                    {
                        ctx.ExamList.Remove(item);
                        ctx.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                PrepareViewBag();
                ViewBag.Message = ex.Message;
                return Index();
            }
        }
    }
}