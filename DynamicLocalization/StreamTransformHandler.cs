namespace Swashbuckle.DynamicLocalization
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Delegating handler that applies a transformation to the outgoing content stream.
    /// </summary>
    public class StreamTransformHandler : DelegatingHandler
    {
        private HttpMessageHandler _innerHandler;
        private Func<Stream, Stream> _transform;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingHandler"/> class.
        /// </summary>
        /// <param name="innerHandler">The inner handler to invoke.</param>
        /// <param name="transform">The transformation to apply to the response content set by the inner handler.</param>
        public StreamTransformHandler(HttpMessageHandler innerHandler, Func<Stream, Stream> transform)
            : base(innerHandler)
        {
            _innerHandler = innerHandler;
            _transform = transform;
        }

        /// <inheritdoc />
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            // TODO: At odds with the name of the class. Either change the class name or the transform signature.
            if (response.Content.Headers.ContentType.MediaType.StartsWith("text"))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.Content = new StreamContent(_transform(stream));
            }
            
            return response;
        }
    }
}
