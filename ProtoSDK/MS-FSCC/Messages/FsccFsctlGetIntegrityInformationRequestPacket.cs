// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.Protocols.TestTools.StackSdk.FileAccessService.Fscc
{
    /// <summary>
    /// the request packet of FSCTL_GET_INTEGRITY_INFORMATION
    /// </summary>
    public class FsccFsctlGetIntegrityInformationRequestPacket : FsccEmptyPacket
    {
        #region Properties
        /// <summary>
        /// the command of fscc packet 
        /// </summary>
        public override uint Command
        {
            get
            {
                return (uint)FsControlCommand.FSCTL_GET_INTEGRITY_INFORMATION;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// default constructor 
        /// </summary>
        public FsccFsctlGetIntegrityInformationRequestPacket()
            : base()
        {
        }

        #endregion
    }
}
