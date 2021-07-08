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
using System.IO;

namespace YoketoruVS21
{
    public partial class Form1 : Form
    {
        const bool isDebug = true;

        const int SpeedMax = 20;
        const int starttime = 300;


        const int PlayerMax = 1;
        const int EnemyMaex = 3;
        const int ItemMax = 10;
        const int ChrMax = PlayerMax + EnemyMaex + ItemMax;

        Label[] chrs = new Label[ChrMax];
        int[] vx = new int[ChrMax];
        int[] vy = new int[ChrMax];

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

        int itemCount = 10;
        int time = starttime + 1;
        int hiscore = 0;
        

        public Form1()
        {
            InitializeComponent();

            if(File.Exists("hisc.txt"))
            {
                string hi = File.ReadAllText("hisc.txt");
                string trimhi = hi.Trim();//空白や改行を消す
                int fhi;
                if(int.TryParse(trimhi, out fhi))
                {
                    hiscore = fhi;
                }
            }

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
                hiLabel.Text = "Highscore" + hiscore;
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
            int state = 0;
            currentState = nextState;
            nextState = State.None;

            switch (currentState)
            {
                case State.Title:
                    for (int i = 0; i > ChrMax;i++)
                        chrs[0].Visible = false;
                    titleLabel.Visible = true;
                    itemLabel.Visible = true;
                    copyrightLabel.Visible = true;
                    hiLabel.Visible = true;
                    gameoverLabel.Visible = false;
                    TitleButton.Visible = false;
                    clearLabel.Visible = false;
                    break;

                case State.Game:
                    if(state == 0)
                    {
                        for(int i = EnemyIndex;i < ChrMax;i++)
                        {
                            chrs[i].Visible = true;
                            itemCount = 10;
                            itemLabel.Text = "∇:" + itemCount;
                            time = starttime + 1;
                            state = 1;
                        }
                    }
                    titleLabel.Visible = false;
                    startButton.Visible = false;
                    copyrightLabel.Visible = false;
                    hiLabel.Visible = false;

                    for(int i = EnemyIndex; i < ChrMax; i++)
                    {
                        chrs[i].Left = rand.Next(ClientSize.Width - chrs[i].Width);
                        chrs[i].Top = rand.Next(ClientSize.Height - chrs[i].Height);
                        vx[i] = rand.Next(-SpeedMax, SpeedMax + 1);
                        vy[i] = rand.Next(-SpeedMax, SpeedMax + 1);
                    }
                    break;

                case State.Clear:
                    //MessageBox.Show("GameOver");
                    clearLabel.Visible = true;
                    TitleButton.Visible = true;
                    startButton.Visible = true;
                    if(time > hiscore)
                    {
                        hiscore = time;
                        hiLabel.Text = $"HighScore {hiscore}";

                        File.WriteAllText("hisc.txt", $"{hiscore}\n");
                    }
                    state = 0;
                    break;

                case State.GameOver:
                    //MessageBox.Show("Clear");
                    gameoverLabel.Visible = true;
                    TitleButton.Visible = true;
                    startButton.Visible = true;
                    state = 0;
                    break;
            }
        }

        

        void UpdateGame()
        {
            time--;
            TimeLabel.Text = "Time" + time;
            if (time <= 0)
                nextState = State.GameOver;

            Point mp = PointToClient(MousePosition);
            // TODO: mpがプレイヤーの中心になるように設定
            chrs[0].Left = mp.X - chrs[0].Width / 2;
            chrs[0].Top = mp.Y - 30;

            for (int i = EnemyIndex; i < ChrMax; i++)
            {

                if (!chrs[i].Visible) continue;

                chrs[i].Top += vy[i];
                chrs[i].Left += vx[i];
                if (chrs[i].Left < 0)
                    vx[i] = Math.Abs(vx[i]);
                if (chrs[i].Top < 0)
                    vy[i] = Math.Abs(vy[i]);
                if (chrs[i].Right > ClientSize.Width)
                    vx[i] = -Math.Abs(vx[i]);
                if (chrs[i].Bottom > ClientSize.Height)
                    vy[i] = -Math.Abs(vy[i]);
                
                
                if (  (mp.X >= chrs[i].Left)
                    &&(mp.X < chrs[i].Right)
                    &&(mp.Y >= chrs[i].Left)
                    &&(mp.Y < chrs[i].Right))
                    //MessageBox.Show("あたった!!");
                    if (i < ItemIndex)
                    {
                        nextState = State.GameOver;
                    }
                    else
                    {
                        chrs[i].Visible = false;
                        itemCount--;
                        itemLabel.Text = "∇:" + itemCount;
                        if (itemCount <= 0)
                            nextState = State.Clear;
                    }
            }
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
