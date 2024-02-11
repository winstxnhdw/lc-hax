using System;
using System.Collections;

enum CoroutineState {
    RUNNING,
    EXHAUSTED,
    ERROR
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

            switch (state) {
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

    internal static Coroutine StartResilientCoroutine(this MonoBehaviour self, Func<object[], IEnumerator> coroutineFactory, params object[] args) =>
        self.StartCoroutine(Extensions.ResilientCoroutine(coroutineFactory, args));

    internal static Coroutine StartResilientCoroutine(this MonoBehaviour self, Func<object[], IEnumerator> coroutineFactory) =>
        self.StartResilientCoroutine(coroutineFactory, []);
}
