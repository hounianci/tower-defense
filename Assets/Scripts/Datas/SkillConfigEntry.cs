using System.IO;
public class SkillConfigEntry : AbstractConfigEntry{
	public int Id{get;set;}
	public int RangeType{get;set;}
	public int Damage{get;set;}
	public int RangeId{get;set;}

    protected override void Init0(BinaryReader br){
		Id= ReadInt32(br);
		RangeType= ReadInt32(br);
		Damage= ReadInt32(br);
		RangeId= ReadInt32(br);

    }
}