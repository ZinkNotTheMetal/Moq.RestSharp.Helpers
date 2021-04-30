using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecutePostAsync
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
        public async Task TypedExample_ShouldReturnRequestAndResponse()
        {
            var fakeUser = new User { Id = 3, FirstName = "Bob", LastName = "Jackson" };

            var response = MockRestClient
                .MockApiResponse<User>()
                .WithStatusCode(HttpStatusCode.OK)
                .Returns(fakeUser)
                .MockExecutePostAsync();

            var user = await FakePostsApi.AddUser(fakeUser);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Resource.ShouldBe("/user/add");
            response.Request.Parameters.ShouldNotBeEmpty();
            response.Request.Method.ShouldBe(Method.POST);
        }
    }
}
