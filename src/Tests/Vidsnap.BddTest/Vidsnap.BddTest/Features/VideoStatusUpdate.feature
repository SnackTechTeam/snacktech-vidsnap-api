Feature: Video status update

  Scenario: Update video status after create it
    Given I have the following valid video data:
      | IdUsuario                            | EmailUsuario       | NomeVideo | Extensao | Tamanho | Duracao |
      | 00000000-0000-0000-0000-000000000003 | usuario@email.com  | Teste.mp4 | mp4      | 200     | 120     |
    And I register this video
    When I receive a message with status "Processando"
    And I successfuly process the message
    And I GET the video by name "Teste.mp4"
    Then the current status should be "Processando"