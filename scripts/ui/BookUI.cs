using Godot;
using System;

public partial class BookUI : Control
{
	private Panel _bookPanel;
	private Label _titleLabel;
	private RichTextLabel _contentLabel;
	private Button _closeButton;
	private string _bookTitle = "Book";
	private string _bookContent = "Empty book";

	public override void _Ready()
	{
		// Setup container
		SetAnchorsPreset(LayoutPreset.FullRect);
		Visible = false;
		MouseFilter = MouseFilterEnum.Stop; // Block input when visible
		
		// Semi-transparent background
		var background = new ColorRect();
		background.Color = new Color(0, 0, 0, 0.7f);
		background.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(background);
		
		// Book panel (seperti kertas/buku)
		_bookPanel = new Panel();
		_bookPanel.SetAnchorsPreset(LayoutPreset.Center);
		_bookPanel.CustomMinimumSize = new Vector2(600, 500);
		_bookPanel.Position = new Vector2(-300, -250);
		AddChild(_bookPanel);
		
		// Add stylebox untuk panel (warna kertas)
		var stylebox = new StyleBoxFlat();
		stylebox.BgColor = new Color(0.95f, 0.9f, 0.8f, 1.0f); // Cream/paper color
		stylebox.BorderWidthLeft = 3;
		stylebox.BorderWidthRight = 3;
		stylebox.BorderWidthTop = 3;
		stylebox.BorderWidthBottom = 3;
		stylebox.BorderColor = new Color(0.4f, 0.3f, 0.2f, 1.0f); // Brown border
		_bookPanel.AddThemeStyleboxOverride("panel", stylebox);
		
		var vbox = new VBoxContainer();
		vbox.SetAnchorsPreset(LayoutPreset.FullRect);
		vbox.AddThemeConstantOverride("separation", 15);
		_bookPanel.AddChild(vbox);
		
		// Margin container untuk padding
		var margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_left", 30);
		margin.AddThemeConstantOverride("margin_right", 30);
		margin.AddThemeConstantOverride("margin_top", 20);
		margin.AddThemeConstantOverride("margin_bottom", 20);
		vbox.AddChild(margin);
		
		var innerVbox = new VBoxContainer();
		innerVbox.AddThemeConstantOverride("separation", 10);
		margin.AddChild(innerVbox);
		
		// Title
		_titleLabel = new Label();
		_titleLabel.Text = "Book Title";
		_titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_titleLabel.AddThemeFontSizeOverride("font_size", 28);
		_titleLabel.AddThemeColorOverride("font_color", new Color(0.2f, 0.1f, 0.05f, 1.0f));
		innerVbox.AddChild(_titleLabel);
		
		// Separator line
		var separator = new HSeparator();
		innerVbox.AddChild(separator);
		
		// Content (scrollable)
		var scrollContainer = new ScrollContainer();
		scrollContainer.CustomMinimumSize = new Vector2(0, 350);
		innerVbox.AddChild(scrollContainer);
		
		_contentLabel = new RichTextLabel();
		_contentLabel.BbcodeEnabled = true;
		_contentLabel.FitContent = true;
		_contentLabel.ScrollActive = false;
		_contentLabel.AddThemeFontSizeOverride("normal_font_size", 16);
		_contentLabel.AddThemeColorOverride("default_color", new Color(0.1f, 0.05f, 0.0f, 1.0f));
		scrollContainer.AddChild(_contentLabel);
		
		// Close button
		_closeButton = new Button();
		_closeButton.Text = "Close (ESC)";
		_closeButton.CustomMinimumSize = new Vector2(150, 40);
		_closeButton.Pressed += OnClosePressed;
		
		var buttonContainer = new HBoxContainer();
		buttonContainer.Alignment = BoxContainer.AlignmentMode.Center;
		buttonContainer.AddChild(_closeButton);
		innerVbox.AddChild(buttonContainer);
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible && @event.IsActionPressed("ui_cancel"))
		{
			Close();
			GetViewport().SetInputAsHandled();
		}
	}

	public void ShowBook(string title, string content)
	{
		_bookTitle = title;
		_bookContent = content;
		
		_titleLabel.Text = title;
		_contentLabel.Text = content;
		
		Visible = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GD.Print($"Opening book: {title}");
	}

	private void OnClosePressed()
	{
		Close();
	}

	public void Close()
	{
		Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GD.Print("Closing book");
	}
}
