using System.Collections.Generic;
using System.IO;
public class HunterQualityLevelConfig : AbstractConfig<HunterQualityLevelConfigEntry>{
    protected override string GetName()
    {
        return "HunterQualityLevel";
    }
    protected override HunterQualityLevelConfigEntry CreateInstance()
    {
        return new HunterQualityLevelConfigEntry();
    }
}