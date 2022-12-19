using AITesting;

namespace AITesting
{
    internal static class Program
    {
        static void Main()
        {
            short[,] state1 = new short[,] { {  -50,  -30,  -32,  -90, -900,  -32,  -30,  -50 },
                                             {  -10,  -10,  -10,  -10,  -10,  -10,  -10,  -10 },
                                             {    0,    0,    0,    0,    0,    0,    0,    0 },
                                             {    0,    0,    0,    0,    0,    0,    0,    0 },
                                             {    0,    0,    0,    0,    0,    0,    0,    0 },
                                             {    0,    0,    0,    0,    0,    0,    0,    0 },
                                             {   10,   10,   10,   10,   10,   10,   10,   10 },
                                             {   50,   30,   32,   90,  900,   32,   30,   50 } };

            short[,] state2 = new short[,] { {  -50,    0,    0,  -90, -900,  -32,  -30,  -50 },
                                             {  -10,  -10,    0,    0,    0,  -10,  -10,  -10 },
                                             {    0,    0,  -30,    0,  -10,    0,    0,    0 },
                                             {    0,   32,    0,  -10,   10,    0,    0,    0 },
                                             {    0,    0,    0,  -10,    0,    0,  -32,    0 },
                                             {    0,    0,    0,    0,    0,   30,    0,    0 },
                                             {   10,   10,   10,   30,    0,   10,   10,   10 },
                                             {   50,    0,   32,   90,    0,   50,  900,    0 } };

            short[,] state3 = new short[,] { {    0,  -50,    0,    0,    0,    0, -900,    0 },
                                             {  -10,    0,    0,    0,    0,  -10,    0,  -10 },
                                             {    0,    0,    0,  -32,  -10,  -32,  -10,   30 },
                                             {    0,    0,    0,  -10,    0,    0,    0,    0 },
                                             {    0,    0,    0,   10,    0,    0,    0,    0 },
                                             {    0,    0,    0,    0,   90,   10,    0,   10 },
                                             {   10,    0,    0,   30,  900,   10,    0,    0 },
                                             {    0,    0,    0,    0,    0,    0,  -90,    0 } };

            short[,] state4 = new short[,] { {    0,    0,    0,    0,    0,    0,    0,    0 },
                                             {  -10,  -10,  -10, -900,    0,    0,    0,    0 },
                                             {    0,    0,    0,    0,    0,    0,   50,    0 },
                                             {    0,    0,    0,    0,    0,    0,    0,    0 },
                                             {    0,    0,    0,    0,   50,    0,  -10,    0 },
                                             {    0,    0,    0,    0,    0,    0,    0,    0 },
                                             {   10,   10,   10,    0,    0,    0,   10,   10 },
                                             {    0,    0,  900,    0,    0,    0,    0,    0 } };

            BoardEvaluator board = new BoardEvaluator();

            Console.WriteLine(board.EvaluateConfiguration(state1));
            Console.WriteLine(board.EvaluateConfiguration(state2));
            Console.WriteLine(board.EvaluateConfiguration(state3));
            Console.WriteLine(board.EvaluateConfiguration(state4));
            Console.WriteLine(board.EvaluateKingThreat(0, 4, state1, 1));
        }
    }
}