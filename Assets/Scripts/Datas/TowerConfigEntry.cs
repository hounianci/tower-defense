using System.IO;
public class TowerConfigEntry : AbstractConfigEntry{
	public int Id{get;set;}
	public int[] SkillList{get;set;}
	public int Interval{get;set;}
	public int Hp{get;set;}

    protected override void Init0(BinaryReader br){
		Id= ReadInt32(br);
		SkillList= ReadArray(br);
		Interval= ReadInt32(br);
		Hp= ReadInt32(br);

    }
}