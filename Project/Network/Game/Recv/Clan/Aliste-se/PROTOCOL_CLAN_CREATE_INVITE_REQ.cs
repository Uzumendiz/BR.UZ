using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CREATE_INVITE_REQ : GamePacketReader
    {
        private int clanId;
        private string text;
        public override void ReadImplement()
        {
            clanId = ReadInt();
            text = ReadString(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || text.Length > 200)
                {
                    return;
                }
                ClanInvite invite = new ClanInvite
                {
                    clanId = clanId,
                    playerId = player.playerId,
                    text = text,
                    inviteDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"))
                };
                if (player.clanId > 0 || player.nickname.Length == 0)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CREATE_INVITE_2147487836_PAK);
                }
                else if (ClanManager.GetClan(clanId).id == 0)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CREATE_INVITE_0x80000000_PAK);
                }
                else if (player.GetInvitesCount(clanId) >= 100)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CREATE_INVITE_2147487831_PAK);
                }
                else if (!player.InsertInvite(invite))
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CREATE_INVITE_2147487848_PAK);
                }
                else
                {
                    client.SendPacket(new PROTOCOL_CLAN_CREATE_INVITE_ACK(0, clanId));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}