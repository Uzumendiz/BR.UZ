using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_JOIN_ROOM_REQ : GamePacketReader
    {
        private int match, channel, unk;
        public override void ReadImplement()
        {
            match = ReadInt();
            unk = ReadShort();
            channel = ReadShort();
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                if (p == null || p.clanId == 0 || p.match == null)
                    return;
                Account leader;
                Channel ch;
                if (p != null && p.nickname.Length > 0 && p.room == null && p.GetChannel(out ch))
                {
                    Room room = ch.GetRoom(match);
                    if (room != null && room.GetLeader(out leader))
                    {
                        if (room.password.Length > 0 && !p.HaveGMLevel())
                            client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487749));
                        else if (room.limit == 1 && (int)room.state >= 1)
                            client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487763)); //Entrada proibida com partida em andamento
                        else if (room.KickedPlayersVote.Contains(p.playerId))
                            client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487756)); //Você foi expulso dessa sala.
                        else if (room.AddPlayer(p, unk) >= 0)
                        {
                            using (PROTOCOL_ROOM_GET_SLOTONEINFO_ACK packet = new PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(p))
                                room.SendPacketToPlayers(packet, p.playerId);
                            client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0, p, leader));
                        }
                        else client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487747));
                    }
                    else client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487748));
                }
                else client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487748));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}