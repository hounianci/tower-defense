using System.IO;
using System;

public class SpawnWaveConfigEntry : AbstractConfigEntry{
	public int Id{get;set;}
	public int MapId{get;set;}
	public int SpawnId{get;set;}
	public float SpawnTime{get;set;}
	public int EnemyId{get;set;}
	public int PathId{get;set;}

    protected override void Init0(BinaryReader br){
		Id= ReadInt32(br);
		MapId= ReadInt32(br);
		SpawnId= ReadInt32(br);
		SpawnTime = ReadFloat(br);
		EnemyId= ReadInt32(br);
		PathId= ReadInt32(br);

    }
}