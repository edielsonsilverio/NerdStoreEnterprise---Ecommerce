using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSE.Catalogo.API.Models;
using NSE.Core.DomainObjects;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;

namespace NSE.Catalogo.API.Services;

public class CatalogoIntegrationHandler : BackgroundService
{
    private readonly IMessageBus _bus;
    private IServiceProvider _serviceProvider;


    public CatalogoIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetSubscribers();
        return Task.CompletedTask;
    }

    private void SetSubscribers()
    {
        _bus.SubscribeAsync<PedidoAutorizadoIntegrationEvent>("PedidoAutorizado", async request =>
            await BaixarEstoque(request));
    }

    private async Task BaixarEstoque(PedidoAutorizadoIntegrationEvent message)
    {
        //Transforma o ServiceProvider em Scope, pois por padrão é Singleton.
        using (var scope = _serviceProvider.CreateScope())
        {
            var produtosComEstoque = new List<Produto>();
            var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

            var idsProdutos = string.Join(",", message.Itens.Select(c => c.Key));
            var produtos = await produtoRepository.ObterProdutosPorId(idsProdutos);

            //Verifica se os produtos do banco e da mensagem tem a mesma quantidades.
            if (produtos.Count != message.Itens.Count)
            {
                CancelarPedidoSemEstoque(message);
                return;
            }


            //Percorre dos projetos para verificação de estoque
            foreach (var produto in produtos)
            {
                var quantidadeProduto = message.Itens.FirstOrDefault(p => p.Key == produto.Id).Value;

                //Verifica se o produto está disponível.
                if (produto.EstaDisponivel(quantidadeProduto))
                {
                    produto.RetirarEstoque(quantidadeProduto);
                    produtosComEstoque.Add(produto);
                }
            }

            //Se algum produto não tiver quantidade, cancela o pedido.
            if (produtosComEstoque.Count != message.Itens.Count)
            {
                CancelarPedidoSemEstoque(message);
                return;
            }

            foreach (var produto in produtosComEstoque)
            {
                produtoRepository.Atualizar(produto);
            }

            //Caso não consiga salvar as informações, então lança uma exception para
            //interroper a mensagem.
            if (!await produtoRepository.UnitOfWork.Commit())
            {
                throw new DomainException($"Problemas ao atualizar estoque do pedido {message.PedidoId}");
            }


            //Envia a mensagem com as informações que o estoque foi baixado.
            var pedidoBaixado = new PedidoBaixadoEstoqueIntegrationEvent(message.ClienteId, message.PedidoId);
            await _bus.PublishAsync(pedidoBaixado);
        }
    }

    public async void CancelarPedidoSemEstoque(PedidoAutorizadoIntegrationEvent message)
    {
        var pedidoCancelado = new PedidoCanceladoIntegrationEvent(message.ClienteId, message.PedidoId);
        await _bus.PublishAsync(pedidoCancelado);
    }
}