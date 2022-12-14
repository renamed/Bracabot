# Bracabot

O Bracabot é um bot para a plataforma da [Twitch](https://www.twitch.tv/bracubi) que eu fiz para o meu amigo [Bracubi](https://www.twitch.tv/bracubi), que faz lives por lá.

O intuito do bot é mostrar informações sobre o jogo de Dota 2.

## Requisitos

Este projeto utiliza o framework [Dotnet na versão 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0). Para rodá-lo, é necessário criar um arquivo `params.env` na raiz do projeto, com as seguintes informações

```
USERNAME_TWITCH_IRC=?
PASSWORD_TWITCH_IRC=?
CLIENT_ID_TWITCH=?
AUTHORIZATION_TWITCH=?
POSTGRES_CONN_STR=?
```
- *USERNAME_TWITCH_IRC* o nome de usuário na Twitch para ser usado como bot. Pessoalmente não utilizar sua conta pessoal.
- *PASSWORD_TWITCH_IRC* o token a ser utilizado para conectar ao servidor IRC da Twitch. Gere o token [nesta página](https://twitchapps.com/tmi/).
- *CLIENT_ID_TWITCH* utilizamos a API da Twitch para obter o jogo atual do streamer, por exemplo. Gere um novo app utilizando sua conta do bot e obtenha o client_id seguindo [este link](https://dev.twitch.tv/console).
- *AUTHORIZATION_TWITCH* utilizamos a API da Twitch para obter o jogo atual do streamer, por exemplo. Gere um novo app utilizando sua conta do bot e obtenha o secret seguindo [este link](https://dev.twitch.tv/console).
- *POSTGRES_CONN_STR* utilizamos um banco de dados Postgres para salvar dados dos jogos, já que a API do Dota retorna apenas os últimos 20 jogados. Aqui coloque a string de conexão ao banco.

Além disso, por favor verifique o arquivo `appsettings.json` com as seguintes configurações:

- *DotaId* é literalmente o seu ID no jogo de Dota 2, essa informação pode ser encontrada no seu perfil dentro do jogo.
- *ChannelName* é o canal em que o bot vai ler mensagens e respondê-las.
- *TwitchBroadcastId* é o ID (e não o nome de usuário) do canal em que o bot vai ler e responder mensagens. Há alguns sites que podem ajudá-lo a obter essa informação, por exemplo, [este](https://www.streamweasels.com/tools/convert-twitch-username-to-user-id/).
- *IpTwitchIrc* é o endereço DNS do servidor IRC da Twitch. Mais informações [neste link](https://dev.twitch.tv/docs/irc#connecting-to-the-twitch-irc-server). Não utilize o protocolo.
- *PortTwitchIrc* Porta para o endereço DNS usado acima. Utilize esse [link](https://dev.twitch.tv/docs/irc#connecting-to-the-twitch-irc-server).
- Os demais dados do arquivo provavelmente não precisam ser trocados.