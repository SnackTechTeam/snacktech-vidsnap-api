# snacktech-videosnap-api

## 🛠️ Sobre o Projeto

Este projeto tem por objetivo receber cadastros de vídeos que devem ser armazenados e posteriormente utilizados para extração de imagens a cada 20 minutos. 
As imagens geradas são colocadas juntas em um arquivo zip e este fica disponível para download pelo usuário dono do cadastro.
A API reponsável por estes cadastros, também será responsável por disponibilizar para cada usuário uma lista com todos os seus vídeos cadastrados, mostrando o status de processamento de cada um, assim como uma URL para download do vídeo após o processamento.
O usuário não terá limite de quantidade de vídeos que poderá cadastrar, podendo mais de um ser processado ao mesmo tempo.

## 💻 Tecnologias utilizadas

- **C#**: Linguagem de programação usada no desenvolvimento do projeto
- **.NET 8**: Framework como base em que a API é executada
- **Sql Server**: Base de dados para armazenar os dados trabalhados pela API
- **S3 Bucket**: Contêiner para arquivos de vídeo e zip
- **SQS**: Orquestrador de mensagens
- **Swagger**: Facilita a documentação da API
- **Docker**: Permite criar uma imagem do serviço e rodá-la em forma de contâiner
- **Terraform**: Para provisionamento de infra

## Como Utilizar

## 🛡️ Pré-requisitos

Antes de rodar o projeto Vidsnap, certifique-se de que você possui os seguintes pré-requisitos:

- **.NET SDK**: O projeto foi desenvolvido com o .NET SDK 8. Instale a versão necessária para garantir a compatibilidade com o código.
- **Docker**: O projeto utiliza Docker para contêinerizar a aplicação e o banco de dados. Instale o Docker Desktop para Windows ou Mac, ou configure o Docker Engine para Linux.
- **Sql Server (Opcional)**: O projeto tem um arquivo de docker-compose que configura e gerencia uma instância do Sql Server dentro de um container Docker. Sendo assim, a instalação ou uso de uma solução em nuvem é opcional.

## 💡 Instalação e Execução Local

Com seu acesso a sua conta AWS configurado, vá até a pastas /infra/terraform e execute os seguintes comandos em seu terminal:
```
terraform init
```
Aguarde finalizar a execução..
```
terraform plan
```
Aguarde...
```
terraform apply
```
Agora o próximo passo é executar o docker compose. Em seu console navegue até o diretório /src e execute o seguinte comando:
```
docker compose up -d
```
Dessa forma seus containers iniciarão em background.

## Equipe

* Adriano de Melo Costa. Email: adriano.dmcosta@gmail.com
* Rafael Duarte Gervásio da Silva. Email: rafael.dgs.1993@gmail.com
* Guilherme Felipe de Souza. Email: gui240799@outlook.com
* Dayvid Ribeiro Correia. Email: dayvidrc@gmail.com
