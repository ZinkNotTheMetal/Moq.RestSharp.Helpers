using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteAsync
{
    public class SimpleNonTypedExample
    {
        public Mock<IRestClient> MockRestClient;
        public PostsApi FakePostsApi;

        // Using xUnit - but all frameworks work
        public SimpleNonTypedExample()
        {
            MockRestClient = new Mock<IRestClient>();
            FakePostsApi = new PostsApi(MockRestClient.Object);
        }

        [Fact]
        public async Task NonTypedExample_ReturnsSimpleJsonString_ShouldReturnRequestAndResponse()
        {
            var response = MockRestClient
                .MockApiResponse()
                    .WithStatusCode(HttpStatusCode.OK)
                    .ReturnsJsonString("{ \"success\": true }")
                .MockExecuteAsync();

            var deletePost = await FakePostsApi.DeletePost(1);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Method.ShouldBe(Method.DELETE);
            response.Request.Resource.ShouldBe("/posts/1");

            deletePost.ShouldBeTrue();
        }
    }
}
