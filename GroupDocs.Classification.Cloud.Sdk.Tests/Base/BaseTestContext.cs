﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Aspose" file="BaseTestContext.cs">
// //   Copyright (c) 2018 Aspose.Words for Cloud
// // </copyright>
// // <summary>
// //   Permission is hereby granted, free of charge, to any person obtaining a copy
// //  of this software and associated documentation files (the "Software"), to deal
// //  in the Software without restriction, including without limitation the rights
// //  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// //  copies of the Software, and to permit persons to whom the Software is
// //  furnished to do so, subject to the following conditions:
// // 
// //  The above copyright notice and this permission notice shall be included in all
// //  copies or substantial portions of the Software.
// // 
// //  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// //  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// //  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// //  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// //  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// //  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// //  SOFTWARE.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace GroupDocs.Classification.Cloud.Sdk.Tests.Base
{
    using System;
    using System.IO;

    using GroupDocs.Classification.Cloud.Sdk.Api;
    using GroupDocs.Storage.Cloud.Sdk.Api;
    using GroupDocs.Storage.Cloud.Sdk.Model.Requests;

    using Newtonsoft.Json;

    /// <summary>
    /// Base class for all tests
    /// </summary>
    public abstract class BaseTestContext
    {        
        protected static readonly string LocalTestDataFolder = DirectoryHelper.GetRootSdkFolder() + "/TestData/";
        private readonly Keys keys;        

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTestContext"/> class.
        /// </summary>
        protected BaseTestContext()
        {
            // To run tests with your own credentials please substitute code bellow with this one
            // this.keys = new Keys { AppKey = "your app key", AppSid = "your app sid" };
            var serverCreds = Path.Combine(DirectoryHelper.GetRootSdkFolder(), "Settings", "servercreds.json");
            this.keys = JsonConvert.DeserializeObject<Keys>(File.ReadAllText(serverCreds));
            if (this.keys == null)
            {
                throw new FileNotFoundException("servercreds.json doesn't contain AppKey and AppSid");
            }

            var configuration = new Configuration { ApiBaseUrl = this.keys.BaseUrl, AppKey = this.keys.AppKey, AppSid = this.keys.AppSid, AuthorizationUrl = this.keys.AuthorizationUrl };

            // Set configuration and requests timeout.
            this.ClassificationApi = new ClassificationApi(configuration);
            this.StorageApi = new StorageApi(new Storage.Cloud.Sdk.Configuration { AppKey = this.AppKey, AppSid = this.AppSid, ApiBaseUrl = this.keys.AuthorizationUrl, DebugMode = true });          
        }

        /// <summary>
        /// Base path to test data        
        /// </summary>
        protected static string RemoteBaseTestDataFolder
        {
            get
            {
                return "Temp/SdkTests/TestData";
            }
        }

        /// <summary>
        /// Base path to output data
        /// </summary>
        protected static string BaseTestOutPath
        {
            get
            {
                return "TestOut";
            }
        }

        /// <summary>
        /// Returns common folder with source test files
        /// </summary>
        protected static string CommonFolder
        {
            get
            {
                return "Common/";
            }
        }
       
        /// <summary>
        /// Storage API
        /// </summary>
        protected StorageApi StorageApi { get; set; }

        /// <summary>
        /// Words API
        /// </summary>
        protected ClassificationApi ClassificationApi { get; set; }

        /// <summary>
        /// AppSid
        /// </summary>
        protected string AppSid
        {
            get
            {
                return this.keys.AppSid;
            }
        }

        /// <summary>
        /// AppSid
        /// </summary>
        protected string AppKey
        {
            get
            {
                return this.keys.AppKey;
            }
        }

        /// <summary>
        /// Base Url for tests
        /// </summary>
        protected string BaseProductUri
        {
            get
            {
                return this.keys.BaseUrl;
            }
        }

        /// <summary>
        /// Returns test data path
        /// </summary>
        /// <param name="subfolder">subfolder for specific tests</param>
        /// <returns>test data path</returns>
        protected static string GetDataDir(string subfolder = null)
        {
            return Path.Combine(LocalTestDataFolder, string.IsNullOrEmpty(subfolder) ? string.Empty : subfolder);
        }

        /// <summary>
        /// Uploads file to storage.
        /// </summary>
        /// <param name="path">Path in storage.</param>
        /// <param name="fileContent">File content.</param>
        /// <param name="versionId">Api version.</param>
        /// <param name="storage">Storage.</param>
        protected void UploadFileToStorage(string path, byte[] fileContent, string versionId = null, string storage = null)
        {
            using (var ms = new MemoryStream(fileContent))
            {
                this.StorageApi.GetListFiles(new GetListFilesRequest());
                var request = new PutCreateRequest(path, ms, versionId, storage);
                var response = this.StorageApi.PutCreate(request);
                if (response?.Code != 200)
                {
                    throw new Exception("Can't upload file to the storage. Details: " + response.ToString());
                }
            }
        }
        
        private class Keys
        {
            public string AppSid { get; set; }

            public string AppKey { get; set; }

            public string BaseUrl { get; set; }

            public string AuthorizationUrl { get; set; }
        }
    }
}