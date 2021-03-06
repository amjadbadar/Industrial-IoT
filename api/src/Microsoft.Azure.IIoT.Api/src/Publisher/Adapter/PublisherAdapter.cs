// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Api.Publisher {
    using Microsoft.Azure.IIoT.OpcUa.Api.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Publisher;
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Implements node and publish services as adapter on top of api.
    /// </summary>
    public sealed class PublisherAdapter : IPublishServices<string> {

        /// <summary>
        /// Create adapter
        /// </summary>
        /// <param name="client"></param>
        public PublisherAdapter(IPublisherServiceApi client) {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <inheritdoc/>
        public async Task<PublishStartResultModel> NodePublishStartAsync(
            string endpoint, PublishStartRequestModel request) {
            var result = await _client.NodePublishStartAsync(endpoint,
                request.Map<PublishStartRequestApiModel>());
            return result.Map<PublishStartResultModel>();
        }

        /// <inheritdoc/>
        public async Task<PublishStopResultModel> NodePublishStopAsync(
            string endpoint, PublishStopRequestModel request) {
            var result = await _client.NodePublishStopAsync(endpoint,
                request.Map<PublishStopRequestApiModel>());
            return result.Map<PublishStopResultModel>();
        }

        /// <inheritdoc/>
        public async Task<PublishedItemListResultModel> NodePublishListAsync(
            string endpoint, PublishedItemListRequestModel request) {
            var result = await _client.NodePublishListAsync(endpoint,
                request.Map<PublishedItemListRequestApiModel>());
            return result.Map<PublishedItemListResultModel>();
        }

        private readonly IPublisherServiceApi _client;
    }
}
