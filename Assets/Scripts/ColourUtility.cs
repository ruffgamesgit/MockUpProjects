public enum ColourEnum
{
    NONE, 
    RED,
    BLUE,
    GREEN,
    PURPLE,
    YELLOW
}
public static class ColourUtility
{
    public static bool CheckIfColorsMatch(ColourEnum boltColor, ColourEnum placementPointColor)
    {
        return boltColor == placementPointColor;
    }
}