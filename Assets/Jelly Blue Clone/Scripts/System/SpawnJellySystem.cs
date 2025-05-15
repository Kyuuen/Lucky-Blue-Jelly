using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;
using System.Linq;
using Newtonsoft.Json;

public interface ISpawnJellySystem : ISystem
{
    public bool gameStarting { get; set; }
    public GameObject GetJellyToSpawn(int type, List<GameObject> jellyPrefabs);

    public void GetSpawnedColor(int type);
    public void GetColorSpawnRate(int rate);
    public void GetRandomJellyInfo(BubbleInfo newBubble);
    public void SetJellyInfo(BubbleInfo newBubble, int numb, int numColor, int colorRate);
    //public void SetJellyInfo();
    public void EnsureUniqueValues<T>(List<T> list);
}

public class SpawnJellySystem : AbstractSystem, ISpawnJellySystem
{

    private int sameColorRate;
    public bool gameStarting { get; set; }
    private List<int> spawnedColors;

    protected override void OnInit()
    {
        spawnedColors = new List<int>();
    }

    public GameObject GetJellyToSpawn(int type, List<GameObject> jellyPrefabs)
    {
        for (int i = 0; i < jellyPrefabs.Count; i++)
        {
            if (i == type)
            {
                return jellyPrefabs[i];
            }
            else
            {
                //Debug.Log($"Jelly type not found, Id: {type.colorId}");
            }
        }
        return null;
    }

    public void GetSpawnedColor(int type)
    {
        spawnedColors.Add(type);
    }

    public void GetColorSpawnRate(int rate)
    {
        sameColorRate = rate;
        spawnedColors.Clear();
    }
    public void GetRandomJellyInfo(BubbleInfo newBubble)
    {
        List<int> availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6 }; // Representing 7 colors by their indexes
        System.Random random = new System.Random();

        int color;
        // Randomly select 3 unique colors
        List<int> selectedColors = new List<int>();
        int numOfColor = Random.Range(1, 3);
        bool sameColor = (Random.Range(0, 100) < sameColorRate); //Random base on rate if new bubble contain more color than spawned
        if (sameColor) //new will have the sam color as spawned
        {
            while (selectedColors.Count < numOfColor)
            {
                int index = random.Next(spawnedColors.Count);
                color = spawnedColors[index];

                selectedColors.Add(color);
                
            }
        }
        else //new might get the different colors
        {
            while (selectedColors.Count < numOfColor)
            {
                int index = random.Next(availableColors.Count); //Get one index from 7 type
                color = availableColors[index]; //Get color at that index

                selectedColors.Add(color); //Add new color

            }

        }

        int numbJelly = Random.Range(numOfColor, newBubble.MaxNumb);
        if (numbJelly == numOfColor)
        {
            foreach (var type in selectedColors)
            {
                newBubble.jellyColor.Add(type);
            }
        }
        else
        {
            for (int i = 0; i < numbJelly; i++)
            {
                color = Random.Range(0, numOfColor);
                newBubble.jellyColor.Add(selectedColors[color]);
            }
        }

    }

    public void SetJellyInfo(BubbleInfo newBubble, int numb, int numColor, int colorRate)
    {
        List<int> availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6 }; // Representing 7 colors by their indexes
        System.Random random = new System.Random();

        int color;
        // Randomly select 3 unique colors
        List<int> selectedColors = new List<int>();
        int numOfColor = numColor;

        while (selectedColors.Count < numOfColor)
        {
            int index = random.Next(availableColors.Count); //Get one index from 7 type
            color = availableColors[index]; //Get color at that index
            if (!selectedColors.Contains(color)) //Check if the color is already contained in the list
            {
                selectedColors.Add(color); //Add new color
            }
            GetSpawnedColor(color);
        }


        int numbJelly = numb;
        if (numbJelly == numOfColor)
        {
            foreach (var type in selectedColors)
            {
                newBubble.jellyColor.Add(type);
            }
        }
        else
        {
            for (int i = 0; i < selectedColors.Count - 1; i++)
            {
                for (int j = 0; j < colorRate; j++)
                {
                    newBubble.jellyColor.Add(selectedColors[i]);
                }
                numbJelly -= colorRate;
            }
            for (int i = 0; i < numbJelly; i++) newBubble.jellyColor.Add(selectedColors[^1]);
        }

    }

    public void EnsureUniqueValues<T>(List<T> list)
    {
        // Find duplicates
        var duplicates = list.GroupBy(x => x)
                             .Where(g => g.Count() > 1)
                             .Select(g => g.Key)
                             .ToList();

        // Remove all duplicates
        foreach (var value in duplicates)
        {
            list.RemoveAll(x => EqualityComparer<T>.Default.Equals(x, value));
        }

        // Re-add one instance of each duplicate
        list.AddRange(duplicates);
    }
}
