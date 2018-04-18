using StudentExams.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using StudentExams.ViewModels;

namespace StudentExams.Controllers
{
    public class StudentWorkController : Controller
    {
        public ActionResult Index(StudentWorkViewModel model)
        {
            if (model == null || model.Id == null)
            {
                model = (Session["StudentWorkViewModel"] as StudentWorkViewModel);
                if (model == null)
                {
                    model = new StudentWorkViewModel();
                    model.Id = 0;
                }
            }
            else
            {
                Session["StudentWorkViewModel"] = model;
            }
            ViewBag.Items = GetListItems(model);
            PrepareViewBag();
            return View("Index", model);
        }


        private void PrepareViewBag()
        {
            ViewBag.Teachers = GetTeachers();
            ViewBag.Predmets = GetPredmets();
            ViewBag.Groups = GetGroups();
            ViewBag.WorkTypes = GetWorkTypes();
            ViewBag.Statuses = GetWorkStatuses();
        }


        private IList GetListItems(StudentWorkViewModel model)
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.StudentWork
                    .AsNoTracking()
                    .Where(t => (t.TeacherId == Authentication.User.UserId || t.StudentId == Authentication.User.UserId || Authentication.IsAdmin))
                    .OrderByDescending(t => t.WorkDate)
                    .ToList()
                    .Where(t => (model == null || model.PredmetId == null || t.PredmetId == model.PredmetId)
                        && (model == null || model.WorkTypeId == null || t.WorkTypeId == model.WorkTypeId)
                        && (model == null || model.TeacherId == null || t.TeacherId == model.TeacherId)
                        && (model == null || model.StudentGroupId == null || t.Student.StudentGroupId == model.StudentGroupId)
                        && (model == null || string.IsNullOrEmpty(model.WorkStatus) || t.WorkStatus == model.WorkStatus))
                    .Select(t => new
                    {
                        Id = t.Id,
                        t.WorkDate,
                        t.WorkStatus,
                        WorkType = t.WorkType.Name,
                        Teacher = t.Teacher.UserName,
                        Student = t.Student.UserName,
                        Predmet = t.Predmet.Name,
                        StudentGroup = t.Student?.StudentGroup?.Name
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

        private IList GetWorkTypes()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.WorkType.AsNoTracking()
                    .OrderBy(t => t.Name)
                    .ToList();
            }
        }

        private IList GetWorkStatuses()
        {
            return new List<string>()
            {
                StudentWorkStatus.DRAFT,
                StudentWorkStatus.CHECKING,
                StudentWorkStatus.APPROOVED,
                StudentWorkStatus.REJECTED
            };
        }

        private IList GetTeachers()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.User.AsNoTracking()
                    .Where(t => t.UserGroupId == 3)
                    .OrderBy(t => t.UserName)
                    .ToList();
            }
        }

        public ActionResult Create()
        {
            try
            {
                StudentWork model = new StudentWork();
                model.InitNewObject();
                model.Student = Authentication.User;
                PrepareViewBag();
                return View("Edit", model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Index(null);
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                StudentWork model;
                using (DataContext ctx = new DataContext())
                {
                    model = ctx.StudentWork
                        .Include(t => t.Teacher)
                        .Include(t => t.Student)
                        .Include(t => t.WorkType)
                        .Include(t => t.Predmet)
                        .Include("Student.StudentGroup")
                        .AsNoTracking()
                        .First(t => t.Id == id);
                }
                PrepareViewBag();
                return View("Edit", model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return Index(null);
            }
        }

        public ActionResult SaveAsDraft(StudentWork model, HttpPostedFileBase upload)
        {
            return Save(model, StudentWorkStatus.DRAFT, upload);
        }

        public ActionResult SaveAsChecking(StudentWork model, HttpPostedFileBase upload)
        {
            return Save(model, StudentWorkStatus.CHECKING, upload);
        }

        public ActionResult SaveAsApprooved(StudentWork model)
        {
            return Save(model, StudentWorkStatus.APPROOVED, null);
        }

        public ActionResult SaveAsRejected(StudentWork model)
        {
            return Save(model, StudentWorkStatus.REJECTED, null);
        }


        private ActionResult Save(StudentWork model, string newStatus, HttpPostedFileBase upload)
        {
            try
            {
                string oldStatus = model.WorkStatus;
                model.WorkStatus = newStatus;
                string msg = model.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    model.WorkStatus = oldStatus;
                    throw new Exception(msg);
                }
                using (DataContext ctx = new DataContext())
                {
                    if (model.Id != 0)
                    {
                        var item = ctx.StudentWork.FirstOrDefault(t => t.Id == model.Id);
                        item.TeacherId = model.TeacherId;
                        item.StudentId = model.StudentId;
                        item.WorkTypeId = model.WorkTypeId;
                        item.WorkStatus = model.WorkStatus;
                        item.TeacherComments = model.TeacherComments;
                        item.PredmetId = model.PredmetId;

                        if (upload != null)
                        {
                            item.WorkFileName = System.IO.Path.GetFileName(upload.FileName);
                            item.WorkFile = new byte[upload.InputStream.Length];
                            upload.InputStream.Read(item.WorkFile, 0, item.WorkFile.Length);
                        }
                    }
                    else
                    {
                        if (upload != null)
                        {
                            model.WorkFileName = System.IO.Path.GetFileName(upload.FileName);
                            model.WorkFile = new byte[upload.InputStream.Length];
                            upload.InputStream.Read(model.WorkFile, 0, model.WorkFile.Length);
                        }

                        ctx.StudentWork.Add(model);
                    }

                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
                PrepareViewBag();
                FillModelForeignKeys(model);
                return View("Edit", model);
            }
        }

        private void FillModelForeignKeys(StudentWork model)
        {
            using (DataContext ctx = new DataContext())
            {
                if (model.Predmet == null)
                    model.Predmet = ctx.Predmet.FirstOrDefault(t => t.Id == model.PredmetId);
                if (model.WorkType == null)
                    model.WorkType = ctx.WorkType.FirstOrDefault(t => t.Id == model.WorkTypeId);
                if (model.Student == null)
                    model.Student = ctx.User.Include(t=>t.StudentGroup).FirstOrDefault(t => t.UserId == model.StudentId);
                if (model.Teacher == null)
                    model.Teacher = ctx.User.FirstOrDefault(t => t.UserId == model.TeacherId);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (DataContext ctx = new DataContext())
                {
                    var item = ctx.StudentWork.FirstOrDefault(t => t.Id == id);
                    if (item != null)
                    {
                        ctx.StudentWork.Remove(item);
                        ctx.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                PrepareViewBag();
                ViewBag.Message = ex.Message;
                return Index(null);
            }
        }

        [HttpGet]
        public ActionResult Download(int id)
        {
            using (DataContext ctx = new DataContext())
            {
                var item = ctx.StudentWork.FirstOrDefault(t => t.Id == id);
                byte[] buf = item.WorkFile;
                return this.File(buf, "application/octet-stream", item.WorkFileName);
            }
        }
    }
}