using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSE.Carrinho.API.Models;
public class CarrinhoCliente
{
    internal const int MAX_QUANTIDADE_ITEM = 5;

    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public decimal ValorTotal { get; set; }
    public List<CarrinhoItem> Itens { get; set; } = new List<CarrinhoItem>();
    public ValidationResult ValidationResult { get; set; }

    public bool VoucherUtilizado { get; set; }
    public decimal Desconto { get; set; }

    public Voucher Voucher { get; set; }

    public CarrinhoCliente(Guid clienteId)
    {
        Id = Guid.NewGuid();
        ClienteId = clienteId;
    }

    public CarrinhoCliente() { }

    public void AplicarVoucher(Voucher voucher)
    {
        Voucher = voucher;
        VoucherUtilizado = true;
        CalcularValorCarrinho();
    }


    //Método para calcular o valor total do carrinho.
    internal void CalcularValorCarrinho()
    {
        ValorTotal = Itens.Sum(p => p.CalcularValor());
        CalcularValorTotalDesconto();
    }

    private void CalcularValorTotalDesconto()
    {
        if (!VoucherUtilizado) return;

        decimal desconto = 0;
        var valor = ValorTotal;

        if (Voucher.TipoDesconto == TipoDescontoVoucher.Porcentagem)
        {
            if (Voucher.Percentual.HasValue)
            {
                desconto = (valor * Voucher.Percentual.Value) / 100;
                valor -= desconto;
            }
        }
        else
        {
            if (Voucher.ValorDesconto.HasValue)
            {
                desconto = Voucher.ValorDesconto.Value;
                valor -= desconto;
            }
        }

        ValorTotal = valor < 0 ? 0 : valor;
        Desconto = desconto;
    }

    //Mètodo para verificar se o item já existe no carrinho.
    internal bool CarrinhoItemExistente(CarrinhoItem item)
    {
        return Itens.Any(p => p.ProdutoId == item.ProdutoId);
    }

    //Método para obter o item do carrinho através do produtoId
    internal CarrinhoItem ObterPorProdutoId(Guid produtoId)
    {
        return Itens.FirstOrDefault(p => p.ProdutoId == produtoId);
    }

    //Método para adicionar um item no carrinho.
    internal void AdicionarItem(CarrinhoItem item)
    {
        //Assiocia o item do carrinho.
        item.AssocinarCarrinho(Id);


        //Verifica se o item já existe e atualiza as informações.
        if (CarrinhoItemExistente(item))
        {
            var itemExistente = ObterPorProdutoId(item.ProdutoId);
            itemExistente.AdicionarUnidades(item.Quantidade);

            //Definine as novas informações e remove as antigas.
            item = itemExistente;
            Itens.Remove(itemExistente);
        }

        Itens.Add(item);
        CalcularValorCarrinho();
    }

    //Método para atuliazar os itens do Carrinho.
    internal void AtualizarItem(CarrinhoItem item)
    {
        //Assiocia o item do carrinho.
        item.AssocinarCarrinho(Id);

        var itemExistente = ObterPorProdutoId(item.ProdutoId);

        Itens.Remove(itemExistente);
        Itens.Add(item);

        CalcularValorCarrinho();
    }

    //Método para atualizar as Unidades
    internal void AtualizarUnidades(CarrinhoItem item, int unidades)
    {
        item.AtualizarUnidades(unidades);
        AtualizarItem(item);
    }

    //Método para remover Item do carrinho.
    internal void RemoverItem(CarrinhoItem item)
    {
        Itens.Remove(ObterPorProdutoId(item.ProdutoId));
        CalcularValorCarrinho();
    }

    internal bool EhValido()
    {
        //Obtêm a lista dos erros.

        //Obtêm os erros de validação do carrinho item
        var erros = Itens.SelectMany(i => new CarrinhoItem.ItemCarrinhoValidation().Validate(i).Errors).ToList();

        //Adiciona o erro do carrinho item aos erros do carrinho cliente.
        erros.AddRange(new CarrinhoClienteValidation().Validate(this).Errors);
        ValidationResult = new ValidationResult(erros);

        return ValidationResult.IsValid;
    }

    //Classe aninhada de Validação
    public class CarrinhoClienteValidation : AbstractValidator<CarrinhoCliente>
    {
        public CarrinhoClienteValidation()
        {
            RuleFor(c => c.ClienteId)
                .NotEqual(Guid.Empty)
                .WithMessage("Cliente não reconhecido");

            RuleFor(c => c.Itens.Count)
                .GreaterThan(0)
                .WithMessage("O carrinho não possui itens");

            RuleFor(c => c.ValorTotal)
                .GreaterThan(0)
                .WithMessage("O valor total do carrinho precisa ser maior que 0");
        }
    }
}
