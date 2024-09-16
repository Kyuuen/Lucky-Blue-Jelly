using System.Collections.Generic;

public class MergeBubbleEvent
{
    public int mergeStatus; //status for seperate cases
    //status = 1 => 2 single color
    //status = 2 => 1 of 2 is single
    //status = 3 => none is single
    public int mergeId;
    public int breakId;
    public int moveAmount;
    public bool moveBack;
    public int colorType;
    public bool isFull;
    public List<int> colorTypes;
}
