using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public partial class QuestionType
    {
        public QuestionType()
        {
            Questions = new HashSet<Question>();
        }

        public int TypeId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; }
    }
}
