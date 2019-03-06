using System;
using System.Collections;

namespace Unity_Dedicated_Server.Content
{
    public static class Board
    {
        public enum Team
        {
            White = 0,
            Black = 1
        }

        //White pieces
        public static Piece wKing;
        public static Piece wQueen;
        public static Piece wBishop1;
        public static Piece wBishop2;
        public static Piece wKnight1;
        public static Piece wKnight2;
        public static Piece wRook1;
        public static Piece wRook2;
        public static Piece wPawn1;
        public static Piece wPawn2;
        public static Piece wPawn3;
        public static Piece wPawn4;
        public static Piece wPawn5;
        public static Piece wPawn6;
        public static Piece wPawn7;
        public static Piece wPawn8;

        //Black pieces
        public static Piece bKing;
        public static Piece bQueen;
        public static Piece bBishop1;
        public static Piece bBishop2;
        public static Piece bKnight1;
        public static Piece bKnight2;
        public static Piece bRook1;
        public static Piece bRook2;
        public static Piece bPawn1;
        public static Piece bPawn2;
        public static Piece bPawn3;
        public static Piece bPawn4;
        public static Piece bPawn5;
        public static Piece bPawn6;
        public static Piece bPawn7;
        public static Piece bPawn8;

        //IDs
        private static Hashtable NetIDs = new Hashtable();

        public static void Set()
        {
            Console.WriteLine("Setting board");

            //Instantiate pieces.
            wKing = new Piece(Piece.Role.King, 3, 0);
            wQueen = new Piece(Piece.Role.Queen, 4, 0);
            wBishop1 = new Piece(Piece.Role.Bishop, 2, 0);
            wBishop2 = new Piece(Piece.Role.Bishop, 5, 0);
            wKnight1 = new Piece(Piece.Role.Knight, 1, 0);
            wKnight2 = new Piece(Piece.Role.Knight, 6, 0);
            wRook1 = new Piece(Piece.Role.Rook, 0, 0);
            wRook2 = new Piece(Piece.Role.Rook, 7, 0);
            wPawn1 = new Piece(Piece.Role.Pawn, 0, 1);
            wPawn2 = new Piece(Piece.Role.Pawn, 1, 1);
            wPawn3 = new Piece(Piece.Role.Pawn, 2, 1);
            wPawn4 = new Piece(Piece.Role.Pawn, 3, 1);
            wPawn5 = new Piece(Piece.Role.Pawn, 4, 1);
            wPawn6 = new Piece(Piece.Role.Pawn, 5, 1);
            wPawn7 = new Piece(Piece.Role.Pawn, 6, 1);
            wPawn8 = new Piece(Piece.Role.Pawn, 7, 1);


            bKing = new Piece(Piece.Role.King, 3, 7);
            bQueen = new Piece(Piece.Role.Queen, 4, 7);
            bBishop1 = new Piece(Piece.Role.Bishop, 2, 7);
            bBishop2 = new Piece(Piece.Role.Bishop, 5, 7);
            bKnight1 = new Piece(Piece.Role.Knight, 1, 7);
            bKnight2 = new Piece(Piece.Role.Knight, 6, 7);
            bRook1 = new Piece(Piece.Role.Rook, 0, 7);
            bRook2 = new Piece(Piece.Role.Rook, 7, 7);
            bPawn1 = new Piece(Piece.Role.Pawn, 0, 6);
            bPawn2 = new Piece(Piece.Role.Pawn, 1, 6);
            bPawn3 = new Piece(Piece.Role.Pawn, 2, 6);
            bPawn4 = new Piece(Piece.Role.Pawn, 3, 6);
            bPawn5 = new Piece(Piece.Role.Pawn, 4, 6);
            bPawn6 = new Piece(Piece.Role.Pawn, 5, 6);
            bPawn7 = new Piece(Piece.Role.Pawn, 6, 6);
            bPawn8 = new Piece(Piece.Role.Pawn, 7, 6);


            //Register pieces with NetIDs.
            AddPiece(wKing);
            AddPiece(wQueen);
            AddPiece(wBishop1);
            AddPiece(wBishop2);
            AddPiece(wKnight1);
            AddPiece(wKnight2);
            AddPiece(wRook1);
            AddPiece(wRook2);
            AddPiece(wPawn1);
            AddPiece(wPawn2);
            AddPiece(wPawn3);
            AddPiece(wPawn4);
            AddPiece(wPawn5);
            AddPiece(wPawn6);
            AddPiece(wPawn7);
            AddPiece(wPawn8);

            AddPiece(bKing);
            AddPiece(bQueen);
            AddPiece(bBishop1);
            AddPiece(bBishop2);
            AddPiece(bKnight1);
            AddPiece(bKnight2);
            AddPiece(bRook1);
            AddPiece(bRook2);
            AddPiece(bPawn1);
            AddPiece(bPawn2);
            AddPiece(bPawn3);
            AddPiece(bPawn4);
            AddPiece(bPawn5);
            AddPiece(bPawn6);
            AddPiece(bPawn7);
            AddPiece(bPawn8);
        }

        public static void Clear()
        {
            Console.WriteLine("Setting board");

            NetIDs.Clear();

            wKing = null;
            wQueen = null;
            wBishop1 = null;
            wBishop2 = null;
            wKnight1 = null;
            wKnight2 = null;
            wRook1 = null;
            wRook2 = null;

            wPawn1 = null;
            wPawn2 = null;
            wPawn3 = null;
            wPawn4 = null;
            wPawn5 = null;
            wPawn6 = null;
            wPawn7 = null;
            wPawn8 = null;


            bKing = null;
            bQueen = null;
            bBishop1 = null;
            bBishop2 = null;
            bKnight1 = null;
            bKnight2 = null;
            bRook1 = null;
            bRook2 = null;

            bPawn1 = null;
            bPawn2 = null;
            bPawn3 = null;
            bPawn4 = null;
            bPawn5 = null;
            bPawn6 = null;
            bPawn7 = null;
            bPawn8 = null;
        }

        //returns a piece by location, null if nothing there.
        public static Piece GetPieceFromLocation(int x, int y)
        {
            foreach(Piece piece in NetIDs)
            {
                if (piece.GetX() == x && piece.GetY() == y)
                    return piece;
            }
            return null;
        }

        //returns a piece by NetID
        public static Piece GetPieceFromID(int ID)
        {
            return (Piece)NetIDs[ID];
        }

        //Adds a piece to the board
        private static void AddPiece(Piece piece)
        {
            int ID = GetEmptyID();

            NetIDs.Add(ID, piece);
        }

        //Returns the next unused NetID up to 50. (50 should never be reached)
        private static int GetEmptyID()
        {
            int maxID = 50;
            for(int id = 0; id < maxID; id++)
            {
                if (NetIDs[id] != null)
                    return id;
            }
            return -1;
        }
    }
}
