Feature: Video submission validation

  Scenario Outline: Submitting invalid video data should return validation errors
    Given I have the following invalid video data:
      | IdUsuario   | EmailUsuario | NomeVideo   | Extensao | Tamanho | Duracao |
      | <IdUsuario> | <Email>      | <Nome>      | <Ext>    | <Tam>   | <Dur>   |
    When I POST this data to "/api/videos"
    Then the response status code should be 400
    And the response should contain the error "<MensagemErroEsperada>"

    Examples:
      | IdUsuario                            | Email           | Nome         | Ext    | Tam | Dur | MensagemErroEsperada                                               |      
      #| 00000000-0000-0000-0000-000000000001 | not-an-email    | MeuVideo     | mp4   | 100 | 60  | O EmailUsuario deve ser um endereço de e-mail válido.               |
      | 00000000-0000-0000-0000-000000000001 | user@email.com  |              | mp4   | 100 | 60  | O NomeVideo é obrigatório.                                          |
      | 00000000-0000-0000-0000-000000000001 | user@email.com  | MeuVideo     | exe   | 100 | 60  | A Extensão deve ser um dos formatos suportados: mp4, avi, mov, mkv  |
      | 00000000-0000-0000-0000-000000000001 | user@email.com  | MeuVideo     | mp4   | 0   | 60  | O Tamanho do vídeo deve ser maior que zero.                         |
      | 00000000-0000-0000-0000-000000000001 | user@email.com  | MeuVideo     | mp4   | 100 | 0   | A Duração do vídeo deve ser maior que zero.                         |