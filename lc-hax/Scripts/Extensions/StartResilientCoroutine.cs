using System;
using System.Collections;
using UnityEngine;

internal enum CoroutineState
{
    RUNNING,
    EXHAUSTED,
    ERROR
}

internal class InvalidCoroutineState(CoroutineState state) : Exception($"Invalid CoroutineState: {state}")
{
}

internal static partial class Extensions
{
    private static CoroutineState ExecuteCoroutineStep(IEnumerator coroutine)
    {
        try
        {
            return coroutine.MoveNext() ? CoroutineState.RUNNING : CoroutineState.EXHAUSTED;
        }

        catch
        {
            return CoroutineState.ERROR;
        }
    }

    private static IEnumerator ResilientCoroutine(Func<object[], IEnumerator> coroutineFactory, object[] args)
    {
        var coroutine = coroutineFactory(args);
        WaitForSeconds waitForOneSecond = new(1.0f);

        while (true)
        {
            var state = ExecuteCoroutineStep(coroutine);

            switch (state)
            {
                case CoroutineState.RUNNING:
                    yield return coroutine.Current;
                    break;

                case CoroutineState.ERROR:
                    coroutine = coroutineFactory(args);
                    yield return waitForOneSecond;
                    break;

                case CoroutineState.EXHAUSTED:
                    yield break;

                default:
                    throw new InvalidCoroutineState(state);
            }
        }
    }

    internal static Coroutine StartResilientCoroutine(this MonoBehaviour self,
        Func<object[], IEnumerator> coroutineFactory, params object[] args)
    {
        return self.StartCoroutine(ResilientCoroutine(coroutineFactory, args));
    }

    internal static Coroutine StartResilientCoroutine(this MonoBehaviour self,
        Func<object[], IEnumerator> coroutineFactory)
    {
        return self.StartResilientCoroutine(coroutineFactory, []);
    }
}