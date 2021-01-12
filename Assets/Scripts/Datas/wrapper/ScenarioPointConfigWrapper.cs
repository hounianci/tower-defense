using System.Collections;
using System.Collections.Generic;

public class ScenarioPointConfigWrapper : ScenarioPointConfig
{

    public List<ScenarioPointConfigEntry> GetByScenarioId(int scenarioId){
        List<ScenarioPointConfigEntry> result = new List<ScenarioPointConfigEntry>();
        foreach(ScenarioPointConfigEntry entry in Datas.Values){
            if(scenarioId == entry.ScenarioId){
                result.Add(entry);
            }
        }
        return result;
    }

}
