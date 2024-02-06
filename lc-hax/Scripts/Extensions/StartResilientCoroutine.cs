using System;
using System.Collections;
using UnityEngine;

enum CoroutineState {
    RUNNING,
    ERROR,
    EXHAUSTED
}

class InvalidCoroutineState(CoroutineState state) : Exception($"Invalid CoroutineState: {state}") { }

internal static partial class Extensions {
    static CoroutineState ExecuteCoroutineStep(IEnumerator coroutine) {
        try {
            return coroutine.MoveNext() ? CoroutineState.RUNNING : CoroutineState.EXHAUSTED;
        }

        catch {
            return CoroutineState.ERROR;
        }
    }

    static IEnumerator ResilientCoroutine(Func<object[], IEnumerator> coroutineFactory, object[] args) {
        IEnumerator coroutine = coroutineFactory(args);
        WaitForSeconds waitForOneSecond = new(1.0f);

        while (true) {
            CoroutineState state = Extensions.ExecuteCoroutineStep(coroutine);

            object? action = state switch {
                CoroutineState.RUNNING => coroutine.Current,
                CoroutineState.ERROR => waitForOneSecond,
                CoroutineState.EXHAUSTED => null,
            };

            if (action is null) {
                yield break;
            }

            yield return action;
        }
    }

    internal static Coroutine StartResilientCoroutine(this MonoBehaviour self, Func<object[], IEnumerator> coroutineFactory, params object[] args) =>
        self.StartCoroutine(Extensions.ResilientCoroutine(coroutineFactory, args));

    internal static Coroutine StartResilientCoroutine(this MonoBehaviour self, Func<object[], IEnumerator> coroutineFactory) =>
        self.StartResilientCoroutine(coroutineFactory, []);
}
