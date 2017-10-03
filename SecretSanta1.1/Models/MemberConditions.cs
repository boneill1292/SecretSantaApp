using System.ComponentModel.DataAnnotations;

namespace SecretSantaApp.Models
{
    public class MemberConditions
    {
        [Key]
        public int ConditionId { get; set; }

        public int GroupId { get; set; }

        public string UserReceivingConditionAcctNo { get; set; }

        public string UserSelectedForConditionAcctNo { get; set; }

        public int ConditionType { get; set; } //Use this later to determine 'CAN HAVE' 'CANT HAVE'

        public string InsertedBy { get; set; }

        public virtual void Update(MemberConditions mc)
        {
            ConditionId = mc.ConditionId;
            GroupId = mc.GroupId;
            UserReceivingConditionAcctNo = mc.UserReceivingConditionAcctNo;
            UserSelectedForConditionAcctNo = mc.UserSelectedForConditionAcctNo;
            ConditionType = mc.ConditionType;
            InsertedBy = mc.InsertedBy;
        }
    }
}