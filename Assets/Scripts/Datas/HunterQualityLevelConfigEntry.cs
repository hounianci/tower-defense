using System.IO;
using System;

public class HunterQualityLevelConfigEntry : AbstractConfigEntry{
	public int Id{get;set;}
	public int Quality{get;set;}
	public int TrainConsumeRoleCoin{get;set;}
	public float ResolveRolesAddRoleCoin{get;set;}
	public string TrainConsumeRoleItem{get;set;}
	public int[] Asd{get;set;}
	public string Name{get;set;}
	public string Pwd{get;set;}
	public int Age{get;set;}

    protected override void Init0(BinaryReader br){
		Id= ReadInt32(br);
		Quality= ReadInt32(br);
		TrainConsumeRoleCoin= ReadInt32(br);
		ResolveRolesAddRoleCoin = ReadFloat(br);
		TrainConsumeRoleItem = ReadString(br);
		Asd= ReadArray(br);
		Name = ReadString(br);
		Pwd = ReadString(br);
		Age= ReadInt32(br);

    }
}