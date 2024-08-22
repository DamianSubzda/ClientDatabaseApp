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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }
        [MaxLength(200, ErrorMessage = "Client name must be 200 characters or less!")]
        public string ClientName { get; set; }
        [MaxLength(30, ErrorMessage = "Phonenumber must be 30 characters or less!")]
        public string Phonenumber { get; set; }
        [MaxLength(40, ErrorMessage = "Email must be 40 characters or less!")]
        public string Email { get; set; }
        [MaxLength(50, ErrorMessage = "City must be 50 characters or less!")]
        public string City { get; set; }
        [MaxLength(1000, ErrorMessage = "Facebook must be 1000 characters or less!")]
        public string Facebook { get; set; }
        [MaxLength(1000, ErrorMessage = "Instagram must be 1000 characters or less!")]
        public string Instagram { get; set; }
        [MaxLength(1000, ErrorMessage = "PageURL must be 1000 characters or less!")]
        public string PageURL { get; set; }
        public DateTime? Data { get ; set; }
        [MaxLength(50, ErrorMessage = "Owner name must be 50 characters or less!")]
        public string Owner { get; set; }
        [MaxLength(2000, ErrorMessage = "Note must be 2000 characters or less!")]
        public string Note { get; set; }
        public int Status { get; set; }

        public virtual List<Activity> Activities { get; set; } = new List<Activity>();

    }
}
