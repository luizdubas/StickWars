using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Noesis;

public class TicTacToe : MonoBehaviour 
{
    FrameworkElement mRoot;
    FrameworkElement mBoardPanel;
    TextBlock mStatusText;

    TextBlock mScorePlayer1Text;
    TextBlock mScorePlayer2Text;
    TextBlock mScoreTiesText;
    TextBlock mScoreText;
    
    Storyboard mWinAnimation;
    Storyboard mTieAnimation;
    Storyboard mResetAnimation;
    Storyboard mProgressAnimation;
    Storyboard mProgressFadeAnimation;
    Storyboard mScoreHalfAnimation;
    Storyboard mScoreEndAnimation;
    Storyboard mStatusHalfAnimation;
    Storyboard mStatusEndAnimation;
    
    DependencyObject mWinAnim0;
    DependencyObject mWinAnim1;
    DependencyObject mWinAnim2;
    
    DependencyObject mScoreHalfAnim0;
    
    DependencyObject mScoreEndAnim0;
    DependencyObject mScoreEndAnim1;
    DependencyObject mScoreEndAnim2;
    
    string mStatusMsg;

    uint mScorePlayer1;
    uint mScorePlayer2;
    uint mScoreTies;
    uint mScore;

    enum Player
    {
        Player_None,
        Player_1,
        Player_2
    };

    Player mPlayer;
    Player mLastStarterPlayer;
    
    public class Cell : Noesis.BaseComponent
    {
        private HandleRef swigCPtr;
        
        public string player
        {
            set { mPlayer = value; }
            get { return mPlayer; }
        }
        private string mPlayer;
        
        public ToggleButton btn
        {
            set { mBtn = value; }
            get { return mBtn; }
        }
        private ToggleButton mBtn;
        
        public static void Register()
        {
        }
    
        public Cell(IntPtr cPtr, bool cMemoryOwn)
            : base(cPtr, cMemoryOwn)
        {
            swigCPtr = new HandleRef(this, cPtr);
        }
    
        public Cell()
            : this(Noesis.Extend.New(typeof(Cell)), true)
        {
            Noesis.Extend.Register(typeof(Cell), swigCPtr.Handle, this);
        }
    
        public override void Dispose()
        {
            lock (this)
            {
                if (swigCPtr.Handle != IntPtr.Zero)
                {
                    if (swigCMemOwn)
                    {
                        swigCMemOwn = false;
                        if (Noesis.Kernel.IsInitialized())
                        {
                            Noesis.Extend.Delete(typeof(Cell), swigCPtr.Handle);
                        }
                    }
                    swigCPtr = new HandleRef(null, IntPtr.Zero);
                }
                GC.SuppressFinalize(this);
                base.Dispose();
            }
        }
    };
    
    private Cell [][] mBoard;    
    
    // Use this for initialization
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void Start () 
    {
        // Access to the NoesisGUIPanel component
        NoesisGUIPanel noesisGUI = GetComponent<NoesisGUIPanel>();
        
        this.mRoot = noesisGUI.GetRoot<FrameworkElement>();

        this.mBoardPanel = this.mRoot.FindName<FrameworkElement>("Board");
        this.mBoardPanel.MouseLeftButtonDown += this.BoardClicked;
        
        this.mBoard = new Cell [][]
        {
            new Cell[]
            {
                new Cell(),
                new Cell(),
                new Cell()
            },
            new Cell[]
            {
                new Cell(),
                new Cell(),
                new Cell()
            },
            new Cell[]
            {
                new Cell(),
                new Cell(),
                new Cell()
            }
        };
        
        for (int row = 0; row < 3; ++row)
        {
            for (int col = 0; col < 3; ++col)
            {
                string cellName = String.Format("Cell{0}{1}", row, col);
    
                this.mBoard[row][col].btn = this.mRoot.FindName<ToggleButton>(cellName);
                this.mBoard[row][col].btn.SetIsEnabled(false);
                this.mBoard[row][col].btn.SetTag (this.mBoard[row][col]);
                this.mBoard[row][col].btn.Checked += this.CellChecked;
            }
        }
    
        this.mStatusText = this.mRoot.FindName<TextBlock>("StatusText");
        this.mScorePlayer1Text = this.mRoot.FindName<TextBlock>("ScorePlayer1");
        this.mScorePlayer2Text = this.mRoot.FindName<TextBlock>("ScorePlayer2");    
        this.mScoreTiesText = this.mRoot.FindName<TextBlock>("ScoreTies");    
        this.mScoreText = null;
        
        this.mWinAnimation = this.mRoot.GetResources().FindName("WinAnim").As<Storyboard>();
        this.mWinAnimation.Completed += this.WinAnimationCompleted;
        
        this.mWinAnim0 = this.mRoot.FindName<DependencyObject>("WinAnim0");
        this.mWinAnim1 = this.mRoot.FindName<DependencyObject>("WinAnim1");
        this.mWinAnim2 = this.mRoot.FindName<DependencyObject>("WinAnim2");
    
        this.mTieAnimation = this.mRoot.GetResources().FindName("TieAnim").As<Storyboard>();
        this.mTieAnimation.Completed += this.TieAnimationCompleted;
    
        this.mResetAnimation = this.mRoot.GetResources().FindName("ResetAnim").As<Storyboard>();
        this.mResetAnimation.Completed += this.ResetAnimationCompleted;
    
        this.mProgressAnimation = this.mRoot.GetResources().FindName("ProgressAnim").As<Storyboard>();
        
        this.mProgressFadeAnimation = this.mRoot.GetResources().FindName("ProgressFadeAnim").As<Storyboard>();
        this.mProgressFadeAnimation.Completed += this.ProgressFadeAnimationCompleted;
    
        this.mScoreHalfAnimation = this.mRoot.GetResources().FindName("ScoreHalfAnim").As<Storyboard>();
        this.mScoreHalfAnimation.Completed += this.ScoreHalfAnimationCompleted;
        
        this.mScoreHalfAnim0 = this.mRoot.FindName<DependencyObject>("ScoreHalfAnim0");
    
        this.mScoreEndAnimation = this.mRoot.GetResources().FindName("ScoreEndAnim").As<Storyboard>();
        
        this.mScoreEndAnim0 = this.mRoot.FindName<DependencyObject>("ScoreEndAnim0");
        this.mScoreEndAnim1 = this.mRoot.FindName<DependencyObject>("ScoreEndAnim1");
        this.mScoreEndAnim2 = this.mRoot.FindName<DependencyObject>("ScoreEndAnim2");
        
        this.mStatusHalfAnimation = this.mRoot.GetResources().FindName("StatusHalfAnim").As<Storyboard>();
        this.mStatusHalfAnimation.Completed += this.StatusHalfAnimationCompleted;
    
        this.mStatusEndAnimation = this.mRoot.GetResources().FindName("StatusEndAnim").As<Storyboard>();
    
        this.mStatusText.SetText("Player 1 Turn");
        this.mPlayer = Player.Player_1;
        this.mLastStarterPlayer = Player.Player_1;
        this.mScorePlayer1 = 0;
        this.mScorePlayer2 = 0;
        this.mScoreTies = 0;
        this.mScore = 0;
    
        this.StartGame();
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Update is called once per frame
    void Update () 
    {    
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void BoardClicked(BaseComponent sender, MouseButtonEventArgs e)
    {            
        if (this.mPlayer == Player.Player_None)
        {
            if (this.mLastStarterPlayer == Player.Player_1)
            {
                this.mPlayer = Player.Player_2;
                this.mLastStarterPlayer = Player.Player_2;
                this.mStatusMsg = "Player 2 Turn";
            }
            else
            {
                this.mPlayer = Player.Player_1;
                this.mLastStarterPlayer = Player.Player_1;
                this.mStatusMsg = "Player 1 Turn";
            }
    
            this.mResetAnimation.Begin(this.mRoot);
            this.mStatusHalfAnimation.Begin(this.mRoot);
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void CellChecked(BaseComponent sender, RoutedEventArgs e)
    {
        FrameworkElement fe = sender.As<FrameworkElement>();
        Cell cell = fe.GetTag().As<Cell>();
    
        this.MarkCell(cell);
    
        string winPlay = "";
        if (this.HasWon(ref winPlay))
        {
            this.WinGame(winPlay);
        }
        else if (this.HasTied())
        {
            this.TieGame();
        }
        else
        {
            this.SwitchPlayer();
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void WinAnimationCompleted(BaseComponent arg0, TimelineEventArgs arg1)
    {
        if (this.mPlayer == Player.Player_1)
        {
            this.mScoreText = this.mScorePlayer1Text;
            this.mScore = ++this.mScorePlayer1;
            this.UpdateScoreAnimation("ScorePlayer1");
        }
        else
        {
            this.mScoreText = this.mScorePlayer2Text;
            this.mScore = ++this.mScorePlayer2;
            this.UpdateScoreAnimation("ScorePlayer2");
        }
    
        this.mPlayer = Player.Player_None;
    
        this.mScoreHalfAnimation.Begin(this.mRoot);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void TieAnimationCompleted(BaseComponent arg0, TimelineEventArgs arg1)
    {
        this.mScoreText = this.mScoreTiesText;
        this.mScore = ++this.mScoreTies;
        this.UpdateScoreAnimation("ScoreTies");
    
        this.mPlayer = Player.Player_None;
    
        mScoreHalfAnimation.Begin(this.mRoot);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void ResetAnimationCompleted(BaseComponent arg0, TimelineEventArgs arg1)
    {
        this.StartGame();
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void ProgressFadeAnimationCompleted(BaseComponent sender, TimelineEventArgs args)
    {
        this.mProgressAnimation.Stop(this.mRoot);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void ScoreHalfAnimationCompleted(BaseComponent arg0, TimelineEventArgs arg1)
    {
        this.mScoreText.SetText(String.Format("{0}", this.mScore));
        mScoreEndAnimation.Begin(this.mRoot);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void StatusHalfAnimationCompleted(BaseComponent arg0, TimelineEventArgs arg1)
    {
        this.mStatusText.SetText(this.mStatusMsg);
        mStatusEndAnimation.Begin(this.mRoot);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    string GetPlayerState()
    {
        return this.mPlayer == Player.Player_1 ? "Player1" : "Player2";
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void StartGame()
    {
        string player = this.GetPlayerState();
    
        PlaneProjection projection = this.mBoardPanel.GetProjection().As<PlaneProjection>();
        projection.ClearAnimation(PlaneProjection.RotationYProperty);
        CompositeTransform t = this.mBoardPanel.GetRenderTransform().As<CompositeTransform>();
        t.ClearAnimation(CompositeTransform.ScaleXProperty);
        t.ClearAnimation(CompositeTransform.ScaleYProperty);

        for (int row = 0; row < 3; ++row)
        {
            for (int col = 0; col < 3; ++col)
            {
                mBoard[row][col].player = "";
                mBoard[row][col].btn.ClearAnimation(UIElement.OpacityProperty);
                mBoard[row][col].btn.SetIsEnabled(true);
                
                mBoard[row][col].btn.SetIsChecked(false);
                VisualStateManager.GoToState(mBoard[row][col].btn, player, false);
            }
        }
        
        this.mProgressAnimation.Begin(this.mRoot, this.mRoot, true);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void WinGame(string winPlay)
    {
        for (int row = 0; row < 3; ++row)
        {
            for (int col = 0; col < 3; ++col)
            {
                this.mBoard[row][col].btn.SetIsEnabled(false);
            }
        }
    
        this.mStatusMsg = String.Format("Player {0} Wins", this.mPlayer == Player.Player_1 ? 1 : 2);
        
        Storyboard.SetTargetName(this.mWinAnim0, winPlay);
    
        Storyboard.SetTargetName(this.mWinAnim1, winPlay);
    
        Storyboard.SetTargetName(this.mWinAnim2, winPlay);
    
        this.mProgressFadeAnimation.Begin(this.mRoot);
        this.mWinAnimation.Begin(this.mRoot);
        this.mStatusHalfAnimation.Begin(this.mRoot);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void TieGame()
    {
        this.mStatusMsg = "Game Tied";
    
        this.mProgressFadeAnimation.Begin(this.mRoot);
        this.mTieAnimation.Begin(this.mRoot);
        this.mStatusHalfAnimation.Begin(this.mRoot);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void SwitchPlayer()
    {
        if (this.mPlayer == Player.Player_1)
        {
            this.mPlayer = Player.Player_2;
            this.mStatusMsg = "Player 2 Turn";
        }
        else
        {
            this.mPlayer = Player.Player_1;
            this.mStatusMsg = "Player 1 Turn";
        }
    
        string player = this.GetPlayerState();
        for (int row = 0; row < 3; ++row)
        {
            for (int col = 0; col < 3; ++col)
            {
                TicTacToe.Cell cell = this.mBoard[row][col];
                if (cell.player == "") 
                {
                    VisualStateManager.GoToState(cell.btn, player, false);
                }
            }
        }
    
        this.mStatusHalfAnimation.Begin(this.mRoot);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void MarkCell(Cell cell)
    {
        string player = this.GetPlayerState();
    
        cell.player = player;
        cell.btn.SetIsEnabled(false);
        VisualStateManager.GoToState(cell.btn, player, false);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void UpdateScoreAnimation(string targetName)
    {
        // Score Half
        Storyboard.SetTargetName(this.mScoreHalfAnim0, targetName);
    
        // Score End
        Storyboard.SetTargetName(this.mScoreEndAnim0, targetName);
        Storyboard.SetTargetName(this.mScoreEndAnim1, targetName);
        Storyboard.SetTargetName(this.mScoreEndAnim2, targetName);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    bool HasWon(ref string winPlay)
    {
        string player = GetPlayerState();
    
        for (int i = 0; i < 3; ++i)
        {
            if (this.PlayerCheckedRow(player, i))
            {
                winPlay = String.Format("WinRow{0}", i + 1);
                return true;
            }
    
            if (this.PlayerCheckedCol(player, i))
            {
                winPlay = String.Format("WinCol{0}", i + 1);
                return true;
            }
        }
    
        if (this.PlayerCheckedDiag(player, 0, 2))
        {
            winPlay = "WinDiagLR";
            return true;
        }
        
        if (this.PlayerCheckedDiag(player, 2, 0))
        {
            winPlay = "WinDiagRL";
            return true;
        }
    
        return false;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    bool HasTied()
    {
        for (int row = 0; row < 3; ++row)
        {
            for (int col = 0; col < 3; ++col)
            {
                if (this.mBoard[row][col].player == "")
                {
                    return false;
                }
            }
        }
    
        return true;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    bool PlayerCheckedRow(string player, int row)
    {
        return this.PlayerCheckedCell(player, row, 0) && this.PlayerCheckedCell(player, row, 1) &&
            this.PlayerCheckedCell(player, row, 2);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    bool PlayerCheckedCol(string player, int col)
    {
        return this.PlayerCheckedCell(player, 0, col) && this.PlayerCheckedCell(player, 1, col) &&
            this.PlayerCheckedCell(player, 2, col);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    bool PlayerCheckedDiag(string player, int start, int end)
    {
        return this.PlayerCheckedCell(player, start, 0) && this.PlayerCheckedCell(player, 1, 1) &&
            this.PlayerCheckedCell(player, end, 2);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    bool PlayerCheckedCell(string player, int row, int col)
    {
        return this.mBoard[row][col].player == player;
    }
}
