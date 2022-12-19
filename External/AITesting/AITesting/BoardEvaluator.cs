using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AITesting
{
    internal class BoardEvaluator
    {
        private const float POSITION_WEIGHT = 1.0f;

        private float[,] pawnPositionWeights = new float[,] { {  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
                                                              {  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f,  5.0f },
                                                              {  1.0f,  1.0f,  2.0f,  3.0f,  3.0f,  2.0f,  1.0f,  1.0f },
                                                              {  0.5f,  0.5f,  1.0f,  2.5f,  2.5f,  1.0f,  0.5f,  0.5f },
                                                              {  0.0f,  0.0f,  0.0f,  2.0f,  2.0f,  0.0f,  0.0f,  0.0f },
                                                              {  0.5f, -0.5f, -1.0f,  0.0f,  0.0f, -1.0f, -0.5f,  0.5f },
                                                              {  0.5f,  1.0f,  1.0f, -2.0f, -2.0f,  1.0f,  1.0f,  0.5f },
                                                              {  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f } };

        private float[,] knightPositionWeights = new float[,] { { -5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f },
                                                                { -4.0f, -2.0f,  0.0f,  0.0f,  0.0f,  0.0f, -2.0f, -4.0f },
                                                                { -3.0f,  0.0f,  1.0f,  1.5f,  1.5f,  1.0f,  0.0f, -3.0f },
                                                                { -3.0f,  0.5f,  1.5f,  2.0f,  2.0f,  1.5f,  0.5f, -3.0f },
                                                                { -3.0f,  0.0f,  1.5f,  2.0f,  2.0f,  1.5f,  0.0f, -3.0f },
                                                                { -3.0f,  0.5f,  1.0f,  1.5f,  1.5f,  1.0f,  0.5f, -3.0f },
                                                                { -4.0f, -2.0f,  0.0f,  0.5f,  0.5f,  0.0f, -2.0f, -4.0f },
                                                                { -5.0f, -4.0f, -3.0f, -3.0f, -3.0f, -3.0f, -4.0f, -5.0f } };

        private float[,] bishopPositionWeights = new float[,] { { -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f },
                                                                { -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f },
                                                                { -1.0f,  0.0f,  0.5f,  1.0f,  1.0f,  0.5f,  0.0f, -1.0f },
                                                                { -1.0f,  0.5f,  0.5f,  1.0f,  1.0f,  0.5f,  0.5f, -1.0f },
                                                                { -1.0f,  0.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.0f, -1.0f },
                                                                { -1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f, -1.0f },
                                                                { -1.0f,  0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.5f, -1.0f },
                                                                { -2.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -2.0f } };

        private float[,] rookPositionWeights = new float[,] { {  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f },
                                                              {  0.5f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  0.5f },
                                                              { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f },
                                                              { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f }, 
                                                              { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f },
                                                              { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f },
                                                              { -0.5f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -0.5f },
                                                              {  0.0f,  0.0f,  0.0f,  0.5f,  0.5f,  0.0f,  0.0f,  0.0f } };

        private float[,] queenPositionWeights = new float[,] { { -2.0f, -1.0f, -1.0f, -0.5f, -0.5f, -1.0f, -1.0f, -2.0f },
                                                               { -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f },
                                                               { -1.0f,  0.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -1.0f },
                                                               { -0.5f,  0.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -0.5f },
                                                               {  0.0f,  0.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -0.5f },
                                                               { -1.0f,  0.5f,  0.5f,  0.5f,  0.5f,  0.5f,  0.0f, -1.0f },
                                                               { -1.0f,  0.0f,  0.5f,  0.0f,  0.0f,  0.0f,  0.0f, -1.0f },
                                                               { -2.0f, -1.0f, -1.0f, -0.5f, -0.5f, -1.0f, -1.0f, -2.0f } };

        private float[,] kingPositionWeights = new float[,] { { -3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f },
                                                              { -3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f },
                                                              { -3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f },
                                                              { -3.0f, -4.0f, -4.0f, -5.0f, -5.0f, -4.0f, -4.0f, -3.0f },
                                                              { -2.0f, -3.0f, -3.0f, -4.0f, -4.0f, -3.0f, -3.0f, -2.0f },
                                                              { -1.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f, -2.0f, -1.0f },
                                                              {  2.0f,  2.0f,  0.0f,  0.0f,  0.0f,  0.0f,  2.0f,  2.0f },
                                                              {  2.0f,  3.0f,  1.0f,  0.0f,  0.0f,  1.0f,  3.0f,  2.0f } };

        public BoardEvaluator() 
        {

        }

        public int EvaluateConfiguration(short[,] configuration)
        {
            if (!CheckSize(configuration))
            {
                throw new ArgumentException();
            }

            int sumValue = 0;

            sumValue += EvaluateMaterial(configuration);
            sumValue += EvaluatePositions(configuration);

            return sumValue;
        }


        private bool CheckSize(short[,] configuration)
        {
            return (configuration.GetLength(0) == 8 && configuration.GetLength(1) == 8);
        }


        private int EvaluateMaterial(short[,] configuration)
        {
            int sum = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    sum += configuration[i, j];
                }
            }

            return sum;
        }


        private int EvaluatePositions(short[,] configuration)
        {
            int sum = 0;

            sum += EvaluateWhitePositions(configuration);
            sum += EvaluateBlackPositions(configuration);

            return sum;
        }

        private int EvaluateWhitePositions(short[,] configuration)
        {
            float sum = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (configuration[i, j])
                    {
                        case 10:
                            sum += configuration[i, j] + pawnPositionWeights[i, j];
                            break;
                        case 30:
                            sum += configuration[i, j] + knightPositionWeights[i, j];
                            break;
                        case 32:
                            sum += configuration[i, j] + bishopPositionWeights[i, j];
                            break;
                        case 50:
                            sum += configuration[i, j] + rookPositionWeights[i, j];
                            break;
                        case 90:
                            sum += configuration[i, j] + queenPositionWeights[i, j];
                            break;
                        case 900:
                            sum += configuration[i, j] + kingPositionWeights[i, j];
                            break;
                        default:
                            sum += 0;
                            break;
                    }
                }
            }

            sum *= POSITION_WEIGHT;

            return (int)sum;
        }


        private int EvaluateBlackPositions(short[,] configuration)
        {
            float sum = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (configuration[i, j])
                    {
                        case -10:
                            sum += configuration[i, j] + -pawnPositionWeights[7 - i, j];
                            break;
                        case -30:
                            sum += configuration[i, j] + -knightPositionWeights[7 - i, j];
                            break;
                        case -32:
                            sum += configuration[i, j] + -bishopPositionWeights[7 - i, j];
                            break;
                        case -50:
                            sum += configuration[i, j] + -rookPositionWeights[7 - i, j];
                            break;
                        case -90:
                            sum += configuration[i, j] + -queenPositionWeights[7 - i, j];
                            break;
                        case -900:
                            sum += configuration[i, j] + -kingPositionWeights[7 - i, j];
                            break;
                        default:
                            sum += 0;
                            break;
                    }
                }
            }

            sum *= POSITION_WEIGHT;

            return (int)sum;
        }
    }
}
