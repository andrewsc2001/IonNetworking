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

        private int x = 0;
        private int y = 0;
        private Role role;
        public State state;
        public Board.Team team;

        public Piece(Board.Team team, Role role, int x, int y)
        {
            this.team = team;
            this.role = role;
            this.x = x;
            this.y = y;
            this.state = State.Alive;
        }

        

        //Move piece to location
        public void MoveTo(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        //returns x
        public int GetX()
        {
            return x;
        }

        //returns y
        public int GetY()
        {
            return y;
        }

        //returns type
        public Role GetRole()
        {
            return role;
        }
    }
}
