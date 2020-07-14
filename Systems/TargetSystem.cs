using System.Collections.Generic;

public class TargetSystem : ClassicECSSystem
{
    private TeamComponent[] _teamComponentArray;
    private TargetComponent[] _targetComponentArray;

    private CustomEntityManager _entityManager;
    private IECSRandom _randomSystem;
    private IECSLogger _logger;

    List<int> _potentialTargetList;


    public TargetSystem(IECSLogger logger, IECSRandom randomSystem)
    {
        _potentialTargetList = new List<int>();
        _logger = logger;
        _randomSystem = randomSystem;
    }



    public override void Initialize(CustomEntityManager customEntityManager)
    {
        _entityManager = customEntityManager;
        _teamComponentArray = _entityManager.GetComponentArray<TeamComponent>();
        _targetComponentArray = _entityManager.GetComponentArray<TargetComponent>();
            
    }

    public override void Update(float deltaTime, uint tick)
    {
        //Run through our system logic here
        //Note, we do this for every entity
        for(int i=0;i<_targetComponentArray.Length;i++)
        {
            //First, we need to make sure we're dealing with an active entity
            if(!_entityManager.IsEntityActive(i))
            {
                return;
            }

            if(!_targetComponentArray[i].IsActive)
            {
                //This entity does not have a target. Let's fix that

                //Lets assess potential targets
                //For now, this means looping through entities by team id and picking one that isn't our team

                _potentialTargetList.Clear();

                for (int t=0;t<_teamComponentArray.Length;t++)
                {
                    //Skip ourselves ;) && Skip any entity that isn't active
                    if(t != i && _entityManager.IsEntityActive(t) && _teamComponentArray[i] != _teamComponentArray[t])
                    {
                        _potentialTargetList.Add(t);
                    }
                }

                //Now we should ideally have a magical list of potential targets. Let's pick one from that list
                if(_potentialTargetList.Count > 0)
                {
                    int targetIndex = _randomSystem.GetNext(0, _potentialTargetList.Count - 1);

                    ref TargetComponent targetComponent = ref _entityManager.AddComponentToEntity<TargetComponent>(i);
                    targetComponent.TargetId = _potentialTargetList[targetIndex];
                    targetComponent.TargetAcquiredTick = tick;

                    _logger.Log(string.Format("Entity {0} found target Entity {1}. Acquired at tick {2}", i, _potentialTargetList[targetIndex], tick));
                }
            }
        }
    }
}