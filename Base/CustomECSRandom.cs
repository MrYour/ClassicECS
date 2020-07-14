using System;

public class CustomECSRandom : IECSRandom
{
    private Random _randomizer;

    public CustomECSRandom(int seed)
    {
        _randomizer = new Random(seed);

    }

    public int GetNext(int minVal, int maxVal)
    {
        return _randomizer.Next(minVal, maxVal);
    }
}
