using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class UpdateTheatreDto
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string CurrentImage { get; set; } // Old image path
        public FileUploadDto NewImage { get; set; } // New image file
    }
}
