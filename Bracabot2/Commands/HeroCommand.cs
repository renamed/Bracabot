﻿using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public class HeroCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private readonly ITwitchService twitchService;

        public HeroCommand(IDotaService dotaService, ITwitchService twitchService)
        {
            this.dotaService = dotaService;
            this.twitchService = twitchService;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {
            
            var dotaId = Environment.GetEnvironmentVariable("DOTA_ID");

            if (!await twitchService.EhOJogoDeDota())
            {
                return "Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.";
            }

            if (!args.Any())
            {
                return "Informe um herói para que eu possa lhe mostrar algumas estatísticas. Por exemplo, !heroi aa";
            }

            var nomeParametro = string.Join(" ", args);
            var idHero = await dotaService.GetIdAsync(nomeParametro);
            if (idHero == null)
            {
                return $"Não conheço o herói chamado {nomeParametro}. Você digitou o nome certo?";                
            }

            var hero = await dotaService.GetHeroAsync(dotaId, idHero);
            if (hero == default)
            {
                return "Achei um bug e não consigo mostrar as estatísticas do hero";
            }

            var localizedName = await dotaService.GetNameAsync(idHero);
            var sb = WriteHeroMessage(hero, localizedName);

            return sb.ToString();
        }

        public static string WriteHeroMessage(DotaApiHeroResponse hero, string localizedName)
        {
            var channelName = Environment.GetEnvironmentVariable("CHANNEL_NAME");
            var sb = new StringBuilder();
            sb.Append($"{channelName} ");
            if (hero.Games == 0)
            {
                sb.Append($"nunca jogou com {localizedName}. ");
            }
            else
            {
                sb.Append($"já jogou com {localizedName} ");
                if (hero.Games == 1)
                {
                    sb.Append($"uma única vez ");
                    if (hero.Win == 0)
                    {
                        sb.Append($"e perdeu. ");
                    }
                    else
                    {
                        sb.Append($"e ganhou. ");
                    }
                }
                else
                {
                    sb.Append($"{hero.Games} vezes ");
                    if (hero.Win == 0)
                    {
                        sb.Append($"e perdeu todas. ");
                    }
                    else if (hero.Win == 1)
                    {
                        sb.Append($"com apenas uma vitória ({hero.WinRate:P1}). ");
                    }
                    else
                    {
                        sb.Append($"com {hero.Win} vitórias ({hero.WinRate:P1}). ");
                    }
                }
            }

            sb.Append("Como aliado, ");
            if (hero.WithGames == 0)
            {
                sb.Append($"nunca jogou com {localizedName}. ");
            }
            else
            {
                if (hero.WithGames == 1)
                {
                    sb.Append($"foi um único jogo ");
                    if (hero.WithWin == 0)
                    {
                        sb.Append("com derrota. ");
                    }
                    else
                    {
                        sb.Append("com vitória. ");
                    }
                }
                else
                {
                    sb.Append($"foram {hero.WithGames} jogos com {hero.WithWin} vitória{(hero.WithWin == 1 ? "" : "s")} ({hero.WithWinRate:P1}). ");
                }
            }

            sb.Append("Como oponente, ");
            if (hero.AgainstGames == 0)
            {
                sb.Append($"nunca jogou contra {localizedName}. ");
            }
            else
            {
                if (hero.AgainstGames == 1)
                {
                    sb.Append($"foi um único jogo ");
                    if (hero.AgainstWin == 0)
                    {
                        sb.Append("com derrota. ");
                    }
                    else
                    {
                        sb.Append("com vitória. ");
                    }
                }
                else
                {
                    sb.Append($"foram {hero.AgainstGames} jogos com {hero.AgainstWin} vitória{(hero.AgainstWin == 1 ? "" : "s")} ({hero.AgainstWinRate:P1}). ");
                }
            }

            if (hero.Games != 0)
            {
                var lastPlayed = DateTimeOffset.FromUnixTimeSeconds(hero.LastPlayed);
                var lastPlayedBrasilia = TimeZoneInfo.ConvertTimeFromUtc(lastPlayed.DateTime, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
                sb.Append($"O último jogo com o heroi foi em {lastPlayedBrasilia:dd/MM/yyyy à\\s HH:mm:ss}.");
            }

            return sb.ToString();
        }
    }
}