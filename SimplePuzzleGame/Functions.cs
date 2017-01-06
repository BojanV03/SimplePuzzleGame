using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimplePuzzleGame
{
    class Functions
    {
        static private Random R = new Random();

        /*  Ideja: za grid 8x5 napraviti matricu ovakvog tipa:
            * * * * * * * * * * * * * * * * *
            * 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 *
            * - | - | - | - | - | - | - | - *
            * 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 *
            * - | - | - | - | - | - | - | - *
            * 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 *
            * - | - | - | - | - | - | - | - *
            * 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 *
            * - | - | - | - | - | - | - | - *
            * 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 *
            * * * * * * * * * * * * * * * * *

            0 predstavlja prazno polje u gridu, - i | predstavlja sam grid a * spoljne granice

            zatim kretati se nasumicnim putanjama po - i |, svako poseceni karakter pritom menjati
            u *. Time cemo iseckati grid na delice. Posto je random, postoji sansa da se naprave
            delici koji se sastoje od samo jednog kvadratica. Njih fixSingleSquares uklanja povezujuci
            sa random okolnom figurom. Zatim prolaskom kroz matricu, za svaku 0 na koju naidjemo, pozovemo
            DFS obilazak i pokupimo sve 0 koje bi pripadale istoj figuri (sve koje se nalaze u istom poligonu
            obrazovanom *icama). Prilikom DFSa '0' pretvaramo u '1' da bi sprecili beskonacnu petlju i ponovno
            obilazenje istog delica usled prolaska kroz matricu. Krajnji rezultat izgleda

            * * * * * * * * * * * * * * * * *
            * 0 * 0 * 0 | 0 * 0 | 0 | 0 | 0 *
            * - * - * - | - * * * * * - | - *
            * 0 * 0 * 0 | 0 * 0 | 0 * 0 | 0 *
            * - * * * * * - * * * * * * * * *
            * 0 | 0 | 0 * 0 * 0 * 0 | 0 | 0 *
            * * * * * * * - * - * - * * * * *
            * 0 | 0 * 0 * 0 * 0 * 0 * 0 | 0 *
            * - * * * - * * * - * * * * * * *
            * 0 * 0 | 0 | 0 * 0 | 0 * 0 | 0 *
            * * * * * * * * * * * * * * * * *
         * */

        // Aktivira double buffering za prosledjenu kontrolu, preuzeto sa StackOverflowa
        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }

        // Na osnovu zadate visine i sirine grida generise gore opisanu matricu koju potom sece
        // nasumicnim putanjama fractureIterations puta i generise listu puzzle delova
        // brinuci o tome da se ne pojavi puzzle deo koji se sastoji od samo 1 pictureBoxa
        public static LinkedList<PuzzlePiece> generateRandomPieces(int height, int width, int pieceSize, int fractureIterations)
        {
            int arrayHeight = 2 * height + 1;
            int arrayWidth = 2 * width + 1;

            char[,] arr = new char[arrayHeight, arrayWidth];

            //generisanje pocetne matrice
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (j == 0 || i == 0 || i == arrayHeight - 1 || j == arrayWidth - 1)
                        arr[i, j] = '*';

                    else if (j % 2 == 0)
                        arr[i, j] = '|';

                    else if (i % 2 == 0)
                        arr[i, j] = '-';

                    else
                        arr[i, j] = '0';
                }
            }

            // delimo na pola po sirini; ovime garantujemo da ce biti barem 2 resenja
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (width % 2 == 1)
                {
                    arr[i, width + 1] = '*';
                }
                else
                    arr[i, width] = '*';
            }

            // seckanje
            for (int i = 0; i < fractureIterations; i++)
            {
                splitHeight(arr);
                splitWidth(arr);
            }

            fixSingleSquares(arr);

            return matrixToPuzzlePieceArray(arr, pieceSize);
        }

        // Iz random pozicije sa gornje strane grida idemo na dole sa tim sto na posle svakog pomeranja dole postoji 33% sanse
        // da cemo se pomeriti levo ili desno
        private static void splitHeight(char[,] arr)
        {
            int height = (arr.GetLength(0) - 1) / 2;
            int randValue = ((R.Next() % (height - 1)) * 2) + 4;

            for (int i = 0; i < arr.GetLength(1); i++)
            {
                int random = R.Next() % 3;
                if (i % 2 == 0 && random == 0 && randValue + 2 < arr.GetLength(0))
                {
                    arr[randValue + 1, i] = '*';
                    randValue += 2;
                }
                else if (i % 2 == 0 && random == 1 && randValue - 2 > 0)
                {
                    arr[randValue - 1, i] = '*';
                    randValue -= 2;
                }
                arr[randValue, i] = '*';
            }
        }

        // Iz random pozicije sa leve strane idemo na desno sa tim sto na posle svakog pomeranja desno postoji 33% sanse
        // da cemo se pomeriti gore ili dole
        private static void splitWidth(char[,] arr)
        {
            int width = (arr.GetLength(1) - 1) / 2;
            int randValue = ((R.Next() % (width - 1)) * 2) + 4;

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                int random = R.Next() % 3;
                if (i % 2 == 0 && random == 0 && randValue + 2 < arr.GetLength(1))
                {
                    arr[i, randValue + 1] = '*';
                    randValue += 2;
                }
                else if (i % 2 == 0 && random == 1 && randValue - 2 > 0)
                {
                    arr[i, randValue - 1] = '*';
                    randValue -= 2;
                }
                arr[i, randValue] = '*';
            }
        }
        // Popravljanje puzzle delova koji se sastoje od samo 1. pictureBoxa
        private static void fixSingleSquares(char[,] arr)
        {
            // Jednostavan prolazak kroz matricu
            for (int i = 1; i < arr.GetLength(0); i += 2)
            {
                for (int j = 1; j < arr.GetLength(1); j += 2)
                {
                    // I ako naidjemo na element koji je okruzen zvezdicama sa svih strana
                    if (arr[i - 1, j] == '*' && arr[i + 1, j] == '*' && arr[i, j - 1] == '*' && arr[i, j + 1] == '*')
                    {
                        bool notDone = true;
                        // obrisemo nasumicnu zvezdicu
                        while (notDone)
                        {
                            int random = new Random().Next() % 4;
                            if (random == 0 && i != 1)
                            {
                                arr[i - 1, j] = '-';
                                notDone = false;
                            }
                            else if (random == 1 && j != 1)
                            {
                                arr[i, j - 1] = '|';
                                notDone = false;
                            }
                            else if (random == 2 && i != arr.GetLength(0) - 2)
                            {
                                arr[i + 1, j] = '-';
                                notDone = false;
                            }
                            else if (random == 3 && j != arr.GetLength(1) - 2)
                            {
                                arr[i, j + 1] = '|';
                                notDone = false;
                            }
                        }
                    }
                }
            }
        }

        public static LinkedList<PuzzlePiece> matrixToPuzzlePieceArray(char[,] arr, int pieceSize)
        {
            LinkedList<PuzzlePiece> list = new LinkedList<PuzzlePiece>();
            bool[,] pieceMatrix = new bool[(arr.GetLength(0)-1)/2, (arr.GetLength(1)-1)/2];

            for (int i = 1; i < arr.GetLength(0); i+=2)
            {
                for(int j = 1; j < arr.GetLength(1); j+=2)
                {
                    if(arr[i, j] == '0')
                    {
                        DFS(arr, pieceMatrix, i, j);    // DFS u pieceMatrix upise oblik puzzleDela i u arr matrici postavi taj deo na jedinice
                        Color C = Color.FromArgb(R.Next() % 128 + 128, R.Next() % 128 + 128, R.Next() % 128 + 128);
                        list.AddLast(new PuzzlePiece(pieceMatrix, new Point(0, 0), pieceSize, C));
                        pieceMatrix = new bool[(arr.GetLength(0) - 1) / 2, (arr.GetLength(1) - 1) / 2];
                    }
                }
            }
            return list;
        }

        // za sve 0 iz arr koje pripadaju jednom puzzle delicu setujemo bitove u pieceMatrix
        private static void DFS(char[,] arr, bool[,] pieceMatrix, int i, int j)
        {
            if(arr[i, j] == '0')
            {
                pieceMatrix[(i - 1) / 2, (j - 1) / 2] = true;
                arr[i, j] = '1';
            }
            // Idemo dole
            if(arr[i+1, j] != '*' && arr[i+2, j] == '0')
                DFS(arr, pieceMatrix, i + 2, j);

            // Idemo gore
            if (arr[i - 1, j] != '*' && arr[i - 2, j] == '0')
                DFS(arr, pieceMatrix, i - 2, j);

            // Idemo desno
            if (arr[i, j+1] != '*' && arr[i, j+2] == '0')
                DFS(arr, pieceMatrix, i, j+2);

            // Idemo levo
            if (arr[i, j - 1] != '*' && arr[i, j - 2] == '0')
                DFS(arr, pieceMatrix, i, j - 2);

            return;
        }
        // Koristio sam Tag pictureBoxa za cuvanje (i, j) pozicije pictureBoxa unutar samog PuzzlePiece
        // Ova funkcija iz Stringa tipa "i, j" cita vrednosti i i j i konvertuje ih u integere
        public static Point tagToLocation(PictureBox pb)
        {
            String tag = pb.Tag.ToString();
            int i = Convert.ToInt32(tag.Substring(0, tag.IndexOf(',')));
            int j = Convert.ToInt32(tag.Substring(tag.IndexOf(',') + 1, tag.Length - (tag.IndexOf(',') + 1)));

            return new Point(i, j);
        }

        // Provera da li se dva pravougaonika preklapaju
        public static bool rectangleOverlapCheck(Point topLeftA, Point bottomRightA, Point topLeftB, Point bottomRightB)
        {
            Size A = new Size(bottomRightA.X - topLeftA.X, bottomRightA.Y - topLeftA.Y);
            Size B = new Size(bottomRightB.X - topLeftB.X, bottomRightB.Y - topLeftB.Y);

            if (topLeftA.X < bottomRightB.X && bottomRightA.X > topLeftB.X &&
                topLeftA.Y < bottomRightB.Y && bottomRightA.Y > topLeftB.Y)
            {
                return true;
            }

            return false;
        }

        // Koriscene u prethodnoj verziji programa, vise nisu neophodne ali mi se nisu brisale
        public int clamp(int number, int lowerLimit, int upperLimit)
        {
            if (number < lowerLimit)
                return lowerLimit;
            else if (number > upperLimit)
                return upperLimit;
            else
                return number;
        }

        public float clamp(float number, float lowerLimit, float upperLimit)
        {
            if (number < lowerLimit)
                return lowerLimit;
            else if (number > upperLimit)
                return upperLimit;
            else
                return number;
        }
    }
}
