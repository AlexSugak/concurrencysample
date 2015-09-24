using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api
{
    public class SimpleToken : IEnumerable<Claim>
    {
        private readonly IEnumerable<Claim> _claims;

        public SimpleToken(params Claim[] claims)
        {
            _claims = claims;
        }

        public IEnumerator<Claim> GetEnumerator()
        {
            return _claims.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join("&", _claims.Select(c => string.Format("{0}={1}", c.Type, c.Value)));
        }

        public static bool TryParse(string input, out SimpleToken token)
        {
            if(input == null)
            {
                token = null;
                return false;
            }

            if(input == string.Empty)
            {
                token = new SimpleToken();
                return true;
            }

            if (!input.Contains("="))
            {
                token = null;
                return false;
            }

            var claimTokens = input.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if(claimTokens.Any(t => !t.Contains("=")))
            {
                token = null;
                return false;
            }

            token = new SimpleToken(claimTokens
                                            .Select(t => 
                                            { 
                                                var tt = t.Split('=');
                                                return new Claim(tt[0], tt[1]);
                                            })
                                            .ToArray());

            return true;
        }
    }
}
