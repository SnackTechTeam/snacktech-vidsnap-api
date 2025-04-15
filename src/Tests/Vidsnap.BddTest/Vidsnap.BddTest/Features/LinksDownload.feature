Feature: Video download links retrieval

  Scenario: Retrieve download links of a registered video
    Given I have the following valid video data:
      | IdUsuario                            | EmailUsuario       | NomeVideo | Extensao | Tamanho | Duracao |
      | 00000000-0000-0000-0000-000000000123 | usuario@email.com  | Teste.mp4 | mp4      | 200     | 120     |
    And I register this video
    And I update the video status to "FinalizadoComSucesso"
    When I GET the download links for this video
    Then the response status code should be 200
    And the response should contain a download link with zip and image URLs