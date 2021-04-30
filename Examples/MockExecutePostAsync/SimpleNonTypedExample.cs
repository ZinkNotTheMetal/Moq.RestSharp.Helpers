using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecutePostAsync
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
            var fakeUser = new User { Id = 3, FirstName = "Bob", LastName = "Jackson" };

            var response = MockRestClient
                .MockApiResponse()
                .WithStatusCode(HttpStatusCode.Accepted)
                .MockExecutePostAsync();

            var responseFromApi = await FakePostsApi.AddUserWithoutResponse(fakeUser);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
            response.IsSuccessful.ShouldBe(true);
            response.Request.Resource.ShouldBe("/user/add");
            response.Request.Parameters.ShouldNotBeEmpty();
            response.Request.Method.ShouldBe(Method.POST);

            responseFromApi.ShouldBeTrue();
        }
    }
}
