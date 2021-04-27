# MoqRestSharp.Helpers
Helps you write a fluent syntax when creating tests for Mock\<IRestClient\>
## Description
This is a Unit Testing Helper for when you are attempting to Mock RestSharp for unit testing using IRestClient and Moq.

These helper extensions alleviate the need to form up the entire Mock Request and response each time you need to setup a unit test on Mock\<IRestRequest\>.
Additionally, if you used the generic RestSharp Execute\<> or ExecuteAsync\<> you would have to create a brand new Mock Object for that as well.

This got me thinking, here is a great place for the Builder Pattern insert MoqRestSharp.Helpers!

A portable unit testing library that helps you with your Mocking\<IRestClient\> needs!

# Before
```csharp
MockRestClient = new Mock<IRestClient>();
MockRestResponse = new Mock<IRestResponse>();

public async Task RestSharpSlackApi_ShouldFormRequestProperly()
{
    MockRestResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
    MockRestResponse.Setup(s => s.Content).Returns("ok");

    MockRestClient
        .Setup(s => s.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
        .Callback<IRestRequest, CancellationToken>((request, cancel) =>
            {
                MockRestResponse.Setup(s => s.Request).Returns(request);
            })
        .ReturnsAsync(MockRestResponse.Object);

    // Using Shouldly
    response.request.Method.ShouldBe(POST);
}
```

Oh and this is just the beginning... what about if you use the generic Execute\<> or ExecuteAsync\<>

```csharp
MockRestClient = new Mock<IRestClient>();
MockRestResponse = new Mock<IRestResponse>();

public async Task PersonRepository_GetListOfPeople_ShouldFormRequestProperly()
{
    //Using NBuilder
    var fakeResponse = Builder<Person>.CreateNewListOfSize(5).Build().ToList();
    var json = JsonConvert.SerializeObject(fakeResponse);

    MockRestResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
    MockRestResponse.Setup(s => s.Content).Returns(json);
    MockRestResponse.Setup(s => s.Data).Returns(fakeResponse);

    MockRestClient
        .Setup(s => s.ExecuteAsync<T>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
        .Callback<IRestRequest, CancellationToken>((request, cancel) =>
            {
                MockRestResponse.Setup(s => s.Request).Returns(request);
            })
        .ReturnsAsync(MockRestResponse.Object);
}
```
I know this is an extreme example putting this in per test, sure you could put it in a base class or a central testing library supporting both generic and non-generic Execute and Execute async or...

# After
Or you could use this...
```csharp
// Using NBuilder
var fakePeopleInRepo = Builder<Person>.CreateNewListOfSize(5).Build().ToList();

// Generic Execute<> using RestClient.Execute<List<Person>>(request);
var response = MockRestClient
    .MockApiResponse<List<Person>>()
        .WithStatusCode(HttpStatusCode.OK)
        .ReturnsFromApi(fakePeopleInRepo)
    .MockExecute();

// Generic ExecuteAsync<> using RestClient.ExecuteAsync<List<Person>>(request);
var response = MockRestClient
    .MockApiResponse<List<Person>>()
        .WithStatusCode(HttpStatusCode.OK)
        .ReturnsFromApi(fakePeopleInRepo)
    .MockExecuteAsync();
```
But what if I don't use the typed Execute or typed execute async. No problem I have you covered

```csharp
// Json as string for RestClient.Execute()
var response = MockRestClient
    .MockApiResponse()
        .WithStatusCode(HttpStatusCode.OK)
        .ReturnsJsonString("successful")
    .MockExecute();

// Json as string for RestClient.ExecuteAsync()
var response = MockRestClient
    .MockApiResponse()
        .WithStatusCode(HttpStatusCode.OK)
        .ReturnsJsonString("successful")
    .MockExecuteAsync();
```

# Code that needs to be tested
```csharp
public interface IPeopleRepository
{
    Task<List<Person>> GetPeopleFromRepository();
}

public class FakeApi : IPeopleRepository
{
    private readonly IRestClient _restClient;

    public FakeApi(IRestClient restClient)
    {
        _restClient = restClient;
    }

    public async Task<List<Person>> GetPeopleFromRepository()
    {
        _restClient.BaseUrl = new Uri("https://wz.me.com");
        var request = new RestRequest("fake-method", Method.GET);

        var response = await _restClient.ExecuteAsync<List<Person>>(request);

        return response.Data;
    }
}

public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```


# Methods available
## Mock\<RestClient\>
* `MockApiRequest()`
* `MockApiRequest\<T\>()`

## Mock\<IRestResponse\>
* `WithResponseStatus(ResponseStatus responseStatus)` - IRestResponse will return you the passed in RestSharp Response Status if you would like to override it
* `WithStatusCode(HttpStatusCode statusCode)` - IRestResponse will return you the passed in HttpStatusCode
* `WithErrorMessage(string errorMessage)` - IRestResponse will return you the passed in error message

If you are not using the typed MockApiRequest()
* `ReturnsJsonAsString(string json)` - IRestResponse will return you the content of the string being passed in

If you are using the typed MockApiRequest\<T\>
* `Returns<T>(T returnValue)` - IRestResponse will return you the content as json and data object you pass in 

## Finally mock the call using
* `MockExecute()` - will return you IRestResponse if you want to query the request
* `MockExecuteAsync()` - will return you IRestResponse if you want to query the request

And that's it!

# Examples!
So I'm better with seeing it in action, so here are some examples...
```csharp
// Test that I am posting to the correct endpoint (non-generic)
var response = MockRestClient
    .MockApiRequest()
        .WithStatusCode(HttpStatusCode.Ok)
        .ReturnsJsonString("successful")
    .MockExecute();

response.Request.Resource.ShouldBe("/api/send-message");
response.Request.Method.ShouldBe(POST);
```

What about mocking some API error cases
```csharp
// Test my code handles errors when the API has an internal server error
var response = MockRestClient
    .MockApiRequest<List<Person>>()
        .WithStatusCode(HttpStatusCode.InternalServerError)
        .WithResponseStatus(ResponseStatus.Error)
        .WithErrorMessage("API seems to be down at the moment, please try again later")
    .MockExecuteAsync();

// Assert that your code handles the error thrown by the API
```

Thanks! 

I hope this project helps you out with your unit testing endevours. If you need any help feel free to contact me. And if you find an issue, feel free to create one in GitHub or create a Pull Request.