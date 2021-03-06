﻿using System.Collections.Generic;
using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class GroupHomeEditModel : Group

    {
        public bool Saved { get; set; }
        public bool NewGroup { get; set; }
        public List<CustomUserEditModel> GroupMembers { get; set; }

        public List<GroupMembership> GroupMembershipModelList { get; set; }

        //Need the ID of this field to add stipulations
        public InviteUsersCollectionModel InviteUsersCollection { get; set; }

        public bool GroupAdminBool { get; set; }
        public CustomUser GroupAdmin { get; set; }

        public List<MemberConditions> GroupConditions { get; set; }

        public bool PairingsAssigned { get; set; }

        public bool IsAuthorized { get; set; }
    }
}