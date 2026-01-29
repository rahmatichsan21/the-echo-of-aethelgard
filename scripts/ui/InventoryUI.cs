using Godot;
using System;

public partial class InventoryUI : Control
{
	private InventorySystem _inventory;
	private GridContainer _inventoryGrid;
	private GridContainer _hotbarGrid;
	private Label _selectedItemLabel;
	private bool _isVisible = false;
	private Control _crosshairContainer;

	public override void _Ready()
	{
		// Setup UI Container
		SetAnchorsPreset(LayoutPreset.FullRect);
		Visible = false;
		
		// Background panel untuk inventory
		var panel = new Panel();
		panel.SetAnchorsPreset(LayoutPreset.Center);
		panel.CustomMinimumSize = new Vector2(400, 300);
		panel.Position = new Vector2(-200, -150);
		AddChild(panel);
		
		var vbox = new VBoxContainer();
		vbox.SetAnchorsPreset(LayoutPreset.FullRect);
		vbox.AddThemeConstantOverride("separation", 10);
		panel.AddChild(vbox);
		
		// Title
		var title = new Label();
		title.Text = "Inventory";
		title.HorizontalAlignment = HorizontalAlignment.Center;
		title.AddThemeFontSizeOverride("font_size", 24);
		vbox.AddChild(title);
		
		// Main inventory grid (6 slots, 3x2)
		_inventoryGrid = new GridContainer();
		_inventoryGrid.Columns = 3;
		_inventoryGrid.AddThemeConstantOverride("h_separation", 5);
		_inventoryGrid.AddThemeConstantOverride("v_separation", 5);
		vbox.AddChild(_inventoryGrid);
		
		// Create inventory slots
		for (int i = 0; i < 6; i++)
		{
			var slot = CreateInventorySlot(i);
			_inventoryGrid.AddChild(slot);
		}
		
		// Hotbar (always visible)
		CreateHotbar();
		
		// Selected item info
		_selectedItemLabel = new Label();
		_selectedItemLabel.Text = "";
		_selectedItemLabel.HorizontalAlignment = HorizontalAlignment.Center;
		vbox.AddChild(_selectedItemLabel);
		
		// Instructions
		var instructions = new Label();
		instructions.Text = "E: Pickup | Q: Drop 1 | Ctrl+Q: Drop All | F: Use Item | Tab/I: Toggle Inventory | 1-6: Select Hotbar";
		instructions.HorizontalAlignment = HorizontalAlignment.Center;
		instructions.AddThemeFontSizeOverride("font_size", 12);
		vbox.AddChild(instructions);
		
		// Add crosshair (always visible)
		CreateCrosshair();
	}
	
	private void CreateCrosshair()
	{
		// Crosshair container di tengah layar
		_crosshairContainer = new Control();
		_crosshairContainer.Name = "Crosshair";
		_crosshairContainer.SetAnchorsPreset(LayoutPreset.Center);
		_crosshairContainer.MouseFilter = MouseFilterEnum.Ignore;
		GetParent().CallDeferred("add_child", _crosshairContainer);
		
		// Horizontal line
		var hLine = new ColorRect();
		hLine.Color = new Color(1, 1, 1, 0.8f);
		hLine.Size = new Vector2(20, 2);
		hLine.Position = new Vector2(-10, -1);
		hLine.MouseFilter = MouseFilterEnum.Ignore;
		_crosshairContainer.AddChild(hLine);
		
		// Vertical line
		var vLine = new ColorRect();
		vLine.Color = new Color(1, 1, 1, 0.8f);
		vLine.Size = new Vector2(2, 20);
		vLine.Position = new Vector2(-1, -10);
		vLine.MouseFilter = MouseFilterEnum.Ignore;
		_crosshairContainer.AddChild(vLine);
		
		// Center dot
		var dot = new ColorRect();
		dot.Color = new Color(1, 1, 1, 0.9f);
		dot.Size = new Vector2(4, 4);
		dot.Position = new Vector2(-2, -2);
		dot.MouseFilter = MouseFilterEnum.Ignore;
		_crosshairContainer.AddChild(dot);
	}
	
	public void SetCrosshairVisible(bool visible)
	{
		if (_crosshairContainer != null)
		{
			_crosshairContainer.Visible = visible;
		}
	}

	private void CreateHotbar()
	{
		// Hotbar container (always visible at bottom)
		var hotbarContainer = new Panel();
		hotbarContainer.SetAnchorsPreset(LayoutPreset.CenterBottom);
		hotbarContainer.CustomMinimumSize = new Vector2(520, 90);
		hotbarContainer.Position = new Vector2(-260, -100); // Centered and higher from bottom
		
		// Use CallDeferred to avoid parent busy error
		GetParent().CallDeferred("add_child", hotbarContainer);
		
		_hotbarGrid = new GridContainer();
		_hotbarGrid.Columns = 6;
		_hotbarGrid.SetAnchorsPreset(LayoutPreset.Center);
		_hotbarGrid.Position = new Vector2(-240, -35);
		_hotbarGrid.AddThemeConstantOverride("h_separation", 5);
		hotbarContainer.AddChild(_hotbarGrid);
		
		// Create 6 hotbar slots
		for (int i = 0; i < 6; i++)
		{
			var slot = CreateHotbarSlot(i);
			_hotbarGrid.AddChild(slot);
		}
	}

	private Panel CreateInventorySlot(int index)
	{
		var slot = new Panel();
		slot.CustomMinimumSize = new Vector2(80, 80);
		
		var label = new Label();
		label.Name = "ItemLabel";
		label.Text = "";
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.VerticalAlignment = VerticalAlignment.Center;
		label.SetAnchorsPreset(LayoutPreset.FullRect);
		label.AddThemeFontSizeOverride("font_size", 14);
		label.AutowrapMode = TextServer.AutowrapMode.Word;
		slot.AddChild(label);
		
		return slot;
	}

	private Panel CreateHotbarSlot(int index)
	{
		var slot = new Panel();
		slot.CustomMinimumSize = new Vector2(80, 80);
		slot.Name = $"HotbarSlot{index}";
		
		// Slot number
		var numberLabel = new Label();
		numberLabel.Text = (index + 1).ToString();
		numberLabel.Position = new Vector2(5, 5);
		numberLabel.AddThemeFontSizeOverride("font_size", 12);
		slot.AddChild(numberLabel);
		
		// Item label
		var itemLabel = new Label();
		itemLabel.Name = "ItemLabel";
		itemLabel.Text = "";
		itemLabel.HorizontalAlignment = HorizontalAlignment.Center;
		itemLabel.VerticalAlignment = VerticalAlignment.Center;
		itemLabel.SetAnchorsPreset(LayoutPreset.Center);
		itemLabel.AddThemeFontSizeOverride("font_size", 14);
		itemLabel.AutowrapMode = TextServer.AutowrapMode.Word;
		slot.AddChild(itemLabel);
		
		return slot;
	}

	public void SetInventory(InventorySystem inventory)
	{
		_inventory = inventory;
		_inventory.InventoryChanged += UpdateDisplay;
		_inventory.HotbarSlotChanged += OnHotbarSlotChanged;
		GD.Print("InventoryUI: Connected to inventory system");
		
		// Delay initial update untuk memastikan hotbar grid sudah ready
		CallDeferred(nameof(DelayedInitialUpdate));
	}
	
	private void DelayedInitialUpdate()
	{
		GD.Print("InventoryUI: Performing delayed initial update...");
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		if (_inventory == null)
		{
			GD.Print("UpdateDisplay: Inventory is NULL!");
			return;
		}
		
		var items = _inventory.GetAllItems();
		GD.Print($"UpdateDisplay: Updating UI with {items.Count} total slots");
		
		// Update main inventory
		for (int i = 0; i < 6 && i < _inventoryGrid.GetChildCount(); i++)
		{
			var slot = _inventoryGrid.GetChild(i) as Panel;
			var label = slot.GetNode<Label>("ItemLabel");
			
			if (items[i] != null)
			{
				string text = items[i].Data.ItemName;
				if (items[i].Quantity > 1)
					text += $"\nx{items[i].Quantity}";
				label.Text = text;
				GD.Print($"  Slot {i}: {items[i].Data.ItemName} x{items[i].Quantity}");
			}
			else
			{
				label.Text = "";
			}
		}
		
		// Update hotbar
		UpdateHotbar();
	}

	private void UpdateHotbar()
	{
		if (_inventory == null || _hotbarGrid == null)
		{
			GD.Print("UpdateHotbar: Waiting for grid to be ready...");
			return;
		}
		
		// Tunggu sampai hotbar grid punya children
		if (_hotbarGrid.GetChildCount() < 6)
		{
			GD.Print($"UpdateHotbar: Grid only has {_hotbarGrid.GetChildCount()} children, waiting...");
			CallDeferred(nameof(UpdateHotbar));
			return;
		}
		
		var items = _inventory.GetAllItems();
		GD.Print($"UpdateHotbar: Updating {_hotbarGrid.GetChildCount()} slots");
		
		for (int i = 0; i < 6 && i < _hotbarGrid.GetChildCount(); i++)
		{
			var slot = _hotbarGrid.GetChild(i) as Panel;
			if (slot == null) continue;
			
			var label = slot.GetNode<Label>("ItemLabel");
			
			if (items[i] != null)
			{
				string text = items[i].Data.ItemName;
				if (items[i].Quantity > 1)
					text += $"\nx{items[i].Quantity}";
				label.Text = text;
				GD.Print($"  Hotbar slot {i}: {items[i].Data.ItemName}");
			}
			else
			{
				label.Text = "";
			}
			
			// Highlight selected slot
			if (i == _inventory.GetSelectedHotbarSlot())
			{
				slot.AddThemeStyleboxOverride("panel", CreateHighlightStylebox());
			}
			else
			{
				slot.RemoveThemeStyleboxOverride("panel");
			}
		}
	}

	private StyleBox CreateHighlightStylebox()
	{
		var stylebox = new StyleBoxFlat();
		stylebox.BgColor = new Color(1, 1, 0, 0.3f); // Yellow highlight
		stylebox.BorderColor = new Color(1, 1, 0, 1);
		stylebox.BorderWidthLeft = 2;
		stylebox.BorderWidthRight = 2;
		stylebox.BorderWidthTop = 2;
		stylebox.BorderWidthBottom = 2;
		return stylebox;
	}

	private void OnHotbarSlotChanged(int slotIndex)
	{
		UpdateHotbar();
		
		// Update selected item info
		var selectedItem = _inventory.GetSelectedHotbarItem();
		if (selectedItem != null)
		{
			_selectedItemLabel.Text = $"Selected: {selectedItem.Data.ItemName} x{selectedItem.Quantity}";
		}
		else
		{
			_selectedItemLabel.Text = "Selected: Empty";
		}
	}

	public void Toggle()
	{
		_isVisible = !_isVisible;
		Visible = _isVisible;
	}

	public bool IsInventoryVisible()
	{
		return _isVisible;
	}
}
