namespace SecretSantaApp.ViewModels
{
    public class InviteUsersEditModel
    {
        public bool Saved { get; set; }
        //public bool NewGroup { get; set; }

        public int GroupId { get; set; }

        public string GroupUrl { get; set; }
        //public string GroupName { get; set; }
        public InviteUsersCollectionModel InviteUsersCollection { get; set; }

        //public string ViewString { get; set; }
    }
}