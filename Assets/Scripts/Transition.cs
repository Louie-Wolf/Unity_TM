public struct Transition
{
    public Transition(int state, string write, bool moveRight)
    {
        State = state;
        Write = write;
        MoveRight = moveRight;
    }

    public int State { get; }
    public string Write { get; }
    public bool MoveRight { get; }
}
