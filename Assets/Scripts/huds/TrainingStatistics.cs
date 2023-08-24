using UnityEngine;
using System.Collections;

[System.Serializable]
public class TrainingStatistics : MonoBehaviour
{
    public static TrainingStat[] statistics;
    public static int shootsFired;
    public static int totalHits;
    public static int grenadeFired;
    public static int headShoot;
    public static int turrets;
    public static int dummies;
    public static int turretsHit;
    public static int dummiesHit;
    public static int eaglesEye;
    public static int totalEaglesEye;
    public static int blueLeaf;
    public static int totalBlueLeaf;
    public static int head;
    public static int chest;
    public static int heart;
    public static int lArm;
    public static int rArm;
    public static int torso;
    public static void ResetStatistics()
    {
        TrainingStatistics.statistics = new TrainingStat[14];
        TrainingStatistics.head = 0;
        TrainingStatistics.chest = 0;
        TrainingStatistics.heart = 0;
        TrainingStatistics.lArm = 0;
        TrainingStatistics.rArm = 0;
        TrainingStatistics.torso = 0;
        TrainingStatistics.totalHits = 0;
        TrainingStatistics.shootsFired = 0;
        TrainingStatistics.grenadeFired = 0;
        TrainingStatistics.headShoot = 0;
        TrainingStatistics.turrets = 0;
        TrainingStatistics.dummies = 0;
        TrainingStatistics.turretsHit = 0;
        TrainingStatistics.dummiesHit = 0;
        TrainingStatistics.eaglesEye = 0;
        TrainingStatistics.blueLeaf = 0;
        TrainingStatistics.totalEaglesEye = 0;
        TrainingStatistics.totalBlueLeaf = 0;
        TrainingStat t = null;
        int i = 0;
        while (i < 14)
        {
            t = new TrainingStat();
            t.dummyPart = (DummyPart) i;
            t.name = t.dummyPart.ToString().Replace("_", " ");
            t.points = 0;
            TrainingStatistics.statistics[i] = t;
            i++;
        }
    }

    public static void Register(DummyPart part)
    {
        if (TrainingStatistics.statistics == null)
        {
            return;
        }
        int i = (int) part;
        if ((i < 0) || (i > (TrainingStatistics.statistics.Length - 1)))
        {
            return;
        }
        TrainingStatistics.totalHits++;
        if (!(TrainingStatistics.statistics[i] == null))
        {
            TrainingStatistics.statistics[i].points++;
        }
        switch (part)
        {
            case DummyPart.HEAD:
                TrainingStatistics.head++;
                break;
            case DummyPart.NECK:
                TrainingStatistics.head++;
                break;
            case DummyPart.FACE:
                TrainingStatistics.head++;
                break;
            case DummyPart.CHEST:
                TrainingStatistics.chest++;
                break;
            case DummyPart.STOMACH:
                TrainingStatistics.torso++;
                break;
            case DummyPart.LOWER_STOMACH:
                TrainingStatistics.torso++;
                break;
            case DummyPart.LEFT_HAND:
                TrainingStatistics.lArm++;
                break;
            case DummyPart.LEFT_FOREARM:
                TrainingStatistics.lArm++;
                break;
            case DummyPart.LEFT_ARM:
                TrainingStatistics.lArm++;
                break;
            case DummyPart.RIGHT_HAND:
                TrainingStatistics.rArm++;
                break;
            case DummyPart.RIGHT_FOREARM:
                TrainingStatistics.rArm++;
                break;
            case DummyPart.RIGHT_ARM:
                TrainingStatistics.rArm++;
                break;
            case DummyPart.HEART:
                TrainingStatistics.heart++;
                break;
            case DummyPart.OTHER:
                TrainingStatistics.torso++;
                break;
        }
    }

}
public enum DummyPart
{
    HEAD = 0,
    NECK = 1,
    FACE = 2,
    CHEST = 3,
    STOMACH = 4,
    LOWER_STOMACH = 5,
    LEFT_HAND = 6,
    LEFT_FOREARM = 7,
    LEFT_ARM = 8,
    RIGHT_HAND = 9,
    RIGHT_FOREARM = 10,
    RIGHT_ARM = 11,
    OTHER = 12,
    HEART = 13
}