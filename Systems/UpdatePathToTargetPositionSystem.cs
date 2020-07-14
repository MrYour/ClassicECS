
/// <summary>
/// This system is responsible for detecting that an entities target has moved, and updates that
/// entities path to match the new position of the target
/// </summary>
public class UpdatePathToTargetPositionSystem : ClassicECSSystem
{
    private CustomEntityManager _entityManager;
    private TargetComponent[] _targetComponentArray;
    private PositionComponent[] _positionComponentArray;
    private CombatStatsComponent[] _combatStatsComponents;
    private PathToPositionComponent[] _pathToTargetComponent;
    private IPathfindingManager _pathfindingManager;
    private IECSLogger _logger;

    public UpdatePathToTargetPositionSystem(IPathfindingManager pathfindingManager, IECSLogger logger)
    {
        _pathfindingManager = pathfindingManager;
        _logger = logger;
    }

    public override void Initialize(CustomEntityManager customEntityManager)
    {
        _entityManager = customEntityManager;
        _targetComponentArray = _entityManager.GetComponentArray<TargetComponent>();
        _positionComponentArray = _entityManager.GetComponentArray<PositionComponent>();
        _pathToTargetComponent = _entityManager.GetComponentArray<PathToPositionComponent>();
        _combatStatsComponents = _entityManager.GetComponentArray<CombatStatsComponent>();
    }


    public override void Update(float deltaTime, uint tick)
    {
        //Loop through all entities that have a target component
        for (int i = 0; i < _targetComponentArray.Length; i++)
        {
            //First, we need to make sure we're dealing with an active entity
            if (!_entityManager.IsEntityActive(i))
            {
                return;
            }

            //Now we need to make sure this current entity "i" has a target. If not, then this system isn't interested
            if (_targetComponentArray[i].IsActive)
            {
                //Make sure the target has a position component
                if(_positionComponentArray[_targetComponentArray[i].TargetId].IsActive)
                {

                    //This entity has a target, now we need to see if we're outside of attack range
                    float dist = _positionComponentArray[i].Position.DistanceTo(_positionComponentArray[_targetComponentArray[i].TargetId].Position);
                    if (dist > _combatStatsComponents[i].AttackRange)
                    {
                        //Well, our worst fears have come true, the target is out of range... Guess we need to get on the dusty trail

                        //First we need to check to see if we were previously pathing, and if so, should we be pathing to the same point
                        if(!_targetComponentArray[i].TargetPosition.SameAs(_positionComponentArray[_targetComponentArray[i].TargetId].Position) || _pathToTargetComponent[i].Path == null)
                        {
                            //Well, they're not in the same position, so we should probably update that
                            //TODO, we should be pathing to somewhere "near" the target based on some range information about this entity. This could easily be offloaded to a pathfinding manager.
                            
                            _pathToTargetComponent[i].Path = _pathfindingManager.GetPath(_positionComponentArray[i], _positionComponentArray[_targetComponentArray[i].TargetId]);
                            _pathToTargetComponent[i].CurrentPathSegmentIndex = 0;

                            //Since we're setting a path to where the target currently is, we need to rememer where the target currently is
                            _targetComponentArray[i].TargetPosition.Set(_positionComponentArray[_targetComponentArray[i].TargetId].Position);

                            _logger.Log(string.Format("Updated path for Entity {0} to Target {1} X:{2} Y:{3} Z:{4}", i, _targetComponentArray[i].TargetId,
                            _positionComponentArray[_targetComponentArray[i].TargetId].Position.X,
                            _positionComponentArray[_targetComponentArray[i].TargetId].Position.Y,
                            _positionComponentArray[_targetComponentArray[i].TargetId].Position.Z));

                            _pathToTargetComponent[i].IsActive = true;
                        }                        
                    }                    
                }
            }
        }
    }
}
