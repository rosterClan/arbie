class local_con : connection
{
    public local_con(string containerId, int port, string loc) : base(containerId, port, loc) {}
    override public HttpClient produceClient() {
        return new HttpClient();
    }
}