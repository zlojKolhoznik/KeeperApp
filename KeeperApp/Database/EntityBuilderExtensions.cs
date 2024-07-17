﻿using KeeperApp.Security;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Database
{
    public static class EntityBuilderExtensions
    {
        public static void HasEncryption<T>(this EntityTypeBuilder<T> entity) where T : class
        {
            var encryptedProperties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(EncryptPropertyAttribute)));
            foreach (var prop in encryptedProperties)
            {
                entity.Property(prop.Name).HasConversion<EncryptionConverter>();
            }
        }
    }
}