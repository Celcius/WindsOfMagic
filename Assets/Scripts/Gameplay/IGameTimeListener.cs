using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameTimeListener
{
    void OnTimeElapsed(float timeElapsed);
}
