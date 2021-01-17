using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/* 
    [PORTUGUESE BRAZIL]
    Este código-fonte é de autoria da <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ é um software privado: você não pode usar,
    redistribuir e/ou modificar sem autorização.

    Você deveria ter recebido uma autorização com esse software/codigo-fonte
    para ter cópias, se você não recebeu, encerre/exclua imediatamente 
    todos os vestígios do mesmo para não ter futuros problemas.

    Atenciosamente, <BR.UZ/>, MomzGames, ExploitTeam.
*/

/*
    [INGLISH USA]
    This source code is written by <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ a private software: you will not be able to use it,
    redistribute it and / or modify without authorization.

    You should have received an authorization with this software/source code
    to have copies, if you did not receive it, immediately shutdown/exclude
    all traces of it so that you have no future problems.

    Sincerely, <BR.UZ/>, MomzGames, ExploitTeam.
*/

namespace PointBlank
{
    public class MissionCardXML
    {
        private static readonly List<MissionItemAward> items = new List<MissionItemAward>();
        private static readonly List<Card> list = new List<Card>();
        private static readonly List<CardAwards> awards = new List<CardAwards>();
        private static void Load(string file)
        {
            string path = $"Data/Missions/Files/{file}.mqf";
            if (!File.Exists(path))
            {
                Logger.Warning($" [MissionCard] {path} no exists.");
                return;
            }
            GenerateList(path, file);
        }
        public static void LoadBasicCards()
        {
            Load("AssaultCard");
            Load("Dino_Basic");
            Load("Dino_Intensify");
            Load("Human_Basic");
            Load("Human_Intensify");
            Load("TutorialCard_Brazil");
            Load("Dino_Tutorial");
            Load("Human_Tutorial");
            Load("Field_o");
            Load("SpecialCard");
            Load("InfiltrationCard");
            Load("DefconCard");
            Load("Company_o");
            Load("BackUpCard");
            Load("Commissioned_o");
            Load("EventCard");
            Logger.Informations($" [MissionCard] Load {list.Count} cards.");
            Logger.Informations($" [MissionCard] Load {awards.Count} card awards.");
            Logger.Informations($" [MissionCard] Load {items.Count} items awards.");
        }
        private static int ConvertStringToInt(string missionName)
        {
            int missionId = 0;
            if (missionName == "TutorialCard_Brazil") missionId = 1;
            else if (missionName == "Dino_Tutorial") missionId = 2;
            else if (missionName == "Human_Tutorial") missionId = 3;
            else if (missionName == "AssaultCard") missionId = 5;
            else if (missionName == "BackUpCard") missionId = 6;
            else if (missionName == "InfiltrationCard") missionId = 7;
            else if (missionName == "SpecialCard") missionId = 8;
            else if (missionName == "DefconCard") missionId = 9;
            else if (missionName == "Commissioned_o") missionId = 10;
            else if (missionName == "Company_o") missionId = 11;
            else if (missionName == "Field_o") missionId = 12;
            else if (missionName == "EventCard") missionId = 13;
            else if (missionName == "Dino_Basic") missionId = 14;
            else if (missionName == "Human_Basic") missionId = 15;
            else if (missionName == "Dino_Intensify") missionId = 16;
            else if (missionName == "Human_Intensify") missionId = 17;
            return missionId;
        }
        public static List<ItemsModel> GetMissionAwards(int missionId)
        {
            List<ItemsModel> result = new List<ItemsModel>();
            lock (items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    MissionItemAward mia = items[i];
                    if (mia.missionId == missionId)
                    {
                        result.Add(mia.item);
                    }
                }
            }
            return result;
        }
        public static List<Card> GetCards(int missionId, int cardBasicId)
        {
            List<Card> result = new List<Card>();
            lock (list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Card card = list[i];
                    if (card.missionId == missionId && ((cardBasicId >= 0 && card.cardBasicId == cardBasicId) || cardBasicId == -1))
                    {
                        result.Add(card);
                    }
                }
            }
            return result;
        }
        public static List<Card> GetCards(List<Card> Cards, int cardBasicId)
        {
            if (cardBasicId == -1)
            {
                return Cards;
            }
            List<Card> result = new List<Card>();
            for (int i = 0; i < Cards.Count; i++)
            {
                Card card = Cards[i];
                if ((cardBasicId >= 0 && card.cardBasicId == cardBasicId) || cardBasicId == -1)
                {
                    result.Add(card);
                }
            }
            return result;
        }
        public static List<Card> GetCards(int missionId)
        {
            List<Card> result = new List<Card>();
            lock (list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Card card = list[i];
                    if (card.missionId == missionId)
                    {
                        result.Add(card);
                    }
                }
            }
            return result;
        }
        private static void GenerateList(string path, string missionName)
        {
            int missionId = ConvertStringToInt(missionName);
            if (missionId == 0)
            {
                Logger.Warning(" [MissionCardXML] mission inválid: " + missionName);
            }
            byte[] buffer;
            try
            {
                buffer = File.ReadAllBytes(path);
            }
            catch
            {
                buffer = new byte[0];
            }
            if (buffer.Length == 0)
            {
                return;
            }
            try
            {
                PacketReader receive = new PacketReader(buffer);
                receive.ReadS(4);
                int questType = receive.ReadD();
                receive.ReadB(16);
                int valor1 = 0, valor2 = 0;
                for (int i = 0; i < 40; i++)
                {
                    int missionBId = valor2++, cardBId = valor1;
                    if (valor2 == 4)
                    {
                        valor2 = 0;
                        valor1++;
                    }
                    int reqType = receive.ReadUH();
                    list.Add(new Card(cardBId, missionBId)
                    {
                        missionType = (MissionTypeEnum)receive.ReadC(),
                        mapId = receive.ReadC(),
                        missionLimit = receive.ReadC(),
                        weaponReq = (ClassTypeEnum)receive.ReadC(),
                        weaponReqId = receive.ReadUH(),

                        missionId = missionId
                    });
                    if (questType == 1)
                    {
                        receive.ReadB(24);
                    }
                }
                int vai = questType == 2 ? 5 : 1;
                for (int i = 0; i < 10; i++)
                {
                    int gold = receive.ReadD();
                    int xp = receive.ReadD();
                    int medals = receive.ReadD();
                    for (int i2 = 0; i2 < vai; i2++)
                    {
                        int unk = receive.ReadD();
                        int type = receive.ReadD();
                        int itemId = receive.ReadD();
                        int itemCount = receive.ReadD();
                    }
                    CardAwards card = new CardAwards { id = missionId, card = i, exp = questType == 1 ? (xp * 10) : xp, gold = gold };
                    GetCardMedalInfo(card, medals);
                    if (!card.Unusable())
                    {
                        awards.Add(card);
                    }
                }
                if (questType == 2)
                {
                    int goldResult = receive.ReadD();
                    receive.ReadB(8);
                    for (int i = 0; i < 5; i++)
                    {
                        int unkI = receive.ReadD();
                        int typeI = receive.ReadD(); //1 - unidade | 2 - dias
                        int itemId = receive.ReadD();
                        int itemCount = receive.ReadD();
                        if (unkI > 0)
                        {
                            items.Add(new MissionItemAward { missionId = missionId, item = new ItemsModel(itemId) { equip = 1, count = itemCount, name = "Mission item" } });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        private static void GetCardMedalInfo(CardAwards card, int medalId)
        {
            if (medalId == 0)
            {
                return;
            }
            if (medalId >= 1 && medalId <= 50)
            {
                card.brooch++;
            }
            else if (medalId >= 51 && medalId <= 100)
            {
                card.insignia++;
            }
            else if (medalId >= 101 && medalId <= 116)
            {
                card.medal++;
            }
        }

        public static CardAwards GetAward(int mission, int cartao)
        {
            //for (int i = 0; i < awards.Count; i++)
            //{
            //    CardAwards card = awards[i];
            //    if (card.id == mission && card.card == cartao)
            //    {
            //        return card;
            //    }
            //}
            //return null;

            return awards.Where(x => x.id == mission && x.card == cartao).FirstOrDefault();
        }
    }

    public class Card
    {
        public ClassTypeEnum weaponReq;
        public MissionTypeEnum missionType;
        public int missionId;
        public int missionBasicId, cardBasicId, arrayIdx;
        public ushort weaponReqId;
        public byte mapId, missionLimit;
        public ushort flag;
        public Card(int cardBasicId, int missionBasicId)
        {
            this.cardBasicId = cardBasicId;
            this.missionBasicId = missionBasicId;
            arrayIdx = (cardBasicId * 4) + this.missionBasicId;
            flag = (ushort)(15 << 4 * this.missionBasicId);
        }
    }
}