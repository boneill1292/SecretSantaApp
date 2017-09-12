namespace SecretSantaApp.ViewModels
{
    public class InviteUsersEditModel
    {
        public bool NewGroup { get; set; }

        public int GroupId { get; set; }

        public InviteUsersCollectionModel InviteUsersCollection { get; set; }
    }
}