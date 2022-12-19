using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AITesting
{
    internal class BoardEvaluator
    {
        private const float POSITION_WEIGHT = 0.2f;

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

        public float EvaluateConfiguration(short[,] configuration)
        {
            if (!CheckSize(configuration))
            {
                throw new ArgumentException();
            }

            float sumValue = 0;

            sumValue += EvaluateMaterial(configuration);
            sumValue += EvaluatePositions(configuration);

            return sumValue;
        }


        private bool CheckSize(short[,] configuration)
        {
            return (configuration.GetLength(0) == 8 && configuration.GetLength(1) == 8);
        }


        private float EvaluateMaterial(short[,] configuration)
        {
            float sum = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    sum += configuration[i, j];
                }
            }

            return sum;
        }


        private float EvaluatePositions(short[,] configuration)
        {
            float sum = 0;

            sum += EvaluateWhitePositions(configuration);
            sum += EvaluateBlackPositions(configuration);

            return sum * POSITION_WEIGHT;
        }


        private float EvaluateWhitePositions(short[,] configuration)
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

            return sum;
        }


        private float EvaluateBlackPositions(short[,] configuration)
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

            return sum;
        }


        public bool EvaluateCheckmate(short[,] configuration, int defendingColor)
        {
            int kingX = 0;
            int kingY = 0;
            FindKing(out kingX, out kingY, configuration, defendingColor);

            if (!EvaluateThreatened(kingX, kingY, configuration, defendingColor))
            {
                return false;
            }

            int validSquares = 0;
            for (int i = 0; i < 8; i++)
            {
                int xDisplacement = ((i + 2) % 3) - 1;
                int yDisplacement = (((i / 3) + 2) %3) - 1;
                int xOut;
                int yOut;
                if (GetInboundsOffset(kingX, kingY, xDisplacement, yDisplacement, out xOut, out yOut))
                {
                    validSquares++;
                }
            }

            return true;
        }


        private void FindKing(out int kingX, out int kingY, short[,] configuration, int defendingColor)
        {
            kingX = 0;
            kingY = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (configuration[i, j] == defendingColor * 900)
                    {
                        kingX = j;
                        kingY = i;
                    }
                }
            }
        }


        public bool EvaluateThreatened(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;

            threatened = threatened || EvaluatePawnThreat(x, y, configuration, defendingColor);
            threatened = threatened || EvaluateKnightThreat(x, y, configuration, defendingColor);
            threatened = threatened || EvaluateDiagonalThreat(x, y, configuration, defendingColor);
            threatened = threatened || EvaluateStraightThreat(x, y, configuration, defendingColor);
            threatened = threatened || EvaluateKingThreat(x, y, configuration, defendingColor);

            return threatened;
        }


        private bool EvaluatePawnThreat(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;

            if ((defendingColor < 0 && y >= 6) || (defendingColor > 1 && y <= 1)) //Pawns can't take the row they start in, or the one behind it
            {
                return false;
            }

            int leftSquare = x - 1;
            int rightSquare = x + 1;
            int rankY = y - defendingColor;

            if (leftSquare >= 0)
            {
                if (configuration[rankY, leftSquare] * -defendingColor == 10)
                {
                    threatened = true;
                }
            }

            if (rightSquare <= 7)
            {
                if (configuration[rankY, rightSquare] * -defendingColor == 10)
                {
                    threatened = true;
                }
            }

            return threatened;
        }


        /// <summary>
        /// It checks if a square is threatened by a knight
        /// </summary>
        /// <param name="x">The x coordinate of the square to check</param>
        /// <param name="y">The y coordinate of the square to check</param>
        /// <param name="configuration">The board state</param>
        /// <param name="defendingColor">1 for white, -1 for black</param>
        /// <returns>True if the square is threatened by a knight</returns>
        public bool EvaluateKnightThreat(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;

            threatened = threatened || CastL(x, y, configuration, defendingColor, 1, 2, 30);
            threatened = threatened || CastL(x, y, configuration, defendingColor, 2, 1, 30);
            threatened = threatened || CastL(x, y, configuration, defendingColor, 2, -1, 30);
            threatened = threatened || CastL(x, y, configuration, defendingColor, 1, -2, 30);
            threatened = threatened || CastL(x, y, configuration, defendingColor, -1, -2, 30);
            threatened = threatened || CastL(x, y, configuration, defendingColor, -2, -1, 30);
            threatened = threatened || CastL(x, y, configuration, defendingColor, -2, 1, 30);
            threatened = threatened || CastL(x, y, configuration, defendingColor, -1, 2, 30);

            return threatened;
        }


        private bool GetInboundsOffset(int x, int y, int xDisplacement, int yDisplacement, out int xFinal, out int yFinal)
        {
            xFinal = 0;
            yFinal = 0;

            xFinal = x + xDisplacement;
            yFinal = y + yDisplacement;

            if (xFinal < 0 || xFinal > 7)
            {
                xFinal = -1; yFinal = -1;
                return false;
            }

            if (yFinal < 0 || yFinal > 7)
            {
                xFinal= -1; yFinal = -1;
                return false;
            }

            return true;
        }


        private bool CastL(int x, int y, short[,] configuration, int defendingColor, int xDisplacement, int yDisplacement, int pieceCheck)
        {
            int checkX, checkY;
            if (GetInboundsOffset(x, y, xDisplacement, yDisplacement, out checkX, out checkY))
            {
                if (configuration[checkY, checkX] * -defendingColor == pieceCheck)
                {
                    return true;
                }
            }

            return false;
        }


        private bool EvaluateDiagonalThreat(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;

            threatened = threatened || CastDiagonal(x, y, configuration, defendingColor, -1, -1); //Top left
            threatened = threatened || CastDiagonal(x, y, configuration, defendingColor, 1, -1); //Top right
            threatened = threatened || CastDiagonal(x, y, configuration, defendingColor, 1, 1); //Bottom right
            threatened = threatened || CastDiagonal(x, y, configuration, defendingColor, -1, 1); //Bottom left

            return threatened;
        }

        /// <summary>
        /// Checks a diagonal line for queens and bishops
        /// </summary>
        /// <param name="x">x coordinate of target square</param>
        /// <param name="y">x coordinate of target square</param>
        /// <param name="configuration">board state</param>
        /// <param name="defendingColor">1 for white, -1 for black</param>
        /// <param name="xPolarity">1 or -1</param>
        /// <param name="yPolarity">1 or -1</param>
        /// <returns></returns>
        private bool CastDiagonal(int x, int y, short[,] configuration, int defendingColor, int xPolarity, int yPolarity)
        {
            for (int i = 1; i <= 8; i++)
            {
                int targetX = x + (xPolarity * i);
                int targetY = y + (yPolarity * i);
                if (targetX >= 0 && targetY >= 0 && targetX <= 7 && targetY <= 7)
                {
                    short targetSquareContents = configuration[targetY, targetX];
                    if (targetSquareContents == 0)
                    {
                        continue;
                    }
                    else if (targetSquareContents * -defendingColor == 32 || targetSquareContents * -defendingColor == 90)
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return false;
        }


        private bool EvaluateStraightThreat(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;

            threatened = threatened || CastVerticalLine(x, y, configuration, defendingColor, 1);
            threatened = threatened || CastVerticalLine(x, y, configuration, defendingColor, -1);
            threatened = threatened || CastHorizontalLine(x, y, configuration, defendingColor, 1);
            threatened = threatened || CastHorizontalLine(x, y, configuration, defendingColor, -1);

            return threatened;
        }


        /// <summary>
        /// Checks vertical lines for rooks or queens
        /// </summary>
        /// <param name="x">x coord of the target square</param>
        /// <param name="y">y coord of the target square</param>
        /// <param name="configuration">the board state</param>
        /// <param name="defendingColor">1 for white, -1 for black</param>
        /// <param name="direction">1 for down, -1 for up</param>
        /// <returns></returns>
        private bool CastVerticalLine(int x, int y, short[,] configuration, int defendingColor, int direction)
        {
            for (int i = 1; i <= 8; i++)
            {
                int targetSquare = y + (i * direction);
                if (targetSquare <= 7 && targetSquare >= 0)
                {
                    short targetSquareContents = configuration[targetSquare, x];
                    if (targetSquareContents == 0)
                    {
                        continue;
                    }
                    else if (targetSquareContents * -defendingColor == 50 || targetSquareContents * -defendingColor == 90)
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return false;
        }


        /// <summary>
        /// Checks horizontal lines for rooks or queens
        /// </summary>
        /// <param name="x">x coord of the target square</param>
        /// <param name="y">y coord of the target square</param>
        /// <param name="configuration">the board state</param>
        /// <param name="defendingColor">1 for white, -1 for black</param>
        /// <param name="direction">1 for right, -1 for left</param>
        /// <returns></returns>
        private bool CastHorizontalLine(int x, int y, short[,] configuration, int defendingColor, int direction)
        {
            for (int i = 1; i <= 8; i++)
            {
                int targetSquare = x + (i * direction);
                if (targetSquare <= 7 && targetSquare >= 0)
                {
                    short targetSquareContents = configuration[y, targetSquare];
                    if (targetSquareContents == 0)
                    {
                        continue;
                    }
                    else if (targetSquareContents * -defendingColor == 50 || targetSquareContents * -defendingColor == 90)
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return false;
        }


        public bool EvaluateKingThreat(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;

            for (int i = 0; i < 8; i++)
            {
                int xDisplacement = ((i + 2) % 3) - 1;
                int yDisplacement = (((i / 3) + 2) % 3) - 1;
                threatened = CastL(x, y, configuration, defendingColor, xDisplacement, yDisplacement, 900);
                if (threatened) { break; }
            }

            return threatened;
        }
    }
}
