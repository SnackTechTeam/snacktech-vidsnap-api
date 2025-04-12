Feature: Video Registration Success

  Scenario: Submitting a valid video should return 200 and a confirmation
    Given I have the following valid video data:
      | IdUsuario                            | EmailUsuario         | NomeVideo | Extensao | Tamanho | Duracao |
      | 00000000-0000-0000-0000-000000000001 | user@example.com     | MeuVideo  | mp4      | 100     | 60      |
    When I POST this data to "api/videos"
    Then the response status code should be 200
    And the response should contain a valid video registration confirmation