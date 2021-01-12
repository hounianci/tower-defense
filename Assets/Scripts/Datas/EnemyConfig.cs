using System.Collections.Generic;
public class EnemyConfig : AbstractConfig{
    public override object GetKey(AbstractConfigEntry t)
    {
        EnemyConfigEntry configEntry = (EnemyConfigEntry) t;
        return configEntry.Id;
    }
    protected override string GetName()
    {
        return "Enemy";
    }
    protected override AbstractConfigEntry CreateInstance()
    {
        return new EnemyConfigEntry();
    }
}