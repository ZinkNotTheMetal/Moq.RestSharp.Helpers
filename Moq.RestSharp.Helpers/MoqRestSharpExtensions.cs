using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using RestSharp;

namespace Moq.RestSharp.Helpers
{
    public static class TypedMockRequest<T>
    {
        public static Mock<IRestResponse<T>> MockTypedRestResponse { get; set; }
    }

    public static class MoqRestSharpExtensions
    {
        public static Mock<IRestClient> MockRestClient { get; set; }

        public static ResponseStatus ResponseStatus { get; set; } = ResponseStatus.None;

        public static HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public static string Server { get; set; }

        #region Rest Client Helpers

        /// <summary>
        /// Setting up the Moq RestSharp extension helper.
        /// </summary>
        /// <param name="client" cref="Mock{IRestClient}">Extension method on the <see cref="Mock"/> of <see cref="IRestClient"/></param>
        /// <returns cref="Mock{IRestResponse}">The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse> MockApiResponse(this Mock<IRestClient> client)
        {
            MockRestClient = client;
            return new Mock<IRestResponse>();
        }

        /// <summary>
        /// Setting up the Moq RestSharp extension helper with a typed response
        ///   If you cast your RestSharp.Execute with a generic then use this method
        /// </summary>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="client" cref="Mock{IRestClient}">Extension method on the <see cref="Mock"/> of <see cref="IRestClient"/></param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse<T>> MockApiResponse<T>(this Mock<IRestClient> client)
        {
            MockRestClient = client;
            return TypedMockRequest<T>.MockTypedRestResponse = new Mock<IRestResponse<T>>();
        }

        #endregion


        # region Response Properties

        /// <summary>
        /// Use when you want to override the ResponseStatus of the &lt;see cref="IRestResponse"/&gt;.
        ///     Default: ResponseStatus.None
        /// </summary>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="responseStatus" cref="ResponseStatus">Response Status that you want the mocked API to return for you.</param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse<T>> WithResponseStatus<T>(this Mock<IRestResponse<T>> response, ResponseStatus responseStatus)
        {
            response.Setup(s => s.ResponseStatus).Returns(responseStatus);

            return response;
        }

        /// <summary>
        /// Used when you want to override the ResponseStatus of the <see cref="IRestResponse"/>.
        ///     Default: ResponseStatus.None
        /// </summary>
        /// <param name="response">Extension method on the mocked <see cref="IRestResponse"/> that will be returned from the builder pattern.</param>
        /// <param name="responseStatus" cref="ResponseStatus">Response Status that you want the mocked API to return for you.</param>
        /// <returns cref="Mock{IRestResponse}">The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse> WithResponseStatus(this Mock<IRestResponse> response, ResponseStatus responseStatus)
        {
            response.Setup(s => s.ResponseStatus).Returns(responseStatus);

            return response;
        }

        /// <summary>
        /// Used when you want the API to return you a certain status code.
        ///     Default: HttpStatusCode.Ok
        /// </summary>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="statusCode" cref="HttpStatusCode">HTTP Status Code that you want the mocked API to return for you.</param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse<T>> WithStatusCode<T>(this Mock<IRestResponse<T>> response, HttpStatusCode statusCode)
        {
            StatusCode = statusCode;

            response.Setup(s => s.StatusCode).Returns(statusCode);
            if (ResponseStatus == ResponseStatus.None)
            {
                response.Setup(s => s.IsSuccessful).Returns(IsSuccessCode(statusCode));
            }
            else
            {
                response.Setup(s => s.IsSuccessful)
                    .Returns(IsSuccessCode(statusCode) && ResponseStatus == ResponseStatus.Completed);
            }

            return response;
        }

        /// <summary>
        /// Used when you want the API to return you a certain status code.
        ///     Default: HttpStatusCode.Ok
        /// </summary>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse"/> that will be returned from the builder pattern.</param>
        /// <param name="statusCode" cref="HttpStatusCode">HTTP Status Code that you want the mocked API to return for you.</param>
        /// <returns cref="Mock{IRestResponse}">The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse> WithStatusCode(this Mock<IRestResponse> response, HttpStatusCode statusCode)
        {
            StatusCode = statusCode;

            response.Setup(s => s.StatusCode).Returns(statusCode);

            if (ResponseStatus == ResponseStatus.None)
            {
                response.Setup(s => s.IsSuccessful).Returns(IsSuccessCode(statusCode));
            }
            else
            {
                response.Setup(s => s.IsSuccessful)
                    .Returns(IsSuccessCode(statusCode) && ResponseStatus == ResponseStatus.Completed);
            }

            return response;
        }

        /// <summary>
        /// Used when you want the API to return you a specific error message.
        /// </summary>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse"/> that will be returned from the builder pattern.</param>
        /// <param name="errorMessage" cref="string">Error message that you want the mocked API to return for you.</param>
        /// <returns cref="Mock{IRestResponse}">The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse> WithErrorMessage(this Mock<IRestResponse> response, string errorMessage)
        {
            response.Setup(s => s.ErrorMessage).Returns(errorMessage);
            return response;
        }

        /// <summary>
        /// Used when you want the API to return you a specific error message.
        /// </summary>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="errorMessage" cref="string">Error message that you want the mocked API to return for you.</param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse<T>> WithErrorMessage<T>(this Mock<IRestResponse<T>> response, string errorMessage)
        {
            response.Setup(s => s.ErrorMessage).Returns(errorMessage);
            return response;
        }

        #endregion

        #region Content Encoding

        /// <summary>
        /// Used when you want the API to return you a specific Content Encoding
        /// </summary>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse"/> that will be returned from the builder pattern.</param>
        /// <param name="contentEncoding" cref="string">The Content Encoding string that you want the mocked API to return for you.</param>
        /// <returns cref="Mock{IRestResponse}">The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse> WithContentEncoding(this Mock<IRestResponse> response, string contentEncoding)
        {
            response.Setup(s => s.ContentEncoding).Returns(contentEncoding);
            return response;
        }

        /// <summary>
        /// Used when you want the API to return you a specific Content Encoding
        /// </summary>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="contentEncoding" cref="string">The Content Encoding string that you want the mocked API to return for you.</param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse<T>> WithContentEncoding<T>(this Mock<IRestResponse<T>> response, string contentEncoding)
        {
            response.Setup(s => s.ContentEncoding).Returns(contentEncoding);
            return response;
        }

        #endregion

        #region Content Type

        /// <summary>
        /// Used when you want the API to return you a specific Content Ctype
        /// </summary>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse"/> that will be returned from the builder pattern.</param>
        /// <param name="contentType" cref="string">The Content Type string that you want the mocked API to return for you.</param>
        /// <returns cref="Mock{IRestResponse}">The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse> WithContentType(this Mock<IRestResponse> response, string contentType)
        {
            response.Setup(s => s.ContentType).Returns(contentType);
            return response;
        }

        /// <summary>
        /// Used when you want the API to return you a specific Content Type
        /// </summary>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="contentType" cref="string">The Content Type string that you want the mocked API to return for you.</param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse<T>> WithContentType<T>(this Mock<IRestResponse<T>> response, string contentType)
        {
            response.Setup(s => s.ContentEncoding).Returns(contentType);
            return response;
        }


        #endregion

        #region Server

        /// <summary>
        /// Used when you want the API to return you a specific server on the response object
        /// </summary>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="server" cref="string">The Server string that you want the mocked API to return for you.</param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse> WithServer(this Mock<IRestResponse> response, string server)
        {
            Server = server;
            response.Setup(s => s.Server).Returns(server);
            return response;
        }

        /// <summary>
        /// Used when you want the API to return you a specific server on the response object
        /// </summary>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="server" cref="string">The Server string that you want the mocked API to return for you.</param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse<T>> WithServer<T>(this Mock<IRestResponse<T>> response, string server)
        {
            Server = server;
            response.Setup(s => s.Server).Returns(server);
            return response;
        }

        #endregion

        #region Protocol Version

        /// <summary>
        /// Used when you want the API to return you a specific server on the response object
        /// </summary>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="protocolVersion" cref="Version">Not all underlying frameworks support this but this will return you a protocol version on the response.</param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse> WithProtocolVersion(this Mock<IRestResponse> response, Version protocolVersion)
        {
            response.Setup(s => s.ProtocolVersion).Returns(protocolVersion);
            return response;
        }

        /// <summary>
        /// Used when you want the API to return you a specific server on the response object
        /// </summary>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="protocolVersion" cref="Version">Not all underlying frameworks support this but this will return you a protocol version on the response.</param>
        /// <returns>The response that you want back from the mocked API</returns>
        public static Mock<IRestResponse<T>> WithProtocolVersion<T>(this Mock<IRestResponse<T>> response, Version protocolVersion)
        {
            response.Setup(s => s.ProtocolVersion).Returns(protocolVersion);
            return response;
        }

        #endregion


        #region Response Returns

        /// <summary>
        /// Used when the API just returns you a plain JSON string and you are not casting it to a specific object.
        /// </summary>
        /// <example>
        /// <code>
        ///     RestClient.Execute(request)
        /// </code>
        /// </example>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse"/> that will be returned from the builder pattern.</param>
        /// <param name="json" cref="string">JSON value(as a string) that the mocked API will return for you.</param>
        /// <returns cref="Mock{IRestResponse}">The response content that you want returned from the mocked API</returns>
        public static Mock<IRestResponse> ReturnsJsonString(this Mock<IRestResponse> response, string json)
        {
            response.Setup(s => s.Content).Returns(json);

            return response;
        }

        /// <summary>
        /// Used when the API returns you execute a response and cast that response to an object.
        ///     This will use Newtonsoft.Json to serialize the object as a JSON string for response.Content as well
        /// </summary>
        /// <example>
        /// <code>
        ///     RestClient.Execute&#60;MyObject&#62;(request)
        /// </code>
        /// </example>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <param name="fakeReturnValue">The objects in the response that you want returned from the mocked API</param>
        /// <returns>The response content and data that you want returned from the mocked API.</returns>
        public static Mock<IRestResponse<T>> Returns<T>(this Mock<IRestResponse<T>> response, T fakeReturnValue)
        {
            var json = JsonConvert.SerializeObject(fakeReturnValue);

            response.Setup(s => s.Content).Returns(json);
            response.Setup(s => s.Data).Returns(fakeReturnValue);

            return response;
        }

        #endregion


        #region Execute

        /// <summary>
        /// Used when ready to build and mock the RestClient API call
        /// </summary>
        /// <remarks>Use when you are using RestClient.Execute(request)</remarks>
        /// <example>
        /// <code>
        /// //In your testing class you can get this response and query the request
        /// public class Test
        /// {
        ///     public void Test_One()
        ///     {
        ///         var response = MockRestClient
        ///             .MockApiResponse()
        ///             .WithStatusCode(HttpStatusCode.Accepted)
        ///             .ReturnsJsonString("ok")
        ///             .MockExecute()
        ///
        ///         response.Request.Method.ShouldBe(GET)
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse"/> that will be returned from the builder pattern.</param>
        /// <returns cref="IRestResponse">The RestSharp.Response object after the work has been complete</returns>
        public static IRestResponse MockExecute(this Mock<IRestResponse> response)
        {
            //If not set to Ok specifically, just default to Ok
            if (StatusCode == HttpStatusCode.OK) response.Setup(s => s.StatusCode).Returns(StatusCode);

            MockRestClient
                .Setup(s => s.Execute(It.IsAny<IRestRequest>()))
                .Callback<IRestRequest>((request) =>
                {
                    if (string.IsNullOrWhiteSpace(Server) && string.IsNullOrWhiteSpace(MockRestClient.Object.BaseUrl?.ToString())) response.Setup(s => s.Server).Returns(MockRestClient.Object.BaseUrl.ToString);

                        response
                        .Setup(s => s.Request)
                        .Returns(request);
                })
                .Returns(response.Object);

            return response.Object;
        }

        /// <summary>
        /// Used when ready to build and mock the RestClient API call
        /// </summary>
        /// <remarks>Use when you are using RestClient.Execute&#60;MyObject&#62;(request)</remarks>
        /// <example>
        /// <code>
        /// //In your testing class you can get this response and query the request
        /// public class Test
        /// {
        ///     public void Test_One()
        ///     {
        ///         var response = MockRestClient
        ///             .MockApiResponse&#60;MyObject&#62;()
        ///             .WithStatusCode(HttpStatusCode.Accepted)
        ///             .Returns(new &#60;MyObject&#62;())
        ///             .MockExecute()
        ///
        ///         response.Request.Method.ShouldBe(GET)
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <returns cref="IRestResponse{T}">The RestSharp.Response object after the work has been complete</returns>
        public static IRestResponse<T> MockExecute<T>(this Mock<IRestResponse<T>> response)
        {
            //If not set to Ok specifically, just default to Ok
            if (StatusCode == HttpStatusCode.OK) response.Setup(s => s.StatusCode).Returns(StatusCode);

            MockRestClient
                .Setup(s => s.Execute<T>(It.IsAny<IRestRequest>()))
                .Callback<IRestRequest>((request) =>
                {
                    if (string.IsNullOrWhiteSpace(Server) && string.IsNullOrWhiteSpace(MockRestClient.Object.BaseUrl?.ToString())) response.Setup(s => s.Server).Returns(MockRestClient.Object.BaseUrl.ToString);

                    response
                        .Setup(s => s.Request)
                        .Returns(request);
                })
                .Returns(response.Object);

            return response.Object;
        }

        /// <summary>
        /// Used when ready to build and mock the RestClient API call
        /// </summary>
        /// <remarks>Use when you are using RestClient.ExecuteAsync(request)</remarks>
        /// <example>
        /// <code>
        /// //In your testing class you can get this response and query the request
        /// public class Test
        /// {
        ///     public void Test_One()
        ///     {
        ///         var response = MockRestClient
        ///             .MockApiResponse()
        ///             .WithStatusCode(HttpStatusCode.Accepted)
        ///             .ReturnsJsonString("ok")
        ///             .MockExecuteAsync()
        ///
        ///         response.Request.Method.ShouldBe(GET)
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <returns cref="IRestResponse">The RestSharp.Response object after the work has been complete</returns>
        public static IRestResponse MockExecuteAsync(this Mock<IRestResponse> response)
        {
            //If not set to Ok specifically, just default to Ok
            if (StatusCode == HttpStatusCode.OK) response.Setup(s => s.StatusCode).Returns(StatusCode);

            MockRestClient
                .Setup(s => s.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .Callback<IRestRequest, CancellationToken>((request, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(Server) && string.IsNullOrWhiteSpace(MockRestClient.Object.BaseUrl?.ToString())) response.Setup(s => s.Server).Returns(MockRestClient.Object.BaseUrl.ToString);

                    response
                        .Setup(s => s.Request)
                        .Returns(request);
                })
                .ReturnsAsync(response.Object);

            return response.Object;
        }

        /// <summary>
        /// Used when ready to build and mock the RestClient API call
        /// </summary>
        /// <remarks>Use when you are using RestClient.ExecuteAsync&#60;MyObject&#62;(request)</remarks>
        /// <typeparam name="T">Generic class that you want the RestResponse to return you</typeparam>
        /// <example>
        /// <code>
        /// //In your testing class you can get this response and query the request
        /// public class Test
        /// {
        ///     public void Test_One()
        ///     {
        ///         var response = MockRestClient
        ///             .MockApiResponse&#60;MyObject&#62;()
        ///             .WithStatusCode(HttpStatusCode.Accepted)
        ///             .Returns(new &#60;MyObject&#62;())
        ///             .MockExecuteAsync()
        ///
        ///         response.Request.Method.ShouldBe(GET)
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="response">Extension method on the <see cref="Mock"/> of <see cref="IRestResponse{T}"/> that will be returned from the builder pattern.</param>
        /// <returns cref="IRestResponse">The RestSharp.Response object after the work has been complete</returns>
        public static IRestResponse MockExecuteAsync<T>(this Mock<IRestResponse<T>> response)
        {
            //If not set to Ok specifically, just default to Ok
            if (StatusCode == HttpStatusCode.OK) response.Setup(s => s.StatusCode).Returns(StatusCode);

            MockRestClient
                .Setup(s => s.ExecuteAsync<T>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .Callback<IRestRequest, CancellationToken>((request, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(Server) && string.IsNullOrWhiteSpace(MockRestClient.Object.BaseUrl?.ToString())) response.Setup(s => s.Server).Returns(MockRestClient.Object.BaseUrl.ToString);

                    response
                        .Setup(s => s.Request)
                        .Returns(request);
                })
                .ReturnsAsync(response.Object);

            return response.Object;
        }


        #endregion


        #region Private Methods

        private static bool IsSuccessCode(HttpStatusCode statusCode)
        {
            return (int) statusCode >= 200 &&
                   (int) statusCode <= 299;
        }
        #endregion
    }
}
