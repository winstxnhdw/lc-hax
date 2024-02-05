public readonly ref struct Size {
    public readonly float Width { get; }
    public readonly float Height { get; }

    public Size(float width, float height) {
        this.Width = width;
        this.Height = height;
    }

    public Size(float size) {
        this.Width = size;
        this.Height = size;
    }

    public static Size operator +(Size a, Size b) => new Size(a.Width + b.Width, a.Height + b.Height);

    public static Size operator -(Size a, Size b) => new Size(a.Width - b.Width, a.Height - b.Height);

    public static Size operator *(Size size, float multiplier) => new Size(size.Width * multiplier, size.Height * multiplier);

    public static Size operator /(Size size, float divider) => new Size(size.Width / divider, size.Height / divider);
}
