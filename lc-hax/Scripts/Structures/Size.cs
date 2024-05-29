internal readonly ref struct Size
{
    internal required float Width { get; init; }
    internal required float Height { get; init; }

    internal Size(float size)
    {
        Width = size;
        Height = size;
    }

    public static Size operator +(Size a, Size b)
    {
        return new Size
        {
            Width = a.Width + b.Width,
            Height = a.Height + b.Height
        };
    }

    public static Size operator -(Size a, Size b)
    {
        return new Size
        {
            Width = a.Width - b.Width,
            Height = a.Height - b.Height
        };
    }

    public static Size operator *(Size size, float multiplier)
    {
        return new Size
        {
            Width = size.Width * multiplier,
            Height = size.Height * multiplier
        };
    }

    public static Size operator /(Size size, float divider)
    {
        return new Size
        {
            Width = size.Width / divider,
            Height = size.Height / divider
        };
    }
}