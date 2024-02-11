namespace MediaPauser.Controls;

internal sealed partial class FAButton
{
    public static readonly BindableProperty GlyphProperty =
        BindableProperty.Create(nameof(Glyph), typeof(string), typeof(FAButton), "");

    public static readonly BindableProperty GlyphColorProperty =
        BindableProperty.Create(nameof(GlyphColor), typeof(Color), typeof(FAButton), Colors.Black);

    public static readonly BindableProperty GlyphFontFamilyProperty =
        BindableProperty.Create(nameof(GlyphFontFamily), typeof(string), typeof(FAButton), "FontAwesome");

    public static readonly BindableProperty GlyphFontSizeProperty =
        BindableProperty.Create(nameof(GlyphFontSize), typeof(double), typeof(FAButton), 30.0);

    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public Color GlyphColor
    {
        get => (Color)GetValue(GlyphColorProperty);
        set => SetValue(GlyphColorProperty, value);
    }

    public string GlyphFontFamily
    {
        get => (string)GetValue(GlyphFontFamilyProperty);
        set => SetValue(GlyphFontFamilyProperty, value);
    }

    public double GlyphFontSize
    {
        get => (double)GetValue(GlyphFontSizeProperty);
        set => SetValue(GlyphFontSizeProperty, value);
    }


    public FAButton()
    {
        InitializeComponent();
    }
}