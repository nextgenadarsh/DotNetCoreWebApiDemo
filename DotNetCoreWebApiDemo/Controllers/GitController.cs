using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetCoreWebApiDemo.Models;
using DotNetCoreWebApiDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApiDemo.Controllers
{
    [Route("api/git")]
    [ApiController]
    public class GitController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly GitService gitService;

        public GitController(IHttpClientFactory httpClientFactory, GitService gitService)
        {
            this.httpClientFactory = httpClientFactory;
            this.gitService = gitService;
        }

        [HttpGet("branches")]
        public async Task<ActionResult<IEnumerable<GitBranch>>> GetBranchesAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/nextgenadarsh/DotNetCoreWebApiDemo/branches");
            req.Headers.Add("Accept", "application/vnd.github.v2+json");
            req.Headers.Add("User-Agent", "HttpClientFactoryDemo");

            var client = httpClientFactory.CreateClient();

            var res = await client.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                using var resStream = await res.Content.ReadAsStreamAsync();
                var branches = await JsonSerializer.DeserializeAsync<IEnumerable<GitBranch>>(resStream);
                return branches.ToList();
            }

            return NotFound();
        }

        [HttpGet("branches-named")]
        public async Task<ActionResult<IEnumerable<GitBranch>>> GetBranchesUsingNamedClientAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "repos/nextgenadarsh/DotNetCoreWebApiDemo/branches");
            var client = httpClientFactory.CreateClient("github"); // Named client

            var res = await client.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                using var resStream = await res.Content.ReadAsStreamAsync();
                var branches = await JsonSerializer.DeserializeAsync<IEnumerable<GitBranch>>(resStream);
                return branches.ToList();
            }

            return NotFound();
        }

        [HttpGet("branches-typed")]
        public async Task<ActionResult<IEnumerable<GitBranch>>> GetBranchesUsingTypedClientAsync()
        {
            var branches = await gitService.GetBranches();
            return branches.ToList();
        }
    }

}
