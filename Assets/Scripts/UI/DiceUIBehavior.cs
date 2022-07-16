using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DiceUIBehavior : MonoBehaviour
{

    public TextMeshProUGUI up;
    public TextMeshProUGUI down;
    public TextMeshProUGUI left;
    public TextMeshProUGUI right;
    public TextMeshProUGUI forward;
    public TextMeshProUGUI back;

    internal void UpdateNum(List<int> nums)
    {
        up.text = nums[0].ToString();
        down.text = nums[1].ToString();
        left.text = nums[2].ToString();
        right.text = nums[3].ToString();
        forward.text = nums[4].ToString();
        back.text = nums[5].ToString();
    }

    internal void UpdateNumDict(Dictionary<string, int> faceNums)
    {
        Debug.Log("Updating text");
        up.text = faceNums["up"].ToString();
        down.text = faceNums["down"].ToString();
        left.text = faceNums["left"].ToString();
        right.text = faceNums["right"].ToString();
        forward.text = faceNums["forward"].ToString();
        back.text = faceNums["back"].ToString();
    }
}
