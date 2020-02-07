// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Subscriber.Handlers {
    using Microsoft.Azure.IIoT.OpcUa.Subscriber.Models;
    using Microsoft.Azure.IIoT.Hub;
    using Microsoft.Azure.IIoT.Serializers;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Publisher message handling
    /// </summary>
    public sealed class SubscriberCdmSampleHandler : IDeviceTelemetryHandler {

        /// <inheritdoc/>
        public string MessageSchema => Models.MessageSchemaTypes.LegacySubscriberSample;

        /// <summary>
        /// Create handler
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="serializer"></param>
        /// <param name="logger"></param>
        public SubscriberCdmSampleHandler(IEnumerable<ISubscriberSampleProcessor> handlers,
            IJsonSerializer serializer, ILogger logger) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _handlers = handlers?.ToList() ?? throw new ArgumentNullException(nameof(handlers));
        }

        /// <inheritdoc/>
        public async Task HandleAsync(string deviceId, string moduleId,
            byte[] payload, IDictionary<string, string> properties, Func<Task> checkpoint) {
            var json = Encoding.UTF8.GetString(payload);
            IEnumerable<VariantValue> messages;
            try {
                var parsed = _serializer.Parse(json);
                if (parsed.Type == VariantValueType.Array) {
                    messages = parsed.Values;
                }
                else {
                    messages = parsed.YieldReturn();
                }
            }
            catch (Exception ex) {
                _logger.Error(ex, "Failed to parse json {json}", json);
                return;
            }
            foreach (var message in messages) {
                try {
                    var sample = message.ToSubscriberSampleModel();
                    if (sample == null) {
                        continue;
                    }
                    await Task.WhenAll(_handlers.Select(h => h.OnSubscriberSampleAsync(
                        sample)));
                }
                catch (Exception ex) {
                    _logger.Error(ex,
                        "Subscriber message {message} failed with exception - skip",
                            message);
                }
            }
        }

        /// <inheritdoc/>
        public Task OnBatchCompleteAsync() {
            return Task.CompletedTask;
        }

        private readonly ILogger _logger;
        private readonly IJsonSerializer _serializer;
        private readonly List<ISubscriberSampleProcessor> _handlers;
    }
}