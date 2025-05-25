using System.Linq.Expressions;
using Caker.Data;
using Caker.Models;

namespace Caker.Repositories
{
    public class RefreshTokenRepository(CakerDbContext context)
        : BaseRepository<RefreshToken>(context)
    {
        protected override Expression<Func<RefreshToken, object?>>[] GetIncludes()
        {
            return [rt => rt.User!];
        }

        public async Task<RefreshToken?> GetByToken(string token)
        {
            return await GetBy(rt => rt.Token.Equals(token));
        }

        public async Task RevokeToken(string token)
        {
            var refreshToken = await GetByToken(token);
            if (refreshToken != null && refreshToken.Revoked == null)
            {
                refreshToken.Revoked = DateTime.UtcNow;
                await Update(refreshToken);
            }
        }

        public async Task RevokeUserTokens(int userId)
        {
            var tokens = await GetWhere(rt => rt.UserId == userId && rt.Revoked == null);
            foreach (var token in tokens)
            {
                token.Revoked = DateTime.UtcNow;
                await Update(token);
            }
        }
    }
}
