namespace WebClient.DTO
{
    public class AnswerDTO
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; } = null!;
        public bool IsCorrect { get; set; }
    }
}
