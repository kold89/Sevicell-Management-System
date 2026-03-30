using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class RepairOrder
{
    public int Id { get; set; }

    public int? DeviceId { get; set; }

    public int? TechnicianId { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Device? Device { get; set; }

    public virtual ICollection<RepairDetail> RepairDetails { get; set; } = new List<RepairDetail>();

    public virtual ICollection<RepairPhoto> RepairPhotos { get; set; } = new List<RepairPhoto>();

    public virtual ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();

    public virtual OrderStatus? Status { get; set; }

    public virtual Technician? Technician { get; set; }
}
