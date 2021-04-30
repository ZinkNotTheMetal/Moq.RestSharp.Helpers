using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.RestSharp.Helpers;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteGetAsync
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
            var fakeCategory = new Category { Id = 3, Name = "Bob" };

            var response = MockRestClient
                .MockApiResponse<Category>()
                .WithStatusCode(HttpStatusCode.OK)
                .Returns(fakeCategory)
                .MockExecuteGetAsync();

            var user = await FakePostsApi.GetCategoryInformation(11);

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            response.Content.ShouldNotBeNull();
            response.Request.Method.ShouldBe(Method.GET);
            response.Request.Resource.ShouldBe("/category/11");
        }
    }
}
