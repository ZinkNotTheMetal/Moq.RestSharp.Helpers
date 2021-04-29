using System;
using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteAsync
{
    public class ErrorExample
    {
        public Mock<IRestClient> MockRestClient;
        public PostsApi FakePostsApi;

        // Using xUnit - but all frameworks work
        public ErrorExample()
        {
            MockRestClient = new Mock<IRestClient>();
            FakePostsApi = new PostsApi(MockRestClient.Object);
        }

        [Fact]
        public async Task DeletePostFails_ResponseIsFormedProperly()
        {
            var response = MockRestClient
                .MockApiResponse()
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .WithResponseStatus(ResponseStatus.Aborted)
                    .WithErrorMessage("Delete failed to update database")
                    .WithErrorException(new ArgumentException())
                .MockExecuteAsync();

            var deletePost = await FakePostsApi.DeletePost(1);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            response.IsSuccessful.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("Delete failed to update database");
            response.ErrorException.ShouldBeOfType<ArgumentException>();
            response.ResponseStatus.ShouldBe(ResponseStatus.Aborted);
            response.Request.Method.ShouldBe(Method.DELETE);
            response.Request.Resource.ShouldBe("/posts/1");

            deletePost.ShouldBeFalse();
        }
    }
}
