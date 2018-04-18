using StudentExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace StudentExams
{
    public static class Authentication
    {
        public static User User { get; set; }

        /// <summary>
        /// Проверка логина и пароля, возвращает true если имя и пароль верные, иначе false
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckAutentication(String userLogin, String password)
        {
            String passwordHash = ComputePasswordHash(password);
            using (DataContext ctx = new DataContext())
            {
                int cnt = ctx.User.Count(u => u.IsActive && u.UserLogin.ToUpper() == userLogin.ToUpper() && u.PasswordHash == passwordHash);
                return (cnt > 0);
            }
        }


        /// <summary>
        /// Изменение пароля учетной записи
        /// Возвращает true если пароль был изменен, или false если не был изменен
        /// </summary>
        /// <param name="login"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static void ChangePassword(String login, String newPassword)
        {
            using (DataContext ctx = new DataContext())
            {
                User user = ctx.User.Where(u => u.UserLogin.ToUpper() == login.ToUpper()).First();
                user.PasswordHash = ComputePasswordHash(newPassword);
                ctx.SaveChanges();
            }
        }


        /// <summary>
        /// Вычисление MD5-хеша для пароля
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static String ComputePasswordHash(String password)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                String strHash = "";
                foreach (byte b in hash)
                {
                    strHash += b.ToString("X2");
                }
                return strHash;
            }
        }


        public static bool IsAdmin
        {
            get
            {
                return User != null && User.UserGroupId == 1;
            }
        }

        public static bool IsTeacher
        {
            get
            {
                return User != null && User.UserGroupId == 3;
            }
        }

        public static bool IsStudent
        {
            get
            {
                return User != null && User.UserGroupId == 6;
            }
        }
    }
}