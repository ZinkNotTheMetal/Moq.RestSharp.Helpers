﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Moq;
using Moq.RestSharp.Helpers;
using Newtonsoft.Json;
using RestSharp;
using Shouldly;
using Xunit;

namespace Examples.MockExecuteAsync
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
        public async Task SimpleExample_ShouldFormProperRequestAndResponse()
        {
            var fakePostsWithoutHittingApi = new List<Post>
            {
                new Post()
                {
                    Title = "Fake Post 1",
                    CategoryId = 3,
                    Content = "Fake Content Here",
                    Hits = 0,
                    Id = 1,
                    UserId = 99,
                    ImageUrl = "fakeurl.com/here",
                    Likes = 0
                }
            };

            var response = MockRestClient
                .MockApiResponse<List<Post>>()
                    .WithStatusCode(HttpStatusCode.OK)
                    .Returns(fakePostsWithoutHittingApi)
                .MockExecuteAsync();

            var fakePosts = await FakePostsApi.GetPosts();

            // Using Shouldly
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.IsSuccessful.ShouldBe(true);
            // Content and Data are setup for you by default
            response.Content.ShouldNotBeNull();
            response.Content.ShouldBe(JsonConvert.SerializeObject(fakePosts));
            response.Request.Method.ShouldBe(Method.GET);
            response.Request.Resource.ShouldBe("/posts");

            fakePosts.ShouldNotBeEmpty();

            //As you can see we can unit test a lot of details about the request we formed. and even get fake posts back!
        }
    }
}
