using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class AddMovieDto
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public FileUploadDto Poster { get; set; } // No more IFormFile!
        public string TrailerUrl { get; set; }
    }
}
