using System;

namespace Creobit
{
    public sealed class SimpleChainBlock : IChainBlock<bool>
    {
        private readonly Action<Action<bool>> Action;

        public SimpleChainBlock(Action<Action<bool>> action)
        {
            Action = action;
        }

        void IChainBlock<bool>.Execute(Action<bool> handler)
        {
            Action?.Invoke(handler);
        }
    }
}