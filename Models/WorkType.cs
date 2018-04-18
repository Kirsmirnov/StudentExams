namespace StudentExams.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkType")]
    public partial class WorkType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual List<StudentWork> StudentWork { get; set; }

        public WorkType()
        {
            StudentWork = new List<Models.StudentWork>();
        }
    }
}
