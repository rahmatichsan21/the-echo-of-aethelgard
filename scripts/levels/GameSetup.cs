using Godot;
using System;

// Script helper untuk setup game
// Note: Player now handles InventoryUI connection directly
public partial class GameSetup : Node
{
	public override void _Ready()
	{
		GD.Print("GameSetup: Game initialized");
	}
}
