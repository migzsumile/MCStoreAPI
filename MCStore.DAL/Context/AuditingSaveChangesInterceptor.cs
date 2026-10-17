using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.DAL.Context
{
    public class AuditingSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditingSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUser()
        {
            try
            {
                var user = _httpContextAccessor?.HttpContext?.User;
                if (user?.Identity?.IsAuthenticated == true)
                {
                    // Try common name claims in order of preference
                    var name = user.FindFirst("name")?.Value
                        ?? user.FindFirst("preferred_username")?.Value
                        ?? user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                        ?? user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                        ?? user.FindFirst("sub")?.Value;

                    if (!string.IsNullOrEmpty(name)) return name;
                }

                return "system";
            }
            catch
            {
                return "system";
            }
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context;
            if (context != null)
            {
                ApplyAuditInformation(context);
            }
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context != null)
            {
                ApplyAuditInformation(context);
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAuditInformation(DbContext context)
        {
            var now = DateTime.UtcNow;
            var user = GetCurrentUser();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    SetPropertyIfExists(entry, "CreatedBy", user);
                    SetPropertyIfExists(entry, "CreatedDate", now);
                    SetPropertyIfExists(entry, "UpdatedBy", user);
                    SetPropertyIfExists(entry, "UpdatedDate", now);
                }
                else if (entry.State == EntityState.Modified)
                {
                    SetPropertyIfExists(entry, "UpdatedBy", user);
                    SetPropertyIfExists(entry, "UpdatedDate", now);
                }
            }
        }

        private void SetPropertyIfExists(EntityEntry entry, string propertyName, object value)
        {
            var prop = entry.Metadata.FindProperty(propertyName);
            if (prop == null) return;

            // For shadow properties use Property(propertyName)
            try
            {
                entry.Property(propertyName).CurrentValue = value;
            }
            catch
            {
                // ignore if cannot set
            }
        }
    }
}
