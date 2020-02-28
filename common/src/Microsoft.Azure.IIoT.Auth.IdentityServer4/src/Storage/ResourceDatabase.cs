﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Auth.IdentityServer4.Storage {
    using Microsoft.Azure.IIoT.Auth.IdentityServer4.Models;
    using Microsoft.Azure.IIoT.Storage;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::IdentityServer4.Models;
    using global::IdentityServer4.Stores;

    /// <summary>
    /// Resource store
    /// </summary>
    public class ResourceDatabase : IResourceStore {

        /// <summary>
        /// Create resource store
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="logger"></param>
        public ResourceDatabase(IItemContainerFactory factory, ILogger logger) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }
            _documents = factory.OpenAsync("resources").Result.AsDocuments();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(
            IEnumerable<string> scopeNames) {
            var client = _documents.OpenSqlClient();
            var results = client.Query<ClientDocumentModel>(
                CreateQuery(out var queryParameters, scopeNames, nameof(IdentityResource)),
                    queryParameters);

            var identityResources = new List<IdentityResource>();
            while (results.HasMore()) {
                var documents = await results.ReadAsync();
                var resources = documents.Select(d => d.Value.ToServiceModel()).ToList();
                identityResources.AddRange(resources.OfType<IdentityResource>());
            }
            return identityResources;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(
            IEnumerable<string> scopeNames) {
            var client = _documents.OpenSqlClient();
            var results = client.Query<ClientDocumentModel>(
                CreateQuery(out var queryParameters, scopeNames, nameof(ApiResource)),
                    queryParameters);

            var apiResources = new List<ApiResource>();
            while (results.HasMore()) {
                var documents = await results.ReadAsync();
                var resources = documents.Select(d => d.Value.ToServiceModel()).ToList();
                apiResources.AddRange(resources.OfType<ApiResource>());
            }
            return apiResources;
        }

        /// <inheritdoc/>
        public async Task<ApiResource> FindApiResourceAsync(string name) {
            var client = await _documents.FindAsync<ResourceDocumentModel>(name);
            if (client?.Value == null) {
                return null;
            }
            return client.Value.ToServiceModel() as ApiResource;
        }

        /// <inheritdoc/>
        public async Task<Resources> GetAllResourcesAsync() {
            var client = _documents.OpenSqlClient();
            var results = client.Query<ClientDocumentModel>("SELECT * FROM r");
            var apiResources = new List<ApiResource>();
            var identityResources = new List<IdentityResource>();
            while (results.HasMore()) {
                var documents = await results.ReadAsync();
                var resources = documents.Select(d => d.Value.ToServiceModel()).ToList();
                apiResources.AddRange(resources.OfType<ApiResource>());
                identityResources.AddRange(resources.OfType<IdentityResource>());
            }
            return new Resources {
                IdentityResources = identityResources,
                ApiResources = apiResources
            };
        }

        /// <summary>
        /// Create query
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <param name="scopeNames"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        private static string CreateQuery(out Dictionary<string, object> queryParameters,
            IEnumerable<string> scopeNames, string resourceType) {
            queryParameters = new Dictionary<string, object> {
                { "@scopes", scopeNames
                      .Select(s => s.ToLowerInvariant()).ToList() }
            };
            var queryString = $"SELECT * FROM r WHERE ";
            queryString +=
$"r.{nameof(ResourceDocumentModel.Name)} IN (@scopes)' AND ";
            queryString +=
$"r.{nameof(ResourceDocumentModel.ResourceType)} = '{resourceType}'";
            return queryString;
        }

        private readonly ILogger _logger;
        private readonly IDocuments _documents;
    }
}