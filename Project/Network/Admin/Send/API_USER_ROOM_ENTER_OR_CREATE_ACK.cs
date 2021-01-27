namespace PointBlank.Api
{
    public class API_USER_ROOM_ENTER_OR_CREATE_ACK : ApiPacketWriter
    {
        private Account player;
        private Room room;
        private bool IsCreate;
        public API_USER_ROOM_ENTER_OR_CREATE_ACK(Account player, Room room, bool IsCreate)
        {
            this.player = player;
            this.room = room;
            this.IsCreate = IsCreate;
        }
        public override void Write()
        {
            WriteH(6);
            WriteC(IsCreate);
            WriteQ(player.playerId);
            WriteD(room.roomId);
            WriteC((byte)room.mapId);
            if (!IsCreate)
            {
                WriteC((byte)room.mapName.Length);
                WriteS(room.mapName, room.mapName.Length);
            }
            WriteC((byte)room.roomName.Length);
            WriteS(room.roomName, room.roomName.Length);
            WriteC(room.leaderSlot == player.slotId); //Is Leader
        }
    }
}
