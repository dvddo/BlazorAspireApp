using Microsoft.JSInterop;
using System.Net.Http;
using static BlazorAspireApp.Web.AuthorizationApiClient;

namespace BlazorAspireApp.Web
{
    public class TodoListsApiClient(HttpClient httpClient)
    {

        // Get TodoLists
        public async Task<ToDoListsResponse> GetTodoListsAsync(IJSRuntime jSRuntime, CancellationToken cancellationToken = default)
        {
            httpClient = await SetHeaderToken(jSRuntime, httpClient);
            return await httpClient.GetFromJsonAsync<ToDoListsResponse>("/api/TodoLists", cancellationToken);
        }

        // Create TodoList
        public async Task<int> CreateTodoListAsync(IJSRuntime jSRuntime, string title, CancellationToken cancellationToken = default)
        {
            httpClient = await SetHeaderToken(jSRuntime, httpClient);
            var payload = new { Title = title };
            int listId = -1;
            try
            {
                HttpResponseMessage resp = await httpClient.PostAsJsonAsync("/api/TodoLists", payload, cancellationToken);
                if (resp.IsSuccessStatusCode && resp.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    // ... proceed to read location header and/or content
                    var content = await resp.Content.ReadAsStringAsync();
                    listId = int.Parse(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting header token: {ex.Message}");
                throw;
            }
            return listId;
        }

        // Update TodoList
        public async Task<HttpResponseMessage> UpdateTodoListAsync(int id, string title, CancellationToken cancellationToken = default)
        {
            var payload = new { Id = id, Title = title };
            return await httpClient.PutAsJsonAsync($"/api/TodoLists/{id}", payload, cancellationToken);
        }

        // Delete TodoList
        public async Task<HttpResponseMessage> DeleteTodoListAsync(int id, CancellationToken cancellationToken = default)
        {
            return await httpClient.DeleteAsync($"/api/TodoLists/{id}", cancellationToken);
        }


        // Get TodoItems
        public async Task<TodoItem[]?> GetTodoItemsAsync( int listId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            
            return await httpClient.GetFromJsonAsync<TodoItem[]>($"/api/TodoItems?ListId={listId}&PageNumber={pageNumber}&PageSize={pageSize}", cancellationToken);
        }

        // Create TodoItem
        public async Task<int> CreateTodoItemAsync(IJSRuntime jSRuntime, int listId, string title, CancellationToken cancellationToken = default)
        {
            httpClient = await SetHeaderToken(jSRuntime, httpClient);
            var payload = new {ListId = listId, Title = title };
            int itemId = -1;
            try
            {
                HttpResponseMessage resp = await httpClient.PostAsJsonAsync("/api/TodoItems", payload, cancellationToken);
                if (resp.IsSuccessStatusCode && resp.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    // ... proceed to read location header and/or content
                    var content = await resp.Content.ReadAsStringAsync();
                    itemId = int.Parse(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting header token: {ex.Message}");
                throw;
            }


            return itemId;
        }

        // Update TodoItem
        public async Task<HttpResponseMessage> UpdateTodoItemAsync(IJSRuntime jSRuntime, int id, int listid, string title, bool done, CancellationToken cancellationToken = default)
        {
            try
            {
                httpClient = await SetHeaderToken(jSRuntime, httpClient);
                var payload = new { Id = id, ListId = listid, Title = title, Done = done };
                return await httpClient.PutAsJsonAsync($"/api/TodoItems/{id}", payload, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting header token: {ex.Message}");
                throw;
            }
            
        }

        // Delete TodoItem
        public async Task<HttpResponseMessage> DeleteTodoItemAsync(IJSRuntime jSRuntime, int id, CancellationToken cancellationToken = default)
        {
            httpClient = await SetHeaderToken(jSRuntime, httpClient);
            return await httpClient.DeleteAsync($"/api/TodoItems/{id}", cancellationToken);
        }

        public class TodoItem
        {
            public int id { get; set; }
            public int listId { get; set; }
            public string title { get; set; }
            public bool done { get; set; }
            public int priority { get; set; }
            public string note { get; set; }
        }

        public class TodoList
        {
            public int id { get; set; }
            public string title { get; set; }
            public string colour { get; set; }
            public List<TodoItem> items { get; set; }
        }

        public class PriorityLevel
        {
            public int id { get; set; }
            public string title { get; set; }
        }

        public class ToDoListsResponse
        {
            public List<PriorityLevel> priorityLevels { get; set; }
            public List<TodoList> lists { get; set; }
        }

        public class ToDoItemsResponse
        {
            public List<TodoItem> items { get; set; }
            public int pageNumber { get; set; }
            public int totalPages { get; set; }
            public int totalCount { get; set; }
            public bool hasPreviousPage { get; set; }
            public bool hasNextPage { get; set; }
        }
    }

}
