using System;
using System.Collections;
public static class Utility 
{
    public static T[] ShuffleArray<T>(T[] array, int seed) {
        Random prng = new Random(seed);
        for (int i = 0; i < array.Length-1; i++)//we dont need the last iterration
        {
            int randomeIndex = prng.Next(i, array.Length);
            T tempItem=array[randomeIndex];
            array[randomeIndex]=array[i];
            array[i]=tempItem;
        }
        return array;   
    }
}
