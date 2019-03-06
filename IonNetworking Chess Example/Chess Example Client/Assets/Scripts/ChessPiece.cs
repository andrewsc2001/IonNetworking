using UnityEngine;
using Assets.Scripts;

public class ChessPiece : MonoBehaviour
{
    public enum Type
    {
        King = 0,
        Queen = 1,
        Bishop = 2,
        Knight = 3,
        Rook = 4,
        Pawn = 5
    }

    private static readonly Vector2 offset = new Vector2(0.5f, 0.5f);

    public GameObject target;
    public GlobalVariables.Team owner;
    public Type type;

    private void Start()
    {
        if (target == null) //If there is no target selected, use parent.
            target = gameObject;
    }

    public void MoveTo(int x, int z)
    {
        if (x < 0 || x > 7 || z < 0 || z > 7) //If its not a position on the board, return
            return;

        target.transform.localPosition = new Vector3(x + offset.x, target.transform.localPosition.y, z + offset.y);
    }

    private bool isMoveLegal(int moveX, int moveZ)
    {
        int currentX = (int)(target.transform.localPosition.x - offset.x);
        int currentZ = (int)(target.transform.localPosition.y - offset.y);

        int deltaX = moveX - currentX;
        int deltaZ = moveZ - currentZ;

        switch (type)
        {
            case Type.King:
                if (deltaX >= -1 && deltaX <= 1 && deltaZ >= -1 && deltaZ <= -1)
                    return true;
                return false;
            case Type.Queen:
                if (deltaX == deltaZ || deltaX == -deltaZ || deltaX == 0 || deltaZ == 0)
                    return true;
                return false;
            case Type.Bishop:
                if (deltaX == deltaZ || deltaX == -deltaZ)
                    return true;
                return false;
            case Type.Knight:
                if (deltaX == 2 || deltaX == -2)
                    if (deltaZ == 1 || deltaZ == -1)
                        return true;
                if (deltaZ == 2 || deltaZ == -2)
                    if (deltaX == 1 || deltaX == -1)
                        return true;

                return false;
            case Type.Rook:
                if (deltaX == 0 || deltaZ == 0)
                    return true;
                break;
            case Type.Pawn:
                if (owner == GlobalVariables.Team.White)
                    if (deltaX == 0 && deltaZ == 1)
                        return true;
                if (owner == GlobalVariables.Team.Black)
                    if (deltaX == 0 && deltaZ == -1)
                        return true;
                return false;
        }

        return false;
    }
}
