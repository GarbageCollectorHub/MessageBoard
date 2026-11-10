using FluentValidation;
using MessageBoard.Application.Comments;
using MessageBoard.Application.Comments.DTOs;
using MessageBoard.Application.Comments.Exceptions;
using MessageBoard.Application.Comments.Validation;
using MessageBoard.Domain.Entities;

namespace MessageBoard.Tests.Comments;

public class CommentServiceTests
{

    // Невалидный запрос не должен приводить к добавлению или сохранению комментария.
    [Fact]
    public async Task CreateAsync_InvalidRequest_DoesNotSaveComment()
    {
        var repository = new TestCommentRepository();
        var service = new CommentService(repository, new CreateCommentRequestDtoValidator());
        var request = CreateValidRequest() with
        {
            UserName = "Test User"
        };

        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(request));

        Assert.False(repository.AddCalled);
        Assert.False(repository.SaveChangesCalled);
    }


    // Ответ на отсутствующий комментарий отклоняется до сохранения.
    [Fact]
    public async Task CreateAsync_ParentNotFound_DoesNotSaveComment()
    {
        var repository = new TestCommentRepository();
        var service = new CommentService(repository, new CreateCommentRequestDtoValidator());
        var request = CreateValidRequest() with
        {
            ParentId = 123
        };

        await Assert.ThrowsAsync<ParentCommentNotFoundException>(
            () => service.CreateAsync(request));

        Assert.False(repository.AddCalled);
        Assert.False(repository.SaveChangesCalled);
    }

    private static CreateCommentRequestDto CreateValidRequest()
    {
        return new CreateCommentRequestDto
        {
            UserName = "TestUser",
            Email = "test@example.com",
            Text = "Test message"
        };
    }


    // Заменяет БД в тестах и отмечает обращения к методам записи.
    private sealed class TestCommentRepository : ICommentRepository
    {
        public bool AddCalled { get; private set; }
        public bool SaveChangesCalled { get; private set; }

        public void Add(Comment comment)
        {
            AddCalled = true;
        }

        public Task<bool> ExistsByIdAsync(long id, CancellationToken ct = default)
        {
            return Task.FromResult(false);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            SaveChangesCalled = true;
            return Task.FromResult(1);
        }

    }
}