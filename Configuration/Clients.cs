using IdentityServer4.Models;
using System.Collections.Generic;

namespace KidsPrize.Configuration
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "web",
                    ClientName = "Web Client",
                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = new List<string>
                    {
                        "http://localhost:5000",
                        "https://www.getpostman.com/oauth2/callback"
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:5000/"
                    },

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        "api1"
                    },

                    AllowAccessTokensViaBrowser = true
                }
            };
        }
    }
}