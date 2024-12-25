using System.ComponentModel.DataAnnotations;

namespace Project__Mini_Stack_Overflow.Models.ViewModels
{
    public class QuestionVM
    {
        public QuestionVM()
        {
            this.QuestionlList = new List<int>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public long AnswerCount { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<int> QuestionlList { get; set; }

    }
}
