using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileRangeTrackerFactory : TrackerFactory
{
    public AbstractTrack CreateTracker(){
        return new FileRangeTrack();
    }
}
