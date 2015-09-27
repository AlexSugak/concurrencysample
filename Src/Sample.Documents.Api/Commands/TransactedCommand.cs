using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Sample.Documents.Api.Commands
{
    /// <summary>
    /// Wraps any given command into transaction
    /// </summary>
    public class TransactedCommand<T> : ICommand<T>
    {
        private readonly ICommand<T> _inner;

        public TransactedCommand(ICommand<T> inner)
        {
            _inner = inner;
        }

        public void Execute(Envelope<T> input)
        {
            using (var trn = new TransactionScope())
            {
                _inner.Execute(input);
                trn.Complete();
            }
        }
    }

    /// <summary>
    /// Composes 1..N commands into one command
    /// </summary>
    public class ComposedCommand<T> : ICommand<T>
    {
        private readonly ICommand<T>[] _inner;

        public ComposedCommand(params ICommand<T>[] inner)
        {
            _inner = inner;
        }

        public void Execute(Envelope<T> input)
        {
            foreach(var cmd in _inner)
            {
                cmd.Execute(input);
            }
        }
    }
}
