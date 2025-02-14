using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class ContactFormVmDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string Subject { get; set; }
        [Required]
        [StringLength(500)]
        public string Message { get; set; }
        [Required]
        [EmailAddress]
        public string AdminEmail { get; set; }
    }
}
