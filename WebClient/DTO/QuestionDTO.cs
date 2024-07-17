using WebClient.Models;

namespace WebClient.DTO
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCritical { get; set; }
        public int? TypeId { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<AnswerDTO> Answers { get; set; } = new List<AnswerDTO>();
    }
}
