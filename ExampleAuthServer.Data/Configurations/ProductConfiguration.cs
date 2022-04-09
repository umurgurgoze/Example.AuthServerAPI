using ExampleAuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAuthServer.Data.Configurations
{
    //1.Core katmanında Data Annotation olarak key tanımlanabilir. Kullanılmamalı.
    //2.Id verdiğimizde EntityFramework ProductId veya Id yazılımlarını otomatik olarak PrimaryKey tanımlar.
    //3.On model builder içerisinden ya da IEntityTypeConfiguration'dan kalıtım alan bir class üzerinden tanımlama.BestPractice*
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.Price).HasColumnType("decimal(18,2");
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}
