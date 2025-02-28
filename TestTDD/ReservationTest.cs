using Moq;
using TDD.Exceptions;
using TDD.objects;
using TDD.Repositories;

namespace TestTDD;

[TestClass]
public class ReservationTest
{
    private Mock<IReservationRepository> _mockReservationRepository;
    private ReservationService _reservationService;
    private Mock<IAdherentRepository> _mockAdherentRepository;

    [TestInitialize]
    public void Setup()
    {
        _mockReservationRepository = new Mock<IReservationRepository>();
        _mockAdherentRepository = new Mock<IAdherentRepository>();
        _reservationService = new ReservationService(_mockReservationRepository.Object, _mockAdherentRepository.Object);
    }

    [TestMethod]
    public void AjouterReservation_LimiteAtteinte_LèveException()
    {
        Adherent adherent = new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        _mockAdherentRepository.Setup(repo => repo.GetReservationsOuvertes(adherent.CodeAdherent))
            .Returns(new List<Reservation>
            {
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1))
            });

        new ReservationService(_mockReservationRepository.Object, _mockAdherentRepository.Object);

        Assert.ThrowsException<ReservationLimitExceededException>(() =>
            _reservationService.AjouterReservation(adherent, DateTime.Now.AddMonths(2))
        );
    }

    [TestMethod]
    public void AjouterReservation_LimiteNonAtteinte_AjouteReservationAvecSucces()
    {
        Adherent adherent = new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        _mockAdherentRepository.Setup(repo => repo.GetReservationsOuvertes(adherent.CodeAdherent))
            .Returns(new List<Reservation>
            {
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1))
            });

        _mockReservationRepository.Setup(repo => repo.Add(It.IsAny<Reservation>()));

        string result = _reservationService.AjouterReservation(adherent, DateTime.Now.AddMonths(2));

        Assert.AreEqual("Réservation ajoutée avec succès.", result);
        _mockReservationRepository.Verify(repo => repo.Add(It.IsAny<Reservation>()), Times.Once);
    }


    [TestMethod]
    public void GetReservationsOuvertes_RetourneListeReservationsNonCloturees()
    {
        List<Reservation> reservations = new List<Reservation>
        {
            new Reservation(new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur),
                DateTime.Now.AddMonths(1)),
            new Reservation(new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur),
                DateTime.Now.AddMonths(2))
        };

        _mockReservationRepository.Setup(repo => repo.GetReservationsOuvertes()).Returns(reservations);

        List<Reservation> result = _reservationService.GetReservationsOuvertes();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void EnvoyerRappel_AdherentAvecReservationsDepassees_EnvoiEmail()
    {
        Adherent adherent = new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        List<Reservation> reservationsDepassees = new List<Reservation>
        {
            new Reservation(adherent, DateTime.Now.AddDays(-1))
        };

        _mockAdherentRepository.Setup(repo => repo.GetReservationsDepassees(adherent.CodeAdherent))
            .Returns(reservationsDepassees);

        //Capture la sortie console pour vérifier l'envoi de l'email
        StringWriter output = new StringWriter();
        Console.SetOut(output);

        _reservationService.EnvoyerRappel(adherent);

        Assert.IsTrue(output.ToString().Contains("Envoi d'un rappel pour les réservations suivantes"));
    }

    [TestMethod]
    public void AjouterReservation_AdherentNull_ThrowsAdherentNotFoundException()
    {
        Assert.ThrowsException<AdherentNotFoundException>(() =>
            _reservationService.AjouterReservation(null, DateTime.Now.AddDays(10))
        );
    }

    [TestMethod]
    public void AjouterReservation_DateLimiteInvalide_ThrowsInvalidReservationDateException()
    {
        Adherent adherent = new Adherent("A001", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        Assert.ThrowsException<InvalidReservationDateException>(() =>
            _reservationService.AjouterReservation(adherent,
                DateTime.Now.AddMonths(5))
        );
    }

    [TestMethod]
    public void AjouterReservation_LimiteDeReservationAtteinte_ThrowsReservationLimitExceededException()
    {
        Adherent adherent = new Adherent("A001", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        _mockAdherentRepository.Setup(repo => repo.GetReservationsOuvertes(adherent.CodeAdherent))
            .Returns(new List<Reservation>
            {
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1))
            });

        Assert.ThrowsException<ReservationLimitExceededException>(() =>
            _reservationService.AjouterReservation(adherent, DateTime.Now.AddDays(10))
        );
    }
}