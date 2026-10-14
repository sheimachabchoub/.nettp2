namespace WebApplication2.Models
{
    public static class OrderStatus
    {
        public const string EnAttente = "En attente";
        public const string Validee = "Validée";
        public const string Expediee = "Expédiée";
        public const string Livree = "Livrée";
        public const string Annulee = "Annulée";
        public static readonly string[] Tous = { EnAttente, Validee, Expediee, Livree, Annulee };

        public static string Couleur(string statut) => statut switch
        {
            EnAttente => "warning", Validee => "info", Expediee => "primary",
            Livree => "success", Annulee => "danger", _ => "secondary"
        };
    }
}
