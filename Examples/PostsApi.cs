using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        // Non-async for demonstration
        public void DeleteCategory(int categoryId)
        {
            _restClient.BaseUrl = new Uri("http://fakeapi.jsonparseronline.com");
            var restRequest = new RestRequest($"/category/{categoryId}", Method.DELETE);
            _restClient.Execute(restRequest);
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
}
