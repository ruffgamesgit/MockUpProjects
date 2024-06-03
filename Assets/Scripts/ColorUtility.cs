using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public static class ColorUtility
    {
        private static readonly Random _random = new Random();

        public static ColorEnum GetRandomColorEnum( )
        {
            List<ColorEnum> uniqueColors = LayerManager.instance.GetUniqueColorInBoxes();
            if (uniqueColors.Count != 0)
            {
                int randomIndex = _random.Next(0, uniqueColors.Count);
                return uniqueColors[randomIndex];
            }

            return ColorEnum.NONE;
        }
    }
}

public enum ColorEnum
{
    NONE,
    BLUE,
    GREEN,
    ORANGE,
    PINK
}


[System.Serializable]
public class ColorData
{
    public ColorEnum ColorEnum;
    public int count;
}