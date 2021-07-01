using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace YoketoruVS21
{
    public partial class Form1 : Form
    {
        const bool isDebug = true;

        const int PlayerMax = 1;
        const int EnemyMaex = 3;
        const int ItemMax = 3;
        const int ChrMax = PlayerMax + EnemyMaex + ItemMax;

        Label[] chrs = new Label[ChrMax];

        const int PlayerIndex = 0;
        const int EnemyIndex = PlayerMax;
        const int ItemIndex = EnemyMaex+EnemyIndex;


        const string PlayerText = "(・_・)";
        const string EnemyText = "ж";
        const string ItemText = "∇";

        static Random rand = new Random();


        enum State
        {
            None = -1,  //無効
            Title,      //タイトル
            Game,       //ゲーム
            GameOver,   //ゲームオーバー
            Clear       //クリアー
        }
        State currentState = State.None;
        State nextState = State.Title;

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        public Form1()
        {
            InitializeComponent();


            for (int i = 0; i < ChrMax; i++)
            {
                chrs[i] = new Label();
                chrs[i].AutoSize = true;
                if (i == PlayerIndex)
                {
                    chrs[i].Text = PlayerText;
                }
                else if (i < ItemIndex)
                {
                    chrs[i].Text = EnemyText;
                }
                else
                {
                    chrs[i].Text = ItemText;
                }
                chrs[i].Font = tempLabel.Font;
                Controls.Add(chrs[i]);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isDebug)
            {
                if(GetAsyncKeyState((int)Keys.O) < 0)
                {
                    nextState = State.GameOver;
                }
                else if (GetAsyncKeyState((int)Keys.C) < 0)
                {
                    nextState = State.Clear;
                }


                if (nextState != State.None)
                {
                    initProc();
                }
            }
            if (currentState == State.Game)
            {
                UpdateGame();
            }

        }

        void initProc()
        {
            currentState = nextState;
            nextState = State.None;

            switch (currentState)
            {
                case State.Title:
                    titleLabel.Visible = true;
                    starLabel.Visible = true;
                    copyrightLabel.Visible = true;
                    hiLabel.Visible = true;
                    gameoverLabel.Visible = false;
                    TitleButton.Visible = false;
                    clearLabel.Visible = false;
                    break;

                case State.Game:
                    titleLabel.Visible = false;
                    startButton.Visible = false;
                    copyrightLabel.Visible = false;
                    hiLabel.Visible = false;

                    for(int i = EnemyIndex; i < ChrMax; i++)
                    {
                        chrs[i].Left = rand.Next(ClientSize.Width - chrs[i].Width);
                        chrs[i].Top = rand.Next(ClientSize.Height - chrs[i].Height);
                    }
                    break;

                case State.Clear:
                    //MessageBox.Show("GameOver");
                    clearLabel.Visible = true;
                    TitleButton.Visible = true;
                    startButton.Visible = true;
                    break;

                case State.GameOver:
                    //MessageBox.Show("Clear");
                    gameoverLabel.Visible = true;
                    TitleButton.Visible = true;
                    startButton.Visible = true;
                    break;
            }
        }

        void UpdateGame()
        {

            Point mp = PointToClient(MousePosition);

            // TODO: mpがプレイヤーの中心になるように設定
            Point spos = MousePosition;
            Point fpos = PointToClient(spos);
            chrs[0].Left = spos.X;
            chrs[0].Top = spos.Y;
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            nextState = State.Game;
        }

        private void TitleButton_Click(object sender, EventArgs e)
        {
            nextState = State.Title;
        }
    }
}
