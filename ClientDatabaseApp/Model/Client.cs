using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientDatabaseApp.Model
{
    [Table("Client")]
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        [MaxLength(200)]
        public string ClientName { get; set; }
        [MaxLength(30)]
        public string Phonenumber { get; set; }
        [MaxLength(40)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(1000)]
        public string Facebook { get; set; }
        [MaxLength(1000)]
        public string Instagram { get; set; }
        [MaxLength(1000)]
        public string PageURL { get; set; }
        public DateTime? Data { get ; set; }
        [MaxLength(50)]
        public string Owner { get; set; }
        [MaxLength(2000)]
        public string Note { get; set; }
        public int Status { get; set; }

        public virtual List<FollowUp> FollowUp { get; set; }

    }
}
