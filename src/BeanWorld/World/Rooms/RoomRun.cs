using BeanWorld.World.Entities;

namespace BeanWorld.World.Rooms;

/// <summary>
/// Owns sequential room progression for one gameplay run.
/// </summary>
public class RoomRun
{
    private readonly List<CombatRoom> _rooms;

    public int CurrentRoomIndex { get; private set; }
    public bool IsCompleted { get; private set; }

    public RoomRun(IEnumerable<CombatRoom> rooms)
    {
        _rooms = rooms.ToList();
        if (_rooms.Count == 0)
            throw new ArgumentException("RoomRun requires at least one room.", nameof(rooms));
    }

    public CombatRoom CurrentRoom => _rooms[CurrentRoomIndex];
    public int TotalRooms => _rooms.Count;

    public void Start()
    {
        foreach (var room in _rooms)
            room.Reset();

        CurrentRoomIndex = 0;
        IsCompleted = false;
    }

    public bool TryAdvance(Player player)
    {
        if (IsCompleted)
            return false;

        var room = CurrentRoom;
        room.RefreshState();

        if (!room.CanTransition(player))
            return false;

        bool wasLastRoom = CurrentRoomIndex >= _rooms.Count - 1;
        if (wasLastRoom)
        {
            IsCompleted = true;
            return true;
        }

        CurrentRoomIndex++;
        return true;
    }
}
