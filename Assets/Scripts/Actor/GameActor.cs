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


    protected int hp;
    public int Hp{
        get=>hp;
        set{hp=value;}
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
        transform.SetParent(tile.transform);
        transform.localPosition = new Vector3(0, 0, 0);
		Init0();
    }
    List<GameActor> blockActors;
    public List<GameActor> BlockActors{
        get=>blockActors;
        set{blockActors=value;}
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
