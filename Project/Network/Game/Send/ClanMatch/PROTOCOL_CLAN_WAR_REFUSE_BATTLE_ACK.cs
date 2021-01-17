namespace PointBlank.Game
{
    public class CLAN_WAR_RECUSED_BATTLE_PAK : GamePacketWriter
    {
        private uint _erro;
        public CLAN_WAR_RECUSED_BATTLE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1560);
            WriteD(_erro);
        }
    }
}