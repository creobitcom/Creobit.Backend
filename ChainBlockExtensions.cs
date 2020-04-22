using Creobit.Backend.Link;
using System;
using System.Collections.Generic;

namespace Creobit
{
    public static class ChainBlockExtensions
    {
        public static void Execute(this IEnumerable<IChainBlock<LinkCodeError?>> chain, Action onComplete, Action<LinkCodeError> onFailure)
        {
            chain.Execute(OnExecuted);

            void OnExecuted(LinkCodeError? error)
            {
                if (!error.HasValue)
                {
                    onComplete?.Invoke();
                    return;
                }

                onFailure?.Invoke(error.Value);
            }
        }

        public static void Execute(this IEnumerable<IChainBlock<LinkCodeError?>> chain, Action<LinkCodeError?> handler)
        {
            Start(chain.GetEnumerator(), handler);
        }

        private static void Start(IEnumerator<IChainBlock<LinkCodeError?>> pointer, Action<LinkCodeError?> handler)
        {
            UnityEngine.Debug.LogWarning($"pointer == null ? {pointer == null} , pointer?.Current == null ? {pointer?.Current == null}");
            if (pointer.MoveNext())
            {
                pointer.Current.Execute(OnExecuted);
                return;
            }

            handler?.Invoke(null);


            void OnExecuted(LinkCodeError? error)
            {
                if (error.HasValue)
                {
                    handler?.Invoke(error.Value);
                    return;
                }

                Start(pointer, handler);
            }
        }
    }
}