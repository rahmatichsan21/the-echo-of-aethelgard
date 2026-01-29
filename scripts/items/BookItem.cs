using Godot;
using System;

// Book behavior untuk item
public class BookUsable : IUsableItem
{
	private string _bookTitle;
	private string _bookContent;

	public BookUsable(string title, string content)
	{
		_bookTitle = title;
		_bookContent = content;
	}

	public void Use(Player player)
	{
		player.ShowBook(_bookTitle, _bookContent);
	}

	public string GetUseText()
	{
		return "Read";
	}
}

// PickableItem khusus untuk buku
public partial class BookItem : PickableItem
{
	[Export] public string BookTitle = "Mysterious Book";
	[Export(PropertyHint.MultilineText)] public string BookContent = "This book is empty...";

	public override void _Ready()
	{
		// Set sebagai usable item
		base._Ready();
		
		// Create ItemData dengan usable flag
		var bookData = new ItemData(ItemId, ItemName, 1, true); // Max stack 1, usable = true
		bookData.Description = "A book that can be read";
		
		// Set book behavior
		bookData.UsableBehavior = new BookUsable(BookTitle, BookContent);
		
		// Override the default item data
		_itemData = bookData;
		
		GD.Print($"BookItem ready: {BookTitle}");
	}
}
