namespace TDD.Models
{
    public class Member
    {
        public string MemberCode { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Civilite Gender { get; set; }

        public List<Reservation> Reservation { get; set; } = new List<Reservation>();

        public Member()
        {
            Reservation = new List<Reservation>();
        }

        public Member(string memberCode, string lastName, string firstName, DateTime dateOfBirth, Civilite gender)
        {
            MemberCode = memberCode;
            LastName = lastName;
            FirstName = firstName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }
    }

    public enum Civilite
    {
        Monsieur,
        Mme
    }
}