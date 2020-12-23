using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFactory
{
    static Dictionary<int, TrackerFactory> trackerTypes;
    static TargetFactory(){
        trackerTypes = new Dictionary<int, TrackerFactory>();
        trackerTypes.Add(1, new FileRangeTrackerFactory());
    }

    public static AbstractTrack CreateTracker(int rangeType){
        return trackerTypes[rangeType].CreateTracker();
    }
}
