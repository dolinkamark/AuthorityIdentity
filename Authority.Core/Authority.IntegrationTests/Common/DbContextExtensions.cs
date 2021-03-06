﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using AuthorityIdentity.DomainModel;

namespace AuthorityIdentity.IntegrationTests.Common
{
    public static class DbContextExtensions
    {
        public static TEntity FindEntityInChangeTracker<TEntity>(this DbContext context, Guid id)
            where TEntity : EntityBase
        {
            IEnumerable<DbEntityEntry> entries = context.ChangeTracker.Entries();
            EntityBase entity = entries.Select(e => e.Entity).OfType<EntityBase>().First(e => e.Id == id);
            return entity as TEntity;
        }

        public static TEntity ReloadEntity<TEntity>(this DbContext context, Guid id)
            where TEntity : EntityBase
        {
            IEnumerable<DbEntityEntry> entries = context.ChangeTracker.Entries();
            DbEntityEntry entry = entries.FirstOrDefault(e => (e.Entity as EntityBase).Id == id);

            if (entry == null)
            {
                return null;
            }

            entry.Reload();
            return entry.Entity as TEntity;
        }
    }
}
