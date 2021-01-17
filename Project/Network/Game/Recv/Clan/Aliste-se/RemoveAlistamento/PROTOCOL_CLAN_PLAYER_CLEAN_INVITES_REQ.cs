using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_PLAYER_CLEAN_INVITES_REQ : GamePacketReader
    {
        /* 
         * DELETA O CONVITE DE CLAN QUANDO CANCELA O ALISTAMENTO.
         */
        public override void RunImplement()
        {
        }

        public override void ReadImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player != null && player.DeleteClanInvite())
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_PLAYER_CLEAN_INVITES_SUCCESS_PAK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_PLAYER_CLEAN_INVITES_2147487835_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}