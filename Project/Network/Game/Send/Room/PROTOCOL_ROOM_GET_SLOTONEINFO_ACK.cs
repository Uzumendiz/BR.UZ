namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_GET_SLOTONEINFO_ACK : GamePacketWriter
    {
        private Account player;
        private Clan clan;
        public PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(Account player)
        {
            this.player = player;
            if (this.player != null)
            {
                clan = ClanManager.GetClan(player.clanId);
            }
        }
        public PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(Account player, Clan clan)
        {
            this.player = player;
            this.clan = clan;
        }
        public override void Write()
        {
            WriteH(3909);
            WriteD(player.slotId);
            WriteC((byte)player.room.GetSlot(player.slotId).state);
            WriteC((byte)player.GetRank());
            WriteD(clan.id);
            WriteD((int)player.clanAuthority);
            WriteC(clan.rank);
            WriteD(clan.logo);
            WriteC(player.pccafe);
            WriteC(player.tourneyLevel);
            WriteD((uint)player.effects);
            WriteS(clan.name, 17);
            WriteD(0);
            WriteC(player.country);
            WriteS(player.nickname, 33);
            WriteC(player.nickcolor);
        }
    }
}