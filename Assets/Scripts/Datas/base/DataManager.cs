using System.Collections.Generic;
using System.IO;
using System;

public class DataManager
{

    static Dictionary<Type, AbstractConfig> Datas = new Dictionary<Type, AbstractConfig>();

    public static void Init(){
        PathConfig pathConfig = new PathConfig();
        pathConfig.Init();
        Datas.Add(typeof(PathConfig), pathConfig);
        SkillConfig skillConfig = new SkillConfig();
        skillConfig.Init();
        Datas.Add(typeof(SkillConfig), skillConfig);
    }

    public static T GetData<T>(Type type, object key) where T : AbstractConfigEntry{
        return (T)Datas[type].Datas[key];
    }
}
