using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITesting
{
    internal static class ThreatEvaluator
    {
        static ThreatEvaluator() { }

        public static bool EvaluateThreatened(int x, int y, short[,] configuration, int defendingColor, bool includeKing, bool includePawn)
        {
            bool threatened = false;

            if (includePawn)
            {
                threatened = threatened || EvaluatePawnThreat(x, y, configuration, defendingColor);
            }
            threatened = threatened || EvaluateKnightThreat(x, y, configuration, defendingColor);
            threatened = threatened || EvaluateDiagonalThreat(x, y, configuration, defendingColor);
            threatened = threatened || EvaluateStraightThreat(x, y, configuration, defendingColor);
            if (includeKing)
            {
                threatened = threatened || EvaluateKingThreat(x, y, configuration, defendingColor);
            }

            return threatened;
        }

        public static bool EvaluateThreatened(int x, int y, short[,] configuration, int defendingColor, bool includeKing, bool includePawn, out List<int[]> attackers)
        {
            attackers = new List<int[]>();

            List<int[]> temp = new List<int[]>();
            if (includePawn)
            {
                EvaluatePawnThreat(x, y, configuration, defendingColor, out temp);
                if (temp.Count > 0)
                {
                    attackers.AddRange(temp);
                }
            }
            EvaluateKnightThreat(x, y, configuration, defendingColor, out temp);
            if (temp.Count > 0)
            {
                attackers.AddRange(temp);
            }
            EvaluateDiagonalThreat(x, y, configuration, defendingColor, out temp);
            if (temp.Count > 0)
            {
                attackers.AddRange(temp);
            }
            EvaluateStraightThreat(x, y, configuration, defendingColor, out temp);
            if (temp.Count > 0)
            {
                attackers.AddRange(temp);
            }
            if (includeKing)
            {
                int[] kingTemp;
                EvaluateKingThreat(x, y, configuration, defendingColor, out kingTemp);
                if (kingTemp.Length > 0)
                {
                    attackers.Add(kingTemp);
                }
            }

            return (attackers.Count > 0);
        }


        public static bool EvaluatePawnThreat(int x, int y, short[,] configuration, int defendingColor)
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

        public static bool EvaluatePawnThreat(int x, int y, short[,] configuration, int defendingColor, out List<int[]> attackers)
        {
            attackers = new List<int[]>();
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
                    attackers.Add(new int[] { leftSquare, rankY });
                }
            }

            if (rightSquare <= 7)
            {
                if (configuration[rankY, rightSquare] * -defendingColor == 10)
                {
                    threatened = true;
                    attackers.Add(new int[] { rightSquare, rankY });
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
        public static bool EvaluateKnightThreat(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;

            int[] xDisses = new int[] { 1, 2, 2, 1, -1, -2, -2, -1 }; //This is a clumsy stupid way to achieve this, but I couldn't figure out the right way to get it with modulo, so
            int[] yDisses = new int[] { -2, -1, 1, 2, 2, 1, -1, -2 };

            for (int i = 0; i < 8; i++)
            {
                threatened = threatened || CastL(x, y, configuration, defendingColor, xDisses[i], yDisses[i], 30);
            }

            return threatened;
        }

        public static bool EvaluateKnightThreat(int x, int y, short[,] configuration, int defendingColor, out List<int[]> attackers)
        {
            attackers = new List<int[]>();
            bool threatened = false;

            int[] xDisses = new int[] { 1, 2, 2, 1, -1, -2, -2, -1 }; //This is a clumsy stupid way to achieve this, but I couldn't figure out the right way to get it with modulo, so
            int[] yDisses = new int[] { -2, -1, 1, 2, 2, 1, -1, -2 };

            for (int i = 0; i < 8; i++)
            {
                int[] attacker;
                bool attacked = CastL(x, y, configuration, defendingColor, xDisses[i], yDisses[i], 30, out attacker);
                if (attacked)
                {
                    threatened = attacked;
                    if (attacker.Length > 0)
                    {
                        attackers.Add(attacker);
                    }
                }
            }

            return threatened;
        }


        public static bool GetInboundsOffset(int x, int y, int xDisplacement, int yDisplacement, out int xFinal, out int yFinal)
        {
            xFinal = 0;
            yFinal = 0;

            xFinal = x + xDisplacement;
            yFinal = y + yDisplacement;

            if (xFinal < 0 || xFinal > 7)
            {
                xFinal = -1;
                yFinal = -1;
                return false;
            }

            if (yFinal < 0 || yFinal > 7)
            {
                xFinal = -1;
                yFinal = -1;
                return false;
            }

            return true;
        }


        private static bool CastL(int x, int y, short[,] configuration, int defendingColor, int xDisplacement, int yDisplacement, int pieceCheck)
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

        private static bool CastL(int x, int y, short[,] configuration, int defendingColor, int xDisplacement, int yDisplacement, int pieceCheck, out int[] attacker)
        {
            int checkX, checkY;
            if (GetInboundsOffset(x, y, xDisplacement, yDisplacement, out checkX, out checkY))
            {
                if (configuration[checkY, checkX] * -defendingColor == pieceCheck)
                {
                    attacker = new int[] { checkX, checkY };
                    return true;
                }
            }

            attacker = new int[0];
            return false;
        }


        public static bool EvaluateDiagonalThreat(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;

            int[] xDisses = new int[] { 1, -1, 1, -1 };
            int[] yDisses = new int[] { 1, 1, -1, -1 };

            for (int i = 0; i < 4; i++)
            {
                threatened = threatened || CastDiagonal(x, y, configuration, defendingColor, xDisses[i], yDisses[i]);
            }

            return threatened;
        }

        public static bool EvaluateDiagonalThreat(int x, int y, short[,] configuration, int defendingColor, out List<int[]> attackers)
        {
            attackers = new List<int[]>();
            bool threatened = false;

            int[] xDisses = new int[] { 1, -1, 1, -1 };
            int[] yDisses = new int[] { 1, 1, -1, -1 };

            for (int i = 0; i < 4; i++)
            {
                int[] attacker;
                bool attacked = CastDiagonal(x, y, configuration, defendingColor, xDisses[i], yDisses[i], out attacker);
                if (attacked)
                {
                    threatened = attacked;
                    if (attacker.Length > 0)
                    {
                        attackers.Add(attacker);

                    }
                }
            }

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
        private static bool CastDiagonal(int x, int y, short[,] configuration, int defendingColor, int xPolarity, int yPolarity)
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

        private static bool CastDiagonal(int x, int y, short[,] configuration, int defendingColor, int xPolarity, int yPolarity, out int[] attacker)
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
                        attacker = new int[] { targetX, targetY };
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

            attacker = new int[0];
            return false;
        }


        public static bool EvaluateStraightThreat(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;
            int[] polar = new int[] { -1, 1 }; //what a stupid array

            for (int i = 0; i < 2; i++) //this is so gross don't look at it
            {
                threatened = threatened || CastVerticalLine(x, y, configuration, defendingColor, polar[i]);
            }

            for (int i = 0; i < 2; i++)
            {
                threatened = threatened || CastHorizontalLine(x, y, configuration, defendingColor, polar[i]);
            }

            return threatened;
        }

        public static bool EvaluateStraightThreat(int x, int y, short[,] configuration, int defendingColor, out List<int[]> attackers)
        {
            attackers = new List<int[]>();
            bool threatened = false;
            int[] polar = new int[] { -1, 1 }; //what a stupid array

            for (int i = 0; i < 2; i++) //this is so gross don't look at it
            {
                int[] attacker;
                bool attacked = CastVerticalLine(x, y, configuration, defendingColor, polar[i], out attacker);
                if (attacked)
                {
                    threatened = attacked;
                    if (attacker.Length > 0)
                    {
                        attackers.Add(attacker);

                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                int[] attacker;
                bool attacked = CastHorizontalLine(x, y, configuration, defendingColor, polar[i], out attacker);
                if (attacked)
                {
                    threatened = attacked;
                    if (attacker.Length > 0)
                    {
                        attackers.Add(attacker);

                    }
                }
            }

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
        private static bool CastVerticalLine(int x, int y, short[,] configuration, int defendingColor, int direction)
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

        private static bool CastVerticalLine(int x, int y, short[,] configuration, int defendingColor, int direction, out int[] attacker)
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
                        attacker = new int[] { x, targetSquare };
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

            attacker = new int[0];
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
        private static bool CastHorizontalLine(int x, int y, short[,] configuration, int defendingColor, int direction)
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

        private static bool CastHorizontalLine(int x, int y, short[,] configuration, int defendingColor, int direction, out int[] attacker)
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
                        attacker = new int[] { targetSquare, y };
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

            attacker = new int[0];
            return false;
        }


        public static bool EvaluateKingThreat(int x, int y, short[,] configuration, int defendingColor)
        {
            bool threatened = false;

            for (int i = 0; i < 8; i++)
            {
                int xDisplacement = ((i + 2) % 3) - 1;
                int yDisplacement = (((i / 3) + 2) % 3) - 1;
                threatened = CastL(x, y, configuration, defendingColor, xDisplacement, yDisplacement, 900);
                if (threatened)
                { break; }
            }

            return threatened;
        }

        public static bool EvaluateKingThreat(int x, int y, short[,] configuration, int defendingColor, out int[] attacker)
        {
            bool threatened = false;

            for (int i = 0; i < 8; i++)
            {
                int xDisplacement = ((i + 2) % 3) - 1;
                int yDisplacement = (((i / 3) + 2) % 3) - 1;
                threatened = CastL(x, y, configuration, defendingColor, xDisplacement, yDisplacement, 900, out attacker);
                if (threatened)
                {
                    break;
                }
            }

            attacker = new int[0];
            return threatened;
        }
    }
}
