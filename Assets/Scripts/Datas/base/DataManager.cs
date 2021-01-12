using System.Collections.Generic;
using System.IO;
using System;

public class DataManager
{

    private static Dictionary<Type, AbstractConfig> Datas = new Dictionary<Type, AbstractConfig>();

    public static void Init(){
        AddConfig(new PathConfig());
        AddConfig(new SkillConfig());
        AddConfig(new EnemyConfig());
        AddConfig(new ScenarioPointConfigWrapper());
        AddConfig(new SpawnSequenceConfigWrapper());
        AddConfig(new SpawnWaveConfig());
        AddConfig(new TowerConfig());
    }

    public static void AddConfig(AbstractConfig config){
        config.Init();
        Type type = config.GetType();
        Datas.Add(type, config);
        if(type.BaseType!=typeof(AbstractConfig)){
            type = type.BaseType;
            Datas.Add(type, config);
        }
    }

    public static T GetConfig<T> () where T : AbstractConfig{
        return (T)Datas[typeof(T)];
    }

    public static T GetData<T>(Type type, object key) where T : AbstractConfigEntry{
        return (T)Datas[type].Datas[key];
    }
}
