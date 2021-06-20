using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Test
{
    public class SolanaRpcClientTestBase
    {
        protected const string TestnetUrl = "https://testnet.solana.com";
        protected static readonly Uri TestnetUri = new Uri(TestnetUrl);

        /// <summary>
        /// Finish the test by asserting the http request went as expected.
        /// </summary>
        /// <param name="expectedUri">The request uri.</param>
        protected void FinishTest(Mock<HttpMessageHandler> mockHandler, Uri expectedUri)
        {
            mockHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post
                    && req.RequestUri == expectedUri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        /// <summary>
        /// Setup the test with the request and response data.
        /// </summary>
        /// <param name="sentPayloadCapture">Capture the sent content.</param>
        /// <param name="responseContent">The response content.</param>
        protected Mock<HttpMessageHandler> SetupTest(Action<string> sentPayloadCapture, string responseContent)
        {
            var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(
                        message => message.Method == HttpMethod.Post &&
                                   message.RequestUri == TestnetUri),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((httpRequest, ct) =>
                    sentPayloadCapture(httpRequest.Content.ReadAsStringAsync(ct).Result))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();
            return messageHandlerMock;
        }
    }
}