using WebClient.Models;

namespace WebClient.DTO
{
    public class AccountExamDTO
    {
        public AccountExamDTO()
        {
            Progresses = new HashSet<Progress>();
        }

        public int UserExamId { get; set; }
        public int? ExamId { get; set; }
        public string ExamName { get; set; }
        public double? Score { get; set; }
        public double? PassScore { get; set; }
        public int Quantity { get; set; }

        public virtual Exam? Exam { get; set; }
        public virtual Account? User { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}
