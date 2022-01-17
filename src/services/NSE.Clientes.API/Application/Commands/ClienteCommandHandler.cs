using FluentValidation.Results;
using MediatR;
using NSE.Clientes.API.Application.Events;
using NSE.Clientes.API.Models;
using NSE.Core.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace NSE.Clientes.API.Application.Commands;

public class ClienteCommandHandler : CommandHandler,
    IRequestHandler<RegistrarClienteCommand, ValidationResult>,
    IRequestHandler<AdicionarEnderecoCommand, ValidationResult>
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteCommandHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ValidationResult> Handle(RegistrarClienteCommand message, CancellationToken cancellationToken)
    {
        //Verifica se existe erro.
        if (!message.EhValido()) return message.ValidationResult;

        //Cria um novo cliente.
        var cliente = new Cliente(message.Id,message.Nome,message.Email,message.Cpf);

        //Verifica se já existe um cliente com o número de cpf
        var clienteExistente = await _clienteRepository.ObterPorCpf(cliente.Cpf.Numero);

        //Se achar um cliente que já existe, adiciona um erro e retorna.
        if (clienteExistente != null)
        {
            AdicionarErro("Este CPF já está em uso.");
            return ValidationResult;
        }
        
        //Adiciona o novo cliente
        _clienteRepository.Adicionar(cliente);

        //Envio o evento caso sucesso
        cliente.AdicionarEvento(new ClienteRegistradoEvent(message.Id, message.Nome, message.Email, message.Cpf));

        //Salva as alterações e retorna.
        return await PersistirDados(_clienteRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(AdicionarEnderecoCommand message, CancellationToken cancellationToken)
    {
        if (!message.EhValido()) return message.ValidationResult;

        var endereco = new Endereco(message.Logradouro, message.Numero, message.Complemento, message.Bairro, message.Cep, message.Cidade, message.Estado, message.ClienteId);
        _clienteRepository.AdicionarEndereco(endereco);

        return await PersistirDados(_clienteRepository.UnitOfWork);
    }
}