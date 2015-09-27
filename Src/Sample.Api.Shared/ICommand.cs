using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Api.Shared
{
    /// <summary>
    /// Generic interface for commands
    /// </summary>
    public interface ICommand<T>
    {
        //TODO: implement EnvelopCommand: ICommand<T> -> ICommand<Envelope<T>>
        void Execute(Envelope<T> input);
    }
}
