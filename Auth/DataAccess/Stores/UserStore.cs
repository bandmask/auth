using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Auth.DataAccess.Contexts;
using Auth.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Auth.DataAccess.Stores
{
    public class UserStore : IUserStore<AppUser>, IUserPasswordStore<AppUser>
    {
        private readonly UserContext _userContext;

        public UserStore(UserContext userContext)
        {
            _userContext = userContext;
        }

        public void Dispose() { }

        public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _userContext.Users.InsertOneAsync(user, cancellationToken : cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            user.DeletedOn = DateTime.Now;
            var query = Builders<AppUser>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<AppUser>.Update.Set(u => u.DeletedOn, user.DeletedOn);

            await _userContext.Users.UpdateOneAsync(query, update, cancellationToken : cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;
        }

        public Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var query = Builders<AppUser>.Filter.And(
                Builders<AppUser>.Filter.Eq(u => u.Id, userId),
                Builders<AppUser>.Filter.Eq(u => u.DeletedOn, null)
            );

            return _userContext.Users.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedUserName))
            {
                throw new ArgumentNullException(nameof(normalizedUserName));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var query = Builders<AppUser>.Filter.And(
                Builders<AppUser>.Filter.Eq(u => u.NormalizedUserName, normalizedUserName),
                Builders<AppUser>.Filter.Eq(u => u.DeletedOn, null)
            );

            return _userContext.Users.Find(query).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            user.NormalizedUserName = normalizedName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var query = Builders<AppUser>.Filter.And(
                Builders<AppUser>.Filter.Eq(u => u.Id, user.Id),
                Builders<AppUser>.Filter.Eq(u => u.DeletedOn, null)
            );

            var replaceResult = await _userContext.Users.ReplaceOneAsync(query, user, new UpdateOptions { IsUpsert = false }).ConfigureAwait(false);

            return replaceResult.IsModifiedCountAvailable && replaceResult.ModifiedCount == 1 ?
                IdentityResult.Success :
                IdentityResult.Failed();
        }

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
