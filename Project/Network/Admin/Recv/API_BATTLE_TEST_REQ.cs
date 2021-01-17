using PointBlank.Game;

namespace PointBlank.Api
{
    public class API_BATTLE_TEST_REQ : ApiPacketReader
    {
        public override void ReadImplement()
        {
            BattleSettings.Unk1 = ReadByte();
            BattleSettings.Unk2 = ReadByte();
            BattleSettings.Unk3 = ReadByte();
            BattleSettings.Unk4 = ReadByte();
            BattleSettings.Unk5 = ReadByte();
            BattleSettings.Unk6 = ReadByte();
            BattleSettings.Unk7 = ReadByte();
            BattleSettings.Unk8 = ReadByte();
            BattleSettings.Unk9 = ReadByte();
            BattleSettings.Unk10 = ReadByte();
            BattleSettings.Unk11 = ReadByte();
            BattleSettings.Unk12 = ReadByte();
            BattleSettings.Unk13 = ReadByte();
            BattleSettings.Unk14 = ReadByte();
            BattleSettings.Unk15 = ReadByte();
            BattleSettings.Unk16 = ReadByte();
            BattleSettings.Unk17 = ReadByte();
            BattleSettings.Unk18 = ReadByte();
            BattleSettings.Unk19 = ReadByte();
            BattleSettings.Unk20 = ReadByte();
            BattleSettings.Unk21 = ReadByte();
            BattleSettings.Unk22 = ReadByte();
            BattleSettings.Unk23 = ReadByte();
            BattleSettings.Unk24 = ReadByte();
            BattleSettings.Unk25 = ReadByte();
            BattleSettings.Unk26 = ReadByte();
            BattleSettings.Unk27 = ReadByte();
            BattleSettings.Unk28 = ReadByte();
            BattleSettings.Unk29 = ReadByte();
            BattleSettings.Unk30 = ReadByte();
            BattleSettings.Unk31 = ReadByte();
            BattleSettings.Unk32 = ReadByte();
            BattleSettings.Unk33 = ReadByte();
            BattleSettings.Unk34 = ReadByte();
            BattleSettings.Unk35 = ReadByte();
        }

        public override void RunImplement()
        {
            byte result = 1;
            client.SendPacket(new API_BATTLE_TESTE_RESULT_ACK(result));
        }
    }
}