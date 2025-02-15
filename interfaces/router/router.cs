

using System.Net;
using System.Text.Json;

class router : abstractRouter
{
    private abstractRouter innerRouter; 
    private static readonly object _padlock = new object();
    private static router _instance;
    private router() {}

    public void setRouter(abstractRouter route) {
        this.innerRouter = route; 
    }

    public static router Instance
    {
        get
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new router();
                }
                return _instance;
            }
        }
    }
    
    public JsonElement performRequest(HttpRequestMessage msg, string platform)
    {
        return this.innerRouter.performRequest(msg, platform);
    }

    public WebProxy getProxy(string platform) {
        return this.innerRouter.getProxy(platform);
    }
}