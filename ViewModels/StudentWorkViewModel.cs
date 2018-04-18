using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentExams.ViewModels
{
    public class StudentWorkViewModel
    {
        public int? Id { get; set; }
        public int? TeacherId { get; set; }
        public int? PredmetId { get; set; }
        public int? StudentGroupId { get; set; }
        public int? WorkTypeId { get; set; }
        public string WorkStatus { get; set; }
    }
}