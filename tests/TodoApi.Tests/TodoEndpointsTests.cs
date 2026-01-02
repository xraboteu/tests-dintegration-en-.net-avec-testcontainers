using System.Net;
using System.Net.Http.Json;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public class TodoEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    public TodoEndpointsTests(ApiFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task POST_todos_ShouldCreateTodoAndReturnCreated()
    {
        // Arrange
        var newTodo = new TodoItem
        {
            Title = "Test Todo",
            IsCompleted = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/todos", newTodo);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var createdTodo = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(createdTodo);
        Assert.True(createdTodo.Id > 0);
        Assert.Equal("Test Todo", createdTodo.Title);
        Assert.False(createdTodo.IsCompleted);
    }

    [Fact]
    public async Task GET_todos_id_ShouldReturnTodoWhenExists()
    {
        // Arrange - Créer un todo d'abord
        var newTodo = new TodoItem
        {
            Title = "Get Test Todo",
            IsCompleted = true
        };

        var createResponse = await _client.PostAsJsonAsync("/todos", newTodo);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(createdTodo);

        // Act
        var getResponse = await _client.GetAsync($"/todos/{createdTodo.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        
        var retrievedTodo = await getResponse.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(retrievedTodo);
        Assert.Equal(createdTodo.Id, retrievedTodo.Id);
        Assert.Equal("Get Test Todo", retrievedTodo.Title);
        Assert.True(retrievedTodo.IsCompleted);
    }

    [Fact]
    public async Task GET_todos_id_ShouldReturnNotFoundWhenNotExists()
    {
        // Act
        var response = await _client.GetAsync("/todos/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

