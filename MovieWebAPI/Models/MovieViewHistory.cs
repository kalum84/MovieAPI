using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieWebAPI.Models
{
    public class MovieViewHistory: CsvableBase
    {
        public int movieId { get; set; }
        public int watchDurationMs { get; set; }

    }
}
