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

    enum TransformDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        FORWARD, 
        BACKWARD,
    }

    // Dice Facing -> World Direction
    Dictionary<DiceDirection, Vector3> diceFace2worldDic = new Dictionary<DiceDirection, Vector3>() 
    {
        { DiceDirection.UP, new Vector3(0f, 1f, 0f) },
        { DiceDirection.DOWN, new Vector3(0f, -1f, 0f)},
        { DiceDirection.LEFT, new Vector3(-1f, 0f, 0f) },
        { DiceDirection.RIGHT, new Vector3(1f, 0f, 0f)},
        { DiceDirection.FORWARD, new Vector3(0f, 0f, 1f)},
        { DiceDirection.BACKWARD, new Vector3(0f, 0f, -1f) },
    };


    // World Direction -> Transform Direction
    Dictionary<Vector3, TransformDirection> world2transDic = new Dictionary<Vector3, TransformDirection>()
    {
        { new Vector3(0f, 1f, 0f), TransformDirection.UP },
        { new Vector3(0f, -1f, 0f), TransformDirection.DOWN},
        { new Vector3(-1f, 0f, 0f), TransformDirection.LEFT },
        { new Vector3(1f, 0f, 0f), TransformDirection.RIGHT},
        { new Vector3(0f, 0f, 1f), TransformDirection.FORWARD},
        { new Vector3(0f, 0f, -1f), TransformDirection.BACKWARD },
    };

    // Transform Dir -> Num
    Dictionary<TransformDirection, int> trans2numDic = new Dictionary<TransformDirection, int>();


    // Num record
    [SerializeField] int up;
    [SerializeField] int down;
    [SerializeField] int left;
    [SerializeField] int right;
    [SerializeField] int forward;
    [SerializeField] int backward;

    private void Start()
    {
        InitWorldFacing();
        UpdateWorld2Trans();
    }
    // By Default
    public void InitWorldFacing()
    {
        trans2numDic[TransformDirection.UP] = up;
        trans2numDic[TransformDirection.DOWN] = down;
        trans2numDic[TransformDirection.RIGHT] = right;
        trans2numDic[TransformDirection.LEFT] = left;
        trans2numDic[TransformDirection.FORWARD] = forward;
        trans2numDic[TransformDirection.BACKWARD] = backward;

        diceFace2worldDic[DiceDirection.UP] = transform.up;
        diceFace2worldDic[DiceDirection.DOWN] = transform.up * -1f;
        diceFace2worldDic[DiceDirection.LEFT] = transform.right * -1f;
        diceFace2worldDic[DiceDirection.RIGHT] = transform.right;
        diceFace2worldDic[DiceDirection.FORWARD] = transform.forward;
        diceFace2worldDic[DiceDirection.BACKWARD] = transform.forward * -1f;
    }

    public void UpdateNum()
    {
        UpdateWorld2Trans();

        var res1 = diceFace2worldDic[DiceDirection.UP];
        DebugF.Log(res1);
        var res2 = world2transDic[res1];
        DebugF.Log(res2);
        var res3 = trans2numDic[res2];
        DebugF.Log(res3);






        //down = trans2numDic[world2transDic[diceFace2worldDic[DiceDirection.DOWN]]];
        //left = trans2numDic[world2transDic[diceFace2worldDic[DiceDirection.LEFT]]];
        //right = trans2numDic[world2transDic[diceFace2worldDic[DiceDirection.RIGHT]]];
        //forward = trans2numDic[world2transDic[diceFace2worldDic[DiceDirection.FORWARD]]];
        //backward = trans2numDic[world2transDic[diceFace2worldDic[DiceDirection.BACKWARD]]];

    }

    public void UpdateWorld2Trans()
    {
        world2transDic.Clear();

        world2transDic[transform.up] = TransformDirection.UP;
        world2transDic[transform.up * -1f] = TransformDirection.DOWN;
        world2transDic[transform.right * -1f] = TransformDirection.LEFT;
        world2transDic[transform.right] = TransformDirection.RIGHT;
        world2transDic[transform.forward] = TransformDirection.FORWARD;
        world2transDic[transform.forward * -1f] = TransformDirection.BACKWARD;

        
    }



    
}
