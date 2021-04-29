using System.Net;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecute
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
        public void SimpleNonTypedExample_ShouldFormRequestCorrectly()
        {
            var response = MockRestClient
                .MockApiResponse()
                    .WithStatusCode(HttpStatusCode.OK)
                .MockExecute();

            FakePostsApi.DeleteCategory(1);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Request.Method.ShouldBe(Method.DELETE);
            response.Request.Resource.ShouldBe("/category/1");
        }
    }
}
