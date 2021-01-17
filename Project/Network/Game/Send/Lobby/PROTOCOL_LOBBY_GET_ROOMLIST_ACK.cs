namespace PointBlank.Game
{
    public class LOBBY_GET_ROOMLIST_PAK : GamePacketWriter
    {
        private int roomPage, playerPage, allPlayers, allRooms, count1, count2;
        private byte[] rooms, players;
        public LOBBY_GET_ROOMLIST_PAK(int allRooms, int allPlayers, int roomPage, int playerPage, int count1, int count2, byte[] rooms, byte[] players)
        {
            this.allRooms = allRooms;
            this.allPlayers = allPlayers;
            this.roomPage = roomPage;
            this.playerPage = playerPage;
            this.rooms = rooms;
            this.players = players;
            this.count1 = count1;
            this.count2 = count2;
        }

        public override void Write()
        {
            WriteH(3074);
            WriteD(allRooms); //total
            WriteD(roomPage); //página atual(roomPages - 1)
            WriteD(count1); //15 salas por página_salas.Count - carregando atualmente
            WriteB(rooms);

            WriteD(allPlayers); //total
            WriteD(playerPage); //página atual
            WriteD(count2); //10 por página
            WriteB(players);
        }
    }
}