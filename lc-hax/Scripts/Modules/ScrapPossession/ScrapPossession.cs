class ScrapPossession() {
    GrabbableObject? item = null;

    internal GrabbableObject? Item => this.item.Unfake();

    internal bool IsPossessed => this.Item is not null;

    internal void SetItem(GrabbableObject item) => this.item = item;

    internal void Clear() => this.item = null;
}
