using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.Interfaces
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetAll(Expression<Func<Movie, bool>>? filter = null, string includeProperties = null );
        Movie Get(Expression<Func<Movie, bool>> filter, string includeProperties = null );
        void Add(Movie entity);
        void Update(Movie entity);
        void Remove(Movie entity);
        void Save();
    }
}
