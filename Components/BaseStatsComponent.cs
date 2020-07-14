public struct BaseStatsComponent : ICustomComponentData
{
    public bool IsActive { get; set; }
    public int EntityId { get; set; }

    public float Strength;
    public float Dexterity;
    public float Stamina;
    public float Intelligence;
    public float Wisdom;
    public float Perception;
}
