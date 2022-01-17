using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Catalogo.API.Models;

namespace NSE.Catalogo.API.Data.Mappings;
public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{

    //Precisa-se mapear os tipos string e decimal quando quer informar o tamanho.
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.Descricao)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Imagem)
            .IsRequired()
            .HasMaxLength(250);

        builder.ToTable("Produtos");
    }
}