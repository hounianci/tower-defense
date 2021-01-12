using System.Collections.Generic;
public class SpawnSequenceConfig : AbstractConfig{
    public override object GetKey(AbstractConfigEntry t)
    {
        SpawnSequenceConfigEntry configEntry = (SpawnSequenceConfigEntry) t;
        return configEntry.Id;
    }
    protected override string GetName()
    {
        return "SpawnSequence";
    }
    protected override AbstractConfigEntry CreateInstance()
    {
        return new SpawnSequenceConfigEntry();
    }
}