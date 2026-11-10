using MessageBoard.Application.Comments.Validation;

namespace MessageBoard.Tests.Comments;

public class CommentHtmlValidatorTests
{

    [Theory]
    [InlineData("I use C# and .NET", true)]
    [InlineData("I use C# & .NET", false)]
    [InlineData("I use C# &amp; .NET", true)]
    [InlineData("<strong>C# &amp; .NET</strong>", true)]
    [InlineData("<strong>C# &amp .NET</strong>", false)]
    public void IsValid_Ampersand_ReturnsExpectedResult(
        string text,
        bool expected)
    {
        var result = CommentHtmlValidator.IsValid(text);

        Assert.Equal(expected, result);
    }


}
