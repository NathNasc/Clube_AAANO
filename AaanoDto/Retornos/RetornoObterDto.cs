using AaanoDto.Base;

namespace AaanoDto.Retornos
{
    public class RetornoObterDto<T> : RetornoDto where T : BaseEntidadeDto
    {
        public T Entidade { get; set; }
    }
}
