using System.IO;
public class TowerConfigEntry : AbstractConfigEntry{
	public int Id{get;set;}
	public int[] SkillList{get;set;}

    protected override void Init0(BinaryReader br){
		Id= ReadInt32(br);
		SkillList= ReadArray(br);

    }
}