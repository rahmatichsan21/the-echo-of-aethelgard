using Godot;
using System;
using System.Collections.Generic;

public partial class PuzzleUI : Control
{
	[Signal]
	public delegate void PuzzleCompletedEventHandler(bool success);

	private Panel _puzzlePanel;
	private Label _titleLabel;
	private Label _instructionLabel;
	private HBoxContainer _numberContainer;
	private Label _feedbackLabel;
	private Button _submitButton;
	private Button _clearButton;
	private Button _closeButton;
	
	private List<Button> _numberButtons = new List<Button>();
	private List<int> _currentInput = new List<int>();
	private int[] _correctSequence;
	private int _maxLength = 6;

	public override void _Ready()
	{
		SetAnchorsPreset(LayoutPreset.FullRect);
		Visible = false;
		MouseFilter = MouseFilterEnum.Stop;
		
		// Semi-transparent background
		var background = new ColorRect();
		background.Color = new Color(0, 0, 0, 0.8f);
		background.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(background);
		
		// Main puzzle panel
		_puzzlePanel = new Panel();
		_puzzlePanel.SetAnchorsPreset(LayoutPreset.Center);
		_puzzlePanel.CustomMinimumSize = new Vector2(500, 600);
		_puzzlePanel.Position = new Vector2(-250, -300);
		AddChild(_puzzlePanel);
		
		var stylebox = new StyleBoxFlat();
		stylebox.BgColor = new Color(0.15f, 0.15f, 0.2f, 0.95f);
		stylebox.BorderWidthLeft = 2;
		stylebox.BorderWidthRight = 2;
		stylebox.BorderWidthTop = 2;
		stylebox.BorderWidthBottom = 2;
		stylebox.BorderColor = new Color(0.3f, 0.5f, 0.7f, 1.0f);
		_puzzlePanel.AddThemeStyleboxOverride("panel", stylebox);
		
		var vbox = new VBoxContainer();
		vbox.SetAnchorsPreset(LayoutPreset.FullRect);
		vbox.AddThemeConstantOverride("separation", 15);
		_puzzlePanel.AddChild(vbox);
		
		var margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 30);
		margin.AddThemeConstantOverride("margin_right", 30);
		margin.AddThemeConstantOverride("margin_top", 20);
		margin.AddThemeConstantOverride("margin_bottom", 20);
		vbox.AddChild(margin);
		
		var innerVbox = new VBoxContainer();
		innerVbox.AddThemeConstantOverride("separation", 15);
		margin.AddChild(innerVbox);
		
		// Title
		_titleLabel = new Label();
		_titleLabel.Text = "🔒 DOOR LOCK SYSTEM";
		_titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_titleLabel.AddThemeFontSizeOverride("font_size", 28);
		_titleLabel.AddThemeColorOverride("font_color", new Color(0.3f, 0.7f, 1.0f, 1.0f));
		innerVbox.AddChild(_titleLabel);
		
		// Instructions
		_instructionLabel = new Label();
		_instructionLabel.Text = "Enter the 6-digit code to unlock the door";
		_instructionLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_instructionLabel.AddThemeFontSizeOverride("font_size", 16);
		_instructionLabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.8f, 0.8f, 1.0f));
		innerVbox.AddChild(_instructionLabel);
		
		var separator1 = new HSeparator();
		innerVbox.AddChild(separator1);
		
		// Current input display
		_numberContainer = new HBoxContainer();
		_numberContainer.Alignment = BoxContainer.AlignmentMode.Center;
		_numberContainer.AddThemeConstantOverride("separation", 10);
		innerVbox.AddChild(_numberContainer);
		
		for (int i = 0; i < _maxLength; i++)
		{
			var display = new Label();
			display.Text = "_";
			display.HorizontalAlignment = HorizontalAlignment.Center;
			display.CustomMinimumSize = new Vector2(50, 60);
			display.AddThemeFontSizeOverride("font_size", 36);
			display.AddThemeColorOverride("font_color", new Color(1, 1, 0, 1));
			
			var displayPanel = new Panel();
			displayPanel.CustomMinimumSize = new Vector2(60, 70);
			var displayStylebox = new StyleBoxFlat();
			displayStylebox.BgColor = new Color(0.1f, 0.1f, 0.15f, 1.0f);
			displayStylebox.BorderWidthLeft = 2;
			displayStylebox.BorderWidthRight = 2;
			displayStylebox.BorderWidthTop = 2;
			displayStylebox.BorderWidthBottom = 2;
			displayStylebox.BorderColor = new Color(0.4f, 0.4f, 0.5f, 1.0f);
			displayPanel.AddThemeStyleboxOverride("panel", displayStylebox);
			displayPanel.AddChild(display);
			
			_numberContainer.AddChild(displayPanel);
		}
		
		var separator2 = new HSeparator();
		innerVbox.AddChild(separator2);
		
		// Number pad (1-6 untuk puzzle)
		var numberPadLabel = new Label();
		numberPadLabel.Text = "Select Numbers:";
		numberPadLabel.HorizontalAlignment = HorizontalAlignment.Center;
		numberPadLabel.AddThemeFontSizeOverride("font_size", 14);
		innerVbox.AddChild(numberPadLabel);
		
		var grid = new GridContainer();
		grid.Columns = 3;
		grid.AddThemeConstantOverride("h_separation", 10);
		grid.AddThemeConstantOverride("v_separation", 10);
		innerVbox.AddChild(grid);
		
		for (int i = 1; i <= 6; i++)
		{
			var btn = new Button();
			btn.Text = i.ToString();
			btn.CustomMinimumSize = new Vector2(120, 60);
			btn.AddThemeFontSizeOverride("font_size", 24);
			int number = i;
			btn.Pressed += () => OnNumberPressed(number);
			grid.AddChild(btn);
			_numberButtons.Add(btn);
		}
		
		// Feedback label
		_feedbackLabel = new Label();
		_feedbackLabel.Text = "";
		_feedbackLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_feedbackLabel.AddThemeFontSizeOverride("font_size", 18);
		innerVbox.AddChild(_feedbackLabel);
		
		// Action buttons
		var buttonBox = new HBoxContainer();
		buttonBox.Alignment = BoxContainer.AlignmentMode.Center;
		buttonBox.AddThemeConstantOverride("separation", 10);
		innerVbox.AddChild(buttonBox);
		
		_clearButton = new Button();
		_clearButton.Text = "Clear";
		_clearButton.CustomMinimumSize = new Vector2(100, 40);
		_clearButton.Pressed += OnClearPressed;
		buttonBox.AddChild(_clearButton);
		
		_submitButton = new Button();
		_submitButton.Text = "Submit";
		_submitButton.CustomMinimumSize = new Vector2(100, 40);
		_submitButton.Pressed += OnSubmitPressed;
		buttonBox.AddChild(_submitButton);
		
		_closeButton = new Button();
		_closeButton.Text = "Close (ESC)";
		_closeButton.CustomMinimumSize = new Vector2(100, 40);
		_closeButton.Pressed += OnClosePressed;
		buttonBox.AddChild(_closeButton);
		
		UpdateDisplay();
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible && @event.IsActionPressed("ui_cancel"))
		{
			Close();
			GetViewport().SetInputAsHandled();
		}
		
		// Keyboard number input
		if (Visible && @event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (keyEvent.Keycode >= Key.Key1 && keyEvent.Keycode <= Key.Key6)
			{
				int number = (int)keyEvent.Keycode - (int)Key.Key1 + 1;
				OnNumberPressed(number);
			}
			else if (keyEvent.Keycode == Key.Backspace || keyEvent.Keycode == Key.Delete)
			{
				OnClearPressed();
			}
			else if (keyEvent.Keycode == Key.Enter || keyEvent.Keycode == Key.KpEnter)
			{
				OnSubmitPressed();
			}
		}
	}

	public void ShowPuzzle(int[] correctSequence)
	{
		_correctSequence = correctSequence;
		_currentInput.Clear();
		_feedbackLabel.Text = "";
		UpdateDisplay();
		Visible = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GD.Print("Puzzle opened");
	}

	private void OnNumberPressed(int number)
	{
		if (_currentInput.Count < _maxLength)
		{
			_currentInput.Add(number);
			UpdateDisplay();
			GD.Print($"Input: {number}");
		}
	}

	private void OnClearPressed()
	{
		if (_currentInput.Count > 0)
		{
			_currentInput.RemoveAt(_currentInput.Count - 1);
			UpdateDisplay();
			_feedbackLabel.Text = "";
		}
	}

	private void OnSubmitPressed()
	{
		if (_currentInput.Count != _maxLength)
		{
			_feedbackLabel.Text = "⚠ Code must be 6 digits!";
			_feedbackLabel.AddThemeColorOverride("font_color", new Color(1, 0.5f, 0, 1));
			return;
		}
		
		bool correct = true;
		for (int i = 0; i < _maxLength; i++)
		{
			if (_currentInput[i] != _correctSequence[i])
			{
				correct = false;
				break;
			}
		}
		
		if (correct)
		{
			_feedbackLabel.Text = "✓ ACCESS GRANTED!";
			_feedbackLabel.AddThemeColorOverride("font_color", new Color(0, 1, 0, 1));
			GD.Print("Puzzle solved!");
			
			// Delay sebelum close
			GetTree().CreateTimer(1.5).Timeout += () =>
			{
				EmitSignal(SignalName.PuzzleCompleted, true);
				Close();
			};
		}
		else
		{
			_feedbackLabel.Text = "✗ INCORRECT CODE!";
			_feedbackLabel.AddThemeColorOverride("font_color", new Color(1, 0, 0, 1));
			_currentInput.Clear();
			UpdateDisplay();
			GD.Print("Wrong code!");
		}
	}

	private void OnClosePressed()
	{
		Close();
	}

	private void Close()
	{
		Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		EmitSignal(SignalName.PuzzleCompleted, false);
	}

	private void UpdateDisplay()
	{
		for (int i = 0; i < _maxLength; i++)
		{
			var panel = _numberContainer.GetChild(i) as Panel;
			var label = panel.GetChild(0) as Label;
			
			if (i < _currentInput.Count)
			{
				label.Text = _currentInput[i].ToString();
			}
			else
			{
				label.Text = "_";
			}
		}
	}
}
