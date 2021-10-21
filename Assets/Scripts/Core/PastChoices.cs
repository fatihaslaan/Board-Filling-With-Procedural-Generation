public class PastChoices
{
    int id = 0;
    int[] location = new int[2] { 0, 0 };
    int rotation = 0;

    public PastChoices(PieceData p)
    {
        id = p.id;
        location = new int[2] { p.location[0], p.location[1] };
        rotation = p.rotation;
    }

    public bool IsTriedAlready(PieceData p) //Check if we are trying to locate same piece to a location that we already tried
    {
        if (id == p.id)
            if (location[0] == p.location[0] && location[1] == p.location[1])
                if (rotation == p.rotation)
                    return true;
        return false;
    }
}