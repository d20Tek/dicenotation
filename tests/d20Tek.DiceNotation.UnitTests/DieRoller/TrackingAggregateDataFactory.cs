using d20Tek.DiceNotation.DieRoller;

namespace d20Tek.DiceNotation.UnitTests.DieRoller;

internal static class TrackingAggregateDataFactory
{
    public static void SetupTrackingSampleData(this IDieRollTracker tracker)
    {
        tracker.AddDieRoll(6, 4, typeof(RandomDieRoller));
        tracker.AddDieRoll(6, 3, typeof(RandomDieRoller));
        tracker.AddDieRoll(6, 1, typeof(RandomDieRoller));
        tracker.AddDieRoll(6, 6, typeof(RandomDieRoller));
        tracker.AddDieRoll(8, 2, typeof(ConstantDieRoller));
        tracker.AddDieRoll(8, 4, typeof(ConstantDieRoller));
        tracker.AddDieRoll(10, 2, typeof(ConstantDieRoller));
        tracker.AddDieRoll(10, 8, typeof(ConstantDieRoller));
        tracker.AddDieRoll(10, 9, typeof(ConstantDieRoller));
        tracker.AddDieRoll(10, 4, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 5, typeof(ConstantDieRoller));
        tracker.AddDieRoll(20, 18, typeof(ConstantDieRoller));
        tracker.AddDieRoll(20, 11, typeof(ConstantDieRoller));
        tracker.AddDieRoll(20, 5, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 5, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 17, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 12, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 20, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 8, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 11, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 12, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 20, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 14, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 13, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
    }

    public static List<AggregateDieTrackingData> SetupStatisticalTrackingData(this IDieRollTracker tracker)
    {
        SetupTrackingSampleData(tracker);

        tracker.AddDieRoll(20, 5, typeof(ConstantDieRoller));
        tracker.AddDieRoll(20, 18, typeof(ConstantDieRoller));
        tracker.AddDieRoll(20, 11, typeof(ConstantDieRoller));
        tracker.AddDieRoll(20, 5, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 5, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 17, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 12, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 20, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 8, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 11, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 10, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 12, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 20, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 14, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 13, typeof(RandomDieRoller));
        tracker.AddDieRoll(20, 9, typeof(RandomDieRoller));

        List<AggregateDieTrackingData> expectedAggegate =
        [
            new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "10", Result = 2, Count = 1, Percentage = 33.3f },
            new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "10", Result = 8, Count = 1, Percentage = 33.3f },
            new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "10", Result = 9, Count = 1, Percentage = 33.3f },
            new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "20", Result = 5, Count = 2, Percentage = 33.3f },
            new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "20", Result = 11, Count = 2, Percentage = 33.3f },
            new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "20", Result = 18, Count = 2, Percentage = 33.3f },
            new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "8", Result = 2, Count = 1, Percentage = 50.0f },
            new AggregateDieTrackingData { RollerType = "ConstantDieRoller", DieSides = "8", Result = 4, Count = 1, Percentage = 50.0f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "10", Result = 4, Count = 1, Percentage = 100.0f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 5, Count = 4, Percentage = 13.8f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 8, Count = 2, Percentage = 6.9f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 9, Count = 6, Percentage = 20.7f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 10, Count = 1, Percentage = 3.4f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 11, Count = 2, Percentage = 6.9f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 12, Count = 4, Percentage = 13.8f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 13, Count = 2, Percentage = 6.9f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 14, Count = 2, Percentage = 6.9f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 17, Count = 2, Percentage = 6.9f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "20", Result = 20, Count = 4, Percentage = 13.8f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "6", Result = 1, Count = 1, Percentage = 25.0f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "6", Result = 3, Count = 1, Percentage = 25.0f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "6", Result = 4, Count = 1, Percentage = 25.0f },
            new AggregateDieTrackingData { RollerType = "RandomDieRoller", DieSides = "6", Result = 6, Count = 1, Percentage = 25.0f }
        ];

        return expectedAggegate;

        /* Expected aggregate data view
            ConstantDieRoller	10	2	1
            ConstantDieRoller	10	8	1
            ConstantDieRoller	10	9	1
            ConstantDieRoller	20	5	2
            ConstantDieRoller	20	11	2
            ConstantDieRoller	20	18	2
            ConstantDieRoller	8	2	1
            ConstantDieRoller	8	4	1
            RandomDieRoller	    10	4	1
            RandomDieRoller	    20	5	4
            RandomDieRoller	    20	8	2
            RandomDieRoller	    20	9	6
            RandomDieRoller	    20	10	1
            RandomDieRoller	    20	11	2
            RandomDieRoller	    20	12	4
            RandomDieRoller	    20	13	2
            RandomDieRoller	    20	14	2
            RandomDieRoller	    20	17	2
            RandomDieRoller	    20	20	4
            RandomDieRoller	    6	1	1
            RandomDieRoller	    6	3	1
            RandomDieRoller	    6	4	1
            RandomDieRoller	    6	6	1
        */
    }
}
