using Moq;
using TDD.Exceptions;
using TDD.Models;
using TDD.Repositories.Interfaces;
using TDD.services;

namespace TestTDD;

[TestClass]
public class ReservationTest
{
    private Mock<IReservationRepository>? _mockReservationRepository;
    private ReservationService? _reservationService;
    private Mock<IMemberRepository>? _mockAdherentRepository;

    [TestInitialize]
    public void Setup()
    {
        _mockReservationRepository = new Mock<IReservationRepository>();
        _mockAdherentRepository = new Mock<IMemberRepository>();
        _reservationService = new ReservationService(_mockReservationRepository.Object, _mockAdherentRepository.Object);
    }

    [TestMethod]
    public void GivenMemberWithAvailableSlots_WhenAddingReservation_ShouldAddReservationSuccessfully()
    {
        Member member = new Member("A100", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        _mockAdherentRepository?.Setup(repo => repo.GetReservationsOuvertes(member.MemberCode))
            .Returns(new List<Reservation>
            {
                new Reservation(member, DateTime.Now.AddMonths(1)),
                new Reservation(member, DateTime.Now.AddMonths(1))
            });

        _mockReservationRepository?.Setup(repo => repo.Add(It.IsAny<Reservation>()));

        string? result = _reservationService?.AddReservation(member, DateTime.Now.AddMonths(2));

        Assert.AreEqual("Réservation ajoutée avec succès.", result);
        _mockReservationRepository?.Verify(repo => repo.Add(It.IsAny<Reservation>()), Times.Once);
    }


    [TestMethod]
    public void GivenReservationsInSystem_WhenGettingOpenReservations_ShouldReturnListOfOpenReservations()
    {
        List<Reservation> reservations = new List<Reservation>
        {
            new Reservation(new Member("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur),
                DateTime.Now.AddMonths(1)),
            new Reservation(new Member("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur),
                DateTime.Now.AddMonths(2))
        };

        _mockReservationRepository?.Setup(repo => repo.GetReservationsOuvertes()).Returns(reservations);

        List<Reservation>? result = _reservationService?.GetOpenReservations();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GivenMemberWithOverdueReservations_WhenSendingReminder_ShouldSendEmail()
    {
        Member member = new Member("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        // La réservation doit être en retard de **plus de 4 mois**
        List<Reservation> overdueReservations = new List<Reservation>
        {
            new Reservation(member, DateTime.Now.AddMonths(-5).Date)
                { ReservationCode = "RES001" }
        };

        _mockAdherentRepository?.Setup(repo => repo.GetReservationsDepassees(member.MemberCode))
            .Returns(overdueReservations);

        // Capture la sortie console pour vérifier l'envoi de l'email
        StringWriter output = new StringWriter();
        Console.SetOut(output);

        Console.WriteLine($"Test - Nombre de réservations dépassées : {overdueReservations.Count}");
        Console.WriteLine(
            $"Test - Dates des réservations : {string.Join(", ", overdueReservations.Select(r => r.ReservationDate))}");

        // Appel de la méthode pour envoyer le rappel
        _reservationService?.SendReminder(member);

        string consoleOutput = output.ToString();

        // Vérification du message attendu
        Assert.IsTrue(consoleOutput.Contains("Envoi d'un rappel pour les réservations suivantes : RES001"));
    }

    [TestMethod]
    public void GivenMemberWithRecentOverdueReservations_WhenSendingReminder_ShouldNotSendEmail()
    {
        Member member = new Member("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        List<Reservation> overdueReservations = new List<Reservation>
        {
            new Reservation(member, DateTime.Now.AddMonths(-5)) { ReservationCode = "RES002" }
        };

        _mockAdherentRepository?.Setup(repo => repo.GetReservationsDepassees(member.MemberCode))
            .Returns(overdueReservations);

        StringWriter output = new StringWriter();

        _reservationService?.SendReminder(member);

        string consoleOutput = output.ToString();

        Assert.IsFalse(consoleOutput.Contains("Envoi d'un rappel"));
    }

    [TestMethod]
    public void GivenNullMember_WhenAddingReservation_ShouldThrowAdherentNotFoundException()
    {
        Assert.ThrowsException<MemberNotFoundException>(() =>
            _reservationService?.AddReservation(null, DateTime.Now.AddDays(10))
        );
    }

    [TestMethod]
    public void GivenMemberWithInvalidReservationDate_WhenAddingReservation_ShouldThrowInvalidReservationDateException()
    {
        Member member = new Member("A001", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        Assert.ThrowsException<InvalidReservationDateException>(() =>
            _reservationService?.AddReservation(member,
                DateTime.Now.AddMonths(5))
        );
    }

    [TestMethod]
    public void GivenMemberWithMaxReservations_WhenAddingReservation_ShouldThrowReservationLimitExceededException()
    {
        Member member = new Member("A001", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        _mockAdherentRepository?.Setup(repo => repo.GetReservationsOuvertes(member.MemberCode))
            .Returns(new List<Reservation>
            {
                new Reservation(member, DateTime.Now.AddMonths(1)),
                new Reservation(member, DateTime.Now.AddMonths(1)),
                new Reservation(member, DateTime.Now.AddMonths(1))
            });

        Assert.ThrowsException<ReservationLimitExceededException>(() =>
            _reservationService?.AddReservation(member, DateTime.Now.AddDays(10))
        );
    }
}