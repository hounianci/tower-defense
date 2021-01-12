using System.Collections;
using System.Collections.Generic;
public class SpawnSequenceConfigWrapper : SpawnSequenceConfig
{
    public List<SpawnSequenceConfigEntry> GetByWaveId(int waveId){
        List<SpawnSequenceConfigEntry> result = new List<SpawnSequenceConfigEntry>();
        foreach(SpawnSequenceConfigEntry entry in Datas.Values){
            if(waveId == entry.WaveId){
                result.Add(entry);
            }
        }
        return result;
    }
}
