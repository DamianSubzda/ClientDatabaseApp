﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientDatabaseApp.Model
{
    [Table("FollowUp")]
    public class FollowUp
    {
        [Key]
        public int FollowUpId { get; set; }
        [Required]
        public int ClientId { get; set; }
        [MaxLength(2000)]
        public string Note { get; set; }
        public DateTime DateOfCreation { get; set; }
        public DateTime DateOfAction { get; set; }
        public virtual Client Client { get; set; }
    }
}
