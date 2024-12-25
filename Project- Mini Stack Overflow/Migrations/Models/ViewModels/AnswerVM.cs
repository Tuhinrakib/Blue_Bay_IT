namespace Project__Mini_Stack_Overflow.Models.ViewModels
{
    public class AnswerVM
    {
        public AnswerVM()
        {
            this.AnswerlList = new List<int>();
        }
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public List<int> AnswerlList { get; set; }
    }
}
