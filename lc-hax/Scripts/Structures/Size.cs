internal readonly ref struct Size {
    internal readonly float Width { get; }
    internal readonly float Height { get; }

    internal Size(float width, float height) {
        this.Width = width;
        this.Height = height;
    }

    internal Size(float size) {
        this.Width = size;
        this.Height = size;
    }

    public static Size operator +(Size a, Size b) {
        return new(a.Width + b.Width, a.Height + b.Height);
    }

    public static Size operator -(Size a, Size b) {
        return new(a.Width - b.Width, a.Height - b.Height);
    }

    public static Size operator *(Size size, float multiplier) {
        return new(size.Width * multiplier, size.Height * multiplier);
    }

    public static Size operator /(Size size, float divider) {
        return new(size.Width / divider, size.Height / divider);
    }
}
