// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Edge.Discovery.Services {
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using Microsoft.Azure.IIoT.Module;
    using Microsoft.Azure.IIoT.Tasks;
    using Microsoft.Azure.IIoT.Utils;
    using Microsoft.Azure.IIoT.Hub;
    using Serilog;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Discovery progress message sender
    /// </summary>
    public class ProgressPublisher : ProgressLogger, IDiscoveryProgress {

        /// <summary>
        /// Create listener
        /// </summary>
        /// <param name="events"></param>
        /// <param name="processor"></param>
        /// <param name="logger"></param>
        public ProgressPublisher(IEventEmitter events, ITaskProcessor processor,
            ILogger logger) : base (logger) {
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        /// <summary>
        /// Send progress
        /// </summary>
        /// <param name="progress"></param>
        protected override void Send(DiscoveryProgressModel progress) {
            progress.DiscovererId = DiscovererModelEx.CreateDiscovererId(
                _events.DeviceId, _events.ModuleId);
            base.Send(progress);
            _processor.TrySchedule(() => SendAsync(progress));
        }

        /// <summary>
        /// Send progress
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        private Task SendAsync(DiscoveryProgressModel progress) {
            return Try.Async(() => _events.SendJsonEventAsync(
                progress, MessageSchemaTypes.DiscoveryMessage));
        }

        private readonly IEventEmitter _events;
        private readonly ITaskProcessor _processor;
    }
}
