using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This system is responsible for updating a position component to follow a path
/// </summary>
public class MoveAlongPathToAttackRangeSystem : ClassicECSSystem
{
    private CustomEntityManager _entityManager;
    private PathToPositionComponent[] _pathToPositionComponentArray;
    private PositionComponent[] _positionComponentArray;
    private CombatStatsComponent[] _combatStatsComponents;
       

    IECSLogger _logger;

    public MoveAlongPathToAttackRangeSystem(IECSLogger logger)
    {
        _logger = logger;   
    }

    public override void Initialize(CustomEntityManager customEntityManager)
    {
        _entityManager = customEntityManager;
        _pathToPositionComponentArray = _entityManager.GetComponentArray<PathToPositionComponent>();
        _positionComponentArray = _entityManager.GetComponentArray<PositionComponent>();
        _combatStatsComponents = _entityManager.GetComponentArray<CombatStatsComponent>();
    }

    public override void Update(float deltaTime, uint tick)
    {
        for (int i = 0; i < _pathToPositionComponentArray.Length; i++)
        {
            //C'mon. If the dude isn't active you shouldn't do a damn thing. Get out of herer
            if (!_entityManager.IsEntityActive(i))
            {
                return;
            }

            //Now that we know this is an active Entity, does it even have a path?
            if(_pathToPositionComponentArray[i].IsActive)
            {
                //There's a path. Now, what to do?
                //This function should use the entities move speed to move them along the path.
                //It should move along the path segment by segment.


                //Move the motherf*@!cker

                //Grab a short hand name for this array element, other wise the following code would be waaaaay longer
                PathToPositionComponent ptpc = _pathToPositionComponentArray[i];

                //Get this doods move speed
                float moveSpeed = _combatStatsComponents[i].MovementSpeed;

                //Grab ye olde vector from this entity to the target position
                Vector3D moveDeltaVector = ptpc.Path[ptpc.CurrentPathSegmentIndex] - _positionComponentArray[i].Position;

                //Normalize the vector, then scale it according to the move speed
                moveDeltaVector.Normalize();

                //This is cool. This normalized vector is our direction, so just go ahead and record that
                _positionComponentArray[i].Direction = moveDeltaVector;

                moveDeltaVector.Scale(moveSpeed * deltaTime);

                //Now add the delta vector to our position
                _positionComponentArray[i].Position.Add(moveDeltaVector);


                _logger.Log(string.Format("Entity {0} moved to {1}, {2}, {3}", i, _positionComponentArray[i].Position.X, _positionComponentArray[i].Position.Y, _positionComponentArray[i].Position.Z));


                //We should check to see if we're at the end point of a path segment OR we've moved beyond our target point

                //First, get the distance                
                float dist = _positionComponentArray[i].Position.DistanceTo(ptpc.Path[ptpc.CurrentPathSegmentIndex]);

                //Now get the dot product to know if we're beyond our target point
                Vector3D postMoveVector = ptpc.Path[ptpc.CurrentPathSegmentIndex] - _positionComponentArray[i].Position;

                float dotProd = Vector3D.DotProduct(postMoveVector, _positionComponentArray[i].Direction);


                //Now, we can compare this squared distance to our squared epsilon. This saves us from having to perform a square root calculation
                if (dist <= _combatStatsComponents[i].AttackRange || dotProd <= 0.0f)
                {
                    //We can now consider ourselves "arrived" at this point
                    //Set the current position to this point, and then up the current path index. We also need to check to see if this was the last path segment
                    //and if so, consider our travel along this path as complete.

                    _positionComponentArray[i].Position.Set(ptpc.Path[ptpc.CurrentPathSegmentIndex]);

                    if (ptpc.CurrentPathSegmentIndex == ptpc.Path.Length - 1)
                    {
                        //Oh boy, we're at the end. What a treat.
                        //Lets remove the path component, because that's within our power
                        _pathToPositionComponentArray[i].Path = null;
                        _pathToPositionComponentArray[i].IsActive = false;

                        _logger.Log(string.Format("Entity {0} arrived at end of path!", i));
                    }
                    else
                    {
                        //Not the end mf. Press on.
                        //Update our index to the next segment... and that's cool.
                        ptpc.CurrentPathSegmentIndex++;
                    }
                }
            }
        }
    }
}
