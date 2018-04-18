namespace StudentExams.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentWork")]
    public partial class StudentWork
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public int StudentId { get; set; }

        public DateTime WorkDate { get; set; }

        public int WorkTypeId { get; set; }

        [Column(TypeName = "image")]
        public byte[] WorkFile { get; set; }
        public string WorkFileName { get; set; }

        public virtual User Student { get; set; }

        public virtual User Teacher { get; set; }

        public string WorkStatus { get; set; }

        public string TeacherComments { get; set; }

        public virtual WorkType WorkType { get; set; }

        public int PredmetId { get; set; }

        public virtual Predmet Predmet { get; set; }

        public void InitNewObject()
        {
            WorkDate = DateTime.Now;
            WorkStatus = StudentWorkStatus.DRAFT;
            StudentId = Authentication.User.UserId;
        }


        public string Validate()
        {
            if (PredmetId == 0)
            {
                return "Необходимо указать предмет.";
            }
            if (WorkTypeId == 0)
            {
                return "Необходимо указать вид работы.";
            }
            if (TeacherId == 0)
            {
                return "Необходимо указать преподавателя.";
            }
            if (WorkTypeId == 0)
            {
                return "Необходимо указать вид работы.";
            }
            if (string.IsNullOrEmpty(WorkStatus))
            {
                return "Не указан статус работы.";
            }
            if (WorkStatus == StudentWorkStatus.REJECTED && string.IsNullOrWhiteSpace(TeacherComments))
            {
                return "Необходимо указать комментарий о причине возврата работы.";
            }
            return "";
        }
    }
}
