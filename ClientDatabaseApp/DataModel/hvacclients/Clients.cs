using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.DataModel.hvacclients
{
    public class clients
    {
        [Key]
        public int idclients { get; set; }
        public string Klient { get; set; }
        public string StronaURL { get; set; }
        public string Telefon { get; set; }
        public string FacebookURL { get; set; }
        public string Miasto { get; set; }
        public DateTime? Data { get; set; }
        public string Wlasciciel { get; set; }
        public string FollowUp1 { get; set; }
        public string FollowUp2 { get; set; }
        public string Notatki { get; set; }

        public virtual List<Follow_ups> Follow_Ups { get; set; }

    }
}
