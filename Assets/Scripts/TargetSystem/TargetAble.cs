using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TargetAble
{    
    Vector2Int GetTilePosition();
    Vector3 GetPosition();
    int TeamId();
    int ApplyDamage(float damage);
}
