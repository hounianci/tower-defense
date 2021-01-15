using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConfigWrapper : PathConfig
{
	public List<PathConfigEntry> GetByPathId(int pathId){
		List<PathConfigEntry> result = new List<PathConfigEntry>();
        foreach(PathConfigEntry entry in Datas.Values){
			if(entry.PathId==pathId){
            	result.Add(entry);
			}
        }
		return result;
	}
}
