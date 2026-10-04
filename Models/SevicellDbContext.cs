using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WpfApp1.Data;

namespace WpfApp1.Models;

public partial class SevicellDbContext : DbContext
{
    public SevicellDbContext()
    {
    }

    public SevicellDbContext(DbContextOptions<SevicellDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditTable> AuditTables { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<ContractPayment> ContractPayments { get; set; }

    public virtual DbSet<ContractStatus> ContractStatuses { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustumerStatus> CustumerStatuses { get; set; }

    public virtual DbSet<DebtInstallment> DebtInstallments { get; set; }

    public virtual DbSet<DebtInstallmentStatus> DebtInstallmentStatuses { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Frequency> Frequencies { get; set; }

    public virtual DbSet<InstallmentPayment> InstallmentPayments { get; set; }

    public virtual DbSet<InventoryMovement> InventoryMovements { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PhoneStatus> PhoneStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductUnit> ProductUnits { get; set; }

    public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }

    public virtual DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }

    public virtual DbSet<RepairDetail> RepairDetails { get; set; }

    public virtual DbSet<RepairOrder> RepairOrders { get; set; }

    public virtual DbSet<RepairPhoto> RepairPhotos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SalesDetail> SalesDetails { get; set; }

    public virtual DbSet<SalesInvoice> SalesInvoices { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Technician> Technicians { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(DbSettings.ConnectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditTab__3214EC07BD572F0A");

            entity.ToTable("AuditTable");

            entity.Property(e => e.Accion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AffectedTable)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DateCreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("dateCreate");
            entity.Property(e => e.ObjectId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.AuditTables)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auditoria_Usuarios");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brand__3214EC070548E4F0");

            entity.ToTable("Brand");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC07C87413D0");

            entity.ToTable("Category");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__contract__3213E83F97471736");

            entity.ToTable("contract");

            entity.HasIndex(e => e.ClientId, "idx_contract_client");

            entity.HasIndex(e => e.FirstDueDate, "idx_contract_due_date");

            entity.HasIndex(e => e.SellerId, "idx_contract_seller");

            entity.HasIndex(e => e.StatusId, "idx_contract_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DownPayment)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("down_payment");
            entity.Property(e => e.FinancedBalance)
                .HasComputedColumnSql("([sale_price]-[down_payment])", true)
                .HasColumnType("decimal(13, 2)")
                .HasColumnName("financed_balance");
            entity.Property(e => e.FirstDueDate).HasColumnName("first_due_date");
            entity.Property(e => e.FrequencyId).HasColumnName("frequency_id");
            entity.Property(e => e.InstallmentAmount)
                .HasComputedColumnSql("(case when [installment_count]>(0) then ([sale_price]-[down_payment])/[installment_count] else (0) end)", true)
                .HasColumnType("decimal(24, 13)")
                .HasColumnName("installment_amount");
            entity.Property(e => e.InstallmentCount).HasColumnName("installment_count");
            entity.Property(e => e.LastDueDate).HasColumnName("last_due_date");
            entity.Property(e => e.LateInterestRate)
                .HasDefaultValue(0.0000m)
                .HasColumnType("decimal(5, 4)")
                .HasColumnName("late_interest_rate");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PendingBalance)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("pending_balance");
            entity.Property(e => e.ProductUnitId).HasColumnName("product_unit_id");
            entity.Property(e => e.SalePrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("sale_price");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TotalDebt)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_debt");

            entity.HasOne(d => d.Client).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contract_client");

            entity.HasOne(d => d.Frequency).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.FrequencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contract_frequency");

            entity.HasOne(d => d.ProductUnit).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ProductUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contract_product_unit");

            entity.HasOne(d => d.Seller).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contract_seller");

            entity.HasOne(d => d.Status).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contract_status");
        });

        modelBuilder.Entity<ContractPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__contract__3213E83F5AAD4D53");

            entity.ToTable("contract_payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.InstallmentId).HasColumnName("installment_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.ReceivedBy).HasColumnName("received_by");

            entity.HasOne(d => d.Contract).WithMany(p => p.ContractPayments)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cp_contract");

            entity.HasOne(d => d.Installment).WithMany(p => p.ContractPayments)
                .HasForeignKey(d => d.InstallmentId)
                .HasConstraintName("fk_cp_installment");
        });

        modelBuilder.Entity<ContractStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__contract__3213E83F7B38F2D2");

            entity.ToTable("contract_status");

            entity.HasIndex(e => e.Code, "UQ__contract__357D4CF9D945CE13").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.ColorBadge)
                .HasMaxLength(7)
                .HasColumnName("color_badge");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.FlowOrder).HasColumnName("flow_order");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07386889BA");

            entity.ToTable("Customer");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Dni)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("DNI");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<CustumerStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__custumer__3213E83FD95E5C3D");

            entity.ToTable("custumer_status");

            entity.HasIndex(e => e.Code, "UQ__custumer__357D4CF99F83EB9D").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
        });

        modelBuilder.Entity<DebtInstallment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__debt_ins__3213E83FCED5D0BA");

            entity.ToTable("debt_installment", tb =>
                {
                    tb.HasTrigger("trg_installment_delete");
                    tb.HasTrigger("trg_installment_paid");
                });

            entity.HasIndex(e => e.ContractId, "idx_installment_contract");

            entity.HasIndex(e => e.DueDate, "idx_installment_due_date");

            entity.HasIndex(e => e.StatusId, "idx_installment_status");

            entity.HasIndex(e => new { e.ContractId, e.InstallmentNumber }, "uq_contract_installment").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.ExpectedAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("expected_amount");
            entity.Property(e => e.InstallmentNumber).HasColumnName("installment_number");
            entity.Property(e => e.LateInterest)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("late_interest");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PaidAmount)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("paid_amount");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.ReceivedBy).HasColumnName("received_by");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Contract).WithMany(p => p.DebtInstallments)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("fk_installment_contract");

            entity.HasOne(d => d.ReceivedByNavigation).WithMany(p => p.DebtInstallments)
                .HasForeignKey(d => d.ReceivedBy)
                .HasConstraintName("fk_installment_received_by");

            entity.HasOne(d => d.Status).WithMany(p => p.DebtInstallments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_installment_status");
        });

        modelBuilder.Entity<DebtInstallmentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__debt_ins__3213E83F171DABD8");

            entity.ToTable("debt_installment_status");

            entity.HasIndex(e => e.Code, "UQ__debt_ins__357D4CF9E4E8B8AD").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsExpired)
                .HasDefaultValue(false)
                .HasColumnName("is_expired");
            entity.Property(e => e.IsPaid)
                .HasDefaultValue(false)
                .HasColumnName("is_paid");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Device__3214EC07B9D1403B");

            entity.ToTable("Device");

            entity.Property(e => e.Imei)
                .HasMaxLength(50)
                .HasColumnName("IMEI");
            entity.Property(e => e.Model).HasMaxLength(100);

            entity.HasOne(d => d.Brand).WithMany(p => p.Devices)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK__Device__BrandId__47DBAE45");

            entity.HasOne(d => d.Customer).WithMany(p => p.Devices)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Device__Customer__46E78A0C");
        });

        modelBuilder.Entity<Frequency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__frequenc__3213E83F5B98DBE4");

            entity.ToTable("frequency");

            entity.HasIndex(e => e.Code, "UQ__frequenc__357D4CF94BCA73B5").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DaysPeriod).HasColumnName("days_period");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
        });

        modelBuilder.Entity<InstallmentPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__installm__3213E83F8077D87B");

            entity.ToTable("installment_payments");

            entity.HasIndex(e => e.InstallmentId, "IX_InstallmentPayments_InstallmentId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.InstallmentId).HasColumnName("installment_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.ReceivedBy)
                .HasMaxLength(100)
                .HasColumnName("received_by");

            entity.HasOne(d => d.Installment).WithMany(p => p.InstallmentPayments)
                .HasForeignKey(d => d.InstallmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InstallmentPayments_Installment");
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC0704C9144E");

            entity.ToTable("InventoryMovement");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.MovementDate).HasColumnType("datetime");
            entity.Property(e => e.MovementType).HasMaxLength(50);

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Inventory__Produ__571DF1D5");

            entity.HasOne(d => d.User).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Inventory__UserI__5812160E");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07E33A2015");

            entity.ToTable("Notification");

            entity.Property(e => e.Body).HasMaxLength(300);
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NotificationType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderSta__3214EC07C038CE36");

            entity.ToTable("OrderStatus");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC07E0777428");

            entity.ToTable("Payment");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK__Payment__Payment__7B5B524B");

            entity.HasOne(d => d.SalesInvoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SalesInvoiceId)
                .HasConstraintName("FK__Payment__SalesIn__7A672E12");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Payment__UserId__7C4F7684");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC078AA458DA");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC077B760297");

            entity.ToTable("Permission");

            entity.HasIndex(e => e.Code, "UQ__Permissi__A25C5AA7579E95E3").IsUnique();

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Modulo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("General");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<PhoneStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__phone_st__3213E83F5AB361F2");

            entity.ToTable("phone_status");

            entity.HasIndex(e => e.Code, "UQ__phone_st__357D4CF9556EBA75").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC075309AE28");

            entity.ToTable("Product");

            entity.HasIndex(e => e.Code, "UQ_Code").IsUnique();

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ProductDescription)
                .HasMaxLength(250)
                .HasColumnName("productDescription");
            entity.Property(e => e.SalePrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK__Product__BrandId__534D60F1");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Product__Categor__5441852A");
        });

        modelBuilder.Entity<ProductUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductU__3214EC077FD3C21D");

            entity.ToTable("ProductUnit");

            entity.HasIndex(e => e.Imei, "UQ_ProductUnit_IMEI").IsUnique();

            entity.Property(e => e.Colour)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COLOUR");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Imei)
                .HasMaxLength(50)
                .HasColumnName("IMEI");
            entity.Property(e => e.Imei2)
                .HasMaxLength(50)
                .HasColumnName("IMEI2");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MODEL");
            entity.Property(e => e.SerialNumber).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Disponible");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductUnits)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductUn__Produ__2CF2ADDF");

            entity.HasOne(d => d.PurchaseDetail).WithMany(p => p.ProductUnits)
                .HasForeignKey(d => d.PurchaseDetailId)
                .HasConstraintName("FK__ProductUn__Purch__2DE6D218");

            entity.HasOne(d => d.SalesDetail).WithMany(p => p.ProductUnits)
                .HasForeignKey(d => d.SalesDetailId)
                .HasConstraintName("FK__ProductUn__Sales__2EDAF651");
        });

        modelBuilder.Entity<PurchaseDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Purchase__3214EC07D3603F87");

            entity.ToTable("PurchaseDetail");

            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TaxPercent).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__PurchaseD__Produ__60A75C0F");

            entity.HasOne(d => d.PurchaseInvoice).WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.PurchaseInvoiceId)
                .HasConstraintName("FK__PurchaseD__Purch__5FB337D6");
        });

        modelBuilder.Entity<PurchaseInvoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Purchase__3214EC079B3B6A61");

            entity.ToTable("PurchaseInvoice");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SupplierNameCasual).HasMaxLength(50);
            entity.Property(e => e.Tax).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseInvoices)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__PurchaseI__Suppl__5CD6CB2B");
        });

        modelBuilder.Entity<RepairDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RepairDe__3214EC07F41E43E3");

            entity.ToTable("RepairDetail");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.LaborCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PartCost).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.RepairOrder).WithMany(p => p.RepairDetails)
                .HasForeignKey(d => d.RepairOrderId)
                .HasConstraintName("FK__RepairDet__Repai__68487DD7");

            entity.HasOne(d => d.Service).WithMany(p => p.RepairDetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__RepairDet__Servi__693CA210");
        });

        modelBuilder.Entity<RepairOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RepairOr__3214EC070EEC8C8A");

            entity.ToTable("RepairOrder");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Device).WithMany(p => p.RepairOrders)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("FK__RepairOrd__Devic__6383C8BA");

            entity.HasOne(d => d.Status).WithMany(p => p.RepairOrders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__RepairOrd__Statu__656C112C");

            entity.HasOne(d => d.Technician).WithMany(p => p.RepairOrders)
                .HasForeignKey(d => d.TechnicianId)
                .HasConstraintName("FK__RepairOrd__Techn__6477ECF3");
        });

        modelBuilder.Entity<RepairPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RepairPh__3214EC07DC69806C");

            entity.ToTable("RepairPhoto");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.FilePath).HasMaxLength(300);

            entity.HasOne(d => d.RepairOrder).WithMany(p => p.RepairPhotos)
                .HasForeignKey(d => d.RepairOrderId)
                .HasConstraintName("FK__RepairPho__Repai__6D0D32F4");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC074574EB3B");

            entity.ToTable("Role");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId }).HasName("PK__RolePerm__6400A1A8941518DA");

            entity.ToTable("RolePermission");

            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RolePermi__Permi__3C69FB99");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RolePermi__RoleI__3B75D760");
        });

        modelBuilder.Entity<SalesDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesDet__3214EC079B168FA2");

            entity.ToTable("SalesDetail");

            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TaxPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.SalesDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__SalesDeta__Produ__778AC167");

            entity.HasOne(d => d.SalesInvoice).WithMany(p => p.SalesDetails)
                .HasForeignKey(d => d.SalesInvoiceId)
                .HasConstraintName("FK__SalesDeta__Sales__76969D2E");
        });

        modelBuilder.Entity<SalesInvoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalesInv__3214EC07D645EBBC");

            entity.ToTable("SalesInvoice");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Tax).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesInvoices)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__SalesInvo__Custo__71D1E811");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.SalesInvoices)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK__SalesInvo__Payme__72C60C4A");

            entity.HasOne(d => d.RepairOrder).WithMany(p => p.SalesInvoices)
                .HasForeignKey(d => d.RepairOrderId)
                .HasConstraintName("FK__SalesInvo__Repai__73BA3083");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__seller__3213E83FB575EE71");

            entity.ToTable("seller");

            entity.HasIndex(e => e.Dni, "UQ__seller__D87608A7EF42684A").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Company)
                .HasMaxLength(150)
                .HasColumnName("company");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Dni)
                .HasMaxLength(20)
                .HasColumnName("dni");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Service__3214EC07DFABDECA");

            entity.ToTable("Service");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplier__3214EC070C112608");

            entity.ToTable("Supplier");

            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Technician>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Technici__3214EC07DE9C5048");

            entity.ToTable("Technician");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07B456F07D");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E4F5769905").IsUnique();

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__RoleId__403A8C7D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
