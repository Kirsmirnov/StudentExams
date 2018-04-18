namespace StudentExams.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("User")]
    public partial class User
    {
        public const string DEFAULT_PASSWORD = "123";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            ExamList = new HashSet<ExamList>();
            ExamListDetail = new HashSet<ExamListDetail>();
            StudentWork = new HashSet<StudentWork>();
            StudentWork1 = new HashSet<StudentWork>();
        }

        [Required]
        [StringLength(255)]
        public string UserLogin { get; set; }

        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        public bool IsActive { get; set; }

        public int UserId { get; set; }

        [StringLength(255)]
        public string PasswordHash { get; set; }

        public int UserGroupId { get; set; }

        public int? StudentGroupId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamList> ExamList { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamListDetail> ExamListDetail { get; set; }

        public virtual StudentGroup StudentGroup { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentWork> StudentWork { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentWork> StudentWork1 { get; set; }

        public virtual UserGroup UserGroup { get; set; }


        public void InitNewObject()
        {
            IsActive = true;
            PasswordHash = Authentication.ComputePasswordHash(DEFAULT_PASSWORD);
        }


        public string Validate()
        {
            if (string.IsNullOrWhiteSpace(UserLogin))
                return "Необходимо ввести логин.";
            if (string.IsNullOrWhiteSpace(UserName))
                return "Необходимо ввести ФИО.";
            if (UserGroupId == 0)
                return "Необходимо указать роль.";
            if (UserGroupId == 6 && StudentGroupId == null)
                return "Необходимо указать группу.";
            if (UserGroupId != 6 && StudentGroupId != null)
                return "Для указанной роли пользователя поле Группа не должно быть заполнено. Данное поле предназначено для студентов.";

            using (DataContext ctx = new DataContext())
            {
                User existing = ctx.User.FirstOrDefault(t => t.UserLogin.ToLower().Trim() == UserLogin.ToLower().Trim() && t.UserId != UserId);
                if (existing != null)
                    return String.Format("Пользователь с логином {0} уже существует в системе.", UserLogin);

                if (UserId > 0)
                {
                    if (UserGroupId != 1
                        || (UserGroupId == 1 && IsActive == false))
                    {
                        User existingAdmin = ctx.User.FirstOrDefault(t => t.UserGroupId == 1 && t.IsActive == true && t.UserId != UserId);
                        if (existingAdmin == null)
                            return String.Format("Изменения учетной записи {0} не могут быть сохранены, т.к. в системе не останется ни одного активного администратора.", UserLogin);
                    }
                }
            }

            return "";
        }


        /// <summary>
        /// Проверка возможности удаления объекта.
        /// Возвращает пустую строку, если удаление возможно. Иначе возвращает текст сообщения для пользователя.
        /// </summary>
        /// <returns></returns>
        public String CheckCanDelete()
        {
            if (UserGroupId == 1)
            {
                using (DataContext ctx = new DataContext())
                {
                    User existingAdmin = ctx.User.FirstOrDefault(t => t.UserGroupId == 1 && t.IsActive == true && t.UserId != UserId);
                    if (existingAdmin == null)
                        return String.Format("Учетная запись {0} не может быть удалена, т.к. в системе не останется ни одного активного администратора.", UserLogin);
                }
            }

            return "";
        }
    }
}
