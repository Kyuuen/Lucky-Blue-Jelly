using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;
using System.Drawing;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

public interface IMergeBubbleSystem : ISystem
{
    public void GetBubbleContact(int id, int otherId);
}
public class MergeBubbleSystem : AbstractSystem, IMergeBubbleSystem
{
    private BubbleInfo info, otherInfo;

    private IGameSceneModel _gameSceneModel;
    protected override void OnInit()
    {
        _gameSceneModel = this.GetModel<IGameSceneModel>();
        this.RegisterEvent<PlayOnEvent>(PlayOn);
    }

    void PlayOn(PlayOnEvent e)
    {
        //int startIndex = _gameSceneModel.Bubbles.Count - 7;
        //_gameSceneModel.Bubbles.RemoveRange(startIndex, 5);
    }

    public void GetBubbleContact(int id, int otherId)
    {
        info = _gameSceneModel.Bubbles[id];
        otherInfo = _gameSceneModel.Bubbles[otherId];
        List<int> sameColorTypes = CountDistinctCommonValues(info.jellyColor, otherInfo.jellyColor);
        if (sameColorTypes.Count == 1)
        {
            MergeBubbleWithOneSameColor(id, otherId, sameColorTypes[0]); //merge if only one single color
        }
        else if (sameColorTypes.Count != 1)
        {
            MergeBubbleWithManySameColor(id, otherId, sameColorTypes);
        }
    }
    public async void MergeBubbleWithOneSameColor(int id, int otherId, int color)
    {
        if (BothSingleColor(info.jellyColor, otherInfo.jellyColor)) //if both of them are single color
        {
            if (info.Size > otherInfo.Size) //one of them is bigger
            {
                TakeAllJelly(info, otherInfo);
                this.SendEvent(new MergeBubbleEvent() //merge the smaller and destroy it
                {
                    mergeStatus = 1,
                    mergeId = id,
                    breakId = otherId
                });
                IsDone(id, info); //if the merge one is full => done
            }
            else if (info.Size == otherInfo.Size) //if they have same size
            {
                if (id > otherId) //choose the one drop later (bigger id)
                {
                    await SendEventType1(id, otherId);
                }
            }
        }
        else
        {
            if (IsSingleColor(info.jellyColor))
            {
                await SendEventType2(id, otherId, color);
            }
            else
            if (!IsSingleColor(otherInfo.jellyColor) && !IsSingleColor(info.jellyColor)) //if both are not single color
            {
                if (CountAmountOfColor(info.jellyColor, color) > CountAmountOfColor(otherInfo.jellyColor, color))
                {
                    await SendEventType3(id, otherId, color);
                }
                else if (CountAmountOfColor(info.jellyColor, color) == CountAmountOfColor(otherInfo.jellyColor, color))
                {
                    if (id > otherId)
                    {
                        await SendEventType3(id, otherId, color);
                    }
                }

            }
        }
    }

    async void MergeBubbleWithManySameColor(int id, int otherId, List<int> sameColorTypes)
    {
        if (sameColorTypes.Count == 2)
        {
            if (CountAmountOfColor(info.jellyColor, sameColorTypes[0]) == CountAmountOfColor(otherInfo.jellyColor, sameColorTypes[0])
                && CountAmountOfColor(info.jellyColor, sameColorTypes[1]) == CountAmountOfColor(otherInfo.jellyColor, sameColorTypes[1]))
            {
                if (id > otherId)
                {
                    await SendEventType4(id, otherId, sameColorTypes);
                }

            }
            else
            {
                if (id > otherId)
                {
                    await SendEventType4(id, otherId, sameColorTypes);
                }
            }
        }
    }

    UniTask SendEventType1(int id, int otherId)
    {
        TakeAllJelly(info, otherInfo);
        this.SendEvent(new MergeBubbleEvent()
        {
            mergeStatus = 1,
            mergeId = id,
            breakId = otherId
        });
        IsDone(id, info);
        return UniTask.CompletedTask;
    }

    UniTask SendEventType2(int id, int otherId, int color)
    {
        TakeJelly(info, otherInfo, color);
        this.SendEvent(new MergeBubbleEvent()
        {
            mergeStatus = 2,
            mergeId = id,
            breakId = otherId,
            colorType = color
        });
        IsDone(id, info);
        return UniTask.CompletedTask;
    }

    UniTask SendEventType3(int id, int otherId, int color)
    {
        bool isFull = false;
        int moveAmount = TradeJellyWithOneSameColor(info, otherInfo, color, ref isFull).ConvertTo<int>();

        this.SendEvent(new MergeBubbleEvent()
        {
            mergeStatus = 3,
            mergeId = id,
            breakId = otherId,
            colorType = color,
            moveAmount = moveAmount,
            isFull = isFull
        });
        IsDone(id, info);
        return UniTask.CompletedTask;
    }

    UniTask SendEventType4(int id, int otherId, List<int> sameColorTypes)
    {
        int moveAmount = TradeJellyWithTwoSameColor(info, otherInfo, sameColorTypes);

        this.SendEvent(new MergeBubbleEvent()
        {
            mergeStatus = 4,
            mergeId = id,
            breakId = otherId,
            colorTypes = sameColorTypes,
            moveAmount = moveAmount
        });
        IsDone(id, info);
        IsDone(otherId, otherInfo);
        return UniTask.CompletedTask;
    }

    void TakeAllJelly(BubbleInfo take, BubbleInfo beingTaken)
    {
        foreach (int jelly in beingTaken.jellyColor)
        {
            take.jellyColor.Add(jelly);
        }
        beingTaken.jellyColor.Clear();
    }

    void TakeJelly(BubbleInfo take, BubbleInfo beingTaken, int color)
    {
        foreach (int jelly in beingTaken.jellyColor)
        {
            if (jelly == color)
            {
                take.jellyColor.Add(jelly);
            }
        }
        beingTaken.jellyColor.RemoveAll(jelly => jelly == color);
    }

    int TradeJellyWithOneSameColor(BubbleInfo take, BubbleInfo beingTaken, int color, ref bool isFull)
    {
        //Debug.Log("Before: " + JsonConvert.SerializeObject(take.jellyColor));
        //Debug.Log("Before: " + JsonConvert.SerializeObject(beingTaken.jellyColor));
        List<int> tempColors = new List<int>();
        foreach (int jelly in beingTaken.jellyColor)
        {
            if (jelly == color)
            {
                tempColors.Add(jelly);
            }
        }
        bool beingTakenFull = false;
        //Debug.Log("Temp list: " + JsonConvert.SerializeObject(tempColors));
        foreach (int jelly in tempColors)
        {
            take.jellyColor.Add(jelly);
            beingTaken.jellyColor.Remove(jelly);
        }
        tempColors.Clear();
        int movedCount = 0; //Increase everytime a jelly is moved
        foreach (int jelly in take.jellyColor)
        {
            if (jelly != color)
            {
                tempColors.Add(jelly);
            }
        }
        //Debug.Log("Temp list: " + JsonConvert.SerializeObject(tempColors));
        foreach (int jelly in tempColors)
        {
            beingTaken.jellyColor.Add(jelly);
            movedCount++;
            take.jellyColor.Remove(jelly);
            if (beingTaken.jellyColor.Count == beingTaken.MaxNumb)
            {
                //Debug.Log("Reach max");
                beingTakenFull = true;
                break;
            }
        }
        isFull = beingTakenFull;
        //Debug.Log("After: " + JsonConvert.SerializeObject(take.jellyColor));
        //Debug.Log("After: " + JsonConvert.SerializeObject(beingTaken.jellyColor));
        return movedCount;
    }

    int TradeJellyWithTwoSameColor(BubbleInfo take, BubbleInfo beingTaken, List<int> sameColor)
    {
        //Debug.Log("Before: " + JsonConvert.SerializeObject(take.jellyColor));
        //Debug.Log("Before: " + JsonConvert.SerializeObject(beingTaken.jellyColor));
        int movedCount = 0;
        List<int> tempColors = new List<int>();
        foreach (int jelly in beingTaken.jellyColor)
        {
            if (jelly == sameColor[0])
            {
                tempColors.Add(jelly);
            }
        }
        //Debug.Log("Temp list: " + JsonConvert.SerializeObject(tempColors));
        foreach (var jelly in tempColors)
        {
            take.jellyColor.Add(jelly);
            beingTaken.jellyColor.Remove(jelly);
        }
        tempColors.Clear();
        foreach (int jelly in take.jellyColor)
        {
            if (jelly == sameColor[1])
            {
                tempColors.Add(jelly);
            }
        }
        //Debug.Log("Temp list: " + JsonConvert.SerializeObject(tempColors));
        foreach (int jelly in tempColors)
        {
            take.jellyColor.Remove(jelly);
            beingTaken.jellyColor.Add(jelly);
            movedCount++;
            if (beingTaken.jellyColor.Count >= beingTaken.MaxNumb)
            {
                Debug.Log("Reach max");
                break;
            }
        }
        //Debug.Log("After: " + JsonConvert.SerializeObject(take.jellyColor));
        //Debug.Log("After: " + JsonConvert.SerializeObject(beingTaken.jellyColor));
        return movedCount;
    }

    int CountAmountOfColor(List<int> list, int color)
    {
        int count = 0;
        foreach (int jelly in list)
        {
            if (jelly == color) count++;
        }
        return count;
    }

    private List<int> CountDistinctCommonValues(List<int> list1, List<int> list2)
    {
        List<int> sameColorTypes = new List<int>();
        HashSet<int> set1 = new HashSet<int>(list1);
        HashSet<int> set2 = new HashSet<int>(list2);

        foreach (int value in set1)
        {
            if (set2.Contains(value))
            {
                sameColorTypes.Add(value);
            }
        }

        return sameColorTypes;
    }

    void IsDone(int bubbleId, BubbleInfo bubble)
    {
        if (bubble.jellyColor.Count >= bubble.MaxNumb && IsSingleColor(bubble.jellyColor))
        {
            this.SendEvent(new DoneBubbleEvent()
            {
                id = bubbleId,
                type = bubble.jellyColor[0],
                size = bubble.Size
            });
        }
    }

    bool BothSingleColor(List<int> list1, List<int> list2)
    {
        if (IsSingleColor(list1) && IsSingleColor(list2)) return true;
        return false;
    }

    bool IsSingleColor(List<int> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] != list[0])
            {
                return false;
            }
        }
        return true;
    }
}
