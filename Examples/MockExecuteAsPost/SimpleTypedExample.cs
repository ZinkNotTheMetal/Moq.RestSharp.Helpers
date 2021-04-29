using System.Net;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteAsPost
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
        public void TypedExample_WithExecuteAsPost_ShouldReturnRequestAndResponse()
        {
            var fakeCategorySuccess = new CategorySuccess
                { Id = 1, CreatedAt = 3333333, Message = "Successful creation", Name = "New Category", Success = true };

            var response = MockRestClient
                .MockApiResponse<CategorySuccess>()
                .WithStatusCode(HttpStatusCode.OK)
                .Returns(fakeCategorySuccess)
                .MockExecuteAsPost("POST");

            var user = FakePostsApi.AddCategory("New Category");

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Method.ShouldBe(Method.POST);
            response.Request.Resource.ShouldBe("/category");
        }
    }
}
