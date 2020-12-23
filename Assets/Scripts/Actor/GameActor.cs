using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActor : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    GameBoard board;
    public GameBoard Board{
        get=>board;
        set{board=value;}
    }
    GameTile tile;
    public GameTile Tile{
        get=>tile;
        set{tile=value;}
    }

    public void Init(GameTile tile, GameBoard board){
        this.tile = tile;
        this.board = board;
		Init0();
    }
	TowerFactory originFactory;
	public TowerFactory OriginFactory {
		get => originFactory;
		set {
			Debug.Assert(originFactory == null, "Redefined origin factory!");
			originFactory = value;
		}
	}
	protected virtual void Init0 () { }
	public virtual void GameUpdate () { }
}
