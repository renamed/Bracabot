using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using System.Text;

namespace Bracabot2.Commands
{
    public class HistogramCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private readonly ITwitchService twitchService;

        public HistogramCommand(IDotaService dotaService, ITwitchService twitchService)
        {
            this.dotaService = dotaService;
            this.twitchService = twitchService;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {
            if (!await twitchService.EhOJogoDeDota())
            {
                return "Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.";
            }

            if (!args.Any())
            {
                return "Use !histograma SEU_MMR para saber como você se compara aos demais players de MMR. Ex: !histograma 3200";
            }

            if (!int.TryParse(args.First(), out int mmr))
            {
                return "{args.First()} não parece um valor de MMR válido. Use !histograma SEU_MMR para saber como você se compara aos demais players de MMR. Ex: !histograma 3200";
            }

            if (mmr < 0)
            {
                return "MMR negativo não parece um valor válido. Use !histograma SEU_MMR para saber como você se compara aos demais players de MMR. Ex: !histograma 3200";
            }


            DotaApiMmrBucketResponse mmrBuckets = await dotaService.GetMmrBucketAsync();

            DotaApiMmrBucketRowsBinResponse mmrBucket = mmrBuckets.Mmr.Rows.Last();
            var lastMmrBucket = mmrBucket;
            for (int i = mmrBuckets.Mmr.Rows.Count - 2; i >= 0; i--)
            {
                var currentMmrBucket = mmrBuckets.Mmr.Rows.ElementAt(i);

                if (mmr >= currentMmrBucket.BinName && mmr <= lastMmrBucket.BinName)
                {
                    mmrBucket = currentMmrBucket;
                    break;
                }

                lastMmrBucket = currentMmrBucket;
            }

            var sb = new StringBuilder();

            double porc = 1.0 * (mmrBucket.CumulativeSum - mmrBucket.Count) / mmrBuckets.Mmr.Rows.Last().CumulativeSum;


            if (mmrBucket == mmrBuckets.Mmr.Rows.Last())
            {
                sb.Append($"Existem {mmrBucket.Count:N0} jogadores com MMR acima de {mmrBucket.BinName:N0} considerando todos os servers de Dota no mundo. ");
            }
            else
            {
                sb.Append($"Existem {mmrBucket.Count:N0} jogadores com MMR entre {mmrBucket.BinName:N0} e {lastMmrBucket.BinName:N0} considerando todos os servers de Dota no mundo. ");
            }

            sb.Append($"Seu MMR é maior que (aproximadamente) {mmrBucket.CumulativeSum - mmrBucket.Count:N0} jogadores. ");
            sb.Append($"Portanto, você está acima de {porc:P1} dos jogadores de Dota que calibraram seu MMR");

            return sb.ToString();
        }
    }
}
