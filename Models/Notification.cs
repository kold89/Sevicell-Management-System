using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Notification
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    public string NotificationType { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime Date { get; set; }

    public int? RelatedId { get; set; }
}
