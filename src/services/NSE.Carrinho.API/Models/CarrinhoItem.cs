using FluentValidation;
using System;
using System.Text.Json.Serialization;

namespace NSE.Carrinho.API.Models;

public class CarrinhoItem
{
    public CarrinhoItem()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public string Nome { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
    public string Imagem { get; set; }

    public Guid CarrinhoId { get; set; }

    [JsonIgnore]
    public CarrinhoCliente CarrinhoCliente { get; set; }


    //Método para associar o id do carrinho.
    internal void AssocinarCarrinho(Guid carrinhoId)
    {
        CarrinhoId = carrinhoId;
    }

    //Método para calcular o valor total;
    internal decimal CalcularValor()
    {
        return Quantidade * Valor;
    }

    //Método para adicionar unidade 
    internal void AdicionarUnidades(int unidades)
    {
        Quantidade += unidades;
    }

    //Método para atualizar a unidade.
    internal void AtualizarUnidades(int unidades)
    {
        Quantidade = unidades;
    }

    //Método para verificar se os campos obrigatórios estão válidos.
    internal bool EhValido()
    {
        //Método do FluentValidation.
        return new ItemCarrinhoValidation().Validate(this).IsValid;
    }

    //Classe aninhada de validação.
    public class ItemCarrinhoValidation : AbstractValidator<CarrinhoItem>
    {
        public ItemCarrinhoValidation()
        {
            RuleFor(c => c.ProdutoId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do produto inválido");

            RuleFor(c => c.Nome)
                .NotEmpty()
                .WithMessage("O nome do produto não foi informado");

            RuleFor(c => c.Quantidade)
                .GreaterThan(0)
                .WithMessage(item => $"A quantidade miníma para o {item.Nome} é 1");

            RuleFor(c => c.Quantidade)
                .LessThanOrEqualTo(CarrinhoCliente.MAX_QUANTIDADE_ITEM)
                .WithMessage(item => $"A quantidade máxima do {item.Nome} é {CarrinhoCliente.MAX_QUANTIDADE_ITEM}");

            RuleFor(c => c.Valor)
                .GreaterThan(0)
                .WithMessage(item => $"O valor do {item.Nome} precisa ser maior que 0");
        }
    }
}

