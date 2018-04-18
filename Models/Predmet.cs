namespace StudentExams.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Predmet")]
    public partial class Predmet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Predmet()
        {
            ExamList = new HashSet<ExamList>();
            StudentWork = new HashSet<StudentWork>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamList> ExamList { get; set; }

        public virtual ICollection<StudentWork> StudentWork { get; set; }

        public void InitNewObject()
        {
            //
        }


        public string Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "Необходимо ввести наименование предмета.";

            return "";
        }
    }
}
