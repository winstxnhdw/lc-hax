using System;
using System.Collections;
using UnityEngine;

internal class AsyncBehaviour : MonoBehaviour
{
    private Func<IEnumerator>? Func { get; set; }

    internal void Init(Func<IEnumerator> func)
    {
        Func = func;
        _ = StartCoroutine(AsyncCoroutine());
    }

    private IEnumerator AsyncCoroutine()
    {
        if (Func != null) yield return StartCoroutine(Func());
        Destroy(gameObject);
    }
}