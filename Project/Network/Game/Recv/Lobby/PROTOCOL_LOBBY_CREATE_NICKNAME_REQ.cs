using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_CREATE_NICKNAME_REQ : GamePacketReader
    {
        private string nickname;
        public override void ReadImplement()
        {
            nickname = ReadString(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.nickname.Length > 0 || AccountManager.CheckNickLengthInvalid(nickname))
                {
                    client.SendCompletePacket(PackageDataManager.LOBBY_CREATE_NICKNAME_ERROR_0x80001013_PAK);
                    return;
                }
                if (StringFilter.CheckStringFilter(nickname) && !AccountManager.CheckNicknameExist(nickname).Result && player.UpdateNick(nickname).Result)
                {
                    player.nickname = nickname;
                    if (!NickHistoryManager.CreateHistory(player.playerId, player.nickname, nickname, "First nick"))
                    {
                        Logger.Analyze($" [LOBBY_CREATE_NICK_NAME_REQ] Não foi possivel salvar o histórico de nome. PlayerId: {player.playerId} Nickname: {nickname} Motivo: First nick.");
                    }
                    List<ItemsModel> awards = DefaultInventoryManager.awards;
                    if (awards.Count > 0)
                    {
                        client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, awards));
                        client.SendCompletePacket(PackageDataManager.GAME_SERVER_MESSAGE_ITEM_RECEIVE_PAK);
                    }
                    client.SendCompletePacket(PackageDataManager.LOBBY_CREATE_NICKNAME_SUCCESS_PAK);
                    client.SendPacket(new PROTOCOL_BASE_QUEST_GET_INFO_ACK(player));
                }
                else
                {
                    Logger.Analyze($" [LOBBY_CREATE_NICK_NAME_REQ] Não foi possivel atualizar o nome do jogador na database. PlayerId: {player.playerId} Nickname: {nickname} Motivo: First nick.");
                    client.SendCompletePacket(PackageDataManager.LOBBY_CREATE_NICKNAME_ERROR_0x80000113_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}