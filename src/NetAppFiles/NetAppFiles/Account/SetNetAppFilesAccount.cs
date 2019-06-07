﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System.Collections;
using System.Management.Automation;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Commands.NetAppFiles.Common;
using Microsoft.Azure.Commands.NetAppFiles.Models;
using Microsoft.Azure.Management.NetApp;
using Microsoft.Azure.Management.NetApp.Models;
using Microsoft.Azure.Commands.NetAppFiles.Helpers;
using System.Collections.Generic;

// Note:
// Both set and Update need to exist
// Patch of active directories can only alter the content
// to remove the active directory a put is required

namespace Microsoft.Azure.Commands.NetAppFiles.Account
{
    [Cmdlet(
        "Set",
        ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "NetAppFilesAccount",
        SupportsShouldProcess = true,
        DefaultParameterSetName = FieldsParameterSet), OutputType(typeof(PSNetAppFilesAccount))]
    [Alias("Set-AnfAccount")]
    public class SetAzureRmNetAppFilesAccount : AzureNetAppFilesCmdletBase
    {
        [Parameter(
            Mandatory = true,
            ParameterSetName = "SetByResourceGroupName",
            HelpMessage = "The resource group of the ANF account")]
        [ValidateNotNullOrEmpty]
        [ResourceGroupCompleter()]
        public string ResourceGroupName { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The location of the resource")]
        [ValidateNotNullOrEmpty]
        [LocationCompleter("Microsoft.NetApp/netAppAccounts")]
        public string Location { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the ANF account")]
        [ValidateNotNullOrEmpty]
        [Alias("AccountName")]
        public string Name { get; set; }

        [Parameter(
            Mandatory = false,
            ParameterSetName = "SetByResourceActiveDirectory",
            HelpMessage = "A hashtable array which represents the active directories")]
        [ValidateNotNullOrEmpty]
        public PSNetAppFilesActiveDirectory[] ActiveDirectory { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "A hashtable which represents resource tags")]
        [ValidateNotNullOrEmpty]
        [Alias("Tags")]
        public Hashtable Tag { get; set; }

        public override void ExecuteCmdlet()
        {
            var netAppAccountBody = new NetAppAccount()
            {
                Location = Location,
                ActiveDirectories = (ActiveDirectory != null) ? ModelExtensions.ConvertActiveDirectoriesFromPs(ActiveDirectory) : new List<ActiveDirectory>(),
                Tags = Tag
            };

            if (ShouldProcess(Name, "Create the new account"))
            {
                var anfAccount = AzureNetAppFilesManagementClient.Accounts.CreateOrUpdate(netAppAccountBody, ResourceGroupName, Name);
                WriteObject(anfAccount.ToPsNetAppFilesAccount());
            }
        }
    }
}
