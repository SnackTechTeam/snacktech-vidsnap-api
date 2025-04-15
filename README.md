# snacktech-vidsnap-api

📹 Vidsnap API

API responsável por gerenciar o cadastro, atualizar status de processamento e disponibilizar arquivos de imagem e zip para usuários. Os vídeos são enviados para processamento, e ao final são disponibilizados arquivos para download via links pré-assinados do S3.

## 💻 Tecnologias utilizadas

- **C#**: Linguagem de programação usada no desenvolvimento do projeto
- **.NET 8**: Framework como base em que a API é executada
- **Sql Server**: Base de dados para armazenar os dados trabalhados pela API
- **Amazon S3 Bucket**: Contêiner para arquivos de vídeo e zip
- **Amazon SQS**: Orquestrador de mensagens
- **BackgroundService**: Worker para leitura da fila SQS
- **Swagger**: Facilita a documentação da API
- **Docker**: Permite criar uma imagem do serviço e rodá-la em forma de contâiner
- **Terraform**: Para provisionamento de infra

---

## 🚀 Endpoints da API

### 🔹 `POST /videos`

**Cadastra um novo vídeo para processamento.**

```http
POST /videos
```

**Headers obrigatórios:** 
| Header | Tipo | Descrição | 
| --- | --- | --- | 
| X-User-Id | `Guid` | ID do usuário | 
| X-User-Email | `string` | Email do usuário |

**Body:**

```json
{
  "nomeVideo": "exemplo.mp4",
  "extensao": ".mp4",
  "tamanho": 12345678,
  "duracao": 120
}
```

**Resposta (200 OK):**

```json
{
  "id": "guid",
  "idUsuario": "guid",
  "nome": "exemplo.mp4",
  "extensao": ".mp4",
  "tamanho": 12345678,
  "dataInclusao": "2025-04-15 08:00:00",
  "statusAtual": "Recebido",
  "urlPreAssinadaDeUpload": "https://s3.amazonaws.com/bucket/video.mp4?signature=..."  
}
```

### 🔹 `GET /videos`

**Lista todos os vídeos cadastrados pelo usuário.**

```http
GET /videos
```

**Headers obrigatórios:** 
| Header | Tipo | Descrição | 
| --- | --- | --- | 
| X-User-Id | `Guid` | ID do usuário |

**Resposta (200 OK):**

```json
[
  {
    "id": "b1a7d6e2-35b0-4f4b-90d1-123456789abc",
    "idUsuario": "e9c3efab-7ad4-4ae4-9311-abcdef123456",
    "nome": "meu-video.mp4",
    "extensao": ".mp4",
    "tamanho": 10485760,
    "duracao": 180,
    "dataInclusao": "2025-04-15T14:30:00Z",
    "statusAtual": "FinalizadoComSucesso",
    "statusHistory": [
      {
        "status": "Recebido",
        "dataInclusao": "2025-04-15T14:30:00Z"
      },
      {
        "status": "Processando",
        "dataInclusao": "2025-04-15T14:35:00Z"
      },
      {
        "status": "FinalizadoComSucesso",
        "dataInclusao": "2025-04-15T14:40:00Z"
      }
    ]
  },
  {
    "id": "a2b3c4d5-6789-4321-aaaa-bbbbccccdddd",
    "idUsuario": "e9c3efab-7ad4-4ae4-9311-abcdef123456",
    "nome": "video-aula01.mov",
    "extensao": ".mov",
    "tamanho": 5242880,
    "duracao": 120,
    "dataInclusao": "2025-04-14T10:00:00Z",
    "statusAtual": "Processando",
    "statusHistory": [
      {
        "status": "Recebido",
        "dataInclusao": "2025-04-14T10:00:00Z"
      },
      {
        "status": "Processando",
        "dataInclusao": "2025-04-14T10:05:00Z"
      }
    ]
  }
]
```

### 🔹 `GET /videos/{id-video}`

**Gera URLs de download para os arquivos processados de um vídeo.**

Apenas disponível para vídeos com status FinalizadoComSucesso.

```http
GET /videos/{id-video}
```

**Headers obrigatórios:** 
| Header | Tipo | Descrição | 
| --- | --- | --- | 
| X-User-Id | `Guid` | ID do usuário |

**Resposta (200 OK):**

```json
{
    "idVideo": "a2b3c4d5-6789-4321-aaaa-bbbbccccdddd",
    "urlZip": "https://s3.amazonaws.com/bucket/video.zip?signature=...",
    "urlImage": "https://s3.amazonaws.com/bucket/video.png?signature=...",
    "dataExpiracao": "2025-04-14T10:00:00Z"
}
```

## ⚙️ Arquitetura e Infraestrutura

### 🔄 Fluxo Assíncrono com SQS

1. Usuário cadastra vídeo via POST /videos usando uma aplicação client.
2. A API salva os dados no banco e gera URL pré assinada de upload do S3.
3. A aplicação client faz upload direto para o S3.
4. O S3 dispara uma mensagem para via SQS para o worker de processamento de vídeo. Mais detalhes desse worker aqui.
5. O worker de processamento de vídeo envia mesangens via SQS para o worker de atualização de status.
  "Processando" - Indica que o processamento do vídeo está ocorrendo
  "FinalizadoComSucesso" - Indica que o processamento foi finalizado com sucesso e as URLs dos arquivos gerados chegam na mensagem
  "FinalizadoComErro" - Indica que o processamento foi finalizado com erro e o usuário precisará tentar novamente.
6. O worker de atualização de status recebe as mensagens e atua de acordo com cada status.
    "Processando" - Apenas atualiza o status no banco de dados.
    "FinalizadoComSucesso" - Atualiza o status no banco de dados juntamente com as urls da imagem e do zip gerados e enviados via mensagem. Envia mensagem para outra fila SQS para notificação por e-mail do usuário.
    "FinalizadoComErro" - Atualiza o status no banco de dados e envia mensagem para outra fila SQS para notificação por e-mail do usuário.
7. Usuário consulta lista apenas de seus vídeos cadastrados e vê seus status via GET /videos.
8. Usuário seleciona um video com status FinalizadoComSucesso e recebe via GET /videos/{video-id} as URLs pré assinada de download dos arquivos de imagem principal e de zip com todas as imagens geradas.

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
