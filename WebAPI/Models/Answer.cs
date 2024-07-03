using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class Answer
    {
        public Answer()
        {
            Progresses = new HashSet<Progress>();
        }

        public int AnswerId { get; set; }
        public int? QuestionId { get; set; }
        public string AnswerText { get; set; } = null!;
        public bool IsCorrect { get; set; }

        public virtual Question? Question { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}
