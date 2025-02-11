using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.DTO
{
    public class FileUploadDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
    }
}
