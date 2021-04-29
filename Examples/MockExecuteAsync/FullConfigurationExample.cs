using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteAsync
{
    public class FullConfigurationExample
    {
        public Mock<IRestClient> MockRestClient;
        public PostsApi FakePostsApi;

        // Using xUnit - but all frameworks work
        public FullConfigurationExample()
        {
            MockRestClient = new Mock<IRestClient>();
            FakePostsApi = new PostsApi(MockRestClient.Object);
        }

        [Fact]
        public async Task FullConfigurationExample_ShouldFormProperRequest()
        {
            var fakePostsWithoutHittingApi = new List<Post>
            {
                new Post()
                {
                    Title = "Fake Post 1",
                    CategoryId = 3,
                    Content = "Fake Content Here",
                    Hits = 0,
                    Id = 1,
                    UserId = 99,
                    ImageUrl = "fakeurl.com/here",
                    Likes = 0
                }
            };

            var response = MockRestClient
                .MockApiResponse<List<Post>>()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithResponseStatus(ResponseStatus.Completed)
                    .WithContentEncoding("Content Encoding - Encoding 1")
                    .WithContentType("Content Type - Type 1")
                    .WithProtocolVersion(Version.Parse("1.1"))
                    .WithServer("Server1")
                    .Returns(fakePostsWithoutHittingApi)
                .MockExecuteAsync();

            var fakePosts = await FakePostsApi.GetPosts();

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Method.ShouldBe(Method.GET);
            response.Request.Resource.ShouldBe("/posts");
            response.Server.ShouldBe("Server1");
            response.ProtocolVersion.Major.ShouldBe(1);
            response.ProtocolVersion.Minor.ShouldBe(1);
            response.ContentType.ShouldBe("Content Type - Type 1");
            response.ContentEncoding.ShouldBe("Content Encoding - Encoding 1");

            fakePosts.ShouldNotBeEmpty();
        }
    }
}
