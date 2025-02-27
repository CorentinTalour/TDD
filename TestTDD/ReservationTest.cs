using Moq;
using TDD.Exceptions;
using TDD.objects;
using TDD.Repositories;

namespace TestTDD;

[TestClass]
public class ReservationTest
{
    [TestMethod]
    public void AjouterReservation_LimiteAtteinte_LèveException()
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

        Assert.ThrowsException<ReservationLimitExceededException>(() =>
            service.AjouterReservation(adherent, DateTime.Now.AddMonths(2))
        );
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

        ReservationService service =
            new ReservationService(mockReservationRepository.Object, mockAdherentRepository.Object);

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
        StringWriter output = new StringWriter();
        Console.SetOut(output);

        service.EnvoyerRappel(adherent);

        Assert.IsTrue(output.ToString().Contains("Envoi d'un rappel pour les réservations suivantes"));
    }

    [TestMethod]
    public void AjouterReservation_AdherentNull_ThrowsAdherentNotFoundException()
    {
        var mockReservationRepository = new Mock<IReservationRepository>();
        var mockAdherentRepository = new Mock<IAdherentRepository>();
        var reservationService =
            new ReservationService(mockReservationRepository.Object, mockAdherentRepository.Object);

        Assert.ThrowsException<AdherentNotFoundException>(() =>
            reservationService.AjouterReservation(null, DateTime.Now.AddDays(10))
        );
    }

    [TestMethod]
    public void AjouterReservation_DateLimiteInvalide_ThrowsInvalidReservationDateException()
    {
        var mockReservationRepository = new Mock<IReservationRepository>();
        var mockAdherentRepository = new Mock<IAdherentRepository>();
        var reservationService =
            new ReservationService(mockReservationRepository.Object, mockAdherentRepository.Object);

        Adherent adherent = new Adherent("A001", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        Assert.ThrowsException<InvalidReservationDateException>(() =>
            reservationService.AjouterReservation(adherent,
                DateTime.Now.AddMonths(5))
        );
    }

    [TestMethod]
    public void AjouterReservation_LimiteDeReservationAtteinte_ThrowsReservationLimitExceededException()
    {
        var mockReservationRepository = new Mock<IReservationRepository>();
        var mockAdherentRepository = new Mock<IAdherentRepository>();
        var reservationService =
            new ReservationService(mockReservationRepository.Object, mockAdherentRepository.Object);

        Adherent adherent = new Adherent("A001", "John", "Doe", DateTime.Now, Civilite.Monsieur);

        mockAdherentRepository.Setup(repo => repo.GetReservationsOuvertes(adherent.CodeAdherent))
            .Returns(new List<Reservation>
            {
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1)),
                new Reservation(adherent, DateTime.Now.AddMonths(1))
            });

        Assert.ThrowsException<ReservationLimitExceededException>(() =>
            reservationService.AjouterReservation(adherent, DateTime.Now.AddDays(10))
        );
    }
}