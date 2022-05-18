using System.Threading.Tasks;

namespace Valutakalkulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var conversionService = new ConversionService();
            await conversionService.ExecuteAsync();
        }
    }
}