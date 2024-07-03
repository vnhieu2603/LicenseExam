namespace WebClient.Models
{
    public class Progress
    {
        public int ProgressId { get; set; }
        public int? UserExamId { get; set; }
        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public bool IsCorrect { get; set; }

        public virtual Answer? Answer { get; set; }
        public virtual Question? Question { get; set; }
        public virtual AccountExam? UserExam { get; set; }
    }
}
