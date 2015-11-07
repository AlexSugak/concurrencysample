using System;
using System.Transactions;

namespace Sample.Api.Shared
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

    public static class TransactedExtention
    {
        public static ICommand<T> Transacted<T>(this ICommand<T> cmd)
        {
            return new TransactedCommand<T>(cmd);
        }
    }
}
