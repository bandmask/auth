using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace Auth.Models
{
    public class AppUser : IdentityUser
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        // public List<MongoUserClaim> Claims { get; set; } = new List<MongoUserClaim>();
        // public List<Claim> Logins { get; set; } = new List<Claim>();

        public AppUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            Id = ObjectId.GenerateNewId().ToString();
            UserName = userName;
            CreatedOn = DateTime.Now;
        }

        public AppUser(string userName, string email) : this(userName)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            Email = email;
        }
    }
}
