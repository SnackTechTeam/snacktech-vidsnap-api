Feature: Video listing by user

  Scenario: Listing videos for a user who has registered videos should return them successfully
    Given the following videos have been registered:
      | IdUsuario                            | EmailUsuario       | NomeVideo     | Extensao | Tamanho | Duracao |
      | 00000000-0000-0000-0000-000000000002 | user1@email.com    | Video A       | mp4      | 100     | 60      |
      | 00000000-0000-0000-0000-000000000002 | user1@email.com    | Video B       | avi      | 120     | 90      |
    When I GET videos for user with ID "00000000-0000-0000-0000-000000000002"
    Then the response status code should be 200
    And the response should contain 2 videos
    And one of the videos should be named "Video A"
    And one of the videos should be named "Video B"