using System.Text.Json;
using System.Net.Http;
using System.Net;
interface abstractRouter {
    JsonElement performRequest(HttpRequestMessage msg, string platform);
    WebProxy getProxy(string platform);
}