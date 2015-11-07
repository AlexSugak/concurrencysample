using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Api.Shared
{
    /// <summary>
    /// Wraps any given command with user permissions check
    /// </summary>
    public class SecuredCommand<T> : ICommand<T>
    {
        private readonly ICommand<T> _inner;

        public SecuredCommand(ICommand<T> inner)
        {
            _inner = inner;
        }

        public void Execute(Envelope<T> c)
        {
            //TODO: implement proper auth
            if(string.IsNullOrEmpty(c.UserName))
            {
                throw new UnauthorizedAccessException("User name required, was empty");
            }

            _inner.Execute(c);
        }
    }

    public static class SecuredCommandExtention
    {
        public static ICommand<T> Secured<T>(this ICommand<T> cmd)
        {
            return new SecuredCommand<T>(cmd);
        }
    }
}
