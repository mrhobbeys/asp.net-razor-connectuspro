using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using SiteBlue.Data.Audit;
using System.Transactions;

namespace SiteBlue.Data.EightHundred
{
    public class EightHundredEntities : EightHundredBaseContext
    {
        private readonly object _currUser;
        private static Dictionary<Type, Type> AuditedEntities;
        private Dictionary<AuditLog, IAudit[]> PendingAudits;

        public EightHundredEntities() : this(Guid.Empty)
        {
        }

        public EightHundredEntities(Guid userKey)
        {
            //if (userKey == Guid.Empty)
            //    throw new ArgumentException("Cannot be null or empty", "userKey");

            SavingChanges += Audit;
            _currUser = userKey;
        }

        public override int SaveChanges(SaveOptions options)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    var result = base.SaveChanges(options);

                    if (PendingAudits != null && PendingAudits.Count > 0)
                    {
                        foreach (var newAudit in PendingAudits)
                        {
                            if (newAudit.Key.KeyValue.EntityKeyValues == null)
                                newAudit.Key.KeyValue = newAudit.Key.EntityBeingAudited.EntityKey;

                            newAudit.Key.EntityID =
                                Convert.ToString(newAudit.Key.KeyValue.EntityKeyValues.First().Value);
                            AddObject(GetSetName(newAudit.Key), newAudit.Key);

                            foreach (var rec in newAudit.Value)
                            {
                                rec.AuditLog = newAudit.Key;
                                AddObject(GetSetName(rec as IEntityWithKey), rec);
                            }
                        }

                        result += base.SaveChanges(options);
                    }

                    scope.Complete();

                    return result;
                }
            }
            finally
            {
                if (PendingAudits != null)
                    PendingAudits.Clear();
            }
        }

        protected virtual void Audit(object sender, EventArgs e)
        {
            if (AuditedEntities == null)
            {
                var AuditedEntities1 = Assembly.GetExecutingAssembly()
                                          .GetReferencedAssemblies()
                                          .SelectMany(asm => Assembly.Load(asm).GetTypes())
                                          .Concat(Assembly.GetExecutingAssembly().GetTypes());
                var collAE = AuditedEntities1.ToList();
                for (int i = 0; i < collAE.Count; i++)
                {
                    try
                    {
                        var myType = collAE[i];
                        var myTypeI = myType.GetInterface(typeof(IAudit<>).FullName);
                    }
                    catch (Exception ex)
                    {
                        string s = ex.ToString();
                    }
                }
                
                var AuditedEntities2 = AuditedEntities1.Where(t => t.GetInterface(typeof(IAudit<>).FullName) != null);
                var AuditedEntities3 = AuditedEntities2.ToDictionary(t => 
                                                    t.GetInterface(typeof(IAudit<>).FullName).GetGenericArguments().First(), 
                                                    t => t
                                                );
                AuditedEntities = AuditedEntities3;
            }

            var changes = ObjectStateManager.GetObjectStateEntries(EntityState.Added).Concat(
            ObjectStateManager.GetObjectStateEntries(EntityState.Modified)).Concat(
            ObjectStateManager.GetObjectStateEntries(EntityState.Deleted))
            .Where(c => c.Entity.GetType().GetInterface(typeof(IAudit<>).FullName) == null && GetAuditType(c.Entity.GetType()) != null).ToArray();

            if (changes.Length == 0) return;

            PendingAudits = changes.Select(c => new {Audit = new AuditLog
                          {
                              AuditDate = DateTime.UtcNow,
                              Type = c.State.ToString(),
                              EntityType = c.Entity.GetType().Name,
                              UserKey = Convert.ToString(_currUser),
                              EntityBeingAudited = (EntityObject)c.Entity,
                              KeyValue = ((IEntityWithKey)c.Entity).EntityKey
                          }, Entries = c.GetModifiedProperties()
                                 .Where(fld => !Equals(c.OriginalValues[fld], c.CurrentValues[fld]) && string.Compare(fld, "timestamp", true) != 0)
                                 .Select(fld => GetAuditEntity(fld, c))
                                 .Where(ae => ae != null)
                                 .ToArray()}).Where(p => p.Entries.Length > 0 || p.Audit.EntityBeingAudited.EntityState != EntityState.Modified).ToDictionary(p => p.Audit, p => p.Entries);
        }

        private string GetSetName(IEntityWithKey entity)
        {
            if (entity.EntityKey != null) return entity.EntityKey.EntitySetName;

            var entityTypeName = entity.GetType().Name;
            var container = MetadataWorkspace.GetEntityContainer(DefaultContainerName, DataSpace.CSpace);
            var entitySetName = container.BaseEntitySets.First(meta => meta.ElementType.Name == entityTypeName).Name;

            return container.Name + "." + entitySetName;
        }

        private static Type GetAuditType(Type entityType)
        {
            Type type;
            AuditedEntities.TryGetValue(entityType, out type);
            return type;
        }

        private static IAudit GetAuditEntity(string fldName, ObjectStateEntry entry)
        {
            var auditType = GetAuditType(entry.Entity.GetType());
            var inst = (IAudit)Activator.CreateInstance(auditType);
            inst.Attribute = fldName;
            inst.NewValue = Convert.ToString(entry.CurrentValues[fldName]);
            inst.OldValue = Convert.ToString(entry.OriginalValues[fldName]);
            return inst;
        }
    }
}
