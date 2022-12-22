using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Koursach_Tri_v_Ryad
{
    public class GameLogic
    {
        Cell[,] gamefield = new Cell[quatity, quatity]; //массив игровых ячеек

        // переменные координат которые понадобятся для перестановки ячеек
        int X = -1;
        int Y = -1;

        const int quatity = 8; // размеры игрового поля
        const int nulltipe = -99; //пустой тип ячейки, не содержащий картинку
        const int moves = 10; // общие число ходов 


        public int leftmoves = moves; // число оставшихся ходов

        // переменные типа картинок у ячеек которые понадобятся для перестановки ячеек
        int ChosenCell1 = -1; //gamefield1
        int ChosenCell2 = -1; //gamefield2

        bool SwitchCells; //логическая переменная подтверждающая возможность перестановки gamefieldzamena
        public int score { get; set; } //число заработанных очков

        //константа необходима для устранения ошибки при подсчета очков, 
        //т.к в начале игры все ячейки пустые он будет считать их совпадающими,
        //данная формула устраняет данную ошибку даже если изменять количество ячеек на поле
        const int MissScore = 5 * (( - 2) * 3 * quatity * 2);

        public void GameSetScore(int score)
        {
            this.score = score;
        }

        List<Cell> MatchedCells = new List<Cell>();

        Random rng = new Random();

        public EventHandler Falled;

        private void FallCellsss()
        {
            MatchThree();
            if (Hasnullpic())
            {
                while (Hasnullpic())
                {
                    FallCells();
                    Falled(this, null);
                    Thread.Sleep(400); //задержка замены ячеек
                }
                StartFall();
            }
            else
            {
                if (leftmoves == 0)
                {
                    MessageBox.Show("Ходы закончились \n ВАШЕ ЧИСЛО ОЧКОВ: " + (score - MissScore));
                    score = MissScore;
                }
            }
        }

        public void StartFall()
        {
            //осуществляет запуск автоматического сдвига ячеек вниз в отдельном потоке
            Thread newThread = new Thread(new ThreadStart(FallCellsss));
            newThread.Start();
        }

        public GameLogic(Cell[,] gamefield)
        {
            this.gamefield = gamefield;
        }

        public int getScore()
        {
            return score;
        }

        public int getLeftMoves()
        {
            return leftmoves;
        }

        public void setMovesLeft(int movesleft)
        {
            this.leftmoves = movesleft;
        }

        public void moveCell(int i, int j)
        {
            MatchedCells.Clear(); //очистка списка ячеек для удаления            

            //если переменные координат отрицательные, то в них записывается индекс массива ячеек
            //записывается тип картинки для ее перестановки
            if ((X == -1) && (Y == -1))
            {
                X = i;
                Y = j;
                ChosenCell1 = gamefield[i, j].typeofpic;
            }
            else
            {
                //в случае если переменные содержат индексы осуществляется проверка для второй ячейки для замены
                //если она ближайшая по вертикали и горизонтали, то происходит их замена
                if (((X == i) && (Math.Abs(Y - j) == 1)) || ((Y == j) && (Math.Abs(X - i) == 1)))
                {
                    ChosenCell2 = gamefield[i, j].typeofpic;

                    gamefield[X, Y].typeofpic = ChosenCell2;
                    gamefield[i, j].typeofpic = ChosenCell1;

                    //получает число совпадающих ячеек                    
                    List<Cell> row = MatchThree();
                    //если совпадения присутствуют то они удаляются
                    if (row.Count() != 0)
                    {
                        MatchThree();
                    }
                    leftmoves--; //отнимается ход
                    SwitchCells = true; //переменная проверки на замену устанавливается положительной в случае ее выполнения
                }
                else
                {
                    SwitchCells = false;
                }
                //если переменная перестановки положительна, то координаты становятся отрицательными 
                //выполняется сдвиг ячеек вниз
                if (SwitchCells == true)
                {
                    X = -1;
                    Y = -1;

                    ChosenCell1 = -1;
                    ChosenCell2 = -1;

                    StartFall();
                }
            }
        }
        public List<Cell> MatchThree()
        {
            MatchedCells.Clear(); //очистка списка совпадающих ячеек

            int count; //счетчик для определения количества похожих ячеек

            //цикл проверки элементов по горизонтали
            for (int i = 0; i < quatity; i++)
            {
                int type = gamefield[0, i].typeofpic; //переменная сохраняет тип картинки
                count = 1;
                for (int j = 0; j < quatity; j++)
                {
                    //при совпадении картинок переменной и следующей ячейки увеличивается счетчик,
                    //показывающий, сколько ячеек совпадают
                    //иначе он остается равным 1
                    if ((type == gamefield[i, j].typeofpic) && (j != 0))
                        count++;
                    else
                        count = 1;
                    //если совпадений больше чем 2, то ячейки записываются в список на удаление
                    if (count > 2)
                    {
                        MatchedCells.Add(gamefield[i, j - 2]);
                        MatchedCells.Add(gamefield[i, j - 1]);
                        MatchedCells.Add(gamefield[i, j]);
                    }
                    type = gamefield[i, j].typeofpic; //обновляем тип картинки ячейки на следующую
                }
            }

            //аналогичная функция проверки по вертикали
            for (int j = 0; j < quatity; j++)
            {
                int type = gamefield[j, 0].typeofpic;
                count = 1;
                for (int i = 0; i < quatity; i++)
                {
                    if ((type == gamefield[i, j].typeofpic) && (i != 0))
                        count++;
                    else
                        count = 1;
                    if (count > 2)
                    {
                        MatchedCells.Add(gamefield[i - 2, j]);
                        MatchedCells.Add(gamefield[i - 1, j]);
                        MatchedCells.Add(gamefield[i, j]);
                    }
                    type = gamefield[i, j].typeofpic;
                }
            }

            //для каждого элемента списка на удаление присваиваем тип пустой картинки ячейки
            foreach (Cell elem in MatchedCells)
            {
                elem.typeofpic = nulltipe;
            }

            score += MatchedCells.Count() * 5; //подсчет очков за каждое совпадение

            return MatchedCells;
        }

        public bool Hasnullpic()
        {
            for (int j = 0; j < quatity; j++)
            {
                for (int i = 0; i < quatity; i++)
                {
                    if (gamefield[i, j].typeofpic == nulltipe) return true;
                }
            }
            return false;
        }

        public void FallCells()
        {
            int typeel = -1;

            for (int j = quatity - 1; j >= 0; j--)
            {
                for (int i = quatity - 2; i >= 0; i--)
                {
                    if (gamefield[i + 1, j].typeofpic == nulltipe)
                    {
                        typeel = gamefield[i, j].typeofpic;

                        gamefield[i + 1, j].typeofpic = typeel;
                        gamefield[i, j].typeofpic = nulltipe;
                    }
                }
            }
            for (int j = 0; j < quatity; j++)
                for (int i = 0; i < quatity; i++)
                {
                    if (gamefield[i, j].typeofpic == nulltipe)
                    {
                        gamefield[i, j].typeofpic = rng.Next(0, 6);
                    }
                    break;
                }
        }
    }
}
