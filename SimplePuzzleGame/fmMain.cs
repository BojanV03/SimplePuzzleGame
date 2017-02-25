using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimplePuzzleGame
{
    public partial class PuzzleForm : Form
    {
        public PuzzleForm()
        {
            InitializeComponent();
        }
        LinkedList<PuzzlePiece> PuzzlePieceList;   //lista puzzle delova
        fmSize inputSizeForm = new fmSize();

        int GridCellCountX = 6, GridCellCountY = 6;
        int CellSize;
        bool GameCompleted = false;
        bool GameStarted = false;
        int fractureIterations;

        Point GridTopLeftCorner, GridBottomRightCorner;
        bool[,] GridStateMatrix;     // matrica za cuvanje informacija o stanju grida
                                    // ako je [i,j] == true, tu je postavljen puzzle piece
        static PuzzlePiece DraggingPiece;   // puzzle deo koji upravo pomeramo misem
        Point RelativeLocation; // lokacija tog puzzle dela u odnosu na mis

        static Size PreviousFormSize;   // koristi se da olaksa racun velicine i pozicije pictureBoxova posle resize-ovanja

        // inicijalizuje igru sa unetom visinom/sirinom, generise random delove i pokrece tajmere
        private void btnNewGame_Click(object sender, EventArgs e)
        {
            // Prikazujemo formu za podesavanje opcija igre
            inputSizeForm.ShowDialog();

            if (inputSizeForm.DialogResult == DialogResult.OK)
            {
                //ako je korisnik kliknuo na OK u formi, generisemo igru
                initGame(inputSizeForm.getHeight(), inputSizeForm.getWidth());
                fractureIterations = inputSizeForm.getFractureIterations();
                generateRandomPieces();

                pbGrid.Refresh();
                drawGrid(pbGrid);
                resetTimer();
                bringButtonsToFront();
                PuzzleForm_Resize(sender, e);
                repositionPieces();
                btnDemo.Enabled = true;
            }
        }

        // Resetuje igru tj ponisti GridState matricu, resetuje tajmer, razbaca delove i stavi GameCompleted na false i GameStarted na true
        private void btnReset_Click(object sender, EventArgs e)
        {
            btnReset.Enabled = false;
            repositionPieces();
            for (int i = 0; i < GridStateMatrix.GetLength(0); i++)
                for (int j = 0; j < GridStateMatrix.GetLength(1); j++)
                    GridStateMatrix[i, j] = false;

            pbGrid.Refresh();
            drawGrid(pbGrid);
            resetTimer();
            GameCompleted = false;
            GameStarted = true;
            bringButtonsToFront();
        }

        // brise postojece puzzle delove, podesava visinu/sirinu i resetuje tajmer
        private void initGame(int height, int width)
        {
            // Ako su vec inicijalizovani puzzle delovi, Dispose-ujemo ih
            if (PuzzlePieceList != null)
            {
                foreach (PuzzlePiece pPiece in PuzzlePieceList)
                {
                    foreach (PictureBox pBox in pPiece.pbList)
                    {
                        pBox.Hide();
                        pBox.Dispose();
                    }
                    pPiece.Dispose();
                }
                PuzzlePieceList = null;
            }

            //Inicijalizacija parametara i prozora
            GridCellCountX = width;
            GridCellCountY = height;
            GridStateMatrix = new bool[GridCellCountY, GridCellCountX];
            lblTimer.Text = "0";
            gameTimer.Enabled = true;
            GameCompleted = false;
            GameStarted = true;
            PuzzleForm_Resize(new Object(), new EventArgs());
        }

        /* generise nasumicne puzzle delove koji savrseno popunjavaju grid na barem 2 razlicita nacina, i postavlja ih u
         PuzzlePieceList listu*/
        private void generateRandomPieces()
        {
            // Generisemo sve potrebne puzzle delove, sacuvamo ih u listi
            PuzzlePieceList = Functions.generateRandomPieces(GridCellCountY, GridCellCountX, CellSize, fractureIterations);
            // i prikazemo ih na ekranu/dodamo onClick funkcionalnosti
            generatePiecesFromList(PuzzlePieceList);
        }

        // zadatu listu puzzle delova prikazuje na ekranu i dodaje onMove i onClick funkcionalnosti
        private void generatePiecesFromList(LinkedList<PuzzlePiece> puzzleList)
        {
            foreach (PuzzlePiece pp in puzzleList)
            {
                foreach (PictureBox pb in pp.pbList)
                {
                    Controls.Add(pb);
                    pb.BringToFront(); pb.BringToFront();
                    pb.MouseDown += new MouseEventHandler(pb_MouseDown);
                    pb.MouseMove += new MouseEventHandler(pb_MouseMove);
                }
            }
        }

        // Iscrtavanje grida
        public void drawGrid(PictureBox pb)
        {
            Graphics gridGraphics = pb.CreateGraphics();
            Pen p = new Pen(Color.DarkBlue, 5);
            int maxGridSize = GridCellCountX > GridCellCountY? GridCellCountX : GridCellCountY;

            // brojac ide od 0 do maksimalnog broja linija koje treba da iscrta(max od broja horizontalnih i broja vertikalnih linija)
            for (int i = 0; i <= maxGridSize; i++)
            {
                // i dok god nije iscrtao dovoljno horizontalnih, iscrtava horizontalne
                if (i <= GridCellCountX)
                {
                    Point p1 = new Point(i * CellSize, 0);
                    Point p2 = new Point(i * CellSize, pb.Height);
                    gridGraphics.DrawLine(p, p1, p2);
                }
                // i dok god nije iscrtao dovoljno vertikalnih, iscrtava vertikalne
                if (i <= GridCellCountY)
                {
                    Point p3 = new Point(0, CellSize * i);
                    Point p4 = new Point(pb.Width, CellSize * i);
                    gridGraphics.DrawLine(p, p3, p4);
                }
            }
            gridGraphics.Dispose();
        }

        //izracunava na kom [i, j] mestu u matrici (gridu) se nalazi zadata lokacija
        private Point locationToGridPoint(Point location)
        {
            // updatovanje Debug labela
            lblRelativeLocation.Text = RelativeLocation.ToString();
            lblWorldLocation.Text = location.ToString();

            // Ukoliko se mis ne nalazi u gridu, samo vrati (-1, -1) lokaciju
            if (location.X < GridTopLeftCorner.X || location.Y < GridTopLeftCorner.Y
             || location.X + ((int)(RelativeLocation.X / CellSize)) * CellSize > GridBottomRightCorner.X
             || location.Y + ((int)(RelativeLocation.Y / CellSize)) * CellSize > GridTopLeftCorner.Y + (GridCellCountY) * CellSize)
                return new Point(-1, -1);

            // Inace izracunamo i vratimo koordinate u gridu
            return new Point((int)(location.X - GridTopLeftCorner.X) / CellSize,
                                     (int)((location.Y - GridTopLeftCorner.Y) / CellSize));
        }

        // Resetuje FPS timer
        private void resetTimer()
        {
            DraggingPiece = null;
            lblTimer.Text = "0";
            gameTimer.Enabled = true;
        }

        // Razbaca Puzzle delove oko table
        private void repositionPieces()
        {
            Random R = new Random();

            foreach (PuzzlePiece P in PuzzlePieceList)
            {
                foreach(PictureBox pb in P.pbList)
                {
                    pb.Visible = false;
                }
            }
                foreach (PuzzlePiece P in PuzzlePieceList)
            {
                // Moguce poboljsanje efikasnosti: podeliti prozor na 8 segmenata, 3 levo od table, 3 desno od table, 1 ispod, 1 iznad
                // i onda u njih jedan po jedan ubacivati delove na nasumicne pozicije a onda proveravati kolizije samo sa vec postavljenim
                // delovima unutar tog jednog segmenta.

                // Ali posto mi za 20x20 sa 15 fracture iteracija treba manje od 0.2s da ih sve postavi kako treba, ostavio sam ovaj nacin
                // jer je dosta jednostavniji za implementaciju
                bool positionIncorrect = true;
                // Dok god je na losoj poziciji
                while(positionIncorrect)
                {
                    int locationX, locationY;
                    // Generisemo novu poziciju
                    locationX = CellSize + R.Next() % (this.Width -20 - (P.PieceWidth+2)*CellSize);
                    locationY = CellSize + R.Next() % (this.Height-50 - (P.PieceHeight+2)*CellSize);
                    // Proverimo da li je unutar grida(sto ne sme)
                    bool belowGrid = (locationY >= pbGrid.Location.Y);
                    bool aboveGrid = (locationY <= pbGrid.Location.Y + pbGrid.Height);
                    bool rightGrid = (locationX >= pbGrid.Location.X);
                    bool leftGrid = (locationX <= pbGrid.Location.X + pbGrid.Width);
                    bool insideGrid = belowGrid && aboveGrid && rightGrid && leftGrid;

                    P.setWorldLocation(new Point(locationX, locationY));
                    // Ako nije unutar grida i nema kolizija sa nekim drugim delom (canBePlaced funkcija to proverava), onda izlazimo iz petlje
                    if (!insideGrid && canBePlaced(P))
                    {
                        P.IsSet = false;
                        positionIncorrect = false;
                        foreach (PictureBox pb in P.pbList)
                        {
                            pb.Visible = true;
                        }
                    }
                }
            }
        }

        private bool overlapsButtons(PictureBox pb)
        {
            if (overlapSingleButton(pb, btnClose) || overlapSingleButton(pb, btnDemo) ||
                overlapSingleButton(pb, btnNewGame) || overlapSingleButton(pb, btnReset) ||
                overlapSingleButton(pb, btnToggleFullScreen))
                return true;

            return false;
        }     
        private bool overlapSingleButton(PictureBox pb, Button b)
        {
            Point btnTopLeft = new Point(b.Location.X, b.Location.Y);
            Point btnBottomRight = new Point(b.Location.X + b.Width, b.Location.Y + b.Height);
            Point pbTopLeft = new Point(pb.Location.X, pb.Location.Y);
            Point pbBottomRight = new Point(pb.Location.X + pb.Width, pb.Location.Y + pb.Height);
            if (Functions.rectangleOverlapCheck(btnTopLeft, btnBottomRight, pbTopLeft, pbBottomRight))
                return true;
            return false;
        }
        private bool canBePlaced(PuzzlePiece p)
        {   // Provera da li se nalazi izvan ekrana
            if (p.WorldLocation.X < 0 || p.WorldLocation.Y < 0 || p.WorldLocation.X + p.PieceWidth * CellSize > this.Width - CellSize || p.WorldLocation.Y + p.PieceHeight * CellSize > this.Height - 37 - CellSize)
                return false;

            // Provera da li se bilo koji pictureBox sece sa ivicom table
            foreach (PictureBox pPb in p.pbList)
            {
                Point pPbTopLeft = pPb.Location;
                Point pPbBottomRight = new Point(pPb.Location.X + CellSize, pPb.Location.Y + CellSize);

                if (pPbTopLeft.Y <= pbGrid.Location.Y + pbGrid.Height && pPbBottomRight.Y >= pbGrid.Location.Y)
                {
                    // Leva ivica
                    if (p.WorldLocation.X < pbGrid.Location.X && pPbBottomRight.X > pbGrid.Location.X)
                        return false;
                    // Desna ivica
                    if (p.WorldLocation.X < pbGrid.Location.X + pbGrid.Width && pPbBottomRight.X > pbGrid.Location.X + pbGrid.Width)
                        return false;
                }
                if (pPbBottomRight.X <= pbGrid.Location.X + pbGrid.Width && pPbTopLeft.X >= pbGrid.Location.X)
                {
                    // Gornja ivica
                    if (p.WorldLocation.Y < pbGrid.Location.Y && pPbBottomRight.Y > pbGrid.Location.Y)
                        return false;
                    // Donja ivica
                    if (p.WorldLocation.Y < pbGrid.Location.Y + pbGrid.Height && pPbBottomRight.Y > pbGrid.Location.Y + pbGrid.Height)
                        return false;
                }
            }

            // Provera da li se bilo koji pictureBox sece sa bilo kojim dugmetom
            foreach(PictureBox pb in p.pbList)
            {
                if (overlapsButtons(pb))
                    return false;
            }

            foreach (PuzzlePiece staticPuzzle in PuzzlePieceList)
            {
                if (staticPuzzle == p)
                    continue;

                {
                    Point pTopLeft = p.WorldLocation;
                    Point staticPTopLeft = staticPuzzle.WorldLocation;
                    Point pBottomRight = new Point(p.WorldLocation.X + (p.PieceWidth+1) * CellSize, p.WorldLocation.Y + (p.PieceHeight+1) * CellSize);
                    Point staticPBottomRight = new Point(staticPuzzle.WorldLocation.X + (staticPuzzle.PieceWidth+1) * CellSize, staticPuzzle.WorldLocation.Y + (staticPuzzle.PieceHeight+1) * CellSize);
                    // proveravamo da li se seku bounding boxovi dva puzzle dela
                    if (!Functions.rectangleOverlapCheck(pTopLeft, pBottomRight, staticPTopLeft, staticPBottomRight))
                    {
                        continue;
                    }
                }
                // ako se seku bounding boxovi, onda proveri da li se seku pictureBoxovi
                foreach (PictureBox staticPb in staticPuzzle.pbList)
                {
                    foreach(PictureBox pPb in p.pbList)
                    {
                        Point pPbTopLeft = pPb.Location;
                        Point staticPbTopLeft = staticPb.Location;
                        Point pPbBottomRight = new Point(pPb.Location.X + CellSize, pPb.Location.Y + CellSize);
                        Point staticPbBottomRight = new Point(staticPb.Location.X + CellSize, staticPb.Location.Y + CellSize);

                        if(Functions.rectangleOverlapCheck(pPbTopLeft, pPbBottomRight, staticPbTopLeft, staticPbBottomRight))
                            return false;
                    }
                }
            }

            return true;
        }

        // klik na pictureBox
        private void pb_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                //provera da li je PictureBox kliknut, ako nije bacice Exception koji ignorisemo
                PictureBox _sender = (PictureBox)sender;

                if (DraggingPiece == null && gameTimer.Enabled) // Ako nista nije attachovano za mis
                {
                    DraggingPiece = findPicBoxParrent(_sender); // attachujemo kliknuti pictureBox
                    lblDragging.Text = DraggingPiece.ToString();// <- za Debug labelu
                    // Izracunavamo poziciju kliknutog pictureBoxa u odnosu na mis
                    RelativeLocation = new Point(e.X + _sender.Location.X - DraggingPiece.WorldLocation.X, e.Y + _sender.Location.Y - DraggingPiece.WorldLocation.Y);
                    DraggingPiece.zOrderUp();
                    DraggingPiece.setTexture(true);
                    setButtonsEnabled(false);
                    if (DraggingPiece.IsSet)    // Ako je bio postavljen u grid, onda moramo da modifikujemo GridState matricu
                    {
                        updateMatrix(DraggingPiece, locationToGridPoint(DraggingPiece.WorldLocation), false);
                        DraggingPiece.IsSet = false;
                    }
                }
                else
                {   // Ako je nesto attachovano za mis, proverimo da li moze da se postavi na trenutnu poziciju i ako moze, detachujemo ga od misa
                    Point bottomRight = new Point(DraggingPiece.WorldLocation.X + (DraggingPiece.PieceWidth + 1) * CellSize, DraggingPiece.WorldLocation.Y + (DraggingPiece.PieceHeight + 1) * CellSize);

                    if (canBePlaced(DraggingPiece)) // Da li uopste mozemo da postavimo
                    {
                        DraggingPiece.setTexture(false);
                        setButtonsEnabled(true);
                        if (matrixCheck(DraggingPiece, locationToGridPoint(DraggingPiece.WorldLocation)) == true) // Za svaki slucaj proveravamo da li je negde u matrici vec zauzeta ta lokacija
                        {
                            DraggingPiece.IsSet = true; //Ako nije, setujemo taj PuzzlePiece
                            updateMatrix(DraggingPiece, locationToGridPoint(DraggingPiece.WorldLocation), true); // Postavimo true u matrici
                            if (isFinished() && gameTimer.Enabled == true && GameCompleted == false)    // I proverimo da li je igra zavrsena
                            {
                                gameTimer.Enabled = false;
                                GameCompleted = true;
                                MessageBox.Show("You solved the puzzle, congratulations");
                            }
                        }

                        if (btnReset.Enabled == false || btnDemo.Enabled == false)
                        {
                            btnReset.Enabled = true;
                            btnDemo.Enabled = true;
                        }
                        DraggingPiece = null;
                    }
                }
            }
            catch (Exception)
            {

                if (btnReset.Enabled == false || btnDemo.Enabled == false)
                {
                    btnReset.Enabled = true;
                    btnDemo.Enabled = true;
                }
                DraggingPiece = null;
            }
        }

        //sprecava prekrivanje New Game, Reset... dugmica
        private void bringButtonsToFront()
        {
            btnNewGame.BringToFront();
            btnReset.BringToFront();
            btnDemo.BringToFront();
            btnClose.BringToFront();
            lblTimer.BringToFront();
            gbDebug.BringToFront();
            btnToggleFullScreen.BringToFront();
        }

        private void setButtonsEnabled(bool enabled)
        {
            btnNewGame.Enabled = enabled;
            btnReset.Enabled = enabled;
            btnDemo.Enabled = enabled;
            btnClose.Enabled = enabled;
            lblTimer.Enabled = enabled;
            gbDebug.Enabled = enabled;
            btnToggleFullScreen.Enabled = enabled;
        }
        public bool isFinished()
        {
            foreach(bool b in GridStateMatrix)
                if (b == false)
                    return false;

            return true;
        }

        // proverava da li P moze da se postavi na zadate
        // koordinate(nema preklapanja sa vec zauzetim poljem)
        private bool matrixCheck(PuzzlePiece P, Point startCoords)
        {
            for (int i = 0; i < P.PieceArray.GetLength(0); i++)
            {
                for (int j = 0; j < P.PieceArray.GetLength(1); j++)
                {
                    if (P.PieceArray[i, j] == true && GridStateMatrix[i + startCoords.Y, j + startCoords.X] == true)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // ako moze da se postavi;
        // setuje/unsetuje polja u glavnoj matrici u odnosu na zadati PuzzlePiece
        private void updateMatrix(PuzzlePiece P, Point startCoords, bool isSet)
        {
            for (int i = 0; i < P.PieceArray.GetLength(0); i++)
            {
                for (int j = 0; j < P.PieceArray.GetLength(1); j++)
                {
                    if (P.PieceArray[i, j] == true)
                    {
                        GridStateMatrix[i + startCoords.Y, j + startCoords.X] = isSet;
                    }
                }
            }
            if (isSet == true)
                P.SetPosition = new Point(startCoords.X, startCoords.Y);
            else
                P.SetPosition = new Point(-1, -1);
            return;
        }

        // vraca PuzzlePiece koji sadrzi kliknuti pictureBox
        private PuzzlePiece findPicBoxParrent(PictureBox pb)
        {
            PuzzlePiece result = null;
            // Trazimo puzzlePiece kome pripada taj pictureBox
            foreach(PuzzlePiece puzzle in PuzzlePieceList)
                if (puzzle.pbList.Contains(pb))
                    return puzzle;

            return result;
        }

        // startuje demo
        private void btnDemo_Click(object sender, EventArgs e)
        {
            initGame(5, 8);
            btnDemo.Enabled = false;
            char[,] pieceMatrix = new char[11, 17];
            String S = "******************0*0*0|0*0|0|0|0**-*-*-|-*****-|-**0*0*0|0*0|0*0|0**-*****-**********0|0|0*0*0*0|0|0********-*-*-******0|0*0*0*0*0*0|0**-***-***-********0*0|0|0*0|0*0|0******************";
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    pieceMatrix[i, j] = S[i * 17 + j];
                }
            }
            PuzzlePieceList = Functions.matrixToPuzzlePieceArray(pieceMatrix, CellSize);
            generatePiecesFromList(PuzzlePieceList);
            bringButtonsToFront();
            Functions.SetDoubleBuffered(pbGrid);
            drawGrid(pbGrid);
            btnReset_Click(sender, e);
            btnReset.Enabled = true;
        }

        public MouseEventArgs MEA;  // Cuva info o poziciji misa
        //Pomera selektovani deo puzle
        public void MouseHasMoved(object sender, MouseEventArgs e)
        {
            if (DraggingPiece != null) // Ako ima nesto attachovano, pomerimo ga za onoliko koliko se mis pomerio
            {
                Point newLocation = new Point(e.X - (int)((int)(RelativeLocation.X / CellSize) * CellSize), e.Y - (int)((int)(RelativeLocation.Y / CellSize) * CellSize));

                Point location = locationToGridPoint(newLocation);

                newLocation = new Point(e.X - RelativeLocation.X, e.Y - RelativeLocation.Y);
                // provera da snapuje samo unutar grida, ne van njega
                if (location.X >= 0 && location.X <= GridCellCountX - DraggingPiece.PieceWidth - 1
                 && location.Y >= 0 && location.Y <= GridCellCountY - DraggingPiece.PieceHeight - 1) // osim ako nije unutar grida, onda ga snapujemo u svoju poziciju
                {
                    newLocation = new Point((int)(location.X * CellSize) + GridTopLeftCorner.X, (int)(location.Y * CellSize) + GridTopLeftCorner.Y);
                }
                DraggingPiece.setWorldLocation(newLocation);
            }
            // Iscrtavamo grid
            drawGrid(pbGrid);
        }

        private void PuzzleForm_MouseMove(object sender, MouseEventArgs e)
        {
            MEA = e;
        }

        private void PuzzleForm_Load(object sender, EventArgs e)
        {
            PuzzleForm_Resize(sender, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnReset_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, e.X + btnReset.Location.X, e.Y + btnReset.Location.Y, e.Delta);
            PuzzleForm_MouseMove(sender, mea);
        }

        private void pbGrid_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, e.X + pbGrid.Location.X, e.Y + pbGrid.Location.Y, e.Delta);
            PuzzleForm_MouseMove(sender, mea);
        }

        private void pb_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, e.X + pb.Location.X, e.Y + pb.Location.Y, e.Delta);
            PuzzleForm_MouseMove(sender, mea);
        }

        private void btnNewGame_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, e.X + btnNewGame.Location.X, e.Y + btnNewGame.Location.Y, e.Delta);
            PuzzleForm_MouseMove(sender, mea);
        }

        private void graphicsTimer_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = Convert.ToString(Convert.ToInt32(lblTimer.Text) + 1);
            MouseHasMoved(new Object(), MEA);
            
                
            lblGridSize.Text = "GridSize: " + pbGrid.Size.ToString();
            lblGridLocation.Text = "GridLoc: " + pbGrid.Location.ToString();
            lblCellSize.Text = "CellSize: " + CellSize.ToString();
            lblMouseLocation.Text = "MouseLoc: " + (new Point(Cursor.Position.X - this.Location.X, Cursor.Position.Y - this.Location.Y)).ToString();
        }

        // reagovanje na promenu velicine prozora tj posto je igra samo full screen
        // onda ovo predstavlja prilagodjavanje za razlicite rezolucije
        private void PuzzleForm_Resize(object sender, EventArgs e)
        {
            float divider = 2.5f;
            // Podesavanje lokacija dugmica
            btnClose.Location = new Point(this.Size.Width - 25 - btnClose.Width, btnClose.Location.Y);
            gbDebug.Location = new Point(this.Size.Width - 20 - gbDebug.Width, 50);
            btnToggleFullScreen.Location = new Point(btnClose.Location.X - 10 - btnToggleFullScreen.Width, btnClose.Location.Y);

            if (GameStarted)    //Ako je pokrenuta igra, izracunamo velicine pictureBoxova, table, i ponovo ih iscrtamo
            {
                pbGrid.Size = new Size((int)(this.Width / divider), (int)((this.Height - 37) / divider));
                int maxGridSize = GridCellCountX > GridCellCountY ? GridCellCountX : GridCellCountY;
                //iz nekog razloga je visina prozora za 37 pixela veca od realne visine prozora?
                {
                    int a = pbGrid.Size.Width / GridCellCountX;
                    int b = pbGrid.Size.Height / GridCellCountY;
                    CellSize = a < b ? a : b;
                }
                pbGrid.Size = new Size(CellSize * GridCellCountX, CellSize * GridCellCountY);
                pbGrid.Location = new Point(this.Width / 2 - pbGrid.Width / 2, (this.Height - 37) / 2 - pbGrid.Height / 2);

                GridTopLeftCorner.X = pbGrid.Location.X;
                GridBottomRightCorner.X = pbGrid.Location.X + pbGrid.Width;
                GridTopLeftCorner.Y = pbGrid.Location.Y;
                GridBottomRightCorner.Y = pbGrid.Location.Y + pbGrid.Height;

                drawGrid(pbGrid);

                if (PuzzlePieceList != null)
                {
                    float ResizeAmmountWidth, ResizeAmmountHeight;
                    ResizeAmmountHeight = (float)PreviousFormSize.Height / (float)this.Height;
                    ResizeAmmountWidth = (float)PreviousFormSize.Width / (float)this.Width;

                    foreach (PuzzlePiece p in PuzzlePieceList)
                        p.resizePiece(CellSize, ResizeAmmountWidth, ResizeAmmountHeight, pbGrid.Location);
                }
                PreviousFormSize = this.Size;
            }
        }

        private void btnToggleFullScreen_Click(object sender, EventArgs e)
        {
            if(btnToggleFullScreen.Text == "FullScreen")
            {
                btnToggleFullScreen.Text = "Windowed";
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                btnToggleFullScreen.Text = "FullScreen";
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
            }
        }
    }
}
