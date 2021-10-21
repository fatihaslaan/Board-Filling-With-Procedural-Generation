using UnityEngine;
using UnityEngine.SceneManagement;

public class PieceLocator : MonoBehaviour
{
    public bool located = false;
    public PieceData pieceData;
    public int rotation=0;

    Vector2 mousePosition;
    Vector3 startingPosition;
    Vector2 currentPosition;

    BoardManager board;

    void Start()
    {
        pieceData=new PieceData(GetComponent<PieceBehaviour>());
        pieceData.ChangeRotation(rotation); //Update piece borders

        board = BoardManager.GetInstance();
        board.selectedPieces.Add(this);
    }

    void OnMouseDown() //To give a better drag effect
    {
        startingPosition = transform.position - Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
    }

    void OnMouseDrag() //Moving piece
    {
        mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        currentPosition = Camera.main.ScreenToWorldPoint(mousePosition) + startingPosition;
        transform.position = currentPosition;
    }

    void OnMouseUp()
    {
        located = false;
        for (int i = 0; i < board.boardWidth; i++)
        {
            for (int j = 0; j < board.boardHeight; j++)
            {
                if (Vector3.Distance(transform.position, board.GetPositionByCoordinates(i, j)) < 0.75f) //Piece is close to a cell
                {
                    pieceData.location[0] = i;
                    pieceData.location[1] = j;
                    if (GlobalAttributes.IsInsideBoard(pieceData, board.boardWidth, board.boardHeight)) //Piece borders are inside board
                    {
                        located = true;
                        transform.position = board.GetPositionByCoordinates(i, j);
                        if (IsBoardFilled()) //Board is filled game is over
                        {
                            GlobalAttributes.currentLevel++;
                            Debug.Log("Game over");
                            SceneManager.LoadScene(0);
                        }
                    }
                }
            }
        }

    }

    bool IsBoardFilled()
    {
        GlobalAttributes.EmptyCells(board.boardCells);
        board.FillCells();
        if (GlobalAttributes.IsBoardFilled(board.boardCells))
            return true;
        return false;
    }
}