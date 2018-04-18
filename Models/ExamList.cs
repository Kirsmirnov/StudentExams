namespace StudentExams.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExamList")]
    public partial class ExamList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExamList()
        {
            ExamListDetail = new List<ExamListDetail>();
        }

        public int Id { get; set; }

        public DateTime ExamDate { get; set; }

        public int ExamTypeId { get; set; }

        public int TeacherId { get; set; }

        public int StudentGroupId { get; set; }

        public int PredmetId { get; set; }
       
        public virtual ExamType ExamType { get; set; }

        public virtual Predmet Predmet { get; set; }

        public virtual StudentGroup StudentGroup { get; set; }

        public virtual User Teacher { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<ExamListDetail> ExamListDetail { get; set; }


        public void InitNewObject()
        {
            ExamDate = DateTime.Now;
            TeacherId = Authentication.User.UserId;
        }


        public string Validate()
        {
            if (PredmetId == 0)
                return "Необходимо указать предмет";
            if (ExamTypeId == 0)
                return "Необходимо указать тип ведомости";
            if (TeacherId == 0)
                return "Не задан преподаватель";
            if (StudentGroupId == 0)
                return "Не указана группа";

            return "";
        }
    }
}
