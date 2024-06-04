using System.Collections.Generic;
using Random = System.Random;

namespace DefaultNamespace
{
    public static class ColorUtility
    {
        private static readonly Random _random = new();

        public static ColorEnum GetRandomColorEnum()
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