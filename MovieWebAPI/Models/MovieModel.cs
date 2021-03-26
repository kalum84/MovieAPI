using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieWebAPI.Models
{
    public class MovieModel
    {
        public int movieId { get; set; }
        public string title { get; set; }
        public string language { get; set; }
        public string duration { get; set; }
        public int releaseYear { get; set; }
    }

}
