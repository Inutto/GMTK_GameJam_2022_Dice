using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Damage Area", menuName = "GMTK/New Damage Area")]
public class DamageAreaList : ScriptableObject
{

    
    /// <summary>
    /// the default area collections of points. default direction = Vector2Int.up
    /// </summary>
    [SerializeField] List<Vector2Int> areaPoints;


    /// <summary>
    /// return a rotated areaPoints. Notice that the origina areaPoints can't be changed via script
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public List<Vector2Int> Rotate(Vector2Int direction)
    {
        var rotatedAreaPoints = new List<Vector2Int>();
        foreach(var areaPoint in areaPoints)
        {
            var newPoint = RotateVector2Int(areaPoint, direction);
            rotatedAreaPoints.Add(newPoint);
        }
        return rotatedAreaPoints;
    }


    #region util


    /// <summary>
    /// rotate a point based on the input delta directino. Output is also a point.
    /// This technically only works for points of damage area
    /// </summary>
    /// <param name="areaPoint"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    Vector2Int RotateVector2Int(Vector2Int areaPoint, Vector2Int direction)
    {
        if (direction == Vector2Int.up) // Default Orientation
        {
            return areaPoint;
        }
        else if (direction == Vector2Int.left)
        {
            return new Vector2Int(-areaPoint.y, areaPoint.x);
        }
        else if (direction == Vector2Int.down)
        {
            return new Vector2Int(-areaPoint.x, -areaPoint.y);
        }
        else if (direction == Vector2Int.right)
        {
            return new Vector2Int(areaPoint.y, -areaPoint.x);
        }
        else
        {
            DebugF.LogWarning("Incorrect direction input: " + direction.ToString());
            return Vector2Int.up; // without direction for default
        }
    }

    #endregion util
}
