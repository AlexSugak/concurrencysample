using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.UnitTests
{
    public class ClaimComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim x, Claim y)
        {
            return object.Equals(x.Type, y.Type)
                && object.Equals(x.Value, y.Value);
        }

        public int GetHashCode(Claim obj)
        {
            return 0;
        }
    }
}
