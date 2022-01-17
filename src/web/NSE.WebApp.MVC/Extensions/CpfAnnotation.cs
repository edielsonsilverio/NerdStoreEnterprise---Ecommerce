using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using NSE.Core.DomainObjects;

namespace NSE.WebApp.MVC.Extensions;

//Classe para criar um atributo de validação na model.
public class CpfAttribute : ValidationAttribute
{
    //Não confundir [ValidationResult] do FluentValidation
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        return Cpf.Validar(value.ToString()) 
                    ? ValidationResult.Success : new ValidationResult("CPF em formato inválido");
    }
}

//Classe para criar um adapitador no front-end de validação
public class CpfAttributeAdapter : AttributeAdapterBase<CpfAttribute>
{
    public CpfAttributeAdapter(CpfAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
    {

    }

    //Método para adicionar a validação no HTML
    public override void AddValidation(ClientModelValidationContext context)
    {
        if(context == null) 
            throw new ArgumentNullException(nameof(context));

        //Criação da tab html
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes,"data-val-cpf",GetErrorMessage(context));
    }

    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return "CPF em formato inválido";
    }
}


//Classe que vai ativar a tag com a mensagem de erro no model.
public class CpfValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();  
    public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
    {
        //Se o tipo do atributo informado for cpf, então retorna
        if (attribute is CpfAttribute CpfAttribute)
            return new CpfAttributeAdapter(CpfAttribute, stringLocalizer);

        return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
    }
}