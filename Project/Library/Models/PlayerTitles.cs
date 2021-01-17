﻿namespace PointBlank
{
    public class PlayerTitles
    {
        public byte Equiped1, Equiped2, Equiped3, Slots = 1;
        public long Flags;

        /// <summary>
        /// Adiciona novo título a lista de flags.
        /// </summary>
        /// <param name="Flag">Valor da flag</param>
        /// <returns></returns>
        public long Add(long flag)
        {
            Flags |= flag; 
            return Flags;
        }

        public bool Contains(long flag)
        {
            long haveFlag = Flags & flag;
            return haveFlag == flag || flag == 0;
        }

        public void SetEquip(int index, byte value)
        {
            if (index == 0)
            {
                Equiped1 = value;
            }
            else if (index == 1)
            {
                Equiped2 = value;
            }
            else if (index == 2)
            {
                Equiped3 = value;
            }
        }

        public int GetEquip(int index)
        {
            if (index == 0)
            {
                return Equiped1;
            }
            else if (index == 1)
            {
                return Equiped2;
            }
            else if (index == 2)
            {
                return Equiped3;
            }
            return 0;
        }
    }
}