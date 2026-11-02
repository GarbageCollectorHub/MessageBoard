using System.Xml;
using System.Xml.Linq;
using MessageBoard.Domain.Constants;

namespace MessageBoard.Application.Comments.Validation;

public static class CommentHtmlValidator
{

    public static bool IsValid(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > CommentLimits.TextMaxLength)
        {
            return false;
        }

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,     // Не принимаем DTD (правила и подстановки XML)
            XmlResolver = null                          // Не читаем внешние файлы и данные из сети
        };

        try
        {
            // Добавляем <root>, чтобы разобрать сообщение с несколькими тегами как XML-документ.
            // Получаем дерево тегов и текста, сохраняя пробелы и переносы строк.
            using var input = new StringReader($"<root>{text}</root>");
            using var reader = XmlReader.Create(input, settings);
            var root = XElement.Load(reader, LoadOptions.PreserveWhitespace);

            foreach (var node in root.DescendantNodes())
            {
                if (node.NodeType == XmlNodeType.Text)
                {
                    continue;
                }

                if (node is not XElement element || !IsAllowedElement(element))
                {
                    return false;
                }
            }

            return true;
        }
        catch (XmlException)
        {
            return false;
        }
    }

    private static bool IsAllowedElement(XElement element)
    {
        // Не принимаем XML-теги с дополнительной пометкой xmlns.
        // В HTML запись <i/> не закрывает тег, поэтому требуем <i></i>.
        if (element.Name.Namespace != XNamespace.None || element.IsEmpty)
        {
            return false;
        }

        switch (element.Name.LocalName)
        {
            case "a":
                return !element.Ancestors("a").Any()
                    && element.Attributes().All(IsAllowedLinkAttribute);

            case "code":
            case "i":
            case "strong":
                return !element.HasAttributes;

            default:
                return false;
        }
    }

    private static bool IsAllowedLinkAttribute(XAttribute attribute)
    {
        if (attribute.Name == "title")
        {
            return true;
        }

        // Проверяем адрес после обработки XML, чтобы не пропустить скрытый javascript:
        return attribute.Name == "href" && HttpUrlValidator.IsValid(attribute.Value);
    }
}