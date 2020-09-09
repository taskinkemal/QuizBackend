using System;
namespace Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int QuizId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Correct { get; set; }

        public int Incorrect { get; set; }

        public decimal Score { get; set; }

        public QuizResult Result { get; set; }
    }
}
