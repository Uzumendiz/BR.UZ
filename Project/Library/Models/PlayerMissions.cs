using System.Collections.Generic;

namespace PointBlank
{
    public class PlayerMissions
    {
        public byte[] list1 = new byte[40];
        public byte[] list2 = new byte[40];
        public byte[] list3 = new byte[40];
        public byte[] list4 = new byte[40];
        public byte actualMission;
        public byte card1;
        public byte card2;
        public byte card3;
        public byte card4;
        public byte mission1;
        public byte mission2;
        public byte mission3;
        public byte mission4;
        public bool selectedCard;
        public PlayerMissions DeepCopy()
        {
            return (PlayerMissions)MemberwiseClone();
        }

        /// <summary>
        /// Retorna a progressão do baralho atual.
        /// </summary>
        /// <returns></returns>
        public byte[] GetCurrentMissionList()
        {
            if (actualMission == 0)
            {
                return list1;
            }
            else if (actualMission == 1)
            {
                return list2;
            }
            else if (actualMission == 2)
            {
                return list3;
            }
            else
            {
                return list4;
            }
        }

        /// <summary>
        /// Retorna o número do cartão selecionado, do baralho atual.
        /// </summary>
        /// <returns></returns>
        public int GetCurrentCard()
        {
            return GetCard(actualMission);
        }

        public int GetCard(int index)
        {
            if (index == 0)
            {
                return card1;
            }
            else if (index == 1)
            {
                return card2;
            }
            else if (index == 2)
            {
                return card3;
            }
            else
            {
                return card4;
            }
        }

        public int GetCurrentMissionId()
        {
            if (actualMission == 0)
            {
                return mission1;
            }
            else if (actualMission == 1)
            {
                return mission2;
            }
            else if (actualMission == 2)
            {
                return mission3;
            }
            else
            {
                return mission4;
            }
        }

        public void UpdateSelectedCard()
        {
            int currentCard = GetCurrentCard();
            if (65535 == GetCardFlags(GetCurrentMissionId(), currentCard, GetCurrentMissionList()))
            {
                selectedCard = true;
            }
        }

        public ushort GetCardFlags(int missionId, int cardIdx, byte[] arrayList)
        {
            if (missionId == 0)
            {
                return 0;
            }
            int result = 0;
            List<Card> list = MissionCardXML.GetCards(missionId, cardIdx);
            for (int i = 0; i < list.Count; i++)
            {
                Card card = list[i];
                if (arrayList[card.arrayIdx] >= card.missionLimit)
                {
                    result |= card.flag;
                }
            }
            return (ushort)result;
        }
    }
}