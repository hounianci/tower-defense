using System.Collections.Generic;
public class ScenarioPointConfig : AbstractConfig{
    public override object GetKey(AbstractConfigEntry t)
    {
        ScenarioPointConfigEntry configEntry = (ScenarioPointConfigEntry) t;
        return configEntry.Id;
    }
    protected override string GetName()
    {
        return "ScenarioPoint";
    }
    protected override AbstractConfigEntry CreateInstance()
    {
        return new ScenarioPointConfigEntry();
    }
}