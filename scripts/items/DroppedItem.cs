using Godot;
using System;

// Class untuk item yang di-drop ke dunia
public partial class DroppedItem : RigidBody3D
{
	private ItemData _itemData;
	private int _quantity = 1;
	private Label3D _label;
	private MeshInstance3D _mesh;
	private float _pickupCooldown = 0.5f;
	private float _pickupTimer = 0.0f;

	public override void _Ready()
	{
		// Create mesh for visual
		_mesh = new MeshInstance3D();
		var sphereMesh = new SphereMesh();
		sphereMesh.Radius = 0.25f;
		sphereMesh.Height = 0.5f;
		_mesh.Mesh = sphereMesh;
		AddChild(_mesh);
		
		// Create collision shape
		var collisionShape = new CollisionShape3D();
		var shape = new SphereShape3D();
		shape.Radius = 0.25f;
		collisionShape.Shape = shape;
		AddChild(collisionShape);
		
		// Create label untuk quantity
		_label = new Label3D();
		_label.Position = new Vector3(0, 0.5f, 0);
		_label.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		_label.FontSize = 20;
		AddChild(_label);
		
		// Physics properties
		GravityScale = 1.0f;
		Mass = 0.5f;
		
		// Auto-pickup area
		var area = new Area3D();
		AddChild(area);
		
		var areaShape = new CollisionShape3D();
		var areaSphere = new SphereShape3D();
		areaSphere.Radius = 1.5f;
		areaShape.Shape = areaSphere;
		area.AddChild(areaShape);
		
		area.BodyEntered += OnBodyEntered;
	}

	public override void _Process(double delta)
	{
		if (_pickupTimer > 0)
		{
			_pickupTimer -= (float)delta;
		}
	}

	public void Initialize(ItemData itemData, int quantity = 1)
	{
		_itemData = itemData;
		_quantity = quantity;
		
		if (_label != null)
		{
			_label.Text = quantity > 1 ? $"{itemData.ItemName} x{quantity}" : itemData.ItemName;
		}
		
		_pickupTimer = _pickupCooldown;
	}

	public void Throw(Vector3 force)
	{
		ApplyCentralImpulse(force);
	}

	private void OnBodyEntered(Node3D body)
	{
		if (_pickupTimer > 0) return; // Cooldown belum habis
		
		if (body is Player player)
		{
			var inventory = player.GetInventory();
			if (inventory != null && inventory.AddItem(_itemData, _quantity))
			{
				GD.Print($"Auto-picked up: {_itemData.ItemName} x{_quantity}");
				QueueFree();
			}
		}
	}

	public ItemData GetItemData()
	{
		return _itemData;
	}

	public int GetQuantity()
	{
		return _quantity;
	}
}
