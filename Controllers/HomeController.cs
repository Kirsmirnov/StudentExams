using StudentExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentExams.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Authentication.User != null)
            {
                if (Authentication.IsAdmin || Authentication.IsTeacher)
                {
                    return RedirectToAction("Index", "ExamList");
                }
                else
                {
                    return RedirectToAction("MyExamResults");
                }
            }
            else
            {
                return RedirectToAction("TeacherLogin", "Login");
            }
        }


        public ActionResult MyExamResults()
        {
            using(DataContext ctx=new DataContext())
            {
                var myExams = ctx.ExamListDetail
                    .Where(t => t.StudentId == Authentication.User.UserId && !string.IsNullOrEmpty(t.ExamResult))
                    .OrderByDescending(t => t.ExamList.ExamDate)
                    .Select(t=>new 
                    {
                        ExamDate = t.ExamList.ExamDate,
                        ExamResult = t.ExamResult,
                        TeacherName = t.ExamList.Teacher.UserName,
                        PredmetName = t.ExamList.Predmet.Name,
                        ExamType = t.ExamList.ExamType.Name
                    })
                    .ToList();
                ViewBag.MyExamResults = myExams;
                return View();
            }
        }
    }
}