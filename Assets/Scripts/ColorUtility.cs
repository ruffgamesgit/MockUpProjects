using System;

namespace DefaultNamespace
{
    public static class ColorUtility
    {
        private static readonly Random _random = new Random();

        public static ColorEnum GetRandomColorEnum()
        {
            Array values = Enum.GetValues(typeof(ColorEnum));
            int randomIndex = _random.Next(1, values.Length);
            return (ColorEnum)values.GetValue(randomIndex);
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