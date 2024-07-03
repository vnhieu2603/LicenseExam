using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class AccountExam
    {
        public AccountExam()
        {
            Progresses = new HashSet<Progress>();
        }

        public int UserExamId { get; set; }
        public int? UserId { get; set; }
        public int? ExamId { get; set; }
        public double? Score { get; set; }

        public virtual Exam? Exam { get; set; }
        public virtual Account? User { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}
