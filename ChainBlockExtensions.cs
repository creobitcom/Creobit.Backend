using System;
using System.Collections.Generic;

namespace Creobit
{
    public static class ChainBlockExtensions
    {
        public static void Execute(this IEnumerable<IChainBlock<bool>> chain, Action onComplete, Action onFailure)
        {
            chain.Execute(OnExecuted);

            void OnExecuted(bool result)
            {
                if (result)
                {
                    onComplete?.Invoke();
                    return;
                }

                onFailure?.Invoke();
            }
        }

        public static void Execute(this IEnumerable<IChainBlock<bool>> chain, Action<bool> handler)
        {
            Start(chain.GetEnumerator(), handler);
        }

        private static void Start(IEnumerator<IChainBlock<bool>> pointer, Action<bool> handler)
        {
            UnityEngine.Debug.LogWarning($"pointer == null ? {pointer == null} , pointer?.Current == null ? {pointer?.Current == null}");
            if (pointer.MoveNext())
            {
                pointer.Current.Execute(OnExecuted);
                return;
            }

            handler?.Invoke(true);


            void OnExecuted(bool isSuccess)
            {
                if (!isSuccess)
                {
                    handler?.Invoke(false);
                    return;
                }

                Start(pointer, handler);
            }
        }
    }
}