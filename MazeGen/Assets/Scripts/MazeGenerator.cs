using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MazeGenerator : MonoBehaviour
{
    [Header("Checks")]
    [SerializeField] private MazePiece piecePrefab;
    [SerializeField] private Vector2Int mazeSize;
    [SerializeField] private bool isGenerating = false;
    [SerializeField] private bool mazeDone = false;
    
    
    [Space(5)] [Header("Public Variables")]
    public float generationTimer = 0.01f;
    
    List<MazePiece> pieces = new List<MazePiece>();
    private int randIndex = 0;
    private int[] randomNum = {72, 99, 56, 34, 43, 62, 31, 4, 70, 22, 6, 65, 96, 71, 29, 9, 98, 41, 90, 7, 30, 3, 97,
    49, 63, 88, 47, 82, 91, 54, 74, 2, 86, 14, 58, 35, 89, 11, 10, 60, 28, 21, 52, 50, 55, 69, 76, 94, 23, 66, 15, 57,
    44, 18, 67, 5, 24, 33, 77, 53, 51, 59, 20, 42, 80, 61, 1, 0, 38, 64, 45, 92, 46, 79, 93, 95, 37, 40, 83, 13, 12, 78,
    75, 73, 84, 81, 8, 32, 27, 19, 87, 85, 16, 25, 17, 68, 26, 39, 48, 36};
    
    public void Generate()
    {
        if (!isGenerating)
            StartCoroutine(GenerateMaze(mazeSize));
    }
    
    private IEnumerator GenerateMaze(Vector2Int size)
    {
        isGenerating = true;
        
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2 piecePos = new Vector2(x, y);
                MazePiece newPiece = Instantiate(piecePrefab, piecePos, Quaternion.identity, transform);
                pieces.Add(newPiece);

                yield return null;
            }
        }

        List<MazePiece> currentPath = new List<MazePiece>();
        List<MazePiece> completedPieces = new List<MazePiece>();

        currentPath.Add(pieces[0]);
        currentPath[0].SetState(State.Working);

        while (completedPieces.Count < pieces.Count)
        {
            List<int> possibleNextPieces = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentPieceIndex = pieces.IndexOf(currentPath[^1]);
            int currentPieceX = currentPieceIndex / size.y;
            int currentPieceY = currentPieceIndex % size.y;

            
            //Top
            if (currentPieceY < size.y - 1)
            {
                if (!completedPieces.Contains(pieces[currentPieceIndex + 1]) &&
                    !currentPath.Contains(pieces[currentPieceIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextPieces.Add(currentPieceIndex + 1);
                }
            }
            
            //Right
            if (currentPieceX < size.x - 1)
            {
                if (!completedPieces.Contains(pieces[currentPieceIndex + size.y]) &&
                    !currentPath.Contains(pieces[currentPieceIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextPieces.Add(currentPieceIndex + size.y);
                }
            }
            
            //Bottom
            if (currentPieceY > 0)
            {
                if (!completedPieces.Contains(pieces[currentPieceIndex - 1]) &&
                    !currentPath.Contains(pieces[currentPieceIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextPieces.Add(currentPieceIndex - 1);
                }
            }
            
            //Left
            if (currentPieceX > 0)
            {
                if (!completedPieces.Contains(pieces[currentPieceIndex - size.y]) &&
                    !currentPath.Contains(pieces[currentPieceIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextPieces.Add(currentPieceIndex - size.y);
                }
            }
            
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = 0;
                MazePiece chosenPiece = null;
                
                if (possibleDirections.Count == 1)
                {
                    chosenDirection = 0;
                }
                else if (possibleDirections.Count >= 2)
                {
                    chosenDirection = GetRan() % possibleDirections.Count;
                }

                chosenPiece = pieces[possibleNextPieces[chosenDirection]];
                
                switch (possibleDirections[chosenDirection])
                {
                    case 1:
                        chosenPiece.RemoveWall(1);
                        currentPath[^1].RemoveWall(0);
                        break;
                    case 2:
                        chosenPiece.RemoveWall(0);
                        currentPath[^1].RemoveWall(1);
                        break;
                    case 3:
                        chosenPiece.RemoveWall(3);
                        currentPath[^1].RemoveWall(2);
                        break;
                    case 4:
                        chosenPiece.RemoveWall(2);
                        currentPath[^1].RemoveWall(3);
                        break;
                }
                
                
                currentPath.Add(chosenPiece);
                chosenPiece.SetState(State.Working);
            }
            else
            {
                completedPieces.Add(currentPath[^1]);
                
                currentPath[^1].SetState(State.Finalized);
                currentPath.RemoveAt(currentPath.Count - 1);
            }

            yield return new WaitForSeconds(generationTimer);
        }

        mazeDone = true;

        if (mazeDone)
        {
            FinishMaze();
        }
    }

    private void FinishMaze()
    {
        foreach (var piece in pieces)
        {
            piece.SetState(State.Done);
        }

        isGenerating = false;
    }

    private int GetRan()
    {
        int old = randIndex;
        randIndex++;
        randIndex %= 100;

        if (randIndex == 99)
        {
            randIndex = 0;
        }
        
        return randomNum[old];
    }

    private void ResetMaze()
    {
        foreach (var piece in pieces)
        {
            Destroy(piece.gameObject);
        }
        
        pieces.Clear();
        mazeDone = false;
    }
    
    public void ChangeX(string x)
    {
        mazeSize.x = int.Parse(x);
    }
    
    public void ChangeY(string y)
    {
        mazeSize.y = int.Parse(y);
    }
    
    public void ChangeTime(string time)
    {
        generationTimer = int.Parse(time);
    }
}
