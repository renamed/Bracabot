# Bracabot

O Bracabot é um bot para a plataforma da [Twitch](https://www.twitch.tv/bracubi) que eu fiz para o meu amigo [Bracubi](https://www.twitch.tv/bracubi), que faz lives por lá.

O intuito do bot é mostrar informações sobre o jogo de Dota 2.

## Requisitos

Este projeto utiliza o framework [Dotnet na versão 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0). Para rodá-lo, é necessário criar um arquivo `params.env` na raiz do projeto, com as seguintes informações

```
DOTA_ID=?
CHANNEL_NAME=?
IP_TWITCH_IRC=?
PORT_TWITCH_IRC=?
USERNAME_TWITCH_IRC=?
PASSWORD_TWITCH_IRC=?
CLIENT_ID_TWITCH=?
AUTHORIZATION_TWITCH=?
```

- *Dota ID* é literalmente o seu ID no jogo de Dota 2, essa informação pode ser encontrada no seu perfil dentro do jogo.
- *CHANNEL_NAME* é o canal em que o bot vai ler mensagens e respondê-las.
- *IP_TWITCH_IRC* é o endereço DNS do servidor IRC da Twitch. Mais informações [neste link](https://dev.twitch.tv/docs/irc#connecting-to-the-twitch-irc-server).
- *PORT_TWITCH_IRC* Porta para o endereço DNS usado acima. Utilize esse [link](https://dev.twitch.tv/docs/irc#connecting-to-the-twitch-irc-server).
- *USERNAME_TWITCH_IRC* o nome de usuário na Twitch para ser usado como bot. Pessoalmente não utilizar sua conta pessoal.
- *PASSWORD_TWITCH_IRC* o token a ser utilizado para conectar ao servidor IRC da Twitch. Gere o token [nesta página](https://twitchapps.com/tmi/).
- *CLIENT_ID_TWITCH* NÃO É USADO ATUALMENTE. PODE SER OMITIDO[.](https://dev.twitch.tv/console).
- *AUTHORIZATION_TWITCH* NÃO É USADO ATUALMENTE. PODE SER OMITIDO.