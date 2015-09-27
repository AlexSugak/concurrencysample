using System;

namespace Sample.Api.Shared
{
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
            foreach (var cmd in _inner)
            {
                cmd.Execute(input);
            }
        }
    }
}
