using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Add This to a gridObject if it needs a dice attribtued health & combat value
/// </summary>
public class DiceAttributeBehavior : MonoBehaviour
{

    enum DiceDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        FORWARD, 
        BACKWARD,
    }

    string[] DiceDirName = new string[6] { "^", "down", "<--", "-->", "Forward", "Backward" };

    // Dice Facing -> World Direction
    List<Vector3> diceFace2worldList = new List<Vector3>(new Vector3[6]);
    //{
    //    new vector3(0f, 1f, 0f),
    //    new vector3(0f, -1f, 0f),
    //    new vector3(-1f, 0f, 0f),
    //    new vector3(1f, 0f, 0f),
    //    new vector3(0f, 0f, 1f),
    //    new vector3(0f, 0f, -1f) ,
    //};


    // World Direction -> Transform Direction
    List<Vector3> world2transList = new List<Vector3>(new Vector3[6]);
 
    // Num record
    [SerializeField] List<int> nums = new List<int>(); // follow the order UP DOWN LEFT RIGHT FORWARD BACKWARD


    private void Start()
    {
        InitWorldFacing();
        UpdateWorld2Trans();
    }
    

    public void InitWorldFacing()
    {

        diceFace2worldList[0] = transform.up;
        diceFace2worldList[1] = transform.up * -1f;
        diceFace2worldList[2] = transform.right * -1f;
        diceFace2worldList[3] = transform.right;
        diceFace2worldList[4] = transform.forward;
        diceFace2worldList[5] = transform.forward * -1f;

        // TEST: only for UI Debug
        DiceUIBehavior UIBehavior;
        if (TryGetComponent<DiceUIBehavior>(out UIBehavior))
        {
            UIBehavior.UpdateNum(nums);
        }
    }



    public void UpdateWorld2Trans()
    {

        for (int i = 0; i < world2transList.Count; ++i)
        {
            world2transList[i] = Vector3.zero;
        }


        world2transList[0] = transform.up; // TransformDirection.UP
        world2transList[1] = transform.up * -1f;
        world2transList[2] = transform.right * -1f;
        world2transList[3] = transform.right;
        world2transList[4] = transform.forward;
        world2transList[5] = transform.forward * -1f;

        foreach(var vec in world2transList)
        {
            DebugF.Log("Current ->" + vec);
        }

    }


    public void UpdateNum()
    {
        UpdateWorld2Trans();

        int[] tempNums = new int[6];

        for(int i = 0; i < nums.Count; ++i)
        {
            var targetDir = diceFace2worldList[i]; // 0,1,0 (UP)

            DebugF.Log(targetDir);

            int targetIndex = -1;
            foreach(var vec in world2transList)
            {
                if(targetDir == vec) targetIndex = world2transList.IndexOf(vec); // transform.forward
            }
            DebugF.Log(targetIndex); // 4
            if (targetIndex == -1)
            {
                DebugF.LogWarning("No Matching Index for vec: " + targetDir);
                return;
            }

            tempNums[i] = nums[targetIndex]; // temp[0] = nums[4], NOW UP = PAST FORWARD
            DebugF.Log("Now " + DiceDirName[i] + " = Past " + DiceDirName[targetIndex]);

        }

        for(int i = 0; i < nums.Count; ++i)
        {
            nums[i] = tempNums[i];
        }


        // TEST: only for UI Debug
        DiceUIBehavior UIBehavior;
        if (TryGetComponent<DiceUIBehavior>(out UIBehavior))
        {
            UIBehavior.UpdateNum(nums);
        }


    }




}
