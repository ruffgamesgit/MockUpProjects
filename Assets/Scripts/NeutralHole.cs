using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NeutralHole : BaseHoleClass
{
    public static NeutralHole instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        HoleManager.instance.NewColoredHoleCameEvent += OnNewColoredHoleCame;
    }

    private void OnNewColoredHoleCame(ColoredHole newColoredHole)
    {
        ColourEnum holeColor = newColoredHole.GetColorEnum();
        List<BaseBoltClass> matchedBolts = new();

        foreach (PlacementPoint point in placementPoints)
        {
            BaseBoltClass bolt = point.GetBolt();
            if (bolt && bolt.GetColor() == holeColor)
            {
                matchedBolts.Add(bolt);
            }
        }

        int lastIndex = matchedBolts.Count >= 3 ? 3 : matchedBolts.Count;
        for (int i = 0; i < matchedBolts.Count; i++)
        {
            if (newColoredHole.GetAvailablePoint() != null)
            {
                PlacementPoint coloredHolePoint = newColoredHole.GetAvailablePoint();
                if (i == lastIndex - 1)
                    matchedBolts[i].GoToPoint(coloredHolePoint, newColoredHole);
                else
                {
                    matchedBolts[i].GoToPoint(coloredHolePoint);
                }
            }
        }
    }

    public override void OnBoltArrived()
    {
        List<BaseBoltClass> bolts = new();
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].GetBolt())
                bolts.Add(placementPoints[i].GetBolt());
        }

        if (bolts.Count != placementPoints.Count) return;

        bool hasMatch = false;
        ColourEnum coloredHoleColor = HoleManager.instance.GetCurrentHole().GetColorEnum();
        for (int i = 0; i < bolts.Count; i++)
        {
            if (bolts[i].GetColor() == coloredHoleColor)
                hasMatch = true;
        }

        if (hasMatch) return;
 
        Debug.LogWarning("No Placeable point left on NEUTRAL HOLE, FAIL");
      //  GameManager.instance.EndGame(false);
    }
}