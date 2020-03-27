using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garbochess.NET
{
    public class ChessEngine
    {
        public int g_timeout = 40;

        private string[] pieceChars = new string[] { " ", "p", "n", "b", "r", "q", "k", " " };
        private string[] pieceCharsUp = new string[] { "", "", "N", "B", "R", "Q", "K", "" };
        public string GetFEN()
        {
            string result = "";

            for(int row = 0; row < 8; row++)
            {
                if (row != 0)
                    result += "/";

                int empty = 0;

                for(int col = 0; col < 8; col++)
                {
                    var piece = g_board[((row + 2) << 4) + col + 4];

                    if(piece == 0)
                    {
                        empty++;
                    }
                    else
                    {
                        if (empty != 0)
                            result += empty;

                        empty = 0;

                        var pieceChar = pieceChars[piece & 0x7];
                        result += ((piece & colorWhite) != 0) ? pieceChar.ToUpper() : pieceChar;
                    }
                }

                if(empty != 0)
                {
                    result += empty;
                }
            }

            result += g_toMove == colorWhite ? " w" : " b";
            result += " ";

            if (g_castleRights == 0)
            {
                result += "-";
            }
            else
            {
                if ((g_castleRights & 1) != 0)
                    result += "K";
                if ((g_castleRights & 2) != 0)
                    result += "Q";
                if ((g_castleRights & 4) != 0)
                    result += "k";
                if ((g_castleRights & 8) != 0)
                    result += "q";
            }

            result += " ";

            if(g_enPassentSquare == -1)
            {
                result += "-";
            }
            else
            {
                result += FormatSquare(g_enPassentSquare);
            }

            return result;
        }

        public string GetMoveSAN(int move, int[] validMoves)
        {
            var from = move & 0xFF;
            var to = (move >> 8) & 0xFF;

            if (move & moveflagCastleKing) return "O-O";
            if (move & moveflagCastleQueen) return "O-O-O";

            var pieceType = g_board[from] & 0x7;
            var result = pieceCharsUp[pieceType];

            bool dupe = false, rowDiff = true, colDiff = true;

            if(validMoves == null)
            {
                validMoves = GenerateValidMoves();
            }

            for(int i = 0; i < validMoves.Length; i++)
            {
                var moveFrom = validMoves[i] & 0xFF;
                var moveTo = (validMoves[i] >> 8) & 0xFF;

                if(moveFrom != from && moveTo == to && (g_board[moveFrom] & 0x7) == pieceType)
                {
                    dupe = true;

                    if((moveFrom & 0xF0) == (from & 0xF0))
                    {
                        rowDiff = false;
                    }

                    if((moveFrom & 0x0F) == (from & 0x0F))
                    {
                        colDiff = false;
                    }
                }
            }

            if(dupe)
            {
                if(colDiff)
                {
                    result += FormatSquare(from)[0];
                }
                else if (rowDiff)
                {
                    result += FormatSquare(from)[1];
                }
                else
                {
                    result += FormatSquare(from);
                }
            }
            else if (pieceType == piecePawn && (g_board[to] != 0 || (move & moveflagEPC)))
            {

            }
        }

    }
}
