using CsvHelper.Configuration.Attributes;
using System;
namespace ClientDatabaseApp
{
    public class ClientData
    {
        // Dodaj właściwości odpowiadające kolumnom w pliku CSV
        [Name("Klient")]
        public string Klient { get; set; }
        [Name("Link do strony")]
        public string StronaURL { get; set; }
        [Name("Telefon")]
        public string Telefon { get; set; }
        [Name("Fb/Ig")]
        public string FacebookURL { get; set; }
        [Name("Miasto")]
        public string Miasto { get; set; }
        [Name("Data")]
        public string Data { get; set; }
        [Name("Imię właściciela")]
        public string Wlasciciel { get; set; }
        [Name("Follow up 1")]
        public string FollowUp1 { get; set; }
        [Name("Follow up 2")]
        public string FollowUp2 { get; set; }
        [Name("Notatki")]
        public string Notatki { get; set; }
    }
}

