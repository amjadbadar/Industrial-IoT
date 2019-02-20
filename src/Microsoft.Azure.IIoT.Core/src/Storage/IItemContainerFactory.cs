// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Storage {
    using System.Threading.Tasks;

    /// <summary>
    /// Injectable container
    /// </summary>
    public interface IItemContainerFactory {

        /// <summary>
        /// Create container
        /// </summary>
        /// <returns></returns>
        Task<IItemContainer> OpenAsync();
    }
}
