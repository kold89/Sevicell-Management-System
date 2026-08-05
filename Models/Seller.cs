using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Seller
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string? Company { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
