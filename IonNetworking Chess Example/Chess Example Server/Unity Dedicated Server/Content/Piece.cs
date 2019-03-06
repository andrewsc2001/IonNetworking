namespace Unity_Dedicated_Server.Content
{
    public class Piece
    {
        public enum Role
        {
            King = 0,
            Queen = 1,
            Bishop = 2,
            Knight = 3,
            Rook = 4,
            Pawn = 5
        }

        public enum State
        {
            Dead = 0,
            Alive = 1
        }

        public int x;
        public int y;
        public Role role;
        public State state;
        public Board.Team team;

        public Piece(Board.Team team, Role role, int x, int y)
        {
            this.team = team;
            this.role = role;
            this.state = State.Alive;

            MoveTo(x, y);
        }

        //Move piece to location
        public void MoveTo(int x, int y)
        {
            this.x = x;
            this.y = y;
            Board.piecesArray[x][y] = this;
        }
    }
}
