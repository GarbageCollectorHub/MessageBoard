namespace MessageBoard.Application.Comments.Validation;

public static class HttpUrlValidator
{
    public static bool IsValid(string? url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // Разрешаем только HTTP/HTTPS и без UserInfo (учётных данных вида user:password@host)
        return (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            && uri.IsWellFormedOriginalString()
            && string.IsNullOrEmpty(uri.UserInfo);
    }
}