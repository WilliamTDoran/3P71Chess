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
using System.Xml.Xsl;

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

            if (EvaluateCheckmate(configuration, 1))
            {
                sumValue += -10000.0f;
            }
            if (EvaluateCheckmate(configuration, -1))
            {
                sumValue += 10000.0f;
            }

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

            List<int[]> attackers;
            if (!ThreatEvaluator.EvaluateThreatened(kingX, kingY, configuration, defendingColor, true, true, out attackers))
            {
                return false;
            }
            if (attackers.Count == 1)
            {
                if (ThreatEvaluator.EvaluateThreatened(attackers[0][0], attackers[0][1], configuration, -defendingColor, true, true))
                {
                    return false;
                }

                int attackerX = attackers[0][0];
                int attackerY = attackers[0][1];

                if (configuration[attackerY, attackerX] * -defendingColor == 32 || configuration[attackerY, attackerX] * -defendingColor == 50 || configuration[attackerY, attackerX] * -defendingColor == 90)
                {
                    List<int[]> intermediateSpaces;
                    FindIntermediateSpaces(kingX, kingY, attackerX, attackerY, configuration, defendingColor, out intermediateSpaces);

                    for (int i = 0; i < intermediateSpaces.Count; i++)
                    {
                        if (ThreatEvaluator.EvaluateThreatened(intermediateSpaces[i][0], intermediateSpaces[i][1], configuration, -defendingColor, false, false))
                        {
                            return false;
                        }

                        if (defendingColor < 0) //this is super gross
                        {
                            if (intermediateSpaces[i][1] == 3 || intermediateSpaces[i][1] == 2)
                            {
                                if (configuration[1, intermediateSpaces[i][0]] == -10)
                                {
                                    return false;
                                }
                            }
                        }
                        if (defendingColor > 0)
                        {
                            if (intermediateSpaces[i][1] == 4 || intermediateSpaces[i][1] == 5)
                            {
                                if (configuration[6, intermediateSpaces[i][0]] == 10)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                int xDisplacement = ((i + 2) % 3) - 1;
                int yDisplacement = (((i / 3) + 2) % 3) - 1;

                int xOut;
                int yOut;

                if (ThreatEvaluator.GetInboundsOffset(kingX, kingY, xDisplacement, yDisplacement, out xOut, out yOut))
                {
                    if (!(ThreatEvaluator.EvaluateThreatened(xOut, yOut, configuration, defendingColor, true, true)))
                    {
                        if (configuration[yOut, xOut] <= defendingColor)
                        {
                            return false;
                        }
                    }
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


        private void FindIntermediateSpaces(int defenderX, int defenderY, int attackerX, int attackerY, short[,] configuration, int defendingColor, out List<int[]> spaces)
        {
            spaces = new List<int[]>();

            if (defenderX == attackerX)
            {
                FindVerticalSpaces(defenderX, defenderY, attackerY, configuration, defendingColor, out spaces);
            }
            else if (defenderY == attackerY)
            {
                FindHorizontalSpaces(defenderY, defenderX, attackerX, configuration, defendingColor, out spaces);
            }
            else
            {
                FindDiagonalSpaces(defenderX, defenderY, attackerX, attackerY, configuration, defendingColor, out spaces);
            }
        }


        private void FindVerticalSpaces(int x, int defenderY, int attackerY, short[,] configuration, int defendingColor, out List<int[]> spaces)
        {
            spaces = new List<int[]>();

            if (defenderY > attackerY)
            {
                for (int i = defenderY - 1; i > attackerY; i--)
                {
                    spaces.Add(new int[] { x, i });
                }
            }

            if (defenderY < attackerY)
            {
                for (int i = defenderY + 1; i < attackerY; i++)
                {
                    spaces.Add(new int[] { x, i });
                }
            }
        }


        private void FindHorizontalSpaces(int y, int defenderX, int attackerX, short[,] configuration, int defendingColor, out List<int[]> spaces)
        {
            spaces = new List<int[]>();

            if (defenderX > attackerX)
            {
                for (int i = defenderX - 1; i > attackerX; i--)
                {
                    spaces.Add(new int[] { i, y });
                }
            }

            if (defenderX < attackerX)
            {
                for (int i = defenderX + 1; i < attackerX; i++)
                {
                    spaces.Add(new int[] { i, y });
                }
            }
        }


        private void FindDiagonalSpaces(int defenderX, int defenderY, int attackerX, int attackerY, short[,] configuration, int defendingColor, out List<int[]> spaces)
        {
            spaces = new List<int[]>();

            int xDisplacement = defenderX < attackerX ? 1 : -1;
            int yDisplacement = defenderY < attackerY ? 1 : -1;
            int numSpaces = Math.Abs(defenderX - attackerX) - 1;

            for (int i = 1; i <= numSpaces; i++)
            {
                int xOut;
                int yOut;
                ThreatEvaluator.GetInboundsOffset(defenderX, defenderY, i * xDisplacement, i * yDisplacement, out xOut, out yOut);
                spaces.Add(new int[] { xOut, yOut });
            }
        }
    }
}
