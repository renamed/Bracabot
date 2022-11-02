using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;

namespace Bracabot2.Commands
{
    public class PeerCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private readonly ITwitchService twitchService;
        private readonly SettingsOptions options;

        public PeerCommand(IDotaService dotaService, ITwitchService twitchService, IOptions<SettingsOptions> options)
        {
            this.dotaService = dotaService;
            this.twitchService = twitchService;
            this.options = options.Value;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {
            var dotaId = options.DotaId;            

            if (!await twitchService.IsCurrentGameDota2())
            {
                return "Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.";
            }

            if (args == null)
            {
                return "Informe seu ID do dota ou nome da steam para visualizar sua estatísticas. Ex: !party [renamede] ou !party 420556150";
            }

            var peerId = string.Join(" ", args);
            if (string.IsNullOrWhiteSpace(peerId))
            {
                return "Informe seu ID do dota ou nome da steam para visualizar sua estatísticas. Ex: !party [renamede] ou !party 420556150";
            }

            var peer = await dotaService.GetPeersAsync(dotaId, peerId);
            if (peer == null)
            {
                return $"Não encontrei nenhum jogo com o identificador {peerId}. Informe seu ID do dota ou nome da steam para visualizar sua estatísticas. Ex: !party [renamede] ou !party 420556150";
            }

            var lastPlayed = DateTimeOffset.FromUnixTimeSeconds(peer.LastPlayed);
            var lastPlayedBrasilia = TimeZoneInfo.ConvertTimeFromUtc(lastPlayed.DateTime, TimeZoneInfoExtension.GetBrasiliaTimeZone());
            return $"{peer.Personaname} jogou {peer.WithGames} vez(es) com {options.ChannelName}, com {peer.WithWin} vitória(s). Último jogo em {lastPlayedBrasilia:dd/MM/yyyy à\\s HH:mm:ss}. (Essas estatísticas podem não estar corretas se você desabilitou a opção de Exportar Dados da sua conta nas configurações do jogo de Dota 2.)";
        }
    }
}
