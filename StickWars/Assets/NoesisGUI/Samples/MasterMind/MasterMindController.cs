using UnityEngine;
using System;
using System.Collections;
using Noesis;

public class MasterMindController : MonoBehaviour 
{
	private static int ColumnsCount = 4;
	private static int ColorsCount = 8;
	private static int RowsCount = 10;
	
	private FrameworkElement mRoot;
	private Grid [] mGrids;
	private Border [] mBorders;
	private Button [] mChkButtons;
	private TextBlock mStatus;
	private RadioButton [] mColors;
	private Ellipse [] mSolutions;
	private TextBlock [] mSolutionTexts;
	private Button mRestartGame;
	
	private static string [] ColorRBNames = new string []
	{
		"RedRB",
		"BlueRB",
		"GreenRB",
		"YellowRB",
		"BrownRB",
		"PurpleRB",
		"OrangeRB",
		"PinkRB"
	};
	
	private class Row
	{
		public Row()
		{
			mStatus = new int[MasterMindController.ColumnsCount];
			for (int i = 0; i < MasterMindController.ColumnsCount; ++i)
			{
				mStatus[i] = -1;
			}
			
			mBtn = new ToggleButton[MasterMindController.ColumnsCount];
			
			mSol = new Ellipse[MasterMindController.ColumnsCount];
		}
		
		public int [] mStatus;
		public ToggleButton [] mBtn;
		public Ellipse [] mSol;
	};
	
	private Row [] mRows;
	
	private int mCurrentRow;
	
	private int mCurrentColor;
	
	private Brush mBlackCircleBrush;
	private Brush mWhiteCircleBrush;
	private Brush mBlackSolidBrush;
	private Brush mSolutionBrush;
	
	private int [] mColorKey;
	
	private System.Random mRandom;
		
	////////////////////////////////////////////////////////////////////////////////////////////
	// Use this for initialization
	void Start () 
	{
		// Access to the NoesisGUI component
        NoesisGUIPanel noesisGUI = GetComponent<NoesisGUIPanel>();
		
        // Obtain the root of the loaded UI resource, in this case it is a Grid element
        this.mRoot = noesisGUI.GetRoot<FrameworkElement>();
		
		// Grids & Borders
		this.mGrids = new Grid [MasterMindController.RowsCount];
		this.mBorders = new Border [MasterMindController.RowsCount];
		for (int i = 0; i < MasterMindController.RowsCount; ++i)
		{
			this.mGrids[i] = this.mRoot.FindName<Grid>(String.Format("Bd{0}", i));
			if (this.mGrids[i] == null)
			{
				Debug.LogError("Grid " + i.ToString() + " not found");
			}
			this.mBorders[i] = this.mRoot.FindName<Border>(String.Format("Bg{0}", i));
			if (this.mBorders[i] == null)
			{
				Debug.LogError("Border " + i.ToString() + " not found");
			}
		}
		
		// Check buttons
		this.mChkButtons = new Button [MasterMindController.RowsCount];
		for (int i = 0; i < MasterMindController.RowsCount; ++i)
		{
			this.mChkButtons[i] = this.mRoot.FindName<Button>(String.Format("ChkBtn{0}", i));
			this.mChkButtons[i].Click += this.OnCheckClick;
			
		}
		
		// Status text
		this.mStatus = this.mRoot.FindName<TextBlock>("StatusText");
		
		// Rows
		this.mRows = new MasterMindController.Row[MasterMindController.RowsCount];
		
		for (int r = 0; r < MasterMindController.RowsCount; ++r)
		{
			this.mRows[r] = new MasterMindController.Row();
			for (int x = 0; x < MasterMindController.ColumnsCount; ++x)
			{
				this.mRows[r].mBtn[x] = this.mRoot.FindName<ToggleButton>(String.Format("Pos{0}{1}", r, x));
				this.mRows[r].mBtn[x].Click += this.OnToggleClick;
				this.mRows[r].mSol[x] = this.mRoot.FindName<Ellipse>(String.Format("Sol{0}{1}", r, x));
			}
		}
		
		// Current color red
		this.mCurrentColor = 0;
		
		// Colors
		this.mColors = new RadioButton [MasterMindController.ColorsCount];
		
		for (int i = 0; i < 8; ++i)
		{
			this.mColors[i] = this.mRoot.FindName<RadioButton>(MasterMindController.ColorRBNames[i]);
			this.mColors[i].Checked += this.OnColorCheck;
		}
		
		// Get the solution ellipses
		this.mSolutions = new Ellipse[MasterMindController.ColumnsCount];
		this.mSolutionTexts = new TextBlock[MasterMindController.ColumnsCount];
		for (int i = 0; i < MasterMindController.ColumnsCount; ++i)
		{
			this.mSolutions[i] = this.mRoot.FindName<Ellipse>(String.Format("Key{0}", i));
			this.mSolutionTexts[i] = this.mRoot.FindName<TextBlock>(String.Format("KeyText{0}", i));
		}		
		
		// Restart game
		this.mRestartGame = this.mRoot.FindName<Button>("btnRestart");
		this.mRestartGame.Click += this.OnRestartGame;
			
		// Get the brushes
		this.mBlackCircleBrush = this.mRoot.GetResources().FindName("BlackCircleBrush").As<Brush>();
		this.mWhiteCircleBrush = this.mRoot.GetResources().FindName("WhiteCircleBrush").As<Brush>();
		this.mBlackSolidBrush = this.mRoot.GetResources().FindName("BlackColorBrush").As<Brush>();
		this.mSolutionBrush = this.mRoot.GetResources().FindName("SolutionBrush").As<Brush>();
		
		this.mRandom = new System.Random();
		
		this.StartGame();
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	void StartGame()
	{
		// Create the color key
		this.mColorKey = new int[MasterMindController.ColumnsCount];
		for (int x = 0; x < MasterMindController.ColumnsCount; ++x)
		{
			this.mColorKey[x] = mRandom.Next(MasterMindController.ColorsCount);
		}
		
		// print (String.Format ("Color key: {0} {1} {2} {3}", mColorKey[0], mColorKey[1], mColorKey[2], mColorKey[3]));
		
		// First row
		this.mCurrentRow = MasterMindController.RowsCount - 1;
		
		// Set the initial text
		this.mStatus.SetText("Lets play!");
		
		// Hide the solution
		for (int i = 0; i < MasterMindController.ColumnsCount; ++i)
		{
			mSolutions[i].SetFill(this.mSolutionBrush);
			mSolutionTexts[i].SetVisibility(Visibility.Visible);
		}
		
		// Reset borders
		for (int i = 0; i < MasterMindController.RowsCount; ++i)
		{
			for (int x = 0; x < MasterMindController.ColumnsCount; ++x)
			{
				this.mRows[i].mStatus[x] = -1;
				this.mRows[i].mBtn[x].SetBackground(this.mBlackSolidBrush);
				this.mRows[i].mSol[x].SetFill(this.mBlackSolidBrush);
			}
			
			// Disable row
			this.DisableRow(i);
		}		
		
		// Enable current row
		EnableRow(this.mCurrentRow);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	void DisableRow(int i)
	{
		// Border
		this.mGrids[i].SetIsEnabled(false);
		this.mBorders[i].SetVisibility(Visibility.Hidden);
		
		// Button
		this.mChkButtons[i].SetVisibility(Visibility.Hidden);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	void EnableRow(int i)
	{
		// Border
		this.mGrids[i].SetIsEnabled(true);
		this.mBorders[i].SetVisibility(Visibility.Visible);
		
		// Button
		this.mChkButtons[i].SetVisibility(Visibility.Visible);
		this.mChkButtons[i].SetIsEnabled(false);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	void OnCheckClick(BaseComponent b, RoutedEventArgs a)
	{
		this.DisableRow(this.mCurrentRow);
		
		if (this.Check(this.mCurrentRow))
		{
			this.FinishGame(true);
			return;
		}
		
		
		if (this.mCurrentRow > 0)
		{
			--this.mCurrentRow;
			this.EnableRow(this.mCurrentRow);
		}
		else
		{
			this.FinishGame(false);
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	void OnColorCheck(BaseComponent b, RoutedEventArgs a)
	{
		FrameworkElement f = b.As<FrameworkElement>();
		string name = f.GetName ();
		
		this.mCurrentColor = Array.IndexOf(MasterMindController.ColorRBNames, name);
	}
	
	void OnToggleClick(BaseComponent b, RoutedEventArgs a)
	{
		FrameworkElement f = b.As<FrameworkElement>();
		string name = f.GetName ();
		name = name.Replace("Pos", "");
		
		int pos = int.Parse(name) % 10;
		
		this.mRows[this.mCurrentRow].mStatus[pos] = 
			this.SetColor(this.mRows[this.mCurrentRow].mBtn[pos], pos);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	int SetColor(ToggleButton btn, int x)
	{
		if (this.mCurrentColor < 0)
		{
			return - 1;
		}
		
		btn.SetBackground(this.mColors[this.mCurrentColor].GetBackground());
		
		bool enabled = true;
		
		for (int i = 0; i < MasterMindController.ColumnsCount && enabled; ++i)
		{
			if (i == x)
			{
				continue;
			}
			enabled = this.mRows[this.mCurrentRow].mStatus[i] >= 0;
		}
		
		this.mChkButtons[this.mCurrentRow].SetIsEnabled(enabled);
		
		return this.mCurrentColor;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	bool Check(int r)
	{
		int numWhites = 0;
		int numBlacks = 0;
		
		bool [] solFound =
		{
			false, false, false, false
		};
		
		// Compute the number of black solution balls
		for (int x = 0; x < MasterMindController.ColumnsCount; ++x)
		{
			if (this.mRows[r].mStatus[x] == this.mColorKey[x])
			{
				++numBlacks;
				solFound[x] = true;
			}
		}
		
		bool [] useForWhite = {false, false, false, false};
		
		// Compute the number of white solution balls
		for (int x = 0; x < MasterMindController.ColumnsCount; ++x)
		{
			if (solFound[x])
			{
				continue;
			}
			
			for (int j = 0; j < MasterMindController.ColumnsCount; ++j)
			{
				if (j == x || solFound[j] || useForWhite[j])
				{
					continue;
				}
				
				if (this.mRows[r].mStatus[x] == this.mColorKey[j])
				{
					++numWhites;
					useForWhite[j] = true;
					break;
				}
			}
		}
		
		for (int i = 0; i < numBlacks; ++i)
		{
			this.mRows[r].mSol[i].SetFill(this.mBlackCircleBrush);
		}
		for (int i = 0; i < numWhites; ++i)
		{
			this.mRows[r].mSol[numBlacks + i].SetFill(this.mWhiteCircleBrush);
		}
		
		return numBlacks == MasterMindController.ColumnsCount;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	void FinishGame(bool win)
	{
		if (win)
		{
			this.mStatus.SetText("You win!");
		}
		else
		{
			this.mStatus.SetText ("You lose...");
		}
		
		// Show the key color
		for (int i = 0; i < MasterMindController.ColumnsCount; ++i)
		{
			this.mSolutions[i].SetFill(this.mColors[this.mColorKey[i]].GetBackground());
			this.mSolutionTexts[i].SetVisibility(Visibility.Hidden);
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	void OnRestartGame(BaseComponent b, RoutedEventArgs a)
	{
		StartGame();
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	// Update is called once per frame
	void Update () 
	{
	
	}
}
