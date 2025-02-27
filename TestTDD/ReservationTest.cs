using Moq;
using TDD.objects;
using TDD.Repositories;

namespace TestTDD;

[TestClass]
public class ReservationTest
{
    [TestMethod]
    public void AjouterReservation_LimiteAtteinte_RetourneMessageLimiteAtteinte()
    {
        var mockReservationRepository = new Mock<IReservationRepository>();
        var mockAdherentRepository = new Mock<IAdherentRepository>();
        Adherent adherent = new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        mockAdherentRepository.Setup(repo => repo.GetReservationsOuvertes(adherent.CodeAdherent))
            .Returns(new List<Reservation>
            {
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1))
            });

        ReservationService service =
            new ReservationService(mockReservationRepository.Object, mockAdherentRepository.Object);

        string result = service.AjouterReservation(adherent, DateTime.Now.AddMonths(2));

        Assert.AreEqual("Limite de 3 réservations ouvertes atteinte.", result);
    }

    [TestMethod]
    public void AjouterReservation_LimiteNonAtteinte_AjouteReservationAvecSucces()
    {
        var mockReservationRepository = new Mock<IReservationRepository>();
        var mockAdherentRepository = new Mock<IAdherentRepository>();
        Adherent adherent = new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        mockAdherentRepository.Setup(repo => repo.GetReservationsOuvertes(adherent.CodeAdherent))
            .Returns(new List<Reservation>
            {
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1))
            });

        mockReservationRepository.Setup(repo => repo.Add(It.IsAny<Reservation>()));

        var service = new ReservationService(mockReservationRepository.Object, mockAdherentRepository.Object);

        string result = service.AjouterReservation(adherent, DateTime.Now.AddMonths(2));

        Assert.AreEqual("Réservation ajoutée avec succès.", result);
        mockReservationRepository.Verify(repo => repo.Add(It.IsAny<Reservation>()), Times.Once);
    }


    [TestMethod]
    public void GetReservationsOuvertes_RetourneListeReservationsNonCloturees()
    {
        var mockReservationRepository = new Mock<IReservationRepository>();
        var mockAdherentRepository = new Mock<IAdherentRepository>();

        List<Reservation> reservations = new List<Reservation>
        {
            new Reservation(new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur),
                DateTime.Now.AddMonths(1)),
            new Reservation(new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur),
                DateTime.Now.AddMonths(2))
        };

        mockReservationRepository.Setup(repo => repo.GetReservationsOuvertes()).Returns(reservations);

        ReservationService service =
            new ReservationService(mockReservationRepository.Object, mockAdherentRepository.Object);

        List<Reservation> result = service.GetReservationsOuvertes();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void EnvoyerRappel_AdherentAvecReservationsDepassees_EnvoiEmail()
    {
        var mockReservationRepository = new Mock<IReservationRepository>();
        var mockAdherentRepository = new Mock<IAdherentRepository>();
        Adherent adherent = new Adherent("A123", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        List<Reservation> reservationsDepassees = new List<Reservation>
        {
            new Reservation(adherent, DateTime.Now.AddDays(-1))
        };

        mockAdherentRepository.Setup(repo => repo.GetReservationsDepassees(adherent.CodeAdherent))
            .Returns(reservationsDepassees);

        ReservationService service =
            new ReservationService(mockReservationRepository.Object, mockAdherentRepository.Object);

        //Capture la sortie console pour vérifier l'envoi de l'email
        var output = new StringWriter();
        Console.SetOut(output);

        service.EnvoyerRappel(adherent);

        Assert.IsTrue(output.ToString().Contains("Envoi d'un rappel pour les réservations suivantes"));
    }

    [TestMethod]
    [ExpectedException(typeof(AdherentNotFoundException))]
    public void AjouterReservation_AdherentNull_ThrowsAdherentNotFoundException()
    {
        _reservationService.AjouterReservation(null, DateTime.Now.AddDays(10));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidReservationDateException))]
    public void AjouterReservation_DateLimiteInvalide_ThrowsInvalidReservationDateException()
    {
        Adherent adherent = new Adherent { CodeAdherent = "A001", Reservations = new List<Reservation>() };

        _reservationService.AjouterReservation(adherent, DateTime.Now.AddMonths(5));
    }

    [TestMethod]
    [ExpectedException(typeof(ReservationLimitExceededException))]
    public void AjouterReservation_LimiteDeReservationAtteinte_ThrowsReservationLimitExceededException()
    {
        Adherent adherent = new Adherent { CodeAdherent = "A001" };

        _mockAdherentRepository
            .Setup(repo => repo.GetReservationsOuvertes(adherent.CodeAdherent))
            .Returns(new List<Reservation>
            {
                new Reservation(adherent, DateTime.Now), new Reservation(adherent, DateTime.Now),
                new Reservation(adherent, DateTime.Now)
            });

        _reservationService.AjouterReservation(adherent, DateTime.Now.AddDays(10));
    }
}