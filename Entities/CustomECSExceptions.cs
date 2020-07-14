using System;

public class EntityNotActiveException : Exception
{
    public EntityNotActiveException(string message) : base(message)
    {
        
    }    
}

public class InvalidEntityIDException : Exception
{
    public InvalidEntityIDException(string message) : base(message)
    {

    }
}

public class OutOfEntitiesException : Exception
{
    public OutOfEntitiesException(string message) : base(message)
    {

    }
}

public class ComponentNotRegisteredException : Exception
{
    public ComponentNotRegisteredException(string message) : base(message)
    {

    }
}

public class ComponentAlreadyRegisteredException : Exception
{
    public ComponentAlreadyRegisteredException(string message) : base(message)
    {

    }
}

public class ComponentTypeNotFoundOfEntityException : Exception
{
    public ComponentTypeNotFoundOfEntityException(string message) : base(message)
    {

    }
}