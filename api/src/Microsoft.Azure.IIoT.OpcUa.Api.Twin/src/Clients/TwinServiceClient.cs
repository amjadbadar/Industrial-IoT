// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Api.Twin.Clients {
    using Microsoft.Azure.IIoT.OpcUa.Api.Twin.Models;
    using Microsoft.Azure.IIoT.Http;
    using Microsoft.Azure.IIoT.Serializers;
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Implementation of twin service api.
    /// </summary>
    public sealed class TwinServiceClient : ITwinServiceApi {

        /// <summary>
        /// Create service client
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="config"></param>
        /// <param name="serializer"></param>
        public TwinServiceClient(IHttpClient httpClient, ITwinConfig config,
            ISerializer serializer) : this(httpClient,
                config?.OpcUaTwinServiceUrl, config?.OpcUaTwinServiceResourceId,
                serializer) {
        }

        /// <summary>
        /// Create service client
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="serviceUri"></param>
        /// <param name="resourceId"></param>
        /// <param name="serializer"></param>
        public TwinServiceClient(IHttpClient httpClient, string serviceUri, string resourceId,
            ISerializer serializer = null) {
            _serviceUri = serviceUri ?? throw new ArgumentNullException(nameof(serviceUri),
                    "Please configure the Url of the endpoint micro service.");
            _resourceId = resourceId;
            _serializer = serializer ?? new NewtonSoftJsonSerializer();
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc/>
        public async Task<string> GetServiceStatusAsync(CancellationToken ct) {
            var request = _httpClient.NewRequest($"{_serviceUri}/healthz",
                _resourceId);
            var response = await _httpClient.GetAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<string>(response);
        }

        /// <inheritdoc/>
        public async Task<BrowseResponseApiModel> NodeBrowseFirstAsync(string endpointId,
            BrowseRequestApiModel content, CancellationToken ct) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v2/browse/{endpointId}",
                _resourceId);
            _serializer.SerializeToRequest(request, content);
            var response = await _httpClient.PostAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<BrowseResponseApiModel>(response);
        }

        /// <inheritdoc/>
        public async Task<BrowseNextResponseApiModel> NodeBrowseNextAsync(string endpointId,
            BrowseNextRequestApiModel content, CancellationToken ct) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }
            if (content.ContinuationToken is null) {
                throw new ArgumentNullException(nameof(content.ContinuationToken));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v2/browse/{endpointId}/next",
                _resourceId);
            _serializer.SerializeToRequest(request, content);
            var response = await _httpClient.PostAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<BrowseNextResponseApiModel>(response);
        }

        /// <inheritdoc/>
        public async Task<BrowsePathResponseApiModel> NodeBrowsePathAsync(string endpointId,
            BrowsePathRequestApiModel content, CancellationToken ct) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }
            if (content.BrowsePaths is null || content.BrowsePaths.Count == 0 ||
                content.BrowsePaths.Any(p => p is null || p.Length == 0)) {
                throw new ArgumentNullException(nameof(content.BrowsePaths));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v2/browse/{endpointId}/path",
                _resourceId);
            _serializer.SerializeToRequest(request, content);
            var response = await _httpClient.PostAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<BrowsePathResponseApiModel>(response);
        }

        /// <inheritdoc/>
        public async Task<ReadResponseApiModel> NodeReadAsync(string endpointId,
            ReadRequestApiModel content, CancellationToken ct) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }
            if (content.Attributes is null || content.Attributes.Count == 0) {
                throw new ArgumentException(nameof(content.Attributes));
            }
            var request = _httpClient.NewRequest(
                $"{_serviceUri}/v2/read/{endpointId}/attributes", _resourceId);
            _serializer.SerializeToRequest(request, content);
            var response = await _httpClient.PostAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<ReadResponseApiModel>(response);
        }

        /// <inheritdoc/>
        public async Task<WriteResponseApiModel> NodeWriteAsync(string endpointId,
            WriteRequestApiModel content, CancellationToken ct) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }
            if (content.Attributes is null || content.Attributes.Count == 0) {
                throw new ArgumentException(nameof(content.Attributes));
            }
            var request = _httpClient.NewRequest(
                $"{_serviceUri}/v2/write/{endpointId}/attributes", _resourceId);
            _serializer.SerializeToRequest(request, content);
            var response = await _httpClient.PostAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<WriteResponseApiModel>(response);
        }

        /// <inheritdoc/>
        public async Task<ValueReadResponseApiModel> NodeValueReadAsync(string endpointId,
            ValueReadRequestApiModel content, CancellationToken ct) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v2/read/{endpointId}",
                _resourceId);
            _serializer.SerializeToRequest(request, content);
            var response = await _httpClient.PostAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<ValueReadResponseApiModel>(response);
        }

        /// <inheritdoc/>
        public async Task<ValueWriteResponseApiModel> NodeValueWriteAsync(string endpointId,
            ValueWriteRequestApiModel content, CancellationToken ct) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }
            if (content.Value is null) {
                throw new ArgumentNullException(nameof(content.Value));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v2/write/{endpointId}",
                _resourceId);
            _serializer.SerializeToRequest(request, content);
            var response = await _httpClient.PostAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<ValueWriteResponseApiModel>(response);
        }

        /// <inheritdoc/>
        public async Task<MethodMetadataResponseApiModel> NodeMethodGetMetadataAsync(
            string endpointId, MethodMetadataRequestApiModel content, CancellationToken ct) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v2/call/{endpointId}/metadata",
                _resourceId);
            _serializer.SerializeToRequest(request, content);
            var response = await _httpClient.PostAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<MethodMetadataResponseApiModel>(response);
        }

        /// <inheritdoc/>
        public async Task<MethodCallResponseApiModel> NodeMethodCallAsync(
            string endpointId, MethodCallRequestApiModel content, CancellationToken ct) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v2/call/{endpointId}",
                _resourceId);
            _serializer.SerializeToRequest(request, content);
            var response = await _httpClient.PostAsync(request, ct).ConfigureAwait(false);
            response.Validate();
            return _serializer.DeserializeResponse<MethodCallResponseApiModel>(response);
        }

        private readonly IHttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly string _serviceUri;
        private readonly string _resourceId;
    }
}
