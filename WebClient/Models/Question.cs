namespace WebClient.Models
{
    public class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
            Progresses = new HashSet<Progress>();
            Exams = new HashSet<Exam>();
            Images = new HashSet<Image>();
        }

        public int QuestionId { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCritical { get; set; }
        public int? TypeId { get; set; }

        public virtual QuestionType? Type { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }

        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
