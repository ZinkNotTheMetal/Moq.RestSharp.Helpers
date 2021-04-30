using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteGetAsync
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
        public async Task TypedExample_ShouldReturnRequestAndResponse()
        {
            var response = MockRestClient
                .MockApiResponse()
                .WithStatusCode(HttpStatusCode.OK)
                .ReturnsJsonString("{ \"id\": 1, \"firstName\": \"Ut\", \"lastName\": \"consectetur\" }")
                .MockExecuteGetAsync();

            var profile = await FakePostsApi.GetProfile();

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Method.ShouldBe(Method.GET);
            response.Request.Resource.ShouldBe("/profile");

            profile.ShouldNotBeNull();
        }
    }
}
