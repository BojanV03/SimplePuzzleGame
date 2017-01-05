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
    class PuzzlePiece:Control
    {

        public bool[,] PieceArray;
        public LinkedList<PictureBox> pbList = new LinkedList<PictureBox>();
        public Point WorldLocation = new Point();
        public bool IsSet = false;
        public Point SetPosition = new Point(-1, -1);
        public int PieceHeight, PieceWidth;

        private static Bitmap staticTexture;

        static PuzzlePiece()
        {
            try
            {
                staticTexture = new Bitmap("texture.png");
            }
            catch (Exception)
            {
                MessageBox.Show("I was unable to load the texture file, please make sure it is in the same folder as the exe file and named texture.png. \nUntil then, enjoy the game without it");
            }
        }

        public PuzzlePiece(bool[,] inputArray, Point location, int cellSize, Color background)
        {
            int pbCounter = 0;
            int height = inputArray.GetLength(0);
            int width = inputArray.GetLength(1);
            // broj uzastopnih kolona sa leve strane u kojima su svi elementi false
            int clipTop = 0;
            // broj uzastopnih vrsta pocevsi od gornje strane u kojima su svi elementi false
            int clipLeft = 0;
            // ti redovi i vrste mogu da se preskoce
            int skipCounter = 0;

            WorldLocation = location;
            // brojanje kolona za preskakanje
            for (int i = 0; i < width; i++)
            {
                skipCounter = 0;
                for (int j = 0; j < height; j++)
                {
                    if (inputArray[j, i] == true)
                        break;

                    skipCounter++;
                }
                if (skipCounter == height)
                    clipLeft++;

                else
                    break;
            }
            // brojanje vrsta za preskakanje
            for (int i = 0; i < height; i++)
            {
                skipCounter = 0;
                for (int j = 0; j < width; j++)
                {
                    if (inputArray[i, j] == true)
                        break;

                    skipCounter++;
                }
                if (skipCounter == width)
                    clipTop++;

                else
                    break;
            }
            // eliminisali smo visak redova sa leve i gornje strane(clipTop i clipLeft), sada jos samo da odredimo visinu
            // i sirinu figure kako bi znali njene minimalne dimenzije(sto se koristi za matricu i kasnije za snapovanje u grid
            calculateWidthHeight(inputArray, inputArray.GetLength(0), inputArray.GetLength(1), clipTop, clipLeft);

            PieceArray = new bool[PieceHeight+1, PieceWidth+1];
            // pravljenje pictureBoxova za svaki true u input matrici
            for (int i = 0; i <= PieceHeight; i++)
            {
                for (int j = 0; j <= PieceWidth; j++)
                {
                    if (inputArray[i + clipTop, j + clipLeft] == true)
                    {
                        PieceArray[i, j] = inputArray[i + clipTop, j + clipLeft];
                        pbCounter++;

                        PictureBox pb = new PictureBox();
                        pb.Width = cellSize;
                        pb.Height = cellSize;
                        pb.Location = new Point(j * cellSize + WorldLocation.X, i * cellSize + WorldLocation.Y);
                        pb.Tag = i + ", " + j;
                        pb.BackColor = background;
                        pb.Image = staticTexture;
                        pb.SizeMode = PictureBoxSizeMode.StretchImage;
                        pbList.AddLast(pb);
                        pb.Show();
                    }
                }
            }

        }

        private void calculateWidthHeight(bool[,] arr, int height, int width, int clipTop, int clipLeft)
        {
            bool doneH = false;
            bool doneW = false;

            // Krecemo od donje desne ivice matrice i penjemo se gore red po red dok ne naidjemo na prvi True bool, tada smo nasli visinu dela
            for (int i = height - 1; i >= 0; i--)
            {
                for (int j = width - 1; j >= 0; j--)
                {
                    if (arr[i, j] == true && doneH == false)
                    {
                        PieceHeight = i - clipTop;
                        doneH = true;
                        break;
                    }
                }
                if (doneH == true)
                    break;
            }

            // Krecemo od donje desne ivice matrice i pomeramo se levo kolonu po kolonu dok ne naidjemo na prvi True bool, tada smo nasli sirinu dela
            for (int i = width - 1; i >= 0; i--)
            {
                for (int j = height - 1; j >= 0; j--)
                {
                    if (arr[j, i] == true && doneW == false)
                    {
                        PieceWidth = i - clipLeft;
                        doneW = true;
                        break;
                    }
                }
                if (doneW == true)
                    break;
            }
        }

        //changes the location of every pictureBox in the PuzzlePiece
        public void setWorldLocation(Point newLocation)
        {
            int deltaX = newLocation.X - WorldLocation.X;
            int deltaY = newLocation.Y - WorldLocation.Y;

            WorldLocation = newLocation;
            foreach (PictureBox b in pbList)
                b.Location = new Point(b.Location.X + deltaX, b.Location.Y + deltaY);
        }

        // Postavlja boju pozadine Puzzle dela
        private void setColor(Color C)
        {
            foreach(PictureBox pb in pbList)
                pb.BackColor = C;
        }

        // gura Puzzle deo na vrh (BringToFront)
        public void zOrderUp()
        {
            foreach(PictureBox P in pbList)
                P.BringToFront();
        }

        // Povecava/Smanjuje velicinu  puzzle dela na osnovu toga da li je povecana ili smanjena velicina prozora
        public void resizePiece(int newSize, float resizeAmmountWidth, float resizeAmmountHeight, Point gridTopLeft)
        {
            // Postavljanje relativne lokacije(lokacija pictureBoxa u odnosu na gornji levi ugao prozora)
            foreach (PictureBox pb in pbList)
            {
                Point relativeLocation = Functions.tagToLocation(pb);

                int i = relativeLocation.X;
                int j = relativeLocation.Y;

                pb.Size = new Size(newSize, newSize);
                pb.Location = new Point((int)(j * newSize + WorldLocation.X), (int)(i * newSize + WorldLocation.Y));
            }
            // Provera da li se nalazi u Gridu ili van njega, ako je u njemu onda mora preciznije da se odredi lokacija
            if(!IsSet)
                setWorldLocation(new Point((int)(WorldLocation.X / resizeAmmountWidth), (int)(WorldLocation.Y / resizeAmmountHeight)));
            else
                setWorldLocation(new Point(gridTopLeft.X + SetPosition.X * newSize, gridTopLeft.Y + SetPosition.Y * newSize));
        }
    }
}
