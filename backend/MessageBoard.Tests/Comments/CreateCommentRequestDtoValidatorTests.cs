using MessageBoard.Application.Comments;
using MessageBoard.Application.Comments.Validation;
using static System.Net.Mime.MediaTypeNames;

namespace MessageBoard.Tests.Comments;

public class CreateCommentRequestDtoValidatorTests
{
    private readonly CreateCommentRequestDtoValidator _validator = new();

    // Корректный запрос проходит проверку; домашняя страница необязательна.
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("https://example.com")]
    public async Task Validate_ValidRequest_IsValid(string? homePage)
    {
        var request = CreateValidRequest() with
        {
            HomePage = homePage
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }


    // Обычный текст и разрешённая разметка проходят проверку.
    [Theory]
    [InlineData("Test message")]
    [InlineData("<strong>Test <i>message</i></strong> <code>x &lt; y</code>")]
    [InlineData("<a href=\"https://example.com/?a=1&amp;b=2\" title=\"Test\">Test</a>")]
    public async Task Validate_AllowedText_IsValid(string text)
    {
        var request = CreateValidRequest() with
        {
            Text = text
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    // Отклоняем запрещённую разметку, опасные ссылки и XML-конструкции.
    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<strong onclick=\"alert(1)\">Test</strong>")]
    [InlineData("<strong><i>Test</strong></i>")]
    [InlineData("<a href=\"java&#x73;cript:alert(1)\">Test</a>")]
    [InlineData("<![CDATA[<script>alert(1)</script>]]>")]
    [InlineData("<!DOCTYPE root [<!ENTITY test 'value'>]>&test;")]
    [InlineData("<i xmlns=\"urn:test\">Test</i>")]
    [InlineData("<i/>")]
    [InlineData("<a><strong><a>Test</a></strong></a>")]
    public async Task Validate_InvalidText_ReturnsTextError(string text)
    {
        var request = CreateValidRequest() with
        {
            Text = text
        };

        var result = await _validator.ValidateAsync(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateCommentRequestDto.Text));
    }


    // Имя допускает только латинские буквы и цифры.
    [Theory]
    [InlineData("Test User")]
    [InlineData("TestUser\n")]
    [InlineData("Денис")]
    public async Task Validate_InvalidUserName_ReturnsUserNameError(string userName)
    {
        var request = CreateValidRequest() with
        {
            UserName = userName
        };

        var result = await _validator.ValidateAsync(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateCommentRequestDto.UserName));
    }


    // Поле должно содержать один email без имени отправителя.
    [Theory]
    [InlineData("not-an-email")]
    [InlineData("Test <test@example.com>")]
    public async Task Validate_InvalidEmail_ReturnsEmailError(string email)
    {
        var request = CreateValidRequest() with
        {
            Email = email
        };

        var result = await _validator.ValidateAsync(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateCommentRequestDto.Email));
    }


    // Домашняя страница должна быть абсолютной HTTP/HTTPS-ссылкой без учётных данных.
    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("/profile")]
    [InlineData("https://user:password@example.com")]
    public async Task Validate_InvalidHomePage_ReturnsHomePageError(string homePage)
    {
        var request = CreateValidRequest() with
        {
            HomePage = homePage
        };

        var result = await _validator.ValidateAsync(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateCommentRequestDto.HomePage));
    }


    private static CreateCommentRequestDto CreateValidRequest()
    {
        return new CreateCommentRequestDto
        {
            UserName = "TestUser",
            Email = "Test@example.com",
            HomePage = "https://example.com",
            Text = "Hello"
        };
    }

    // ParentId: null или положительное число - допустимо, 0 и отрицательные - нет.
    [Theory]
    [InlineData(null, true)]
    [InlineData(1L, true)]
    [InlineData(100L, true)]
    [InlineData(0L, false)]
    [InlineData(-1L, false)]
    public async Task Validate_ParentId_ReturnsExpectedResult(long? parentId, bool expected)
    {
        var request = CreateValidRequest() with
        { 
            ParentId = parentId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.Equal(expected, result.IsValid);
    }


}