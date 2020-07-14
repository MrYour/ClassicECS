public struct PositionComponent : ICustomComponentData
{
    public bool IsActive { get; set; }
    public int EntityId { get; set; }
    public Vector3D Position;
    public Vector3D Direction;
}
