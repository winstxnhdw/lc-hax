using System;
using System.Collections;
using UnityEngine;

enum CoroutineState {
    RUNNING,
    EXHAUSTED,
    ERROR
}

class InvalidCoroutineState(CoroutineState state) : Exception($"Invalid CoroutineState: {state}") { }

public static partial class Extensions {
    public static Coroutine StartResilientCoroutine(this MonoBehaviour self, Func<IEnumerator> coroutineFactory) {
        static CoroutineState ExecuteCoroutineStep(IEnumerator coroutine) {
            try {
                return coroutine.MoveNext() ? CoroutineState.RUNNING : CoroutineState.EXHAUSTED;
            }

            catch {
                return CoroutineState.ERROR;
            }
        }

        static IEnumerator ResilientCoroutine(Func<IEnumerator> coroutineFactory) {
            IEnumerator coroutine = coroutineFactory();

            while (true) {
                CoroutineState state = ExecuteCoroutineStep(coroutine);

                switch (state) {
                    case CoroutineState.RUNNING:
                        yield return coroutine.Current;
                        break;

                    case CoroutineState.ERROR:
                        coroutine = coroutineFactory();
                        yield return new WaitForSeconds(1.0f);
                        break;

                    case CoroutineState.EXHAUSTED:
                        yield break;

                    default:
                        throw new InvalidCoroutineState(state);
                }
            }
        }

        return self.StartCoroutine(ResilientCoroutine(coroutineFactory));
    }
}
