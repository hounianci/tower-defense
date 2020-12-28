using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileRangeTrack : AbstractTrack
{

    int rangeId;
    List<List<int>> range;
	Vector2Int targetRangeSelfPos;
    public int RangeId{
        get=>rangeId;
        set{rangeId=value;}
    }

    protected override void TurnRange0(Direction newDirection){
        DirectionChange change = DirectionExtensions.GetDirectionChangeTo(direction, newDirection);
		if(change==DirectionChange.TurnLeft){
			range = FileUtil.matrixTurnRight(range);
		}else if(change==DirectionChange.TurnRight){
			range = FileUtil.matrixTurnLeft(range);
		}else if(change==DirectionChange.TurnAround){
			if(newDirection==Direction.North || newDirection==Direction.South){
				range = FileUtil.matrixTurnAround(range);
			}else{
				range = FileUtil.matrixTurnAround2(range);
			}
		}
		loadSelfRangePos();
    }



    protected override Vector2Int RangeSelfOffet(){
        return targetRangeSelfPos;
    }

    protected override void Init0(int rangeId)
    {
		this.rangeId = rangeId;
        range = loadFile();
        loadSelfRangePos();
    }

    public override List<List<int>> Range(){
        return range;
    }
	private List<List<int>> loadFile(){
		List<List<int>> range = FileUtil.readFileMatrix(string.Format("Assets/TargetRange/{0}_Range.txt", rangeId));
		return FileUtil.matrixTurnAround(range);
	}

	public void loadSelfRangePos(){
		for(int i=0; i<range.Count; i++){
			for(int j=0; j<range[i].Count; j++){
				if(range[i][j]==2||range[i][j]==3){
					targetRangeSelfPos = new Vector2Int(j, i);
				}
			}
		}
	}
}
