namespace SimplePuzzleGame
{
    partial class PuzzleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnNewGame = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.pbGrid = new System.Windows.Forms.PictureBox();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.lblTimer = new System.Windows.Forms.Label();
            this.btnDemo = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.gbDebug = new System.Windows.Forms.GroupBox();
            this.lblDragging = new System.Windows.Forms.Label();
            this.lblWorldLocation = new System.Windows.Forms.Label();
            this.lblRelativeLocation = new System.Windows.Forms.Label();
            this.lblMouseLocation = new System.Windows.Forms.Label();
            this.lblCellSize = new System.Windows.Forms.Label();
            this.lblGridLocation = new System.Windows.Forms.Label();
            this.lblGridSize = new System.Windows.Forms.Label();
            this.btnToggleFullScreen = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbGrid)).BeginInit();
            this.gbDebug.SuspendLayout();
            this.SuspendLayout();
            //
            // btnNewGame
            //
            this.btnNewGame.BackColor = System.Drawing.Color.DarkGray;
            this.btnNewGame.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNewGame.Location = new System.Drawing.Point(13, 13);
            this.btnNewGame.Name = "btnNewGame";
            this.btnNewGame.Size = new System.Drawing.Size(75, 23);
            this.btnNewGame.TabIndex = 0;
            this.btnNewGame.Text = "Custom";
            this.btnNewGame.UseVisualStyleBackColor = false;
            this.btnNewGame.Click += new System.EventHandler(this.btnNewGame_Click);
            this.btnNewGame.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnNewGame_MouseMove);
            //
            // btnReset
            //
            this.btnReset.BackColor = System.Drawing.Color.DarkGray;
            this.btnReset.Enabled = false;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnReset.Location = new System.Drawing.Point(94, 13);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            this.btnReset.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnReset_MouseMove);
            //
            // pbGrid
            //
            this.pbGrid.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pbGrid.Location = new System.Drawing.Point(13, 42);
            this.pbGrid.Name = "pbGrid";
            this.pbGrid.Size = new System.Drawing.Size(156, 62);
            this.pbGrid.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbGrid.TabIndex = 2;
            this.pbGrid.TabStop = false;
            this.pbGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbGrid_MouseMove);
            //
            // gameTimer
            //
            this.gameTimer.Interval = 20;
            this.gameTimer.Tick += new System.EventHandler(this.graphicsTimer_Tick);
            //
            // lblTimer
            //
            this.lblTimer.AutoSize = true;
            this.lblTimer.Location = new System.Drawing.Point(13, 42);
            this.lblTimer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(13, 13);
            this.lblTimer.TabIndex = 3;
            this.lblTimer.Text = "0";
            this.lblTimer.Visible = false;
            //
            // btnDemo
            //
            this.btnDemo.BackColor = System.Drawing.Color.DarkGray;
            this.btnDemo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDemo.Location = new System.Drawing.Point(175, 13);
            this.btnDemo.Name = "btnDemo";
            this.btnDemo.Size = new System.Drawing.Size(74, 23);
            this.btnDemo.TabIndex = 4;
            this.btnDemo.Text = "Demo";
            this.btnDemo.UseVisualStyleBackColor = false;
            this.btnDemo.Click += new System.EventHandler(this.btnDemo_Click);
            //
            // btnClose
            //
            this.btnClose.BackColor = System.Drawing.Color.DarkGray;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClose.Location = new System.Drawing.Point(832, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(103, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // gbDebug
            //
            this.gbDebug.Controls.Add(this.lblDragging);
            this.gbDebug.Controls.Add(this.lblWorldLocation);
            this.gbDebug.Controls.Add(this.lblRelativeLocation);
            this.gbDebug.Controls.Add(this.lblMouseLocation);
            this.gbDebug.Controls.Add(this.lblCellSize);
            this.gbDebug.Controls.Add(this.lblGridLocation);
            this.gbDebug.Controls.Add(this.lblGridSize);
            this.gbDebug.Location = new System.Drawing.Point(616, 68);
            this.gbDebug.Name = "gbDebug";
            this.gbDebug.Size = new System.Drawing.Size(200, 131);
            this.gbDebug.TabIndex = 6;
            this.gbDebug.TabStop = false;
            this.gbDebug.Text = "debugging";
            this.gbDebug.Visible = false;
            //
            // lblDragging
            //
            this.lblDragging.AutoSize = true;
            this.lblDragging.Location = new System.Drawing.Point(6, 101);
            this.lblDragging.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDragging.Name = "lblDragging";
            this.lblDragging.Size = new System.Drawing.Size(35, 13);
            this.lblDragging.TabIndex = 6;
            this.lblDragging.Text = "label1";
            //
            // lblWorldLocation
            //
            this.lblWorldLocation.AutoSize = true;
            this.lblWorldLocation.Location = new System.Drawing.Point(7, 74);
            this.lblWorldLocation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWorldLocation.Name = "lblWorldLocation";
            this.lblWorldLocation.Size = new System.Drawing.Size(86, 13);
            this.lblWorldLocation.TabIndex = 5;
            this.lblWorldLocation.Text = "lblWorldLocation";
            //
            // lblRelativeLocation
            //
            this.lblRelativeLocation.AutoSize = true;
            this.lblRelativeLocation.Location = new System.Drawing.Point(6, 88);
            this.lblRelativeLocation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRelativeLocation.Name = "lblRelativeLocation";
            this.lblRelativeLocation.Size = new System.Drawing.Size(97, 13);
            this.lblRelativeLocation.TabIndex = 4;
            this.lblRelativeLocation.Text = "lblRelativeLocation";
            //
            // lblMouseLocation
            //
            this.lblMouseLocation.AutoSize = true;
            this.lblMouseLocation.Location = new System.Drawing.Point(6, 59);
            this.lblMouseLocation.Name = "lblMouseLocation";
            this.lblMouseLocation.Size = new System.Drawing.Size(57, 13);
            this.lblMouseLocation.TabIndex = 3;
            this.lblMouseLocation.Text = "MouseLoc";
            //
            // lblCellSize
            //
            this.lblCellSize.AutoSize = true;
            this.lblCellSize.Location = new System.Drawing.Point(6, 46);
            this.lblCellSize.Name = "lblCellSize";
            this.lblCellSize.Size = new System.Drawing.Size(35, 13);
            this.lblCellSize.TabIndex = 2;
            this.lblCellSize.Text = "label3";
            //
            // lblGridLocation
            //
            this.lblGridLocation.AutoSize = true;
            this.lblGridLocation.Location = new System.Drawing.Point(6, 33);
            this.lblGridLocation.Name = "lblGridLocation";
            this.lblGridLocation.Size = new System.Drawing.Size(35, 13);
            this.lblGridLocation.TabIndex = 1;
            this.lblGridLocation.Text = "label2";
            //
            // lblGridSize
            //
            this.lblGridSize.AutoSize = true;
            this.lblGridSize.Location = new System.Drawing.Point(7, 20);
            this.lblGridSize.Name = "lblGridSize";
            this.lblGridSize.Size = new System.Drawing.Size(35, 13);
            this.lblGridSize.TabIndex = 0;
            this.lblGridSize.Text = "label1";
            //
            // btnToggleFullScreen
            //
            this.btnToggleFullScreen.BackColor = System.Drawing.Color.DarkGray;
            this.btnToggleFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnToggleFullScreen.Location = new System.Drawing.Point(723, 13);
            this.btnToggleFullScreen.Name = "btnToggleFullScreen";
            this.btnToggleFullScreen.Size = new System.Drawing.Size(103, 23);
            this.btnToggleFullScreen.TabIndex = 7;
            this.btnToggleFullScreen.Text = "FullScreen";
            this.btnToggleFullScreen.UseVisualStyleBackColor = false;
            this.btnToggleFullScreen.Click += new System.EventHandler(this.btnToggleFullScreen_Click);
            //
            // PuzzleForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(944, 561);
            this.Controls.Add(this.btnToggleFullScreen);
            this.Controls.Add(this.gbDebug);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDemo);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnNewGame);
            this.Controls.Add(this.pbGrid);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(639, 358);
            this.Name = "PuzzleForm";
            this.Text = "SimplePuzzleGame, Bojan Vasiljevic";
            this.Load += new System.EventHandler(this.PuzzleForm_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PuzzleForm_MouseMove);
            this.Resize += new System.EventHandler(this.PuzzleForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pbGrid)).EndInit();
            this.gbDebug.ResumeLayout(false);
            this.gbDebug.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNewGame;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.PictureBox pbGrid;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnDemo;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gbDebug;
        private System.Windows.Forms.Label lblCellSize;
        private System.Windows.Forms.Label lblGridLocation;
        private System.Windows.Forms.Label lblGridSize;
        private System.Windows.Forms.Label lblMouseLocation;
        private System.Windows.Forms.Label lblWorldLocation;
        private System.Windows.Forms.Label lblRelativeLocation;
        private System.Windows.Forms.Label lblDragging;
        private System.Windows.Forms.Button btnToggleFullScreen;
    }
}
