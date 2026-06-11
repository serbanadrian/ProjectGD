public class StartCircuitCommand : IBoardCommand
{
    private ArduinoBoardManager board;

    public StartCircuitCommand(ArduinoBoardManager board)
    {
        this.board = board;
    }

    public void Execute()
    {
        board.StartCircuit();
    }
}