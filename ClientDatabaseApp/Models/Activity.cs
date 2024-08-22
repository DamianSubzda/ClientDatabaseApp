using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientDatabaseApp.Model
{
    [Table("Activity")]
    public class Activity
    {
        public Activity()
        {
            DateOfCreation = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityId { get; set; }
        [Required]
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }
        [MaxLength(2000, ErrorMessage = "Must be 2000 characters or less!")]
        public string Note { get; set; }
        public DateTime DateOfCreation { get; set; }
        public DateTime? DateOfAction { get; set; }

    }
}
