using System;

interface IPhantomExtension : IDisposable {
    bool ControlsCamera { get; }

    void Update();
    void LateUpdate();
}
