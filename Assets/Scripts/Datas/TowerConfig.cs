public class TowerConfig : AbstractConfig{
    public override object GetKey(AbstractConfigEntry t)
    {
        TowerConfigEntry configEntry = (TowerConfigEntry) t;
        return configEntry.Id;
    }
    protected override string GetName()
    {
        return "Tower";
    }
    protected override AbstractConfigEntry CreateInstance()
    {
        return new TowerConfigEntry();
    }
}