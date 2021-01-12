using System.IO;
public class ScenarioPointConfigEntry : AbstractConfigEntry{
	public int Id{get;set;}
	public int ScenarioId{get;set;}
	public int Type{get;set;}
	public int X{get;set;}
	public int Y{get;set;}

    protected override void Init0(BinaryReader br){
		Id= ReadInt32(br);
		ScenarioId= ReadInt32(br);
		Type= ReadInt32(br);
		X= ReadInt32(br);
		Y= ReadInt32(br);

    }
}