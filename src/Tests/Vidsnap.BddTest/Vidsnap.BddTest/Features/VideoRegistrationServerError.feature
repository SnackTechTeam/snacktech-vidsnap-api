Feature: Video registration failure

  Scenario: Unexpected error during video registration returns 500
    Given I have the following valid video data:
      | IdUsuario                            | EmailUsuario      | NomeVideo | Extensao | Tamanho | Duracao |
      | 00000000-0000-0000-0000-000000000001 | user@email.com    | MeuVideo  | mp4      | 100     | 60      |
    And I want to simulate a Internal Server Error
    When I POST this data to "/api/videos"
    Then the response status code should be 500