using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    AbstractTrack tracker;
    public AbstractTrack Tracker{
        get=>tracker;
        set{tracker = value;}
    }
    int damage;
    public int Damage{
        get=>damage;
        set{damage=value;}
    }
    public void Init(int skillId){
        string skillInfo = FileUtil.readFile(string.Format("Assets/Skill/{0}_Skill.txt", skillId));
        string[] infoStr = skillInfo.Split(',');
        int rangeType = int.Parse(infoStr[0]);
        int damage = int.Parse(infoStr[1]);
        int rangeId = int.Parse(infoStr[2]);
        tracker = TargetFactory.CreateTracker(rangeType);
        tracker.Init(rangeId);
    }
}
