using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PostHog.Exceptions;
using PostHog.Model;

namespace PostHog.Request
{
    internal class BlockingRequestHandler : IRequestHandler
    {
        /// <summary>
        /// Segment.io client to mark statistics
        /// </summary>
        private readonly PostHogClient _client;

        private readonly Backoff _backoff;

        private readonly HttpClient _httpClient;

        /// <summary>
        /// The maximum amount of time to wait before calling
        /// the HTTP flush a timeout failure.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        internal BlockingRequestHandler(PostHogClient client, TimeSpan timeout) : this(client, timeout, new Backoff(max: 10000, jitter: 5000))
        {
        }

        internal BlockingRequestHandler(PostHogClient client, TimeSpan timeout, Backoff backoff)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _backoff = backoff;

            Timeout = timeout;

            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

            var userAgent = _client.Config.UserAgent;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        public async Task MakeRequest(Batch batch)
        {
            var watch = new Stopwatch();
            _backoff.Reset();
            try
            {
                var uriBuilder = new UriBuilder
                {
                    Host = _client.Config.Host,
                    Scheme = "https",
                    Path = "batch"
                };

                var uri = uriBuilder.Uri;

                var json = JsonConvert.SerializeObject(batch);

                // Prepare request data;
                var requestData = Encoding.UTF8.GetBytes(json);

                using (var memory = new MemoryStream())
                {
                    using (var gzip = new GZipStream(memory, CompressionMode.Compress, true))
                    {
                        gzip.Write(requestData, 0, requestData.Length);
                    }

                    requestData = memory.ToArray();
                }

                // Retries with exponential backoff
                var statusCode = HttpStatusCode.OK;
                var responseStr = "";

                while (!_backoff.HasReachedMax)
                {
                    watch.Start();

                    var content = new ByteArrayContent(requestData);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    content.Headers.ContentEncoding.Add("gzip");

                    HttpResponseMessage? response = null;
                    var retry = false;
                    try
                    {
                        response = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        responseStr = ex.Message;
                        retry = true;
                    }
                    
                    watch.Stop();
                    statusCode = response?.StatusCode ?? HttpStatusCode.InternalServerError;

                    if (response is { StatusCode: HttpStatusCode.OK })
                    {
                        Succeed(batch);
                        break;
                    }

                    responseStr = response?.ReasonPhrase;

                    if ((int)statusCode >= 500 && (int)statusCode <= 600 || statusCode == HttpStatusCode.TooManyRequests || retry)
                    {
                        // If status code is greater than 500 and less than 600, it indicates server error
                        // Error code 429 indicates rate limited.
                        // Retry uploading in these cases.
                        await _backoff.AttemptAsync();
                    }
                    else
                    {
                        //HTTP status codes smaller than 500 or greater than 600 except for 429 are either Client errors or a correct status
                        //This means it should not retry
                        break;
                    }
                }

                var hasBackoffReachedMax = _backoff.HasReachedMax;
                if (hasBackoffReachedMax || statusCode != HttpStatusCode.OK)
                {
                    var message = $"Has Backoff reached max: {hasBackoffReachedMax} with number of Attempts:{_backoff.CurrentAttempt},\nStatus Code: {statusCode},\nReason: {responseStr}";
                    Fail(batch, new ApiException(statusCode.ToString(), message));
                }
            }
            catch (Exception e)
            {
                watch.Stop();
                Fail(batch, e);
            }
        }

        private void Fail(Batch batch, Exception e)
        {
            foreach (var action in batch.Actions)
            {
                _client.Statistics.IncrementFailed();
                _client.RaiseFailure(action, e);
            }
        }

        private void Succeed(Batch batch)
        {
            foreach (var action in batch.Actions)
            {
                _client.Statistics.IncrementSucceeded();
                _client.RaiseSuccess(action);
            }
        }
    }
}