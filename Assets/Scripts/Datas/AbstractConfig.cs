using System.Collections.Generic;
using System.IO;
using System;
public abstract class AbstractConfig<T> where T:AbstractConfigEntry{
    public List<T> datas{
        get;set;
    }

    public void Init(){      
        FileStream fs = new FileStream("Assets/bin/"+GetName()+".bin",FileMode.Open);
        BinaryReader br = new BinaryReader(fs);
        int size = br.ReadInt32();
        byte[] bytes = BitConverter.GetBytes(size);
        Array.Reverse(bytes);
        size = BitConverter.ToInt32(bytes, 0);
        datas = new List<T>();
        for(int i=0; i<size; i++){
            T t = CreateInstance();
            t.Init(br);
            datas.Add(t);
        }
        br.Close();
        fs.Close();
    }

    protected abstract string GetName();
    protected abstract T CreateInstance();

}