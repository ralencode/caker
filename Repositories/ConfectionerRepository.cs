using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;

namespace Caker.Repositories
{
    public class ConfectionerRepository(CakerDbContext context) : BaseRepository<Confectioner>(context)
    {
        protected override Expression<Func<Confectioner, object?>>[] GetIncludes()
        {
            return [c => c.User, c => c.Feedbacks, c => c.Cakes];
        }
    }
}