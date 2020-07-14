using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamComponent : ICustomComponentData
{
    public bool IsActive { get; set; }
    public int EntityId { get; set; }
    public short TeamID;
}
