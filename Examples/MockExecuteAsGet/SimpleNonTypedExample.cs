using System.Net;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteAsGet
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
            var response = MockRestClient
                .MockApiResponse()
                .WithStatusCode(HttpStatusCode.OK)
                .ReturnsJsonString("{ \"id\": 1,\"firstName\": \"Ut\", \"lastName\": \"Consectetur\" }")
                .MockExecuteAsGet("GET");

            var user = FakePostsApi.GetUser(1);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Method.ShouldBe(Method.GET);
            response.Request.Resource.ShouldBe("/user/1");
        }
    }
}
