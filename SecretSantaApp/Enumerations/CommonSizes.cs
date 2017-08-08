using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Enumerations
{
    public enum CommonSizes
    {
        [Display(Name = "XS")]
        Xs = 1,
        [Display(Name = "S")]
        Sm = 2,
        [Display(Name = "M")]
        Md = 3,
        [Display(Name = "L")]
        Lg = 4,
        [Display(Name = "XL")]
        XLg = 5,
        [Display(Name = "2XL")]
        XxLg = 6,
        [Display(Name = "3 XL")]
        XxxLg = 7,
        [Display(Name = "4 XL")]
        XxxxLg = 8


    }
}
