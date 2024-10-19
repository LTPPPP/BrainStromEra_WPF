using System;
using System.Collections.Generic;

namespace BrainStormEra_WPF.Models;

public partial class Role
{
    public int UserRole { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
