class concretePlatformFactory : platformFactory
{
    public override platform createPlatform(string platform_name)
    {
        return new platform(this.eventFactories[platform_name], this.eventProviders[platform_name]);
    }
}