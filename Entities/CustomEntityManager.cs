using System.Collections.Generic;

public class CustomEntityManager
{
    private int _entityBufferSize;
    private int _activeEntityCounter;

    private bool _shouldExpandBuffer = false;


    private bool[] _entityBuffer;

    private Dictionary<System.Type, object> _componentCollection;

    public CustomEntityManager(int entityBufferSize, bool shouldExpandBuffer)
    {
        _entityBufferSize = entityBufferSize;
        _shouldExpandBuffer = shouldExpandBuffer;        
    }

    /// <summary>
    /// 
    /// </summary>
    public void Initialize()
    {
        if(_entityBuffer != null)
        {
            _entityBuffer = null;
        }
        _entityBuffer = new bool[_entityBufferSize];

        for(int i=0;i< _entityBufferSize;i++)
        {
            _entityBuffer[i] = false;
        }

        _activeEntityCounter = 0;


        _componentCollection = new Dictionary<System.Type, object>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetEntityBufferSize()
    {
        return _entityBufferSize;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetNumberOfActiveEntities()
    {
        return _activeEntityCounter;
    }


    /// <summary>
    /// Returns an integer representing the ID of the newly assigned Entity.
    /// Note, this ID can be 0.
    /// </summary>
    /// <returns>An integer representing the id of the assigned Entity OR throws an exception due to out of space</returns>
    public int CreateNewEntity()
    {
        for(int i=0;i<_entityBuffer.Length;i++)
        {
            if(!_entityBuffer[i])
            {
                _entityBuffer[i] = true;
                _activeEntityCounter++;
                return i;
            }
        }

        //If we get here, it means that none of the slots in the Entity buffer are clear. We should check to see if
        //the entity manager is configured to expand the buffer or fail.

        //TODO: Implement expand functionality        
        throw new OutOfEntitiesException(string.Format("Out of space for new Entities. Reached cap of {0}", _entityBufferSize));        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entityId"></param>
    public void RemoveEntity(int entityId)
    {
        if (IsValidEntityId(entityId))
        {
            foreach(KeyValuePair<System.Type, object> componentEntry in _componentCollection)
            {
                ICustomComponentData[] componentArray = componentEntry.Value as ICustomComponentData[];
                componentArray[entityId].IsActive = false;
            }
            _activeEntityCounter--;
        }
        else
        {
            throw new InvalidEntityIDException(string.Format("Invalid Entity Id {0} provided while trying to remove entity", entityId));         
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public ref T AddComponentToEntity<T>(int entityId) where T : ICustomComponentData
    {
        if (_componentCollection.ContainsKey(typeof(T)))
        {
            if(IsValidEntityId(entityId))
            {
                T[] componentArray = _componentCollection[typeof(T)] as T[];
                componentArray[entityId].IsActive = true;
                return ref componentArray[entityId];
            }
            else
            {
                throw new InvalidEntityIDException(string.Format("Invalid Entity Id {0} provided while trying to add component to entity", entityId));
            }            
        }
        else
        {
            throw new ComponentNotRegisteredException(string.Format("Component Type {0} Not Registered Exception encountered while trying to add Component to entity id {1}", typeof(T), entityId));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public ref T GetComponentForEntity<T>(int entityId) where T : ICustomComponentData
    {
        if (_componentCollection.ContainsKey(typeof(T)))
        {
            if (IsValidEntityId(entityId))
            {                
                T[] componentArray = (T[])_componentCollection[typeof(T)];
                if (componentArray[entityId].IsActive)
                {                    
                    return ref componentArray[entityId];
                }
                else
                {
                    throw new ComponentTypeNotFoundOfEntityException(string.Format("No Component of Type {0} for Entity {1}", typeof(T), entityId));                    
                }                
            }
            else
            {
                throw new InvalidEntityIDException(string.Format("Invalid Entity Id {0} provided while trying to get component type {1} on entity", entityId, typeof(T)));
            }
        }
        else
        {
            throw new ComponentNotRegisteredException(string.Format("Component Type {0} Not Registered", typeof(T)));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityId"></param>
    public void RemoveComponentFromEntity<T>(int entityId) where T : ICustomComponentData
    {
        if (_componentCollection.ContainsKey(typeof(T)))
        {
            if (IsValidEntityId(entityId))
            {
                T[] componentArray = _componentCollection[typeof(T)] as T[];
                componentArray[entityId].IsActive = false;                
            }
            else
            {
                throw new InvalidEntityIDException(string.Format("Invalid Entity Id {0} provided while trying to remove component type {1} from entity", entityId, typeof(T)));
            }
        }
        else
        {
            throw new ComponentNotRegisteredException(string.Format("Component Type {0} Not Registered", typeof(T)));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RegisterComponentType<T>() where T : ICustomComponentData, new()
    {
        if(!_componentCollection.ContainsKey(typeof(T)))
        {            
            T[] componentArray = new T[_entityBufferSize];
            for (int i=0;i<_entityBufferSize;i++)
            {
                T newComponent = new T();
                newComponent.EntityId = i;
                newComponent.IsActive = false;
                componentArray[i] = newComponent;
            }

            _componentCollection.Add(typeof(T), componentArray);
        }
        else
        {
            throw new ComponentAlreadyRegisteredException(string.Format("Component Type {0} Already Registered", typeof(T)));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void UnregisterComponentType<T>() where T : ICustomComponentData
    {
        if (_componentCollection.ContainsKey(typeof(T)))
        {
            _componentCollection[typeof(T)] = null;
            _componentCollection.Remove(typeof(T));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T[] GetComponentArray<T>() where T : ICustomComponentData
    {
        if (_componentCollection.ContainsKey(typeof(T)))
        {
            return _componentCollection[typeof(T)] as T[];
        }
        else
        {
            throw new ComponentNotRegisteredException(string.Format("Component Type {0} Not Registered", typeof(T)));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entityId"></param>
    /// <returns></returns>
    private bool IsValidEntityId(int entityId)
    {
        if(entityId < 0 || entityId > _entityBufferSize - 1)
        {
            return false;
        }
        return true;
    }

    public bool IsEntityActive(int entityId)
    {
        if(IsValidEntityId(entityId))
        {
            return _entityBuffer[entityId];
        }
        return false;
    }
}