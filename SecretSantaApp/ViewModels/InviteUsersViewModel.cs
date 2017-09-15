namespace SecretSantaApp.ViewModels
{
    public class InviteUsersViewModel
    {
        public int TempId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string GroupUrl { get; set; }
    }
}