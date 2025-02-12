using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class AddTheatreDto
    {
        public string TheatreName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public FileUploadDto TheatreImage { get; set; } // Replaces IFormFile
    }
}
