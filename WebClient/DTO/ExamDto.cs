namespace WebClient.DTO
{
    public class ExamDto
    {
        public string Name { get; set; }
        public int Time { get; set; }
        public int Quantity { get; set; }
        public int PassScore { get; set; }
        public List<int> QuestionIds { get; set; }
    }
}
