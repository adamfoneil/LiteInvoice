﻿using Database.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LiteInvoice.Database;

/// <summary>
/// Collection of hours, expenses, and invoices for a customer.
/// </summary>
public class Project : BaseEntity
{
	public int CustomerId { get; set; }
	public string Name { get; set; } = default!;
	public string? Description { get; set; } = default!;
	public decimal HourlyRate { get; set; }
	/// <summary>
	/// if true, then project cannot be invoiced, but instead is used to hold template entries
	/// </summary>
	public bool IsTemplate { get; set; }
	/// <summary>
	/// allow hours and invoicing
	/// </summary>
	public bool IsActive { get; set; } = true;

	public Customer Customer { get; set; } = default!;
	public ICollection<Invoice> Invoices { get; set; } = [];
	public ICollection<HoursEntry> Hours { get; set; } = [];
	public ICollection<ExpenseEntry> Expenses { get; set; } = [];	
	public ICollection<ScheduledInvoice> ScheduledInvoices { get; set; } = [];
	public ICollection<ScheduledInvoice> ScheduledTemplateInvoices { get; set; } = [];
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
	public void Configure(EntityTypeBuilder<Project> builder)
	{
		builder.HasIndex(e => new { e.CustomerId, e.Name }).IsUnique();
		builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
		builder.Property(e => e.HourlyRate).HasColumnType("money");		
		builder.HasOne(e => e.Customer).WithMany(e => e.Projects).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
		builder.Property(e => e.Description).HasMaxLength(255);		
	}
}
