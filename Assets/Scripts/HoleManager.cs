using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoSingleton<HoleManager>
{
    public List<ColoredHoleController> coloredHoles;


    public ColoredHoleController GetCurrentHole()
    {
        return coloredHoles[0];
    }
}