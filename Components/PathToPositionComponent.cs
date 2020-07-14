public struct PathToPositionComponent : ICustomComponentData
{
    public bool IsActive { get; set; }
    public int EntityId { get; set; }
        
    public Vector3D[] Path;
    public int CurrentPathSegmentIndex;
}
