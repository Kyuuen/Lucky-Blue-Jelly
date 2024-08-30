using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BubbleInfo
{
    public int ID;
    public int Size;
    public int MaxNumb;
    public List<int> jellyColor = new List<int>();

    enum MaxNumbs
    {
        Size_0 = 3,
        Size_1 = 5,
        Size_2 = 7,
        Size_3 = 10
    }

    public BubbleInfo() 
    {

    }

    public void GetNewBubbleInfo() 
    {
        Size = UnityEngine.Random.Range(0, 4);

        MaxNumbs[] maxNumbs = (MaxNumbs[])Enum.GetValues(typeof(MaxNumbs));
        MaxNumb = (int)maxNumbs[Size];
    }

    public void SetNewBubbleInfo(int size)
    {
        Size = size;

        MaxNumbs[] maxNumbs = (MaxNumbs[])Enum.GetValues(typeof(MaxNumbs));
        MaxNumb = (int)maxNumbs[Size];
    }
}
