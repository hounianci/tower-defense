using System.Collections.Generic;
using System.IO;
using System;
public abstract class AbstractConfig{

    public Dictionary<object, AbstractConfigEntry> Datas{
        get;set;
    }

    public abstract object GetKey(AbstractConfigEntry t);

    public void Init(){      
        FileStream fs = new FileStream("Assets/bin/"+GetName()+".bin",FileMode.Open);
        BinaryReader br = new BinaryReader(fs);
        int size = br.ReadInt32();
        byte[] bytes = BitConverter.GetBytes(size);
        Array.Reverse(bytes);
        size = BitConverter.ToInt32(bytes, 0);
        Datas = new Dictionary<object, AbstractConfigEntry>();
        for(int i=0; i<size; i++){
            AbstractConfigEntry t = CreateInstance();
            t.Init(br);
            Datas.Add(GetKey(t), t);
        }
        br.Close();
        fs.Close();
    }

    protected abstract string GetName();
    protected abstract AbstractConfigEntry CreateInstance();
}