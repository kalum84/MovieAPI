using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieWebAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MovieWebAPI.Controllers
{
    [ApiController]
    public class MovieController : ControllerBase
    {

        [HttpPost]
        [Route("metadata")]
        public ActionResult<Movie> AddMovie([FromBody] MovieModel movie)
        {
            var cr = new CsvReader<Movie>();
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Resources/metadata.csv";
            var movies = cr.Read(path, true);
            int lastId = movies.OrderByDescending(m => m.Id).FirstOrDefault().Id;
            var m = new Movie
            {
                Id = lastId,
                MovieId = movie.movieId,
                Duration = movie.duration,
                Language = movie.language,
                ReleaseYear = movie.releaseYear,
                Title = movie.title,
            };

            var cw = new CsvWriter<Movie>();
            cw.Write(m, path);
            return Ok();
        }

        [HttpGet]
        [Route("metadata/{movieId}")]
        public ActionResult<Movie> GetMovieById(int movieId)
        {
            var cr = new CsvReader<Movie>();
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Resources/metadata.csv";
            var movies = cr.Read(path, true);
            if (movies != null)
            {
                var result = movies.Where(m => m.MovieId == movieId)
                                        .GroupBy(m => m.Language, (key, g) => g.OrderByDescending(e => e.Id).First())
                                        .Select(movie => new 
                                        { 
                                            movieId = movie.MovieId,
                                            title = movie.Title, 
                                            language = movie.Language, 
                                            duration =movie.Duration, 
                                            releaseYear = movie.ReleaseYear
                                        }).OrderBy(m => m.language);
                if (result.Any())
                {
                    return Ok(result); 
                }
                else
                {
                    return NotFound();

                }
            }
            return NotFound();
        }

        [HttpGet]
        [Route("movies/stats")]
        public ActionResult<Movie> GetMovieStats()
        {
            var crMovies = new CsvReader<Movie>();
            var crStats = new CsvReader<MovieViewHistory>();
            var movies = crMovies.Read(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Resources/metadata.csv", true);
            var stats = crStats.Read(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Resources/stats.csv", true);
            if (movies != null && stats != null)
            {
                var statsGrouped = stats.GroupBy(m => m.movieId).Select(group => new
                {
                    movieId = group.Key,
                    averageWatchDurationS = group.Average(p => p.watchDurationMs),
                    watches = group.Count()
                });

                var moviesGrouped = movies.GroupBy(m => m.MovieId, (key, g) => g.OrderByDescending(e => e.Id).First()).Select(group => new
                {
                    movieId = group.MovieId,
                    title = group.Title,
                    releaseYear = group.ReleaseYear
                });

                return Ok(from s in statsGrouped
                             join m in moviesGrouped on s.movieId equals m.movieId
                             orderby s.watches, m.releaseYear
                             select new 
                             { 
                                 movieId = s.movieId, 
                                 title = m.title,
                                 averageWatchDurationS = s.averageWatchDurationS,
                                 watches = s.watches,
                                 releaseYear = m.releaseYear
                             });
            }
            return NotFound();
        }
    }
}
