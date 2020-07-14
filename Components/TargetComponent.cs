public struct TargetComponent : ICustomComponentData
{
    public bool IsActive { get; set; }
    public int EntityId { get; set; }

    public int TargetId;    
    public uint TargetAcquiredTick;
    public Vector3D TargetPosition;
}
