namespace PointBlank
{
    public enum ClanAuthorityEnum
    {
        None,
        Master,
        Auxiliar,
        Membro
    }     

    public enum ClanAuthorityConfigEnum
    {
        Sem_Autoridade = 0,
        Autoridade_de_administrar_cadastros = 1,
        Autoridade_de_expulsar_membro = 2,
        Autoridade_de_convidar_ao_clã = 4,
        Autoridade_de_administração_de_posts = 8,
    }
}