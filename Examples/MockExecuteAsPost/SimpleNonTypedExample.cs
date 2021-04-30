using System.Net;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteAsPost
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
        public void NonTypedExample_ReturnsSimpleJsonString_ShouldReturnRequestAndResponse()
        {
            var comment = "fake comment here";

            var response = MockRestClient
                .MockApiResponse()
                .WithStatusCode(HttpStatusCode.OK)
                .MockExecuteAsPost("POST");

            FakePostsApi.PostComment(comment);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Request.Method.ShouldBe(Method.POST);
            response.Request.Resource.ShouldBe("/comments");
        }
    }
}
