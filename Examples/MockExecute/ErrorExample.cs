using System;
using System.Net;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecute
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
        public void DeleteCategoryFails_ResponseIsFormedProperly()
        {
            var response = MockRestClient
                .MockApiResponse()
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .WithErrorMessage("Category Id is not present")
                    .WithErrorException(new ArgumentNullException())
                .MockExecute();

            FakePostsApi.DeleteCategory(1);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            response.IsSuccessful.ShouldBe(false);
            response.ErrorMessage.ShouldBe("Category Id is not present");
            response.ErrorException.ShouldBeOfType<ArgumentNullException>();
            response.Request.Method.ShouldBe(Method.DELETE);
            response.Request.Resource.ShouldBe("/category/1");
        }
    }
}
