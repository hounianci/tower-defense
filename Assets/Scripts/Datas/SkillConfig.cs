public class SkillConfig : AbstractConfig{
    public override object GetKey(AbstractConfigEntry t)
    {
        SkillConfigEntry configEntry = (SkillConfigEntry) t;
        return configEntry.Id;
    }
    protected override string GetName()
    {
        return "Skill";
    }
    protected override AbstractConfigEntry CreateInstance()
    {
        return new SkillConfigEntry();
    }
}