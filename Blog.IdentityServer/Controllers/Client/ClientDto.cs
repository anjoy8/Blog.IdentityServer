namespace Blog.IdentityServer.Controllers.Client
{
    public class ClientDto
    {
        public int id { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientSecrets { get; set; }
        public string Description { get; set; }
        public string AllowAccessTokensViaBrowser { get; set; }
        public string AllowedGrantTypes { get; set; }
        public string AllowedScopes { get; set; }
        public string AllowedCorsOrigins { get; set; }
        public string RedirectUris { get; set; }
        public string PostLogoutRedirectUris { get; set; }
    }
}