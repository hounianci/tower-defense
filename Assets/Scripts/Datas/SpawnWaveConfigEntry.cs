using System.IO;
public class SpawnWaveConfigEntry : AbstractConfigEntry{
	public int Id{get;set;}
	public int ScenarioId{get;set;}
	public int TriggerType{get;set;}
	public int TriggerCondition{get;set;}

    protected override void Init0(BinaryReader br){
		Id= ReadInt32(br);
		ScenarioId= ReadInt32(br);
		TriggerType= ReadInt32(br);
		TriggerCondition= ReadInt32(br);

    }
}