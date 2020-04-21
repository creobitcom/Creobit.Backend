using System;

namespace Creobit
{
    public sealed class SimpleChainBlock<T> : IChainBlock<T>
    {
        private readonly Action<Action<T>> Action;

        public SimpleChainBlock(Action<Action<T>> action)
        {
            Action = action;
        }

        void IChainBlock<T>.Execute(Action<T> handler)
        {
            Action?.Invoke(handler);
        }
    }
}