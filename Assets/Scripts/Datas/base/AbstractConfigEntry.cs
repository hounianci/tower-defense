using System.IO;
using System;

public abstract class AbstractConfigEntry{

    public void Init(BinaryReader br){
        Init0(br);
    }

    protected abstract void Init0(BinaryReader br);

    protected int ReadInt32(BinaryReader br){
		byte[] tmpArray = br.ReadBytes(4);
		Array.Reverse(tmpArray);
        return BitConverter.ToInt32(tmpArray, 0);
    }
    protected float ReadFloat(BinaryReader br){
		byte[] tmpArray = br.ReadBytes(4);
		Array.Reverse(tmpArray);
        return BitConverter.ToSingle(tmpArray, 0);
    }

    protected string ReadString(BinaryReader br){
		int len = br.ReadByte();
		byte[] tmpArray = br.ReadBytes(len);
		return System.Text.Encoding.Default.GetString (tmpArray);
    }
    protected int[] ReadArray(BinaryReader br){
		int len = br.ReadByte();
		int[] tmpArr= new int[len];
		for(int i=0; i<len; i++){
			int data = br.ReadInt32();
			byte[] dataByte = BitConverter.GetBytes(data);
			Array.Reverse(dataByte);
			tmpArr[i] = BitConverter.ToInt32(dataByte, 0);
		}
        return tmpArr;
    }
    protected bool ReadBool(BinaryReader br){
        return br.ReadBoolean();
    }
}