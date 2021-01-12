using System.Collections.Generic;
public class SpawnWaveConfig : AbstractConfig{
    public override object GetKey(AbstractConfigEntry t)
    {
        SpawnWaveConfigEntry configEntry = (SpawnWaveConfigEntry) t;
        return configEntry.Id;
    }
    protected override string GetName()
    {
        return "SpawnWave";
    }
    protected override AbstractConfigEntry CreateInstance()
    {
        return new SpawnWaveConfigEntry();
    }
}