﻿using System;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Client;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace NonInteractivePatGenerationSample
{
    public class Program
    {
        // This is the resource ID for the VSTS application - don't change this.
        private const string VstsResourceId = "499b84ac-1321-427f-aa17-267ca6975798";

        public static async Task Main(string[] args)
        {
            //var username = "[your AAD username]"; // This is your AAD username in the form user@domain.com.
            //var password = "[your AAD password]"; // This is your AAD password.
            var aadApplicationID = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";// "[your AAD application ID]"; // Created when you register an AAD application: https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-integrating-applications.

            //var adalCredential = new UserPasswordCredential(username, password);

            var authenticationContext = new AuthenticationContext("https://login.microsoftonline.com/common");
            var result = authenticationContext.AcquireTokenAsync(VstsResourceId, aadApplicationID, new UserCredential()).Result;

            var token = new VssAadToken(result);
            var vstsCredential = new VssAadCredential(token);

            var connection = new VssConnection(new Uri("https://spsprodwcus0.vssps.visualstudio.com/A850a26fd-8300-ce32-bb6e-28e032a3a0fd"), vstsCredential);
            var client = connection.GetClient<TokenHttpClient>();

            var pat = client.CreateSessionTokenAsync(
                new SessionToken()
                {
                    DisplayName = "Generated by sample code " + DateTime.UtcNow.ToString("o"),
                    Scope = "vso.code",
                    TargetAccounts = new Guid[] {
                        Guid.Parse("0efb4611-d565-4cd1-9a64-7d6cb6d7d5f0"), //mseng
                        Guid.Parse("8b119ea1-2e2a-4839-8db7-8c9e8d50f6fa"), //msdata
                    },
                },
                SessionTokenType.Compact,
                isPublic: false
                ).Result;

            Console.WriteLine(pat.Token);
        }
    }
}
