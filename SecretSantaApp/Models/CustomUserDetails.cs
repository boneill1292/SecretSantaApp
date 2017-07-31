using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
    public class CustomUserDetails
    {
        [Key]
        public int DetailsId { get; set; }

        public string UserAcctNo { get; set; }

        public int UserId { get; set; }

        public int ShoeSize { get; set; }
        public string ShirtSize { get; set; }

        public string PantsSize { get; set; }

        public string SweatShirtSize { get; set; }

        public string FavoriteBrands { get; set; }

        public string Notes { get; set; }
        


        public virtual void Update(CustomUserDetails c)
        {
            DetailsId = c.DetailsId;
            UserAcctNo = c.UserAcctNo;
            UserId = c.UserId;
            ShoeSize = c.ShoeSize;
            ShirtSize = c.ShirtSize;
            PantsSize = c.PantsSize;
            SweatShirtSize = c.SweatShirtSize;
            FavoriteBrands = c.FavoriteBrands;
            Notes = c.Notes;

        }
    }
}
