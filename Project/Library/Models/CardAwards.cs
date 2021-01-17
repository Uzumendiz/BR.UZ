namespace PointBlank
{
    public class CardAwards
    {
        public int id, card, insignia, medal, brooch, exp, gold;
        public bool Unusable()
        {
            return insignia == 0 && medal == 0 && brooch == 0 && exp == 0 && gold == 0;
        }
    }
}