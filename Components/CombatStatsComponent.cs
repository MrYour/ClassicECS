public struct CombatStatsComponent : ICustomComponentData
{
    public bool IsActive { get; set; }
    public int EntityId { get; set; }

    public float MovementSpeed;
    public float AttackPower;
    public float Defense;
    public float DodgeChance;
    public float CritChance;
    public float AttackRange;
}
