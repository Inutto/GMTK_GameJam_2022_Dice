using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Add This to a gridObject if it needs a dice attribtued health & combat value
/// </summary>

/// 
///             up
///             ^    forward
///             |   /
///             |  /                CAUTION:
/// left <---       ---> right      Forward is INTO the board!  (z+)
///           / |                   Back is OUT OF the board!   (z-)
///          /  |
///       back  v
///            down
///
public class DiceAttributeBehavior : MonoBehaviour
{
    Dictionary<string, int> faceNums = new Dictionary<string, int>();

    // for serialization purpose, Dictionary.ToString() may not be
    // predictable, but the order is correct for now. Use the Dict
    // for rigid purposes
    [SerializeField] List<int> nums = new List<int>();

    [SerializeField] DiceUIBehavior debugUI;

    private void Start()
    {
        Init();
        debugUI.UpdateNumDict(faceNums);
    }

    private void FixedUpdate()
    {
        nums = faceNums.Values.ToList();
    }

    public void Init()
    {

        faceNums.Add("up", 1);
        faceNums.Add("down", 6);
        faceNums.Add("left", 2);
        faceNums.Add("right", 5);
        faceNums.Add("forward", 3);
        faceNums.Add("back", 4);

    }

    public void UpdateNum(Vector3 axis)
    {
        // right
        if(axis == Vector3.down)
        {
            int temp = faceNums["back"];
            faceNums["back"] = faceNums["left"];
            faceNums["left"] = faceNums["forward"];
            faceNums["forward"] = faceNums["right"];
            faceNums["right"] = temp;
        }

        // left
        if(axis == Vector3.up)
        {
            int temp = faceNums["back"];
            faceNums["back"] = faceNums["right"];
            faceNums["right"] = faceNums["forward"];
            faceNums["forward"] = faceNums["left"];
            faceNums["left"] = temp;
        }

        // up
        if (axis == Vector3.right)
        {
            int temp = faceNums["back"];
            faceNums["back"] = faceNums["down"];
            faceNums["down"] = faceNums["forward"];
            faceNums["forward"] = faceNums["up"];
            faceNums["up"] = temp;
        }

        // down
        if (axis == Vector3.left)
        {
            int temp = faceNums["back"];
            faceNums["back"] = faceNums["up"];
            faceNums["up"] = faceNums["forward"];
            faceNums["forward"] = faceNums["down"];
            faceNums["down"] = temp;
        }

        debugUI.UpdateNumDict(faceNums);
        
    }




}
