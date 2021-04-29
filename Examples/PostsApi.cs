using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace Examples
{
    public class PostsApi
    {
        private readonly IRestClient _restClient;

        public PostsApi(IRestClient restClient)
        {
            _restClient = restClient;
        }

        // http://fakeapi.jsonparseronline.com/posts
        public async Task<List<Post>> GetPosts()
        {
            _restClient.BaseUrl = new Uri("http://fakeapi.jsonparseronline.com");
            var restRequest = new RestRequest("/posts", Method.GET);

            var response = await _restClient.ExecuteAsync<List<Post>>(restRequest);
            return response.Data;
        }

        public async Task<bool> DeletePost(int postId)
        {
            _restClient.BaseUrl = new Uri("http://fakeapi.jsonparseronline.com");
            var restRequest = new RestRequest($"/posts/{postId}", Method.DELETE);

            var response = await _restClient.ExecuteAsync(restRequest);
            return response.IsSuccessful;
        }

        // Non-async for demonstration
        public Post GetPost(int postId)
        {
            _restClient.BaseUrl = new Uri("http://fakeapi.jsonparseronline.com");
            var restRequest = new RestRequest($"/posts/{postId}", Method.GET);
            var response = _restClient.Execute<Post>(restRequest);

            return response.Data;
        }

        public User GetUser(int userId)
        {
            _restClient.BaseUrl = new Uri("http://fakeapi.jsonparseronline.com");
            var restRequest = new RestRequest($"/user/{userId}", Method.GET);
            var response = _restClient.ExecuteAsGet(restRequest, "GET");
            return JsonConvert.DeserializeObject<User>(response.Content);
        }

        public User GetUserInformation(int userId)
        {
            _restClient.BaseUrl = new Uri("http://fakeapi.jsonparseronline.com");
            var restRequest = new RestRequest($"/user/{userId}", Method.GET);
            var response = _restClient.ExecuteAsGet<User>(restRequest, "GET");
            return response.Data;
        }

        // Non-async for demonstration
        public void DeleteCategory(int categoryId)
        {
            _restClient.BaseUrl = new Uri("http://fakeapi.jsonparseronline.com");
            var restRequest = new RestRequest($"/category/{categoryId}", Method.DELETE);
            _restClient.Execute(restRequest);
        }

        public void PostComment(string stringComment)
        {
            _restClient.BaseUrl = new Uri("http://fakeapi.jsonparseronline.com");
            var restRequest = new RestRequest($"/comments");
            restRequest.AddJsonBody(new {commentContent = stringComment});
            _restClient.ExecuteAsPost(restRequest, "POST");
        }

        public CategorySuccess AddCategory(string categoryType)
        {
            _restClient.BaseUrl = new Uri("http://fakeapi.jsonparseronline.com");
            var restRequest = new RestRequest($"/category", Method.POST);
            restRequest.AddJsonBody(new { name = categoryType });
            var response = _restClient.ExecuteAsPost<CategorySuccess>(restRequest, "POST");
            return response.Data;
        }
    }

    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; }

        public int Likes { get; set; }

        public int Hits { get; set; }

        public int CategoryId { get; set; }

        public string ImageUrl { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CategorySuccess
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int Id { get; set; }
        public long CreatedAt { get; set; }

    }
}