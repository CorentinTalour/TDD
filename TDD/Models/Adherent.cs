using System;
using System.Collections.Generic;

namespace TDD.objects
{
    public class Adherent
    {
        public string CodeAdherent { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime DateNaissance { get; set; }
        public Civilite Civilite { get; set; }

        public List<Reservation> Reservations { get; set; } = new List<Reservation>();

        public Adherent(string codeAdherent, string nom, string prenom, DateTime dateNaissance, Civilite civilite)
        {
            CodeAdherent = codeAdherent;
            Nom = nom;
            Prenom = prenom;
            DateNaissance = dateNaissance;
            Civilite = civilite;
        }
    }

    public enum Civilite
    {
        Monsieur,
        Mme
    }
}