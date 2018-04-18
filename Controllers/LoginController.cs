using StudentExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace StudentExams.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult StudentLogin()
        {
            return View("StudentLogin");
        }

        public ActionResult TeacherLogin()
        {
            ViewBag.UserList = GetTeacherList();
            return View("TeacherLogin");
        }

        public ActionResult CheckLoginStudent(string Login, string Password)
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Message = "Необходимо ввести логин и пароль. Логином для студента является номер студенческого билета.";
                return StudentLogin();
            }

            if (Authentication.CheckAutentication(Login, Password))
            {
                using (DataContext ctx = new DataContext())
                {
                    Authentication.User = ctx.User
                        .Include(t => t.UserGroup)
                        .Include(t => t.StudentGroup)
                        .First(t => t.UserLogin.ToLower() == Login.ToLower());
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Message = "Неверное имя пользователя или пароль.";
                return TeacherLogin();
            }
        }

        public ActionResult CheckLoginTeacher(string Login, string Password)
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Message = "Необходимо ввести логин и пароль.";
                return TeacherLogin();
            }

            if (Authentication.CheckAutentication(Login, Password))
            {
                using (DataContext ctx = new DataContext())
                {
                    Authentication.User = ctx.User
                        .Include(t=>t.UserGroup)
                        .Include(t=>t.StudentGroup)
                        .First(t => t.UserLogin.ToLower() == Login.ToLower());
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Message = "Неверное имя пользователя или пароль.";
                return TeacherLogin();
            }
        }        

        public ActionResult ChangePassword()
        {
            return View("ChangePassword");
        }


        public ActionResult ChangePasswordConfirm(string OldPassword, string NewPassword, string NewPasswordConfirm)
        {
            if (!Authentication.CheckAutentication(Authentication.User.UserLogin, OldPassword))
            {
                ViewBag.Message = "Неверно введен старый пароль.";
                return ChangePassword();
            }
            if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(NewPasswordConfirm))
            {
                ViewBag.Message = "Необходимо ввести старый пароль, новый пароль и подтверждение пароля.";
                return ChangePassword();
            }
            if (NewPassword != NewPasswordConfirm)
            {
                ViewBag.Message = "Новый пароль и подтверждение пароля не совпадают.";
                return ChangePassword();
            }

            Authentication.ChangePassword(Authentication.User.UserLogin, NewPassword);
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Logout()
        {
            Authentication.User = null;
            return TeacherLogin();
        }

        private List<User> GetTeacherList()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.User
                    .AsNoTracking()
                    .Where(t => t.UserGroupId == 1 || t.UserGroupId == 3)
                    .OrderBy(t => t.UserName)
                    .ToList();
            }
        }
    }
}