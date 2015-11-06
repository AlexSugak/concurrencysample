using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Api.Shared
{
    /// <summary>
    /// Generic interface for queries
    /// </summary>
    public interface IQuery<TIn, TOut>
    {
        TOut Execute(Envelope<TIn> request);
    }

    public class EmptyRequest
    {
    }
}
