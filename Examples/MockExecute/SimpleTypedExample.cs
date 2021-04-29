using System.Net;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecute
{
    public class SimpleTypedExample
    {
        public Mock<IRestClient> MockRestClient;
        public PostsApi FakePostsApi;

        // Using xUnit - but all frameworks work
        public SimpleTypedExample()
        {
            MockRestClient = new Mock<IRestClient>();
            FakePostsApi = new PostsApi(MockRestClient.Object);
        }

        [Fact]
        public void SimpleTypedExample_()
        {
            var fakePostId = 1;
            var fakePostToReturn = new Post()
            {
                Title = "Fake Post 1",
                CategoryId = 3,
                Content = "Fake Content Here",
                Hits = 0,
                Id = fakePostId,
                UserId = 99,
                ImageUrl = "fakeurl.com/here",
                Likes = 0
            };

            var response = MockRestClient
                .MockApiResponse<Post>()
                    .WithStatusCode(HttpStatusCode.OK)
                    .Returns(fakePostToReturn)
                .MockExecute();

            var fakePost = FakePostsApi.GetPost(fakePostId);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Method.ShouldBe(Method.GET);
            response.Request.Resource.ShouldBe($"/posts/{fakePostId}");

            fakePost.ShouldNotBeNull();
        }
    }
}
