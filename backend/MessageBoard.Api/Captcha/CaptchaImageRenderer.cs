using SkiaSharp;

namespace MessageBoard.Api.Captcha;

public sealed class CaptchaImageRenderer : IDisposable
{
    private const int ImageWidth = 224;
    private const int ImageHeight = 72;

    private readonly SKTypeface _typeface;

    public CaptchaImageRenderer()
    {
        // Формирует путь к шрифту относительно папки собранного API.
        string fontPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Captcha", "LiberationMono-Bold.ttf");

        // Загрузка шрифта для генерации CAPTCHA
        _typeface = SKTypeface.FromFile(fontPath) 
            ?? throw new InvalidOperationException("The CAPTCHA font could not be loaded.");
    }


    /// <summary>
    /// Создаёт PNG изображение CAPTCHA и возвращает его как строку Data URL (Base64).
    /// Фронтенд испльзует эту строку в теге img (src="..."), для отображения CAPTCHA.
    /// </summary>
    public string RenderImageDataUrl(string captchaCode)
    {
        // SKSurface хранит изображение в памяти.
        
        using SKSurface surface = SKSurface.Create(new SKImageInfo(ImageWidth, ImageHeight))
            ?? throw new InvalidOperationException("The CAPTCHA image could not be created.");

        // SKCanvas предоставляет методы для рисования на изображении текста, линий и точек.
        SKCanvas canvas = surface.Canvas;
        Random random = Random.Shared;
        canvas.Clear(SKColors.White);   // Задаёт белый фон изображения


        // Линии добавляют помехи для автоматического распознавания CAPTCHA.
        // Сначала рисуется фон, затем символы и кривая поверх текста.
        DrawBackgroundLines(canvas, random);    
        DrawCharacters(canvas, captchaCode, random);
        DrawForegroundCurve(canvas, random);


        // Получает результат рисования и преобразует его в PNG для передачи клиенту.
        using SKImage image = surface.Snapshot();
        using SKData png = image.Encode()
            ?? throw new InvalidOperationException("The CAPTCHA image could not be encoded.");

        // Кодирует PNG в Base64 и формирует Data URL, который можно подставить в src тега <img>.
        return $"data:image/png;base64,{Convert.ToBase64String(png.ToArray())}";
    }


    // Helpers
    private static void DrawBackgroundLines(SKCanvas canvas, Random random)
    {
        using var paint = new SKPaint
        {
            Color = new SKColor(200, 209, 220),
            StrokeWidth = 1,
            IsAntialias = true // Сглаживает ступеньки на наклонных линиях.
        };

        // Высота каждого конца выбирается отдельно, поэтому наклон линий может различаться.
        for (int line = 0; line < 3; line++)
        {
            canvas.DrawLine(8, random.Next(12, 60), ImageWidth - 8, random.Next(12, 60), paint);
        }
    }

    private void DrawCharacters(SKCanvas canvas, string captchaCode, Random random)
    {
        using var font = new SKFont(_typeface, 36);
        using var paint = new SKPaint { Color = new SKColor(24, 35, 58), IsAntialias = true };

        for (int index = 0; index < captchaCode.Length; index++)
        {
            font.Size = random.Next(31, 41);
            float x = 29 + index * 33 + random.Next(-5, 6);
            float y = 35 + random.Next(-5, 6);

            // Опускает линию, на которой стоит символ, чтобы поворачивать его примерно вокруг центра.
            float baseline = font.Metrics.CapHeight / 2;

            canvas.Save();
            canvas.Translate(x, y);
            canvas.RotateDegrees(random.Next(-15, 16));
            canvas.Skew(random.Next(-30, 31) / 100f, 0); // Сдвигает верх символа вбок относительно низа.
            canvas.DrawText(captchaCode[index].ToString(), 0, baseline, SKTextAlign.Center, font, paint);
            canvas.Restore(); // Возвращает холст в исходное состояние перед следующим символом.
        }
    }

    private static void DrawForegroundCurve(SKCanvas canvas, Random random)
    {
        using var paint = new SKPaint
        {
            Color = new SKColor(119, 136, 158, 225),
            StrokeWidth = 1.8f,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };

        using var path = new SKPathBuilder();
        path.MoveTo(12, random.Next(24, 45));

        // Первые две точки задают изгиб, последняя — правый конец линии.
        path.CubicTo(80, random.Next(8, 31), 145, random.Next(42, 65), ImageWidth - 12, random.Next(24, 45));

        using SKPath curve = path.Detach();
        canvas.DrawPath(curve, paint);
    }

    public void Dispose() => _typeface.Dispose();
}