using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DiceUIBehavior : MonoBehaviour
{

    public TMP_Text up;
    public TMP_Text down;
    public TMP_Text left;
    public TMP_Text right;
    public TMP_Text forward;
    public TMP_Text backward;

    internal void UpdateNum(List<int> nums)
    {
        up.text = nums[0].ToString();
        down.text = nums[1].ToString();
        left.text = nums[2].ToString();
        right.text = nums[3].ToString();
        forward.text = nums[4].ToString();
        backward.text = nums[5].ToString();
    }
}
