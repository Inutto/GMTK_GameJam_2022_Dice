using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomGrid;

public class DamageAreaBehavior : MonoBehaviour
{

    [SerializeField] DamageAreaList[] damageAreaCollection = new DamageAreaList[6];
    

    public List<Vector2Int> GetDamageArea(int areaType, Vector2Int direction)
    {
        // Get reference list
        var initialAreaList = damageAreaCollection[areaType];
        if(initialAreaList == null)
        {
            DebugF.LogWarning("Damage Area not working with areaType: " + areaType);
            return null;
        }


        return initialAreaList.Rotate(direction);

    }

}
