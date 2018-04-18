using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentExams.Models
{
    public static class StudentWorkStatus
    {
        public const string DRAFT = "Черновик";
        public const string CHECKING = "На проверке";
        public const string APPROOVED = "Принято";
        public const string REJECTED = "Возвращено на доработку";
    }
}