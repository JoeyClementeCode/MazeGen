using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Free,
    Working,
    Finalized,
    Done
}

public class MazePiece : MonoBehaviour
{
    [SerializeField] private GameObject[] walls;
    [SerializeField] private GameObject floor;

    public void RemoveWall(int direction)
    {
        walls[direction].gameObject.SetActive(false);
    }
    
    public void SetState(State state)
    {
        switch (state)
        {
            case State.Free:
                floor.GetComponent<SpriteRenderer>().color = Color.white;
                break;
            case State.Working:
                floor.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            case State.Finalized:
                floor.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case State.Done:
                floor.GetComponent<SpriteRenderer>().color = Color.grey;
                break;
        }
    }
}
