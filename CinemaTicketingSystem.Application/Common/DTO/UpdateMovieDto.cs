using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class UpdateMovieDto
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public string PosterPath { get; set; } // For old poster
        public FileUploadDto NewPoster { get; set; } // New poster file
        public string TrailerUrl { get; set; }
    }
}
