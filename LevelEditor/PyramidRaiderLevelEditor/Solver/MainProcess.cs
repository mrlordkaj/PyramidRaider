using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PyramidRaiderLevelEditor.Solver
{
    public class MainProcess
    {
        private static short xExit, yExit;
        // 11 1111 last four digits: up, right, down, left
        // first two digits: key, traps
        private short step;
        private static short stateSide;
        public static short[] stateSideCount = new short[2];
        private static List<StaticState> existedState;
        public static State[][] mang = new State[2][] { new State[1000], new State[1000] }; //State arrays

        private static Human[] trace;

        public MainProcess(State state, short xxExit, short yyExit, List<int> solution)
        {
            StaticState tam = new StaticState(state);
            state.setStep((short)1);
            state.getMaze()[state.getHuman().getX()][state.getHuman().getY()][1] = 1;
            stateSide = 0;
            stateSideCount[stateSide] = 0; //It's count - 1 for the array sake
            stateSideCount[(short)((stateSide + 1) % 2)] = -1;
            mang[0][0] = state;
            xExit = xxExit;
            yExit = yyExit;

            mang[0][0] = state;
            existedState = new List<StaticState>();
            existedState.Add(tam);

            breadthFirstSearch(solution);
            //printResult(tam);
        }

        private static void breadthFirstSearch(List<int> solution)
        {
            while (true)
            {
                if (stateSideCount[stateSide] == -1)
                {
                    System.Diagnostics.Debug.WriteLine("Cannot find any solution!");
                    solution.Clear();
                    return;
                }
                for (int i = 0; i <= stateSideCount[stateSide]; i++)
                {
                    State tam = mang[stateSide][i].getHuman().attemptMove(mang[stateSide][i], stateSide, i);
                    if (tam != null)
                    { //Exit condition
                        //Trace back
                        int xx = tam.getHuman().getX();
                        int yy = tam.getHuman().getY();
                        int j = tam.getStep();
                        trace = new Human[j];
                        trace[j - 1] = new Human(xx, yy);

                        for (j -= 1; j > 0; j--)
                        { //k here is the currentStep being traced
                            if (traceStep(tam, j, xx - 1, yy))
                            {
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("down");
#endif
                                solution.Insert(0, 2);
                                xx--;
                                trace[j - 1] = new Human(xx, yy);
                            }
                            else if (traceStep(tam, j, xx + 1, yy))
                            {
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("up");
#endif
                                solution.Insert(0, 0);
                                xx++;
                                trace[j - 1] = new Human(xx, yy);
                            }
                            else if (traceStep(tam, j, xx, yy - 1))
                            {
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("right");
#endif
                                solution.Insert(0, 1);
                                yy--;
                                trace[j - 1] = new Human(xx, yy);
                            }
                            else if (traceStep(tam, j, xx, yy + 1))
                            {
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("left");
#endif
                                solution.Insert(0, 3);
                                yy++;
                                trace[j - 1] = new Human(xx, yy);
                            }
                            else if (traceStep(tam, j, xx, yy))
                            {
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("stand");
#endif
                                solution.Insert(0, 4);
                                trace[j - 1] = new Human(xx, yy);
                            }
                        }
                        return;
                    }
                }
                stateSideCount[stateSide] = -1; //Reset count for the used one
                stateSide = (short)((stateSide + 1) % 2); //Change side
            }
        }

        //private static void printResult(StaticState state)
        //{

        //    StaticState[] resultState = new StaticState[trace.Length];
        //    resultState[0] = state;

        //    short[][][] tam;
        //    for (int i = 0; i < trace.Length - 1; i++)
        //    {
        //        trace[i].move(trace[i + 1].getX(), trace[i + 1].getY());
        //        tam = new short[resultState[i].getStaticMaze().Length][][];
        //        for (int j = 0; j < tam.Length; j++)
        //        {
        //            tam[j] = new short[resultState[i].getStaticMaze()[0].Length][];
        //            for (int k = 0; k < tam[j].Length; k++)
        //            {
        //                tam[j][k] = new short[1];
        //            }
        //        }
        //        for (int j = 0; j < tam.Length; j++)
        //        {
        //            for (int k = 0; k < tam[0].Length; k++)
        //            {
        //                tam[j][k][0] = resultState[i].getStaticMaze()[j][k];
        //            }
        //        }
        //        State stateTam = new State(tam, resultState[i].getKey(), resultState[i].getMummyWhite(), resultState[i].getMummyRed(),
        //                resultState[i].getSk(), resultState[i].getSkRed(), trace[i], (short)0);
        //        stateTam = Human.keyCheck(stateTam, stateTam.getHuman());
        //        stateTam = stateTam.getHuman().testOfMummy(stateTam);
        //        resultState[i + 1] = new StaticState(stateTam);
        //    }
        //}

        private static bool traceStep(State state, int currentStep, int findX, int findY)
        {
            try
            {
                int test = state.getMaze()[findX][findY][0];
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
            for (int i = 1; i < 15; i++)
            {
                try
                {
                    int test = state.getMaze()[findX][findY][i];
                }
                catch (IndexOutOfRangeException e)
                {
                    return false;
                }
                if (state.getMaze()[findX][findY][i] == currentStep)
                {
                    return true;
                }
            }
            return false;
        }

        public static int getTraceLength()
        {
            return trace.Length;
        }

        //Getter & Setter Auto Generated Code
        public static short getxExit()
        {
            return xExit;
        }

        public static short getyExit()
        {
            return yExit;
        }

        public short getStep()
        {
            return step;
        }

        public static short getStateSide()
        {
            return stateSide;
        }

        public static short[] getStateSideCount()
        {
            return stateSideCount;
        }

        public static State[][] getMang()
        {
            return mang;
        }

        public static void setxExit(short xExit)
        {
            MainProcess.xExit = xExit;
        }

        public static void setyExit(short yExit)
        {
            MainProcess.yExit = yExit;
        }

        public void setStep(short step)
        {
            this.step = step;
        }

        public static void setStateSide(short stateSide)
        {
            MainProcess.stateSide = stateSide;
        }

        public static void setStateSideCount(short[] stateSideCount)
        {
            MainProcess.stateSideCount = stateSideCount;
        }

        public static void setMang(State[][] mang)
        {
            MainProcess.mang = mang;
        }

        public static List<StaticState> getExistedState()
        {
            return existedState;
        }

        public static void setExistedState(List<StaticState> existedState)
        {
            MainProcess.existedState = existedState;
        }
    }
}
