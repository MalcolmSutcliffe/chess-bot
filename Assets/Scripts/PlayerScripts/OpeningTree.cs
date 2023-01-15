using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OpeningTree
{
    private static string FILE = "Assets\\Data\\chess_openings.csv";
    public static List<Move> GetOpeningMove(ChessState chessState)
    {
        List<Move> nextMoves = new List<Move>();
        
        if (chessState.fullMoveCount > 12)
        {
            return nextMoves;
        }
        // get current opening moves
        string currentOpening = chessState.gameMoves;

        
        // check reference in database
        using (StreamReader sr = new StreamReader(FILE))
        {
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                string opening = parts[2];
                
                if (opening.Contains(currentOpening))
                {
                    // remove current opening
                    string nextMove = opening.Replace(currentOpening, "");
                    
                    // remove move numbering
                    if ('1' <= nextMove[0] && nextMove[0] <= 9)
                    {
                        nextMove = nextMove.Remove(0, nextMove.IndexOf('.')+1);
                    }

                    if (nextMove.Contains(' '))
                        nextMove = nextMove.Substring(0, nextMove.IndexOf(' '));
                    
                    nextMoves.Add(Move.DecodeMoveSAN(nextMove, chessState));
                }
            }
        }

        return nextMoves;
    }
}