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
        SkillConfigEntry skillConfigEntry = DataManager.GetData<SkillConfigEntry>(typeof(SkillConfig), skillId);
        int rangeType = skillConfigEntry.RangeType;
        int damage = skillConfigEntry.Damage;
        int rangeId = skillConfigEntry.RangeId;
        this.owner = owner;
        tracker = TargetFactory.CreateTracker(rangeType);
        tracker.Init(rangeId, owner, 1);
        this.damage = damage;
    }
}
