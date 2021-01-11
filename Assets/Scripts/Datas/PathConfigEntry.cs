using System.IO;
using System;

public class PathConfigEntry : AbstractConfigEntry{
	public int Id{get;set;}
	public int PathId{get;set;}
	public int Index{get;set;}
	public int X{get;set;}
	public int Y{get;set;}

    protected override void Init0(BinaryReader br){
		Id= ReadInt32(br);
		PathId= ReadInt32(br);
		Index= ReadInt32(br);
		X= ReadInt32(br);
		Y= ReadInt32(br);

    }
}