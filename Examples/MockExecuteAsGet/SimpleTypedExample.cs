using System.Net;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteAsGet
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
        public void NonTypedExample_ShouldReturnRequestAndResponse()
        {
            var fakeUser = new User { Id = 3, FirstName = "Bob", LastName = "Jackson" };

            var response = MockRestClient
                .MockApiResponse<User>()
                .WithStatusCode(HttpStatusCode.OK)
                .Returns(fakeUser)
                .MockExecuteAsGet("GET");

            var user = FakePostsApi.GetUserInformation(1);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Method.ShouldBe(Method.GET);
            response.Request.Resource.ShouldBe("/user/1");
        }
    }
}
