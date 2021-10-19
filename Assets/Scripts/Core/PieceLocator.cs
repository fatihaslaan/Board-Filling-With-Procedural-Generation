using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PieceLocator : MonoBehaviour
{
    public bool located = false;
    public PieceBehaviour piece;

    Vector2 mousePosition;
    Vector3 startingPosition;
    Vector2 currentPosition;

    BoardManager board;

    void Start()
    {
        piece = GetComponent<PieceBehaviour>();
        board = BoardManager.GetInstance();
        board.selectedPieces.Add(this);
    }

    void OnMouseDown()
    {
        startingPosition = transform.position - Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
    }

    void OnMouseDrag()
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
                if (Vector3.Distance(transform.position, board.GetPositionByLocation(i, j)) < 0.5f) //Piece is close to a cell
                {
                    piece.location[0] = i;
                    piece.location[1] = j;
                    if (GlobalAttributes.IsInsideBoard(piece, board.boardWidth, board.boardHeight)) //Piece borders are inside board
                    {
                        located = true;
                        transform.position = board.GetPositionByLocation(i, j);
                        if (IsGameOver())
                        {
                            Debug.Log("Game over");
                            SceneManager.LoadScene(0);
                        }
                    }
                }
            }
        }

    }

    bool IsGameOver()
    {
        GlobalAttributes.EmptyCells(board.boardCells);
        board.FillCells();
        if (GlobalAttributes.IsBoardFilled(board.boardCells))
            return true;
        return false;
    }
}