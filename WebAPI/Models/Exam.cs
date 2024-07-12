using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class Exam
    {
        public Exam()
        {
            AccountExams = new HashSet<AccountExam>();
            Questions = new HashSet<Question>();
        }

        public int ExamId { get; set; }
        public string Name { get; set; } = null!;
        public int Time { get; set; }
        public int Quantity { get; set; }
        public int PassScore { get; set; }

        public virtual ICollection<AccountExam> AccountExams { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
