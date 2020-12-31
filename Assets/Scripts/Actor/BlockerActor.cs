using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BlockerActor
{
    bool IsBlockByMe(GameActor actor);
}
