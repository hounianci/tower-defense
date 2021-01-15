using System.IO;
public class EnemyConfigEntry : AbstractConfigEntry{
	public int Id{get;set;}
	public int Hp{get;set;}
	public int Speed{get;set;}
	public float Scale{get;set;}
	public string Model{get;set;}
	public int Interval{get;set;}

    protected override void Init0(BinaryReader br){
		Id= ReadInt32(br);
		Hp= ReadInt32(br);
		Speed= ReadInt32(br);
		Scale = ReadFloat(br);
		Model = ReadString(br);
		Interval= ReadInt32(br);

    }
}