readonly ref struct Size {
    internal required float Width { get; init; }
    internal required float Height { get; init; }

    internal Size(float size) {
        this.Width = size;
        this.Height = size;
    }

    public static Size operator +(Size a, Size b) => new() {
        Width = a.Width + b.Width,
        Height = a.Height + b.Height
    };

    public static Size operator -(Size a, Size b) => new() {
        Width = a.Width - b.Width,
        Height = a.Height - b.Height
    };

    public static Size operator *(Size size, float multiplier) => new() {
        Width = size.Width * multiplier,
        Height = size.Height * multiplier
    };

    public static Size operator /(Size size, float divider) => new() {
        Width = size.Width / divider,
        Height = size.Height / divider
    };
}
