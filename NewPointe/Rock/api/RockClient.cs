using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NewPointe.Rock.Api
{

    class RockClient
    {

        private readonly HttpClient client;

        public RockClient(string server)
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri(server);
            client.DefaultRequestHeaders.Add("User-Agent", "NewPointe.Rock.Api.RockClient");
        }

    }

}