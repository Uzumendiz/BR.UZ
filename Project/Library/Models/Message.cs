using System;
using System.Globalization;

namespace PointBlank
{
    public class Message
    {
        public int objectId;
        public int clanId;
        public byte type;
        public int state;
        public long senderId;
        public int expireDate;
        public string senderName = "";
        public string text = "";
        public NoteMessageClanEnum noteEnum;
        public byte DaysRemaining;
        public Message(int expire, DateTime start)
        {
            expireDate = expire;
            SetDaysRemaining(start);
        }
        public Message(double days)
        {
            DateTime date = DateTime.Now.AddDays(days);
            expireDate = int.Parse(date.ToString("yyMMddHHmm"));
            SetDaysRemaining(date, DateTime.Now);
        }
        private void SetDaysRemaining(DateTime now)
        {
            DateTime end = DateTime.ParseExact(expireDate.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);
            SetDaysRemaining(end, now);
        }
        private void SetDaysRemaining(DateTime end, DateTime now)
        {
            byte days = (byte)Math.Ceiling((end - now).TotalDays);
            DaysRemaining = (byte)(days < 0 ? 0 : days);
        }
    }
}