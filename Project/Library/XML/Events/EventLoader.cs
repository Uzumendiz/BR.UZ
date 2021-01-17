namespace PointBlank
{
    public static class EventLoader
    {
        public static void LoadAll()
        {
            EventVisitSyncer.Load();
            EventLoginSyncer.Load();
            EventMapSyncer.Load();
            EventPlayTimeSyncer.Load();
            EventQuestSyncer.Load();
            EventRankUpSyncer.Load();
            EventXmasSyncer.Load();
        }

        public static void ReloadEvent(int index)
        {
            if (index == 0)
            {
                EventVisitSyncer.ReGenerateList();
            }
            else if (index == 1)
            {
                EventLoginSyncer.ReGenerateList();
            }
            else if (index == 2)
            {
                EventMapSyncer.ReGenerateList();
            }
            else if (index == 3)
            {
                EventPlayTimeSyncer.ReGenerateList();
            }
            else if (index == 4)
            {
                EventQuestSyncer.ReGenerateList();
            }
            else if (index == 5)
            {
                EventRankUpSyncer.ReGenerateList();
            }
            else if (index == 6)
            {
                EventXmasSyncer.ReGenerateList();
            }
        }
    }
}