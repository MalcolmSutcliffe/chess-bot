using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PawnPromotionSelector : MonoBehaviour
{
    public Sprite whiteQueenSprite;
    public Sprite whiteKnightSprite;
    public Sprite whiteRookSprite;
    public Sprite whiteBishopSprite;

    public Sprite blackQueenSprite;
    public Sprite blackKnightSprite;
    public Sprite blackRookSprite;
    public Sprite blackBishopSprite;

    public Button queenButton;
    public Button knightButton;
    public Button rookButton;
    public Button bishopButton;

    public PieceType selectedPiece;

    public void Awake()
    {
        Button qBtn = queenButton.GetComponent<Button>();
		qBtn.onClick.AddListener(SelectQueen);

        Button nBtn = knightButton.GetComponent<Button>();
		nBtn.onClick.AddListener(SelectKnight);

        Button rBtn = rookButton.GetComponent<Button>();
		rBtn.onClick.AddListener(SelectRook);

        Button bBtn = bishopButton.GetComponent<Button>();
		bBtn.onClick.AddListener(SelectBishop);

        SetWhiteSprites();
    }

    private void SetWhiteSprites()
    {
        queenButton.GetComponent<Image>().sprite = whiteQueenSprite;
        knightButton.GetComponent<Image>().sprite = whiteKnightSprite;
        rookButton.GetComponent<Image>().sprite = whiteRookSprite;
        bishopButton.GetComponent<Image>().sprite = whiteBishopSprite;
    }

    private void SetBlackSprites()
    {
        queenButton.GetComponent<Image>().sprite = blackQueenSprite;
        knightButton.GetComponent<Image>().sprite = blackKnightSprite;
        rookButton.GetComponent<Image>().sprite = blackRookSprite;
        bishopButton.GetComponent<Image>().sprite = blackBishopSprite;
    }
    
    public void CheckPromotion(int[] fromPos, int[] toPos, bool isPromotionCapture, PlayerType playerType)
    {   
        if (playerType == PlayerType.White)
        {
            SetWhiteSprites();
        }
        if (playerType == PlayerType.Black)
        {
            SetBlackSprites();
        }
        if (selectedPiece == PieceType.Pawn)
        {
            return;
        }
        PieceType pieceToSend = selectedPiece;
        selectedPiece = PieceType.Pawn;
        EventManager.instance.OnPlayerMoveOccured(new Move(PieceType.Pawn, fromPos, toPos, isPromotionCapture, true, pieceToSend), playerType);
        EventManager.instance.OnPawnPromotionEnded();
    }

    private void SelectQueen()
    {
        selectedPiece = PieceType.Queen;
    }
    private void SelectKnight()
    {
        selectedPiece = PieceType.Knight;
    }
    private void SelectRook()
    {
        selectedPiece = PieceType.Rook;
    }
    private void SelectBishop()
    {
        selectedPiece = PieceType.Bishop;
    }

}