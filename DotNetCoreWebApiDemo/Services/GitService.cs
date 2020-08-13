using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetCoreWebApiDemo.Models;
using Newtonsoft.Json;

namespace DotNetCoreWebApiDemo.Services
{
    public interface IGitService
    {
        public Task<IEnumerable<GitBranch>> GetBranches();

    }

    public class GitService : IGitService
    {
        private readonly HttpClient httpClient;

        public GitService(HttpClient httpClient)
        {
            // HttpClient can be configured while registering the HttpClient service
            //httpClient.BaseAddress = new Uri("https://api.github.com/");
            //httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v2+json");
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactoryDemo");

            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<GitBranch>> GetBranches()
        {
            var response = await httpClient.GetAsync("repos/nextgenadarsh/DotNetCoreWebApiDemo/branches");
            response.EnsureSuccessStatusCode();

            using var resStream = await response.Content.ReadAsStreamAsync();

            var jsonSerializer = new JsonSerializer();

            using var streamReader = new StreamReader(resStream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            
            return jsonSerializer.Deserialize<IEnumerable<GitBranch>>(jsonTextReader);
        }
    }
}
