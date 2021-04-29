using System;
using System.Collections.Generic;
using System.Net;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecute
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
        public void FullConfigurationExample_ShouldFormProperRequest()
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
                    .WithServer("Server1")
                    .WithProtocolVersion(Version.Parse("1.1"))
                    .WithContentEncoding("Content Encoding - Encoding 1")
                    .WithContentType("Content Type - Type 1")
                    .Returns(fakePostToReturn)
                .MockExecute();

            var fakePost = FakePostsApi.GetPost(fakePostId);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Method.ShouldBe(Method.GET);
            response.Request.Resource.ShouldBe("/posts/1");
            response.Server.ShouldBe("Server1");
            response.ProtocolVersion.Major.ShouldBe(1);
            response.ProtocolVersion.Minor.ShouldBe(1);
            response.ContentType.ShouldBe("Content Type - Type 1");
            response.ContentEncoding.ShouldBe("Content Encoding - Encoding 1");

            fakePost.ShouldNotBeNull();
        }
    }
}
