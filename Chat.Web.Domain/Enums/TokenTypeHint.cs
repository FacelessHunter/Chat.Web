using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Web.Domain.Enums
{
    public class TokenTypeHint
    {
        private TokenTypeHint(string value) { Value = value; }

        public string Value { get; set; }

        public static TokenTypeHint AccessToken { get { return new TokenTypeHint("access_token"); } }
        public static TokenTypeHint RefreshToken { get { return new TokenTypeHint("refresh_token"); } }

    }
}
