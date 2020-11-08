using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Web.Domain.Abstractions.Auth
{
    public class TokenPair
    {
        public TokenPair(string AccesToken, string RefreshToken)
        {
            this.AccessToken = AccesToken;
            this.RefreshToken = RefreshToken;
        }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
