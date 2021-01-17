using System;

namespace PointBlank.Game
{
    public class PROTOCOL_GM_KICK_PLAYER_BYSLOT_REQ : GamePacketReader
    {
        private int Slot;
        public override void ReadImplement()
        {                
            //Ativa quando usa "/KICK (slotId)"
            Slot = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room == null || player.slotId == Slot)
                {
                    return;
                }
                if (!player.IsGM())
                {
                    Logger.Warning($" [GM_KICK_PLAYER_BYSLOT] O jogador foi desconectado por não ter acesso aos comandos. Nick: {player.nickname} Login: {player.login} PlayerId: {player.playerId}");
                    client.Close(1000);
                    return;
                }
                Account playerRoom = room.GetPlayerBySlot(Slot);
                if (playerRoom == null)
                {
                    client.SendPacket(new LOBBY_CHATTING_PAK(player, $" [GM_KICK_PLAYER_BYSLOT] Jogador não encontrado no slot: {Slot}", true));
                    return;
                }
                playerRoom.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2));
                playerRoom.Close(1000, true);
                client.SendPacket(new LOBBY_CHATTING_PAK(player, $" [GM_KICK_PLAYER_BYSLOT] Jogador foi desconectado com sucesso!", true));
                Logger.ChatCommands($" [GM_KICK_PLAYER_BYSLOT] Jogador foi desconectado com sucesso! Nick: {playerRoom.nickname} Login: {playerRoom.login} PlayerId: {playerRoom.playerId} SlotId: {Slot}");
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}