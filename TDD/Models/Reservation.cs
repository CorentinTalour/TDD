namespace TDD.Models
{
    public class Reservation
    {
        public string ReservationCode { get; set; }
        public Member Member { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsClosed { get; set; }

        public Reservation(Member member, DateTime dueDate)
        {
            Member = member;
            ReservationDate = DateTime.Now;
            DueDate = dueDate;
            IsClosed = false;
        }
    }
}