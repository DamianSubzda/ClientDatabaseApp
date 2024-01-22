using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.DataModel.hvacclients
{
    public class Follow_ups
    {
        [Key]
        public int idFollow_ups { get; set; }
        public int idclients { get; set; }
        public virtual clients client { get; set; }
    }
}
