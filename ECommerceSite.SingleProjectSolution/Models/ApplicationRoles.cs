using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSite.SingleProjectSolution.Models
{
    public class ApplicationRoles : IdentityRole<int>
    { 
        public ApplicationRoles() : base()
        {

        }

        public ApplicationRoles(String name) : base(name)
        {

        }
    }
}
