using System.Collections.Generic;

public class SpawnWaveConfigWrapper : SpawnWaveConfig{
    
    public List<SpawnWaveConfigEntry> GetByScenarioId(int scenarioId){
        List<SpawnWaveConfigEntry> result = new List<SpawnWaveConfigEntry>();
        foreach(SpawnWaveConfigEntry entry in Datas.Values){
            if(entry.ScenarioId == scenarioId){
                result.Add(entry);
            }
        }
        return result;
    }

}
