using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using KidsPrize.Data;
using KidsPrize.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace KidsPrize
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Authentication
            var openIdCfg = GetOpenIdConfiguration();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.Authority = "https://apperic.auth0.com/";
                opt.Audience = "npeMSLRd6HPDGyegxzhGfnHrkIu83F8B";
                opt.Configuration = openIdCfg;
            });

            // Add Authorization
            services.AddAuthorization(opts =>
            {
                opts.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim("email")
                    .RequireClaim("email_verified", new[] { "true", "True", "1" })
                    .Build();
            });

            // Add DBContext
            services.AddNpgsqlDbContext(Configuration.GetConnectionString("DefaultConnection"));

            services.AddScoped<IChildService, ChildService>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "KidsPrize API", Version = "v1" });
            // });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }

            // app.UseSwagger();
            // app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "KidsPrize API V1"));

            //app.UseHttpsRedirection();
            app.UseNpgsqlDbContext();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // load from local instead of remote
        private static OpenIdConnectConfiguration GetOpenIdConfiguration()
        {
            var cfg = "{\"issuer\":\"https://apperic.auth0.com/\",\"authorization_endpoint\":\"https://apperic.auth0.com/authorize\",\"token_endpoint\":\"https://apperic.auth0.com/oauth/token\",\"userinfo_endpoint\":\"https://apperic.auth0.com/userinfo\",\"mfa_challenge_endpoint\":\"https://apperic.auth0.com/mfa/challenge\",\"jwks_uri\":\"https://apperic.auth0.com/.well-known/jwks.json\",\"registration_endpoint\":\"https://apperic.auth0.com/oidc/register\",\"revocation_endpoint\":\"https://apperic.auth0.com/oauth/revoke\",\"scopes_supported\":[\"openid\",\"profile\",\"offline_access\",\"name\",\"given_name\",\"family_name\",\"nickname\",\"email\",\"email_verified\",\"picture\",\"created_at\",\"identities\",\"phone\",\"address\"],\"response_types_supported\":[\"code\",\"token\",\"id_token\",\"code token\",\"code id_token\",\"token id_token\",\"code token id_token\"],\"code_challenge_methods_supported\":[\"S256\",\"plain\"],\"response_modes_supported\":[\"query\",\"fragment\",\"form_post\"],\"subject_types_supported\":[\"public\"],\"id_token_signing_alg_values_supported\":[\"HS256\",\"RS256\"],\"token_endpoint_auth_methods_supported\":[\"client_secret_basic\",\"client_secret_post\"],\"claims_supported\":[\"aud\",\"auth_time\",\"created_at\",\"email\",\"email_verified\",\"exp\",\"family_name\",\"given_name\",\"iat\",\"identities\",\"iss\",\"name\",\"nickname\",\"phone_number\",\"picture\",\"sub\"],\"request_uri_parameter_supported\":false,\"device_authorization_endpoint\":\"https://apperic.auth0.com/oauth/device/code\"}";
            var keys = "{\"keys\":[{\"alg\":\"RS256\",\"kty\":\"RSA\",\"use\":\"sig\",\"n\":\"0coM3MzRKDCMyL7Fhuh5N_xbCpgR_E6yHWpKA2eSmNmRpHdWzuqy5u1PEgWdj8OGayRmHW3l-5kZWF5ZJmZTcDOwd9krVZmTJYqQ_6ieQsUzoSAc-UlY8JWfb4AhShzQ66JkWpNIuvVH6-zKlaB5myqu94l_kA66ifqBgdJubM6esHT-vUHmgkm2CKmfCTy6YLQaqOwJAv-aOW-PnroKAmXTm1-mCPcz6tKGC579OrmUTXuthsjjo8iaCHq4Wc5JlQkMVwo8bHvCw4dGwE_IaAZ8NzMckLFrAZrwvnIGTxCMlLXRHmJ9ND8LwakBnuLqvMnlbnm-mf-Ju8_yb3XemQ\",\"e\":\"AQAB\",\"kid\":\"MzRGQ0Q0NTQxNkJGNEZGMzRFMzM4NjJDNDgyNENCNkJBMzJCMDFEMQ\",\"x5t\":\"MzRGQ0Q0NTQxNkJGNEZGMzRFMzM4NjJDNDgyNENCNkJBMzJCMDFEMQ\",\"x5c\":[\"MIIC6jCCAdKgAwIBAgIJB+82XFxmQzM+MA0GCSqGSIb3DQEBBQUAMBwxGjAYBgNVBAMTEWFwcGVyaWMuYXV0aDAuY29tMB4XDTE2MTIwNTAzMzA0NVoXDTMwMDgxNDAzMzA0NVowHDEaMBgGA1UEAxMRYXBwZXJpYy5hdXRoMC5jb20wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDRygzczNEoMIzIvsWG6Hk3/FsKmBH8TrIdakoDZ5KY2ZGkd1bO6rLm7U8SBZ2Pw4ZrJGYdbeX7mRlYXlkmZlNwM7B32StVmZMlipD/qJ5CxTOhIBz5SVjwlZ9vgCFKHNDromRak0i69Ufr7MqVoHmbKq73iX+QDrqJ+oGB0m5szp6wdP69QeaCSbYIqZ8JPLpgtBqo7AkC/5o5b4+eugoCZdObX6YI9zPq0oYLnv06uZRNe62GyOOjyJoIerhZzkmVCQxXCjxse8LDh0bAT8hoBnw3MxyQsWsBmvC+cgZPEIyUtdEeYn00PwvBqQGe4uq8yeVueb6Z/4m7z/Jvdd6ZAgMBAAGjLzAtMAwGA1UdEwQFMAMBAf8wHQYDVR0OBBYEFO0tPirpZI26uPdakD1CXF9DnY+PMA0GCSqGSIb3DQEBBQUAA4IBAQCdPZTGzM+lCC03wmsL3kC/vxANIqrk7nL/ArLWDZ6xlkOky7TUKciq5Z+iSk1Ho1zGzVIFOkBDRQBTnH1U+PTCECYdNdMc0M2yePU/5J6jaj9OzUTC/QL+5U7b743agULQ6FqhmPBWs0n6HO8WMdRUS9ZTeMz6pDlUuxb6pUHtqA83F2reDNR4FnbDgQ8NLKpaZCHl7YeO5p02Esg3Tu7akX+GgMmE1RVPjbTVAyv0M+ec4Ui+nWlAnLHHB/+RaLUhV8pBQPxG54FYdQAIxsRmYOEuQVp+D1u1LrSeP2SGn0oO69HwuZr+1s1AQ7xx0IMfCSG35gaBnYgdEo7q3gej\"]}]}";
            var openIdCfg = JsonConvert.DeserializeObject<OpenIdConnectConfiguration>(cfg);
            openIdCfg.JsonWebKeySet = JsonConvert.DeserializeObject<JsonWebKeySet>(keys);
            foreach (var key in openIdCfg.JsonWebKeySet.GetSigningKeys())
            {
                openIdCfg.SigningKeys.Add(key);
            }
            return openIdCfg;
        }
    }
}