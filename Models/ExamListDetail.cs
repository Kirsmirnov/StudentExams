namespace StudentExams.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExamListDetail")]
    public partial class ExamListDetail
    {
        public int Id { get; set; }

        public int ExamListId { get; set; }

        public int StudentId { get; set; }

        [StringLength(255)]
        public string ExamResult { get; set; }

        public virtual ExamList ExamList { get; set; }

        public virtual User Student { get; set; }
    }
}
