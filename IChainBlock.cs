using System;

namespace Creobit
{
    public interface IChainBlock<T>
    {
        void Execute(Action<T> handler);
    }
}