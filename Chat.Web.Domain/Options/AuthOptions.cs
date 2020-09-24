using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Web.Domain.Options
{
    public class AuthOptions
    {
        public string AuthServer { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string AccessTokenName { get; set; }
        public string RefreshTokenName { get; set; }
    }
}
