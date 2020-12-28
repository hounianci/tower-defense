using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    AbstractTrack tracker;
    GameActor owner;
    int skillId;
    public int SkillId{
        get=>skillId;
        set{skillId=value;}
    }
    public AbstractTrack Tracker{
        get=>tracker;
        set{tracker = value;}
    }
    public GameActor Owner{
        get=>owner;
        set{owner=value;}
    }
    int damage;
    public int Damage{
        get=>damage;
        set{damage=value;}
    }
    public void Init(int skillId, GameActor owner){
        this.skillId=skillId;
        string skillInfo = FileUtil.readFile(string.Format("Assets/Skill/{0}_Skill.txt", skillId));
        string[] infoStr = skillInfo.Split(',');
        int rangeType = int.Parse(infoStr[0]);
        int damage = int.Parse(infoStr[1]);
        int rangeId = int.Parse(infoStr[2]);
        this.owner = owner;
        tracker = TargetFactory.CreateTracker(rangeType);
        tracker.Init(rangeId, owner);
    }
}
