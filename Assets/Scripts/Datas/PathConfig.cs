public class PathConfig : AbstractConfig{
    public override object GetKey(AbstractConfigEntry t)
    {
        PathConfigEntry configEntry = (PathConfigEntry) t;
        return configEntry.Id;
    }
    protected override string GetName()
    {
        return "Path";
    }
    protected override AbstractConfigEntry CreateInstance()
    {
        return new PathConfigEntry();
    }
}